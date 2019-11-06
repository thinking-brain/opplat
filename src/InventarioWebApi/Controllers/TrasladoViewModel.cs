namespace InventarioWebApi.Controllers
{
    public class TrasladoViewModel
    {
        public int OrigenId { get; set; }
        public int DestinoId { get; set; }
        public int ProductoId { get; set; }
        public decimal Cantidad { get; set; }
        public int UnidadDeMedidaId { get; set; }
    }
}