using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models
{
    [Table("bajas")]
    public class Baja
    {
        public int Id { get; set; }

        public int TrabajadorId { get; set; }

        public virtual Trabajador Trabajador { get; set; }

        public DateTime Fecha { get; set; }

        public CausaDeBaja CausaDeBaja { get; set; }
    }
}