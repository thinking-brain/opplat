using System.ComponentModel.DataAnnotations;

namespace InventarioWebApi.Models{
    public class TipoMovimiento
    {
        public int Id { get; set; } 
        [Required]
        public string Descripcion { get; set; }
        public int Factor { get; set; }
    }
}