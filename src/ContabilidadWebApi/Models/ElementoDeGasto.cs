using System.Collections.Generic;

namespace ContabilidadWebApi.Models
{
    public class ElementoDeGasto
    {
        public int Id { get; set; }

        public string Descripcion { get; set; }

        public string Codigo { get; set; }

        public bool Activo { get; set; }

        public virtual ICollection<SubElementoDeGasto> Subelementos { get; set; }
    }
}