using System;

namespace ImportadorDatos.Models.EnlaceVersat
{
    public class PartidaDeGasto
    {
        public int Id { get; set; }

        public int PartidaId { get; set; }

        public int PartidaVersatId { get; set; }

        public DateTime Fecha { get; set; }
    }
}