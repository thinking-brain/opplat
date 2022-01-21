using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Data;

namespace RhWebApi.Models {
    [Table ("OtrosMovimientos")]
    public class OtroMovimiento : Movimiento {
        public string Nombre { get; set; }
        public Estados EstadoOrden { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
    }
}