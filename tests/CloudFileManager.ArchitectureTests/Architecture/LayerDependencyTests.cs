using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Application.Sessions;

namespace CloudFileManager.ArchitectureTests;

public sealed class LayerDependencyTests
{
    private const string domain = "CloudFileManager.Core.Domain";
    private const string app = "CloudFileManager.Core.Application";
    private static bool Under(Type t, string prefix) => t.Namespace == prefix || (t.Namespace?.StartsWith(prefix + ".", StringComparison.Ordinal) ?? false);
    private static bool CompilerSynthesizedContainer(Type t)
        {
            while (t.DeclaringType is Type parent) t = parent;
            return t.Name.StartsWith('<'); // C# compiler containers cannot be authored as C# type identifiers.
        }
    private static readonly System.Reflection.Assembly assembly = typeof(FsNode).Assembly;
    private readonly Type[] all = assembly.GetTypes();
    private readonly Type[] domainTypes = assembly.GetTypes().Where(t => Under(t, domain)).ToArray();
    private readonly LayerDependencies.Edge[] edges = assembly.GetTypes().Where(t => Under(t, domain)).SelectMany(LayerDependencies.Read).ToArray();
    private static void Check(bool value, string? message = null,
        [System.Runtime.CompilerServices.CallerArgumentExpression(nameof(value))] string? expression = null)
        => Assert.True(value, message ?? expression);
    private static void Equal<T>(T expected, T actual) => Assert.Equal(expected, actual);
    private static void Throws<T>(Action action) where T : Exception => Assert.ThrowsAny<T>(action);
    [Fact(DisplayName = "L01 Domain compiled metadata and IL depend only on Domain or BCL")]
    [Trait("LegacyId", "L01")]
    [Trait("Category", "Architecture")]
    public void L01()
    {
        Check(domainTypes.Length >= 14, "Domain inventory unexpectedly empty/incomplete");
        var invalid = edges.Where(e => e.Target.Assembly == assembly ? !Under(e.Target, domain) :
            !(e.Target.Assembly == typeof(object).Assembly || (e.Target.Assembly.GetName().Name?.StartsWith("System.", StringComparison.Ordinal) ?? false))).ToArray();
        Check(invalid.Length == 0, string.Join("\n", invalid.Select(e => e.Source + " -> " + e.Target + " " + e.Location)));
    }

    [Fact(DisplayName = "L02 Domain has no Console TextWriter or XML presentation dependencies")]
    [Trait("LegacyId", "L02")]
    [Trait("Category", "Architecture")]
    public void L02()
    {
        var invalid = edges.Where(e => e.Target == typeof(Console) || e.Target == typeof(TextWriter) || e.Target.Namespace?.StartsWith("System.Xml", StringComparison.Ordinal) == true).ToArray();
        Check(invalid.Length == 0, string.Join("\n", invalid.Select(e => e.ToString())));
    }

    [Fact(DisplayName = "L03 all production types classified; Application stays inward")]
    [Trait("LegacyId", "L03")]
    [Trait("Category", "Architecture")]
    public void L03()
    {
        Check(all.Where(t => !CompilerSynthesizedContainer(t)).All(t => Under(t, domain) || Under(t, app)), "Unclassified Core type: " + string.Join(", ", all.Where(t => !CompilerSynthesizedContainer(t) && !Under(t, domain) && !Under(t, app)).Select(t => t.FullName + " [" + string.Join(",", t.GetCustomAttributesData().Select(a => a.AttributeType.Name)) + "]")));
        var outgoing = all.Where(t => Under(t, app)).SelectMany(LayerDependencies.Read);
        Check(outgoing.All(e => e.Target.Assembly == assembly || e.Target.Assembly == typeof(object).Assembly ||
            (e.Target.Assembly.GetName().Name?.StartsWith("System.", StringComparison.Ordinal) ?? false)), "Application depends on outer project");
    }

    [Fact(DisplayName = "L04 negative control detects fully qualified Application reference in method body")]
    [Trait("LegacyId", "L04")]
    [Trait("Category", "Architecture")]
    public void L04()
    {
        var negative = LayerDependencies.Read(typeof(LayerFixtures.BodyDependency));
        Check(negative.Any(e => Under(e.Target, app) && e.Location.StartsWith("IL:")), "Guard missed body-only dependency");
    }

    [Fact(DisplayName = "L05 negative control detects Application nested generic and signature dependency")]
    [Trait("LegacyId", "L05")]
    [Trait("Category", "Architecture")]
    public void L05()
    {
        var negative = LayerDependencies.Read(typeof(LayerFixtures.SignatureDependency));
        Check(negative.Any(e => e.Target == typeof(FileSystemSession) && e.Location.StartsWith("signature:")), "Guard missed generic argument");
    }

    [Fact(DisplayName = "L06 Application mutation access limited to approved commands bootstrap and revision reader")]
    [Trait("LegacyId", "L06")]
    [Trait("Category", "Architecture")]
    public void L06()
    {
        var mutations = new[] { "Insert", "Remove", "SetTag", "LoadTags", "get_State", "Revision" };
        var access = all.Where(t => Under(t, app)).SelectMany(LayerDependencies.Read)
            .Where(e => e.Location.StartsWith("IL:") && e.Member?.DeclaringType is Type owner && Under(owner, domain) && mutations.Contains(e.Member.Name));
        foreach (var edge in access)
        {
            var owner = edge.Source;
            while (owner.DeclaringType is Type parent) owner = parent;
            var name = owner.FullName;
            var member = edge.Member!.Name;
            bool allowed = member switch {
                "Remove" => name is "CloudFileManager.Core.Application.Commands.DeleteCommand" or "CloudFileManager.Core.Application.Commands.PasteCommand",
                "Insert" => name is "CloudFileManager.Core.Application.Commands.DeleteCommand" or "CloudFileManager.Core.Application.Commands.PasteCommand" or "CloudFileManager.Core.Application.Persistence.FileSystemDocumentMapper",
                "SetTag" => name == "CloudFileManager.Core.Application.Commands.TagCommand",
                "LoadTags" => name is "CloudFileManager.Core.Application.Samples.ReferenceTree" or "CloudFileManager.Core.Application.Persistence.FileSystemDocumentMapper",
                "get_State" or "Revision" => name == "CloudFileManager.Core.Application.Sessions.EditingSession",
                _ => false
            };
            Check(allowed, "Unapproved mutation access: " + edge);
        }
    }
}
