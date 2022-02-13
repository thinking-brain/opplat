using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using RhWebApi.Utils;

namespace RhWebApi.Models {
    public class Plantilla {
        public int Id { get; set; }
        public int CargoId { get; set; }
        public virtual Cargo Cargo { get; set; }
        public int UnidadOrganizativaId { get; set; }
        public virtual UnidadOrganizativa UnidadOrganizativa { get; set; }
        public int PlantillaAprobada { get; set; }
    }
}