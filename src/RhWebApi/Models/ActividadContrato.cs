using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models
{
     [Table("actividades_de_ContratoTrabs")]
    public class ActividadContratoTrab
    {
        public virtual int Id { get; set; }
        public virtual int ActividadLaboralId { get; set; }
        public virtual ActividadLaboral ActividadLaboral { get; set; }

        public virtual ContratoTrab ContratoTrab { get; set; }
        public virtual int ContratoTrabId { get; set; }
        public virtual Decimal PrecioCUC { get; set; }
        public virtual Decimal PrecioCUP { get; set; }


        [NotMapped]
        public virtual String Codigo
        {
            get
            {
                if (ActividadLaboral == null)
                {
                    return String.Empty;
                }
                return ActividadLaboral.Codigo;
            }
        }
    }
}