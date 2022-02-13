using System.ComponentModel.DataAnnotations;

namespace InventarioWebApi.ViewModels
{
    public class MovimientoViewModel
    {
        public int AlmacenId { get; set; }

        public int ProductoId { get; set; }
        [Required]
        public decimal Cantidad { get; set; }
        public int UnidadDeMedidaId { get; set; }

        public int TipoMovimientoId { get; set; }
    }
}