using System.Text;
using System.Xml;

namespace CloudFileManager.Core;

public static class TreeOperations
{
    private static IEnumerable<FsNode> Walk(FsNode root)
    {
        var stack = new Stack<FsNode>(); stack.Push(root);
        while (stack.TryPop(out var node))
        {
            yield return node;
            for (var i = node.Children.Count - 1; i >= 0; i--) stack.Push(node.Children[i]);
        }
    }
    public static long CalculateTotalSize(DirectoryNode root, TextWriter? log = null)
    {
        ArgumentNullException.ThrowIfNull(root); log ??= Console.Out;
        long total = 0;
        foreach (var node in Walk(root))
        {
            log.WriteLine($"Visiting: {node.FullPath}");
            if (node is FileNode file) total = checked(total + file.SizeBytes);
        }
        return total;
    }
    public static IReadOnlyList<string> SearchByExtension(DirectoryNode root, string extension, TextWriter? log = null)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentException.ThrowIfNullOrWhiteSpace(extension);
        extension = extension.Trim();
        if (!extension.StartsWith('.')) extension = "." + extension;
        if (extension.Length == 1 || extension[1..].Any(c => c is '.' or '/' or '\\' or '*' or '?' || char.IsWhiteSpace(c)))
            throw new ArgumentException("Supply a single extension, for example .docx.", nameof(extension));
        log ??= Console.Out;
        var result = new List<string>();
        foreach (var node in Walk(root))
        {
            log.WriteLine($"Visiting: {node.FullPath}");
            if (node is FileNode file && string.Equals(file.Extension, extension, StringComparison.OrdinalIgnoreCase))
                result.Add(file.FullPath);
        }
        return result.AsReadOnly();
    }
    public static string Render(DirectoryNode root)
    {
        ArgumentNullException.ThrowIfNull(root);
        var output = new StringBuilder();
        var stack = new Stack<(FsNode Node, string Prefix, bool Last, bool Root)>();
        stack.Push((root, "", true, true));
        while (stack.TryPop(out var item))
        {
            var (node, prefix, last, isRoot) = item;
            var type = node switch { DirectoryNode => "目錄", WordFile => "Word 檔案", ImageFile => "圖片", TextFile => "純文字檔", _ => "節點" };
            output.Append(prefix).Append(isRoot ? "" : last ? "└── " : "├── ").Append(node.Name)
                .Append(" [").Append(type).Append("] (").Append(node.Details)
                .Append(", 建立: ").Append(node.CreatedAt.ToString("O")).AppendLine(")");
            var childPrefix = prefix + (isRoot ? "" : last ? "    " : "│   ");
            for (var i = node.Children.Count - 1; i >= 0; i--)
                stack.Push((node.Children[i], childPrefix, i == node.Children.Count - 1, false));
        }
        return output.ToString();
    }
    public static string ToXml(DirectoryNode root)
    {
        ArgumentNullException.ThrowIfNull(root);
        var output = new StringBuilder();
        using (var writer = XmlWriter.Create(output, new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true }))
        {
            var stack = new Stack<(FsNode Node, string Name, bool Close)>();
            stack.Push((root, XmlName(root), false));
            while (stack.TryPop(out var item))
            {
                if (item.Close) { writer.WriteEndElement(); continue; }
                writer.WriteStartElement(item.Name);
                if (item.Node is FileNode) { writer.WriteString(item.Node.Details); writer.WriteEndElement(); continue; }
                stack.Push((item.Node, item.Name, true));
                var used = new HashSet<string>(StringComparer.Ordinal);
                var children = new List<(FsNode Node, string Name, bool Close)>();
                foreach (var child in item.Node.Children)
                {
                    var basis = XmlName(child); var name = basis; var suffix = 2;
                    while (!used.Add(name)) name = basis + "__" + suffix++;
                    children.Add((child, name, false));
                }
                for (var i = children.Count - 1; i >= 0; i--) stack.Push(children[i]);
            }
        }
        return output.ToString();
    }
    private static string XmlName(FsNode node)
        => XmlConvert.EncodeLocalName(node is DirectoryNode { XmlAlias: not null } dir ? dir.XmlAlias : node.Name.Replace('.', '_'));
}
