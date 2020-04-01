using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using FinanzasWebApi.Models;
using FinanzasWebApi.Data;
using OfficeOpenXml;
using System.IO;
using System.Text;
using FinanzasWebApi.ViewModels;

namespace FinanzasWebApi.Helpers
{


    public class ReadExcelHelper
    {

        private readonly DbContext _context;

        public ReadExcelHelper(FinanzasDbContext _context)
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
            var reportName = (worksheet.Cells[1, 1].Value) != null ? (worksheet.Cells[1, 1].Value).ToString().Trim() : "New Report";
            var report = new ReporteEstadoFinanciero { Descripcion = reportName };
            _context.Add(report);
            _context.SaveChanges();

            var repSelect = _context.Set<ReporteEstadoFinanciero>().Include(s=>s.ElementosDelReporteEF).ThenInclude(s=>s.SubElementosEfReports).SingleOrDefault(s=>s.Descripcion.Equals(reportName));

            for (int row = 4; row <= rowCount.Value; row++)
            {
                var element = (worksheet.Cells[row, 1].Value) != null ? (worksheet.Cells[row, 1].Value).ToString().Trim() : "";
                var subElements = (worksheet.Cells[row, 2].Value) != null ? (worksheet.Cells[row, 2].Value).ToString().Trim() : "";
                var tipo = (worksheet.Cells[row, 3].Value) != null ? (worksheet.Cells[row, 3].Value).ToString().Trim() : "";
                var sumar = (worksheet.Cells[row, 4].Value) != null ? (worksheet.Cells[row, 4].Value).ToString().Trim() : "";
                var restar = (worksheet.Cells[row, 5].Value) != null ? (worksheet.Cells[row, 5].Value).ToString().Trim() : "";
                var dividir = (worksheet.Cells[row, 6].Value) != null ? (worksheet.Cells[row, 6].Value).ToString().Trim() : "";
                var celda = (worksheet.Cells[row, 7].Value) != null ? (worksheet.Cells[row, 7].Value).ToString().Trim() : "";
                
                if (dividir!= "")
                {
                    
                }
                
                var elemento = new ElementosDelReporteEF
                {
                    Descripcion = element,
                    Celda = celda,
                    Restar = restar,
                    Sumar = sumar,
                    Dividir = dividir,
                    Tipo = tipo,
                    ReporteEstadoFinancieroDescripcion = report.Descripcion
                };

                if (subElements != "")
                {
                    var subE = subElements.Split(',');
                    foreach (var item in subE)
                    {
                        elemento.SubElementosEfReports.Add(new SubElementosEfReport { Descripcion = item, ElementosDelReporteEFDescripcion = element });
                    }

                }
                repSelect.ElementosDelReporteEF.Add(elemento);
                _context.SaveChanges();
            }


            
        }

    }

}
