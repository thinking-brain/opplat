using System.Collections.Generic;

namespace FinanzasWebApi.ViewModels
{
    public class EstadoFinancieroVM
    {
        public string Concepto { get; set; }
        public string EFE { get; set; }
        public decimal Valor { get; set; }
        public List<DetalleEstadoFinancieroVM> Detalles { get; set; }


    }
}