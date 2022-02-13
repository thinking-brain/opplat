using System;
using System.ComponentModel.DataAnnotations.Schema;
using RhWebApi.Data;

namespace RhWebApi.Models {
    [Table ("caracteristicas_de_los_socios")]
    public class CaracteristicasSocio {
        public int Id { get; set; }
        public string Direccion { get; set; }
        public Sexo? Sexo { get; set; }
        public int? MunicipioId { get; set; }
        public virtual Municipio Municipio { get; set; }
        public int? PerfilOcupacionalId { get; set; }
        public virtual PerfilOcupacional PerfilOcupacional { get; set; }
        public virtual NivelDeEscolaridad NivelDeEscolaridad { get; set; }
        public int? EdadDesde { get; set; }
        public int? EdadHasta { get; set; }

    }
}