using System.Reflection;
using System.Text.Json;
using CloudFileManager.Core.Application.Persistence;
using CloudFileManager.Core.Application.Samples;
using CloudFileManager.Core.Application.Sessions;
using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Domain.Values;
using CloudFileManager.Infrastructure;
using CloudFileManager.Web;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace CloudFileManager.PersistenceTests;

public sealed class PersistenceTests : IDisposable
{
    private readonly string directory = Path.Combine(Path.GetTempPath(), "cfm-persistence-" + Guid.NewGuid());
    private string Database => Path.Combine(directory, "filesystem.db");
    private static FileSystemDocument Seed() => FileSystemDocumentMapper.Capture(ReferenceTree.Create());
    private static string Fingerprint(DirectoryNode root) => JsonSerializer.Serialize(FileSystemDocumentMapper.Capture(root));
    private static string Fingerprint(FileSystemDocument doc) => JsonSerializer.Serialize(doc);
    private SqliteFileSystemStore Open() => new(Database, timeoutSeconds: 1);
    private SqliteConnection Connection()
    {
        var c = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = Database, ForeignKeys = true, Pooling = false }.ToString()); c.Open(); return c;
    }
    private void Sql(string text)
    { using var c = Connection(); using var cmd = c.CreateCommand(); cmd.CommandText = text; cmd.ExecuteNonQuery(); }
    private static long Revision(DirectoryNode root) => (long)typeof(FsNode).GetProperty("State", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(root)!.GetType().GetField("Revision", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(typeof(FsNode).GetProperty("State", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(root))!;
    private (DirectoryNode, EditingSession) Session(IFileSystemStore store)
    {
        var loaded = store.LoadOrInitialize(Seed()); var root = FileSystemDocumentMapper.Restore(loaded.Document);
        return (root, new EditingSession(root, store, loaded.Revision));
    }
    private void FailWrites() => Sql("CREATE TRIGGER fail_write BEFORE INSERT ON node WHEN NEW.kind='Text' BEGIN SELECT RAISE(ABORT,'injected real SQL failure'); END;");
    private void AllowWrites() => Sql("DROP TRIGGER fail_write;");

    [Fact] public void P01_SeedOnceAndRoundtripAllMetadataTagsIdsAndOrder()
    {
        var at = new DateTimeOffset(2025, 2, 3, 4, 5, 6, TimeSpan.FromHours(8));
        var root = DirectoryNode.CreateRoot("根", at, "Root_Alias"); var d = root.AddDirectory("Z", at, "Dir_Alias");
        d.AddWord("α.docx", long.MaxValue, 12, at); d.AddImage("圖.png", 42, 1920, 1080, at); d.AddText("A.txt", 500, "UTF-8", at);
        var edit = new EditingSession(root); edit.AddTag(d, TagKind.Work); edit.AddTag(d, TagKind.Personal);
        var seed = FileSystemDocumentMapper.Capture(root);
        using (var store = Open()) Assert.Equal(Fingerprint(seed), Fingerprint(store.LoadOrInitialize(seed).Document));
        using (var store = Open())
        {
            var loaded = store.LoadOrInitialize(Seed()); Assert.Equal(0, loaded.Revision);
            Assert.Equal(Fingerprint(root), Fingerprint(FileSystemDocumentMapper.Restore(loaded.Document)));
        }
    }
    [Fact] public void P02_DeletedAllChildrenStayEmptyAfterRestart()
    {
        Guid id;
        using (var store = Open()) { var (r, e) = Session(store); id = r.Id; foreach (var n in r.Children.ToArray()) e.Delete(n); Assert.Equal(3, e.DurableRevision); }
        using (var store = Open()) { var (r, e) = Session(store); Assert.Equal(id, r.Id); Assert.Empty(r.Children); Assert.Equal(0, e.UndoCount); Assert.False(e.HasClipboard); }
    }
    [Fact] public void P03_CopySnapshotPasteDeleteUndoRedoAreDurable()
    {
        using var store = Open(); var (r, e) = Session(store); var source = r.Children[0]; var destination = (DirectoryNode)r.Children[1];
        e.Copy(source); e.AddTag(source, TagKind.Urgent); var copy = e.Paste(destination);
        Assert.NotEqual(source.Id, copy.Id); Assert.DoesNotContain(TagKind.Urgent, copy.Tags);
        Assert.Equal(Fingerprint(r), Fingerprint(store.LoadOrInitialize(Seed()).Document));
        e.Delete(source); Assert.True(e.Undo()); Assert.Same(source, r.Children[0]); Assert.True(e.Redo()); Assert.DoesNotContain(source, r.Children);
        Assert.Equal(Fingerprint(r), Fingerprint(store.LoadOrInitialize(Seed()).Document));
        var reloaded = FileSystemDocumentMapper.Restore(store.LoadOrInitialize(Seed()).Document);
        Assert.Contains(reloaded.Children[0].Children, x => x.Id == copy.Id);
    }
    [Theory] [InlineData("delete")] [InlineData("paste")] [InlineData("tag")]
    public void P04_ConfirmedRollbackPreservesEverythingAndRetry(string operation)
    {
        using var store = Open(); var (r, e) = Session(store); var source = r.Children[0]; var destination = (DirectoryNode)r.Children[1];
        e.AddTag(r, TagKind.Work); e.AddTag(r, TagKind.Urgent); e.Undo(); e.Copy(source);
        var before = Fingerprint(r); var revision = Revision(r); var durable = e.DurableRevision;
        void Mutate() { if (operation == "delete") e.Delete(source); else if (operation == "paste") e.Paste(destination); else e.AddTag(source, TagKind.Personal); }
        FailWrites(); var error = Assert.Throws<PersistenceException>(Mutate); Assert.Equal(PersistenceOutcome.ConfirmedNotCommitted, error.Outcome);
        Assert.Equal(before, Fingerprint(r)); Assert.Equal(revision, Revision(r)); Assert.Equal(durable, e.DurableRevision);
        Assert.Equal(1, e.UndoCount); Assert.Equal(1, e.RedoCount); Assert.True(e.HasClipboard); Assert.Same(source, r.Children[0]);
        Assert.Equal(before, Fingerprint(store.LoadOrInitialize(Seed()).Document));
        AllowWrites(); Assert.True(e.Redo()); Assert.Contains(TagKind.Urgent, r.Tags); Assert.True(e.Undo());
        Mutate(); Assert.Equal(0, e.RedoCount); Assert.Equal(2, e.UndoCount);
        if (operation != "paste") { var copied = e.Paste(destination); Assert.Equal(source.Name, copied.Name); Assert.DoesNotContain(TagKind.Urgent, copied.Tags); }
    }
    [Theory] [InlineData(false)] [InlineData(true)]
    public void P05_FailedUndoRedoPreservesStacksRevisionAndCanRetry(bool redo)
    {
        using var store = Open(); var (r, e) = Session(store); var node = r.Children[0]; e.Delete(node); if (redo) e.Undo();
        var before = Fingerprint(r); var revision = Revision(r); var u = e.UndoCount; var d = e.RedoCount;
        FailWrites(); Assert.Throws<PersistenceException>(() => { if (redo) e.Redo(); else e.Undo(); });
        Assert.Equal(before, Fingerprint(r)); Assert.Equal(revision, Revision(r)); Assert.Equal(u, e.UndoCount); Assert.Equal(d, e.RedoCount);
        Assert.Equal(before, Fingerprint(store.LoadOrInitialize(Seed()).Document)); AllowWrites();
        Assert.True(redo ? e.Redo() : e.Undo()); Assert.Equal(Fingerprint(r), Fingerprint(store.LoadOrInitialize(Seed()).Document));
        if (!redo) Assert.Same(node, r.Children[0]);
    }
    [Fact] public void P06_NoopConflictAndCopyNeverSaveOrClearRedo()
    {
        using var store = Open(); var (r, e) = Session(store); var n = r.Children[0]; e.AddTag(r, TagKind.Work); e.AddTag(r, TagKind.Urgent); e.Undo();
        var revision = e.DurableRevision; e.Copy(n); FailWrites();
        Assert.False(e.AddTag(r, TagKind.Work)); Assert.False(e.RemoveTag(r, TagKind.Personal)); Assert.Throws<ArgumentException>(() => e.Paste(r));
        Assert.Equal(revision, e.DurableRevision); Assert.Equal(1, e.RedoCount); Assert.Equal(1, e.UndoCount);
    }
    [Fact] public void P07_RealWriterLockFailureRestoresAndRetryWorks()
    {
        using var store = Open(); var (r, e) = Session(store); var before = Fingerprint(r);
        using (var c = Connection()) using (var tx = c.BeginTransaction(deferred: false))
        { Assert.Throws<PersistenceException>(() => e.AddTag(r, TagKind.Work)); Assert.Equal(before, Fingerprint(r)); tx.Rollback(); }
        Assert.True(e.AddTag(r, TagKind.Work));
    }
    [Fact] public void P08_DeferredForeignKeyCommitFailureRollsBackMemoryAndDatabase()
    {
        using var store = Open(); var (r, e) = Session(store); var before = Fingerprint(r);
        Sql("CREATE TRIGGER fail_commit AFTER UPDATE ON store_meta BEGIN UPDATE store_meta SET root_id='missing' WHERE singleton_key=1; END;");
        var ex = Assert.Throws<PersistenceException>(() => e.AddTag(r, TagKind.Work)); Assert.Equal(PersistenceOutcome.ConfirmedNotCommitted, ex.Outcome);
        Assert.Equal(before, Fingerprint(r)); Assert.Equal(before, Fingerprint(store.LoadOrInitialize(Seed()).Document));
        Sql("DROP TRIGGER fail_commit;"); Assert.True(e.AddTag(r, TagKind.Work));
    }
    [Theory] [InlineData(false)] [InlineData(true)]
    public void P09_UnknownOutcomeFailsClosedWhetherDatabaseCommittedOrNot(bool actuallyCommit)
    {
        using var real = Open(); var uncertain = new UnknownStore(real, actuallyCommit); var (r, e) = Session(uncertain);
        Assert.Throws<SessionUnavailableException>(() => e.AddTag(r, TagKind.Work));
        Assert.Throws<SessionUnavailableException>(() => e.Delete(r.Children[0])); Assert.Throws<SessionUnavailableException>(() => e.Undo()); Assert.Throws<SessionUnavailableException>(() => e.Copy(r));
        Assert.Equal(actuallyCommit ? 1 : 0, real.LoadOrInitialize(Seed()).Revision);
    }
    private sealed class UnknownStore(IFileSystemStore real, bool commit) : IFileSystemStore
    {
        public PersistedFileSystem LoadOrInitialize(FileSystemDocument seed) => real.LoadOrInitialize(seed);
        public CommitReceipt Commit(FileSystemDocument document, long expectedRevision, Guid operationId)
        { if (commit) real.Commit(document, expectedRevision, operationId); throw new PersistenceException("simulated indeterminate acknowledgement", PersistenceOutcome.Unknown); }
    }
    [Theory] [InlineData("PRAGMA user_version=99;")] [InlineData("UPDATE node SET created_at='invalid';")]
    [InlineData("UPDATE node SET sibling_ordinal=sibling_ordinal+4 WHERE parent_id IS NOT NULL;")]
    [InlineData("DELETE FROM node_tag; DELETE FROM store_meta;")]
    [InlineData("UPDATE node SET width=2147483648 WHERE kind='Image';")]
    public void P10_InvalidDatabaseNeverReseeds(string corrupt)
    {
        using (var store = Open()) store.LoadOrInitialize(Seed()); Sql(corrupt);
        var bytes = File.ReadAllBytes(Database);
        using (var store = Open()) Assert.ThrowsAny<Exception>(() => store.LoadOrInitialize(Seed()));
        Assert.Equal(bytes, File.ReadAllBytes(Database));
    }
    [Fact] public void P11_UnversionedAndCorruptFilesRejectedWithoutOverwrite()
    {
        Directory.CreateDirectory(directory); File.WriteAllText(Database, "not a sqlite database"); var before = File.ReadAllBytes(Database);
        using (var store = Open()) Assert.Throws<SqliteException>(() => store.LoadOrInitialize(Seed())); Assert.Equal(before, File.ReadAllBytes(Database));
        File.Delete(Database); Sql("CREATE TABLE unrelated(value TEXT);");
        using var second = Open(); Assert.Throws<InvalidDataException>(() => second.LoadOrInitialize(Seed()));
    }
    [Fact] public void P12_InvalidSeedLeavesNoInitializedDatabaseAndRetryWorks()
    {
        using var store = Open(); Assert.Throws<ArgumentException>(() => store.LoadOrInitialize(new FileSystemDocument([])));
        Assert.Equal(10, store.LoadOrInitialize(Seed()).Document.Rows.Count);
    }
    [Fact] public void P13_OwnerLockPreventsSecondApplicationAndReleasesOnDispose()
    {
        using (var first = Open()) { first.LoadOrInitialize(Seed()); Assert.Throws<IOException>(() => Open()); }
        using var next = Open(); Assert.Equal(10, next.LoadOrInitialize(Seed()).Document.Rows.Count);
    }
    [Fact] public void P14_DeepTreeAndInvalidTopologyValidation()
    {
        var r = DirectoryNode.CreateRoot("r", DateTimeOffset.UnixEpoch); var n = r;
        for (int i=0;i<2000;i++) n=n.AddDirectory("d",DateTimeOffset.UnixEpoch);
        var document = FileSystemDocumentMapper.Capture(r); Assert.Equal(2001,FileSystemDocumentMapper.Capture(FileSystemDocumentMapper.Restore(document)).Rows.Count);
        var rows = Seed().Rows.ToArray(); rows[1] = rows[1] with { ParentId = rows[1].Id };
        Assert.Throws<ArgumentException>(() => FileSystemDocumentMapper.Restore(new(rows)));
        rows = Seed().Rows.ToArray(); rows[1] = rows[1] with { Id = rows[0].Id };
        Assert.Throws<ArgumentException>(() => FileSystemDocumentMapper.Restore(new(rows)));
    }
    [Fact] public void P15_RealSingletonRestartCleansTransientStateKeepsDurableTree()
    {
        Guid id; string expected;
        using (var store = Open())
        {
            var s=FileSystemSession.Instance; s.Restore(store.LoadOrInitialize(Seed()),store); id=s.Root.Id;
            s.Copy(s.Root.Children[0]); s.AddTag(s.Root,TagKind.Work); s.AddTag(s.Root,TagKind.Urgent); s.Undo(); expected=Fingerprint(s.Root);
        }
        using (var store = Open())
        {
            var s=FileSystemSession.Instance;s.Restore(store.LoadOrInitialize(Seed()),store);
            Assert.Equal(id,s.Root.Id); Assert.Equal(expected,Fingerprint(s.Root)); Assert.Equal(0,s.UndoCount);Assert.Equal(0,s.RedoCount);Assert.False(s.HasClipboard);
            Assert.Equal(3,s.DurableRevision);
        }
    }
    [Fact] public void P16_WebCompositionReloadAndFallbackSelection()
    {
        ServiceProvider Host()
        {
            var services=new ServiceCollection();typeof(WebWorkspace).Assembly.GetType("Program")!.GetMethod("ConfigureServices")!.Invoke(null,[services,Database]);return services.BuildServiceProvider();
        }
        using(var host=Host())
        {
            var w=host.GetRequiredService<WebWorkspace>();Assert.Same(FileSystemSession.Instance,host.GetRequiredService<IFileSystemSession>());
            w.Execute(new("delete"),_=>{});
        }
        using(var host=Host())
        {
            var w=host.GetRequiredService<WebWorkspace>();var state=JsonSerializer.SerializeToElement(w.State());
            Assert.Equal(FileSystemSession.Instance.Root.Id,state.GetProperty("selected").GetGuid());Assert.Equal(9,state.GetProperty("rows").GetArrayLength());
            Assert.Equal(0,state.GetProperty("logs").GetArrayLength());Assert.Equal("idle",state.GetProperty("progress").GetProperty("status").GetString());Assert.Equal(0,state.GetProperty("undoCount").GetInt32());
        }
    }
    [Fact] public void P17_WebFailedSaveOnlyAddsErrorAndPreservesSelectionMatchesHistory()
    {
        using var store=Open();var s=FileSystemSession.Instance;s.Restore(store.LoadOrInitialize(Seed()),store);var w=new WebWorkspace(s,s.Root.Id);
        w.Execute(new("search",Extension:".docx"),_=>{});w.Execute(new("addTag",Tag:"Work"),_=>{});w.Execute(new("addTag",Tag:"Urgent"),_=>{});w.Execute(new("undo"),_=>{});w.Execute(new("copy"),_=>{});
        var before=JsonSerializer.SerializeToElement(w.State());FailWrites();var result=JsonSerializer.SerializeToElement(w.Execute(new("addTag",Tag:"Personal"),_=>{}));Assert.True(result.TryGetProperty("error",out _));var after=result.GetProperty("state");
        foreach(var key in new[]{"rows","selected","undoCount","redoCount","canPaste","progress","searchSummary"})Assert.Equal(before.GetProperty(key).GetRawText(),after.GetProperty(key).GetRawText());
        Assert.Equal(before.GetProperty("logs").GetArrayLength()+1,after.GetProperty("logs").GetArrayLength());Assert.Equal("Error",after.GetProperty("logs").EnumerateArray().Last().GetProperty("Kind").GetString());
    }
    [Fact] public void P18_CoreHasNoProviderDependencyAndHydrationNotPublicMutation()
    {
        var core=typeof(FsNode).Assembly;var edges=core.GetTypes().SelectMany(LayerDependencies.Read);
        Assert.DoesNotContain(edges,e=>e.Target.Namespace?.StartsWith("Microsoft.Data.Sqlite")==true||e.Target.Namespace?.StartsWith("CloudFileManager.Infrastructure")==true);
        Assert.Null(typeof(FsNode).GetProperty("Id")!.SetMethod);Assert.All(typeof(DirectoryNode).GetConstructors(),c=>Assert.Fail("Domain constructor unexpectedly public"));
        Assert.Single(core.GetTypes(), t=>t.IsInterface&&t.Namespace=="CloudFileManager.Core.Application.Persistence");
    }
    [Fact] public void P19_StaleDurableRevisionFailsClosedRatherThanOverwrite()
    {
        using var store=Open();var (r,e)=Session(store);Sql("UPDATE store_meta SET durable_revision=99;");
        Assert.Throws<SessionUnavailableException>(()=>e.AddTag(r,TagKind.Work));
        Assert.Throws<SessionUnavailableException>(()=>e.Redo());Assert.Equal(99,store.LoadOrInitialize(Seed()).Revision);
    }
    [Fact] public void P20_FaultedSingletonCannotProjectOrContinueWorkspace()
    {
        using var real=Open();var unknown=new UnknownStore(real,false);var session=FileSystemSession.Instance;
        session.Restore(unknown.LoadOrInitialize(Seed()),unknown);var w=new WebWorkspace(session,session.Root.Id);
        Assert.Throws<SessionUnavailableException>(()=>w.Execute(new("addTag",Tag:"Work"),_=>{}));
        Assert.Throws<SessionUnavailableException>(()=>w.State());
        Assert.Throws<SessionUnavailableException>(()=>session.RemoveTag(session.Root,TagKind.Work));
        Assert.Equal(0,real.LoadOrInitialize(Seed()).Revision);
    }
    [Fact] public void P21_SeparateTemporaryDatabasesHaveIndependentDurableState()
    {
        using var a=Open();using var b=new SqliteFileSystemStore(Path.Combine(directory,"second.db"));
        var (ar,ae)=Session(a);var (br,be)=Session(b);ae.AddTag(ar,TagKind.Urgent);
        Assert.NotEqual(ar.Id,br.Id);Assert.Equal(0,be.DurableRevision);Assert.DoesNotContain(TagKind.Urgent,FileSystemDocumentMapper.Restore(b.LoadOrInitialize(Seed()).Document).Tags);
    }
    public void Dispose()
    {
        FileSystemSession.Instance.Reset(DirectoryNode.CreateRoot("cleanup",DateTimeOffset.UnixEpoch));
        if(Directory.Exists(directory))Directory.Delete(directory,true);
    }
}
