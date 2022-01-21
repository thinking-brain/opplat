using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using TallerWebApi.Data;
using TallerWebApi.Models;

namespace TallerWebApi.Models {
    public class EquipoDto {
        public int Id { get; set; }
        public int NumeroSerie { get; set; }
        public DateTime FechaFabricacion { get; set; }
        public int TipoEquipo { get; set; }
        public int Marca { get; set; }
        public int Modelo { get; set; }
        public int Cliente { get; set; }
        public string Observaciones { get; set; }
        public EstadoEquipo EstadoEquipo { get; set; }
        public bool Activo { get; set; }
    }
}