using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Data;

namespace RhWebApi.Models {
    [Table ("trabajadores")]
    public class Trabajador {
        public int Id { get; set; }
        public string Codigo { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellidos { get; set; }

        [Required]
        [RegularExpression (@"^[0-9]*$", ErrorMessage = "Inserte un número de identidad válido")]
        [StringLength (11, MinimumLength = 11, ErrorMessage = "Inserte un número de identidad válido")]
        public string CI { get; set; }
        public string TelefonoFijo { get; set; }
        public string TelefonoMovil { get; set; }
        public string Correo { get; set; }
        public Sexo? Sexo { get; set; }
        public string Direccion { get; set; }
        public int? MunicipioId { get; set; }
        public virtual Municipio Municipio { get; set; }

        [Required]
        public string Perfil_Ocupacional { get; set; }
        public int? PuestoDeTrabajoId { get; set; }
        public virtual PuestoDeTrabajo PuestoDeTrabajo { get; set; }
        public virtual NivelDeEscolaridad NivelDeEscolaridad { get; set; }
        public virtual Estados EstadoTrabajador { get; set; }
        public virtual CaracteristicasTrab CaracteristicasTrab { get; set; }
        public int? AperturaSocioId { get; set; }
        public virtual AperturaSocio AperturaSocio { get; set; }
        public DateTime Fecha_Nac { get; set; }
    }
}