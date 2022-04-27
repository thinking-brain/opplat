using System.ComponentModel.DataAnnotations.Schema;

namespace Opplat.Contabilidad.Domain.Entities;
    [Table("contb_centros_de_costo")]
    public class CentroDeCosto
    {
        public int Id { get; set; }

        public string Codigo { get; set; }

        public string Nombre { get; set; }
    }
