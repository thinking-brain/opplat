using System.ComponentModel.DataAnnotations;

namespace InventarioWebApi.Models
{
    public class TipoDeProducto
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Display(Name = "Descripci�n")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }
    }
}