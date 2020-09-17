using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
// using RhWebApi.Models;

namespace ContratacionWebApi.Models {
    public class Contrato {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public Tipo Tipo { get; set; }
        //Trabajador
        public int AdminContratoId { get; set; }
        // public Trabajador AdminContrato { get; set; }
        public int EntidadId { get; set; }
        public virtual Entidad Entidad { get; set; }
        public string ObjetoDeContrato { get; set; }
        public string Numero { get; set; }
        public ICollection<Monto> Montos { get; set; }
        
        [DataType (DataType.Date)]
        [Display (Name = "Fecha de Llegada")]
        public DateTime FechaDeRecepcion { get; set; }

        [DataType (DataType.Date)]
        [Display (Name = "Fecha de Firmado")]
        public DateTime FechaDeFirmado { get; set; }

        [DataType (DataType.Date)]
        [Display (Name = "Fecha de Vencimiento")]
        public DateTime FechaVenContrato { get; set; }

        [DataType (DataType.Date)]
        public DateTime FechaDeVenOferta { get; set; }
        public string FilePath { get; set; }

        [NotMapped]
        [Display (Name = "Formas de Pago")]
        public virtual ICollection<FormaDePago> FormasDePago { get; set; }

        [NotMapped]
        public ICollection<Departamento> Departamentos { get; set; }

        [NotMapped]
        public List<EspecialistaExterno> EspecialistasExternos { get; set; }
        //Término de pago en días
        [Display (Name = "Término de Pago")]
        public int TerminoDePago { get; set; }

        [NotMapped]
        public virtual ICollection<HistoricoEstadoContrato> Estados { get; set; }
        public Estado EstadoEconomico { get; set; }
        public Estado EstadoJuridico { get; set; }
        public Estado EstadoComitContratacion { get; set; }
        public Estado EstadoContrato { get; set; }
        public bool Cliente { get; set; }
        public virtual ICollection<Dictamen> Dictamenes { get; set; }
        public virtual ICollection<Suplemento> Suplementos { get; set; }

        // [NotMapped]
        // public string Descripcion => $"{Entidad.Nombre}-{Tipo} ({Numero})";

        // [NotMapped]
        // public Estado EstadoActual {
        //     get {
        //         var estadoActual = Estados.OrderBy (e => e.Fecha).Last ();
        //         if (estadoActual == null) {
        //             return Estado.SinEstado;
        //         }
        //         return estadoActual.Estado;
        //     }
        // }

        public Contrato () {
            Estados = new HashSet<HistoricoEstadoContrato> ();
        }
    }
}