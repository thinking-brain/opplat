using System.Collections.Generic;

namespace FinanzasWebApi.ViewModels
{
    public class EfsViewModel
    {
        public string name { get; set; }
        public string celda { get; set; }
        public bool visible { get; set; }

        public List<int> cuentas { get; set; }

        public string elemento { get; set; }
        public List<int> sub_elementos { get; set; }
    }
}