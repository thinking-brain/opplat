using System.ComponentModel.DataAnnotations;

namespace ContratacionWebApi.Models {
    public class CuentaBancaria {
        public int Id { get; set; }

        [RegularExpression ("[0-9]*", ErrorMessage = "Solo se admiten números")]
        public string NumeroCuenta { get; set; }

        [RegularExpression ("[0-9]*", ErrorMessage = "Solo se admiten números")]
        public string NumeroSucursal { get; set; }
        public NombreSucursal NombreSucursal { get; set; }
        public Moneda Moneda { get; set; }
        public int EntidadId { get; set; }
        public Entidad Entidad { get; set; }
    }
}