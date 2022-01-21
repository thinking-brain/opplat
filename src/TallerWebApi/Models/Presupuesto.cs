using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using TallerWebApi.Data;
using TallerWebApi.Models;

namespace TallerWebApi.Models {
    public class Presupuesto {
        public int Id { get; set; }
        public double ManoObra { get; set; }
        public double Impuesto { get; set; }
        public string DetalleManoObra { get; set; }
        public string DetalledeRepuesto { get; set; }
        public DateTime Fecha { get; set; }
        public bool NotificadoCliente { get; set; }
        public EstadoPresupuesto EstadoPresupuesto { get; set; }
        public string InformeCliente { get; set; }
        public bool Garantia { get; set; }
        public DateTime FechaGarantia { get; set; }
        public bool Activo { get; set; }

    }
}