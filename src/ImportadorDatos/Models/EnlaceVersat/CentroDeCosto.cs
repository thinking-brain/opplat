using System;

namespace ImportadorDatos.Models.EnlaceVersat
{
    public class CentroDeCosto
    {
        public int Id { get; set; }

        public int CentroId { get; set; }

        public int CentroVersatId { get; set; }

        public DateTime Fecha { get; set; }
    }
}