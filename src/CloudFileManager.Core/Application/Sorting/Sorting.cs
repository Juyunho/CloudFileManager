using CloudFileManager.Core.Domain.Nodes;

namespace CloudFileManager.Core.Application.Sorting;

public enum SortDirection { Asc, Desc }
public interface INodeSortStrategy
{
    IEnumerable<FsNode> Order(IReadOnlyList<FsNode> nodes, SortDirection direction);
}
internal static class StableSort
{
    internal static IEnumerable<FsNode> By<T>(IReadOnlyList<FsNode> nodes, Func<FsNode, T> key,
        SortDirection direction, IComparer<T>? comparer = null)
        => direction == SortDirection.Asc ? nodes.OrderBy(key, comparer) : nodes.OrderByDescending(key, comparer);
}
public sealed class NameSortStrategy : INodeSortStrategy
{
    public IEnumerable<FsNode> Order(IReadOnlyList<FsNode> nodes, SortDirection direction)
        => StableSort.By(nodes, n => n.Name, direction, StringComparer.OrdinalIgnoreCase);
}
public sealed class ExtensionSortStrategy : INodeSortStrategy
{
    public IEnumerable<FsNode> Order(IReadOnlyList<FsNode> nodes, SortDirection direction)
        => StableSort.By(nodes, n => n is FileNode f ? f.Extension : "", direction, StringComparer.OrdinalIgnoreCase);
}
public sealed class SizeSortStrategy : INodeSortStrategy
{
    public IEnumerable<FsNode> Order(IReadOnlyList<FsNode> nodes, SortDirection direction)
        => StableSort.By(nodes, Size, direction);
    public static long Size(FsNode root)
    {
        ArgumentNullException.ThrowIfNull(root);
        long total = 0;
        var pending = new Stack<FsNode>(); pending.Push(root);
        while (pending.TryPop(out var node))
        {
            if (node is FileNode file) total = checked(total + file.SizeBytes);
            foreach (var child in node.Children) pending.Push(child);
        }
        return total;
    }
}
public static class SortedView
{
    public static IReadOnlyList<FsNode> Children(DirectoryNode directory, INodeSortStrategy strategy, SortDirection direction)
    {
        ArgumentNullException.ThrowIfNull(directory); ArgumentNullException.ThrowIfNull(strategy);
        if (!Enum.IsDefined(direction)) throw new ArgumentOutOfRangeException(nameof(direction));
        var directories = directory.Children.Where(n => n is DirectoryNode).ToArray();
        var files = directory.Children.Where(n => n is FileNode).ToArray();
        return Array.AsReadOnly(strategy.Order(directories, direction).Concat(strategy.Order(files, direction)).ToArray());
    }
}

public sealed class TagSortStrategy : INodeSortStrategy
{
    private static int Key(FsNode node) => node.Tags.Min(t => (int?)t) ?? int.MaxValue;
    public IEnumerable<FsNode> Order(IReadOnlyList<FsNode> nodes, SortDirection direction)
    {
        var tagged = nodes.Where(n => n.Tags.Count != 0).ToArray();
        return StableSort.By(tagged, Key, direction).Concat(nodes.Where(n => n.Tags.Count == 0));
    }
}
