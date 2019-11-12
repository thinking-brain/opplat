using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Dtos;
using RhWebApi.Models;

namespace RhWebApi.Dtos {
    public class GrupoEscalaDto {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public int CategoriaOcupacionalId { get; set; }
        public bool SalarioDiferenciado { get; set; }
        public decimal SalarioEscala { get; set; }
    }
}