using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models
{
    [Table("entidad")]
    public class Entidad
    {
        public int Id { get; set; }

        [Display(Name = "Código")]
        public string Codigo { get; set; }
        public string Nombre { get; set; }

        [Display(Name = "Tipo de Entidad")]
        public int TipoEntidadId { get; set; }

        [Display(Name = "Tipo de Entidad")]
        public virtual TipoEntidad TipoEntidad { get; set; }

        [Display(Name = "Pertenece a")]
        public int? PerteneceAId { get; set; }

        [Display(Name = "Pertenece a")]
        public virtual Entidad PerteneceA { get; set; }

        [Display(Name = "Unidades subordinadas")]
        public virtual ICollection<Entidad> EntidadesSubordinadas { get; set; }

        [Display(Name = "Municipios")]
        public int MunicipioId { get; set; }

        [Display(Name = "Municipios")]
        public virtual Municipio Municipio { get; set; }

        [NotMapped]
        private string _descripcionCompleta;

        [NotMapped]
        public string DescripcionCompleta
        {
            get { return TipoEntidad.Nombre + " - " + Nombre ; }
            set { _descripcionCompleta = value; }
        }

    }

     
}