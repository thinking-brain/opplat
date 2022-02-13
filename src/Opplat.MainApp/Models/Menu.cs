namespace Opplat.MainApp.Models;

public interface IMenu
{

}
public class MenuHeader : IMenu
{
    public string Header { get; set; }  = String.Empty;
}
public class MenuGroup
{
    //todo: implementar en el futuro cuando se complejicen los menu
}
public class MenuItem : IMenu
{
    public string Title { get; set; } = String.Empty;
    public string Group { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public string Target { get; set; } = String.Empty;
    public string Icon { get; set; } = String.Empty;
    public string Cant { get; set; } = String.Empty;
    public string Component { get; set; } = String.Empty;
    public List<SubMenu> Items { get; set; }

    public List<string> Roles { get; set; }

    public MenuItem()
    {
        Items = new List<SubMenu>();
        Roles = new List<string>();
    }
}

public class SubMenu
{
    public string Name { get; set; } = String.Empty;
    public string Title { get; set; } = String.Empty;
    public string Badge { get; set; } = String.Empty;
    public string Component { get; set; } = String.Empty;

    public List<string> Roles { get; set; }

    public SubMenu()
    {
        Roles = new List<string>();
    }
}
