using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Domain.Values;
namespace CloudFileManager.Core.Application.Persistence;

public static class FileSystemDocumentMapper
{
    public static FileSystemDocument Capture(DirectoryNode root)
    {
        ArgumentNullException.ThrowIfNull(root);
        if (root.Parent is not null) throw new ArgumentException("Root required.");
        var rows = new List<FileSystemRow>();
        var pending = new Stack<(FsNode Node, int Ordinal)>(); pending.Push((root, 0));
        while (pending.TryPop(out var item))
        {
            var n = item.Node;
            rows.Add(new(n.Id, n.Parent?.Id, item.Ordinal, n switch {
                DirectoryNode => "Directory", WordFile => "Word", ImageFile => "Image", TextFile => "Text",
                _ => throw new ArgumentException("Unsupported node type.") }, n.Name, n.CreatedAt,
                (n as DirectoryNode)?.XmlAlias, (n as FileNode)?.SizeBytes, (n as WordFile)?.Pages,
                (n as ImageFile)?.Width, (n as ImageFile)?.Height, (n as TextFile)?.Encoding, n.Tags));
            for (int i = n.Children.Count - 1; i >= 0; i--) pending.Push((n.Children[i], i));
        }
        return new(rows);
    }

    public static DirectoryNode Restore(FileSystemDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        var rows = document.Rows;
        if (rows.Count == 0 || rows.Select(r => r.Id).Distinct().Count() != rows.Count)
            throw new ArgumentException("A nonempty tree with unique IDs is required.");
        var roots = rows.Where(r => r.ParentId is null).ToArray();
        if (roots.Length != 1 || roots[0].Kind != "Directory" || roots[0].Ordinal != 0)
            throw new ArgumentException("Exactly one directory root is required.");
        var byId = rows.ToDictionary(r => r.Id);
        foreach (var row in rows)
        {
            if (row.Id == Guid.Empty || row.Ordinal < 0 || row.Tags.Any(t => !Enum.IsDefined(t)) || row.Tags.Distinct().Count() != row.Tags.Count)
                throw new ArgumentException("Invalid ID, ordering or Tags.");
            if (row.ParentId is { } parent && (!byId.TryGetValue(parent, out var p) || p.Kind != "Directory"))
                throw new ArgumentException("Invalid directory parent.");
        }
        var groups = rows.Where(r => r.ParentId is not null).GroupBy(r => r.ParentId!.Value)
            .ToDictionary(g => g.Key, g => g.OrderBy(r => r.Ordinal).ToArray());
        foreach (var group in groups.Values)
            if (!group.Select(r => r.Ordinal).SequenceEqual(Enumerable.Range(0, group.Length)) || group.Select(r => r.Name).Distinct(StringComparer.Ordinal).Count() != group.Length)
                throw new ArgumentException("Invalid sibling ordering or duplicate names.");
        DirectoryNode? root = null; var visited = new HashSet<Guid>();
        var pending = new Stack<(FileSystemRow Row, DirectoryNode? Parent)>(); pending.Push((roots[0], null));
        while (pending.TryPop(out var item))
        {
            var r = item.Row;
            if (!visited.Add(r.Id)) throw new ArgumentException("Cycle detected.");
            FsNode node = r.Kind switch {
                "Directory" when r.SizeBytes is null && r.Pages is null && r.Width is null && r.Height is null && r.Encoding is null
                    => new DirectoryNode(r.Name, r.CreatedAt, item.Parent, r.XmlAlias, r.Id),
                "Word" when item.Parent is not null && r.XmlAlias is null && r.SizeBytes is not null && r.Pages is not null && r.Width is null && r.Height is null && r.Encoding is null
                    => new WordFile(r.Name, r.SizeBytes.Value, r.Pages.Value, r.CreatedAt, item.Parent, r.Id),
                "Image" when item.Parent is not null && r.XmlAlias is null && r.SizeBytes is not null && r.Width is not null && r.Height is not null && r.Pages is null && r.Encoding is null
                    => new ImageFile(r.Name, r.SizeBytes.Value, r.Width.Value, r.Height.Value, r.CreatedAt, item.Parent, r.Id),
                "Text" when item.Parent is not null && r.XmlAlias is null && r.SizeBytes is not null && r.Encoding is not null && r.Pages is null && r.Width is null && r.Height is null
                    => new TextFile(r.Name, r.SizeBytes.Value, r.Encoding, r.CreatedAt, item.Parent, r.Id),
                _ => throw new ArgumentException("Invalid subtype metadata.")
            };
            node.LoadTags(r.Tags);
            if (item.Parent is null) root = (DirectoryNode)node;
            else item.Parent.Insert(r.Ordinal, node, notify: false);
            if (groups.TryGetValue(r.Id, out var children))
                for (int i = children.Length - 1; i >= 0; i--) pending.Push((children[i], (DirectoryNode)node));
        }
        if (visited.Count != rows.Count) throw new ArgumentException("Disconnected or cyclic tree.");
        return root!;
    }
}
