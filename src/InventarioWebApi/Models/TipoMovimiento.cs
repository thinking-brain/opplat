using System.ComponentModel.DataAnnotations;

namespace InventarioWebApi.Models{
    public class TipoMovimiento
    {
        public int Id { get; set; } 
        [Required]
        public string Nombre { get; set; }
        public int Factor { get; set; }
    }
}