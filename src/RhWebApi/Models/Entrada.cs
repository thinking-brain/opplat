using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models {
    [Table ("Entradas")]
    public class Entrada : Movimiento {
        public int CargoId { get; set; }
        public virtual Cargo Cargo { get; set; }
        public int? UnidadOrganizativaId { get; set; }
        public UnidadOrganizativa UnidadOrganizativa { get; set; }

    }
}