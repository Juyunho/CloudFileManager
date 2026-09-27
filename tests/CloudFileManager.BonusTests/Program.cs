using CloudFileManager.Core.Application;
using CloudFileManager.Core.Application.Formatting;
using CloudFileManager.Core.Application.Samples;
using CloudFileManager.Core.Application.Sessions;
using CloudFileManager.Core.Application.Sorting;
using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Domain.Values;
using System.Text.Json;

var now = DateTimeOffset.Parse("2025-01-01T12:30:00+08:00");
int passed = 0, failed = 0;
void Check(bool ok, string message = "assertion") { if (!ok) throw new Exception(message); }
void Throws<T>(Action a) where T : Exception { try { a(); } catch (T) { return; } throw new Exception("Expected " + typeof(T).Name); }
void Test(string name, Action body)
{ try { body(); passed++; Console.WriteLine("PASS " + name); } catch (Exception e) { failed++; Console.WriteLine("FAIL " + name + ": " + e); } }
DirectoryNode Root() => DirectoryNode.CreateRoot("root", now);
FsNode[] Walk(FsNode root)
{
    var list = new List<FsNode>(); var q = new Queue<FsNode>(); q.Enqueue(root);
    while (q.TryDequeue(out var n)) { list.Add(n); foreach (var c in n.Children) q.Enqueue(c); }
    return list.ToArray();
}
string State(FsNode root, bool ids = true) => JsonSerializer.Serialize(Walk(root).Select(n => new
{ Id = ids ? n.Id : Guid.Empty, Kind = n.GetType().Name, n.Name, n.CreatedAt, Details = NodeDetailsFormatter.Format(n),
  Alias = (n as DirectoryNode)?.XmlAlias, n.Tags, Children = n.Children.Select(c => c.Name).ToArray() }));
void Order(IReadOnlyList<FsNode> actual, params string[] names) => Check(actual.Select(n => n.Name).SequenceEqual(names), "order: " + string.Join(",",actual.Select(n=>n.Name)));

Test("B01 Name stable Asc/Desc directories first", () => {
    var r=Root();r.AddText("z.txt",1,"UTF-8",now);r.AddDirectory("b",now);r.AddDirectory("A",now);r.AddDirectory("a",now);
    r.AddText("A.txt",2,"UTF-8",now);r.AddText("a.txt",3,"UTF-8",now);
    Order(SortedView.Children(r,new NameSortStrategy(),SortDirection.Asc),"A","a","b","A.txt","a.txt","z.txt");
    Order(SortedView.Children(r,new NameSortStrategy(),SortDirection.Desc),"b","A","a","z.txt","A.txt","a.txt");
});

Test("B02 Size subtree keys and stable ties", () => {
    var r=Root();r.AddText("z",7,"UTF-8",now);var a=r.AddDirectory("large",now);a.AddDirectory("nested",now).AddText("x",11,"UTF-8",now);
    r.AddDirectory("empty",now);r.AddDirectory("tied",now).AddText("y",11,"UTF-8",now);r.AddText("a",7,"UTF-8",now);r.AddText("small",1,"UTF-8",now);
    Order(SortedView.Children(r,new SizeSortStrategy(),SortDirection.Asc),"empty","large","tied","small","z","a");
    Order(SortedView.Children(r,new SizeSortStrategy(),SortDirection.Desc),"large","tied","empty","z","a","small");
});
Test("B03 Extension empty directories and case-insensitive stability", () => {
    var r=Root();r.AddText("z.TXT",2,"UTF-8",now);r.AddDirectory("z.dir",now);r.AddDirectory("a.dir",now);
    r.AddText("a.txt",2,"UTF-8",now);r.AddWord("w.DOCX",1,1,now);r.AddText("bare",0,"UTF-8",now);
    Order(SortedView.Children(r,new ExtensionSortStrategy(),SortDirection.Asc),"z.dir","a.dir","bare","w.DOCX","z.TXT","a.txt");
    Order(SortedView.Children(r,new ExtensionSortStrategy(),SortDirection.Desc),"z.dir","a.dir","z.TXT","a.txt","w.DOCX","bare");
});
Test("B04 sorting and Tags leave baseline XML traversal intact", () => {
    var r=SampleTree.Create();var before=State(r);var xml=TreeOperations.ToXml(r);using var l1=new StringWriter();TreeOperations.CalculateTotalSize(r,l1);
    var s=new EditingSession(r);s.AddTag(r.Children[0],TagKind.Work);
    using var output=new StringWriter();var old=Console.Out;
    try { Console.SetOut(output); foreach(var strategy in new INodeSortStrategy[]{new NameSortStrategy(),new SizeSortStrategy(),new ExtensionSortStrategy()})
        foreach(var dir in Enum.GetValues<SortDirection>()) SortedView.Children(r,strategy,dir); }
    finally { Console.SetOut(old); }
    Check(output.ToString()=="","sorting leaked traversal logs");Check(TreeOperations.ToXml(r)==xml,"tags/sort changed XML");
    using var l2=new StringWriter();TreeOperations.CalculateTotalSize(r,l2);Check(l1.ToString()==l2.ToString());
    using var l3=new StringWriter();TreeOperations.SearchByExtension(r,"docx",l3);Check(l1.ToString()==l3.ToString());
    s.Undo();Check(State(r)==before,"children or domain changed");
});
Test("B05 sorting overflow invalid direction and readonly result", () => {
    var r=Root();r.AddDirectory("huge",now).AddText("a",long.MaxValue,"UTF-8",now);
    ((DirectoryNode)r.Children[0]).AddText("b",1,"UTF-8",now);var before=State(r);
    Throws<OverflowException>(()=>SortedView.Children(r,new SizeSortStrategy(),SortDirection.Asc));Check(before==State(r));
    Throws<ArgumentOutOfRangeException>(()=>SortedView.Children(r,new NameSortStrategy(),(SortDirection)9));
    Throws<NotSupportedException>(()=>((IList<FsNode>)SortedView.Children(r,new NameSortStrategy(),SortDirection.Asc)).Clear());
});
Test("B06 Delete full subtree and exact original position/identity Undo Redo", () => {
    var r=Root();r.AddText("first",1,"UTF-8",now);var d=r.AddDirectory("d",now,"alias");var w=d.AddWord("w",4,8,now);r.AddText("last",2,"UTF-8",now);
    var s=new EditingSession(r);s.AddTag(w,TagKind.Urgent);s.AddTag(d,TagKind.Work);var before=State(r);var index=r.Children.ToList().IndexOf(d);
    s.Delete(d);Check(!r.Children.Contains(d));Check(s.Undo());Check(before==State(r));Check(ReferenceEquals(r.Children[index],d));
    Check(s.Redo());Check(!r.Children.Contains(d));Check(s.Undo());Check(before==State(r));
});
Test("B07 root foreign detached invalid operations preserve state/history", () => {
    var r=Root();var d=r.AddDirectory("d",now);var s=new EditingSession(r);s.Delete(d);var state=State(r);var u=s.UndoCount;
    Throws<InvalidOperationException>(()=>s.Delete(r));Throws<InvalidOperationException>(()=>s.Copy(d));Throws<InvalidOperationException>(()=>s.AddTag(d,TagKind.Work));
    Throws<InvalidOperationException>(()=>s.Delete(Root()));Throws<InvalidOperationException>(()=>s.Paste(r));
    Check(State(r)==state && s.UndoCount==u && s.RedoCount==0);
});
Test("B08 Copy full metadata Tags snapshot after source modifications/deletion", () => {
    var r=Root();var src=r.AddDirectory("src",now,"xml-alias");src.AddWord("w.docx",10,3,now);src.AddImage("i.png",20,64,32,now);src.AddText("t.txt",30,"UTF-16",now);
    src.AddDirectory("nested",now).AddText("n",4,"ASCII",now);var dst=r.AddDirectory("dst",now);var s=new EditingSession(r);
    s.AddTag(src,TagKind.Work);s.AddTag(src.Children[0],TagKind.Personal);var expected=State(src,false);var originals=Walk(src).Select(n=>n.Id).ToHashSet();
    s.Copy(src);s.RemoveTag(src,TagKind.Work);s.Delete(src.Children[2]);s.Delete(src);var copy=s.Paste(dst);
    Check(State(copy,false)==expected,"snapshot lost values");Check(Walk(copy).All(n=>!originals.Contains(n.Id)),"reused source IDs");
    Check(Walk(copy).Select(n=>n.Id).Distinct().Count()==Walk(copy).Length);
    foreach(var n in Walk(copy))foreach(var child in n.Children)Check(ReferenceEquals(child.Parent,n));
    Check(ReferenceEquals(copy.Parent,dst));
});
Test("B09 each Paste independent and snapshot immutable", () => {
    var r=Root();var src=r.AddDirectory("src",now);src.AddText("a",1,"UTF-8",now);var x=r.AddDirectory("x",now);var y=r.AddDirectory("y",now);var z=r.AddDirectory("z",now);
    var s=new EditingSession(r);s.Copy(src);var a=(DirectoryNode)s.Paste(x);s.AddTag(a.Children[0],TagKind.Urgent);s.Delete(a.Children[0]);
    var b=s.Paste(y);var c=s.Paste(z);Check(State(src,false)==State(b,false));Check(State(b,false)==State(c,false));Check(b.Id!=c.Id);
});
Test("B10 conflicting Paste atomic and preserves pending Redo", () => {
    var r=Root();var a=r.AddDirectory("a",now);a.AddText("x",1,"UTF-8",now);var dst=r.AddDirectory("dst",now);dst.AddDirectory("a",now);
    var s=new EditingSession(r);s.AddTag(a,TagKind.Work);s.Undo();s.Copy(a);var before=State(r);var u=s.UndoCount;var re=s.RedoCount;
    Throws<ArgumentException>(()=>s.Paste(dst));Check(before==State(r));Check(s.UndoCount==u&&s.RedoCount==re);Check(s.Redo());Check(a.Tags.Contains(TagKind.Work));
});
Test("B11 Paste Undo Redo retains copy ID index metadata", () => {
    var r=Root();var src=r.AddText("source",5,"ASCII",now);var dst=r.AddDirectory("dst",now);dst.AddText("first",0,"UTF-8",now);
    var s=new EditingSession(r);s.Copy(src);var pasted=s.Paste(dst);var before=State(dst);s.Undo();Check(dst.Children.Count==1);
    s.Redo();Check(State(dst)==before);Check(ReferenceEquals(dst.Children[1],pasted));
});
Test("B12 Tag catalog multi-tag directory/file readonly and exact Undo", () => {
    var r=Root();var f=r.AddText("f",1,"UTF-8",now);var s=new EditingSession(r);
    Check(TagCatalog.Color(TagKind.Urgent)=="紅"&&TagCatalog.Color(TagKind.Work)=="藍"&&TagCatalog.Color(TagKind.Personal)=="綠");
    foreach(var n in new FsNode[]{r,f})foreach(var tag in Enum.GetValues<TagKind>())Check(s.AddTag(n,tag));
    Check(r.Tags.Count==3&&f.Tags.Count==3);var before=State(r);Check(s.RemoveTag(f,TagKind.Work));s.Undo();Check(State(r)==before);s.Redo();Check(!f.Tags.Contains(TagKind.Work));
    Throws<NotSupportedException>(()=>((IList<TagKind>)f.Tags).Clear());Throws<ArgumentOutOfRangeException>(()=>s.AddTag(f,(TagKind)99));
});
Test("B13 no-op failed Copy and sorting preserve Redo and entry counts", () => {
    var r=Root();var f=r.AddText("f",1,"UTF-8",now);var s=new EditingSession(r);s.AddTag(f,TagKind.Work);s.AddTag(f,TagKind.Personal);s.Undo();
    var u=s.UndoCount;var re=s.RedoCount;var before=State(r);
    Check(!s.AddTag(f,TagKind.Work));Check(!s.RemoveTag(f,TagKind.Urgent));s.Copy(f);SortedView.Children(r,new NameSortStrategy(),SortDirection.Desc);
    Throws<ArgumentException>(()=>s.Paste(r));Throws<InvalidOperationException>(()=>s.Copy(Root()));
    Check(s.UndoCount==u&&s.RedoCount==re&&before==State(r));Check(s.Redo());Check(f.Tags.Contains(TagKind.Personal));
});
Test("B14 each successful new edit clears Redo", () => {
    for(var kind=0;kind<4;kind++){
        var r=Root();var f=r.AddText("f",1,"UTF-8",now);var dst=r.AddDirectory("dst",now);var s=new EditingSession(r);s.AddTag(f,TagKind.Work);s.AddTag(f,TagKind.Personal);s.Undo();s.Copy(f);
        switch(kind){case 0:s.Delete(f);break;case 1:s.Paste(dst);break;case 2:s.AddTag(f,TagKind.Urgent);break;case 3:s.RemoveTag(f,TagKind.Work);break;}
        Check(s.RedoCount==0&&s.UndoCount==2);Check(!s.Redo());
    }
});
Test("B15 mixed operations Undo all Redo all exact states", () => {
    var r=Root();var f=r.AddText("f",3,"UTF-8",now);var dst=r.AddDirectory("dst",now);var s=new EditingSession(r);var before=State(r);
    s.AddTag(f,TagKind.Urgent);s.Copy(f);var copy=s.Paste(dst);s.RemoveTag(copy,TagKind.Urgent);s.Delete(f);var after=State(r);
    for(int i=0;i<4;i++)Check(s.Undo());Check(State(r)==before);Check(!s.Undo());
    for(int i=0;i<4;i++)Check(s.Redo());Check(State(r)==after);Check(!s.Redo());
});
Test("B16 sessions isolate clipboard/history and reject stale tree", () => {
    var a=Root();var f=a.AddText("f",1,"UTF-8",now);var b=Root();var x=new EditingSession(a);var y=new EditingSession(b);x.Copy(f);
    Check(!y.HasClipboard&&y.UndoCount==0);Throws<InvalidOperationException>(()=>y.Paste(b));
    var other=new EditingSession(a);other.AddTag(f,TagKind.Work);Throws<InvalidOperationException>(()=>x.Delete(f));Check(x.UndoCount==0);
    var fresh=new EditingSession(a);a.AddText("outside",1,"UTF-8",now);Throws<InvalidOperationException>(()=>fresh.Undo());
});
Test("B17 deep subtree snapshot Paste Delete Undo iterative", () => {
    var r=Root();var source=r.AddDirectory("source",now);var last=source;for(int i=0;i<2000;i++)last=last.AddDirectory("d",now);last.AddText("leaf",7,"UTF-8",now);
    var dst=r.AddDirectory("dst",now);var s=new EditingSession(r);s.Copy(source);s.Delete(source);var copy=s.Paste(dst);Check(Walk(copy).Length==2002);Check(SizeSortStrategy.Size(copy)==7);
    s.Undo();s.Undo();Check(ReferenceEquals(r.Children[0],source));
});
Test("B18 empty operations and empty directory copy", () => {
    var r=Root();var empty=r.AddDirectory("empty",now);var dst=r.AddDirectory("dst",now);var s=new EditingSession(r);
    Check(!s.Undo()&&!s.Redo());s.Copy(empty);var p=s.Paste(dst);Check(p.Children.Count==0);s.Delete(p);s.Undo();Check(ReferenceEquals(dst.Children[0],p));
});
Test("B19 Copy into descendant finite snapshot without cycles", () => {
    var r=Root();var source=r.AddDirectory("source",now);var child=source.AddDirectory("child",now);source.AddText("leaf",2,"UTF-8",now);
    var s=new EditingSession(r);s.Copy(source);var clone=s.Paste(child);Check(Walk(clone).Length==3);Check(Walk(r).Length==7);Check(Walk(r).Select(n=>n.Id).Distinct().Count()==7);
});
Test("B20 baseline Ordinal uniqueness not changed by sorting policy", () => {
    var r=Root();var f=r.AddText("a.txt",1,"UTF-8",now);var dst=r.AddDirectory("dst",now);dst.AddText("A.txt",1,"UTF-8",now);
    var s=new EditingSession(r);s.Copy(f);s.Paste(dst);Check(dst.Children.Count==2);
    Order(SortedView.Children(dst,new NameSortStrategy(),SortDirection.Desc),"A.txt","a.txt");
});
Console.WriteLine($"RESULT {passed} passed; {failed} failed");
return failed==0 ? 0 : 1;
