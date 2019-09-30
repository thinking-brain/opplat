using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using System.Text.RegularExpressions;
using System.Globalization;

using Microsoft.EntityFrameworkCore;
using ContabilidadWebApi.VersatModels;
using ContabilidadWebApi.Models;
using ContabilidadWebApi.Data;

namespace ContabilidadWebApi.Helper
{

    /// <summary>
    /// Helper para importar el plan de costo en formato csv.
    /// </summary>
    public class PlanCsvHelper
    {

        private readonly DbContext _context;
        private readonly VersatDbContext _vcontext;

        public PlanCsvHelper(VersatDbContext _vcontext, ApiDbContext _context)
        {
            this._context = _context;
            this._vcontext = _vcontext;
        }
        public PlanCsvHelper(VersatDbContext _vcontext, DbContext _context)
        {
            this._context = _context;
            this._vcontext = _vcontext;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader">Instancia del nuget CsvReader que contine el puntero al fichero csv.</param>
        public void readCsv(CsvReader reader)
        {
            string costCenterid;
            string costcenterAreaId;
            string cuent;
            //Obtain the cost center and the costAreaid
            ReadingCostCenter(reader, out costCenterid, out costcenterAreaId, out cuent);

            //Skip the um
            reader.Read();
            List<string> headers = readValues(reader);

            //Skip empty line after the headers.
            reader.Read();

            //Start reading the elements from the table.
            readingElements(reader, costCenterid, cuent, costcenterAreaId);


        }

        /// <summary>
        /// Divide el registro obtendido.
        /// </summary>
        /// <param name="record">Linea obtenida por el puntero al fichero.</param>
        /// <returns>Arreglo de string</returns>
        private string[] SplitRecord(string record)
        {
            var line = record.Replace(',', '.');
            return line.Split(new char[] { ';' });
        }

        /// <summary>
        /// Obtiene el centrode costo y la id de centroCosto_Area
        /// </summary>
        /// <param name="reader">Puntero al fichero en csv.</param>
        /// <param name="centroCostid">Codigo del centro de costo</param>
        /// <param name="centroAreaid">Id del Area por centro de costo.</param>
        /// <param name="cuent"></param>
        private void ReadingCostCenter(CsvReader reader, out string centroCostid, out string centroAreaid, out string cuent)
        {
            reader.Read();
            var record = SplitRecord(reader[0]);
            string costCenter = record[1].Trim();
            string cuentaClave = record[3].Trim();
            centroCostid = record[1].Trim();
            var centros = _context.Set<CentroCostoArea>().ToList();

            var centroArea = _context.Set<CentroCostoArea>()
                                 .FirstOrDefault(c => c.CentroCostoId == costCenter);

            var cuenta = _vcontext.Set<ConCuenta>()
                              .SingleOrDefault(c => c.Clave.Trim() == cuentaClave.Trim());

            centroAreaid = centroArea.AreaId.ToString();
            cuent = cuenta.Clave.ToString();

        }

        /// <summary>
        /// lee solmante los valores mensuales
        /// </summary>
        /// <param name="reader">Puntero al fichero en csv.</param>
        /// <returns></returns>
        private List<string> readValues(CsvReader reader)
        {
            reader.Read();
            var record = SplitRecord(reader[0]);
            return readValues(record);

        }

        /// <summary>
        /// Lee solamente los valores mensuales.
        /// </summary>
        /// <param name="record">registro de un subElemento.</param>
        /// <returns></returns>
        private List<string> readValues(string[] record)
        {
            var result = record.Skip(3).ToList<string>();
            result.RemoveAt(result.Count - 1);
            return result;
        }

        /// <summary>
        /// Determina si un registro solo posee valores vacios o nulos.
        /// </summary>
        /// <param name="record">Registro</param>
        /// <returns></returns>
        private bool isEmpty(string[] record)
        {
            int count = 0;
            for (int i = 1; i < record.Length; i++)
            {
                if (record[i] == "" || record[i] == "0") count++;
            }

            return count == record.Length;
        }

        /// <summary>
        /// Metodo principal que itera sobre los subelementos y va almacenado en la bd.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="costCenterId"></param>
        /// <param name="cuent"></param>
        /// <param name="costcenterAreaId"></param>
        private void readingElements(CsvReader reader, string costCenterId, string cuent, string costcenterAreaId)
        {
            //costCenterId = costCenterId;
            while (reader.Read())
            {
                var line = SplitRecord(reader[0]);

                //Si la linea es vacia entoces continua a la proxima.
                if (isEmpty(line)) continue;

                string code;
                string coin;
                string top;


                //Si la linea no es un subelemento entoces continua a la proxima.
                if (!isAtomicElement(line[0], out code, out coin, out top)) continue;

                var months = readValues(line);
                //Si no existen valores mensuales continua.
                if (isEmtpyPlan(months)) continue;

                Plan plan = new Plan();
                plan.CentroCosto = costCenterId;
                plan.CentroCostoAreaId = int.Parse(costcenterAreaId);
                int subcode = int.Parse(code);

                //Llena el plan
                if (!ReadElement(subcode, costCenterId, cuent, coin, plan)) continue;
                ReadMonthValues(months, plan);

                _context.Add(plan);
                _context.SaveChanges();

                //Saves the elemnts at range with value 0 for the plan.
                SaveElementsAtRange(code, top, costCenterId, cuent, coin, costcenterAreaId);


            }
        }

        /// <summary>
        /// Determina si es un subElemento
        /// </summary>
        /// <param name="record">Regsitro</param>
        /// <param name="code">Codigo del subElemento.</param>
        /// <param name="coin">Codigo de la moneda</param>
        /// /// <param name="top">Tope maximo del rango si este existe (Ej: 220340-27).</param>
        /// <returns></returns>
        private bool isAtomicElement(string record, out string code, out string coin, out string top)
        {
            Regex re = new Regex(@"[0-9]{6}");
            Regex recoin = new Regex(@"[MN]{2}|[CUC]{3}");
            Regex reTop = new Regex(@"-[0-9]{2}");

            var result = re.Match(record);
            var money = recoin.Match(record);

            top = reTop.Match(record).Value;
            code = result.Value;


            coin = (money.Value == "CUC") ? "101" : "100";

            return (result.Value != "");

        }

        /// <summary>
        /// Determina si no existe una cuenta para el subElemento.
        /// </summary>
        /// <param name="values">Valores Mensuales</param>
        /// <returns></returns>
        private bool isEmtpyPlan(List<string> values)
        {

            foreach (var i in values)
            {
                if (i != "" && i != "0") return false;
            }
            return true;
        }

        /// <summary>
        /// Asigna a una instancia de plan los valores de subelemento,cuenta,centroCosto,moneda y descripci[on de la cuenta.
        /// </summary>
        /// <param name="code">Codigo del subElemento</param>
        /// <param name="centroCosto">Codigo del centro de costo</param>
        /// <param name="cuent"></param>
        /// <param name="coin">Codigo del tipo de moneda</param>
        /// <param name="plan">Instancia del plan</param>
        /// <returns></returns>
        private bool ReadElement(int code, string centroCosto, string cuent, string coin, Plan plan)
        {

            CosCentro centro = _vcontext.Set<CosCentro>()
                                          .FirstOrDefault(c => c.Clave == centroCosto);

            var COD = code.ToString().Substring(2, 1);

            var subcode = code / 10000;
            //var subcode = code.ToString().Substring(3);
            //if (subcode == 2) code += 10000;

            //int codeA = code;

            //if (coin == "101" && code > 800000)
            //{
            //    codeA -= 100;
            //}

            CosSubelementogasto subElemento = _vcontext.Set<CosSubelementogasto>()
                                                    .FirstOrDefault(s => s.Codigo == code.ToString());

            CosElementogasto elemento = _vcontext.Set<CosElementogasto>()
                                                  .SingleOrDefault(s => s.Idelementogasto == subElemento.Idelementogasto);

            //CosCuentasAsociadas cuentaCentro = _vcontext.Set<CosCuentasAsociadas>()
            //                     .FirstOrDefault(c => c.Clave == centroCosto);


            //Cuenta cuenta = _acontext.Set<Cuenta>()
            //                        .FirstOrDefault(c => c.CTA == centro.Grupo && c.Epigrafe == centroCosto && (c.SubAnalisis == code.ToString() || c.SubAnalisis == codeA.ToString()));


            ConCuenta cuenta = _vcontext.Set<ConCuenta>()
                                    .FirstOrDefault(c => c.Clave == cuent);



            if (cuenta == null) return false;

            plan.SubElemento = subElemento.Codigo;
            plan.Elemento = elemento.Codigo;
            plan.Cuenta = cuenta.Clave;
            plan.SubCuenta = coin;
            plan.Descripcion = subElemento.Descripcion;
            plan.Fecha = DateTime.Now;
            plan.Analisis = coin;
            plan.SubAnalisis = subElemento.Codigo;
            return true;

        }

        /// <summary>
        /// Mapea los valores mensuales de un registro al plan en cuestion.
        /// </summary>
        /// <param name="values">Valores mensuales</param>
        /// <param name="plan">Instancia del plan que se esta importando.</param>
        private void ReadMonthValues(List<string> values, Plan plan)
        {

            plan.Enero = 1000 * ((values[0] == "") ? 0 : Decimal.Parse(values[0], CultureInfo.InvariantCulture));
            plan.Febrero = 1000 * ((values[1] == "") ? 0 : Decimal.Parse(values[1], CultureInfo.InvariantCulture));
            plan.Marzo = 1000 * ((values[2] == "") ? 0 : Decimal.Parse(values[2], CultureInfo.InvariantCulture));
            plan.Abril = 1000 * ((values[3] == "") ? 0 : Decimal.Parse(values[3], CultureInfo.InvariantCulture));
            plan.Mayo = 1000 * ((values[4] == "") ? 0 : Decimal.Parse(values[4], CultureInfo.InvariantCulture));
            plan.Junio = 1000 * ((values[5] == "") ? 0 : Decimal.Parse(values[5], CultureInfo.InvariantCulture));
            plan.Julio = 1000 * ((values[6] == "") ? 0 : Decimal.Parse(values[6], CultureInfo.InvariantCulture));
            plan.Agosto = 1000 * ((values[7] == "") ? 0 : Decimal.Parse(values[7], CultureInfo.InvariantCulture));
            plan.Septiembre = 1000 * ((values[8] == "") ? 0 : Decimal.Parse(values[8], CultureInfo.InvariantCulture));
            plan.Octubre = 1000 * ((values[9] == "") ? 0 : Decimal.Parse(values[9], CultureInfo.InvariantCulture));
            plan.Noviembre = 1000 * ((values[10] == "") ? 0 : Decimal.Parse(values[10], CultureInfo.InvariantCulture));
            plan.Diciembre = 1000 * ((values[11] == "") ? 0 : Decimal.Parse(values[11], CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Obtiene los codigos de los subelementos contenidos en un rango.
        /// </summary>
        /// <param name="code">Codigo del subelemento</param>
        /// <param name="top">Tope maximo del rango de subelementos</param>
        /// <returns>Un ienumerable que contine los codigos de los subelementos contenidos en el rango.</returns>
        private IEnumerable<string> GetElementsAtRange(string code, string top)
        {

            List<string> result = new List<string>();


            if (top == "") return result;

            top = top.Substring(1);

            string subCode = code.Substring(0, 4);

            int first = int.Parse(code) % 100;
            int last = int.Parse(top);

            for (int i = first + 1; i <= last; i++)
            {
                result.Add(subCode + i);
            }

            return result;


        }


        private void SaveElementsAtRange(string code, string top, string costCenterId, string cuent, string coin, string costcenterAreaId)
        {
            foreach (var c in GetElementsAtRange(code, top))
            {
                Plan emptyPlan = new Plan();
                emptyPlan.CentroCosto = costCenterId;
                emptyPlan.CentroCostoAreaId = int.Parse(costcenterAreaId);
                int subcode = int.Parse(c);

                //Llena el plan
                if (!ReadElement(subcode, costCenterId, cuent, coin, emptyPlan)) continue;

                _context.Add(emptyPlan);
                _context.SaveChanges();
            }
        }
    }
}
