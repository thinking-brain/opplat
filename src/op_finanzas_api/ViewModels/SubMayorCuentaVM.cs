using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace op_finanzas_api.Models
{
     public class SubMayorCuentaVM
    {
        public int Id { get; set; }
        public int IdCuenta { get; set; }
        public int? IdPaseDeCuenta { get; set; }
        public string ClaveCuenta { get; set; }
        public int Mes { get; set; }
        public int Año { get; set; }
        public decimal Importe { get; set; }

    }
}
