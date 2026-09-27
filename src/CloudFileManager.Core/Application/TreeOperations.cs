using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Application.Queries;
using CloudFileManager.Core.Application.Rendering;
using CloudFileManager.Core.Application.Export;
using CloudFileManager.Core.Application.Traversal;

namespace CloudFileManager.Core.Application;

// Compatibility entry points: preserve the Console default log; algorithms live in focused helpers.
public static class TreeOperations
{
    public static long CalculateTotalSize(DirectoryNode root, TextWriter? log = null)
        => TreeQueries.CalculateTotalSize(root, log ?? Console.Out);
    public static IReadOnlyList<string> SearchByExtension(DirectoryNode root, string extension, TextWriter? log = null)
        => TreeQueries.SearchByExtension(root, extension, log ?? Console.Out);
    public static string Render(DirectoryNode root) => TreeTextRenderer.Render(root);
    public static string ToXml(DirectoryNode root) => SerializeXml(root);
    public static string SerializeXml(FsNode root, TraversalProgressSource? progress = null)
        => TreeXmlExporter.SerializeXml(root, progress);
}
