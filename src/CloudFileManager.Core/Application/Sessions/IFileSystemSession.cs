using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Domain.Values;

namespace CloudFileManager.Core.Application.Sessions;

/// <summary>Consumer operations on an initialized session. Reset belongs to bootstrap/lifecycle ownership.</summary>
public interface IFileSystemSession
{
    bool IsInitialized { get; }
    DirectoryNode Root { get; }
    int UndoCount { get; }
    int RedoCount { get; }
    bool HasClipboard { get; }
    void Copy(FsNode node);
    FsNode Paste(DirectoryNode destination);
    void Delete(FsNode node);
    bool AddTag(FsNode node, TagKind tag);
    bool RemoveTag(FsNode node, TagKind tag);
    bool Undo();
    bool Redo();
}
