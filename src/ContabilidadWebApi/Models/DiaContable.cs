using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContabilidadWebApi.Models
{
    [Table("contb_dia_contable")]
    public class DiaContable
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public bool Abierto { get; set; }

        public DateTime? HoraEnQueCerro { get; set; }

        public int PeriodoContableId { get; set; }

        public virtual PeriodoContable PeriodoContable { get; set; }
        public virtual ICollection<Asiento> Asientos { get; set; }
        public DiaContable()
        {
            Asientos = new HashSet<Asiento>();
        }
    }
}