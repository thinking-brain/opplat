using System;

namespace ImportadorDatos.Models.EnlaceVersat
{
    public class UnidadOrganizativa
    {
        public int Id { get; set; }

        public int UnidadOrganizativaId { get; set; }

        public int AreaVersatId { get; set; }

        public DateTime Fecha { get; set; }
    }
}