using System;
using System.ComponentModel.DataAnnotations.Schema;
using RhWebApi.Data;

namespace RhWebApi.Models {
    [Table ("caracteristicas_de_los_socios")]
    public class CaracteristicasSocio {
        public int Id { get; set; }
        public string Direccion { get; set; }
        public Sexo? Sexo { get; set; }
        public ColorDePiel ColorDePiel { get; set; }
        public int? MunicipioId { get; set; }
        public virtual Municipio Municipio { get; set; }
        public string Perfil_Ocupacional { get; set; }
        public virtual NivelDeEscolaridad NivelDeEscolaridad { get; set; }

    }
}