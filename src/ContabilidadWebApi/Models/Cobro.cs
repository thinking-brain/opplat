using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ContabilidadWebApi.Models
{
    public class Cobro
    {
        public int Id { get; set; }

        public decimal Importe { get; set; }

        public int DiaContableId { get; set; }

        public virtual DiaContable DiaContable { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }

        public int AsientoId { get; set; }

        public virtual Asiento Asiento { get; set; }

        [Required]
        public string UsuarioId { get; set; }
    }
}
