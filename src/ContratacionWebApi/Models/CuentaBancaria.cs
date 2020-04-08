using System.ComponentModel.DataAnnotations;

namespace ContratacionWebApi.Models {
    public class CuentaBancaria {
        public int Id { get; set; }

        [Required]
        [RegularExpression ("[0-9]*", ErrorMessage = "Solo se admiten números")]
        public int NumeroCuenta { get; set; }

        [Required]
        [RegularExpression ("[0-9]*", ErrorMessage = "Solo se admiten números")]
        public int NumeroSucursal { get; set; }
        public NombreSucursal NombreSucursal { get; set; }
        public Moneda Moneda { get; set; }
        public int EntidadId { get; set; }
        public Entidad Entidad { get; set; }
    }
}