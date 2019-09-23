using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanzasWebApi.Data
{
    public class DatosPlanGI
    {
        public static List<dynamic> Datos()
        {
            var data = new List<dynamic> {
                new { Dato = "Ventas por servicios en moneda nacional", Valor = 912, Tipo = "Ingresos" },
                new { Dato = "Ventas por servicios en CUC", Valor = 913, Tipo = "Ingresos"},
                new { Dato = "Ingresos financieros", Valor = 920, Tipo = "Ingresos" },
                new { Dato = "Impuestos por las ventas", Valor = 805, Tipo = "Egresos" },
                new { Dato = "Costo de ventas", Valor = 810, Tipo = "Egresos" },
                new { Dato = "Gastos generales de Administracion", Valor = 822, Tipo = "Egresos" },
                new { Dato = "Gastos financieros en MN", Valor = 835, Tipo = "Egresos" },
                new { Dato = "Gastos financieros en MLC", Valor = 836, Tipo = "Egresos" },
                new { Dato = "Gastos por perdida", Valor = 845, Tipo = "Egresos" },
                new { Dato = "Otros impuestos tasas y contribuciones", Valor = 856, Tipo = "Egresos" },
                new { Dato = "Utilidad", Valor = 1000, Tipo = "Utilidad" },
                new { Dato = "Pago a cargo de la utilidad", Valor = 1001, Tipo = "Utilidad" },
                new { Dato = "Utilidad despues de pagos de anticipos", Valor = 1002, Tipo = "Utilidad" },
                new { Dato = "Reserva de contingencia del 2% al 10%", Valor = 1003, Tipo = "Utilidad" },
                new { Dato = "Utilidad libre despues de la reserva", Valor = 1004, Tipo = "Utilidad" },
                new { Dato = "Reserva de contingencia 30%", Valor = 1005, Tipo = "Utilidad" },
                new { Dato = "Utilidad despues de la reserva del 30%", Valor = 1006, Tipo = "Utilidad" },

            };
           
            return data;
        }
    }
}
