using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models {
    [Table ("cargo")]
    public class Cargo {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }
        public string Sigla { get; set; }
        //public virtual NivelDeEscolaridad NivelDeEscolaridad { get; set; }
        public int GrupoEscalaId { get; set; }
        public virtual GrupoEscala GrupoEscala { get; set; }
        public virtual ICollection<PuestoDeTrabajo> PuestosDeTrabajo { get; set; }

        public bool Activo { get; set; }
        public Cargo () {
            PuestosDeTrabajo = new HashSet<PuestoDeTrabajo> ();
        }
    }
}