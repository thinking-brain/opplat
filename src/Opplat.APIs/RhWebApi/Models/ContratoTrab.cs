using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models {
    [Table ("ContratoTrabs")]
    public class ContratoTrab {
        public int Id { get; set; }
        public string CentroDeCosto { get; set; }
        public string Descripcion { get; set; }

        [DataType (DataType.Currency)]
        public decimal MontoCUC { get; set; }

        [DataType (DataType.Currency)]
        public decimal MontoCUP { get; set; }
        public int EmpresaId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaTerminado { get; set; }
        public bool Pagado { get; set; }
        public virtual Boolean Sobregirado { get; set; }
        public virtual ICollection<ActividadContratoTrab> ActividadContratoTrabs { get; set; }

    }
}