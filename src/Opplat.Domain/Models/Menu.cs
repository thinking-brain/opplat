namespace Opplat.Domain.Models;

public interface IMenu
{

}
public class MenuHeader : IMenu
{
    public string Header { get; set; } = string.Empty;
}
public class MenuGroup
{
    //todo: implementar en el futuro cuando se complejicen los menu
}
public class MenuItem : IMenu
{
    public string Title { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Cant { get; set; } = string.Empty;
    public string Component { get; set; } = string.Empty;
    public List<SubMenu> Items { get; set; } = [];

    public List<string> Roles { get; set; } = [];
}

public class SubMenu
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Badge { get; set; } = string.Empty;
    public string Component { get; set; } = string.Empty;

    public List<string> Roles { get; set; } = [];
}
