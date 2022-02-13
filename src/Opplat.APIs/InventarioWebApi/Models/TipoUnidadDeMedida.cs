using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioWebApi.Models
{
    public class TipoUnidadDeMedida
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
    }
}