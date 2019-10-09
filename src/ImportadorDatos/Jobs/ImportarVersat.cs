
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ContabilidadWebApi.Data;
using ContabilidadWebApi.Models;
using ImportadorDatos.Models.EnlaceVersat;
using ImportadorDatos.Models.Versat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ImportadorDatos.Jobs
{
    public class ImportarVersat
    {
        VersatDbContext _vContext;

        ContabilidadDbContext _cContext;

        EnlaceVersatDbContext _enlaceContext;

        ILogger _logger;

        public ImportarVersat(VersatDbContext vContext, ContabilidadDbContext cContext, EnlaceVersatDbContext enlaceContext, ILogger<ImportarVersat> logger)
        {
            _vContext = vContext;
            _cContext = cContext;
            _enlaceContext = enlaceContext;
            _logger = logger;
        }

        public void ImportarCuentasAsync()
        {
            //todo: revisar las cuentas que faltan por importar
            //todo: guardar en bd independiente las cuentas que se importaron
            var cuentasImportadas = _enlaceContext.Set<Cuentas>().Select(c => c.CuentaVersatId).ToList();
            var cuentas = _vContext.Set<ConCuenta>()
                .Include(c => c.IdaperturaNavigation.IdmascaraNavigation)
                .Where(c => !cuentasImportadas.Contains(c.Idcuenta))
                .OrderBy(c => c.Clave.Length);

            string baseUrl = "https://localhost:5001/contabilidad/cuentas";
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            var cuentasApi = new List<CuentaDto>();
            using (HttpClient client = new HttpClient(handler))
            {
                using (HttpResponseMessage res = client.GetAsync(baseUrl).Result)
                {
                    if (res.StatusCode != HttpStatusCode.OK)
                    {
                        cuentasApi = res.Content.ReadAsAsync<List<CuentaDto>>().Result;
                    }
                }
            }
            handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            using (HttpClient client = new HttpClient(handler))
            {
                foreach (var cta in cuentas)
                {
                    var numero = GetNumeroCuenta(cta.Clave, cta.IdaperturaNavigation.IdmascaraNavigation.Posicion);
                    var descripcion = _vContext.Query<Con_Cuentadescrip>().SingleOrDefault(c => c.Idcuenta == cta.Idcuenta).Descripcion;
                    var naturaleza = _vContext.Query<ConCuentanatur>().SingleOrDefault(c => c.Idcuenta == cta.Idcuenta).Naturaleza;
                    if (!cuentasApi.Any(c => c.Numero == numero))
                    {
                        var datosLogin = new Dictionary<string, string>();
                        datosLogin.Add("numero", numero);
                        datosLogin.Add("nombre", descripcion);
                        datosLogin.Add("naturaleza", (naturaleza < 0 ? 0 : naturaleza).ToString());
                        using (HttpResponseMessage res = client.PostAsJsonAsync(baseUrl, datosLogin).Result)
                        {
                            if (res.StatusCode != HttpStatusCode.OK)
                            {
                                var data = res.Content.ReadAsStringAsync().Result;
                                _logger.LogError($"error con cuenta {numero},  {data}");
                            }
                            if (res.StatusCode == HttpStatusCode.OK)
                            {
                                var data = res.Content.ReadAsAsync<CuentaDto>().Result;
                                var id = data.Id;
                                _enlaceContext.Add(new Cuentas { CuentaId = id, CuentaVersatId = cta.Idcuenta, Fecha = DateTime.Now });
                                _enlaceContext.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        //todo: hacer PUT con cambios de cuentas
                    }
                }
            }

        }

        public void ImportarPeriodosContables()
        {
            //todo: guardar en db independiente los periodos migrados
            var periodosVersat = _vContext.Set<GenPeriodo>().OrderBy(p => p.Inicio);
            foreach (var per in periodosVersat)
            {
                if (_cContext.Set<PeriodoContable>().Any(p => p.FechaInicio == per.Inicio && p.FechaFin == per.Fin))
                {

                }
                else
                {
                    _cContext.Add(new PeriodoContable { FechaInicio = per.Inicio, FechaFin = per.Fin, Activo = per.Enuso ?? per.Enuso.Value });
                }
            }
            _cContext.SaveChanges();
        }


        public void ImportarAsientos()
        {
            //todo: controlar en DB independiente cuando importe un asiento
            var operacionesVersat = _vContext.Set<ConPase>()
                .Include(c => c.IdcomprobanteNavigation.ConComprobanteoperacion.IdusuarioNavigation)
                .Include(c => c.IdcomprobanteNavigation.ConComprobanteoperacion.IdperiodoNavigation)
                .Include(c => c.IdcuentaNavigation.IdaperturaNavigation.IdmascaraNavigation)
                .Where(c => c.IdcomprobanteNavigation.ConComprobanteoperacion.Idestado == 5)
                .GroupBy(c => c.IdcomprobanteNavigation).Select(c => new
                {
                    Comprobante = c.Key,
                    Operaciones = c
                });
            foreach (var asi in operacionesVersat)
            {

                var diaContable = _cContext.Set<DiaContable>().SingleOrDefault(d => d.Fecha.Date == asi.Comprobante.ConComprobanteoperacion.Fecha.Date);
                if (diaContable == null)
                {
                    var periodo = _cContext.Set<PeriodoContable>()
                    .SingleOrDefault(p => p.FechaInicio.Date == asi.Comprobante.ConComprobanteoperacion.IdperiodoNavigation.Inicio.Date
                        && p.FechaFin.Date == asi.Comprobante.ConComprobanteoperacion.IdperiodoNavigation.Fin.Date);
                    diaContable = new DiaContable
                    {
                        Fecha = asi.Comprobante.ConComprobanteoperacion.Fecha,
                        Abierto = false,
                        HoraEnQueCerro = asi.Comprobante.ConComprobanteoperacion.Fecha.Date.AddHours(23),
                        PeriodoContableId = periodo.Id
                    };
                }
                var nuevoAsiento = new Asiento
                {
                    Detalle = asi.Comprobante.Descripcion,
                    Fecha = asi.Comprobante.ConComprobanteoperacion.Fecha,
                    UsuarioId = asi.Comprobante.ConComprobanteoperacion.IdusuarioNavigation.Loginusuario,
                    DiaContable = diaContable,
                };
                foreach (var op in asi.Operaciones)
                {
                    var numero = GetNumeroCuenta(op.IdcuentaNavigation.Clave, op.IdcuentaNavigation.IdaperturaNavigation.IdmascaraNavigation.Posicion);
                    var cuenta = _cContext.Set<Cuenta>()
                        .Include(c => c.CuentaSuperior.CuentaSuperior.CuentaSuperior.CuentaSuperior)
                        .SingleOrDefault(c => c.Numero == numero);
                    if (cuenta == null)
                    {
                        Console.WriteLine($"Error cuenta {numero} no existe.");
                    }
                    else
                    {
                        var tipoOperacion = TipoDeOperacion.Debito;
                        if ((cuenta.Naturaleza == Naturaleza.Deudora && op.Importe < 0) || (cuenta.Naturaleza == Naturaleza.Acreedora && op.Importe > 0))
                        {
                            tipoOperacion = TipoDeOperacion.Credito;
                        }
                        var mov = new Movimiento
                        {
                            CuentaId = cuenta.Id,
                            Importe = Math.Abs(op.Importe),
                            TipoDeOperacion = tipoOperacion
                        };
                        nuevoAsiento.Movimientos.Add(mov);
                    }

                }
                _cContext.Add(nuevoAsiento);
                _cContext.SaveChanges();
            }
        }

        private string GetNumeroCuenta(string clave, int posicion)
        {
            var formatos = new int[6] { 0, 3, 4, 6, 6, 6 };
            var numero = "";
            var index = 0;
            var lenght = clave.Length;
            for (int i = 1; i <= posicion; i++)
            {
                var offset = index;
                for (int j = index; j < offset + formatos[i]; j++, index++)
                {
                    if (j >= lenght)
                    {
                        break;
                    }
                    numero += clave[j];
                }
                numero += "-";
            }
            numero = numero.Remove(numero.Length - 1);
            return numero;
        }
    }


    class CuentaDto
    {
        public int Id { get; set; }

        public string Numero { get; set; }

        public string Descripcion { get; set; }

        public int Naturaleza { get; set; }
    }
}