using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Opplat.Contabilidad.Domain.Entities;
public class CierreDeCaja
{
    public int Id { get; set; }

    public int DiaContableId { get; set; }

    public virtual DiaContable DiaContable { get; set; }

    public DateTime Fecha { get; set; }

    public int CajaId { get; set; }

    public virtual Caja Caja { get; set; }

    public virtual ICollection<DenominacionesEnCierreDeCaja> Desglose { get; set; }

    public CierreDeCaja()
    {
        Desglose = new HashSet<DenominacionesEnCierreDeCaja>();
    }
}
