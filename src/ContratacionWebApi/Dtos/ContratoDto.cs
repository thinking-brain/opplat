using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ContratacionWebApi.Models;

namespace ContratacionWebApi.Models {
    public class ContratoDto {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public Tipo Tipo { get; set; }
        public int TrabajadorId { get; set; }
        public int EntidadId { get; set; }
        public string ObjetoDeContrato { get; set; }
        public string Numero { get; set; }

        [Display (Name = "Monto CUP")]
        [DataType (DataType.Currency)]
        public decimal? MontoCup { get; set; }

        [Display (Name = "Monto CUC")]
        [DataType (DataType.Currency)]
        public decimal? MontoCuc { get; set; }

        [DataType (DataType.Date)]
        [Display (Name = "Fecha de Llegada")]
        public DateTime FechaDeRecepcion { get; set; }

        [DataType (DataType.Date)]
        [Display (Name = "Fecha de Vencimiento")]
        public DateTime FechaVenContrato { get; set; }

        [DataType (DataType.Date)]
        [Required]
        public DateTime FechaDeVenOferta { get; set; }
        public int Vigencia { get; set; }
        public DateTime FechaDeFirmado { get; set; }

        [Display (Name = "Formas de Pago")]
        public virtual List<int> FormasDePago { get; set; }

        //Término de pago en días
        [Display (Name = "Término de Pago")]
        public double TerminoDePago { get; set; }
        public string Usuario { get; set; }
        public List<int> DictaminadoresId { get; set; }
        public List<int> EspExternoId { get; set; }
        public bool AprobJurico { get; set; }
        public bool AprobEconomico { get; set; }
        public bool AprobComitContratacion { get; set; }

    }
}