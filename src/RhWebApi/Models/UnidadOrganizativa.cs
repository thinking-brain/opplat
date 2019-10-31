using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models {
    [Table ("unidades_organizativas")]
    public class UnidadOrganizativa {
        public int Id { get; set; }

        [Required]
        public string Codigo { get; set; }

        [Required]
        public string Nombre { get; set; }

        public int TipoUnidadOrganizativaId { get; set; }

        public virtual TipoUnidadOrganizativa TipoUnidadOrganizativa { get; set; }

        public int? PerteneceAId { get; set; }

        public virtual UnidadOrganizativa PerteneceA { get; set; }

        public virtual ICollection<UnidadOrganizativa> UnidadesSubordinadas { get; set; }

        public virtual ICollection<PuestoDeTrabajo> PuestoDeTrabajo { get; set; }

        public bool Activa { get; set; }

        public UnidadOrganizativa () {
            UnidadesSubordinadas = new HashSet<UnidadOrganizativa> ();
            PuestoDeTrabajo = new HashSet<PuestoDeTrabajo> ();
        }

        [NotMapped]
        public string Detalle {
            get { return "[" + Codigo + "] " + Nombre; }
        }

        [NotMapped]
        public string UnidTipo {
            get { return "[" + TipoUnidadOrganizativa.Nombre + "] " + Nombre; }
        }

    }
}