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
