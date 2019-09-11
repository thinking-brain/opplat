using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace op_costos_api.Models
{
    [Table("Costos_SubmayorDeCuentas")]
    public class SubMayorCuenta
    {
        public int Id { get; set; }
        public int IdCuenta { get; set; }
        public string ClaveCuenta { get; set; }
        public int Mes { get; set; }
        public int Año { get; set; }
        public decimal Importe { get; set; }

    }
}
