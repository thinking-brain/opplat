using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RhWebApi.Models;

namespace RhWebApi.Dtos {
    public class OtroMovimientoDto {
        public int Id { get; set; }
        public int TrabajadorId { get; set; }
        public string Trabajador { get; set; }
        public string Nombre { get; set; }
        public DateTime CuandoSeHizo { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
    }
}