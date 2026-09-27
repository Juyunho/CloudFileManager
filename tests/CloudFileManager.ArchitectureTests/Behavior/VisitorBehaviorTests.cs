using CloudFileManager.Core.Application;
using CloudFileManager.Core.Application.Sessions;
using CloudFileManager.Core.Application.Sorting;
using CloudFileManager.Core.Application.Traversal;
using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Domain.Values;
using CloudFileManager.Core.Domain.Visiting;

namespace CloudFileManager.ArchitectureTests;

public sealed class VisitorBehaviorTests : SessionFixture
{
    [Fact(DisplayName = "A02 typed Accept dispatch is one node, traversal owns DFS")]
    [Trait("LegacyId", "A02")]
    [Trait("Category", "Integration")]
    public void A02()
    {
        var r=Root();var d=r.AddDirectory("d",now);d.AddWord("w",1,1,now);r.AddImage("i",2,2,3,now);r.AddText("t",3,"UTF-8",now);
        var v=new RecordingVisitor();r.Accept(v);Check(v.Items.SequenceEqual(new[]{"D:root"}));
        v=new RecordingVisitor();using var log=new StringWriter();FileSystemTraversal.Visit(r,v,log);
        Check(v.Items.SequenceEqual(new[]{"D:root","D:d","W:w","I:i","T:t"}));
        Check(log.ToString().Split(Environment.NewLine,StringSplitOptions.RemoveEmptyEntries).SequenceEqual(new[]{"Visiting: root","Visiting: root/d","Visiting: root/d/w","Visiting: root/i","Visiting: root/t"}));
    }

    [Fact(DisplayName = "A03 production size visitor all types and repeated call isolation")]
    [Trait("LegacyId", "A03")]
    [Trait("Category", "Integration")]
    public void A03()
    {
        var r=Root();r.AddWord("w",3,1,now);r.AddImage("i",5,1,1,now);r.AddDirectory("d",now).AddText("t",7,"ASCII",now);
        for(int i=0;i<2;i++){Check(TreeOperations.CalculateTotalSize(r,TextWriter.Null)==15);Check(SizeSortStrategy.Size(r)==15);}
        var empty=Root("empty");Check(TreeOperations.CalculateTotalSize(empty,TextWriter.Null)==0);
        var v=new SizeVisitor();FileSystemTraversal.Visit(r,v);Check(v.TotalBytes==15);
    }

    [Fact(DisplayName = "A04 production search visitor type filtering normalization readonly")]
    [Trait("LegacyId", "A04")]
    [Trait("Category", "Integration")]
    public void A04()
    {
        var r=Root();r.AddDirectory("dir.txt",now).AddText("t.TXT",1,"UTF-8",now);r.AddImage("i.txt",2,1,1,now);r.AddWord("w.txt",3,1,now);
        string[] expected={"root/dir.txt/t.TXT","root/i.txt","root/w.txt"};
        for(int i=0;i<2;i++)Check(TreeOperations.SearchByExtension(r," .TxT ",TextWriter.Null).SequenceEqual(expected));
        Check(TreeOperations.SearchByExtension(r,"zip",TextWriter.Null).Count==0);
        var v=new ExtensionSearchVisitor("txt");FileSystemTraversal.Visit(r,v);Check(v.Paths.SequenceEqual(expected));
        Throws<NotSupportedException>(()=>((IList<string>)v.Paths).Clear());
    }

    [Fact(DisplayName = "A05 visitor deep traversal and checked overflow")]
    [Trait("LegacyId", "A05")]
    [Trait("Category", "Integration")]
    public void A05()
    {
        var r=Root();var last=r;for(int i=0;i<2000;i++)last=last.AddDirectory("d",now);last.AddText("leaf.txt",9,"UTF-8",now);
        Check(SizeSortStrategy.Size(r)==9);Check(TreeOperations.SearchByExtension(r,"txt",TextWriter.Null).Count==1);
        var over=Root();over.AddText("one",long.MaxValue,"UTF-8",now);over.AddText("two",1,"UTF-8",now);
        Throws<OverflowException>(()=>TreeOperations.CalculateTotalSize(over,TextWriter.Null));Throws<OverflowException>(()=>SizeSortStrategy.Size(over));
    }

    [Fact(DisplayName = "A06 invalid visitor/search arguments do not visit")]
    [Trait("LegacyId", "A06")]
    [Trait("Category", "Integration")]
    public void A06()
    {
        var r=Root();var v=new RecordingVisitor();Throws<ArgumentNullException>(()=>r.Accept(null!));Throws<ArgumentNullException>(()=>FileSystemTraversal.Visit(null!,v));
        Throws<ArgumentNullException>(()=>FileSystemTraversal.Visit(r,null!));Check(v.Items.Count==0);
        using var log=new StringWriter();Throws<ArgumentException>(()=>TreeOperations.SearchByExtension(r,"*.txt",log));Check(log.ToString()=="");
    }
}
