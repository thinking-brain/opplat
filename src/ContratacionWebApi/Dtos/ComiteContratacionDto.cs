using System;
using System.Collections.Generic;

namespace ContratacionWebApi.Dtos {
    public class ComiteContratacionDto {
        public List<int> Trabajadores { get; set; }
        public bool Activo { get; set; }
    }
}