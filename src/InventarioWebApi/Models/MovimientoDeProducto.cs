using System;
using System.ComponentModel.DataAnnotations;

namespace InventarioWebApi.Models
{
    public class MovimientoDeProducto
    {
        public int Id { get; set; }
        public int AlmacenId { get; set; }
        public virtual Almacen Almacen { get; set; }

        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
        public int ProductoId { get; set; }
        public virtual Producto Producto { get; set; }
        [Required]
        public decimal Cantidad { get; set; }
        public int UnidadDeMedidaId { get; set; }
        public virtual UnidadDeMedida UnidadDeMedida { get; set; }

        public int TipoMovimientoId { get; set; }
        public virtual TipoMovimiento TipoMovimiento { get; set; }
    }
}