using System.Collections.Generic;

namespace FinanzasWebApi.ViewModels
{
    public class EstadoFinancieroJsVM
    {
        public string Concepto { get; set; }
        public string EFE { get; set; }
        public decimal Plan { get; set; }
        public decimal Real { get; set; }
        public int Mes { get; set; }
        public string Celda { get; set; }
        public bool Visible { get; set; }

        public bool Encabezado { get; set; }
        //5922
        public string Elemento { get; set; }
        public string SubElementos { get; set; }



    }
}