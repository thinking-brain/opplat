using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinanzasWebApi.ViewModels
{

    public class PlanPronosticoProductivoVM
    {
        public int Id { get; set; }
        public string Año { get; set; }

        public int ConceptoPlanVMId { get; set; }
        public virtual ConceptoPlanVM ConceptoPlan { get; set; }

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

        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }


        public decimal this[int index]
        {
            get
            {

                Dictionary<int, decimal> months = new Dictionary<int, decimal>(12);
                months.Add(1, Enero);
                months.Add(2, Febrero);
                months.Add(3, Marzo);
                months.Add(4, Abril);
                months.Add(5, Mayo);
                months.Add(6, Junio);
                months.Add(7, Julio);
                months.Add(8, Agosto);
                months.Add(9, Septiembre);
                months.Add(10, Octubre);
                months.Add(11, Noviembre);
                months.Add(12, Diciembre);
                return months[index];
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
