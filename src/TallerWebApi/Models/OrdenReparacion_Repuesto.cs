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
    public class OrdenReparacion_Repuesto {
        public int Id { get; set; }
        public int OrdenReparacionId { get; set; }
        public OrdenReparacion OrdenReparacion { get; set; }
        public int RepuestoId { get; set; }
        public Repuesto Repuesto { get; set; }
        public int Cantidad { get; set; }
                public bool Activo { get; set; }

    }
}