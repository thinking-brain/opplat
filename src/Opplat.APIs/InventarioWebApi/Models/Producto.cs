using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioWebApi.Models
{
    public class Producto
    {
        public int Id { get; set; }
        [Required]
        public string Codigo { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Display(Name = "Descripción")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }
        [Required]
        [Display(Name = "Unidad de medida")]
        public int UnidadId { get; set; }

        public virtual UnidadDeMedida Unidad { get; set; }

        [Required]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        public virtual Categoria Categoria { get; set; }

        [Required]
        [Display(Name = "Tipo")]
        public int TipoId { get; set; }

        public virtual TipoDeProducto Tipo { get; set; }
        public virtual ICollection<Derivado> Derivados { get; set; }
    }
}