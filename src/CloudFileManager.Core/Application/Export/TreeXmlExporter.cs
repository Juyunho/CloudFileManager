using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Application.Traversal;

namespace CloudFileManager.Core.Application.Export;

public static class TreeXmlExporter
{
    public static string SerializeXml(FsNode root, TraversalProgressSource? progress = null)
    {
        ArgumentNullException.ThrowIfNull(root);
        using var visitor = new XmlExportVisitor();
        FileSystemTraversal.Visit(root, visitor, progress: progress);
        return visitor.Complete();
    }
}
