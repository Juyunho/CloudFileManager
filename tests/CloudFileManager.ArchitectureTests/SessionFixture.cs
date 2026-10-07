using CloudFileManager.Core.Application;
using CloudFileManager.Core.Application.Sessions;
using CloudFileManager.Core.Application.Sorting;
using CloudFileManager.Core.Application.Traversal;
using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Domain.Values;
using CloudFileManager.Core.Domain.Visiting;


namespace CloudFileManager.ArchitectureTests;
public abstract class VisitorFixture
{
    protected readonly DateTimeOffset now = DateTimeOffset.Parse("2025-01-01T00:00:00Z");
    protected DirectoryNode Root(string name = "root") => DirectoryNode.CreateRoot(name, now);
    protected static void Check(bool value, string? message = null,
        [System.Runtime.CompilerServices.CallerArgumentExpression(nameof(value))] string? expression = null)
        => Assert.True(value, message ?? expression);
    protected static void Equal<T>(T expected, T actual) => Assert.Equal(expected, actual);
    protected static void Throws<T>(Action action) where T : Exception => Assert.ThrowsAny<T>(action);
}
public abstract class SessionFixture : VisitorFixture, IDisposable
{
    protected SessionFixture() => FileSystemSession.Instance.Reset(Root("test"));
    public void Dispose() => FileSystemSession.Instance.Reset(Root("cleanup"));
}
sealed class RecordingVisitor:IFileSystemVisitor
{
    public List<string> Items {get;}=[];
    public void Visit(DirectoryNode n)=>Items.Add("D:"+n.Name);
    public void Visit(WordFile n)=>Items.Add("W:"+n.Name);
    public void Visit(ImageFile n)=>Items.Add("I:"+n.Name);
    public void Visit(TextFile n)=>Items.Add("T:"+n.Name);
}
