using CloudFileManager.Core.Domain.Nodes;

namespace CloudFileManager.Core.Application.Formatting;

public static class NodeDetailsFormatter
{
    public static string Format(FsNode node) => node switch
    {
        DirectoryNode => "目錄",
        WordFile w => $"頁數: {w.Pages}, 大小: {BinarySizeFormatter.Format(w.SizeBytes)}",
        ImageFile i => $"解析度: {i.Width}x{i.Height}, 大小: {BinarySizeFormatter.Format(i.SizeBytes)}",
        TextFile t => $"編碼: {t.Encoding}, 大小: {BinarySizeFormatter.Format(t.SizeBytes)}",
        _ => throw new ArgumentException("Unsupported node.", nameof(node))
    };
}
