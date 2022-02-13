﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace op_costos_api.ViewModels
{
    public class CostosGroupApiVM
    {
        public string Titulo { get; set; }
        public string Fecha { get; set; }
        public Dictionary<string, Dictionary<string, CostosViewModel>> Valores { get; set; }
        public Dictionary<string, List<decimal>> Parciales { get; set; }
        public List<decimal> General { get; set; }
    }
}
