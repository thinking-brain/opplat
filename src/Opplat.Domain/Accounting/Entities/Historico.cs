using System;

namespace Opplat.Contabilidad.Domain.Entities;
    public class Historico
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public string Descripcion { get; set; }
    }
