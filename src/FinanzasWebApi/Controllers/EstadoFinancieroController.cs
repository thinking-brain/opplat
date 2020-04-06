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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace FinanzasWebApi.Controllers {
    [Route ("finanzas/[controller]")]
    [ApiController]
    public class EstadoFinancieroController : ControllerBase {
        private const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private readonly IHostingEnvironment _hostingEnvironment;
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

    }
}