using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ContratacionWebApi.Models;

namespace ContratacionWebApi.Dtos {
    public class AproContratoDto {
        public List<string> roles { get; set; }
        public int ContratoId { get; set; }
        public string UserName { get; set; }
        public DateTime FechaDeFirmado { get; set; }
        public DateTime FechaDeVencimiento { get; set; }
        public Estado Estado { get; set; }
        public string Numero { get; set; }
    }
}