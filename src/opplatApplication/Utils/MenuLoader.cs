using System.Collections.Generic;
using opplatApplication.Models;

namespace opplatApplication.Utils
{
    public class MenuLoader
    {
        public List<IMenu> Load(string view)
        {
            var menus_list = new List<IMenu>();
            menus_list.Add(new MenuHeader { Header = "Apps" });
            menus_list.Add(new MenuItem
            {
                Title = "Dashboard",
                Group = "apps",
                Icon = "dashboard",
                Name = "Dashboard"
            });
            menus_list.Add(new MenuItem
            {
                Title = "Chat",
                Group = "apps",
                Icon = "chat_bubble",
                Target = "_blank",
                Name = "Chat"
            });
            menus_list.Add(new MenuItem
            {
                Title = "Inbox",
                Group = "apps",
                Icon = "email",
                Name = "Mail",
                Target = "_blank"
            });
            menus_list.Add(new MenuItem
            {
                Title = "Widgets",
                Group = "widgets",
                Icon = "widgets",
                Component = "widgets",
                Items = new List<SubMenu>(){
                    new SubMenu{Name = "social", Title= "Social", Component = "SocialWidget"},
                    new SubMenu{Name = "statistic", Title= "Statistic", Component = "StatisticWidget", Badge = "new"},
                    new SubMenu{Name = "chart", Title= "Chart", Component = "ChartWidget"},
                    new SubMenu{Name = "list", Title= "List", Component = "ListWidget"},
                }
            });
            menus_list.Add(new MenuHeader { Header = "CMS" });
            menus_list.Add(new MenuItem
            {
                Title = "List & Query",
                Group = "layout",
                Icon = "view_compact",
                Items = new List<SubMenu>(){
                    new SubMenu{Name = "Table", Title= "Basic Table", Component = "ListWidget"},
                }
            });
            return menus_list;
        }

        public List<IMenu> LoadModuleMenu(string module)
        {
            var menus = new List<IMenu>();
            //todo: cargar los menu del modulo actual
            return menus;
        }

        public List<IMenu> LoadAllModulesMenu()
        {
            var menus = new List<IMenu>();
            //todo: cargar los menu de los modulos
            return menus;
        }

        public List<IMenu> LoadPersonalizeMenu(string userName)
        {
            var menus = new List<IMenu>();
            //todo: cargar los menus personalizados del usuario
            return menus;
        }
    }
}