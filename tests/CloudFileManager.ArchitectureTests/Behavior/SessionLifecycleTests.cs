using CloudFileManager.Core.Application;
using CloudFileManager.Core.Application.Sessions;
using CloudFileManager.Core.Application.Sorting;
using CloudFileManager.Core.Application.Traversal;
using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Domain.Values;
using CloudFileManager.Core.Domain.Visiting;

namespace CloudFileManager.ArchitectureTests;

public sealed class SessionLifecycleTests : SessionFixture
{
    [Fact(DisplayName = "A01 singleton identity private construction explicit initialization")]
    [Trait("LegacyId", "A01")]
    [Trait("Category", "Integration")]
    public async Task A01()
    {
        var start = new System.Diagnostics.ProcessStartInfo(Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet")
        { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
        start.ArgumentList.Add(Path.Combine(AppContext.BaseDirectory, "ColdStartProbeAssets", "ColdStartProbe.dll"));
        using var process = System.Diagnostics.Process.Start(start);
        Assert.NotNull(process);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(30));
        var stdout = process.StandardOutput.ReadToEndAsync(timeout.Token);
        var stderr = process.StandardError.ReadToEndAsync(timeout.Token);
        try { await process.WaitForExitAsync(timeout.Token); }
        catch (OperationCanceledException) { process.Kill(entireProcessTree: true); await process.WaitForExitAsync(CancellationToken.None); throw; }
        var text = await stdout;
        var errors = await stderr;
        Assert.True(process.ExitCode == 0, $"Cold-start probe exit {process.ExitCode}: {errors}");
        using var report = System.Text.Json.JsonDocument.Parse(text);
        foreach (var condition in new[] { "sameInstance", "uninitialized", "sealedPrivate", "rootThrows", "undoThrows", "clipboardThrows" })
            Assert.True(report.RootElement.GetProperty(condition).GetBoolean(), "Cold-start contract: " + condition);
    }

    [Fact(DisplayName = "A07 reset replaces Root clears populated clipboard undo redo")]
    [Trait("LegacyId", "A07")]
    [Trait("Category", "Integration")]
    public void A07()
    {
        var r=Root("old");var f=r.AddText("f",1,"UTF-8",now);var s=FileSystemSession.Instance;s.Reset(r);
        s.AddTag(f,TagKind.Work);s.AddTag(f,TagKind.Personal);s.Undo();s.Copy(f);
        Check(s.UndoCount==1&&s.RedoCount==1&&s.HasClipboard);
        var next=Root("new");s.Reset(next);Check(ReferenceEquals(s.Root,next));Check(ReferenceEquals(s,FileSystemSession.Instance));
        Check(!s.HasClipboard&&s.UndoCount==0&&s.RedoCount==0);Check(!s.Undo()&&!s.Redo());
        Throws<InvalidOperationException>(()=>s.Paste(next));Check(f.Tags.Contains(TagKind.Work),"Reset mutated old domain");
        Throws<InvalidOperationException>(()=>s.Copy(f));Throws<InvalidOperationException>(()=>s.Delete(f));
    }

    [Fact(DisplayName = "A08 invalid Reset preserves root clipboard and both histories")]
    [Trait("LegacyId", "A08")]
    [Trait("Category", "Integration")]
    public void A08()
    {
        var r=Root();var f=r.AddText("f",1,"UTF-8",now);var child=r.AddDirectory("child",now);var s=FileSystemSession.Instance;s.Reset(r);
        s.AddTag(f,TagKind.Work);s.AddTag(f,TagKind.Personal);s.Undo();s.Copy(f);
        Throws<ArgumentNullException>(()=>s.Reset(null!));Throws<ArgumentException>(()=>s.Reset(child));
        Check(ReferenceEquals(s.Root,r)&&s.HasClipboard&&s.UndoCount==1&&s.RedoCount==1);Check(s.Redo());Check(f.Tags.Contains(TagKind.Personal));
        var pasted=s.Paste(child);Check(pasted.Tags.SequenceEqual(new[]{TagKind.Work}),"clipboard snapshot was lost");
    }

    [Fact(DisplayName = "A09 same Root Reset cleans session without clearing domain")]
    [Trait("LegacyId", "A09")]
    [Trait("Category", "Integration")]
    public void A09()
    {
        var r=Root();var f=r.AddText("f",2,"ASCII",now);var s=FileSystemSession.Instance;s.Reset(r);s.AddTag(f,TagKind.Work);s.Copy(f);
        s.Reset(r);Check(ReferenceEquals(s.Root,r)&&f.Tags.Contains(TagKind.Work));Check(s.UndoCount==0&&!s.HasClipboard&&!s.Undo());
    }

    [Fact(DisplayName = "A10 shared references use one Command history and preserve failure/noop")]
    [Trait("LegacyId", "A10")]
    [Trait("Category", "Integration")]
    public void A10()
    {
        var r=Root();var f=r.AddText("f",1,"UTF-8",now);var a=FileSystemSession.Instance;a.Reset(r);var b=FileSystemSession.Instance;
        a.AddTag(f,TagKind.Work);b.AddTag(f,TagKind.Personal);a.Undo();b.Copy(f);
        Check(a.HasClipboard&&a.UndoCount==1&&b.RedoCount==1);Check(!a.AddTag(f,TagKind.Work));
        Throws<ArgumentException>(()=>b.Paste(r));Check(a.RedoCount==1&&a.UndoCount==1);Check(b.Redo());Check(f.Tags.Count==2);
        a.Delete(f);Check(r.Children.Count==0);b.Undo();Check(ReferenceEquals(r.Children[0],f));
    }

    [Fact(DisplayName = "A11 Reset does not alter independent EditingSession")]
    [Trait("LegacyId", "A11")]
    [Trait("Category", "Integration")]
    public void A11()
    {
        var other=Root("isolated");var f=other.AddText("f",1,"UTF-8",now);var local=new EditingSession(other);local.AddTag(f,TagKind.Work);local.Copy(f);
        var global=FileSystemSession.Instance;global.Reset(Root("g1"));global.Reset(Root("g2"));
        Check(local.HasClipboard&&local.UndoCount==1);local.Undo();Check(f.Tags.Count==0&&local.RedoCount==1);Check(global.UndoCount==0);
    }

    [Fact(DisplayName = "A12 serial Reset cycles prevent previous case state leaks")]
    [Trait("LegacyId", "A12")]
    [Trait("Category", "Integration")]
    public void A12()
    {
        var s=FileSystemSession.Instance;for(int i=0;i<10;i++){
            var r=Root("r"+i);var f=r.AddText("f",i,"UTF-8",now);s.Reset(r);Check(!s.HasClipboard&&s.UndoCount==0&&s.RedoCount==0);
            s.AddTag(f,TagKind.Urgent);s.Copy(f);s.Delete(f);s.Undo();Check(s.UndoCount==1&&s.RedoCount==1);
        }
    }
}
