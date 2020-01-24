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
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        [Required (ErrorMessage = "El Campo {0} es Obligatorio")]
        public string CI { get; set; }
        public string TelefonoFijo { get; set; }
        public string TelefonoMovil { get; set; }
        public string Correo { get; set; }
        public Sexo? Sexo { get; set; }
        public string Direccion { get; set; }
        public int? MunicipioId { get; set; }
        public virtual Municipio Municipio { get; set; }
        public string Perfil_Ocupacional { get; set; }
        public int? PuestoDeTrabajoId { get; set; }
        public virtual PuestoDeTrabajo PuestoDeTrabajo { get; set; }
        public virtual NivelDeEscolaridad NivelDeEscolaridad { get; set; }
        public virtual Estados EstadoTrabajador { get; set; }
        public virtual CaracteristicasTrab CaracteristicasTrab { get; set; }
    }
}