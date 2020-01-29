using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanzasWebApi.Data;
using FinanzasWebApi.Helpers;
using FinanzasWebApi.Models;
using FinanzasWebApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FinanzasWebApi.Helper.EstadoFinanciero
{
    public class GetEF
    {
        IConfiguration _config { get; set; }
        FinanzasDbContext _context;

        public GetEF(IConfiguration config, FinanzasDbContext context)
        {
            _config = config;
            _context = context;
        }





        public List<EstadoFinancieroJsVM> activoCirculantes(int year, int meses)
        {
            var config = System.IO.File.ReadAllText("../FinanzasWebApi/Helper/EstadoFinanciero/Configs/Config5920.json");
            List<EfViewModel> configs = JsonConvert.DeserializeObject<List<EfViewModel>>(config);
            var estado = new List<EstadoFinancieroJsVM>();
            var planes = GetPlanesPeriodo.Get(year, _config);

            //Activo Circulante
            estado.Add(new EstadoFinancieroJsVM { Concepto = "Activo Circulante", EFE = "5920", Mes = meses, Real = 0, Plan = 0, Celda = "C5" });

            foreach (var concepto in configs.Where(s => s.concepto.Equals("Activo Circulante")))
            {
                foreach (var item2 in concepto.conceptos)
                {
                    var estadoTemp = new EstadoFinancieroJsVM();
                    string concept = FixText.Fix(item2.name);
                    var planEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim()))[meses] : 0M;

                    estadoTemp.Concepto = item2.name.ToString();
                    estadoTemp.Real = 0;
                    estadoTemp.Mes = meses;
                    estadoTemp.Celda = item2.celda.ToString();
                    estadoTemp.EFE = "5920";
                    estadoTemp.Plan = planEnMes;

                    foreach (var cuenta in item2.cuentas)
                    {

                        string cta = Convert.ToString(cuenta);
                        var cache = _context.Set<CacheCuentaPeriodo>().SingleOrDefault(c => c.Cuenta == cta && c.Mes == meses && c.Year == year);
                        if (cache != null)
                        {
                            estadoTemp.Real = estadoTemp.Real + cache.Saldo;
                        }
                    }
                    estado.Add(estadoTemp);
                }
            }

            //Total de Inventario
            estado.Add(new EstadoFinancieroJsVM { Concepto = "Total de Inventario", EFE = "5920", Mes = meses, Real = 0, Plan = 0, Celda = "C27" });
            foreach (var concepto in configs.Where(s => s.concepto.Equals("Total de Inventario")))
            {
                foreach (var item2 in concepto.conceptos)
                {
                    var estadoTemp = new EstadoFinancieroJsVM();
                    string concept = FixText.Fix(item2.name);
                    var planEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim()))[meses] : 0M;

                    estadoTemp.Concepto = item2.name.ToString();
                    estadoTemp.Real = 0;
                    estadoTemp.Mes = meses;
                    estadoTemp.Celda = item2.celda.ToString();
                    estadoTemp.EFE = "5920";
                    estadoTemp.Plan = planEnMes;

                    foreach (var cuenta in item2.cuentas)
                    {

                        string cta = Convert.ToString(cuenta);
                        var cache = _context.Set<CacheCuentaPeriodo>().SingleOrDefault(c => c.Cuenta == cta && c.Mes == meses && c.Year == year);
                        if (cache != null)
                        {
                            estadoTemp.Real = estadoTemp.Real + cache.Saldo;
                        }
                    }
                    estado.Add(estadoTemp);
                }
            }


            //Activos a Largo Plazo
            estado.Add(new EstadoFinancieroJsVM { Concepto = "Activos a Largo Plazo", EFE = "5920", Mes = meses, Real = 0, Plan = 0, Celda = "C55" });
            foreach (var concepto in configs.Where(s => s.concepto.Equals("Activos a Largo Plazo")))
            {
                foreach (var item2 in concepto.conceptos)
                {
                    var estadoTemp = new EstadoFinancieroJsVM();
                    string concept = FixText.Fix(item2.name);
                    var planEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim()))[meses] : 0M;

                    estadoTemp.Concepto = item2.name.ToString();
                    estadoTemp.Real = 0;
                    estadoTemp.Mes = meses;
                    estadoTemp.Celda = item2.celda.ToString();
                    estadoTemp.EFE = "5920";
                    estadoTemp.Plan = planEnMes;

                    foreach (var cuenta in item2.cuentas)
                    {

                        string cta = Convert.ToString(cuenta);
                        var cache = _context.Set<CacheCuentaPeriodo>().SingleOrDefault(c => c.Cuenta == cta && c.Mes == meses && c.Year == year);
                        if (cache != null)
                        {
                            estadoTemp.Real = estadoTemp.Real + cache.Saldo;
                        }
                    }
                    estado.Add(estadoTemp);
                }
            }

            //Activos Fijos
            estado.Add(new EstadoFinancieroJsVM { Concepto = "Activos Fijos", EFE = "5920", Mes = meses, Real = 0, Plan = 0, Celda = "C60" });
            foreach (var concepto in configs.Where(s => s.concepto.Equals("Activos Fijos")))
            {
                foreach (var item2 in concepto.conceptos)
                {
                    var estadoTemp = new EstadoFinancieroJsVM();
                    string concept = FixText.Fix(item2.name);
                    var planEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim()))[meses] : 0M;
                    estadoTemp.Concepto = item2.name.ToString();
                    estadoTemp.Real = 0;
                    estadoTemp.Mes = meses;
                    estadoTemp.Celda = item2.celda.ToString();
                    estadoTemp.EFE = "5920";
                    estadoTemp.Plan = planEnMes;

                    foreach (var cuenta in item2.cuentas)
                    {

                        string cta = Convert.ToString(cuenta);
                        var cache = _context.Set<CacheCuentaPeriodo>().SingleOrDefault(c => c.Cuenta == cta && c.Mes == meses && c.Year == year);
                        if (cache != null)
                        {
                            estadoTemp.Real = estadoTemp.Real + cache.Saldo;
                        }
                    }
                    estado.Add(estadoTemp);
                }
            }

            //Activos Diferidos
            estado.Add(new EstadoFinancieroJsVM { Concepto = "Activos Diferidos", EFE = "5920", Mes = meses, Real = 0, Plan = 0, Celda = "C73" });
            foreach (var concepto in configs.Where(s => s.concepto.Equals("Activos Diferidos")))
            {
                foreach (var item2 in concepto.conceptos)
                {
                    var estadoTemp = new EstadoFinancieroJsVM();
                    string concept = FixText.Fix(item2.name);
                    var planEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim()))[meses] : 0M;
                    estadoTemp.Concepto = item2.name.ToString();
                    estadoTemp.Real = 0;
                    estadoTemp.Mes = meses;
                    estadoTemp.Celda = item2.celda.ToString();
                    estadoTemp.EFE = "5920";
                    estadoTemp.Plan = planEnMes;

                    foreach (var cuenta in item2.cuentas)
                    {

                        string cta = Convert.ToString(cuenta);
                        var cache = _context.Set<CacheCuentaPeriodo>().SingleOrDefault(c => c.Cuenta == cta && c.Mes == meses && c.Year == year);
                        if (cache != null)
                        {
                            estadoTemp.Real = estadoTemp.Real + cache.Saldo;
                        }
                    }
                    estado.Add(estadoTemp);
                }
            }


            //Otros Activos
            estado.Add(new EstadoFinancieroJsVM { Concepto = "Otros Activos", EFE = "5920", Mes = meses, Real = 0, Plan = 0, Celda = "C78" });
            foreach (var concepto in configs.Where(s => s.concepto.Equals("Otros Activos")))
            {
                foreach (var item2 in concepto.conceptos)
                {
                    var estadoTemp = new EstadoFinancieroJsVM();
                    string concept = FixText.Fix(item2.name);
                    var planEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim()))[meses] : 0M;
                    estadoTemp.Concepto = item2.name.ToString();
                    estadoTemp.Real = 0;
                    estadoTemp.Mes = meses;
                    estadoTemp.Celda = item2.celda.ToString();
                    estadoTemp.EFE = "5920";
                    estadoTemp.Plan = planEnMes;

                    foreach (var cuenta in item2.cuentas)
                    {

                        string cta = Convert.ToString(cuenta);
                        var cache = _context.Set<CacheCuentaPeriodo>().SingleOrDefault(c => c.Cuenta == cta && c.Mes == meses && c.Year == year);
                        if (cache != null)
                        {
                            estadoTemp.Real = estadoTemp.Real + cache.Saldo;
                        }
                    }
                    estado.Add(estadoTemp);
                }
            }

            //TOTAL DEL ACTIVO 
            estado.Add(new EstadoFinancieroJsVM { Concepto = "TOTAL DEL ACTIVO", EFE = "5920", Mes = meses, Real = 0, Plan = 0, Celda = "C91" });

            //PASIVO 
            estado.Add(new EstadoFinancieroJsVM { Concepto = "PASIVO", EFE = "5920", Mes = meses, Real = 0, Plan = 0, Celda = "C92" });

            //Pasivo Circulante 
            estado.Add(new EstadoFinancieroJsVM { Concepto = "Pasivo Circulante", EFE = "5920", Mes = meses, Real = 0, Plan = 0, Celda = "C93" });
            foreach (var concepto in configs.Where(s => s.concepto.Equals("Pasivo Circulante")))
            {
                foreach (var item2 in concepto.conceptos)
                {
                    var estadoTemp = new EstadoFinancieroJsVM();
                    string concept = FixText.Fix(item2.name);
                    var planEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim()))[meses] : 0M;

                    estadoTemp.Concepto = item2.name.ToString();
                    estadoTemp.Real = 0;
                    estadoTemp.Mes = meses;
                    estadoTemp.Celda = item2.celda.ToString();
                    estadoTemp.EFE = "5920";
                    estadoTemp.Plan = planEnMes;

                    foreach (var cuenta in item2.cuentas)
                    {

                        string cta = Convert.ToString(cuenta);
                        var cache = _context.Set<CacheCuentaPeriodo>().SingleOrDefault(c => c.Cuenta == cta && c.Mes == meses && c.Year == year);
                        if (cache != null)
                        {
                            estadoTemp.Real = estadoTemp.Real + cache.Saldo;
                        }
                    }
                    estado.Add(estadoTemp);
                }
            }


            //Pasivos a Largo Plazo
            estado.Add(new EstadoFinancieroJsVM { Concepto = "Pasivos a Largo Plazo", EFE = "5920", Mes = meses, Real = 0, Plan = 0, Celda = "C115" });
            foreach (var concepto in configs.Where(s => s.concepto.Equals("Pasivos a Largo Plazo")))
            {
                foreach (var item2 in concepto.conceptos)
                {
                    var estadoTemp = new EstadoFinancieroJsVM();
                    string concept = FixText.Fix(item2.name);
                    var planEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim()))[meses] : 0M;

                    estadoTemp.Concepto = item2.name.ToString();
                    estadoTemp.Real = 0;
                    estadoTemp.Mes = meses;
                    estadoTemp.Celda = item2.celda.ToString();
                    estadoTemp.EFE = "5920";
                    estadoTemp.Plan = planEnMes;

                    foreach (var cuenta in item2.cuentas)
                    {

                        string cta = Convert.ToString(cuenta);
                        var cache = _context.Set<CacheCuentaPeriodo>().SingleOrDefault(c => c.Cuenta == cta && c.Mes == meses && c.Year == year);
                        if (cache != null)
                        {
                            estadoTemp.Real = estadoTemp.Real + cache.Saldo;
                        }
                    }
                    estado.Add(estadoTemp);
                }
            }

            //Pasivos Diferidos
            estado.Add(new EstadoFinancieroJsVM { Concepto = "Pasivos Diferidos", EFE = "5920", Mes = meses, Real = 0, Plan = 0, Celda = "C123" });
            foreach (var concepto in configs.Where(s => s.concepto.Equals("Pasivos Diferidos")))
            {
                foreach (var item2 in concepto.conceptos)
                {
                    var estadoTemp = new EstadoFinancieroJsVM();
                    string concept = FixText.Fix(item2.name);
                    var planEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim()))[meses] : 0M;

                    estadoTemp.Concepto = item2.name.ToString();
                    estadoTemp.Real = 0;
                    estadoTemp.Mes = meses;
                    estadoTemp.Celda = item2.celda.ToString();
                    estadoTemp.EFE = "5920";
                    estadoTemp.Plan = planEnMes;

                    foreach (var cuenta in item2.cuentas)
                    {

                        string cta = Convert.ToString(cuenta);
                        var cache = _context.Set<CacheCuentaPeriodo>().SingleOrDefault(c => c.Cuenta == cta && c.Mes == meses && c.Year == year);
                        if (cache != null)
                        {
                            estadoTemp.Real = estadoTemp.Real + cache.Saldo;
                        }
                    }
                    estado.Add(estadoTemp);
                }
            }

            //Otros Pasivos
            estado.Add(new EstadoFinancieroJsVM { Concepto = "Otros Pasivos", EFE = "5920", Mes = meses, Real = 0, Plan = 0, Celda = "C126" });
            foreach (var concepto in configs.Where(s => s.concepto.Equals("Otros Pasivos")))
            {
                foreach (var item2 in concepto.conceptos)
                {
                    var estadoTemp = new EstadoFinancieroJsVM();
                    string concept = FixText.Fix(item2.name);
                    var planEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim()))[meses] : 0M;

                    estadoTemp.Concepto = item2.name.ToString();
                    estadoTemp.Real = 0;
                    estadoTemp.Mes = meses;
                    estadoTemp.Celda = item2.celda.ToString();
                    estadoTemp.EFE = "5920";
                    estadoTemp.Plan = planEnMes;

                    foreach (var cuenta in item2.cuentas)
                    {

                        string cta = Convert.ToString(cuenta);
                        var cache = _context.Set<CacheCuentaPeriodo>().SingleOrDefault(c => c.Cuenta == cta && c.Mes == meses && c.Year == year);
                        if (cache != null)
                        {
                            estadoTemp.Real = estadoTemp.Real + cache.Saldo;
                        }
                    }
                    estado.Add(estadoTemp);
                }
            }


            //Total de Pasivos
            estado.Add(new EstadoFinancieroJsVM { Concepto = "TOTAL PASIVO", EFE = "5920", Mes = meses, Real = 0, Plan = 0, Celda = "C132" });

            //Patrimonio Neto
            estado.Add(new EstadoFinancieroJsVM { Concepto = "PATRIMONIO NETO O CAPITAL CONTABLE", EFE = "5920", Mes = meses, Real = 0, Plan = 0, Celda = "C133" });
            foreach (var concepto in configs.Where(s => s.concepto.Equals("PATRIMONIO NETO O CAPITAL CONTABLE")))
            {
                foreach (var item2 in concepto.conceptos)
                {
                    var estadoTemp = new EstadoFinancieroJsVM();
                    string concept = FixText.Fix(item2.name);
                    var planEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim()))[meses] : 0M;

                    estadoTemp.Concepto = item2.name.ToString();
                    estadoTemp.Real = 0;
                    estadoTemp.Mes = meses;
                    estadoTemp.Celda = item2.celda.ToString();
                    estadoTemp.EFE = "5920";
                    estadoTemp.Plan = planEnMes;

                    foreach (var cuenta in item2.cuentas)
                    {

                        string cta = Convert.ToString(cuenta);
                        var cache = _context.Set<CacheCuentaPeriodo>().SingleOrDefault(c => c.Cuenta == cta && c.Mes == meses && c.Year == year);
                        if (cache != null)
                        {
                            estadoTemp.Real = estadoTemp.Real + cache.Saldo;
                        }
                    }
                    estado.Add(estadoTemp);
                }
            }
            estado.Add(new EstadoFinancieroJsVM { Concepto = "Capital Social Suscrito y Pagado", EFE = "5920", Mes = meses, Real = 0, Plan = 0, Celda = "C136" });
            foreach (var concepto in configs.Where(s => s.concepto.Equals("Capital Social Suscrito y Pagado")))
            {
                foreach (var item2 in concepto.conceptos)
                {
                    var estadoTemp = new EstadoFinancieroJsVM();
                    string concept = FixText.Fix(item2.name);
                    var planEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(concept.ToUpper().Trim()))[meses] : 0M;

                    estadoTemp.Concepto = item2.name.ToString();
                    estadoTemp.Real = 0;
                    estadoTemp.Mes = meses;
                    estadoTemp.Celda = item2.celda.ToString();
                    estadoTemp.EFE = "5920";
                    estadoTemp.Plan = planEnMes;

                    foreach (var cuenta in item2.cuentas)
                    {

                        string cta = Convert.ToString(cuenta);
                        var cache = _context.Set<CacheCuentaPeriodo>().SingleOrDefault(c => c.Cuenta == cta && c.Mes == meses && c.Year == year);
                        if (cache != null)
                        {
                            estadoTemp.Real = estadoTemp.Real + cache.Saldo;
                        }
                    }
                    estado.Add(estadoTemp);
                }
            }

            estado.Add(new EstadoFinancieroJsVM { Concepto = "Resultado del Período", EFE = "5920", Mes = meses, Real = 0, Plan = 0, Celda = "C155" });
            estado.Add(new EstadoFinancieroJsVM { Concepto = "TOTAL  PATRIMONIO NETO", EFE = "5920", Mes = meses, Real = 0, Plan = 0, Celda = "C156" });
            estado.Add(new EstadoFinancieroJsVM { Concepto = "TOTAL DEL PASIVO Y PATRIMONIO NETO O CAPITAL CONTABLE", EFE = "5920", Mes = meses, Real = 0, Plan = 0, Celda = "C157" });

            var activoC = estado.SingleOrDefault(s => s.Concepto.Equals("Activo Circulante"));

            decimal C6 = estado.SingleOrDefault(s => s.Celda.Equals("C6")).Real;
            decimal C7 = estado.SingleOrDefault(s => s.Celda.Equals("C7")).Real;
            decimal C8 = estado.SingleOrDefault(s => s.Celda.Equals("C8")).Real;
            decimal C9 = estado.SingleOrDefault(s => s.Celda.Equals("C9")).Real;
            decimal C10 = estado.SingleOrDefault(s => s.Celda.Equals("C10")).Real;
            decimal C11 = estado.SingleOrDefault(s => s.Celda.Equals("C11")).Real;
            decimal C13 = estado.SingleOrDefault(s => s.Celda.Equals("C13")).Real;
            decimal C14 = estado.SingleOrDefault(s => s.Celda.Equals("C14")).Real;
            decimal C15 = estado.SingleOrDefault(s => s.Celda.Equals("C15")).Real;
            decimal C16 = estado.SingleOrDefault(s => s.Celda.Equals("C16")).Real;
            decimal C17 = estado.SingleOrDefault(s => s.Celda.Equals("C17")).Real;
            decimal C18 = estado.SingleOrDefault(s => s.Celda.Equals("C18")).Real;
            decimal C19 = estado.SingleOrDefault(s => s.Celda.Equals("C19")).Real;
            decimal C20 = estado.SingleOrDefault(s => s.Celda.Equals("C20")).Real;
            decimal C21 = estado.SingleOrDefault(s => s.Celda.Equals("C21")).Real;
            decimal C22 = estado.SingleOrDefault(s => s.Celda.Equals("C22")).Real;
            decimal C23 = estado.SingleOrDefault(s => s.Celda.Equals("C23")).Real;
            decimal C54 = estado.SingleOrDefault(s => s.Celda.Equals("C54")).Real;
            decimal activoCValue = (C6 + C7 + C8 + C9 + C11 + C14 + C15 + C16 + C17 + C18 + C19 + C20 + C21 + C22 + C23 + C54) - (C10 + C13);
            activoC.Real = activoCValue;
            string activoCConcept = FixText.Fix("Activo Circulante");
            var activoCPlanEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(activoCConcept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(activoCConcept.ToUpper().Trim()))[meses] : 0M;
            activoC.Plan = activoCPlanEnMes;

            var totalDeInvetario = estado.SingleOrDefault(s => s.Concepto.Equals("Total de Inventario"));
            decimal C28 = estado.SingleOrDefault(s => s.Celda.Equals("C28")).Real;
            decimal C29 = estado.SingleOrDefault(s => s.Celda.Equals("C29")).Real;
            decimal C30 = estado.SingleOrDefault(s => s.Celda.Equals("C30")).Real;
            decimal C31 = estado.SingleOrDefault(s => s.Celda.Equals("C31")).Real;
            decimal C32 = estado.SingleOrDefault(s => s.Celda.Equals("C32")).Real;
            decimal C33 = estado.SingleOrDefault(s => s.Celda.Equals("C33")).Real;
            decimal C34 = estado.SingleOrDefault(s => s.Celda.Equals("C34")).Real;
            decimal C35 = estado.SingleOrDefault(s => s.Celda.Equals("C35")).Real;
            decimal C36 = estado.SingleOrDefault(s => s.Celda.Equals("C36")).Real;
            decimal C37 = estado.SingleOrDefault(s => s.Celda.Equals("C37")).Real;
            decimal C38 = estado.SingleOrDefault(s => s.Celda.Equals("C38")).Real;
            decimal C39 = estado.SingleOrDefault(s => s.Celda.Equals("C39")).Real;
            decimal C40 = estado.SingleOrDefault(s => s.Celda.Equals("C40")).Real;
            decimal C41 = estado.SingleOrDefault(s => s.Celda.Equals("C41")).Real;
            decimal C42 = estado.SingleOrDefault(s => s.Celda.Equals("C42")).Real;
            decimal C43 = estado.SingleOrDefault(s => s.Celda.Equals("C43")).Real;
            decimal C44 = estado.SingleOrDefault(s => s.Celda.Equals("C44")).Real;
            decimal C45 = estado.SingleOrDefault(s => s.Celda.Equals("C45")).Real;
            decimal C46 = estado.SingleOrDefault(s => s.Celda.Equals("C46")).Real;
            decimal C47 = estado.SingleOrDefault(s => s.Celda.Equals("C47")).Real;
            decimal C48 = estado.SingleOrDefault(s => s.Celda.Equals("C48")).Real;
            decimal C49 = estado.SingleOrDefault(s => s.Celda.Equals("C49")).Real;
            decimal C50 = estado.SingleOrDefault(s => s.Celda.Equals("C50")).Real;
            decimal C51 = estado.SingleOrDefault(s => s.Celda.Equals("C51")).Real;
            decimal C52 = estado.SingleOrDefault(s => s.Celda.Equals("C52")).Real;
            decimal C53 = estado.SingleOrDefault(s => s.Celda.Equals("C53")).Real;
            decimal totalDeInvetarioValue = (C28 + C29 + C30 + C31 + C32 + C34 + C35 + C37 + C42 + C43 + C44 + C45 + C46 + C47 + C48 + C49 + C50 + C51 + C52 + C53) - (C33 + C36 + C39 + C41);
            totalDeInvetario.Real = totalDeInvetarioValue;
            string totalDeInvetarioConcept = FixText.Fix("Total de Inventario");
            var totalDeInvetarioPlanEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(totalDeInvetarioConcept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(totalDeInvetarioConcept.ToUpper().Trim()))[meses] : 0M;
            totalDeInvetario.Plan = totalDeInvetarioPlanEnMes;

            var activoLP = estado.SingleOrDefault(s => s.Concepto.Equals("Activos a Largo Plazo"));
            string activoLPConcept = FixText.Fix("Activos a Largo Plazo");
            var activoLPPlanEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(activoLPConcept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(activoLPConcept.ToUpper().Trim()))[meses] : 0M;
            activoLP.Plan = activoLPPlanEnMes;
            decimal C56 = estado.SingleOrDefault(s => s.Celda.Equals("C56")).Real;
            decimal C57 = estado.SingleOrDefault(s => s.Celda.Equals("C57")).Real;
            decimal C58 = estado.SingleOrDefault(s => s.Celda.Equals("C58")).Real;
            decimal C59 = estado.SingleOrDefault(s => s.Celda.Equals("C59")).Real;
            decimal activoLPValue = C56 + C57 + C58 + C59;
            activoLP.Real = activoLPValue;

            var activoF = estado.SingleOrDefault(s => s.Concepto.Equals("Activos Fijos"));
            string activoFConcept = FixText.Fix("Activos Fijos");
            var activoFPlanEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(activoFConcept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(activoFConcept.ToUpper().Trim()))[meses] : 0M;
            activoF.Plan = activoFPlanEnMes;
            decimal C61 = estado.SingleOrDefault(s => s.Celda.Equals("C61")).Real;
            decimal C62 = estado.SingleOrDefault(s => s.Celda.Equals("C62")).Real;
            decimal C63 = estado.SingleOrDefault(s => s.Celda.Equals("C63")).Real;
            decimal C64 = estado.SingleOrDefault(s => s.Celda.Equals("C64")).Real;
            decimal C65 = estado.SingleOrDefault(s => s.Celda.Equals("C65")).Real;
            decimal C66 = estado.SingleOrDefault(s => s.Celda.Equals("C66")).Real;
            decimal C67 = estado.SingleOrDefault(s => s.Celda.Equals("C67")).Real;
            decimal C69 = estado.SingleOrDefault(s => s.Celda.Equals("C69")).Real;
            decimal C70 = estado.SingleOrDefault(s => s.Celda.Equals("C70")).Real;
            decimal C71 = estado.SingleOrDefault(s => s.Celda.Equals("C71")).Real;
            decimal C72 = estado.SingleOrDefault(s => s.Celda.Equals("C72")).Real;
            decimal activoFValue = (C64 + C63 + C61 + C66 + C67 + C70 + C71 + C72) - (C62 + C65 + C69);
            activoF.Real = activoFValue;

            var activoD = estado.SingleOrDefault(s => s.Concepto.Equals("Activos Diferidos"));
            string activoDConcept = FixText.Fix("Activos Diferidos");
            var activoDPlanEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(activoDConcept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(activoDConcept.ToUpper().Trim()))[meses] : 0M;
            activoD.Plan = activoDPlanEnMes;
            decimal C74 = estado.SingleOrDefault(s => s.Celda.Equals("C74")).Real;
            decimal C75 = estado.SingleOrDefault(s => s.Celda.Equals("C75")).Real;
            decimal C76 = estado.SingleOrDefault(s => s.Celda.Equals("C76")).Real;
            decimal C77 = estado.SingleOrDefault(s => s.Celda.Equals("C77")).Real;
            decimal activoDValue = C74 + C75 + C76 + C77;
            activoD.Real = activoDValue;

            var otrosActivos = estado.SingleOrDefault(s => s.Concepto.Equals("Otros Activos"));
            string otrosActivosConcept = FixText.Fix("Otros Activos");
            var otrosActivosPlanEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(otrosActivosConcept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(otrosActivosConcept.ToUpper().Trim()))[meses] : 0M;
            otrosActivos.Plan = otrosActivosPlanEnMes;
            decimal C79 = estado.SingleOrDefault(s => s.Celda.Equals("C79")).Real;
            decimal C80 = estado.SingleOrDefault(s => s.Celda.Equals("C80")).Real;
            decimal C81 = estado.SingleOrDefault(s => s.Celda.Equals("C81")).Real;
            decimal C82 = estado.SingleOrDefault(s => s.Celda.Equals("C82")).Real;
            decimal C83 = estado.SingleOrDefault(s => s.Celda.Equals("C83")).Real;
            decimal C84 = estado.SingleOrDefault(s => s.Celda.Equals("C84")).Real;
            decimal C85 = estado.SingleOrDefault(s => s.Celda.Equals("C85")).Real;
            decimal C86 = estado.SingleOrDefault(s => s.Celda.Equals("C86")).Real;
            decimal C87 = estado.SingleOrDefault(s => s.Celda.Equals("C87")).Real;
            decimal C88 = estado.SingleOrDefault(s => s.Celda.Equals("C88")).Real;
            decimal C89 = estado.SingleOrDefault(s => s.Celda.Equals("C89")).Real;
            decimal C90 = estado.SingleOrDefault(s => s.Celda.Equals("C90")).Real;
            decimal otrosActivosValue = (C79 + C80 + C81 + C82 + C83 + C84 + C85 + C86 + C87 + C88 + C89) - C90;
            otrosActivos.Real = otrosActivosValue;

            var pasivoCirculante = estado.SingleOrDefault(s => s.Concepto.Equals("Pasivo Circulante"));
            string pasivoCirculanteConcept = FixText.Fix("Pasivo Circulante");
            var pasivoCirculantePlanEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(pasivoCirculanteConcept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(pasivoCirculanteConcept.ToUpper().Trim()))[meses] : 0M;
            pasivoCirculante.Plan = pasivoCirculantePlanEnMes;
            decimal C94 = estado.SingleOrDefault(s => s.Celda.Equals("C94")).Real;
            decimal C95 = estado.SingleOrDefault(s => s.Celda.Equals("C94")).Real;
            decimal C96 = estado.SingleOrDefault(s => s.Celda.Equals("C94")).Real;
            decimal C97 = estado.SingleOrDefault(s => s.Celda.Equals("C94")).Real;
            decimal C98 = estado.SingleOrDefault(s => s.Celda.Equals("C94")).Real;
            decimal C99 = estado.SingleOrDefault(s => s.Celda.Equals("C94")).Real;
            decimal C100 = estado.SingleOrDefault(s => s.Celda.Equals("C100")).Real;
            decimal C101 = estado.SingleOrDefault(s => s.Celda.Equals("C101")).Real;
            decimal C102 = estado.SingleOrDefault(s => s.Celda.Equals("C102")).Real;
            decimal C103 = estado.SingleOrDefault(s => s.Celda.Equals("C103")).Real;
            decimal C104 = estado.SingleOrDefault(s => s.Celda.Equals("C104")).Real;
            decimal C105 = estado.SingleOrDefault(s => s.Celda.Equals("C105")).Real;
            decimal C106 = estado.SingleOrDefault(s => s.Celda.Equals("C106")).Real;
            decimal C107 = estado.SingleOrDefault(s => s.Celda.Equals("C107")).Real;
            decimal C108 = estado.SingleOrDefault(s => s.Celda.Equals("C108")).Real;
            decimal C109 = estado.SingleOrDefault(s => s.Celda.Equals("C109")).Real;
            decimal C110 = estado.SingleOrDefault(s => s.Celda.Equals("C110")).Real;
            decimal C111 = estado.SingleOrDefault(s => s.Celda.Equals("C111")).Real;
            decimal C112 = estado.SingleOrDefault(s => s.Celda.Equals("C112")).Real;
            decimal C113 = estado.SingleOrDefault(s => s.Celda.Equals("C113")).Real;
            decimal C114 = estado.SingleOrDefault(s => s.Celda.Equals("C114")).Real;
            decimal pasivoCirculanteValue = C94 + C95 + C96 + C97 + C98 + C99 + C100 + C101 + C102 + C103 + C104 + C105 + C106 + C107 + C108 + C109 + C110 + C111 + C112 + C113 + C114;
            pasivoCirculante.Real = pasivoCirculanteValue;

            var pasivosALargoPlazo = estado.SingleOrDefault(s => s.Concepto.Equals("Pasivos a Largo Plazo"));
            string pasivosALargoPlazoConcept = FixText.Fix("Pasivos a Largo Plazo");
            var pasivosALargoPlazoPlanEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(pasivosALargoPlazoConcept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(pasivosALargoPlazoConcept.ToUpper().Trim()))[meses] : 0M;
            pasivosALargoPlazo.Plan = pasivosALargoPlazoPlanEnMes;
            decimal C116 = estado.SingleOrDefault(s => s.Celda.Equals("C116")).Real;
            decimal C117 = estado.SingleOrDefault(s => s.Celda.Equals("C117")).Real;
            decimal C118 = estado.SingleOrDefault(s => s.Celda.Equals("C118")).Real;
            decimal C119 = estado.SingleOrDefault(s => s.Celda.Equals("C119")).Real;
            decimal C120 = estado.SingleOrDefault(s => s.Celda.Equals("C120")).Real;
            decimal C121 = estado.SingleOrDefault(s => s.Celda.Equals("C121")).Real;
            decimal C122 = estado.SingleOrDefault(s => s.Celda.Equals("C122")).Real;
            decimal pasivosALargoPlazoValue = C116 + C117 + C118 + C119 + C120 + C121 + C122;
            pasivosALargoPlazo.Real = pasivosALargoPlazoValue;

            var pasivosDiferidos = estado.SingleOrDefault(s => s.Concepto.Equals("Pasivos Diferidos"));
            string pasivosDiferidosConcept = FixText.Fix("Pasivos Diferidos");
            var pasivosDiferidosPlanEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(pasivosDiferidosConcept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(pasivosDiferidosConcept.ToUpper().Trim()))[meses] : 0M;
            pasivosDiferidos.Plan = pasivosDiferidosPlanEnMes;
            decimal C124 = estado.SingleOrDefault(s => s.Celda.Equals("C124")).Real;
            decimal C125 = estado.SingleOrDefault(s => s.Celda.Equals("C125")).Real;
            decimal pasivosDiferidosValue = C124 + C125;
            pasivosDiferidos.Real = pasivosDiferidosValue;

            var otrosPasivos = estado.SingleOrDefault(s => s.Concepto.Equals("Otros Pasivos"));
            string otrosPasivosConcept = FixText.Fix("Otros Pasivos");
            var otrosPasivosPlanEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(otrosPasivosConcept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(otrosPasivosConcept.ToUpper().Trim()))[meses] : 0M;
            otrosPasivos.Plan = otrosPasivosPlanEnMes;
            decimal C127 = estado.SingleOrDefault(s => s.Celda.Equals("C127")).Real;
            decimal C128 = estado.SingleOrDefault(s => s.Celda.Equals("C128")).Real;
            decimal C129 = estado.SingleOrDefault(s => s.Celda.Equals("C129")).Real;
            decimal C130 = estado.SingleOrDefault(s => s.Celda.Equals("C130")).Real;
            decimal C131 = estado.SingleOrDefault(s => s.Celda.Equals("C131")).Real;
            decimal otrosPasivosValue = C127 + C128 + C129 + C130 + C131;
            otrosPasivos.Real = otrosPasivosValue;

            var totalActivos = estado.SingleOrDefault(s => s.Concepto.Equals("TOTAL DEL ACTIVO"));
            string totalActivosConcept = FixText.Fix("TOTAL DEL ACTIVO");
            var totalActivosPlanEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(totalActivosConcept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(totalActivosConcept.ToUpper().Trim()))[meses] : 0M;
            totalActivos.Plan = totalActivosPlanEnMes;
            decimal C5 = estado.SingleOrDefault(s => s.Celda.Equals("C5")).Real;
            decimal C55 = estado.SingleOrDefault(s => s.Celda.Equals("C55")).Real;
            decimal C60 = estado.SingleOrDefault(s => s.Celda.Equals("C60")).Real;
            decimal C73 = estado.SingleOrDefault(s => s.Celda.Equals("C73")).Real;
            decimal C78 = estado.SingleOrDefault(s => s.Celda.Equals("C78")).Real;
            decimal totalActivosValue = C5 + C55 + C60 + C73 + C78;
            totalActivos.Real = totalActivosValue;

            var totalPasivo = estado.SingleOrDefault(s => s.Concepto.Equals("TOTAL PASIVO"));
            string totalPasivoConcept = FixText.Fix("TOTAL PASIVO");
            var totalPasivoPlanEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(totalPasivoConcept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(totalPasivoConcept.ToUpper().Trim()))[meses] : 0M;
            totalPasivo.Plan = totalPasivoPlanEnMes;
            decimal C93 = estado.SingleOrDefault(s => s.Celda.Equals("C93")).Real;
            decimal C115 = estado.SingleOrDefault(s => s.Celda.Equals("C115")).Real;
            decimal C123 = estado.SingleOrDefault(s => s.Celda.Equals("C123")).Real;
            decimal C126 = estado.SingleOrDefault(s => s.Celda.Equals("C126")).Real;
            decimal totalPasivoValue = C93 + C115 + C123 + C126;
            totalPasivo.Real = totalPasivoValue;

            var totalPatrimonio = estado.SingleOrDefault(s => s.Concepto.Equals("TOTAL  PATRIMONIO NETO"));
            string totalPatrimonioConcept = FixText.Fix("TOTAL  PATRIMONIO NETO");
            var totalPatrimonioPlanEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(totalPatrimonioConcept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(totalPatrimonioConcept.ToUpper().Trim()))[meses] : 0M;
            totalPatrimonio.Plan = totalPatrimonioPlanEnMes;
            decimal C134 = estado.SingleOrDefault(s => s.Celda.Equals("C134")).Real;
            decimal C135 = estado.SingleOrDefault(s => s.Celda.Equals("C135")).Real;
            decimal C136 = estado.SingleOrDefault(s => s.Celda.Equals("C136")).Real;
            decimal C137 = estado.SingleOrDefault(s => s.Celda.Equals("C137")).Real;
            decimal C138 = estado.SingleOrDefault(s => s.Celda.Equals("C138")).Real;
            decimal C139 = estado.SingleOrDefault(s => s.Celda.Equals("C139")).Real;
            decimal C140 = estado.SingleOrDefault(s => s.Celda.Equals("C140")).Real;
            decimal C141 = estado.SingleOrDefault(s => s.Celda.Equals("C141")).Real;
            decimal C142 = estado.SingleOrDefault(s => s.Celda.Equals("C142")).Real;
            decimal C143 = estado.SingleOrDefault(s => s.Celda.Equals("C143")).Real;
            decimal C144 = estado.SingleOrDefault(s => s.Celda.Equals("C144")).Real;
            decimal C145 = estado.SingleOrDefault(s => s.Celda.Equals("C145")).Real;
            decimal C146 = estado.SingleOrDefault(s => s.Celda.Equals("C146")).Real;
            decimal C147 = estado.SingleOrDefault(s => s.Celda.Equals("C147")).Real;
            decimal C148 = estado.SingleOrDefault(s => s.Celda.Equals("C148")).Real;
            decimal C149 = estado.SingleOrDefault(s => s.Celda.Equals("C149")).Real;
            decimal C150 = estado.SingleOrDefault(s => s.Celda.Equals("C150")).Real;
            decimal C151 = estado.SingleOrDefault(s => s.Celda.Equals("C151")).Real;
            decimal C152 = estado.SingleOrDefault(s => s.Celda.Equals("C152")).Real;
            decimal C153 = estado.SingleOrDefault(s => s.Celda.Equals("C153")).Real;
            decimal totalPatrimonioValue = (C134 + C135 + C136 + C137 + C138 + C139 + C140 + C141 + C142 + C143 + C144) - (C145 + C146 + C147 + C148 + C149) + (C151 + C152 + C153);
            totalPatrimonio.Real = totalPatrimonioValue;


            var totalPasivoPatrimonio = estado.SingleOrDefault(s => s.Concepto.Equals("TOTAL DEL PASIVO Y PATRIMONIO NETO O CAPITAL CONTABLE"));
            string totalPasivoPatrimonioConcept = FixText.Fix("TOTAL DEL PASIVO Y PATRIMONIO NETO O CAPITAL CONTABLE");
            var totalPasivoPatrimonioPlanEnMes = planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(totalPasivoPatrimonioConcept.ToUpper().Trim())) != null ? planes.SingleOrDefault(s => s.Concepto.ToUpper().Trim().Equals(totalPasivoPatrimonioConcept.ToUpper().Trim()))[meses] : 0M;
            totalPasivoPatrimonio.Plan = totalPasivoPatrimonioPlanEnMes;
            decimal C132 = estado.SingleOrDefault(s => s.Celda.Equals("C132")).Real;
            decimal C154 = estado.SingleOrDefault(s => s.Celda.Equals("C154")).Real;
            decimal C155 = estado.SingleOrDefault(s => s.Celda.Equals("C155")).Real;
            decimal C156 = estado.SingleOrDefault(s => s.Celda.Equals("C156")).Real;
            decimal totalPasivoPatrimonioValue = C132 + C134 + C135 + C136 + C137 + C139 + C140 + C142 + C143 + C144 + C145 + C146 + C147 + C148 + C149 + C150 + C151 + C152 + C153 + C154 + C155 + C156;
            totalPasivoPatrimonio.Real = totalPasivoPatrimonioValue;


            return estado;
        }



    }
}
