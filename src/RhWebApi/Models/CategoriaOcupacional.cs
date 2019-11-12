using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models
{
    [Table("categorias_ocupacionales")]
    public class CategoriaOcupacional
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        public virtual ICollection<GrupoEscala> GruposEscala { get; set; }

        public CategoriaOcupacional()
        {
            GruposEscala = new HashSet<GrupoEscala>();
        }
    }
}