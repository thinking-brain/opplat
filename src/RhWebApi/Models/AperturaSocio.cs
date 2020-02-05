using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Data;
using RhWebApi.Models;

namespace RhWebApi.Models {
    public class AperturaSocio {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int CantTrabajadores { get; set; }
        public int NumeroAcuerdo { get; set; }
        public virtual ICollection<Trabajador> ListaTrabajadores { get; set; }
        public int? CaracteristicasSocioId { get; set; }
        public virtual CaracteristicasSocio CaracteristicasSocio { get; set; }
        public bool Cerrada { get; set; }
    }
}