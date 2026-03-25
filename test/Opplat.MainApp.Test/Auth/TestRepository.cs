using System.IO;

namespace Opplat.MainApp.Test.Auth;

internal static class TestRepository
{
    private static readonly Lazy<string> RepoRoot = new(FindRepoRoot);

    public static string ResolvePath(params string[] segments)
    {
        return Path.Combine(new[] { RepoRoot.Value }.Concat(segments).ToArray());
    }

    public static string ReadAllText(params string[] segments)
    {
        return File.ReadAllText(ResolvePath(segments));
    }

    private static string FindRepoRoot()
    {
        var current = AppContext.BaseDirectory;

        while (!string.IsNullOrEmpty(current))
        {
            if (File.Exists(Path.Combine(current, "opplat.sln")) ||
                File.Exists(Path.Combine(current, "opplat.slnx")) ||
                (Directory.Exists(Path.Combine(current, "src")) && Directory.Exists(Path.Combine(current, "test"))))
            {
                return current;
            }

            current = Directory.GetParent(current)?.FullName!;
        }

        throw new DirectoryNotFoundException("Could not locate repository root from test output directory.");
    }
}
