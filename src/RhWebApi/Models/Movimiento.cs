using System;

namespace RhWebApi.Models {
    public class Movimiento {
        public int Id { get; set; }       

        public DateTime Fecha { get; set; }
        public int TrabajadorId { get; set; }
        public virtual Trabajador Trabajador { get; set; }
    }
}