using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsvHelper;
using FinanzasWebApi.Data;
using FinanzasWebApi.Helper;
using FinanzasWebApi.Models;
using FinanzasWebApi.ViewModels;
using FinanzasWebApi.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace FinanzasWebApi.Controllers {
    [Route ("finanzas/[controller]")]
    [ApiController]
    public class EstadoFinancieroController : ControllerBase {
        private const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private readonly IHostingEnvironment _hostingEnvironment;
        // GetEstadoFinanciero _obtenerEstadoFinanciero { get; set; }
        // GetEF _obtenerEF { get; set; }
        FinanzasDbContext _context { get; set; }
        IConfiguration _config { get; set; }

        public EstadoFinancieroController (FinanzasDbContext context, IHostingEnvironment hostingEnvironment, IConfiguration config) {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _config = config;
        }

        [HttpGet ("estadoFinancieroReport/{años}/{meses}/{tipo}")]
        public List<EstadoFinancieroJsVM> EstadoFinancierosReport (int años, int meses, string tipo) {
            // string efe = "5921";
            var result = new List<EstadoFinancieroJsVM> ();
            var resultFaltantes = new List<string> ();
            var reporte = _context.Set<ReporteEstadoFinanciero> ().Include (s => s.ElementosDelReporteEF).ThenInclude (s => s.SubElementosEfReports).SingleOrDefault (s => s.Descripcion.Equals (tipo));
            var planes = GetPlanesPeriodo.Get (años, _config);
            foreach (var item in reporte.ElementosDelReporteEF) {

                var real = 0M;
                var planEnMes = planes.SingleOrDefault (s => s.Concepto.ToUpper ().Trim ().Equals (item.Descripcion.ToUpper ().Trim ())) != null ? planes.SingleOrDefault (s => s.Concepto.ToUpper ().Trim ().Equals (item.Descripcion.ToUpper ().Trim ())) [meses] : 0M;
                //ENCABEZADOS Y CUENTAS
                if ((item.Tipo.Equals ("Encabezado") || item.Tipo.Equals ("Cuenta")) && item.SubElementosEfReports.Count > 0) {
                    foreach (var cuenta in item.SubElementosEfReports) {
                        string cta = Convert.ToString (cuenta.Descripcion);
                        var cache = _context.Set<CacheCuentaPeriodo> ().SingleOrDefault (c => c.Cuenta == cta && c.Mes == meses && c.Year == años);
                        if (cache != null) {
                            real = real + cache.Saldo;
                        }
                    }
                    var data = new EstadoFinancieroJsVM {
                        Concepto = item.Descripcion,
                        Celda = item.Celda,
                        EFE = tipo,
                        Encabezado = item.Tipo.Equals ("Encabezado") ? true : false,
                        Visible = true,
                        Plan = planEnMes,
                        Mes = meses,
                        Real = real,
                    };
                    result.Add (data);
                }
                //SUBELEMENTOS
                if (item.Tipo.Equals ("SubElemento") && item.SubElementosEfReports.Count > 0) {
                    foreach (var subElemento in item.SubElementosEfReports) {
                        string subElem = Convert.ToString (subElemento.Descripcion);
                        var cache = _context.Set<CacheSubElementoPeriodo> ().SingleOrDefault (c => c.subElemento == subElem && c.Mes == meses && c.Year == años);
                        if (cache != null) {
                            real = real + cache.Saldo;
                        }
                    }
                    var data = new EstadoFinancieroJsVM {
                        Concepto = item.Descripcion,
                        Celda = item.Celda,
                        EFE = tipo,
                        Encabezado = item.Tipo.Equals ("Encabezado") ? true : false,
                        Visible = true,
                        Plan = planEnMes,
                        Mes = meses,
                        Real = real,
                    };
                    result.Add (data);
                }
                //ELEMENTOS
                if (item.Tipo.Equals ("Elemento") && item.SubElementosEfReports.Count > 0) {
                    foreach (var element in item.SubElementosEfReports) {
                        string elemento = Convert.ToString (element.Descripcion);
                        var cache = _context.Set<CacheSubElementoPeriodo> ().Where (c => c.elemento == elemento && c.Mes == meses && c.Year == años);
                        if (cache != null) {
                            real = real + cache.Sum (s => s.Saldo);
                        }
                    }
                    //SI FORMULA?
                    if (item.Sumar != "" || item.Restar != "") {
                        var suma = item.Sumar.Split ('+');
                        var restar = item.Restar.Split ('-');
                        if (item.Sumar != "") {
                            foreach (var celda in suma) {
                                var valueNewLine = Regex.Replace (celda, @"[^\d]", "");
                                var valueLine = Regex.Replace (item.Celda, @"[^\d]", "");
                                var moneyS = result.SingleOrDefault (s => s.Celda.Equals (celda));
                                real = real + moneyS.Real;
                            }

                        }
                        if (item.Restar != "") {
                            foreach (var celda in restar) {
                                var valueNewLine = Regex.Replace (celda, @"[^\d]", "");
                                var valueLine = Regex.Replace (item.Celda, @"[^\d]", "");
                                var moneyR = result.SingleOrDefault (s => s.Celda.Equals (celda));
                                real = real - moneyR.Real;
                            }
                        }
                    }
                    if (item.Dividir != "") {
                        var dividir = item.Dividir.Split ('/');
                        if (item.Dividir != "") {
                            foreach (var celda in dividir) {
                                var moneyD = result.SingleOrDefault (s => s.Celda.Equals (celda));
                                real = real > 0 ? real + moneyD.Real : real / moneyD.Real;
                            }

                        }
                    }
                    var data = new EstadoFinancieroJsVM {
                        Concepto = item.Descripcion,
                        Celda = item.Celda,
                        EFE = tipo,
                        Encabezado = item.Tipo.Equals ("Encabezado") ? true : false,
                        Visible = true,
                        Plan = planEnMes,
                        Mes = meses,
                        Real = real,
                    };
                    result.Add (data);
                }
                //CON FORMULAS
                if (item.Tipo.Equals ("Encabezado") && item.SubElementosEfReports.Count == 0 && (item.Sumar != "" || item.Restar != "")) {
                    var suma = item.Sumar.Split ('+');
                    var restar = item.Restar.Split ('-');
                    if (item.Sumar != "") {

                        foreach (var celda in suma) {
                            var valueNewLine = Regex.Replace (celda, @"[^\d]", "");
                            var valueLine = Regex.Replace (item.Celda, @"[^\d]", "");
                            if (Convert.ToInt32 (valueNewLine) > Convert.ToInt32 (valueLine)) {
                                if (!resultFaltantes.Contains (item.Descripcion)) {
                                    resultFaltantes.Add (item.Descripcion);
                                }
                            } else {
                                var moneyS = result.SingleOrDefault (s => s.Celda.Equals (celda));
                                if (moneyS == null && !resultFaltantes.Contains (item.Descripcion)) {
                                    resultFaltantes.Add (item.Descripcion);
                                }
                                if (moneyS != null) {
                                    real = real + moneyS.Real;
                                }

                            }

                        }
                    }
                    if (item.Restar != "") {
                        foreach (var celda in restar) {
                            var valueNewLine = Regex.Replace (celda, @"[^\d]", "");
                            var valueLine = Regex.Replace (item.Celda, @"[^\d]", "");
                            if (Convert.ToInt32 (valueNewLine) > Convert.ToInt32 (valueLine)) {
                                if (!resultFaltantes.Contains (item.Descripcion)) {
                                    resultFaltantes.Add (item.Descripcion);
                                }
                            } else {
                                var moneyR = result.SingleOrDefault (s => s.Celda.Equals (celda));
                                if (moneyR == null && !resultFaltantes.Contains (item.Descripcion)) {
                                    resultFaltantes.Add (item.Descripcion);
                                }
                                if (moneyR != null) {
                                    real = real - moneyR.Real;
                                }
                            }
                        }
                    }
                    if (item.Dividir != "") {
                        var dividir = item.Dividir.Split ('/');
                        if (item.Dividir != "") {
                            foreach (var celda in dividir) {
                                var moneyD = result.SingleOrDefault (s => s.Celda.Equals (celda));
                                real = real > 0 ? real + moneyD.Real : real / moneyD.Real;
                            }

                        }
                    }
                    var data = new EstadoFinancieroJsVM {
                        Concepto = item.Descripcion,
                        Celda = item.Celda,
                        EFE = tipo,
                        Encabezado = item.Tipo.Equals ("Encabezado") ? true : false,
                        Visible = true,
                        Plan = planEnMes,
                        Mes = meses,
                        Real = real,
                    };
                    result.Add (data);
                }

                //SIN FORMULAS
                if (item.Tipo.Equals ("Encabezado") && item.SubElementosEfReports.Count == 0 && item.Sumar == "" && item.Restar == "") {
                    var data = new EstadoFinancieroJsVM {
                    Concepto = item.Descripcion,
                    Celda = item.Celda,
                    EFE = tipo,
                    Encabezado = item.Tipo.Equals ("Encabezado") ? true : false,
                    Visible = true,
                    Plan = planEnMes,
                    Mes = meses,
                    Real = real,
                    };
                    result.Add (data);

                }

            }
            //ELEMENTOS ENCABEZADOS QUE SU FORMULA ES MAYOR A LOS ELEMENTOS QUE TIENE LA LISTA
            foreach (var faltante in resultFaltantes) {
                var final = result.FirstOrDefault (s => s.Concepto.Equals (faltante));
                var original = reporte.ElementosDelReporteEF.FirstOrDefault (s => s.Descripcion.Equals (faltante));
                if (original.Tipo.Equals ("Encabezado") && original.SubElementosEfReports.Count == 0) {
                    var suma = original.Sumar.Split ('+');
                    var restar = original.Restar.Split ('-');

                    if (original.Sumar != "") {
                        foreach (var celda in suma) {
                            var moneyS = result.SingleOrDefault (s => s.Celda.Equals (celda));
                            if (moneyS != null) {
                                final.Real = final.Real + moneyS.Real;
                            }

                        }
                    }
                    if (original.Restar != "") {
                        foreach (var celda in restar) {
                            var moneyR = result.SingleOrDefault (s => s.Celda.Equals (celda));
                            if (moneyR != null) {
                                final.Real = final.Real - moneyR.Real;
                            }
                        }
                    }
                    if (original.Dividir != "") {
                        var dividir = original.Dividir.Split ('/');
                        if (original.Dividir != "") {
                            foreach (var celda in dividir) {
                                var moneyD = result.SingleOrDefault (s => s.Celda.Equals (celda));
                                final.Real = final.Real > 0 ? final.Real + moneyD.Real : final.Real / moneyD.Real;
                            }

                        }
                    }
                }
            }
            return result;
        }

        [HttpPost ("exportexcel")]
        public async Task<FileResult> Export (ColeccionViewModel viewModel) {
            List<EstadoFinancieroJsVM> data = viewModel.Data;
            var fileDownloadName = "Report.xlsx";
            var reportsFolder = "Reportes_EstadoFinanciero";
            var path = new FileInfo (Path.Combine (_hostingEnvironment.WebRootPath, reportsFolder, fileDownloadName));
            using (var package = createExcelPackage (data)) {
                package.SaveAs (path);
            }

            var bytes = System.IO.File.ReadAllBytes (path.ToString ());

            const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Response.ContentType = contentType;
            HttpContext.Response.Headers.Add ("Access-Control-Expose-Headers", "Content-Disposition");

            var fileContentResult = new FileContentResult (bytes, contentType) {
                FileDownloadName = fileDownloadName
            };

            return fileContentResult;
        }

        [HttpGet ("export")]
        private ExcelPackage createExcelPackage (List<EstadoFinancieroJsVM> data) {
            // var meses = inicioSemana.Date.Month;
            // var años = inicioSemana.Date.Year;

            var package = new ExcelPackage ();

            package.Workbook.Properties.Title = "Estado Financiero Report-" + data.First ().EFE;
            package.Workbook.Properties.Author = "Opplat";
            package.Workbook.Properties.Subject = "La Concordia";
            package.Workbook.Properties.Keywords = data.First ().EFE;

            var worksheet = package.Workbook.Worksheets.Add (data.First ().EFE);

            worksheet.Cells[1, 1].Value = "Elementos";
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 2].Value = "Plan en Mes";
            worksheet.Cells[1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[1, 2].Style.Font.Bold = true;
            worksheet.Cells[1, 3].Value = "Real en Mes";
            worksheet.Cells[1, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[1, 3].Style.Font.Bold = true;

            var numberformat = "#,##0.00";
            var dataCellStyleName = "TableNumber";
            var numStyle = package.Workbook.Styles.CreateNamedStyle (dataCellStyleName);
            numStyle.Style.Numberformat.Format = numberformat;

            int count = 2;
            foreach (var item in data) {
                worksheet.Cells[count, 1].Value = item.Concepto;
                worksheet.Cells[count, 2].Value = item.Plan;
                worksheet.Cells[count, 2].Style.Numberformat.Format = numberformat;
                worksheet.Cells[count, 3].Value = item.Real;
                worksheet.Cells[count, 3].Style.Numberformat.Format = numberformat;

                count++;

            }

            const double minWidth = 0.00;
            const double maxWidth = 50.00;

            // worksheet.Cells[2, 1, 4, 4].AutoFitColumns();
            worksheet.Cells.AutoFitColumns (minWidth, maxWidth);
            return package;
        }

        [HttpGet ("configurarReporte/{tipo_plan}")]
        public ActionResult configurarReporte (string tipo_plan) {
            var urlitems = $"../FinanzasWebApi/Helper/EstadoFinanciero/Configs/Config{tipo_plan}.json";
            var urlselection = $"../FinanzasWebApi/Helper/EstadoFinanciero/Configs/Config{tipo_plan}Hoja.json";
            var plan = System.IO.File.ReadAllText (urlitems);
            var planconfig = System.IO.File.ReadAllText (urlselection);
            dynamic obj = new {
                items = JsonConvert.DeserializeObject<List<dynamic>> (plan),
                selection = JsonConvert.DeserializeObject<List<dynamic>> (planconfig),
            };
            return Ok (obj);
        }

        [HttpPost ("configurarReporte")]
        public ActionResult configurarReportePost (ConfiguradorPlanViewModel viewModel) {
            var url = $"../FinanzasWebApi/Helper/EstadoFinanciero/Configs/Config{viewModel.TipoPlan}Hoja.json";
            string json = JsonConvert.SerializeObject (viewModel.Items);
            System.IO.File.WriteAllText (url, json);
            var result = JsonConvert.DeserializeObject<List<dynamic>> (json);
            return Ok (result);
        }

    }
}