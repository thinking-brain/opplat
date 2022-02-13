using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using ContabilidadWebApi.Models;
using ContabilidadWebApi.Data;
using ContabilidadWebApi.Services;
using OfficeOpenXml;
using System.IO;
using System.Text;
using ContabilidadWebApi.ViewModels;

namespace ContabilidadWebApi.Helpers
{

    /// <summary>
    /// Helper para importar el plan de costo en formato csv.
    /// </summary>
    public class ReadExcelHelper
    {

        private readonly DbContext _context;

        public ReadExcelHelper(ContabilidadDbContext _context)
        {
            this._context = _context;

        }
        public ReadExcelHelper(DbContext _context)
        {
            this._context = _context;

        }

        public void readExcelPackageToString(ExcelPackage package, ExcelWorksheet worksheet, string year)
        {
            var rowCount = worksheet.Dimension?.Rows;
            var colCount = worksheet.Dimension?.Columns;


            var sb = new StringBuilder();

            var lts = new List<DatosViewModel>();

            for (int row = 3; row <= rowCount.Value; row++)
            {
                var element = (worksheet.Cells[row, 1].Value) != null ? (worksheet.Cells[row, 1].Value).ToString().Trim() : "";

                if (!_context.Set<ConceptoPlan>().Any(s => s.Concepto.ToUpper().TrimEnd().Equals(element.ToUpper().TrimEnd())))
                {
                    var concept = new ConceptoPlan { Concepto = element };
                    // var cuenta = DatosPlanGI.Datos().SingleOrDefault(s=>s.Dato.Equals(element));
                    // var cuenta = DatosPlanGI.Datos().SingleOrDefault(s=>s.Dato.Equals(element));
                    // concept.Cuentas.Add(new ConceptoCuentas{ })

                    _context.Set<ConceptoPlan>().Add(new ConceptoPlan { Concepto = element });
                    _context.SaveChanges();
                }
                try
                {
                    lts.Add(new DatosViewModel
                    {
                        Columna0 = (worksheet.Cells[row, 1].Value) != null ? (worksheet.Cells[row, 1].Value).ToString() : "",
                        Columna1 = (worksheet.Cells[row, 2].Value) != null ? (worksheet.Cells[row, 2].Value).ToString() : "",
                        Columna2 = (worksheet.Cells[row, 3].Value) != null ? (worksheet.Cells[row, 3].Value).ToString() : "",
                        Columna3 = (worksheet.Cells[row, 4].Value) != null ? (worksheet.Cells[row, 4].Value).ToString() : "",
                        Columna4 = (worksheet.Cells[row, 5].Value) != null ? (worksheet.Cells[row, 5].Value).ToString() : "",
                        Columna5 = (worksheet.Cells[row, 6].Value) != null ? (worksheet.Cells[row, 6].Value).ToString() : "",
                        Columna6 = (worksheet.Cells[row, 7].Value) != null ? (worksheet.Cells[row, 7].Value).ToString() : "",
                        Columna7 = (worksheet.Cells[row, 8].Value) != null ? (worksheet.Cells[row, 8].Value).ToString() : "",
                        Columna8 = (worksheet.Cells[row, 9].Value) != null ? (worksheet.Cells[row, 9].Value).ToString() : "",
                        Columna9 = (worksheet.Cells[row, 10].Value) != null ? (worksheet.Cells[row, 10].Value).ToString() : "",
                        Columna10 = (worksheet.Cells[row, 11].Value) != null ? (worksheet.Cells[row, 11].Value).ToString() : "",
                        Columna11 = (worksheet.Cells[row, 12].Value) != null ? (worksheet.Cells[row, 12].Value).ToString() : "",
                        Columna12 = (worksheet.Cells[row, 13].Value) != null ? (worksheet.Cells[row, 13].Value).ToString() : "",
                        Columna13 = (worksheet.Cells[row, 14].Value) != null ? (worksheet.Cells[row, 14].Value).ToString() : "",
                        Columna14 = (worksheet.Cells[row, 15].Value) != null ? (worksheet.Cells[row, 15].Value).ToString() : "",

                    });
                }
                catch (System.Exception ex)
                {

                    throw;
                }



            }
            var plan = _context.Set<PlanGI>().Any(s => s.Year == year) ? _context.Set<PlanGI>().SingleOrDefault(s => s.Year == year) : new PlanGI();

            if (!_context.Set<PlanGI>().Any(s => s.Year == year))
            {
                plan.Titulo = "Plan de Igresos y Gastos";
                plan.Fecha = DateTime.Now;
                plan.Year = year;
                _context.Set<PlanGI>().Add(plan);
                _context.SaveChanges();
            }


            foreach (var item in lts)
            {
                var elemento = _context.Set<ConceptoPlan>().SingleOrDefault(s => s.Concepto.Trim().ToUpper().Equals(item.Columna0.Trim().ToUpper()));
                if (!_context.Set<DetallePlanGI>().Any(s => s.Plan == plan && s.Concepto == elemento))
                {
                    _context.Set<DetallePlanGI>().Add(new DetallePlanGI
                    {
                        Concepto = elemento,
                        Plan = plan,
                        Enero = !item.Columna1.Equals("") ? Convert.ToDecimal(item.Columna1) : 0,
                        Febrero = !item.Columna2.Equals("") ? Convert.ToDecimal(item.Columna2) : 0,
                        Marzo = !item.Columna3.Equals("") ? Convert.ToDecimal(item.Columna3) : 0,
                        Abril = !item.Columna4.Equals("") ? Convert.ToDecimal(item.Columna4) : 0,
                        Mayo = !item.Columna5.Equals("") ? Convert.ToDecimal(item.Columna5) : 0,
                        Junio = !item.Columna6.Equals("") ? Convert.ToDecimal(item.Columna6) : 0,
                        Julio = !item.Columna7.Equals("") ? Convert.ToDecimal(item.Columna7) : 0,
                        Agosto = !item.Columna8.Equals("") ? Convert.ToDecimal(item.Columna8) : 0,
                        Septiembre = !item.Columna9.Equals("") ? Convert.ToDecimal(item.Columna9) : 0,
                        Octubre = !item.Columna10.Equals("") ? Convert.ToDecimal(item.Columna10) : 0,
                        Noviembre = !item.Columna11.Equals("") ? Convert.ToDecimal(item.Columna11) : 0,
                        Diciembre = !item.Columna12.Equals("") ? Convert.ToDecimal(item.Columna12) : 0,

                    });

                }
                else
                {
                    var detalle = _context.Set<DetallePlanGI>().SingleOrDefault(s => s.Concepto == elemento && s.Plan == plan);
                    detalle.Enero = !item.Columna1.Equals("") ? Convert.ToDecimal(item.Columna1) : 0;
                    detalle.Febrero = !item.Columna2.Equals("") ? Convert.ToDecimal(item.Columna2) : 0;
                    detalle.Marzo = !item.Columna3.Equals("") ? Convert.ToDecimal(item.Columna3) : 0;
                    detalle.Abril = !item.Columna4.Equals("") ? Convert.ToDecimal(item.Columna4) : 0;
                    detalle.Mayo = !item.Columna5.Equals("") ? Convert.ToDecimal(item.Columna5) : 0;
                    detalle.Junio = !item.Columna6.Equals("") ? Convert.ToDecimal(item.Columna6) : 0;
                    detalle.Julio = !item.Columna7.Equals("") ? Convert.ToDecimal(item.Columna7) : 0;
                    detalle.Agosto = !item.Columna8.Equals("") ? Convert.ToDecimal(item.Columna8) : 0;
                    detalle.Septiembre = !item.Columna9.Equals("") ? Convert.ToDecimal(item.Columna9) : 0;
                    detalle.Octubre = !item.Columna10.Equals("") ? Convert.ToDecimal(item.Columna10) : 0;
                    detalle.Noviembre = !item.Columna11.Equals("") ? Convert.ToDecimal(item.Columna11) : 0;
                    detalle.Diciembre = !item.Columna12.Equals("") ? Convert.ToDecimal(item.Columna12) : 0;
                    _context.Update(detalle);
                }



            }

            _context.SaveChanges();
        }

    }

}
