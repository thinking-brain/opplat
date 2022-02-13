using System.ComponentModel.DataAnnotations;

namespace InventarioWebApi.Models
{
    public class Almacen
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Codigo { get; set; }
        public string Localizacion { get; set; }
    }
}