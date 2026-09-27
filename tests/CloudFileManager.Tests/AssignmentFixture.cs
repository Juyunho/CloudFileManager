using System.Numerics;
using System.Text.Json;
using System.Xml.Linq;
using System.Reflection;
using CloudFileManager.Core.Application;
using CloudFileManager.Core.Application.Formatting;
using CloudFileManager.Core.Application.Samples;
using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Domain.Values;


namespace CloudFileManager.Tests;
public abstract class AssignmentFixture
{
    protected readonly DateTimeOffset now = new(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
    protected readonly DirectoryNode root = SampleTree.Create();
    protected readonly SourceFile[] source;
    protected AssignmentFixture()
    {
        using var document = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures/source-files.json")));
        source = document.RootElement.EnumerateArray().Select(r => new SourceFile(
            r.GetProperty("name").GetString()!, r.GetProperty("path").GetString()!,
            r.GetProperty("value").GetInt64(), r.GetProperty("unit").GetString()!)).ToArray();
    }
    // Independent oracle; never calls production conversion code.
    protected static BigInteger Oracle(SourceFile r) => new BigInteger(r.Value) * (r.Unit switch
    { "B" => BigInteger.One, "KB" => BigInteger.One << 10, "MB" => BigInteger.One << 20, _ => throw new Exception("Unknown source unit") });
    protected static void Check(bool value, string? message = null,
        [System.Runtime.CompilerServices.CallerArgumentExpression(nameof(value))] string? expression = null)
        => Assert.True(value, message ?? expression);
    protected static void Equal<T>(T expected, T actual) => Assert.Equal(expected, actual);
    protected static void Throws<T>(Action action) where T : Exception => Assert.ThrowsAny<T>(action);
    protected static List<FsNode> Flatten(FsNode root)
    {
        var result = new List<FsNode>(); var queue = new Queue<FsNode>(); queue.Enqueue(root);
        while (queue.TryDequeue(out var n)) { result.Add(n); foreach (var c in n.Children) queue.Enqueue(c); }
        return result;
    }
    protected DirectoryNode Fresh() => DirectoryNode.CreateRoot("root", now);

    protected sealed record SourceFile(string Name, string Path, long Value, string Unit);
}
