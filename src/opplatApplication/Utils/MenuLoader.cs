using Newtonsoft.Json;
using opplatApplication.Models;

namespace opplatApplication.Utils;

public class MenuLoader
{
    ILogger _logger;

    public MenuLoader(ILogger<MenuLoader> logger)
    {
        _logger = logger;
    }
    public List<MenuItem> LoadPersonalizeMenu(string userName)
    {
        try
        {
            var menus_list = new List<MenuItem>();
            var f = System.IO.File.ReadAllText("Data/menus/users/" + userName + ".json");
            var menus = JsonConvert.DeserializeAnonymousType(f, menus_list);
            return menus!;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error cargando menus perzonalizados del usuario {userName}. " + ex.Message);
            return new List<MenuItem>();
        }
    }

    public List<MenuItem> LoadModuleMenu(string module, string[] roles)
    {
        try
        {
            var menus_list = new List<MenuItem>();
            var f = System.IO.File.ReadAllText("Data/menus/modulos/" + module + ".json");
            var contratacionContador = new ContratacionContador();
            var menus = JsonConvert.DeserializeAnonymousType(f, menus_list);
            foreach (var menu in menus!)
            {
                if (menu.Roles.Any(r => roles.Contains(r)))
                {
                    if (menu.Name == "Contratos")
                    {
                        menu.Cant = contratacionContador.ContratosCant(menu.Name);
                    }
                    else if (menu.Name == "Ofertas")
                    {
                        menu.Cant = contratacionContador.ContratosCant(menu.Name);
                    }
                    menus_list.Add(menu);
                }
                if (menu.Items != null)
                {
                    menu.Items.RemoveAll(m => !m.Roles.Any(r => roles.Contains(r)));
                }
            }
            return menus_list;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error cargando menus del modulo {module}. " + ex.Message);
            return new List<MenuItem>();
        }
    }

    public List<MenuItem> LoadAllModulesMenu(string[] roles)
    {
        var f = System.IO.File.ReadAllText("Data/menus/modulos.json");
        var menus_list = new List<MenuItem>();
        var contratacionContador = new ContratacionContador();

        // var r = new JsonTextReader(f);
        var modulos = JsonConvert.DeserializeAnonymousType(f, menus_list);
        foreach (var menu in modulos!)
        {
            if (menu.Roles.Any(r => roles.Contains(r)))
            {
                if (menu.Name == "contratacion")
                {
                    menu.Cant = contratacionContador.ContratosCant("Contratos");
                }
                else if (menu.Name == "contratacion")
                {
                    menu.Cant = contratacionContador.ContratosCant("Ofertas");
                }
                menus_list.Add(menu);
            }
        }
        return menus_list;
    }
}
