using System;
using System.Collections.Generic;
using System.Linq;
using ContratacionWebApi.Data;
using ContratacionWebApi.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using opplatApplication.Models;

namespace opplatApplication.Utils {
    public class ContratacionContador {
        private readonly ContratacionDbContext _context;

        public ContratacionContador () { }
        public ContratacionContador (ContratacionDbContext context) {
            _context = context;
        }
        public string ContratosCant (string tipoTramite) {
            var cantidad = "";

            if (tipoTramite == "Ofertas") {
                cantidad = "2";
            }
            if (tipoTramite == "Contratos") {
                cantidad = "0";
            }
            return cantidad;
        }
    }
}