using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Domain.Values;

namespace CloudFileManager.Core.Application.Samples;

public static class SampleTree
{
    public static DirectoryNode Create()
    {
        // The assignment supplies no creation times. This is a deterministic demo value.
        var created = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var root = DirectoryNode.CreateRoot("根目錄 (Root)", created, "根目錄_Root");
        var projects = root.AddDirectory("專案文件 (Project_Docs)", created, "專案文件_Project_Docs");
        projects.AddWord("需求規格書.docx", BinarySize.From(500, "KB"), 15, created);
        projects.AddImage("系統架構圖.png", BinarySize.From(2, "MB"), 1920, 1080, created);
        var notes = root.AddDirectory("個人筆記 (Personal_Notes)", created, "個人筆記_Personal_Notes");
        notes.AddText("待辦清單.txt", BinarySize.From(1, "KB"), "UTF-8", created);
        var archive = notes.AddDirectory("2025備份 (Archive_2025)", created, "Archive_2025");
        archive.AddWord("舊會議記錄.docx", BinarySize.From(200, "KB"), 5, created);
        root.AddText("README.txt", BinarySize.From(500, "B"), "ASCII", created);
        return root;
    }
}
