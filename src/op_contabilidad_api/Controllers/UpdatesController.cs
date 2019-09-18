using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using op_contabilidad_api.Helper;
using op_contabilidad_api.Models;
using op_contabilidad_api.VersatModels;
using op_contabilidad_api.VersatModels2;
using op_contabilidad_api.ViewModels;

namespace op_contabilidad_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdatesController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly VersatDbContext _vcontext;
        private readonly VersatDbContext2 _v2context;

        public UpdatesController(ApiDbContext context, VersatDbContext vcontext, VersatDbContext2 v2context)
        {
            _context = context;
            _vcontext = vcontext;
            _v2context = v2context;
        }

        /// <summary>
        /// Actualizar Areas
        /// </summary>
        /// <returns></returns>
        // GET api/values
        [HttpGet("api/UpdateArea/")]
        public ActionResult<IEnumerable<string>> UpdateArea()
        {
            _context.Set<Area>().Add(new Area { Codigo = "01", Nombre = "LA CONCORDIA" });
            _context.SaveChanges();
            return new string[] { "UPDATE AREA" };
        }


        /// <summary>
        /// Actualizar Areas + Centros de Costos
        /// </summary>
        /// <returns></returns>
        // GET api/values
        [HttpGet("api/UpdateAC/")]
        public ActionResult<IEnumerable<string>> UpdateACentro()
        {
            var area = _context.Set<Area>().First();
            var relacion = _context.Set<CentroCostoArea>().ToList();
            var opt = _vcontext.Set<VersatModels.OptCuentaCentroSubPeriodo>().Select(s => s.Clavecentro).Distinct().ToList();
            foreach (var item in opt)
            {
                if (!relacion.Any(s => s.CentroCostoId == item && s.AreaId == 1))
                {
                    relacion.Add(new CentroCostoArea { AreaId = 1, CentroCostoId = item, Detalles = area.Codigo + "-" + area.Nombre });

                }
            }
            foreach (var item in relacion.ToList())
            {
                _context.Set<CentroCostoArea>().Add(item);
                _context.SaveChanges();
            }

            return new string[] { "UPDATE AC" };
        }


        /// <summary>
        /// Actualizar Grupos de SubElementos + SubElementos
        /// </summary>
        /// <returns></returns>
        // GET api/values
        [HttpGet("api/UpdateGSS/")]
        public ActionResult<IEnumerable<string>> UpdateGSS()
        {
            var grupos = _vcontext.Set<VersatModels.CosPartida>().ToList();


            foreach (var item in grupos)
            {
                if (!_context.Set<GrupoSubelemento>().Any(s => s.Nombre == item.Descripcion))
                {
                    _context.Add(new GrupoSubelemento { Nombre = item.Descripcion, Codigo = item.Codigo });
                }
                _context.SaveChanges();
            }


            var GSSelect = _context.Set<GrupoSubelemento>().ToList();

            foreach (var item in GSSelect)
            {
                var partida = _vcontext.Set<VersatModels.CosPartida>().SingleOrDefault(s => s.Codigo.Equals(item.Codigo));
                var sub = _vcontext.Set<VersatModels.CosSubelementogasto>().Where(s => s.Idpartida == partida.Idpartida).ToList();

                foreach (var subelemento in sub)
                {
                    if (!_context.Set<GrupoSubElemento_SubElemento>().Any(s => s.GrupoSubelementoId == item.Id && s.SubElementoGastoId == subelemento.Codigo))
                    {
                        _context.Set<GrupoSubElemento_SubElemento>().Add(new GrupoSubElemento_SubElemento { GrupoSubelementoId = item.Id, SubElementoGastoId = subelemento.Codigo, Descripcion = subelemento.Descripcion });
                        _context.SaveChanges();
                    }

                }
            }

            return new string[] { "UPDATE GSS" };
        }

        /// <summary>
        /// Actualizar SubMayor de Cuentas
        /// </summary>
        /// <returns></returns>
        // GET api/values
        [HttpGet("api/UpdateSubmayorDeCuentas/")]
        public ActionResult<IEnumerable<string>> UpdateSubmayorDeCuentas()
        {
            var submayorCuenta = _context.Set<SubMayorCuenta>().ToList();
            if (!submayorCuenta.Any(s=>s.Año == 2019 && s.Mes > 4))
            {
                var reportED = new UpdateSubmayorCuenta2Helper(_v2context, _context);
                reportED.LlenarSubMayor2();
            }

            var reportVersat = new UpdateSubmayorCuentaHelper(_vcontext, _context);
            reportVersat.LlenarSubMayor2();

            return new string[] { "UPDATE SC" };
        }

        /// <summary>
        /// Actualizar Objetos de Obras
        /// </summary>
        /// <returns></returns>
        // GET api/values
        [HttpGet("api/UpdateObjetosDeObra/")]
        public ActionResult<IEnumerable<string>> UpdateObjetosDeObra()
        {
            var oo = new List<ConceptoPlan>();
            foreach (var item in _vcontext.ConCuenta)
            {
                var cuentaNat = _vcontext.ConCuentanat.SingleOrDefault(s => s.Idcuenta == item.Idcuenta);
                if (item.Idcuenta == 2504)
                {
                    var r = 0;
                }
                var clav = item.Clave.TrimEnd().Split(' ');
                List<string> clavFix = new List<string>();
                foreach (var itemX in clav)
                {
                    if (itemX != "")
                    {
                        clavFix.Add(itemX + " ");
                    }
                   
                }
                if (cuentaNat != null)
                {
                    if (cuentaNat.Naturaleza == -1 && clavFix.ToList().Count == 3)
                    {
                        var cuenta = _vcontext.ConCuenta.SingleOrDefault(s => s.Idcuenta == item.Idcuenta);
                        string moneda = _vcontext.ConCuentamoneda.SingleOrDefault(s => s.Idcuenta == cuenta.Idcuenta).Idmoneda == 2 ? "CUC" : "CUP";

                        if (!oo.Any(s => s.Concepto.Equals(cuentaNat.Descripcion)))
                        {
                            var concepto = new ConceptoPlan { Concepto = cuentaNat.Descripcion };
                            concepto.Cuentas.Add(new ConceptoCuentas { CuentaId = item.Idcuenta });
                            oo.Add(concepto);

                        }
                        else
                        {
                            var concepto = oo.SingleOrDefault(s => s.Concepto.Equals(cuentaNat.Descripcion));
                            concepto.Cuentas.Add(new ConceptoCuentas { CuentaId = item.Idcuenta });
                        }
                    }

                }


            }
            foreach (var item in oo)
            {
                if (item.Cuentas.Count == 2)
                {
                    var conc = new ConceptoPlan { Concepto = item.Concepto };
                    foreach (var cuent in item.Cuentas)
                    {
                        conc.Cuentas.Add(cuent);
                    }
                   
                    _context.Set<ConceptoPlan>().Add(conc);
                    _context.SaveChanges();
                }
            }
           
                return new string[] { "UPDATE OO" };
        }

        /// <summary>
        /// Actualizar Conceptos Plan Pronostico Productivo
        /// </summary>
        /// <returns></returns>
        // GET api/values
        [HttpGet("api/UpdateCPP/")]
        public ActionResult<IEnumerable<string>> UpdateCPP()
        {
            var Conceptos = new List<ConceptoPlan>();

            var concepto1 = new ConceptoPlan { Concepto = "Reparación Restaurante -Bar Cafeteria Bahia" };
            concepto1.Cuentas.Add(new ConceptoCuentas { CuentaId = 2031 }); 
            concepto1.Cuentas.Add(new ConceptoCuentas { CuentaId = 2034 });
            Conceptos.Add(concepto1);

            var concepto2 = new ConceptoPlan { Concepto = "Reparacion Teatro Sauto Cafet.Artex y Edific.Socio Administrativo" };
            concepto2.Cuentas.Add(new ConceptoCuentas { CuentaId = 2010 });
            concepto2.Cuentas.Add(new ConceptoCuentas { CuentaId = 2011 });
            Conceptos.Add(concepto2);

            var concepto3 = new ConceptoPlan { Concepto = "Hotel Dominica" };
            concepto3.Cuentas.Add(new ConceptoCuentas { CuentaId = 2052 });
            concepto3.Cuentas.Add(new ConceptoCuentas { CuentaId = 2054 });
            Conceptos.Add(concepto3);

            var concepto4 = new ConceptoPlan { Concepto = "Cafeteria Franboyan" };
            concepto4.Cuentas.Add(new ConceptoCuentas { CuentaId = 2238 });
            concepto4.Cuentas.Add(new ConceptoCuentas { CuentaId = 2239 });
            Conceptos.Add(concepto4);


            foreach (var item in Conceptos)
            {
                _context.Set<PlanPronosticoProductivo>().Add(new PlanPronosticoProductivo
                {
                    ConceptoPlan = item,
                    Año = "2019",
                    Enero = 0M,
                    Febrero = 0M,
                    Marzo = 0M,
                    Abril = 0M,
                    Mayo = 0M,
                    Junio = 0M,
                    Julio = 0M,
                    Agosto = 0M,
                    Septiembre = 0M,
                    Octubre = 0M,
                    Noviembre = 0M,
                    Diciembre = 0M,
                    Fecha = DateTime.Now

                });
                _context.SaveChanges();
            }


            return new string[] { "UPDATE PPP" };
        }

    }
}
