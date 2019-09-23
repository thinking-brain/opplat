using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models
{
    [Table("trabajadores")]
    public class Trabajador
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string CI { get; set; }
        public string Telefono { get; set; }
        public Sexo Sexo { get; set; }       

        public int DireccionId { get; set; }
        public virtual Direccion Direccion { get; set; }

        public virtual NivelDeEscolaridad NivelDeEscolaridad { get; set; }

        public Area Area { get; set; }

        public Cargo Cargo { get; set; }

        public virtual PuestoDeTrabajo PuestoDeTrabajo { get; set; }

        public virtual CaracteristicasTrabajador Caracteristicas { get; set; }

        public DateTime FechaContratacion { get; set; }
        public TipoDeContratoTrabajador TipoDeContrato { get; set; }
        public virtual ICollection<HistoricoPuestoDeTrabajo> PuestosOcupados { get; set; }
        public EstadoTrabajador Estado { get; set; }
        
          [NotMapped]
        private string _nombreCompleto;

        [NotMapped]
        public string NombreCompleto
        {
            get { return Nombre + " " + PrimerApellido + " " + SegundoApellido; }
            set { _nombreCompleto = value; }
        }
    }
}