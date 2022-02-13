using System.Collections.Generic;

namespace FinanzasWebApi.ViewModels
{
    public class EfViewModel
    {
        public string concepto { get; set; }
        public string elemento { get; set; }
        public List<EfsViewModel> conceptos { get; set; }
    }
}