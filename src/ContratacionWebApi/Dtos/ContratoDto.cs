using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ContratacionWebApi.Models;
using Microsoft.AspNetCore.Http;

namespace ContratacionWebApi.Models {
    public class ContratoDto {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public Tipo Tipo { get; set; }
        public int AdminContrato { get; set; }
        public int Entidad { get; set; }
        public string ObjetoDeContrato { get; set; }
        public string Numero { get; set; }

        [Display (Name = "Monto CUP")]
        [DataType (DataType.Currency)]
        public decimal? MontoCup { get; set; }

        [Display (Name = "Monto CUC")]
        [DataType (DataType.Currency)]
        public decimal? MontoCuc { get; set; }

        [Display (Name = "Monto USD")]
        [DataType (DataType.Currency)]
        public decimal? MontoUsd { get; set; }
        public string[] monedas { get; set; }

        [DataType (DataType.Date)]
        [Display (Name = "Fecha de Llegada")]
        public DateTime FechaDeRecepcion { get; set; }

        [DataType (DataType.Date)]
        [Display (Name = "Fecha de Vencimiento")]
        public DateTime FechaVenContrato { get; set; }

        [DataType (DataType.Date)]
        [Required]
        public DateTime FechaDeVenOferta { get; set; }
        public DateTime FechaDeFirmado { get; set; }
        public IFormFile File;

        [Display (Name = "Formas de Pago")]
        public virtual List<FormaDePago> FormasDePago { get; set; }

        //Término de pago en días
        [Display (Name = "Término de Pago")]
        public int TerminoDePago { get; set; }
        public string UserName { get; set; }
        public List<int> Departamentos { get; set; }
        public List<int> EspecialistasExternos { get; set; }
        public List<Monto> Montos { get; set; }
        public EstadoOrden EstadoEconomico { get; set; }
        public EstadoOrden EstadoJuridico { get; set; }
        public EstadoOrden EstadoComitContratacion { get; set; }
        public EstadoOrden EstadoContrato { get; set; }
        public bool Cliente { get; set; }
        public int? ContratoId { get; set; }
        public string MotivoSuplemento { get; set; }
    }
}