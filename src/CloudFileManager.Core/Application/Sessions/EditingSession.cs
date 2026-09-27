using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Domain.Values;
using CloudFileManager.Core.Domain.Prototypes;
using CloudFileManager.Core.Application.Commands;

namespace CloudFileManager.Core.Application.Sessions;

// Single-threaded session. Out-of-session builder edits invalidate this history.
public sealed class EditingSession
{
    private readonly DirectoryNode root;
    private readonly Stack<IEditCommand> undo = new();
    private readonly Stack<IEditCommand> redo = new();
    private INodePrototype? clipboard;
    private long revision;
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
        command.Execute();
        revision = root.State.Revision;
        undo.Push(command); redo.Clear();
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
        command.Undo(); revision = root.State.Revision; undo.Pop(); redo.Push(command); return true;
    }
    public bool Redo()
    {
        CheckVersion(); if (!redo.TryPeek(out var command)) return false;
        command.Execute(); revision = root.State.Revision; redo.Pop(); undo.Push(command); return true;
    }
}
