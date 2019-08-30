using System.Collections.Generic;
using opplatApplication.Models;

namespace opplatApplication.Utils
{
    public class MenuLoader
    {
        List<IMenu> menus_list { get; set; }
        public MenuLoader()
        {
            menus_list = new List<IMenu>();
        }
        public List<IMenu> Load(string view)
        {
            LoadPersonalizeMenu("user");
            LoadModuleMenu("Modulo de prueba");
            LoadAllModulesMenu();
            return menus_list;
        }

        public void LoadPersonalizeMenu(string userName)
        {
            //todo: cargar los menus personalizados del usuario
            menus_list.Add(new MenuHeader { Header = "Mis acciones" });
        }

        public void LoadModuleMenu(string module)
        {
            //todo: cargar los menu del modulo actual
            menus_list.Add(new MenuHeader { Header = module });
            menus_list.Add(new MenuItem
            {
                Title = "List & Query",
                Group = "layout",
                Icon = "view_compact",
                Items = new List<SubMenu>(){
                    new SubMenu{Name = "Table", Title= "Basic Table", Component = "ListWidget"},
                }
            });
        }

        public void LoadAllModulesMenu()
        {
            //todo: cargar los menu de los modulos
            menus_list.Add(new MenuHeader { Header = "Modules" });
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
        }
    }
}