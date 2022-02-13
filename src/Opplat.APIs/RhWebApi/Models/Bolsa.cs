using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Data;

namespace RhWebApi.Models {
    public class Bolsa {
        public int Id { get; set; }
        public int TrabajadorId { get; set; }
        public virtual Trabajador Trabajador { get; set; }
        public DateTime Fecha { get; set; }
        public string Nombre_Referencia{ get; set; }
    }
}