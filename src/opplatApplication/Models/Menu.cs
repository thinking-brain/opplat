using System.Collections.Generic;

namespace opplatApplication.Models
{
    public interface IMenu
    {

    }
    public class MenuHeader : IMenu
    {
        public string Header { get; set; }
    }
    public class MenuGroup
    {
        //todo: implementar en el futuro cuando se complejicen los menu
    }
    public class MenuItem : IMenu
    {
        public string Title { get; set; }
        public string Group { get; set; }
        public string Name { get; set; }
        public string Target { get; set; }
        public string Icon { get; set; }
        public string Component { get; set; }
        public List<SubMenu> Items { get; set; }

        public MenuItem()
        {
            Items = new List<SubMenu>();
        }
    }

    public class SubMenu
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Badge { get; set; }
        public string Component { get; set; }
    }
}