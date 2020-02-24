using System;

namespace ImportadorDatos.Models.EnlaceVersat
{
    public class SubElementoDeGasto
    {
        public int Id { get; set; }

        public int SubElementoId { get; set; }

        public int SubElementoVersatId { get; set; }

        public DateTime Fecha { get; set; }
    }
}