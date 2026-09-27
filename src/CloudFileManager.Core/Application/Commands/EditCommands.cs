using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Domain.Values;

namespace CloudFileManager.Core.Application.Commands;

internal interface IEditCommand { void Execute(); void Undo(); }
internal sealed class DeleteCommand(DirectoryNode parent, FsNode node, int index) : IEditCommand
{
    public void Execute() => parent.Remove(node);
    public void Undo() => parent.Insert(index, node);
}
internal sealed class PasteCommand(DirectoryNode parent, FsNode node, int index) : IEditCommand
{
    public void Execute() => parent.Insert(index, node);
    public void Undo() => parent.Remove(node);
}
internal sealed class TagCommand(FsNode node, TagKind tag, bool present) : IEditCommand
{
    public void Execute() => node.SetTag(tag, present);
    public void Undo() => node.SetTag(tag, !present);
}
