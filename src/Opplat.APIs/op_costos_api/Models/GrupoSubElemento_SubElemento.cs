using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace op_costos_api.Models
{
    [Table("Costos_GrupoSubElemento_SubElemento")]
    public class GrupoSubElemento_SubElemento
    {
        public int Id { get; set; }

        public int GrupoSubelementoId { get; set; }

        public virtual GrupoSubelemento GrupoSubelemento { get; set; }

        public string SubElementoGastoId { get; set; }
        public string Descripcion { get; set; }
    }
}
