using CloudFileManager.Core;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
if (args.Length > 1 || (args.Length == 1 && args[0] is not ("--xml" or "--help" or "--bonus")))
{
    Console.Error.WriteLine("Usage: CloudFileManager [--xml|--help|--bonus]");
    return 2;
}
if (args.Contains("--help")) { Console.WriteLine("CloudFileManager: no arguments for demo; --xml for XML only; --bonus for editing/sorting/tags demo."); return 0; }
if (args.Contains("--bonus")) { BonusDemo.Run(); return 0; }
var session = FileSystemSession.Instance;
session.Reset(SampleTree.Create());
var root = session.Root;
if (args.Contains("--xml")) { Console.WriteLine(TreeOperations.ToXml(root)); return 0; }
Console.WriteLine("容量採二進位：1 KB = 1024 B；1 MB = 1024 KB");
Console.WriteLine("建立時間為固定示範值，非題目提供的真實時間。\n");
Console.WriteLine(TreeOperations.Render(root));
Console.WriteLine("=== 計算總容量 ===");
var total = TreeOperations.CalculateTotalSize(root);
Console.WriteLine($"總容量: {total} B\n");
Console.WriteLine("=== 搜尋 .docx ===");
foreach (var path in TreeOperations.SearchByExtension(root, ".docx")) Console.WriteLine($"Found: {path}");
Console.WriteLine("\n=== XML ===");
Console.WriteLine(TreeOperations.ToXml(root));
return 0;
