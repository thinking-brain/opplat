using System;

namespace ImportadorDatos.Models.EnlaceVersat
{
    public class RegistroDeGasto
    {
        public int Id { get; set; }

        public int RegistroId { get; set; }

        public int RegistroVersatId { get; set; }

        public DateTime Fecha { get; set; }
    }
}