using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models
{
    [Table("contratos")]
    public class Contrato
    {
        public int Id { get; set; }

        [Display(Name = "Centro de Costo")]
        public string CentroDeCosto { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Valor de la mano de obra en CUC")]
        [DataType(DataType.Currency)]
        public decimal MontoCUC { get; set; }

        [Display(Name = "Valor de la mano de obra en CUP")]
        [DataType(DataType.Currency)]
        public decimal MontoCUP { get; set; }      

        [Display(Name = "Empresa que Recibe el Servicio")]
        public int EmpresaId { get; set; }
       
        [Display(Name = "Fecha  de Inicio")]
        public DateTime FechaInicio { get; set; }

        [Display(Name = "Fecha de Finalizado")]
        public DateTime? FechaTerminado { get; set; }
        public bool Pagado { get; set; }
        public virtual Boolean Sobregirado { get; set; }
        public virtual ICollection<ActividadContrato> ActividadContratos { get; set; }

    }
}