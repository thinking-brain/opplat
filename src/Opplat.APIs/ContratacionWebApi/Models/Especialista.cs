using System.Collections.Generic;

namespace ContratacionWebApi.Models {
    public class Especialista {
        public int Id { get; set; }
        public int TrabajadorId { get; set; }

        public int EspecialidadId { get; set; }

        public virtual Especialidad Especialidad { get; set; }
        public string DetallesEspecialista { get; set; }

        public bool Activo { get; set; }

        public virtual ICollection<Dictamen> Dictamenes { get; set; }

        public Especialista () {
            Dictamenes = new HashSet<Dictamen> ();
        }

    }
}