using CloudFileManager.Core.Application;
using CloudFileManager.Core.Application.Sessions;
using CloudFileManager.Core.Application.Sorting;
using CloudFileManager.Core.Application.Traversal;
using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Domain.Values;
using CloudFileManager.Core.Domain.Visiting;

int passed=0,failed=0;
var now=DateTimeOffset.Parse("2025-01-01T00:00:00Z");
DirectoryNode Root(string name="root")=>DirectoryNode.CreateRoot(name,now);
void Check(bool ok,string message="assertion"){if(!ok)throw new Exception(message);}
void Throws<T>(Action a) where T:Exception {try{a();}catch(T){return;}throw new Exception("Expected "+typeof(T).Name);}
void Test(string name,Action body,bool initialize=true)
{
    // Singleton cases are intentionally serial. Clean state before and after each case.
    try{if(initialize)FileSystemSession.Instance.Reset(Root("test"));body();passed++;Console.WriteLine("PASS "+name);}
    catch(Exception e){failed++;Console.WriteLine("FAIL "+name+": "+e);}
    finally{FileSystemSession.Instance.Reset(Root("cleanup"));}
}
Test("A01 singleton identity private construction explicit initialization",()=>{
    var s=FileSystemSession.Instance;Check(ReferenceEquals(s,FileSystemSession.Instance));Check(!s.IsInitialized);
    Check(typeof(FileSystemSession).IsSealed&&typeof(FileSystemSession).GetConstructors().Length==0);
    Throws<InvalidOperationException>(()=>{_ = s.Root;});Throws<InvalidOperationException>(()=>s.Undo());Throws<InvalidOperationException>(()=>{_ = s.HasClipboard;});
},false);
Test("A02 typed Accept dispatch is one node, traversal owns DFS",()=>{
    var r=Root();var d=r.AddDirectory("d",now);d.AddWord("w",1,1,now);r.AddImage("i",2,2,3,now);r.AddText("t",3,"UTF-8",now);
    var v=new RecordingVisitor();r.Accept(v);Check(v.Items.SequenceEqual(new[]{"D:root"}));
    v=new RecordingVisitor();using var log=new StringWriter();FileSystemTraversal.Visit(r,v,log);
    Check(v.Items.SequenceEqual(new[]{"D:root","D:d","W:w","I:i","T:t"}));
    Check(log.ToString().Split(Environment.NewLine,StringSplitOptions.RemoveEmptyEntries).SequenceEqual(new[]{"Visiting: root","Visiting: root/d","Visiting: root/d/w","Visiting: root/i","Visiting: root/t"}));
});
Test("A03 production size visitor all types and repeated call isolation",()=>{
    var r=Root();r.AddWord("w",3,1,now);r.AddImage("i",5,1,1,now);r.AddDirectory("d",now).AddText("t",7,"ASCII",now);
    for(int i=0;i<2;i++){Check(TreeOperations.CalculateTotalSize(r,TextWriter.Null)==15);Check(SizeSortStrategy.Size(r)==15);}
    var empty=Root("empty");Check(TreeOperations.CalculateTotalSize(empty,TextWriter.Null)==0);
    var v=new SizeVisitor();FileSystemTraversal.Visit(r,v);Check(v.TotalBytes==15);
});
Test("A04 production search visitor type filtering normalization readonly",()=>{
    var r=Root();r.AddDirectory("dir.txt",now).AddText("t.TXT",1,"UTF-8",now);r.AddImage("i.txt",2,1,1,now);r.AddWord("w.txt",3,1,now);
    string[] expected={"root/dir.txt/t.TXT","root/i.txt","root/w.txt"};
    for(int i=0;i<2;i++)Check(TreeOperations.SearchByExtension(r," .TxT ",TextWriter.Null).SequenceEqual(expected));
    Check(TreeOperations.SearchByExtension(r,"zip",TextWriter.Null).Count==0);
    var v=new ExtensionSearchVisitor("txt");FileSystemTraversal.Visit(r,v);Check(v.Paths.SequenceEqual(expected));
    Throws<NotSupportedException>(()=>((IList<string>)v.Paths).Clear());
});
Test("A05 visitor deep traversal and checked overflow",()=>{
    var r=Root();var last=r;for(int i=0;i<2000;i++)last=last.AddDirectory("d",now);last.AddText("leaf.txt",9,"UTF-8",now);
    Check(SizeSortStrategy.Size(r)==9);Check(TreeOperations.SearchByExtension(r,"txt",TextWriter.Null).Count==1);
    var over=Root();over.AddText("one",long.MaxValue,"UTF-8",now);over.AddText("two",1,"UTF-8",now);
    Throws<OverflowException>(()=>TreeOperations.CalculateTotalSize(over,TextWriter.Null));Throws<OverflowException>(()=>SizeSortStrategy.Size(over));
});
Test("A06 invalid visitor/search arguments do not visit",()=>{
    var r=Root();var v=new RecordingVisitor();Throws<ArgumentNullException>(()=>r.Accept(null!));Throws<ArgumentNullException>(()=>FileSystemTraversal.Visit(null!,v));
    Throws<ArgumentNullException>(()=>FileSystemTraversal.Visit(r,null!));Check(v.Items.Count==0);
    using var log=new StringWriter();Throws<ArgumentException>(()=>TreeOperations.SearchByExtension(r,"*.txt",log));Check(log.ToString()=="");
});
Test("A07 reset replaces Root clears populated clipboard undo redo",()=>{
    var r=Root("old");var f=r.AddText("f",1,"UTF-8",now);var s=FileSystemSession.Instance;s.Reset(r);
    s.AddTag(f,TagKind.Work);s.AddTag(f,TagKind.Personal);s.Undo();s.Copy(f);
    Check(s.UndoCount==1&&s.RedoCount==1&&s.HasClipboard);
    var next=Root("new");s.Reset(next);Check(ReferenceEquals(s.Root,next));Check(ReferenceEquals(s,FileSystemSession.Instance));
    Check(!s.HasClipboard&&s.UndoCount==0&&s.RedoCount==0);Check(!s.Undo()&&!s.Redo());
    Throws<InvalidOperationException>(()=>s.Paste(next));Check(f.Tags.Contains(TagKind.Work),"Reset mutated old domain");
    Throws<InvalidOperationException>(()=>s.Copy(f));Throws<InvalidOperationException>(()=>s.Delete(f));
});
Test("A08 invalid Reset preserves root clipboard and both histories",()=>{
    var r=Root();var f=r.AddText("f",1,"UTF-8",now);var child=r.AddDirectory("child",now);var s=FileSystemSession.Instance;s.Reset(r);
    s.AddTag(f,TagKind.Work);s.AddTag(f,TagKind.Personal);s.Undo();s.Copy(f);
    Throws<ArgumentNullException>(()=>s.Reset(null!));Throws<ArgumentException>(()=>s.Reset(child));
    Check(ReferenceEquals(s.Root,r)&&s.HasClipboard&&s.UndoCount==1&&s.RedoCount==1);Check(s.Redo());Check(f.Tags.Contains(TagKind.Personal));
    var pasted=s.Paste(child);Check(pasted.Tags.SequenceEqual(new[]{TagKind.Work}),"clipboard snapshot was lost");
});
Test("A09 same Root Reset cleans session without clearing domain",()=>{
    var r=Root();var f=r.AddText("f",2,"ASCII",now);var s=FileSystemSession.Instance;s.Reset(r);s.AddTag(f,TagKind.Work);s.Copy(f);
    s.Reset(r);Check(ReferenceEquals(s.Root,r)&&f.Tags.Contains(TagKind.Work));Check(s.UndoCount==0&&!s.HasClipboard&&!s.Undo());
});
Test("A10 shared references use one Command history and preserve failure/noop",()=>{
    var r=Root();var f=r.AddText("f",1,"UTF-8",now);var a=FileSystemSession.Instance;a.Reset(r);var b=FileSystemSession.Instance;
    a.AddTag(f,TagKind.Work);b.AddTag(f,TagKind.Personal);a.Undo();b.Copy(f);
    Check(a.HasClipboard&&a.UndoCount==1&&b.RedoCount==1);Check(!a.AddTag(f,TagKind.Work));
    Throws<ArgumentException>(()=>b.Paste(r));Check(a.RedoCount==1&&a.UndoCount==1);Check(b.Redo());Check(f.Tags.Count==2);
    a.Delete(f);Check(r.Children.Count==0);b.Undo();Check(ReferenceEquals(r.Children[0],f));
});
Test("A11 Reset does not alter independent EditingSession",()=>{
    var other=Root("isolated");var f=other.AddText("f",1,"UTF-8",now);var local=new EditingSession(other);local.AddTag(f,TagKind.Work);local.Copy(f);
    var global=FileSystemSession.Instance;global.Reset(Root("g1"));global.Reset(Root("g2"));
    Check(local.HasClipboard&&local.UndoCount==1);local.Undo();Check(f.Tags.Count==0&&local.RedoCount==1);Check(global.UndoCount==0);
});
Test("A12 serial Reset cycles prevent previous case state leaks",()=>{
    var s=FileSystemSession.Instance;for(int i=0;i<10;i++){
        var r=Root("r"+i);var f=r.AddText("f",i,"UTF-8",now);s.Reset(r);Check(!s.HasClipboard&&s.UndoCount==0&&s.RedoCount==0);
        s.AddTag(f,TagKind.Urgent);s.Copy(f);s.Delete(f);s.Undo();Check(s.UndoCount==1&&s.RedoCount==1);
    }
});
Console.WriteLine($"RESULT {passed} passed; {failed} failed");
var layerFailures = LayerVerification.Run();
return failed==0 && layerFailures==0 ? 0 : 1;
sealed class RecordingVisitor:IFileSystemVisitor
{
    public List<string> Items {get;}=[];
    public void Visit(DirectoryNode n)=>Items.Add("D:"+n.Name);
    public void Visit(WordFile n)=>Items.Add("W:"+n.Name);
    public void Visit(ImageFile n)=>Items.Add("I:"+n.Name);
    public void Visit(TextFile n)=>Items.Add("T:"+n.Name);
}
