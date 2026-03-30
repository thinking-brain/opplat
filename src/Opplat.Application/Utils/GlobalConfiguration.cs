using System.Reflection;

namespace Opplat.Application.Utils;

public static class GlobalConfiguration
{
    public static IList<ModuleInfo> Modules { get; set; } = [];

    public static IList<Assembly> OtherAssemblies { get; set; } = [];

    public static string DefaultCulture => "en-US";

    public static string WebRootPath { get; set; } = string.Empty;

    public static string ContentRootPath { get; set; } = string.Empty;
}
