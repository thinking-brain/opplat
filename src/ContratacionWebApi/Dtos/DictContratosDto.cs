using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ContratacionWebApi.Models;

namespace ContratacionWebApi.Dtos {
    public class DictContratosDto {
        public List<int> Dictaminadores { get; set; }
        public int DepartamentoId { get; set; }
    }
}