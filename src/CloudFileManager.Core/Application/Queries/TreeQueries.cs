using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Domain.Visiting;
using CloudFileManager.Core.Application.Traversal;

namespace CloudFileManager.Core.Application.Queries;

public static class TreeQueries
{
    public static long CalculateTotalSize(DirectoryNode root, TextWriter? log = null)
    {
        ArgumentNullException.ThrowIfNull(root);
        var visitor = new SizeVisitor();
        FileSystemTraversal.Visit(root, visitor, log);
        return visitor.TotalBytes;
    }
    public static IReadOnlyList<string> SearchByExtension(DirectoryNode root, string extension, TextWriter? log = null)
    {
        ArgumentNullException.ThrowIfNull(root);
        var visitor = new ExtensionSearchVisitor(extension);
        FileSystemTraversal.Visit(root, visitor, log);
        return visitor.Paths;
    }
}
