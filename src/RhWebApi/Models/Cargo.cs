using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models {
    public class Cargo {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Sigla { get; set; }
        
        public int? GrupoEscalaId { get; set; }
        public virtual GrupoEscala GrupoEscala { get; set; }    

        public int? JefeId { get; set; }
        public virtual Cargo Jefe { get; set; }
    }
}