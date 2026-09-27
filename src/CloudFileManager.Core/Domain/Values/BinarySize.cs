
namespace CloudFileManager.Core.Domain.Values;

public static class BinarySize
{
    public static long From(long value, string unit)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        ArgumentNullException.ThrowIfNull(unit);
        var factor = unit.Trim().ToUpperInvariant() switch
        { "B" => 1L, "KB" => 1024L, "MB" => 1024L * 1024, _ => throw new ArgumentException("Use B, KB or MB.", nameof(unit)) };
        return checked(value * factor);
    }
}
