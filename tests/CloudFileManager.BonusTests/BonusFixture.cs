using CloudFileManager.Core.Application;
using CloudFileManager.Core.Application.Formatting;
using CloudFileManager.Core.Application.Samples;
using CloudFileManager.Core.Application.Sessions;
using CloudFileManager.Core.Application.Sorting;
using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Domain.Values;
using System.Text.Json;


namespace CloudFileManager.BonusTests;
public abstract class BonusFixture
{
    protected readonly DateTimeOffset now = DateTimeOffset.Parse("2025-01-01T12:30:00+08:00");
    protected static void Check(bool value, string? message = null,
        [System.Runtime.CompilerServices.CallerArgumentExpression(nameof(value))] string? expression = null)
        => Assert.True(value, message ?? expression);
    protected static void Equal<T>(T expected, T actual) => Assert.Equal(expected, actual);
    protected static void Throws<T>(Action action) where T : Exception => Assert.ThrowsAny<T>(action);
    protected DirectoryNode Root() => DirectoryNode.CreateRoot("root", now);
    protected FsNode[] Walk(FsNode root)
    {
        var list = new List<FsNode>(); var q = new Queue<FsNode>(); q.Enqueue(root);
        while (q.TryDequeue(out var n)) { list.Add(n); foreach (var c in n.Children) q.Enqueue(c); }
        return list.ToArray();
    }
    protected string State(FsNode root, bool ids = true) => JsonSerializer.Serialize(Walk(root).Select(n => new
    { Id = ids ? n.Id : Guid.Empty, Kind = n.GetType().Name, n.Name, n.CreatedAt, Details = NodeDetailsFormatter.Format(n),
      Alias = (n as DirectoryNode)?.XmlAlias, n.Tags, Children = n.Children.Select(c => c.Name).ToArray() }));
    protected void Order(IReadOnlyList<FsNode> actual, params string[] names) => Check(actual.Select(n => n.Name).SequenceEqual(names), "order: " + string.Join(",",actual.Select(n=>n.Name)));


}
