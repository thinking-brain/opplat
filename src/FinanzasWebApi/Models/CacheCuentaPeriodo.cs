using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanzasWebApi.Models
{
    public class CacheCuentaPeriodo
    {
        public int Id { get; set; }

        public string Cuenta { get; set; }

        public int Mes { get; set; }

        public int Year { get; set; }

        public DateTime FechaActualizado { get; set; }

        public decimal Saldo { get; set; }

        public decimal Acumulado { get; set; }
    }
}