using System.Threading.Tasks;
using ImportadorDatos.Jobs;
using Microsoft.AspNetCore.Mvc;

namespace ImportadorDatos.Controllers
{
    [Route("importador/[controller]")]
    [ApiController]
    public class ImportarVersatController : ControllerBase
    {
        ImportarVersat importador;

        public ImportarVersatController(ImportarVersat importador)
        {
            this.importador = importador;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            importador.ImportarCuentasAsync();
            return Ok("Cuentas importadas.");
        }

        [HttpGet("periodos")]
        public async Task<IActionResult> GetPeriodos()
        {
            importador.ImportarPeriodosContables();
            return Ok("Periodos importadas.");
        }

        [HttpGet("asientos")]
        public async Task<IActionResult> GetAsientos()
        {
            importador.ImportarAsientos();
            return Ok("Asientos importados.");
        }

        [HttpGet("trabajadores")]
        public async Task<IActionResult> GetTrabajadores()
        {
            importador.ImportarTrabajadores();
            return Ok("Trabajadores importados.");
        }

        [HttpGet("unidades-organizativas")]
        public async Task<IActionResult> GetUnidadesOrganizativas()
        {
            importador.ImportarUnidadesOrganizativas();
            return Ok("Unidades Organizativas importadas.");
        }

        [HttpGet("elementos-de-gastos")]
        public async Task<IActionResult> GetElementosDeGastos()
        {
            importador.ImportarElementosDeGastos();
            return Ok("Elementos de gastos importados correctamente.");
        }

        [HttpGet("partidas-de-gastos")]
        public async Task<IActionResult> GetPartidasDeGastos()
        {
            importador.ImportarPartidasDeGastos();
            return Ok("Partidas de gastos importadas correctamente.");
        }

        [HttpGet("subelementos-de-gastos")]
        public async Task<IActionResult> GetSubElementosDeGastos()
        {
            importador.ImportarSubElementosDeGastos();
            return Ok("SubElementos de gastos importados correctamente.");
        }

    }
}