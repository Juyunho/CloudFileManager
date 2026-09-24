using System.Numerics;
using System.Text.Json;
using System.Xml.Linq;
using System.Reflection;
using CloudFileManager.Core;

var failed = 0; var passed = 0;
var now = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
var source = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures/source-files.json")))
    .RootElement.EnumerateArray().Select(r => new SourceFile(r.GetProperty("name").GetString()!, r.GetProperty("path").GetString()!,
        r.GetProperty("value").GetInt64(), r.GetProperty("unit").GetString()!)).ToArray();
// This oracle uses only values transcribed from the source DOCX, not product conversion code.
BigInteger Oracle(SourceFile r) => new BigInteger(r.Value) * (r.Unit switch
{ "B" => BigInteger.One, "KB" => BigInteger.One << 10, "MB" => BigInteger.One << 20, _ => throw new Exception("Unknown source unit") });
void Check(bool value, string message) { if (!value) throw new Exception(message); }
void Equal<T>(T expected, T actual) { if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new Exception($"Expected {expected}; actual {actual}"); }
void Throws<T>(Action action) where T : Exception
{ try { action(); } catch (T) { return; } throw new Exception("Expected " + typeof(T).Name); }
void Test(string id, Action body)
{ try { body(); Console.WriteLine("PASS " + id); passed++; } catch (Exception e) { failed++; Console.WriteLine("FAIL " + id + ": " + e); } }
List<FsNode> Flatten(FsNode root)
{
    var result = new List<FsNode>(); var queue = new Queue<FsNode>(); queue.Enqueue(root);
    while (queue.TryDequeue(out var n)) { result.Add(n); foreach (var c in n.Children) queue.Enqueue(c); }
    return result;
}
DirectoryNode Fresh() => DirectoryNode.CreateRoot("root", now);
var root = SampleTree.Create();
Test("T01 independent source oracle and exact file set", () =>
{
    Equal(5, source.Length);
    var actualFiles = Flatten(root).OfType<FileNode>().ToDictionary(n => n.FullPath);
    Equal(source.Length, actualFiles.Count);
    BigInteger expected = 0;
    foreach (var row in source)
    {
        var bytes = Oracle(row); expected += bytes;
        Check(actualFiles.ContainsKey(row.Path), "Missing source path " + row.Path);
        Equal(bytes, new BigInteger(actualFiles[row.Path].SizeBytes));
        Console.WriteLine($"SOURCE {row.Name}: {row.Value} {row.Unit} => {bytes} B");
    }
    var actual = TreeOperations.CalculateTotalSize(root, TextWriter.Null);
    Console.WriteLine($"INDEPENDENT SUM = {expected} B; PRODUCT = {actual} B");
    Equal(expected, new BigInteger(actual));
});
Test("T02 each subtree independently grouped from source paths", () =>
{
    foreach (var dir in Flatten(root).OfType<DirectoryNode>())
    {
        var expected = source.Where(r => r.Path.StartsWith(dir.FullPath + "/", StringComparison.Ordinal))
            .Aggregate(BigInteger.Zero, (sum, r) => sum + Oracle(r));
        Equal(expected, new BigInteger(TreeOperations.CalculateTotalSize(dir, TextWriter.Null)));
        Console.WriteLine($"SUBTREE {dir.Name} => {expected} B");
    }
});
Test("T03 binary units and byte precision", () =>
{
    Equal(1L << 10, BinarySize.From(1,"KB")); Equal(1L << 20, BinarySize.From(1,"MB"));
    Equal(500L, BinarySize.From(500,"B")); Equal("500B", BinarySize.Format(500)); Equal("0B",BinarySize.Format(0));
    Equal("1025B",BinarySize.Format(1025)); Equal("1KB",BinarySize.Format(1024)); Equal("1MB",BinarySize.Format(1L<<20));
    Throws<ArgumentException>(() => BinarySize.From(1,"GB")); Throws<ArgumentOutOfRangeException>(() => BinarySize.From(-1,"B"));
});
Test("T04 search normalization scope and no result", () =>
{
    var expected=source.Where(r => r.Name.EndsWith(".docx",StringComparison.Ordinal)).Select(r=>r.Path).ToArray();
    foreach(var ext in new[]{".docx","DOCX"," .DoCx "})
        Check(expected.SequenceEqual(TreeOperations.SearchByExtension(root,ext,TextWriter.Null)),"search paths mismatch");
    var archive=Flatten(root).OfType<DirectoryNode>().Single(d=>d.Name.StartsWith("2025備份"));
    Equal(1,TreeOperations.SearchByExtension(archive,"docx",TextWriter.Null).Count);
    Equal(source.Single(r=>r.Name=="舊會議記錄.docx").Path,TreeOperations.SearchByExtension(archive,"docx",TextWriter.Null)[0]);
    Equal(0,TreeOperations.SearchByExtension(root,"zip",TextWriter.Null).Count);
});
Test("T05 exact traversal sequence for both operations", () =>
{
    string[] order = ["根目錄 (Root)","根目錄 (Root)/專案文件 (Project_Docs)",
        source[0].Path, source[1].Path,"根目錄 (Root)/個人筆記 (Personal_Notes)",source[2].Path,
        "根目錄 (Root)/個人筆記 (Personal_Notes)/2025備份 (Archive_2025)",source[3].Path,source[4].Path];
    using var sizeLog=new StringWriter(); using var searchLog=new StringWriter();
    TreeOperations.CalculateTotalSize(root,sizeLog); TreeOperations.SearchByExtension(root,"docx",searchLog);
    var expected=order.Select(p=>"Visiting: "+p);
    Check(expected.SequenceEqual(sizeLog.ToString().Split(Environment.NewLine,StringSplitOptions.RemoveEmptyEntries)),"size traversal");
    Check(expected.SequenceEqual(searchLog.ToString().Split(Environment.NewLine,StringSplitOptions.RemoveEmptyEntries)),"search traversal");
});
Test("T06 XML matches original assignment fixture", () =>
{
    var expected=XDocument.Load(Path.Combine(AppContext.BaseDirectory,"Fixtures/expected.xml"));
    var actual=XDocument.Parse(TreeOperations.ToXml(root));
    Check(XNode.DeepEquals(expected,actual),"XML differs from original assignment");
});
Test("T07 XML invalid names collisions and escaped text", () =>
{
    var r=DirectoryNode.CreateRoot("9 root:&",now);
    r.AddText("a.b",0,"<&>",now); r.AddText("a_b",0,"ASCII",now); r.AddText("a_b__2",0,"UTF-8",now);
    r.AddDirectory("a b:c",now).AddText("_x0041_.txt",1,"UTF-8",now);
    var xml=TreeOperations.ToXml(r); var doc=XDocument.Parse(xml);
    var children=doc.Root!.Elements().ToArray(); Equal(4,children.Length);
    Equal(4,children.Select(e=>e.Name.LocalName).Distinct().Count());
    Check(children[0].Value.Contains("<&>"),"XML text did not roundtrip");
    Check(xml.Contains("&lt;&amp;&gt;"),"text not escaped");
    Equal(5,doc.Root.Descendants().Count());
});
Test("T08 empty directory and zero byte file", () =>
{
    var r=Fresh(); using var log=new StringWriter();
    Equal(0L,TreeOperations.CalculateTotalSize(r,log)); Equal("Visiting: root"+Environment.NewLine,log.ToString());
    Equal(0,TreeOperations.SearchByExtension(r,"txt",TextWriter.Null).Count);
    r.AddText("empty.txt",0,"UTF-8",now); Equal(0L,TreeOperations.CalculateTotalSize(r,TextWriter.Null));
    Equal(1,TreeOperations.SearchByExtension(r,"txt",TextWriter.Null).Count);
});
Test("T09 invalid creation is rejected without mutation", () =>
{
    var r=Fresh();
    Throws<ArgumentOutOfRangeException>(()=>r.AddText("a",-1,"UTF-8",now));
    Throws<ArgumentOutOfRangeException>(()=>r.AddWord("a",1,0,now));
    Throws<ArgumentOutOfRangeException>(()=>r.AddImage("a",1,0,1,now));
    Throws<ArgumentOutOfRangeException>(()=>r.AddImage("a",1,1,0,now));
    Throws<ArgumentException>(()=>r.AddText("a",1," ",now));
    Throws<ArgumentException>(()=>r.AddText("a",1,"bad\n",now));
    foreach(var bad in new[]{""," ",".","..","a/b","a\\b","a\0b"})
        Throws<ArgumentException>(()=>r.AddDirectory(bad,now));
    Equal(0,r.Children.Count);r.AddText("same",1,"UTF-8",now);
    Throws<ArgumentException>(()=>r.AddDirectory("same",now)); Equal(1,r.Children.Count);
});
Test("T10 topology and public ownership contract", () =>
{
    foreach(var type in new[]{typeof(WordFile),typeof(ImageFile),typeof(TextFile)}) Equal(0,type.GetConstructors().Length);
    Equal<MethodInfo?>(null,typeof(FsNode).GetProperty("Parent")!.SetMethod);
    Check(!typeof(DirectoryNode).GetMethods().Any(m=>m.GetParameters().Any(a=>typeof(FsNode).IsAssignableFrom(a.ParameterType))),"public attach/reparent API");
    var nodes=Flatten(root); Equal(9,nodes.Count); Equal(9,nodes.Select(n=>n.Id).Distinct().Count());
    foreach(var d in nodes.OfType<DirectoryNode>()) foreach(var c in d.Children) Check(ReferenceEquals(d,c.Parent),"parent mismatch");
    Throws<NotSupportedException>(()=>((IList<FsNode>)root.Children).Clear());
    Equal(9,Flatten(root).Count);
});
Test("T11 deep hierarchy without recursive call stack", () =>
{
    var r=Fresh(); var last=r;
    for(var i=0;i<2000;i++) last=last.AddDirectory("d",now);
    last.AddText("deep.txt",7,"UTF-8",now);
    Equal(7L,TreeOperations.CalculateTotalSize(r,TextWriter.Null));
    Equal(1,TreeOperations.SearchByExtension(r,"txt",TextWriter.Null).Count);
    var xml=TreeOperations.ToXml(r);Check(xml.Contains("deep_txt"),"missing deep XML leaf");
});
Test("T12 checked arithmetic prevents silent overflow", () =>
{
    Throws<OverflowException>(()=>BinarySize.From(long.MaxValue,"KB"));
    var r=Fresh();r.AddText("one",long.MaxValue,"UTF-8",now);r.AddText("two",1,"UTF-8",now);
    Throws<OverflowException>(()=>TreeOperations.CalculateTotalSize(r,TextWriter.Null));
});
Test("T13 metadata and rendered details", () =>
{
    var nodes=Flatten(root);var word=nodes.OfType<WordFile>().Single(w=>w.Name=="需求規格書.docx");Equal(15,word.Pages);
    Equal(5,nodes.OfType<WordFile>().Single(w=>w.Name=="舊會議記錄.docx").Pages);
    var image=nodes.OfType<ImageFile>().Single();Equal(1920,image.Width);Equal(1080,image.Height);
    Equal("UTF-8",nodes.OfType<TextFile>().Single(w=>w.Name=="待辦清單.txt").Encoding);
    Equal("ASCII",nodes.OfType<TextFile>().Single(w=>w.Name=="README.txt").Encoding);
    Check(nodes.All(n=>n.CreatedAt==now),"created time mismatch");
    var display=TreeOperations.Render(root);
    foreach(var row in source) Check(display.Contains(row.Name),"missing name in tree");
    foreach(var token in new[]{"頁數: 15","頁數: 5","1920x1080","UTF-8","ASCII","├──","└──"}) Check(display.Contains(token),"missing "+token);
});
Test("T14 invalid search input", () =>
{
    foreach(var ext in new[]{""," ",".","*.txt","a/b","a.b","a b"})
        Throws<ArgumentException>(()=>TreeOperations.SearchByExtension(root,ext,TextWriter.Null));
});
Console.WriteLine($"RESULT {passed} passed; {failed} failed");
return failed==0 ? 0 : 1;
record SourceFile(string Name,string Path,long Value,string Unit);
