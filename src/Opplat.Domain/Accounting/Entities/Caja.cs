using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opplat.Contabilidad.Domain.Entities;
public class Caja
{
    public int Id { get; set; }

    public string Descripcion { get; set; }

    public virtual ICollection<DenominacionEnCaja> Efectivo { get; set; }

    public Caja()
    {
        Efectivo = new HashSet<DenominacionEnCaja>();
    }
}
