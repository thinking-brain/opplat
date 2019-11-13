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

        public async Task<bool> ActualizarSubmayor(
            int almacenId,
            int productoId,
            decimal cantidad,
            int unidadMedidaId,
            int tipoMovimientoId)
        {
            using (var tran = _context.Database.BeginTransaction())
            {
                TipoMovimiento tipoM = _context.TiposDeMovimiento.Find(tipoMovimientoId);
                Submayor submayor = await _context.Submayores.FirstOrDefaultAsync(s => s.AlmacenId == almacenId & s.ProductoId == productoId);

                if (submayor != null)
                {
                    var cc = await Convertir(cantidad, unidadMedidaId, productoId);
                    submayor.Cantidad += cc * tipoM.Factor;
                    _context.Submayores.Update(submayor);
                }
                else
                {
                    Submayor submayor2 = new Submayor
                    {
                        AlmacenId = almacenId,
                        ProductoId = productoId,
                        Cantidad = await Convertir(cantidad, unidadMedidaId, productoId)
                    };
                    _context.Submayores.Add(submayor2);
                };
                try
                {
                    await _context.SaveChangesAsync();
                    tran.Commit();
                    return true;
                }
                catch (System.Exception)
                {
                    return false;
                    throw;
                }
            }
        }

        public async Task<decimal> Convertir(decimal cantidad, int unidadMedidaId, int productoId)
        {
            var um = await _context.UnidadesDeMedida.FindAsync(unidadMedidaId);
            Producto producto = await _context.Productos.Include(p => p.Unidad).FirstOrDefaultAsync(p => p.Id == productoId);
            return (cantidad * um.FactorConversion) / producto.Unidad.FactorConversion;
        }
    }
}
