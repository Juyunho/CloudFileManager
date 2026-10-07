using CloudFileManager.Core.Domain.Values;
namespace CloudFileManager.Core.Application.Persistence;

// Detached value data. Neither commands nor live node references cross the storage boundary.
public sealed record FileSystemRow(Guid Id, Guid? ParentId, int Ordinal, string Kind,
    string Name, DateTimeOffset CreatedAt, string? XmlAlias, long? SizeBytes,
    int? Pages, int? Width, int? Height, string? Encoding, IReadOnlyList<TagKind> Tags);
public sealed class FileSystemDocument
{
    public IReadOnlyList<FileSystemRow> Rows { get; }
    public FileSystemDocument(IEnumerable<FileSystemRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        Rows = Array.AsReadOnly(rows.Select(r => r with { Tags = Array.AsReadOnly(r.Tags.ToArray()) }).ToArray());
    }
}
