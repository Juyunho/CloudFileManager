using CloudFileManager.Core.Application;
using CloudFileManager.Core.Application.Samples;
using CloudFileManager.Core.Application.Sessions;
using CloudFileManager.Core.Application.Sorting;
using CloudFileManager.Core.Application.Traversal;
using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Domain.Values;
using CloudFileManager.Core.Domain.Visiting;
using CloudFileManager.Web;
using System.Text.Json;
using System.Xml.Linq;

namespace CloudFileManager.WebTests;

public abstract class WebWorkspaceContractTests
{
    private static void Check(bool value, string? message = null,
        [System.Runtime.CompilerServices.CallerArgumentExpression(nameof(value))] string? expression = null)
        => Assert.True(value, message ?? expression);
    private static void Equal<T>(T expected, T actual) => Assert.Equal(expected, actual);
    private static void Throws<T>(Action action) where T : Exception => Assert.ThrowsAny<T>(action);
    JsonElement Json(object value) => JsonSerializer.SerializeToElement(value);
    JsonElement State(WebWorkspace w) => Json(w.State());
    JsonElement Act(WebWorkspace w, string action, string? id=null,string? tag=null,string? criterion=null,string? ext=null) => Json(w.Execute(new(action,id,tag,criterion,ext),_=>{}));
    string Id(WebWorkspace w,string name)=>State(w).GetProperty("rows").EnumerateArray().Single(x=>x.GetProperty("name").GetString()==name).GetProperty("id").GetString()!;
    void Select(WebWorkspace w,string name)=>Act(w,"select",Id(w,name));
    int Count(WebWorkspace w,string tag)=>State(w).GetProperty("counts").GetProperty(tag).GetInt32();
    Guid[] Matches(WebWorkspace w)=>State(w).GetProperty("rows").EnumerateArray().Where(x=>x.GetProperty("searchMatch").GetBoolean()).Select(x=>x.GetProperty("id").GetGuid()).Order().ToArray();

    protected abstract IFileSystemSession Session { get; }
    private WebWorkspace CreateWorkspace()
    {
        var project = Session.Root.Children.Single(n => n.Name == "專案文件");
        return new WebWorkspace(Session, project.Children.Single(n => n.Name == "API介面定義.docx").Id);
    }
    [Fact(DisplayName = "W01 clean seed")]
    [Trait("LegacyId", "W01")]
    [Trait("Category", "Integration")]
    public void W01()
    {
        var w=CreateWorkspace();var s=State(w);Check(s.GetProperty("rows").GetArrayLength()==10);Check(s.GetProperty("logs").GetArrayLength()==0);Check(s.GetProperty("undoCount").GetInt32()==0&&s.GetProperty("redoCount").GetInt32()==0);Check(!s.GetProperty("canPaste").GetBoolean());Check(s.GetProperty("progress").GetProperty("status").GetString()=="idle");Check(s.GetProperty("criterion").GetString()=="Size"&&s.GetProperty("direction").GetString()=="Asc");Check(Count(w,"Personal")==2&&Count(w,"Work")==1&&Count(w,"Urgent")==0);
    }

    [Fact(DisplayName = "W02 exact capacities and display")]
    [Trait("LegacyId", "W02")]
    [Trait("Category", "Integration")]
    public void W02()
    {
        var w=CreateWorkspace();var readme=State(w).GetProperty("rows").EnumerateArray().Single(x=>x.GetProperty("name").GetString()=="README.txt");Check(readme.GetProperty("sizeBytes").GetInt64()==500&&readme.GetProperty("details").GetString()!.StartsWith("0.5 KB"));Select(w,"README.txt");var s=Act(w,"size").GetProperty("state");Check(s.GetProperty("logs")[0].GetProperty("Text").GetString()!.Contains("500 B"));Check(s.GetProperty("progress").GetProperty("total").GetInt32()==1);
    }

    [Fact(DisplayName = "W03 sort controls do not alter tree/history")]
    [Trait("LegacyId", "W03")]
    [Trait("Category", "Integration")]
    public void W03()
    {
        var w=CreateWorkspace();var root=Session.Root;var order=root.Children.Select(x=>x.Id).ToArray();Act(w,"sort",criterion:"Size");Check(State(w).GetProperty("direction").GetString()=="Desc");Act(w,"sort",criterion:"Tag");Check(State(w).GetProperty("direction").GetString()=="Asc");Check(root.Children.Select(x=>x.Id).SequenceEqual(order));Check(State(w).GetProperty("undoCount").GetInt32()==0);
    }

    [Fact(DisplayName = "W04 tag key stable directions")]
    [Trait("LegacyId", "W04")]
    [Trait("Category", "Unit")]
    public void W04()
    {
        var at=DateTimeOffset.UtcNow;var root=DirectoryNode.CreateRoot("r",at);var a=root.AddText("a",1,"x",at);var b=root.AddText("b",1,"x",at);var c=root.AddText("c",1,"x",at);var d=root.AddText("d",1,"x",at);var e=root.AddText("e",1,"x",at);var edit=new EditingSession(root);edit.AddTag(a,TagKind.Urgent);edit.AddTag(a,TagKind.Personal);edit.AddTag(b,TagKind.Personal);edit.AddTag(c,TagKind.Urgent);var strategy=new TagSortStrategy();Check(strategy.Order(root.Children,SortDirection.Asc).SequenceEqual(new FsNode[]{a,c,b,d,e}));Check(strategy.Order(root.Children,SortDirection.Desc).SequenceEqual(new FsNode[]{b,a,c,d,e}));
    }

    [Fact(DisplayName = "W05 tag noop retains redo/log")]
    [Trait("LegacyId", "W05")]
    [Trait("Category", "Integration")]
    public void W05()
    {
        var w=CreateWorkspace();Act(w,"addTag",tag:"Work");Act(w,"undo");var before=State(w);Act(w,"addTag",tag:"Personal");var after=State(w);Check(after.GetProperty("redoCount").GetInt32()==1);Check(before.GetProperty("logs").GetArrayLength()==after.GetProperty("logs").GetArrayLength());
    }

    [Fact(DisplayName = "W06 badge remove does not change selection or descendants")]
    [Trait("LegacyId", "W06")]
    [Trait("Category", "Integration")]
    public void W06()
    {
        var w=CreateWorkspace();var selected=State(w).GetProperty("selected").GetGuid();Act(w,"removeTag",Id(w,"2025備份"),"Personal");Check(State(w).GetProperty("selected").GetGuid()==selected);Check(Count(w,"Personal")==1&&Count(w,"Work")==1);Act(w,"undo");Check(Count(w,"Personal")==2);
    }

    [Fact(DisplayName = "W07 Paste file disabled/conflict atomic/redo")]
    [Trait("LegacyId", "W07")]
    [Trait("Category", "Integration")]
    public void W07()
    {
        var w=CreateWorkspace();Act(w,"copy");Check(!State(w).GetProperty("canPaste").GetBoolean());Check(Act(w,"paste").TryGetProperty("error",out _));Select(w,"專案文件");Check(State(w).GetProperty("canPaste").GetBoolean());Act(w,"addTag",tag:"Work");Act(w,"undo");var before=TreeOperations.ToXml(Session.Root);Check(Act(w,"paste").TryGetProperty("error",out _));Check(TreeOperations.ToXml(Session.Root)==before);Check(State(w).GetProperty("redoCount").GetInt32()==1);
    }

    [Fact(DisplayName = "W08 Prototype snapshot independent and paste selection")]
    [Trait("LegacyId", "W08")]
    [Trait("Category", "Integration")]
    public void W08()
    {
        var w=CreateWorkspace();Act(w,"copy");Act(w,"addTag",tag:"Urgent");Select(w,"個人筆記");var dest=State(w).GetProperty("selected").GetGuid();Act(w,"paste");Check(State(w).GetProperty("selected").GetGuid()==dest);var root=Session.Root;var copy=root.Children.Single(n=>n.Name=="個人筆記").Children.Single(n=>n.Name=="API介面定義.docx");Check(!copy.Tags.Contains(TagKind.Urgent)&&copy.Tags.Contains(TagKind.Personal));Check(Count(w,"Personal")==3);
    }

    [Fact(DisplayName = "W09 delete undo selection and global count")]
    [Trait("LegacyId", "W09")]
    [Trait("Category", "Integration")]
    public void W09()
    {
        var w=CreateWorkspace();Select(w,"個人筆記");Act(w,"delete");Check(State(w).GetProperty("selected").GetGuid()==Session.Root.Id);Check(Count(w,"Personal")==1&&Count(w,"Work")==0);Act(w,"undo");Check(State(w).GetProperty("selected").GetGuid()==Session.Root.Id);Check(Count(w,"Personal")==2&&Count(w,"Work")==1);
    }

    [Fact(DisplayName = "W10 ancestor fallback on Undo Paste")]
    [Trait("LegacyId", "W10")]
    [Trait("Category", "Integration")]
    public void W10()
    {
        var w=CreateWorkspace();Select(w,"2025備份");Act(w,"copy");Select(w,"專案文件");var dest=State(w).GetProperty("selected").GetGuid();Act(w,"paste");var parent=Session.Root.Children.Single(n=>n.Name=="專案文件");var copy=parent.Children.Single(n=>n.Name=="2025備份");Act(w,"select",copy.Children[0].Id.ToString());Act(w,"undo");Check(State(w).GetProperty("selected").GetGuid()==dest);
    }

    [Fact(DisplayName = "W11 visitor observed actual sequence and scope")]
    [Trait("LegacyId", "W11")]
    [Trait("Category", "Integration")]
    public void W11()
    {
        var root=ReferenceTree.Create();var events=new List<TraversalProgress>();var source=new TraversalProgressSource();source.Progressed+=events.Add;var v=new SizeVisitor();FileSystemTraversal.Visit(root,v,progress:source);Check(events.Count==10);Check(events.Select(e=>e.Visited).SequenceEqual(Enumerable.Range(1,10)));Check(events.All(e=>e.Total==10));Check(v.TotalBytes==(200L+1+120+500+2048)*1024+500);var w=CreateWorkspace();var emissions=new List<object>();w.Execute(new("size"),emissions.Add);Check(Json(emissions[0]).GetProperty("progress").GetProperty("total").GetInt32()==1);
    }

    [Fact(DisplayName = "W12 XML regression contract and file scope")]
    [Trait("LegacyId", "W12")]
    [Trait("Category", "Integration")]
    public void W12()
    {
        Check(XNode.DeepEquals(XElement.Parse(TreeOperations.ToXml(SampleTree.Create())), XElement.Load(Path.Combine(AppContext.BaseDirectory, "Fixtures/expected.xml"))));var w=CreateWorkspace();var result=Act(w,"xml");var xml=result.GetProperty("download").GetProperty("content").GetString()!;Check(XElement.Parse(xml).Name.LocalName=="API介面定義_docx");Check(!State(w).GetProperty("logs")[0].GetProperty("Text").GetString()!.Contains("<"));Check(State(w).GetProperty("undoCount").GetInt32()==0);
    }

    [Fact(DisplayName = "W13 exact extension within scope, no filter")]
    [Trait("LegacyId", "W13")]
    [Trait("Category", "Integration")]
    public void W13()
    {
        var w=CreateWorkspace();Act(w,"search",ext:"DOCX");var state=State(w);Check(state.GetProperty("searchSummary").GetString()=="找到 1 項");Check(state.GetProperty("rows").GetArrayLength()==10);Act(w,"search",ext:"doc");Check(State(w).GetProperty("searchSummary").GetString()=="找到 0 項");
    }

    [Fact(DisplayName = "W14 invalid search never fake scan")]
    [Trait("LegacyId", "W14")]
    [Trait("Category", "Integration")]
    public void W14()
    {
        var w=CreateWorkspace();Check(Act(w,"search",ext:"*.docx").TryGetProperty("error",out _));Check(State(w).GetProperty("progress").GetProperty("status").GetString()=="idle");Check(State(w).GetProperty("undoCount").GetInt32()==0);
    }

    [Fact(DisplayName = "W15 Size distinguishes formatted equality")]
    [Trait("LegacyId", "W15")]
    [Trait("Category", "Unit")]
    public void W15()
    {
        var at=DateTimeOffset.UtcNow;var r=DirectoryNode.CreateRoot("r",at);var a=r.AddText("512",512,"x",at);var b=r.AddText("500",500,"x",at);Check(SortedView.Children(r,new SizeSortStrategy(),SortDirection.Asc).SequenceEqual(new FsNode[]{b,a}));
    }

    [Fact(DisplayName = "W16 search trace exact production visit order")]
    [Trait("LegacyId", "W16")]
    [Trait("Category", "Integration")]
    public void W16()
    {
        var w=CreateWorkspace();Select(w,"我的根目錄");var events=new List<object>();w.Execute(new("search",Extension:".docx"),events.Add);var logs=State(w).GetProperty("logs").EnumerateArray().ToArray();var traces=logs.Where(x=>x.GetProperty("Kind").GetString()=="Trace").Select(x=>x.GetProperty("Text").GetString()).ToArray();Check(traces.SequenceEqual(new[]{"搜尋目錄: 我的根目錄","搜尋目錄: 個人筆記","搜尋目錄: 2025備份","掃描檔案: 會議記錄.docx","掃描檔案: 待辦清單.txt","搜尋目錄: 專案文件","掃描檔案: API介面定義.docx","掃描檔案: 需求規格書.docx","掃描檔案: 系統架構圖.png","掃描檔案: README.txt"}));Check(logs.Count(x=>x.GetProperty("Kind").GetString()=="Match")==3);Check(logs[^1].GetProperty("Text").GetString()=="找到 3 項");var stream=events.Select(Json).ToArray();Check(stream[0].GetProperty("type").GetString()=="searchReset");Check(stream.Count(x=>x.GetProperty("type").GetString()=="match")==3);Check(State(w).GetProperty("progress").GetProperty("name").GetString()=="README.txt");Check(State(w).GetProperty("progress").GetProperty("visited").GetInt32()==10);
    }

    [Fact(DisplayName = "W17 case insensitive replacement zero and identity sort")]
    [Trait("LegacyId", "W17")]
    [Trait("Category", "Integration")]
    public void W17()
    {
        var w=CreateWorkspace();Select(w,"我的根目錄");var id=State(w).GetProperty("selected").GetGuid();Act(w,"search",ext:".docx");var first=Matches(w);Check(first.Length==3);Act(w,"search",ext:".DOCX");Check(Matches(w).SequenceEqual(first));Act(w,"sort",criterion:"Name");Check(Matches(w).SequenceEqual(first));Act(w,"search",ext:".png");Check(Matches(w).Length==1&&!first.Contains(Matches(w)[0]));Act(w,"search",ext:".unknown");Check(Matches(w).Length==0);Check(State(w).GetProperty("selected").GetGuid()==id&&State(w).GetProperty("rows").GetArrayLength()==10);
    }

    [Fact(DisplayName = "W18 highlight mutation removal never resurrects stale ids")]
    [Trait("LegacyId", "W18")]
    [Trait("Category", "Integration")]
    public void W18()
    {
        var w=CreateWorkspace();Select(w,"我的根目錄");Act(w,"search",ext:"docx");Select(w,"API介面定義.docx");Act(w,"delete");Check(Matches(w).Length==2);Act(w,"undo");Check(Matches(w).Length==2);Act(w,"redo");Check(Matches(w).Length==2);
    }

    [Fact(DisplayName = "W19 search preserves domain history redo and last progress")]
    [Trait("LegacyId", "W19")]
    [Trait("Category", "Integration")]
    public void W19()
    {
        var w=CreateWorkspace();Act(w,"addTag",tag:"Urgent");Act(w,"undo");var root=Session.Root;var xml=TreeOperations.ToXml(root);Act(w,"search",ext:"docx");Check(State(w).GetProperty("redoCount").GetInt32()==1&&State(w).GetProperty("undoCount").GetInt32()==0);Check(TreeOperations.ToXml(root)==xml&&Count(w,"Urgent")==0);var before=State(w).GetProperty("progress").GetRawText();Select(w,"README.txt");Act(w,"sort",criterion:"Tag");Check(State(w).GetProperty("progress").GetRawText()==before);Act(w,"search",ext:"docx");Check(State(w).GetProperty("progress").GetProperty("total").GetInt32()==1&&Matches(w).Length==0);
    }

    [Fact(DisplayName = "W20 directory extension never matches")]
    [Trait("LegacyId", "W20")]
    [Trait("Category", "Integration")]
    public void W20()
    {
        var at=DateTimeOffset.UtcNow;var root=DirectoryNode.CreateRoot("fake.docx",at);var file=root.AddText("actual.DOCX",1,"UTF8",at);var visitor=new ExtensionSearchVisitor(".docx");var events=new List<TraversalProgress>();var source=new TraversalProgressSource();source.Progressed+=events.Add;FileSystemTraversal.Visit(root,visitor,progress:source);Check(visitor.MatchedNodeIds.SequenceEqual(new[]{file.Id}));Check(events.Count==2&&events[0].NodeId==root.Id);
    }
}
