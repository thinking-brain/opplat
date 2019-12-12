using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models {
    public class Requisitos {

        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int? Cargo { get; set; }
        public virtual Cargo Cargo { get; set; }

    }
}