using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models
{
    [Table("tipos_de_unidad_organizativa")]
    public class TipoUnidadOrganizativa
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        public int Prioridad { get; set; }

        [Display(Name = "Unidades organizativas")]
        public virtual ICollection<UnidadOrganizativa> UnidadesOrganizativas { get; set; }

        public TipoUnidadOrganizativa()
        {
            UnidadesOrganizativas = new HashSet<UnidadOrganizativa>();
        }
    }
}