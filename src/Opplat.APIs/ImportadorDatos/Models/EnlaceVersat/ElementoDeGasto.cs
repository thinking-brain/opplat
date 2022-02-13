using System;

namespace ImportadorDatos.Models.EnlaceVersat
{
    public class ElementoDeGasto
    {
        public int Id { get; set; }

        public int ElementoId { get; set; }

        public int ElementoVersatId { get; set; }

        public DateTime Fecha { get; set; }
    }
}