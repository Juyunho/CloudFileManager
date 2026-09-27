using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Domain.Visiting;

namespace CloudFileManager.Core.Application.Traversal;

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
