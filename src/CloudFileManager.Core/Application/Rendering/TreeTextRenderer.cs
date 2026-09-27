using System.Text;
using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Application.Formatting;

namespace CloudFileManager.Core.Application.Rendering;

public static class TreeTextRenderer
{
    public static string Render(DirectoryNode root)
    {
        ArgumentNullException.ThrowIfNull(root);
        var output = new StringBuilder();
        var stack = new Stack<(FsNode Node, string Prefix, bool Last, bool Root)>();
        stack.Push((root, "", true, true));
        while (stack.TryPop(out var item))
        {
            var (node, prefix, last, isRoot) = item;
            var type = node switch { DirectoryNode => "目錄", WordFile => "Word 檔案", ImageFile => "圖片", TextFile => "純文字檔", _ => "節點" };
            output.Append(prefix).Append(isRoot ? "" : last ? "└── " : "├── ").Append(node.Name)
                .Append(" [").Append(type).Append("] (").Append(NodeDetailsFormatter.Format(node))
                .Append(", 建立: ").Append(node.CreatedAt.ToString("O")).AppendLine(")");
            var childPrefix = prefix + (isRoot ? "" : last ? "    " : "│   ");
            for (var i = node.Children.Count - 1; i >= 0; i--)
                stack.Push((node.Children[i], childPrefix, i == node.Children.Count - 1, false));
        }
        return output.ToString();
    }
}
