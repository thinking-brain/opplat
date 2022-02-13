using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RhWebApi.Models;

namespace RhWebApi.Dtos {
    public class PlantillaDto {
       
        public int Id { get; set; }
        public int CargoId { get; set; }
        public int UnidadOrganizativaId { get; set; }
        public int PlantillaAprobada { get; set; }
    }
}