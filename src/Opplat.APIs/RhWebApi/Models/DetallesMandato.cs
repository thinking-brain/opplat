using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models
{
    [Table("detalles_de_mandatos")]
    public class DetallesMandato
    {
        public virtual int Id { get; set; }
        public virtual Decimal PagoCUC { get; set; }

        public virtual Decimal PagoCUP { get; set; }

        public virtual ActividadContratoTrab Actividad { get; set; }
        public int ActividadContratoTrabId { get; set; }

        public virtual MandatoDePago MandatoDePago { get; set; }
        public int MandatoDePagoId { get; set; }
        public virtual Trabajador Trabajador { get; set; }
        public int TrabajadorId { get; set; }
        /// <summary>
        /// Campo a guardar con las horas trabajadas
        /// http://stackoverflow.com/questions/8503132/how-do-i-map-timespan-with-greater-than-24-hours-to-sql-server-code-first
        /// </summary>
        public virtual int Horas { get; set; }
    }
}