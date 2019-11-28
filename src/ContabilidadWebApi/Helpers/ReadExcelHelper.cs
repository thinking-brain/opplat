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
                        Enero = Convert.ToDecimal(item.Columna3),
                        Febrero = Convert.ToDecimal(item.Columna4),
                        Marzo = Convert.ToDecimal(item.Columna5),
                        Abril = Convert.ToDecimal(item.Columna6),
                        Mayo = Convert.ToDecimal(item.Columna7),
                        Junio = Convert.ToDecimal(item.Columna8),
                        Julio = Convert.ToDecimal(item.Columna9),
                        Agosto = Convert.ToDecimal(item.Columna10),
                        Septiembre = Convert.ToDecimal(item.Columna11),
                        Octubre = Convert.ToDecimal(item.Columna12),
                        Noviembre = Convert.ToDecimal(item.Columna13),
                        Diciembre = Convert.ToDecimal(item.Columna14),

                    });

                }
                else
                {
                    var detalle = _context.Set<DetallePlanGI>().SingleOrDefault(s => s.Concepto == elemento && s.Plan == plan);
                    detalle.Enero = Convert.ToDecimal(item.Columna3);
                    detalle.Febrero = Convert.ToDecimal(item.Columna4);
                    detalle.Marzo = Convert.ToDecimal(item.Columna5);
                    detalle.Abril = Convert.ToDecimal(item.Columna6);
                    detalle.Mayo = Convert.ToDecimal(item.Columna7);
                    detalle.Junio = Convert.ToDecimal(item.Columna8);
                    detalle.Julio = Convert.ToDecimal(item.Columna9);
                    detalle.Agosto = Convert.ToDecimal(item.Columna10);
                    detalle.Septiembre = Convert.ToDecimal(item.Columna11);
                    detalle.Octubre = Convert.ToDecimal(item.Columna12);
                    detalle.Noviembre = Convert.ToDecimal(item.Columna13);
                    detalle.Diciembre = Convert.ToDecimal(item.Columna14);
                    _context.Update(detalle);
                }



            }

            _context.SaveChanges();
        }

    }

}
