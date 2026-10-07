using CloudFileManager.Core.Application.Persistence;
using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Domain.Values;

namespace CloudFileManager.Core.Application.Sessions;

/// <summary>
/// Application-wide context for the single-threaded Console application.
/// Instance uniqueness does not make Root, clipboard, history or Reset thread-safe.
/// Initialize explicitly with Reset before accessing session state. No locking is provided.
/// </summary>
public sealed class FileSystemSession : IFileSystemSession
{
    public static FileSystemSession Instance { get; } = new();
    private FileSystemSession() { }
    private sealed record CurrentState(DirectoryNode Root, EditingSession Editing);
    private CurrentState? current;
    private CurrentState Current
    {
        get
        {
            var value = current ?? throw new InvalidOperationException("Initialize the application session with Reset(root) first.");
            value.Editing.EnsureAvailable(); return value;
        }
    }
    public bool IsInitialized => current is not null;
    public DirectoryNode Root => Current.Root;
    public int UndoCount => Current.Editing.UndoCount;
    public int RedoCount => Current.Editing.RedoCount;
    public bool HasClipboard => Current.Editing.HasClipboard;

    /// <summary>
    /// Replaces the root and clears clipboard/undo/redo, including when given the same root.
    /// Not a domain command and cannot be undone. Invalid input preserves the old state.
    /// Existing root references are not destroyed. This operation is not thread-safe.
    /// </summary>
    public void Reset(DirectoryNode newRoot)
    {
        var next = new EditingSession(newRoot);
        current = new CurrentState(newRoot, next);
    }
    // Bootstrap only: builds a validated tree before replacing the active Singleton state.
    public void Restore(PersistedFileSystem persisted, IFileSystemStore store)
    {
        ArgumentNullException.ThrowIfNull(persisted);
        var root = FileSystemDocumentMapper.Restore(persisted.Document);
        var next = new EditingSession(root, store, persisted.Revision);
        current = new CurrentState(root, next);
    }
    public long DurableRevision => Current.Editing.DurableRevision;
    public void Copy(FsNode node) => Current.Editing.Copy(node);
    public FsNode Paste(DirectoryNode destination) => Current.Editing.Paste(destination);
    public void Delete(FsNode node) => Current.Editing.Delete(node);
    public bool AddTag(FsNode node, TagKind tag) => Current.Editing.AddTag(node, tag);
    public bool RemoveTag(FsNode node, TagKind tag) => Current.Editing.RemoveTag(node, tag);
    public bool Undo() => Current.Editing.Undo();
    public bool Redo() => Current.Editing.Redo();
}
