using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

<<<<<<< HEAD
namespace ContratacionWebApi.Models {
=======
namespace ContratacionWebApi.Models
{
>>>>>>> 74f96f4520b199584c84165057d3754bc522849c
    public class Contrato {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public Tipo Tipo { get; set; }
        public int AdminContratoId { get; set; }
        public int EntidadId { get; set; }
        public virtual Entidad Entidad { get; set; }
        public string ObjetoDeContrato { get; set; }
        public string Numero { get; set; }
<<<<<<< HEAD

=======
>>>>>>> 74f96f4520b199584c84165057d3754bc522849c
        [Display (Name = "Monto CUP")]
        [DataType (DataType.Currency)]
        public decimal? MontoCup { get; set; }

        [Display (Name = "Monto CUC")]
        [DataType (DataType.Currency)]
        public decimal? MontoCuc { get; set; }

        [DataType (DataType.Date)]
        [Display (Name = "Fecha de Llegada")]
        public DateTime FechaDeLlegada { get; set; }
<<<<<<< HEAD

        [DataType (DataType.Date)]
        [Display (Name = "Fecha de Firmado")]
        public DateTime? FechaDeFirmado { get; set; }
=======
>>>>>>> 74f96f4520b199584c84165057d3754bc522849c

        [DataType (DataType.Date)]
        [Display (Name = "Fecha de Vencimiento")]
        public DateTime? FechaDeVencimiento { get; set; }
        public DateTime? FechaDeFirmado { get; set; }

        [Display (Name = "Formas de Pago")]
        public virtual ICollection<FormaDePago> FormasDePago { get; set; }

<<<<<<< HEAD
        //Término de pago en días
=======
>>>>>>> 74f96f4520b199584c84165057d3754bc522849c
        [Display (Name = "Término de Pago")]
        public int TerminoDePago { get; set; }
        public virtual ICollection<HistoricoEstadoContrato> Estados { get; set; }

        [NotMapped]
        public string Descripcion => $"{Entidad.Nombre}-{Tipo} ({Numero})";

        [NotMapped]
        public Estado Estado {
            get {
                var estadoActual = Estados.OrderBy (e => e.Fecha).Last ();
                if (estadoActual == null) {
                    return Estado.SinEstado;
                }
                return estadoActual.Estado;
            }
        }

        public Contrato () {
            Estados = new HashSet<HistoricoEstadoContrato> ();
        }

    }
}