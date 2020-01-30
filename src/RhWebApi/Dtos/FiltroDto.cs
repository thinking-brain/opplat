using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using RhWebApi.Data;
using RhWebApi.Models;

namespace RhWebApi.Dtos {

    public class FiltroDto {
        public string UnidadOrganizativa { get; set; }
        public string Cargo { get; set; }
        public string Sexo { get; set; }
        public string ColorDePiel { get; set; }
        public string NivelDeEscolaridad { get; set; }
        public string Edad { get; set; }
        public string Estado { get; set; }
    }
}