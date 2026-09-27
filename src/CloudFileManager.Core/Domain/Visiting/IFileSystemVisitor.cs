using CloudFileManager.Core.Domain.Nodes;

namespace CloudFileManager.Core.Domain.Visiting;

public interface IFileSystemVisitor
{
    void Visit(DirectoryNode node);
    void Visit(WordFile node);
    void Visit(ImageFile node);
    void Visit(TextFile node);
}
