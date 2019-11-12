using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models {
    [Table ("Traslados")]
    public class Traslado : Movimiento {
        public int? CargoOrigenId { get; set; }
        public Cargo CargoOrigen { get; set; }

        public int CargoDestinoId { get; set; }
        public Cargo CargoDestino { get; set; }

        public int UnidadOrganizativaId { get; set; }
        public UnidadOrganizativa UnidadOrganizativa { get; set; }
    }
}