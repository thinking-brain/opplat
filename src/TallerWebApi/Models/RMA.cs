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
    public class RMA {
        public int Id { get; set; }
        public DateTime FechaEnvio { get; set; }
        public int OrdenReparacionId { get; set; }
        public OrdenReparacion OrdenReparacion { get; set; }
        public int ProveedorId { get; set; }
        public Proveedor Proveedor { get; set; }
        public int EquipoId { get; set; }
        public Equipo Equipo { get; set; }
                public bool Activo { get; set; }

    }
}