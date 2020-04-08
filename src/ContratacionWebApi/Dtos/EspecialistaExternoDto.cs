using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ContratacionWebApi.Models;

namespace ContratacionWebApi.Models {
    public class EspecialistaExternoDto {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public int EntidadId { get; set; }
        public string Area { get; set; }
        public string Departamento { get; set; }
        public string Cargo { get; set; }

        [NotMapped]
        private string nombreCompleto;

        [NotMapped]
        public string NombreCompleto {
            get { return Nombre + " " + Apellidos; }
            set { nombreCompleto = value; }
        }

    }
}