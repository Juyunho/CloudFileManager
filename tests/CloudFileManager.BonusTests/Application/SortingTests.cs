using CloudFileManager.Core.Application;
using CloudFileManager.Core.Application.Formatting;
using CloudFileManager.Core.Application.Samples;
using CloudFileManager.Core.Application.Sessions;
using CloudFileManager.Core.Application.Sorting;
using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Domain.Values;
using System.Text.Json;

namespace CloudFileManager.BonusTests;

public sealed class SortingTests : BonusFixture
{
    [Fact(DisplayName = "B01 Name stable Asc/Desc directories first")]
    [Trait("LegacyId", "B01")]
    [Trait("Category", "Unit")]
    public void B01()
    {
        var r=Root();r.AddText("z.txt",1,"UTF-8",now);r.AddDirectory("b",now);r.AddDirectory("A",now);r.AddDirectory("a",now);
        r.AddText("A.txt",2,"UTF-8",now);r.AddText("a.txt",3,"UTF-8",now);
        Order(SortedView.Children(r,new NameSortStrategy(),SortDirection.Asc),"A","a","b","A.txt","a.txt","z.txt");
        Order(SortedView.Children(r,new NameSortStrategy(),SortDirection.Desc),"b","A","a","z.txt","A.txt","a.txt");
    }

    [Fact(DisplayName = "B02 Size subtree keys and stable ties")]
    [Trait("LegacyId", "B02")]
    [Trait("Category", "Unit")]
    public void B02()
    {
        var r=Root();r.AddText("z",7,"UTF-8",now);var a=r.AddDirectory("large",now);a.AddDirectory("nested",now).AddText("x",11,"UTF-8",now);
        r.AddDirectory("empty",now);r.AddDirectory("tied",now).AddText("y",11,"UTF-8",now);r.AddText("a",7,"UTF-8",now);r.AddText("small",1,"UTF-8",now);
        Order(SortedView.Children(r,new SizeSortStrategy(),SortDirection.Asc),"empty","large","tied","small","z","a");
        Order(SortedView.Children(r,new SizeSortStrategy(),SortDirection.Desc),"large","tied","empty","z","a","small");
    }

    [Fact(DisplayName = "B03 Extension empty directories and case-insensitive stability")]
    [Trait("LegacyId", "B03")]
    [Trait("Category", "Unit")]
    public void B03()
    {
        var r=Root();r.AddText("z.TXT",2,"UTF-8",now);r.AddDirectory("z.dir",now);r.AddDirectory("a.dir",now);
        r.AddText("a.txt",2,"UTF-8",now);r.AddWord("w.DOCX",1,1,now);r.AddText("bare",0,"UTF-8",now);
        Order(SortedView.Children(r,new ExtensionSortStrategy(),SortDirection.Asc),"z.dir","a.dir","bare","w.DOCX","z.TXT","a.txt");
        Order(SortedView.Children(r,new ExtensionSortStrategy(),SortDirection.Desc),"z.dir","a.dir","z.TXT","a.txt","w.DOCX","bare");
    }

    [Fact(DisplayName = "B04 sorting and Tags leave baseline XML traversal intact")]
    [Trait("LegacyId", "B04")]
    [Trait("Category", "Integration")]
    public void B04()
    {
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
    }

    [Fact(DisplayName = "B05 sorting overflow invalid direction and readonly result")]
    [Trait("LegacyId", "B05")]
    [Trait("Category", "Unit")]
    public void B05()
    {
        var r=Root();r.AddDirectory("huge",now).AddText("a",long.MaxValue,"UTF-8",now);
        ((DirectoryNode)r.Children[0]).AddText("b",1,"UTF-8",now);var before=State(r);
        Throws<OverflowException>(()=>SortedView.Children(r,new SizeSortStrategy(),SortDirection.Asc));Check(before==State(r));
        Throws<ArgumentOutOfRangeException>(()=>SortedView.Children(r,new NameSortStrategy(),(SortDirection)9));
        Throws<NotSupportedException>(()=>((IList<FsNode>)SortedView.Children(r,new NameSortStrategy(),SortDirection.Asc)).Clear());
    }

    [Fact(DisplayName = "B20 baseline Ordinal uniqueness not changed by sorting policy")]
    [Trait("LegacyId", "B20")]
    [Trait("Category", "Unit")]
    public void B20()
    {
        var r=Root();var f=r.AddText("a.txt",1,"UTF-8",now);var dst=r.AddDirectory("dst",now);dst.AddText("A.txt",1,"UTF-8",now);
        var s=new EditingSession(r);s.Copy(f);s.Paste(dst);Check(dst.Children.Count==2);
        Order(SortedView.Children(dst,new NameSortStrategy(),SortDirection.Desc),"A.txt","a.txt");
    }
}
