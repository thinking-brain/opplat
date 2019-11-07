namespace ImportadorDatos.Models.Versat
{
    public class AsientosView
    {
        public string Clave { get; set; }

        public int Idasiento { get; set; }

        public int Iddocumento { get; set; }

        public int Idcuenta { get; set; }

        public int Idcriterio { get; set; }

        public int Idpase { get; set; }

        public decimal n_importe { get; set; }

        public int Idmoneda { get; set; }

        public int? Idtasa { get; set; }

        public decimal? n_importemo { get; set; }

        public decimal? n_importe_anx { get; set; }

        public bool AsientoGasto { get; set; }

        public decimal Signo { get; set; }

    }
}