using CloudFileManager.Core.Application.Samples;
using CloudFileManager.Core.Application.Sessions;
using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Domain.Values;

namespace CloudFileManager.WebTests;

public sealed class WebWorkspaceTests : WebWorkspaceContractTests, IDisposable
{
    protected override IFileSystemSession Session => FileSystemSession.Instance;
    public WebWorkspaceTests() => FileSystemSession.Instance.Reset(ReferenceTree.Create());
    public void Dispose() => FileSystemSession.Instance.Reset(DirectoryNode.CreateRoot("cleanup", DateTimeOffset.UnixEpoch));
}

public sealed class IsolatedWebWorkspaceTests : WebWorkspaceContractTests
{
    protected override IFileSystemSession Session { get; } = new IsolatedTestSession(ReferenceTree.Create());
}

// Test-only wiring; all editing rules/history/snapshots are the real production EditingSession.
internal sealed class IsolatedTestSession : IFileSystemSession
{
    private readonly EditingSession editing;
    public IsolatedTestSession(DirectoryNode root) { editing = new EditingSession(root); Root = root; }
    public bool IsInitialized => true;
    public DirectoryNode Root { get; }
    public int UndoCount => editing.UndoCount;
    public int RedoCount => editing.RedoCount;
    public bool HasClipboard => editing.HasClipboard;
    public void Copy(FsNode node) => editing.Copy(node);
    public FsNode Paste(DirectoryNode destination) => editing.Paste(destination);
    public void Delete(FsNode node) => editing.Delete(node);
    public bool AddTag(FsNode node, TagKind tag) => editing.AddTag(node, tag);
    public bool RemoveTag(FsNode node, TagKind tag) => editing.RemoveTag(node, tag);
    public bool Undo() => editing.Undo();
    public bool Redo() => editing.Redo();
}
