using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioWebApi.Models
{
    public class UnidadDeMedida
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Display(Name = "Descripción")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }
        public int TipoId { get; set; }
        //basado en una unidad de medida, en este caso gramo
        public decimal FactorConversion {get;set;}
    }
}