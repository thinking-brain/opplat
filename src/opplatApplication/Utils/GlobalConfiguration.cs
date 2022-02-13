using System.Reflection;

namespace opplatApplication.Utils;

public static class GlobalConfiguration
{
    static GlobalConfiguration()
    {
        Modules = new List<ModuleInfo>();
        OtherAssemblies = new List<Assembly>();
    }

    public static IList<ModuleInfo> Modules { get; set; }

    public static IList<Assembly> OtherAssemblies { get; set; }

    public static string DefaultCulture => "en-US";

    public static string WebRootPath { get; set; } = String.Empty;

    public static string ContentRootPath { get; set; } = String.Empty;
}
