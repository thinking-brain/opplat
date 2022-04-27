using System;
using System.Collections.Generic;

namespace Opplat.MainApp.Areas.Caja.ViewModels;

public class ResumenDeOperaciones
{
    public DateTime Fecha { get; set; }

    public string Detalle { get; set; }

    public string Tipo { get; set; }

    public decimal Importe { get; set; }

    public string CentroDeCosto { get; set; }

    public string Usuario { get; set; }
}
