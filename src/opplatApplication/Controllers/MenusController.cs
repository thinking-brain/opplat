using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using opplatApplication.Models;
using opplatApplication.Utils;

namespace opplatApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenusController
    {
        private MenuLoader _menuLoader;
        public MenusController(MenuLoader menuLoader)
        {
            _menuLoader = menuLoader;
        }
        [HttpGet]
        public ActionResult<IEnumerable<IMenu>> Get()
        {
            return _menuLoader.Load("home");
        }
    }
}