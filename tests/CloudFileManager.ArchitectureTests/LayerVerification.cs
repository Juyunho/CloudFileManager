using CloudFileManager.Core.Domain.Nodes;
using CloudFileManager.Core.Application.Sessions;

internal static class LayerVerification
{
    internal static int Run()
    {
        int passed = 0, failed = 0;
        void Test(string name, Action body)
        {
            try { body(); passed++; Console.WriteLine("PASS " + name); }
            catch (Exception e) { failed++; Console.WriteLine("FAIL " + name + ": " + e); }
        }
        void Check(bool value, string message) { if (!value) throw new Exception(message); }
        const string domain = "CloudFileManager.Core.Domain";
        const string app = "CloudFileManager.Core.Application";
        bool Under(Type t, string prefix) => t.Namespace == prefix || (t.Namespace?.StartsWith(prefix + ".", StringComparison.Ordinal) ?? false);
        bool CompilerSynthesizedContainer(Type t)
        {
            while (t.DeclaringType is Type parent) t = parent;
            return t.Name.StartsWith('<'); // C# compiler containers cannot be authored as C# type identifiers.
        }
        var assembly = typeof(FsNode).Assembly;
        var all = assembly.GetTypes();
        var domainTypes = all.Where(t => Under(t, domain)).ToArray();
        var edges = domainTypes.SelectMany(LayerDependencies.Read).ToArray();
        Test("L01 Domain compiled metadata and IL depend only on Domain or BCL", () => {
            Check(domainTypes.Length >= 14, "Domain inventory unexpectedly empty/incomplete");
            var invalid = edges.Where(e => e.Target.Assembly == assembly ? !Under(e.Target, domain) :
                !(e.Target.Assembly == typeof(object).Assembly || (e.Target.Assembly.GetName().Name?.StartsWith("System.", StringComparison.Ordinal) ?? false))).ToArray();
            Check(invalid.Length == 0, string.Join("\n", invalid.Select(e => e.Source + " -> " + e.Target + " " + e.Location)));
        });
        Test("L02 Domain has no Console TextWriter or XML presentation dependencies", () => {
            var invalid = edges.Where(e => e.Target == typeof(Console) || e.Target == typeof(TextWriter) || e.Target.Namespace?.StartsWith("System.Xml", StringComparison.Ordinal) == true).ToArray();
            Check(invalid.Length == 0, string.Join("\n", invalid.Select(e => e.ToString())));
        });
        Test("L03 all production types classified; Application stays inward", () => {
            Check(all.Where(t => !CompilerSynthesizedContainer(t)).All(t => Under(t, domain) || Under(t, app)), "Unclassified Core type: " + string.Join(", ", all.Where(t => !CompilerSynthesizedContainer(t) && !Under(t, domain) && !Under(t, app)).Select(t => t.FullName + " [" + string.Join(",", t.GetCustomAttributesData().Select(a => a.AttributeType.Name)) + "]")));
            var outgoing = all.Where(t => Under(t, app)).SelectMany(LayerDependencies.Read);
            Check(outgoing.All(e => e.Target.Assembly == assembly || e.Target.Assembly == typeof(object).Assembly ||
                (e.Target.Assembly.GetName().Name?.StartsWith("System.", StringComparison.Ordinal) ?? false)), "Application depends on outer project");
        });
        Test("L04 negative control detects fully qualified Application reference in method body", () => {
            var negative = LayerDependencies.Read(typeof(LayerFixtures.BodyDependency));
            Check(negative.Any(e => Under(e.Target, app) && e.Location.StartsWith("IL:")), "Guard missed body-only dependency");
        });
        Test("L05 negative control detects Application nested generic and signature dependency", () => {
            var negative = LayerDependencies.Read(typeof(LayerFixtures.SignatureDependency));
            Check(negative.Any(e => e.Target == typeof(FileSystemSession) && e.Location.StartsWith("signature:")), "Guard missed generic argument");
        });
        Test("L06 Application mutation access limited to approved commands bootstrap and revision reader", () => {
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
                    "Insert" or "Remove" => name is "CloudFileManager.Core.Application.Commands.DeleteCommand" or "CloudFileManager.Core.Application.Commands.PasteCommand",
                    "SetTag" => name == "CloudFileManager.Core.Application.Commands.TagCommand",
                    "LoadTags" => name == "CloudFileManager.Core.Application.Samples.ReferenceTree",
                    "get_State" or "Revision" => name == "CloudFileManager.Core.Application.Sessions.EditingSession",
                    _ => false
                };
                Check(allowed, "Unapproved mutation access: " + edge);
            }
        });
        Console.WriteLine($"LAYER RESULT {passed} passed; {failed} failed");
        return failed;
    }
}

namespace LayerFixtures
{
    // These forbidden examples are test-only; never added to the Core production assembly.
    internal sealed class BodyDependency
    {
        public object Reference() => CloudFileManager.Core.Application.Sessions.FileSystemSession.Instance;
    }
    internal sealed class SignatureDependency
    {
        public List<CloudFileManager.Core.Application.Sessions.FileSystemSession[]> Reference() => [];
    }
}
