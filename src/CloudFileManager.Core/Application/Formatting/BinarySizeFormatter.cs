using System.Globalization;

namespace CloudFileManager.Core.Application.Formatting;

public static class BinarySizeFormatter
{
    public static string Format(long bytes)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(bytes);
        if (bytes > 0 && bytes % (1024L * 1024) == 0) return (bytes / (1024L * 1024)).ToString(CultureInfo.InvariantCulture) + "MB";
        if (bytes > 0 && bytes % 1024 == 0) return (bytes / 1024).ToString(CultureInfo.InvariantCulture) + "KB";
        return bytes.ToString(CultureInfo.InvariantCulture) + "B";
    }
}
