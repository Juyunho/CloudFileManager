using CloudFileManager.Core.Application.Formatting;
using CloudFileManager.Core.Application.Sessions;
using CloudFileManager.Core.Application.Sorting;
using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Domain.Values;

internal static class BonusDemo
{
    public static void Run(IFileSystemSession session)
    {
        var root = session.Root;
        var destination = (DirectoryNode)root.Children.Single(n => n.Name == "Bonus_Copies");
        var source = (DirectoryNode)root.Children[0];
        Console.WriteLine("=== TASK-002 Bonus: in-memory session ===");
        session.AddTag(source, TagKind.Work); session.AddTag(source.Children[0], TagKind.Urgent);
        session.AddTag(source.Children[0], TagKind.Personal);
        Show(root, session, "Tags");
        session.Copy(source);
        session.RemoveTag(source, TagKind.Work);
        var copy = session.Paste(destination);
        Show(root, session, "Copy-time snapshot / Paste (copy retains Work 藍)");
        try { session.Paste(destination); } catch (ArgumentException e) { Console.WriteLine("Expected conflict: " + e.Message); }
        Show(root, session, "Conflict leaves history unchanged");
        session.Delete(source); Show(root, session, "Delete subtree");
        session.Undo(); Show(root, session, "Undo Delete restores position and metadata");
        session.Redo(); Show(root, session, "Redo Delete");
        session.Undo();
        session.AddTag(copy, TagKind.Personal); Show(root, session, "New edit clears Redo");
        session.Undo(); Show(root, session, "Undo Tag");
        session.Redo(); Show(root, session, "Redo Tag");
        foreach (var (name, strategy) in new (string, INodeSortStrategy)[]
            { ("Name", new NameSortStrategy()), ("Size", new SizeSortStrategy()), ("Extension", new ExtensionSortStrategy()) })
            foreach (var direction in Enum.GetValues<SortDirection>())
            {
                Console.WriteLine($"=== Sort {name} {direction} (directories first) ===");
                foreach (var node in SortedView.Children(root, strategy, direction))
                    Console.WriteLine($"{node.GetType().Name} {node.Name} | {SizeSortStrategy.Size(node)} B");
            }
        Console.WriteLine("BONUS COMPLETE");
    }
    private static void Show(DirectoryNode root, IFileSystemSession session, string title)
    {
        Console.WriteLine($"=== {title} | Undo={session.UndoCount} Redo={session.RedoCount} ===");
        var pending = new Stack<FsNode>(); pending.Push(root);
        while (pending.TryPop(out var node))
        {
            Console.WriteLine($"{node.FullPath} | {NodeDetailsFormatter.Format(node)} | Tags: " +
                string.Join(", ", node.Tags.Select(t => $"{t} {TagCatalog.Color(t)}")));
            for (var i = node.Children.Count - 1; i >= 0; i--) pending.Push(node.Children[i]);
        }
    }
}
