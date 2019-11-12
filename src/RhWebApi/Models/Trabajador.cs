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
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string CI { get; set; }
        public string TelefonoFijo { get; set; }
        public string TelefonoMovil { get; set; }
        public virtual Sexo Sexo { get; set; }
        public string Direccion { get; set; }
        public int? MunicipioId { get; set; }
        public virtual Municipio Municipio { get; set; }
        public int? PuestoDeTrabajoId { get; set; }
        public virtual PuestoDeTrabajo PuestoDeTrabajo { get; set; }
        public virtual NivelDeEscolaridad NivelDeEscolaridad { get; set; }
        public string EstadoTrabajador { get; set; }     
    }
}