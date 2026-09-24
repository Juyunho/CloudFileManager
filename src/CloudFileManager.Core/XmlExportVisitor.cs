using System.Text;
using System.Xml;
namespace CloudFileManager.Core;

// Stateful per-operation writer; ownership and traversal remain outside the visitor.
public sealed class XmlExportVisitor : IFileSystemVisitor, IDisposable
{
    private readonly StringBuilder output = new();
    private readonly XmlWriter writer;
    private readonly Stack<(DirectoryNode Node, HashSet<string> Names)> parents = new();
    private bool started, completed;
    public XmlExportVisitor() => writer = XmlWriter.Create(output, new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true });
    public void Visit(DirectoryNode node) { Start(node); parents.Push((node, new(StringComparer.Ordinal))); }
    public void Visit(WordFile node) => File(node);
    public void Visit(ImageFile node) => File(node);
    public void Visit(TextFile node) => File(node);
    private void Start(FsNode node)
    {
        if (completed) throw new InvalidOperationException("Visitor already completed.");
        while (parents.TryPeek(out var parent) && !ReferenceEquals(parent.Node, node.Parent))
        { writer.WriteEndElement(); parents.Pop(); }
        if (started && parents.Count == 0) throw new InvalidOperationException("One subtree per visitor.");
        var basis = XmlConvert.EncodeLocalName(node is DirectoryNode { XmlAlias: not null } d ? d.XmlAlias : node.Name.Replace('.', '_'));
        var name = basis; var suffix = 2;
        if (parents.TryPeek(out var context)) while (!context.Names.Add(name)) name = basis + "__" + suffix++;
        writer.WriteStartElement(name); started = true;
    }
    private void File(FileNode node) { Start(node); writer.WriteString(node.Details); writer.WriteEndElement(); }
    public string Complete()
    {
        if (!completed) { while (parents.Count > 0) { writer.WriteEndElement(); parents.Pop(); } writer.Flush(); completed = true; }
        return output.ToString();
    }
    public void Dispose() => writer.Dispose();
}
