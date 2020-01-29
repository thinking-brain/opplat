using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanzasWebApi.Models
{
    public class CacheEstadoFinanciero
    {
        public int Id { get; set; }

        public string Concepto { get; set; }
        public string Grupo { get; set; }
        public string EFE { get; set; }

        public int Mes { get; set; }

        public int Year { get; set; }

        public DateTime FechaActualizado { get; set; }

        public decimal Saldo { get; set; }
        public decimal PlanAnual { get; set; }
        public decimal Apertura { get; set; }
    }
}