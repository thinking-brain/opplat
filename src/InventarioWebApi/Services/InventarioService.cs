using System.Linq;
using System.Threading.Tasks;
using InventarioWebApi.Data;
using InventarioWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioWebApi.Services.InventarioService
{
    public class InventarioService
    {
        private readonly InventarioDbContext _context;
        public InventarioService(InventarioDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> ActualizarSubmayor(int movimientoId)
        {
            using (var tran = _context.Database.BeginTransaction())
            {
                MovimientoDeProducto movimiento = await _context.MovimientosDeProductos.FindAsync(movimientoId);
                Submayor submayor = await _context.Submayores.FirstOrDefaultAsync(s => s.AlmacenId == movimiento.AlmacenId & s.ProductoId == movimiento.ProductoId);
                submayor.Cantidad += movimiento.Cantidad * movimiento.TipoMovimiento.Factor;
                _context.Update(submayor);
                await _context.SaveChangesAsync();
                tran.Commit();
                return submayor.Cantidad;
            }
        }
    }
}
