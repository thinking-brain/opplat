using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinanzasWebApi.ViewModels
{
    public class DetalleRazonYearVM
    {
        public string Razon { get; set; }

        public int Tipo { get;set; }

        [DataType(DataType.Currency)]
        public decimal Enero { get; set; }

        [DataType(DataType.Currency)]
        public decimal Febrero { get; set; }

        [DataType(DataType.Currency)]
        public decimal Marzo { get; set; }

        [DataType(DataType.Currency)]
        public decimal Abril { get; set; }

        [DataType(DataType.Currency)]
        public decimal Mayo { get; set; }

        [DataType(DataType.Currency)]
        public decimal Junio { get; set; }

        [DataType(DataType.Currency)]
        public decimal Julio { get; set; }

        [DataType(DataType.Currency)]
        public decimal Agosto { get; set; }

        [DataType(DataType.Currency)]
        public decimal Septiembre { get; set; }

        [DataType(DataType.Currency)]
        public decimal Octubre { get; set; }

        [DataType(DataType.Currency)]
        public decimal Noviembre { get; set; }

        [DataType(DataType.Currency)]
        public decimal Diciembre { get; set; }

        public decimal this[int index]
        {
            get
            {
                switch (index)
                {
                    case 1: return Enero;
                    case 2: return Febrero;
                    case 3: return Marzo;
                    case 4: return Abril;
                    case 5: return Mayo;
                    case 6: return Junio;
                    case 7: return Julio;
                    case 8: return Agosto;
                    case 9: return Septiembre;
                    case 10: return Octubre;
                    case 11: return Noviembre;
                    case 12: return Diciembre;
                    default: return 0;
                }
            }
        }

        public decimal Acumulado(int mes)
        {

            var acum = 0M;

            for (int i = 1; i <= mes; i++)
            {
                acum += this[i];
            }
            return acum;
        }
    }
}

