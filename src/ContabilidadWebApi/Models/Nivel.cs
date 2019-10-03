using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContabilidadWebApi.Models
{
    [Table("contb_niveles")]
    public class Nivel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Número")]
        public string Numero { get; set; }

        [Required]
        public string Nombre { get; set; }

        public int? NivelSuperiorId { get; set; }

        public virtual Nivel NivelSuperior { get; set; }

        public virtual ICollection<Nivel> NivelesInferiores { get; set; }
    }
}