using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ContratacionWebApi.Models {
    public class DocumentoDeContrato {
        public int Id { get; set; }

        [Display (Name = "Número")]
        public string Numero { get; set; }

        [Display (Name = "Número Oficial")]
        public string NoOficial { get; set; }

        [Display (Name = "Fecha de firmado")]
        [DataType (DataType.Date)]
        public DateTime? FechaFirmado { get; set; }

        public string Dictamen { get; set; }

        [Display (Name = "Objetos de Contrato")]
        public virtual ICollection<ObjetoDeContrato> ObjetosDeContrato { get; set; }

        [Display (Name = "Monto CUP")]
        [DataType (DataType.Currency)]
        public decimal? MontoCup { get; set; }

        [Display (Name = "Monto CUC")]
        [DataType (DataType.Currency)]
        public decimal? MontoCuc { get; set; }

        [DataType (DataType.Date)]
        [Display (Name = "Fecha de Vencimiento")]
        public DateTime? FechaDeVencimiento { get; set; }

        [Display (Name = "Administrador")]
        public int AdminContratoId { get; set; }

        public virtual AdminContrato AdminContrato { get; set; }

        public string RevisionActual { get; set; }

        public virtual ICollection<Especialidad> Especialidades { get; set; }

        public virtual ICollection<HistoricoDeDocumento> Historicos { get; set; }

        public DocumentoDeContrato () {
            ObjetosDeContrato = new HashSet<ObjetoDeContrato> ();
            Especialidades = new HashSet<Especialidad> ();
            Historicos = new HashSet<HistoricoDeDocumento> ();
        }

        //     [NotMapped]
        //     public bool SeDictamino
        //     {
        //         get
        //         {
        //             if (Versiones.Count == 0)
        //             {
        //                 return false;
        //             }
        //             return Versiones.Last().Revisiones.Count == Especialidades.Count && Especialidades.Count > 0;
        //         }
        //     }

        //     [NotMapped]
        //     public string FechaAprobado
        //     {
        //         get
        //         {
        //             return Versiones.Last().Aprobaciones.Aggregate("", (current, aprob) => current + (aprob.Fecha.ToShortDateString() + " "));
        //         }
        //     }

        //     [NotMapped]
        //     public VersionDocumento UltimaRevision
        //     {
        //         get
        //         {
        //             return Versiones.Last();
        //         }
        //     }

        //     [NotMapped]
        //     public VersionDocumento VersionInicial
        //     {
        //         get
        //         {
        //             return Versiones.First();
        //         }
        //     }
    }
}