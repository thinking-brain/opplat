using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RhWebApi.Models;

namespace RhWebApi.Dtos {
    public class CargoDto {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Sigla { get; set; }
        public int GrupoEscalaId { get; set; }
        public int JefeId { get; set; }
    }
}