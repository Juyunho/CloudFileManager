namespace CloudFileManager.Core.Application.Persistence;

public interface IFileSystemStore
{
    PersistedFileSystem LoadOrInitialize(FileSystemDocument seed);
    CommitReceipt Commit(FileSystemDocument document, long expectedRevision, Guid operationId);
}

public sealed record PersistedFileSystem(FileSystemDocument Document, long Revision);
public sealed record CommitReceipt(long Revision, Guid OperationId);
public enum PersistenceOutcome { ConfirmedNotCommitted, Unknown }
public sealed class PersistenceException(string message, PersistenceOutcome outcome, Exception? inner = null)
    : Exception(message, inner)
{
    public PersistenceOutcome Outcome { get; } = outcome;
}
public sealed class SessionUnavailableException(string message, Exception? inner = null) : Exception(message, inner);
