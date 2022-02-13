using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContabilidadWebApi.Models
{
    [Table("contb_disponibilidades")]
    public class Disponibilidad
    {
        [Key]
        public int CuentaId { get; set; }

        public virtual Cuenta Cuenta { get; set; }

        public DateTime Fecha { get; set; }

        public decimal Saldo { get; set; }
    }
}