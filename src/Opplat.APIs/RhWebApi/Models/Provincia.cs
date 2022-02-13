using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models
{
    [Table("provincias")]
    public class Provincia
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        [Display(Name = "Municipios")]
        public virtual ICollection<Municipio> Municipios { get; set; }

        public Provincia()
        {
            Municipios = new HashSet<Municipio>();
        }
    }
}