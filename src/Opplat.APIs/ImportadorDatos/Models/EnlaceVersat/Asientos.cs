using System;

namespace ImportadorDatos.Models.EnlaceVersat
{
    public class Asientos
    {
        public int Id { get; set; }

        public int AsientoId { get; set; }

        public int ComprobanteId { get; set; }

        public DateTime Fecha { get; set; }
    }
}