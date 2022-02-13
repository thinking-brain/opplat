using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ContratacionWebApi.Models;

namespace ContratacionWebApi.Dtos {
    public class EditJuridico {
        public int ContratoId { get; set; }
        public string Numero { get; set; }
        public DateTime FechaDeFirmado { get; set; }
        public DateTime FechaDeVencimiento { get; set; }
        public string UserName { get; set; }
    }
}