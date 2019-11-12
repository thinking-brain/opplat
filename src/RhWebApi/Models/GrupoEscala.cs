using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models
{
    [Table("grupos_escalas")]
    public class GrupoEscala
    {
        public int Id { get; set; }

        public string Codigo { get; set; }

        public int CategoriaOcupacionalId { get; set; }

        public virtual CategoriaOcupacional CategoriaOcupacional { get; set; }

        public bool SalarioDiferenciado { get; set; }

        public decimal SalarioEscala { get; set; }

        public virtual ICollection<Cargo> Cargos { get; set; }
    }
}