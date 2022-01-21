using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContratacionWebApi.Models {
    [Table ("cont_especialidades")]
    public class Especialidad {
        public int Id { get; set; }

        [Required]
        [RegularExpression ("[a-z A-Z,ñ,Ñ,í,ó,á,é,ú,Í,Ó,Á,É,Ú, ]*", ErrorMessage = "Solo letras")]
        public string Nombre { get; set; }

        public virtual ICollection<Especialista> Especialistas { get; set; }

        [NotMapped]
        public virtual ICollection<Documento> Documentos { get; set; }

        public Especialidad () {
            Especialistas = new HashSet<Especialista> ();
            Documentos = new HashSet<Documento> ();
        }

    }
}