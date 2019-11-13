using System.ComponentModel.DataAnnotations;

namespace InventarioWebApi.ViewModels
{
    public class TransferenciaViewModel
    {
        public int AlmacenOrigenId { get; set; }
        public int AlmacenDestinoId { get; set; }
        public int ProductoId { get; set; }
        public decimal Cantidad { get; set; }
        public int UnidadDeMedidaId { get; set; }
    }
}