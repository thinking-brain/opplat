using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContabilidadWebApi.ViewModels
{
    public class PlanesViewModel
    {
        public int CentroCostoAreaId { get; set; }
        public string Cuenta { get; set; }
        public string SubCuenta { get; set; }
        public string Analisis { get; set; }
        public string SubAnalisis { get; set; }
        public string CentroCosto { get; set; }
        public string Elemento { get; set; }
        public string SubElemento { get; set; }
        public string Descripcion { get; set; }
        public decimal Enero { get; set; }
        public decimal Febrero { get; set; }
        public decimal Marzo { get; set; }
        public decimal Abril { get; set; }
        public decimal Mayo { get; set; }
        public decimal Junio { get; set; }
        public decimal Julio { get; set; }
        public decimal Agosto { get; set; }
        public decimal Septiembre { get; set; }
        public decimal Octubre { get; set; }
        public decimal Noviembre { get; set; }
        public decimal Diciembre { get; set; }
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

    }
}
