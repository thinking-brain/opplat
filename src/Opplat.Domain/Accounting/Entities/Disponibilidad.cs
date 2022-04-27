using System;
using System.ComponentModel.DataAnnotations;

namespace Opplat.Contabilidad.Domain.Entities;
    public class Disponibilidad
    {
        [Key]
        public int CuentaId { get; set; }

        public virtual Cuenta Cuenta { get; set; }

        public DateTime Fecha { get; set; }

        public decimal Saldo { get; set; }
    }
