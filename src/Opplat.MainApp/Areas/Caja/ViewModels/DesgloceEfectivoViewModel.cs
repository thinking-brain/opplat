﻿using System.Collections.Generic;

namespace Opplat.MainApp.Areas.Caja.ViewModels;

public class DesgloceEfectivoViewModel
{
    public ICollection<DenominacionViewModel> Billetes { get; set; }

    public ICollection<DenominacionViewModel> Monedas { get; set; }

    public DesgloceEfectivoViewModel()
    {
        Billetes = new List<DenominacionViewModel>();
        Monedas = new List<DenominacionViewModel>();
    }
}
