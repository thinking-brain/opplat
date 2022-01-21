using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ContratacionWebApi.Models;

namespace ContratacionWebApi.Dtos {
    public class Dashboard {
        public int[] OfertasProceso { get; set; }
        public int[] OfertasVencidas { get; set; }
        public int OfertasEnProceso { get; set; }
        public int OfertasProcesadas { get; set; }
        public int OfertasVenHastaFecha { get; set; }
        public int OfertasVenEsteMes { get; set; }
        public double PromCircuOferta { get; set; }
        public double PromCircuOfertaMes { get; set; }
        public int[] ContratosProceso { get; set; }
        public int[] ContratosVencidas { get; set; }
        public string[] TiempoCircuOfertas { get; set; }
        public int[] ContratosProximosVencer { get; set; }
        public int[] ContratosTipo { get; set; }
        public AdminCont_Cant[] AdminContratos_Cant { get; set; }
    }
}