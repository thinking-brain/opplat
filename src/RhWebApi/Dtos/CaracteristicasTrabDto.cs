using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Data;

namespace RhWebApi.Dtos {
    public class CaracteristicasTrabDto {
        public int Id { get; set; }
        public int? TrabajadorId { get; set; }

        public ColorDePiel ColorDePiel { get; set; }

        public ColorDeOjos ColorDeOjos { get; set; }

        public string TallaPantalon { get; set; }

        public TallaDeCamisa TallaDeCamisa { get; set; }

        public double TallaCalzado { get; set; }

        public string OtrasCaracteristicas { get; set; }

        [NotMapped]
        private string resumen;

        [NotMapped]
        public string Resumen {
            get { return String.Format ("TallaPantalon {0} , TallaCalzado {1} , OtrasCaracteristicas{2}", TallaPantalon, TallaCalzado, OtrasCaracteristicas); }
            set { resumen = value; }
        }
    }
}