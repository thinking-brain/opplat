using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models
{
    [Table("historicos_de_puestos_de_trabajos")]
    public class HistoricoPuestoDeTrabajo
    {
        public int Id { get; set; }

        public int TrabajadorId { get; set; }

        public virtual Trabajador Trabajador { get; set; }

        public int PuestoDeTrabajoId { get; set; }

        public virtual PuestoDeTrabajo PuestoDeTrabajo { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime? FechaTerminado { get; set; }


    }
}