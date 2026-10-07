using CloudFileManager.Core.Application.Persistence;
using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Domain.Values;
using CloudFileManager.Core.Domain.Prototypes;
using CloudFileManager.Core.Application.Commands;

namespace CloudFileManager.Core.Application.Sessions;

// Single-threaded session. Out-of-session builder edits invalidate this history.
public sealed class EditingSession
{
    private readonly DirectoryNode root;
    private Stack<IEditCommand> undo = new();
    private Stack<IEditCommand> redo = new();
    private INodePrototype? clipboard;
    private long revision;
    private readonly IFileSystemStore? store;
    private long durableRevision;
    private bool faulted;
    public long DurableRevision => durableRevision;
    public void EnsureAvailable()
    {
        if (faulted) throw new SessionUnavailableException("Session unavailable after an uncertain persistence outcome; restart required.");
    }
    public EditingSession(DirectoryNode root, IFileSystemStore store, long durableRevision) : this(root)
    {
        ArgumentNullException.ThrowIfNull(store);
        ArgumentOutOfRangeException.ThrowIfNegative(durableRevision);
        this.store = store; this.durableRevision = durableRevision;
    }
    public EditingSession(DirectoryNode root)
    {
        ArgumentNullException.ThrowIfNull(root);
        if (root.Parent is not null) throw new ArgumentException("Supply a root directory.", nameof(root));
        this.root = root; revision = root.State.Revision;
    }
    public int UndoCount => undo.Count;
    public int RedoCount => redo.Count;
    public bool HasClipboard => clipboard is not null;
    private void CheckVersion()
    {
        EnsureAvailable();
        if (revision != root.State.Revision)
            throw new InvalidOperationException("Tree changed outside this session. Create a new session.");
    }
    private void CheckLive(FsNode node)
    {
        ArgumentNullException.ThrowIfNull(node); CheckVersion();
        var current = node;
        while (current.Parent is { } parent)
        {
            if (!parent.Children.Contains(current)) throw new InvalidOperationException("Node is not in the live tree.");
            current = parent;
        }
        if (!ReferenceEquals(current, root)) throw new InvalidOperationException("Node belongs to another tree.");
    }
    private void Execute(IEditCommand command)
    {
        var nextUndo = new Stack<IEditCommand>(undo.Reverse()); nextUndo.Push(command);
        Apply(command.Execute, command.Undo, nextUndo, new Stack<IEditCommand>());
    }
    private void Apply(Action forward, Action inverse, Stack<IEditCommand> nextUndo, Stack<IEditCommand> nextRedo)
    {
        var beforeRevision = root.State.Revision;
        // Built-in commands validate/allocate before mutation; compensation retains the original objects.
        forward();
        var committedRevision = durableRevision;
        try
        {
            if (store is not null)
            {
                var document = FileSystemDocumentMapper.Capture(root);
                var operation = Guid.NewGuid();
                CommitReceipt receipt;
                try { receipt = store.Commit(document, durableRevision, operation); }
                catch (PersistenceException ex) when (ex.Outcome == PersistenceOutcome.ConfirmedNotCommitted) { throw; }
                catch (Exception ex)
                {
                    faulted = true;
                    throw new SessionUnavailableException("Durable commit outcome is unknown; session stopped.", ex);
                }
                if (receipt.OperationId != operation || receipt.Revision != checked(durableRevision + 1))
                {
                    faulted = true;
                    throw new SessionUnavailableException("Invalid durable commit receipt; session stopped.");
                }
                committedRevision = receipt.Revision;
            }
        }
        catch (SessionUnavailableException) { throw; }
        catch (Exception)
        {
            try { inverse(); root.State.Revision = beforeRevision; }
            catch (Exception recovery)
            {
                faulted = true;
                throw new SessionUnavailableException("In-memory compensation failed; session stopped.", recovery);
            }
            // Old stacks, clipboard, session revision and durable revision were never published.
            throw;
        }
        // No callbacks or allocations after the durable commit.
        undo = nextUndo; redo = nextRedo;
        revision = root.State.Revision; durableRevision = committedRevision;
    }
    public void Copy(FsNode node) { CheckLive(node); clipboard = NodeSnapshot.Capture(node); }
    public FsNode Paste(DirectoryNode destination)
    {
        CheckLive(destination);
        var snapshot = clipboard ?? throw new InvalidOperationException("Clipboard is empty.");
        if (destination.Children.Any(n => string.Equals(n.Name, snapshot.Name, StringComparison.Ordinal)))
            throw new ArgumentException("A sibling already has this name.");
        var copy = snapshot.CloneInto(destination);
        Execute(new PasteCommand(destination, copy, destination.Children.Count));
        return copy;
    }
    public void Delete(FsNode node)
    {
        CheckLive(node);
        var parent = node.Parent ?? throw new InvalidOperationException("Cannot delete the root.");
        var index = parent.Children.ToList().IndexOf(node);
        Execute(new DeleteCommand(parent, node, index));
    }
    public bool AddTag(FsNode node, TagKind tag) => SetTag(node, tag, true);
    public bool RemoveTag(FsNode node, TagKind tag) => SetTag(node, tag, false);
    private bool SetTag(FsNode node, TagKind tag, bool present)
    {
        CheckLive(node);
        if (!Enum.IsDefined(tag)) throw new ArgumentOutOfRangeException(nameof(tag));
        if (node.Tags.Contains(tag) == present) return false;
        Execute(new TagCommand(node, tag, present)); return true;
    }
    public bool Undo()
    {
        CheckVersion(); if (!undo.TryPeek(out var command)) return false;
        var nextUndo = new Stack<IEditCommand>(undo.Reverse()); nextUndo.Pop();
        var nextRedo = new Stack<IEditCommand>(redo.Reverse()); nextRedo.Push(command);
        Apply(command.Undo, command.Execute, nextUndo, nextRedo); return true;
    }
    public bool Redo()
    {
        CheckVersion(); if (!redo.TryPeek(out var command)) return false;
        var nextRedo = new Stack<IEditCommand>(redo.Reverse()); nextRedo.Pop();
        var nextUndo = new Stack<IEditCommand>(undo.Reverse()); nextUndo.Push(command);
        Apply(command.Execute, command.Undo, nextUndo, nextRedo); return true;
    }
}
