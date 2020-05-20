using System;
using System.Collections.Generic;

namespace ImportadorDatos.Models.Versat {
    public partial class GenEntidad {
        public int Id { get; set; }
        public int Codigo { get; set; }
        public string Codigoreu { get; set; }
        public string Nombre { get; set; }
        public string Abreviatura { get; set; }
        public string Direccion { get; set; }
        public bool? Activo { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string NIT { get; set; }
        public string IRCC { get; set; }
        public string Provincia { get; set; }
        public string Pais { get; set; }
    }
}