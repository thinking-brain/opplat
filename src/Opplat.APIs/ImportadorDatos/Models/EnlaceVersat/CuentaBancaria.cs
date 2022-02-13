namespace ImportadorDatos.Models.EnlaceVersat {
    public class CuentaBancaria {
        public int Id { get; set; }
        public string NumeroCta { get; set; }
        public int CtaBancoEntidadVersatId { get; set; }
        public int CuentaBancariaId { get; set; }
    }
}