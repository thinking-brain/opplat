using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models
{
    [Table("caracteristicas_del_trabjador")]
    public class CaracteristicasTrabajador
    {
        [Key]
        public int TrabajadorId { get; set; }

        public virtual Trabajador Trabajador { get; set; }

        public byte[] Foto { get; set; }

        [Display(Name = "Color de piel")]
        public ColorDePiel ColorDePiel { get; set; }

        public ColorDeOjos ColorDeOjos { get; set; }

        public double TallaPantalon { get; set; }

        public TallaDeCamisa TallaDeCamisa { get; set; }

        public double TallaCalzado { get; set; }

        public string OtrasCaracteristicas { get; set; }
    }
}