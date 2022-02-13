using System.ComponentModel.DataAnnotations;

namespace InventarioWebApi.Models
{
    public class Derivado:Producto
    {
        public int ProductoOrigenId { get; set; }
        public virtual Producto ProductoOrigen { get; set; }
    }
}