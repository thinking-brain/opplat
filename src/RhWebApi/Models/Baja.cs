using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Data;

namespace RhWebApi.Models {
    [Table ("bajas")]
    public class Baja : Movimiento {
              public CausaDeBaja CausaDeBaja { get; set; }
    }
}