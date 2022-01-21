using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using opplatApplication.Models;
using opplatApplication.Services;
using opplatApplication.Utils;

namespace opplatApplication.Controllers
{
    [Route("admin/[controller]")]
    [ApiController]
    [Authorize]
    public class MenusController : Controller
    {
        private MenuLoader _menuLoader;
        public MenusController(MenuLoader menuLoader)
        {
            _menuLoader = menuLoader;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var usuario = User.Identity.Name;
            var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            var personales = _menuLoader.LoadPersonalizeMenu(usuario);
            var modulos = _menuLoader.LoadAllModulesMenu(roles);
            return Ok(new { Modulos = modulos, Personalizados = personales });
        }

        [HttpGet("FromModulo")]
        public ActionResult<IEnumerable<IMenu>> FromModulo(string modulo)
        {
            var usuario = User.Identity.Name;
            var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            var menus = _menuLoader.LoadModuleMenu(modulo, roles);
            return menus;
        }
    }
}