using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models
{
    [Table("unidades_organizativas")]
    public class UnidadOrganizativa
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Display(Name = "Tipo de unidad organizativa")]
        public int TipoUnidadOrganizativaId { get; set; }

        [Display(Name = "Tipo de unidad organizativa")]
        public virtual TipoUnidadOrganizativa TipoUnidadOrganizativa { get; set; }

        [Display(Name = "Pertenece a")]
        public int? PerteneceAId { get; set; }

        [Display(Name = "Pertenece a")]
        public virtual UnidadOrganizativa PerteneceA { get; set; }

        [Display(Name = "Unidades subordinadas")]
        public virtual ICollection<UnidadOrganizativa> UnidadesSubordinadas { get; set; }

        [Display(Name = "Puestos de trabajo")]
        public virtual ICollection<PuestoDeTrabajo> PuestosDeTrabajo { get; set; }

        public bool Activa { get; set; }

        public UnidadOrganizativa()
        {
            UnidadesSubordinadas = new HashSet<UnidadOrganizativa>();
            PuestosDeTrabajo = new HashSet<PuestoDeTrabajo>();
        }

        [NotMapped]
        public string Detalle
        {
            get { return "[" + Codigo + "] " + Nombre; }
        }

        [NotMapped]
        public string UnidTipo
        {
            get { return "[" + TipoUnidadOrganizativa.Nombre + "] " + Nombre; }
        }

      
    }
}