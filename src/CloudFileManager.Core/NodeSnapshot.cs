namespace CloudFileManager.Core;

// Value-only preorder rows: no source node or mutable source collection survives Copy.
internal sealed class NodeSnapshot
{
    private sealed record Row(int Parent, string Kind, string Name, DateTimeOffset Created,
        string? Alias, long Bytes, int Pages, int Width, int Height, string? Encoding, TagKind[] Tags);
    private readonly List<Row> rows;
    private NodeSnapshot(List<Row> rows) => this.rows = rows;
    internal string Name => rows[0].Name;
    internal static NodeSnapshot Capture(FsNode source)
    {
        var rows = new List<Row>(); var pending = new Stack<(FsNode Node, int Parent)>(); pending.Push((source, -1));
        while (pending.TryPop(out var item))
        {
            var n = item.Node; var index = rows.Count;
            rows.Add(new Row(item.Parent, n.GetType().Name, n.Name, n.CreatedAt,
                (n as DirectoryNode)?.XmlAlias, (n as FileNode)?.SizeBytes ?? 0,
                (n as WordFile)?.Pages ?? 0, (n as ImageFile)?.Width ?? 0, (n as ImageFile)?.Height ?? 0,
                (n as TextFile)?.Encoding, n.Tags.ToArray()));
            for (var i = n.Children.Count - 1; i >= 0; i--) pending.Push((n.Children[i], index));
        }
        return new NodeSnapshot(rows);
    }
    internal FsNode Create(DirectoryNode destination)
    {
        var copies = new List<FsNode>(rows.Count);
        foreach (var row in rows)
        {
            var parent = row.Parent < 0 ? destination : (DirectoryNode)copies[row.Parent];
            FsNode copy = row.Kind switch
            {
                nameof(DirectoryNode) => new DirectoryNode(row.Name, row.Created, parent, row.Alias),
                nameof(WordFile) => new WordFile(row.Name, row.Bytes, row.Pages, row.Created, parent),
                nameof(ImageFile) => new ImageFile(row.Name, row.Bytes, row.Width, row.Height, row.Created, parent),
                nameof(TextFile) => new TextFile(row.Name, row.Bytes, row.Encoding!, row.Created, parent),
                _ => throw new InvalidOperationException("Unsupported snapshot kind.")
            };
            copy.LoadTags(row.Tags);
            if (row.Parent >= 0) parent.Insert(parent.Children.Count, copy, notify: false);
            copies.Add(copy);
        }
        return copies[0];
    }
}
