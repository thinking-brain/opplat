using System;

namespace ImportadorDatos.Models.EnlaceVersat
{
    public class PeriodosContables
    {
        public int Id { get; set; }

        public int PeriodoId { get; set; }

        public int PeriodoVersatId { get; set; }

        public DateTime Fecha { get; set; }
    }
}