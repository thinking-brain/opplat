using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanzasWebApi.Data;
using FinanzasWebApi.Models;
using FinanzasWebApi.ViewModels;

namespace FinanzasWebApi.Helper.EstadoFinanciero
{
    public class GetEstadoFinanciero
    {

        FinanzasDbContext _context;
        public GetEstadoFinanciero(FinanzasDbContext context)
        {
            _context = context;
        }

        public static string EstadoFinancieroGetDato(string dato)
        {
            var data = GetEstadoFinancieroDatos.EstadoFinancieroDatos().SingleOrDefault(s => s.Dato.Trim().ToUpper().Equals(dato.Trim().ToUpper()));
            return (data.Valor.ToString());
        }
        public decimal EstadoFinancieroGetValor(string dato, int year, int meses)
        {
            string variable = EstadoFinancieroGetDato(dato);
            string[] variableCuentas = variable.Split(',');
            decimal variableValue = 0;
            for (int i = 0; i < variableCuentas.Length; i++)
            {
                string cta = variableCuentas[i].ToString();
                var cache = _context.Set<CacheCuentaPeriodo>().SingleOrDefault(c => c.Cuenta == cta && c.Mes == meses && c.Year == year);
                decimal Importe = 0;
                if (cache != null)
                {
                    Importe = cache.Saldo;
                }
                variableValue = variableValue + Importe;
            }
            return variableValue;
        }
        public List<EstadoFinancieroVM> EstadoFinancieroDetalle(int year, int meses)
        {
            var resultado = new List<EstadoFinancieroVM>();
            var group = GetEstadoFinancieroDatos.EstadoFinancieroDatos().GroupBy(s => s.Grupo).ToList();
            foreach (var item in group)
            {
                var grupo = new EstadoFinancieroVM { Concepto = item.Key.ToString(), EFE = item.First().EF.ToString(), Detalles = new List<DetalleEstadoFinancieroVM>() };
                foreach (var det in item)
                {
                    decimal valor = EstadoFinancieroGetValor(det.Dato.ToString(), year, meses);
                    grupo.Detalles.Add(new DetalleEstadoFinancieroVM { Razon = det.Dato.ToString(), Valor = valor });
                }
                resultado.Add(grupo);
            }
            return resultado;
        }



    }
}
