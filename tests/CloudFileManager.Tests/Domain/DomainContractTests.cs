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

public sealed class DomainContractTests : AssignmentFixture
{
    private readonly ITestOutputHelper output;
    public DomainContractTests(ITestOutputHelper output) => this.output = output;
    [Fact(DisplayName = "T09 invalid creation is rejected without mutation")]
    [Trait("LegacyId", "T09")]
    [Trait("Category", "Unit")]
    public void T09()
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
    }

    [Fact(DisplayName = "T10 topology and public ownership contract")]
    [Trait("LegacyId", "T10")]
    [Trait("Category", "Unit")]
    public void T10()
    {
        foreach(var type in new[]{typeof(WordFile),typeof(ImageFile),typeof(TextFile)}) Equal(0,type.GetConstructors().Length);
        Equal<MethodInfo?>(null,typeof(FsNode).GetProperty("Parent")!.SetMethod);
        Check(!typeof(DirectoryNode).GetMethods().Any(m=>m.GetParameters().Any(a=>typeof(FsNode).IsAssignableFrom(a.ParameterType))),"public attach/reparent API");
        var nodes=Flatten(root); Equal(9,nodes.Count); Equal(9,nodes.Select(n=>n.Id).Distinct().Count());
        foreach(var d in nodes.OfType<DirectoryNode>()) foreach(var c in d.Children) Check(ReferenceEquals(d,c.Parent),"parent mismatch");
        Throws<NotSupportedException>(()=>((IList<FsNode>)root.Children).Clear());
        Equal(9,Flatten(root).Count);
    }
}
