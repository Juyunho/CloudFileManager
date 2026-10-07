using System.Reflection;
using System.Text.Json;
using CloudFileManager.Core.Application.Samples;
using CloudFileManager.Core.Application.Sessions;
using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Domain.Values;
using CloudFileManager.Web;
using Microsoft.Extensions.DependencyInjection;

namespace CloudFileManager.WebTests;

public sealed class DependencyInjectionTests
{
    private static JsonElement State(WebWorkspace w) => JsonSerializer.SerializeToElement(w.State());

    [Fact]
    public void ConstructionPreservesPopulatedDependencyAndRejectsInvalidSelection()
    {
        var session = new IsolatedTestSession(ReferenceTree.Create());
        var node = session.Root.Children[0];
        session.AddTag(node, TagKind.Urgent);
        session.AddTag(node, TagKind.Work);
        session.Undo();
        session.Copy(node);
        var w = new WebWorkspace(session, node.Id);
        Assert.Equal(1, session.UndoCount);
        Assert.Equal(1, session.RedoCount);
        Assert.True(session.HasClipboard);
        Assert.Equal(node.Id, State(w).GetProperty("selected").GetGuid());
        Assert.Throws<ArgumentException>(() => new WebWorkspace(session, Guid.NewGuid()));
        Assert.Throws<ArgumentNullException>(() => new WebWorkspace(null!, node.Id));
        Assert.Equal(1, session.UndoCount);
        Assert.Equal(1, session.RedoCount);
        Assert.True(session.HasClipboard);
    }

    [Fact]
    public async Task IndependentWorkspacesCanExecuteConcurrentlyWithoutGlobalSession()
    {
        var a = new IsolatedTestSession(ReferenceTree.Create());
        var wa = new WebWorkspace(a, a.Root.Id);
        wa.Execute(new("addTag", Tag: "Urgent"), _ => { });
        wa.Execute(new("copy"), _ => { });
        var before = State(wa).GetRawText();
        var b = new IsolatedTestSession(ReferenceTree.Create());
        var wb = new WebWorkspace(b, b.Root.Id);
        Assert.Equal(before, State(wa).GetRawText());
        Assert.NotEqual(a.Root.Id, b.Root.Id);
        var ready = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        async Task Edit(WebWorkspace workspace, string tag)
        {
            await ready.Task;
            workspace.Execute(new("addTag", Tag: tag), _ => { });
            workspace.Execute(new("copy"), _ => { });
        }
        var first = Task.Run(() => Edit(wa, "Work"), TestContext.Current.CancellationToken);
        var second = Task.Run(() => Edit(wb, "Personal"), TestContext.Current.CancellationToken);
        ready.SetResult();
        await Task.WhenAll(first, second);
        Assert.Contains(TagKind.Work, a.Root.Tags);
        Assert.DoesNotContain(TagKind.Personal, a.Root.Tags);
        Assert.Contains(TagKind.Personal, b.Root.Tags);
        Assert.DoesNotContain(TagKind.Urgent, b.Root.Tags);
        Assert.Equal(2, a.UndoCount);
        Assert.Equal(1, b.UndoCount);
        Assert.True(a.HasClipboard && b.HasClipboard);
        Assert.Equal(a.Root.Id, State(wa).GetProperty("selected").GetGuid());
        Assert.Equal(b.Root.Id, State(wb).GetProperty("selected").GetGuid());
        var bBefore = State(wb).GetRawText();
        wa.Execute(new("undo"), _ => { });
        Assert.Equal(bBefore, State(wb).GetRawText());
    }

    [Fact]
    public void CompiledWorkspaceHasNoConcreteSingletonOrServiceLocatorDependency()
    {
        var types = new[] { typeof(WebWorkspace) }.Concat(typeof(WebWorkspace).GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic));
        var edges = types.SelectMany(LayerDependencies.Read).ToArray();
        Assert.DoesNotContain(edges, edge => edge.Target == typeof(FileSystemSession) || edge.Target == typeof(IServiceProvider));
        Assert.Contains(edges, edge => edge.Target == typeof(IFileSystemSession));
    }

    [Fact]
    public void ConsumerContractDoesNotExposeResetOrOptionalFallback()
    {
        Assert.Null(typeof(IFileSystemSession).GetMethod("Reset"));
        var constructor = Assert.Single(typeof(WebWorkspace).GetConstructors());
        Assert.Equal(new[] { typeof(IFileSystemSession), typeof(Guid) }, constructor.GetParameters().Select(p => p.ParameterType));
        Assert.All(constructor.GetParameters(), p => Assert.False(p.IsOptional));
    }
}

public sealed class ProductionCompositionTests : IDisposable
{
    private static void Register(IServiceCollection services) =>
        typeof(WebWorkspace).Assembly.GetType("Program")!.GetMethod("ConfigureServices", BindingFlags.Public | BindingFlags.Static)!.Invoke(null, new object[] { services });

    [Fact]
    public void ActualRegistrationUsesGoFInstanceAndPreservesStateAcrossScopes()
    {
        var services = new ServiceCollection();
        Register(services);
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
        var session = provider.GetRequiredService<IFileSystemSession>();
        Assert.Same(FileSystemSession.Instance, session);
        using (var scope = provider.CreateScope())
        {
            var workspace = scope.ServiceProvider.GetRequiredService<WebWorkspace>();
            workspace.Execute(new("addTag", Tag: "Urgent"), _ => { });
            workspace.Execute(new("copy"), _ => { });
        }
        using (var next = provider.CreateScope())
        {
            Assert.Same(session, next.ServiceProvider.GetRequiredService<IFileSystemSession>());
            var workspace = next.ServiceProvider.GetRequiredService<WebWorkspace>();
            Assert.Same(provider.GetRequiredService<WebWorkspace>(), workspace);
            Assert.True(session.HasClipboard);
            Assert.Equal(1, session.UndoCount);
            workspace.Execute(new("undo"), _ => { });
            Assert.Equal(1, FileSystemSession.Instance.RedoCount);
        }
    }

    [Fact]
    public void OverrideBeforeResolutionDoesNotInitializeOrResetGlobalState()
    {
        var root = DirectoryNode.CreateRoot("sentinel", DateTimeOffset.UnixEpoch);
        FileSystemSession.Instance.Reset(root);
        FileSystemSession.Instance.AddTag(root, TagKind.Work);
        var services = new ServiceCollection();
        Register(services);
        var isolated = new IsolatedTestSession(ReferenceTree.Create());
        services.AddSingleton<IFileSystemSession>(isolated);
        using var provider = services.BuildServiceProvider();
        var w = provider.GetRequiredService<WebWorkspace>();
        w.Execute(new("addTag", Tag: "Urgent"), _ => { });
        Assert.Same(root, FileSystemSession.Instance.Root);
        Assert.Equal(1, FileSystemSession.Instance.UndoCount);
        Assert.DoesNotContain(TagKind.Urgent, root.Tags);
        Assert.Equal(1, isolated.UndoCount);
    }

    public void Dispose() => FileSystemSession.Instance.Reset(DirectoryNode.CreateRoot("cleanup", DateTimeOffset.UnixEpoch));
}
