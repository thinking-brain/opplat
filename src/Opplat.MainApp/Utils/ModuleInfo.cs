using System.Reflection;

namespace Opplat.MainApp.Utils;

public class ModuleInfo
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public Assembly Assembly { get; set; } = null!;

    public bool IsBundledWithHost { get; set; }

    public Version Version { get; set; } = null!;

    public string ShortName
    {
        get
        {
            return Name.Split('.').Last();
        }
    }

    public string Path { get; set; } = string.Empty;
}
