// // MenusController has been replaced by Features/Menus/ (MediatR + Minimal API).
// // Kept for reference only — [ApiController] and [Route] removed.

// using System.Security.Claims;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Opplat.Api.Main.Models;
// using Opplat.Api.Main.Utils;

// namespace Opplat.Api.Main.Controllers;

// // [Route("admin/[controller]")]
// // [ApiController]
// // // [Authorize]
// public class MenusController_Archived : Controller
// {
//     private MenuLoader _menuLoader;
//     public MenusController_Archived(MenuLoader menuLoader)
//     {
//         _menuLoader = menuLoader;
//     }

//     [HttpGet]
//     public async Task<IActionResult> Get()
//     {
//         string usuario = User!.Identity!.Name!;
//         var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
//         var personales = _menuLoader.LoadPersonalizeMenu(usuario);
//         var modulos = _menuLoader.LoadAllModulesMenu(roles);
//         await Task.CompletedTask;
//         return Ok(new { Modulos = modulos, Personalizados = personales });
//     }

//     [HttpGet("FromModulo")]
//     public ActionResult<IEnumerable<IMenu>> FromModulo(string modulo)
//     {
//         var usuario = User!.Identity!.Name;
//         var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
//         var menus = _menuLoader.LoadModuleMenu(modulo, roles);
//         return menus;
//     }
// }
