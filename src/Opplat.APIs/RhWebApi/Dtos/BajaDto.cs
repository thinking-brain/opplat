using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using RhWebApi.Data;

namespace RhWebApi.Dtos {

    public class BajaDto {

        public int Id { get; set; }
        public int TrabajadorId { get; set; }
        public string Trabajador { get; set; }
        public CausaDeBaja CausaDeBaja { get; set; }
        public DateTime Fecha { get; set; }
    }
}