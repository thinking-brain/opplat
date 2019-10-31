using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanzasWebApi.Dtos
{
    public class CuentaDto
    {
        public int Id { get; set; }

        public string Numero { get; set; }

        public string Descripcion { get; set; }
    }
}