namespace CloudFileManager.Core;

public static class ReferenceTree
{
    // Seed data is not user Command history. The original assignment SampleTree stays unchanged.
    public static DirectoryNode Create()
    {
        var at = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var root = DirectoryNode.CreateRoot("我的根目錄", at);
        var notes = root.AddDirectory("個人筆記", at);
        var archive = notes.AddDirectory("2025備份", at); archive.LoadTags([TagKind.Personal]);
        archive.AddWord("會議記錄.docx", 200 * 1024, 5, at).LoadTags([TagKind.Work]);
        notes.AddText("待辦清單.txt", 1024, "UTF-8", at);
        var projects = root.AddDirectory("專案文件", at);
        projects.AddWord("API介面定義.docx", 120 * 1024, 12, at).LoadTags([TagKind.Personal]);
        projects.AddWord("需求規格書.docx", 500 * 1024, 35, at);
        projects.AddImage("系統架構圖.png", 2048 * 1024, 1920, 1080, at);
        root.AddText("README.txt", 500, "ASCII", at);
        return root;
    }
}
