using System.Text.Json;
using CloudFileManager.Core.Application.Sessions;

// Observation only. The parent xUnit Fact asserts every cold-start contract in a fresh process.
var session = FileSystemSession.Instance;
bool InvalidOperation(Action action)
{
    try { action(); return false; }
    catch (InvalidOperationException) { return true; }
}
Console.WriteLine(JsonSerializer.Serialize(new
{
    sameInstance = ReferenceEquals(session, FileSystemSession.Instance),
    uninitialized = !session.IsInitialized,
    sealedPrivate = typeof(FileSystemSession).IsSealed && typeof(FileSystemSession).GetConstructors().Length == 0,
    rootThrows = InvalidOperation(() => { _ = session.Root; }),
    undoThrows = InvalidOperation(() => session.Undo()),
    clipboardThrows = InvalidOperation(() => { _ = session.HasClipboard; })
}));
