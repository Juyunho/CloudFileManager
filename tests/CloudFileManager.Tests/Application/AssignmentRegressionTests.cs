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

public sealed class AssignmentRegressionTests : AssignmentFixture
{
    private readonly ITestOutputHelper output;
    public AssignmentRegressionTests(ITestOutputHelper output) => this.output = output;
    [Fact(DisplayName = "T01 independent source oracle and exact file set")]
    [Trait("LegacyId", "T01")]
    [Trait("Category", "Integration")]
    public void T01()
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
            output.WriteLine($"SOURCE {row.Name}: {row.Value} {row.Unit} => {bytes} B");
        }
        var actual = TreeOperations.CalculateTotalSize(root, TextWriter.Null);
        output.WriteLine($"INDEPENDENT SUM = {expected} B; PRODUCT = {actual} B");
        Equal(expected, new BigInteger(actual));
    }

    [Fact(DisplayName = "T02 each subtree independently grouped from source paths")]
    [Trait("LegacyId", "T02")]
    [Trait("Category", "Integration")]
    public void T02()
    {
        foreach (var dir in Flatten(root).OfType<DirectoryNode>())
        {
            var expected = source.Where(r => r.Path.StartsWith(dir.FullPath + "/", StringComparison.Ordinal))
                .Aggregate(BigInteger.Zero, (sum, r) => sum + Oracle(r));
            Equal(expected, new BigInteger(TreeOperations.CalculateTotalSize(dir, TextWriter.Null)));
            output.WriteLine($"SUBTREE {dir.Name} => {expected} B");
        }
    }

    [Fact(DisplayName = "T03 binary units and byte precision")]
    [Trait("LegacyId", "T03")]
    [Trait("Category", "Integration")]
    public void T03()
    {
        Equal(1L << 10, BinarySize.From(1,"KB")); Equal(1L << 20, BinarySize.From(1,"MB"));
        Equal(500L, BinarySize.From(500,"B")); Equal("500B", BinarySizeFormatter.Format(500)); Equal("0B",BinarySizeFormatter.Format(0));
        Equal("1025B",BinarySizeFormatter.Format(1025)); Equal("1KB",BinarySizeFormatter.Format(1024)); Equal("1MB",BinarySizeFormatter.Format(1L<<20));
        Throws<ArgumentException>(() => BinarySize.From(1,"GB")); Throws<ArgumentOutOfRangeException>(() => BinarySize.From(-1,"B"));
    }

    [Fact(DisplayName = "T04 search normalization scope and no result")]
    [Trait("LegacyId", "T04")]
    [Trait("Category", "Integration")]
    public void T04()
    {
        var expected=source.Where(r => r.Name.EndsWith(".docx",StringComparison.Ordinal)).Select(r=>r.Path).ToArray();
        foreach(var ext in new[]{".docx","DOCX"," .DoCx "})
            Check(expected.SequenceEqual(TreeOperations.SearchByExtension(root,ext,TextWriter.Null)),"search paths mismatch");
        var archive=Flatten(root).OfType<DirectoryNode>().Single(d=>d.Name.StartsWith("2025備份"));
        Equal(1,TreeOperations.SearchByExtension(archive,"docx",TextWriter.Null).Count);
        Equal(source.Single(r=>r.Name=="舊會議記錄.docx").Path,TreeOperations.SearchByExtension(archive,"docx",TextWriter.Null)[0]);
        Equal(0,TreeOperations.SearchByExtension(root,"zip",TextWriter.Null).Count);
    }

    [Fact(DisplayName = "T05 exact traversal sequence for both operations")]
    [Trait("LegacyId", "T05")]
    [Trait("Category", "Integration")]
    public void T05()
    {
        string[] order = ["根目錄 (Root)","根目錄 (Root)/專案文件 (Project_Docs)",
            source[0].Path, source[1].Path,"根目錄 (Root)/個人筆記 (Personal_Notes)",source[2].Path,
            "根目錄 (Root)/個人筆記 (Personal_Notes)/2025備份 (Archive_2025)",source[3].Path,source[4].Path];
        using var sizeLog=new StringWriter(); using var searchLog=new StringWriter();
        TreeOperations.CalculateTotalSize(root,sizeLog); TreeOperations.SearchByExtension(root,"docx",searchLog);
        var expected=order.Select(p=>"Visiting: "+p);
        Check(expected.SequenceEqual(sizeLog.ToString().Split(Environment.NewLine,StringSplitOptions.RemoveEmptyEntries)),"size traversal");
        Check(expected.SequenceEqual(searchLog.ToString().Split(Environment.NewLine,StringSplitOptions.RemoveEmptyEntries)),"search traversal");
    }

    [Fact(DisplayName = "T06 XML matches original assignment fixture")]
    [Trait("LegacyId", "T06")]
    [Trait("Category", "Integration")]
    public void T06()
    {
        var expected=XDocument.Load(Path.Combine(AppContext.BaseDirectory,"Fixtures/expected.xml"));
        var actual=XDocument.Parse(TreeOperations.ToXml(root));
        Check(XNode.DeepEquals(expected,actual),"XML differs from original assignment");
    }

    [Fact(DisplayName = "T07 XML invalid names collisions and escaped text")]
    [Trait("LegacyId", "T07")]
    [Trait("Category", "Integration")]
    public void T07()
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
    }

    [Fact(DisplayName = "T08 empty directory and zero byte file")]
    [Trait("LegacyId", "T08")]
    [Trait("Category", "Integration")]
    public void T08()
    {
        var r=Fresh(); using var log=new StringWriter();
        Equal(0L,TreeOperations.CalculateTotalSize(r,log)); Equal("Visiting: root"+Environment.NewLine,log.ToString());
        Equal(0,TreeOperations.SearchByExtension(r,"txt",TextWriter.Null).Count);
        r.AddText("empty.txt",0,"UTF-8",now); Equal(0L,TreeOperations.CalculateTotalSize(r,TextWriter.Null));
        Equal(1,TreeOperations.SearchByExtension(r,"txt",TextWriter.Null).Count);
    }

    [Fact(DisplayName = "T11 deep hierarchy without recursive call stack")]
    [Trait("LegacyId", "T11")]
    [Trait("Category", "Integration")]
    public void T11()
    {
        var r=Fresh(); var last=r;
        for(var i=0;i<2000;i++) last=last.AddDirectory("d",now);
        last.AddText("deep.txt",7,"UTF-8",now);
        Equal(7L,TreeOperations.CalculateTotalSize(r,TextWriter.Null));
        Equal(1,TreeOperations.SearchByExtension(r,"txt",TextWriter.Null).Count);
        var xml=TreeOperations.ToXml(r);Check(xml.Contains("deep_txt"),"missing deep XML leaf");
    }

    [Fact(DisplayName = "T12 checked arithmetic prevents silent overflow")]
    [Trait("LegacyId", "T12")]
    [Trait("Category", "Integration")]
    public void T12()
    {
        Throws<OverflowException>(()=>BinarySize.From(long.MaxValue,"KB"));
        var r=Fresh();r.AddText("one",long.MaxValue,"UTF-8",now);r.AddText("two",1,"UTF-8",now);
        Throws<OverflowException>(()=>TreeOperations.CalculateTotalSize(r,TextWriter.Null));
    }

    [Fact(DisplayName = "T13 metadata and rendered details")]
    [Trait("LegacyId", "T13")]
    [Trait("Category", "Integration")]
    public void T13()
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
    }

    [Fact(DisplayName = "T14 invalid search input")]
    [Trait("LegacyId", "T14")]
    [Trait("Category", "Integration")]
    public void T14()
    {
        foreach(var ext in new[]{""," ",".","*.txt","a/b","a.b","a b"})
            Throws<ArgumentException>(()=>TreeOperations.SearchByExtension(root,ext,TextWriter.Null));
    }
}
