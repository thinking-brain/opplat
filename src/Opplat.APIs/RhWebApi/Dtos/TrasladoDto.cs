using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RhWebApi.Models;

namespace RhWebApi.Dtos {
    public class TrasladoDto {
        public int Id { get; set; }
        public int TrabajadorId { get; set; }
        public int CargoOrigenId { get; set; }
        public int CargoDestinoId { get; set; }
        public int UnidOrgOrigenId { get; set; }
        public int UnidOrgDestinoId { get; set; }
        public DateTime Fecha { get; set; }
    }
}