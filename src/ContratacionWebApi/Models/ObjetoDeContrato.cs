using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContratacionWebApi.Models
{
    [Table("cont_objs_de_contratos")]
    public class ObjetoDeContrato
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }
        [NotMapped]
        public virtual ICollection<DocumentoDeContrato> Documentos { get; set; }

        public ObjetoDeContrato()
        {
            Documentos = new HashSet<DocumentoDeContrato>();
        }
    }
}