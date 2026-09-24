using System.Collections.ObjectModel;
using System.Globalization;

namespace CloudFileManager.Core;

public abstract class FsNode
{
    private protected FsNode(string name, DateTimeOffset createdAt, DirectoryNode? parent)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (name is "." or ".." || name.Any(c => char.IsControl(c) || c is '/' or '\\'))
            throw new ArgumentException("Name cannot be a path or contain control characters.", nameof(name));
        Name = name; CreatedAt = createdAt; Parent = parent;
        State = parent?.State ?? new TreeState();
    }
    internal TreeState State { get; }
    private readonly HashSet<TagKind> tags = [];
    public IReadOnlyList<TagKind> Tags => Array.AsReadOnly(tags.Order().ToArray());
    internal bool SetTag(TagKind tag, bool present)
    {
        var changed = present ? tags.Add(tag) : tags.Remove(tag);
        if (changed) State.Revision++;
        return changed;
    }
    internal void LoadTags(IEnumerable<TagKind> values) => tags.UnionWith(values);
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; }
    public DateTimeOffset CreatedAt { get; }
    public DirectoryNode? Parent { get; }
    public virtual IReadOnlyList<FsNode> Children => Array.Empty<FsNode>();
    public abstract string Details { get; }
    public abstract void Accept(IFileSystemVisitor visitor);
    public string FullPath
    {
        get
        {
            var parts = new Stack<string>();
            for (FsNode? n = this; n is not null; n = n.Parent) parts.Push(n.Name);
            return string.Join('/', parts);
        }
    }
}

public sealed class DirectoryNode : FsNode
{
    private readonly List<FsNode> children = [];
    private readonly ReadOnlyCollection<FsNode> view;
    internal DirectoryNode(string name, DateTimeOffset createdAt, DirectoryNode? parent, string? xmlAlias)
        : base(name, createdAt, parent)
    {
        if (xmlAlias is not null) ArgumentException.ThrowIfNullOrWhiteSpace(xmlAlias);
        XmlAlias = xmlAlias; view = children.AsReadOnly();
    }
    public override void Accept(IFileSystemVisitor visitor)
    { ArgumentNullException.ThrowIfNull(visitor); visitor.Visit(this); }
    public string? XmlAlias { get; }
    public override IReadOnlyList<FsNode> Children => view;
    public override string Details => "目錄";
    public static DirectoryNode CreateRoot(string name, DateTimeOffset createdAt, string? xmlAlias = null)
        => new(name, createdAt, null, xmlAlias);
    private T Attach<T>(T node) where T : FsNode
    {
        if (children.Any(c => string.Equals(c.Name, node.Name, StringComparison.Ordinal)))
            throw new ArgumentException("A sibling already has this name.", nameof(node));
        children.Add(node);
        State.Revision++;
        return node;
    }
    internal void Insert(int index, FsNode node, bool notify = true)
    {
        if (!ReferenceEquals(node.Parent, this)) throw new InvalidOperationException("Parent mismatch.");
        if (children.Any(c => string.Equals(c.Name, node.Name, StringComparison.Ordinal)))
            throw new ArgumentException("A sibling already has this name.");
        children.Insert(index, node);
        if (notify) State.Revision++;
    }
    internal void Remove(FsNode node)
    {
        if (!children.Remove(node)) throw new InvalidOperationException("Node is not attached.");
        State.Revision++;
    }
    public DirectoryNode AddDirectory(string name, DateTimeOffset createdAt, string? xmlAlias = null)
        => Attach(new DirectoryNode(name, createdAt, this, xmlAlias));
    public WordFile AddWord(string name, long bytes, int pages, DateTimeOffset createdAt)
        => Attach(new WordFile(name, bytes, pages, createdAt, this));
    public ImageFile AddImage(string name, long bytes, int width, int height, DateTimeOffset createdAt)
        => Attach(new ImageFile(name, bytes, width, height, createdAt, this));
    public TextFile AddText(string name, long bytes, string encoding, DateTimeOffset createdAt)
        => Attach(new TextFile(name, bytes, encoding, createdAt, this));
}

public abstract class FileNode : FsNode
{
    private protected FileNode(string name, long bytes, DateTimeOffset createdAt, DirectoryNode parent)
        : base(name, createdAt, parent)
    {
        ArgumentNullException.ThrowIfNull(parent);
        ArgumentOutOfRangeException.ThrowIfNegative(bytes);
        SizeBytes = bytes;
    }
    public long SizeBytes { get; }
    public string Extension => Path.GetExtension(Name);
}

public sealed class WordFile : FileNode
{
    internal WordFile(string name, long bytes, int pages, DateTimeOffset createdAt, DirectoryNode parent)
        : base(name, bytes, createdAt, parent)
    { ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pages); Pages = pages; }
    public override void Accept(IFileSystemVisitor visitor)
    { ArgumentNullException.ThrowIfNull(visitor); visitor.Visit(this); }
    public int Pages { get; }
    public override string Details => $"頁數: {Pages}, 大小: {BinarySize.Format(SizeBytes)}";
}

public sealed class ImageFile : FileNode
{
    internal ImageFile(string name, long bytes, int width, int height, DateTimeOffset createdAt, DirectoryNode parent)
        : base(name, bytes, createdAt, parent)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
        Width = width; Height = height;
    }
    public override void Accept(IFileSystemVisitor visitor)
    { ArgumentNullException.ThrowIfNull(visitor); visitor.Visit(this); }
    public int Width { get; }
    public int Height { get; }
    public override string Details => $"解析度: {Width}x{Height}, 大小: {BinarySize.Format(SizeBytes)}";
}

public sealed class TextFile : FileNode
{
    internal TextFile(string name, long bytes, string encoding, DateTimeOffset createdAt, DirectoryNode parent)
        : base(name, bytes, createdAt, parent)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(encoding);
        if (encoding.Any(char.IsControl)) throw new ArgumentException("Encoding cannot contain control characters.", nameof(encoding));
        Encoding = encoding;
    }
    public override void Accept(IFileSystemVisitor visitor)
    { ArgumentNullException.ThrowIfNull(visitor); visitor.Visit(this); }
    public string Encoding { get; }
    public override string Details => $"編碼: {Encoding}, 大小: {BinarySize.Format(SizeBytes)}";
}

public static class BinarySize
{
    public static long From(long value, string unit)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        ArgumentNullException.ThrowIfNull(unit);
        var factor = unit.Trim().ToUpperInvariant() switch
        { "B" => 1L, "KB" => 1024L, "MB" => 1024L * 1024, _ => throw new ArgumentException("Use B, KB or MB.", nameof(unit)) };
        return checked(value * factor);
    }
    public static string Format(long bytes)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(bytes);
        if (bytes > 0 && bytes % (1024L * 1024) == 0) return (bytes / (1024L * 1024)).ToString(CultureInfo.InvariantCulture) + "MB";
        if (bytes > 0 && bytes % 1024 == 0) return (bytes / 1024).ToString(CultureInfo.InvariantCulture) + "KB";
        return bytes.ToString(CultureInfo.InvariantCulture) + "B";
    }
}

internal sealed class TreeState { internal long Revision; }
