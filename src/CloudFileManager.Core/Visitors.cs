namespace CloudFileManager.Core;

// Operations are separate from node ownership. Accept dispatches only one node.
public interface IFileSystemVisitor
{
    void Visit(DirectoryNode node);
    void Visit(WordFile node);
    void Visit(ImageFile node);
    void Visit(TextFile node);
}

public static class FileSystemTraversal
{
    // Visitors must not mutate the tree during traversal. Null log means silent.
    public static void Visit(FsNode root, IFileSystemVisitor visitor, TextWriter? log = null, TraversalProgressSource? progress = null)
    {
        ArgumentNullException.ThrowIfNull(root); ArgumentNullException.ThrowIfNull(visitor);
        var total = 0; var visited = 0;
        if (progress is not null)
        {
            var count = new Stack<FsNode>(); count.Push(root);
            while (count.TryPop(out var item)) { total++; foreach (var child in item.Children) count.Push(child); }
        }
        var stack = new Stack<FsNode>(); stack.Push(root);
        while (stack.TryPop(out var node))
        {
            log?.WriteLine($"Visiting: {node.FullPath}");
            node.Accept(visitor);
            progress?.Publish(new TraversalProgress(node.Id, node.Name, node.FullPath, ++visited, total));
            for (var i = node.Children.Count - 1; i >= 0; i--) stack.Push(node.Children[i]);
        }
    }
}

// One accumulator per operation; callers should create a fresh visitor for each traversal.
public sealed class SizeVisitor : IFileSystemVisitor
{
    public long TotalBytes { get; private set; }
    public void Visit(DirectoryNode node) { }
    public void Visit(WordFile node) => Add(node);
    public void Visit(ImageFile node) => Add(node);
    public void Visit(TextFile node) => Add(node);
    private void Add(FileNode node) => TotalBytes = checked(TotalBytes + node.SizeBytes);
}

public sealed class ExtensionSearchVisitor : IFileSystemVisitor
{
    private readonly string extension;
    private readonly List<string> paths = [];
    private readonly List<Guid> matchedNodeIds = [];
    public ExtensionSearchVisitor(string extension)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(extension);
        extension = extension.Trim();
        if (!extension.StartsWith('.')) extension = "." + extension;
        if (extension.Length == 1 || extension[1..].Any(c => c is '.' or '/' or '\\' or '*' or '?' || char.IsWhiteSpace(c)))
            throw new ArgumentException("Supply a single extension, for example .docx.", nameof(extension));
        this.extension = extension;
    }
    public IReadOnlyList<string> Paths => paths.AsReadOnly();
    public IReadOnlyList<Guid> MatchedNodeIds => matchedNodeIds.AsReadOnly();
    public void Visit(DirectoryNode node) { }
    public void Visit(WordFile node) => Match(node);
    public void Visit(ImageFile node) => Match(node);
    public void Visit(TextFile node) => Match(node);
    private void Match(FileNode node)
    {
        if (string.Equals(node.Extension, extension, StringComparison.OrdinalIgnoreCase))
        { paths.Add(node.FullPath); matchedNodeIds.Add(node.Id); }
    }
}

public sealed record TraversalProgress(Guid NodeId, string Name, string Path, int Visited, int Total);
public sealed class TraversalProgressSource
{
    public event Action<TraversalProgress>? Progressed;
    internal void Publish(TraversalProgress progress) => Progressed?.Invoke(progress);
}
