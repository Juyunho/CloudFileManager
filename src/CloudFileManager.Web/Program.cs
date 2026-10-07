using CloudFileManager.Core.Application.Persistence;
using CloudFileManager.Infrastructure;
using CloudFileManager.Core.Application.Samples;
using CloudFileManager.Core.Application.Sessions;
using System.Text.Json;
using System.Threading.Channels;
using CloudFileManager.Web;
var builder = WebApplication.CreateBuilder(args);
Program.ConfigureServices(builder.Services, builder.Configuration["FileSystem:DatabasePath"] ?? Path.Combine(builder.Environment.ContentRootPath, "App_Data", "filesystem.db"));
var app = builder.Build();
_ = app.Services.GetRequiredService<WebWorkspace>(); // Fail startup before serving an invalid/uninitialized store.
app.Use(async (context, next) =>
{
    try { await next(context); }
    catch (SessionUnavailableException)
    {
        if (context.Response.HasStarted) context.Abort();
        else { context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable; await context.Response.WriteAsJsonAsync(new { error = "Filesystem session unavailable; restart required." }); }
    }
});
app.UseDefaultFiles(); app.UseStaticFiles();
var json = new JsonSerializerOptions(JsonSerializerDefaults.Web);
app.MapGet("/api/state", async (WebWorkspace workspace) =>
{
    await workspace.Gate.WaitAsync();
    try { return Results.Json(workspace.State()); } finally { workspace.Gate.Release(); }
});
app.MapPost("/api/action", async (ActionRequest request, WebWorkspace workspace, HttpContext context) =>
{
    context.Response.ContentType = "application/x-ndjson; charset=utf-8";
    var channel = Channel.CreateUnbounded<object>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = true });
    var operation = Task.Run(async () =>
    {
        await workspace.Gate.WaitAsync();
        try { channel.Writer.TryWrite(workspace.Execute(request, item => channel.Writer.TryWrite(item))); }
        finally { workspace.Gate.Release(); channel.Writer.TryComplete(); }
    });
    try
    {
        await foreach (var item in channel.Reader.ReadAllAsync(context.RequestAborted))
        { await context.Response.WriteAsync(JsonSerializer.Serialize(item, json) + "\n", context.RequestAborted); await context.Response.Body.FlushAsync(context.RequestAborted); }
    }
    finally { await operation; }
});
app.Run();

public partial class Program
{
    // One host/workspace per process: DI supplies the existing GoF object, not a new session.
    public static void ConfigureServices(IServiceCollection services, string databasePath)
    {
        services.AddSingleton<IFileSystemStore>(_ => new SqliteFileSystemStore(databasePath));
        services.AddSingleton<IFileSystemSession>(provider =>
        {
            var store = provider.GetRequiredService<IFileSystemStore>();
            var loaded = store.LoadOrInitialize(FileSystemDocumentMapper.Capture(ReferenceTree.Create()));
            var session = FileSystemSession.Instance;
            session.Restore(loaded, store);
            return session;
        });
        services.AddSingleton<WebWorkspace>(provider =>
        {
            var session = provider.GetRequiredService<IFileSystemSession>();
            var project = session.Root.Children.SingleOrDefault(n => n.Name == "專案文件" && n is CloudFileManager.Core.Domain.Nodes.DirectoryNode);
            var initial = project?.Children.SingleOrDefault(n => n.Name == "API介面定義.docx" && n is CloudFileManager.Core.Domain.Nodes.FileNode);
            return new WebWorkspace(session, initial?.Id ?? session.Root.Id);
        });
    }
}
