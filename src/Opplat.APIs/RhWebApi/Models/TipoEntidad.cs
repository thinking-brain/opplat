using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models
{
    [Table("tipos_de_entidades")]
    public class TipoEntidad
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        public int Prioridad { get; set; }

        [Display(Name = "Entidades")]
        public virtual ICollection<Entidad> Entidades { get; set; }

        public TipoEntidad()
        {
            Entidades = new HashSet<Entidad>();
        }
    }
}