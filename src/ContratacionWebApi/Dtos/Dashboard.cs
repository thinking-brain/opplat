using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ContratacionWebApi.Models;

namespace ContratacionWebApi.Models {
    public class Dashboard {
        public int[] OfertasProceso { get; set; }
        public int[] OfertasVencidas { get; set; }
        public int[] ContratosProceso { get; set; }
        public int[] ContratosVencidas { get; set; }
        public string[] TiempoCircuOfertas { get; set; }
        public int[] ContratosProximosVencer { get; set; }
        public AdminCont_Cant[] AdminContratos_Cant { get; set; }
    }
}