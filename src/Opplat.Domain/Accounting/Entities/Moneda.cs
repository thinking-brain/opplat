using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Opplat.Contabilidad.Domain.Entities;
    public class Moneda
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Sigla { get; set; }
    }

