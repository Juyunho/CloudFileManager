
namespace CloudFileManager.Core.Application.Traversal;

public sealed record TraversalProgress(Guid NodeId, string Name, string Path, int Visited, int Total);
public sealed class TraversalProgressSource
{
    public event Action<TraversalProgress>? Progressed;
    internal void Publish(TraversalProgress progress) => Progressed?.Invoke(progress);
}
