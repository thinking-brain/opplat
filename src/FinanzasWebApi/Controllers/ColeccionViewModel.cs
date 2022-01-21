using System.Collections.Generic;
using FinanzasWebApi.ViewModels;

namespace FinanzasWebApi.Controllers
{
    public class ColeccionViewModel
    {
        public string Nombre { get; set; }
        public List<EstadoFinancieroJsVM> Data { get; set; }
    }
}