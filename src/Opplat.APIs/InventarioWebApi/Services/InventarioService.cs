using System.Linq;
using System.Threading.Tasks;
using InventarioWebApi.Data;
using InventarioWebApi.Models;
using InventarioWebApi.ViewModels;
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

        public async Task<TaskVM> ActualizarSubmayor(
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
                var t = new TaskVM();

                if (submayor != null)
                {
                    var cc = await Convertir(cantidad, unidadMedidaId, productoId);
                    var s = submayor.Cantidad += cc * tipoM.Factor;
                    if (s >= 0)
                    {
                        _context.Submayores.Update(submayor);
                    }
                    else
                    {
                        t.IsSuccess = false;
                        t.Message = "No se dispone de esta cantidad en el almacen";
                        return t;
                    }
                }
                else
                {
                    if (tipoM.Factor == 1)
                    {
                        Submayor submayor2 = new Submayor
                        {
                            AlmacenId = almacenId,
                            ProductoId = productoId,
                            Cantidad = await Convertir(cantidad, unidadMedidaId, productoId)
                        };
                        _context.Submayores.Add(submayor2);
                    }
                    else
                    {
                        t.IsSuccess = false;
                        t.Message = "No existe este submayor";
                    }
                };
                try
                {
                    await _context.SaveChangesAsync();
                    tran.Commit();
                    t.IsSuccess=true;
                    return t;
                }
                catch (System.Exception)
                {
                    t.IsSuccess = true;
                    t.Message = "Error al guardar";
                    return t;
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
