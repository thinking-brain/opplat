using System;

namespace ImportadorDatos.Models.EnlaceVersat
{
    public class Cuentas
    {
        public int Id { get; set; }

        public int CuentaId { get; set; }

        public int CuentaVersatId { get; set; }

        public DateTime Fecha { get; set; }
    }
}