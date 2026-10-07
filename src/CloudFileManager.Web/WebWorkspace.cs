using CloudFileManager.Core.Application.Persistence;
using CloudFileManager.Core.Application.Export;
using CloudFileManager.Core.Application.Sessions;
using CloudFileManager.Core.Application.Sorting;
using CloudFileManager.Core.Application.Traversal;
using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Domain.Values;
using CloudFileManager.Core.Domain.Visiting;
using System.Globalization;
namespace CloudFileManager.Web;

public sealed record ActionRequest(string Action, string? Id = null, string? Tag = null, string? Criterion = null, string? Extension = null);
public sealed record LogEntry(string Kind, string Text, string? Path = null);
public sealed class WebWorkspace
{
    // All access, including projection, is serialized here; Core is not thread-safe.
    public SemaphoreSlim Gate { get; } = new(1, 1);
    private readonly IFileSystemSession session;
    private Guid selected;
    private string criterion = "Size";
    private SortDirection direction = SortDirection.Asc;
    private readonly List<LogEntry> logs = [];
    private readonly HashSet<Guid> searchMatches = [];
    private string? searchSummary;
    private sealed record ProgressView(string status, string name, int visited, int total, string? path = null);
    private ProgressView progress = new("idle", "尚未執行", 0, 0);
    public WebWorkspace(IFileSystemSession session, Guid initialSelection)
    {
        ArgumentNullException.ThrowIfNull(session);
        if (!session.IsInitialized) throw new InvalidOperationException("Initialize the session before constructing a workspace.");
        this.session = session;
        if (!Nodes().Any(n => n.Id == initialSelection))
            throw new ArgumentException("Initial selection must belong to the live tree.", nameof(initialSelection));
        selected = initialSelection;
    }
    private List<FsNode> Nodes()
    {
        var result = new List<FsNode>(); var stack = new Stack<FsNode>(); stack.Push(session.Root);
        while (stack.TryPop(out var node)) { result.Add(node); for (var i = node.Children.Count - 1; i >= 0; i--) stack.Push(node.Children[i]); }
        return result;
    }
    private FsNode Selected() => Nodes().Single(n => n.Id == selected);
    private void Log(string kind, string text) => logs.Add(new(kind, text));
    private INodeSortStrategy Strategy() => criterion switch
    { "Name" => new NameSortStrategy(), "Size" => new SizeSortStrategy(), "Extension" => new ExtensionSortStrategy(), "Tag" => new TagSortStrategy(), _ => throw new ArgumentException("Unknown sorting criterion.") };
    public object State()
    {
        var nodes = Nodes(); var rows = new List<object>(); var pending = new Stack<(FsNode Node, int Depth)>(); pending.Push((session.Root, 0));
        var strategy = Strategy();
        while (pending.TryPop(out var item))
        {
            var node = item.Node;
            var kb = node is FileNode f ? (f.SizeBytes / 1024m).ToString("0.#", CultureInfo.InvariantCulture) : "0";
            var metadata = node switch { WordFile w => $", pages: {w.Pages}", ImageFile i => $", res: {i.Width}x{i.Height}", TextFile t => $", enc: {t.Encoding}", _ => "" };
            rows.Add(new { id = node.Id, name = node.Name, path = node.FullPath, depth = item.Depth, directory = node is DirectoryNode, kind = node.GetType().Name, details = kb + " KB" + metadata, sizeBytes = (node as FileNode)?.SizeBytes, searchMatch = searchMatches.Contains(node.Id), tags = node.Tags.Select(t => t.ToString()).ToArray() });
            if (node is DirectoryNode directory)
            {
                var children = SortedView.Children(directory, strategy, direction);
                for (var i = children.Count - 1; i >= 0; i--) pending.Push((children[i], item.Depth + 1));
            }
        }
        return new { rows, selected, criterion, direction = direction.ToString(), undoCount = session.UndoCount, redoCount = session.RedoCount,
            canPaste = session.HasClipboard && Selected() is DirectoryNode, canDelete = Selected().Parent is not null,
            counts = Enum.GetValues<TagKind>().ToDictionary(t => t.ToString(), t => nodes.Count(n => n.Tags.Contains(t))), logs = logs.ToArray(), progress, searchSummary };
    }
    public object Execute(ActionRequest request, Action<object> emit)
    {
        var node = Selected(); var ancestors = new List<Guid>();
        for (var parent = node.Parent; parent is not null; parent = parent.Parent) ancestors.Add(parent.Id);
        object? download = null;
        try
        {
            switch (request.Action)
            {
                case "select":
                    if (!Guid.TryParse(request.Id, out var id) || !Nodes().Any(n => n.Id == id)) throw new ArgumentException("Node no longer exists.");
                    selected = id; break;
                case "sort":
                    if (request.Criterion is not ("Name" or "Size" or "Extension" or "Tag")) throw new ArgumentException("Invalid sort.");
                    direction = criterion == request.Criterion ? (direction == SortDirection.Asc ? SortDirection.Desc : SortDirection.Asc) : SortDirection.Asc;
                    criterion = request.Criterion; Log("Strategy", $"執行 {criterion} {direction} 排序策略"); break;
                case "copy": session.Copy(node); Log("Prototype", $"複製快照：{node.FullPath}"); break;
                case "paste":
                    if (node is not DirectoryNode destination) throw new ArgumentException("請選取目錄作為貼上目的地。");
                    var copy = session.Paste(destination); Log("Prototype", $"建立獨立副本：{copy.FullPath}"); Log("Command", $"執行 貼上項目至 {destination.FullPath}"); break;
                case "delete": session.Delete(node); Log("Command", $"執行 刪除 {node.FullPath}"); break;
                case "undo": if (session.Undo()) Log("Undo", "復原上一個變更"); break;
                case "redo": if (session.Redo()) Log("Redo", "重做上一個變更"); break;
                case "addTag": case "removeTag":
                    if (!Enum.TryParse<TagKind>(request.Tag, out var tag) || !Enum.IsDefined(tag)) throw new ArgumentException("Invalid Tag.");
                    var target = request.Action == "addTag" ? node : Nodes().SingleOrDefault(n => n.Id.ToString() == request.Id) ?? throw new ArgumentException("Node no longer exists.");
                    var changed = request.Action == "addTag" ? session.AddTag(target, tag) : session.RemoveTag(target, tag);
                    if (changed) Log("Command", $"執行 {(request.Action == "addTag" ? "新增" : "移除")}標籤({tag})：{target.FullPath}");
                    break;
                case "size": case "search": case "xml":
                    if (request.Action == "search")
                    {
                        searchMatches.Clear(); searchSummary = null;
                        emit(new { type = "searchReset" });
                    }
                    IFileSystemVisitor visitor = request.Action switch { "size" => new SizeVisitor(), "search" => new ExtensionSearchVisitor(request.Extension ?? ""), _ => new XmlExportVisitor() };
                    var source = new TraversalProgressSource();
                    var nodeLookup = request.Action == "search" ? Nodes().ToDictionary(n => n.Id) : null;
                    void Trace(string kind, string text, string path)
                    {
                        var entry = new LogEntry(kind, text, path); logs.Add(entry);
                        emit(new { type = "log", entry });
                    }
                    void Observe(TraversalProgress item)
                    {
                        progress = new("running", item.Name, item.Visited, item.Total, item.Path);
                        emit(new { type = "progress", progress });
                        if (visitor is ExtensionSearchVisitor searching)
                        {
                            var current = nodeLookup![item.NodeId];
                            Trace("Trace", (current is DirectoryNode ? "搜尋目錄: " : "掃描檔案: ") + item.Name, item.Path);
                            if (searching.MatchedNodeIds.Count > 0 && searching.MatchedNodeIds[^1] == item.NodeId)
                            {
                                searchMatches.Add(item.NodeId);
                                Trace("Match", "[符合] " + item.Name, item.Path);
                                emit(new { type = "match", nodeId = item.NodeId });
                            }
                        }
                    }
                    source.Progressed += Observe;
                    try
                    {
                        FileSystemTraversal.Visit(node, visitor, progress: source);
                        if (visitor is SizeVisitor size) Log("Visitor", $"計算大小：{node.FullPath} = {size.TotalBytes} B ({(size.TotalBytes / 1024m).ToString("0.####", CultureInfo.InvariantCulture)} KB)");
                        if (visitor is ExtensionSearchVisitor search)
                        {
                            searchMatches.Clear(); searchMatches.UnionWith(search.MatchedNodeIds);
                            searchSummary = $"找到 {search.Paths.Count} 項";
                            Trace("SearchSummary", searchSummary, node.FullPath);
                        }
                        if (visitor is XmlExportVisitor xml)
                        {
                            var content = xml.Complete();
                            download = new { filename = "export.xml", content };
                            Log("Visitor", $"XML 匯出完成：{node.FullPath}");
                        }
                        // The terminal event retains actual counters; it does not invent a completed scan.
                        progress = progress with { status = "completed" };
                        emit(new { type = "progress", progress });

                    }
                    finally { source.Progressed -= Observe; (visitor as IDisposable)?.Dispose(); }
                    break;
                default: throw new ArgumentException("Unknown action.");
            }
            var live = Nodes();
            searchMatches.IntersectWith(live.Select(n => n.Id));
            if (!live.Any(n => n.Id == selected)) selected = ancestors.FirstOrDefault(id => live.Any(n => n.Id == id && n is DirectoryNode), session.Root.Id);
            return new { type = "result", state = State(), download };
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or OverflowException or PersistenceException)
        {
            Log("Error", ex.Message);
            return new { type = "result", error = ex.Message, state = State() };
        }
    }
}
