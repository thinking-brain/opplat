using System.Threading.Tasks;
using ImportadorDatos.Jobs;
using Microsoft.AspNetCore.Mvc;

namespace ImportadorDatos.Controllers
{
    [Route("importador/[controller]")]
    [ApiController]
    public class ImportarVersatController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            ImportarVersat.ImportarCuentasAsync();
            return Ok("Cuentas importadas.");
        }
    }
}