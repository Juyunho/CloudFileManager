using CloudFileManager.Core.Application.Samples;
using CloudFileManager.Core.Application.Sessions;
using System.Text.Json;
using System.Threading.Channels;
using CloudFileManager.Web;
var builder = WebApplication.CreateBuilder(args);
Program.ConfigureServices(builder.Services);
var app = builder.Build();
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
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IFileSystemSession>(_ =>
        {
            var session = FileSystemSession.Instance;
            session.Reset(ReferenceTree.Create());
            return session;
        });
        services.AddSingleton<WebWorkspace>(provider =>
        {
            var session = provider.GetRequiredService<IFileSystemSession>();
            var project = session.Root.Children.Single(n => n.Name == "專案文件");
            var initial = project.Children.Single(n => n.Name == "API介面定義.docx");
            return new WebWorkspace(session, initial.Id);
        });
    }
}
