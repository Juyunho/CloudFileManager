using CloudFileManager.Core.Domain.Nodes;

namespace CloudFileManager.Core.Domain.Visiting;

// One accumulator per operation; callers should create a fresh visitor for each traversal.
public sealed class SizeVisitor : IFileSystemVisitor
{
    public long TotalBytes { get; private set; }
    public void Visit(DirectoryNode node) { }
    public void Visit(WordFile node) => Add(node);
    public void Visit(ImageFile node) => Add(node);
    public void Visit(TextFile node) => Add(node);
    private void Add(FileNode node) => TotalBytes = checked(TotalBytes + node.SizeBytes);
}

public sealed class ExtensionSearchVisitor : IFileSystemVisitor
{
    private readonly string extension;
    private readonly List<string> paths = [];
    private readonly List<Guid> matchedNodeIds = [];
    public ExtensionSearchVisitor(string extension)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(extension);
        extension = extension.Trim();
        if (!extension.StartsWith('.')) extension = "." + extension;
        if (extension.Length == 1 || extension[1..].Any(c => c is '.' or '/' or '\\' or '*' or '?' || char.IsWhiteSpace(c)))
            throw new ArgumentException("Supply a single extension, for example .docx.", nameof(extension));
        this.extension = extension;
    }
    public IReadOnlyList<string> Paths => paths.AsReadOnly();
    public IReadOnlyList<Guid> MatchedNodeIds => matchedNodeIds.AsReadOnly();
    public void Visit(DirectoryNode node) { }
    public void Visit(WordFile node) => Match(node);
    public void Visit(ImageFile node) => Match(node);
    public void Visit(TextFile node) => Match(node);
    private void Match(FileNode node)
    {
        if (string.Equals(node.Extension, extension, StringComparison.OrdinalIgnoreCase))
        { paths.Add(node.FullPath); matchedNodeIds.Add(node.Id); }
    }
}
