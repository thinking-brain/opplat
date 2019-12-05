
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RhWebApi.Data;

namespace ImportadorDatos.Jobs
{
    public class ImportarVersat
    {
        VersatDbContext _vContext;

        ContabilidadDbContext _cContext;

        RhWebApiDbContext _rhContext;

        EnlaceVersatDbContext _enlaceContext;

        IConfiguration _config;

        ILogger _logger;

        public ImportarVersat(VersatDbContext vContext, ContabilidadDbContext cContext, RhWebApiDbContext rhContext, EnlaceVersatDbContext enlaceContext, ILogger<ImportarVersat> logger, IConfiguration config)
        {
            _vContext = vContext;
            _cContext = cContext;
            _rhContext = rhContext;
            _enlaceContext = enlaceContext;
            _logger = logger;
            _config = config;
        }

        public void ImportarCuentasAsync()
        {
            //todo: revisar las cuentas que su padre es vacio            
            var cuentasImportadas = _enlaceContext.Set<Cuentas>().Select(c => c.CuentaVersatId).ToList();
            var cuentas = _vContext.Set<ConCuenta>()
                .Include(c => c.IdaperturaNavigation.IdmascaraNavigation)
                .Where(c => !cuentasImportadas.Contains(c.Idcuenta))
                .OrderBy(c => c.Clave.Length);

            string baseUrl = _config.GetValue<string>("ContabilidadApi") + "/cuentas";
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            using (HttpClient client = new HttpClient(handler))
            {
                foreach (var cta in cuentas)
                {
                    var numero = GetNumeroCuenta(cta.Clave, cta.IdaperturaNavigation.IdmascaraNavigation.Posicion);
                    var descripcion = _vContext.Query<Con_Cuentadescrip>().SingleOrDefault(c => c.Idcuenta == cta.Idcuenta).Descripcion;
                    var naturaleza = _vContext.Query<ConCuentanatur>().SingleOrDefault(c => c.Idcuenta == cta.Idcuenta).Naturaleza;
                    if (!cuentasImportadas.Contains(cta.Idcuenta))
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
            //todo: revisar los periodos cuando ya existe uno en la BD
            var periodosVersat = _vContext.Set<GenPeriodo>().OrderBy(p => p.Inicio);
            var periodosImportados = _enlaceContext.Set<PeriodosContables>().Select(c => c.PeriodoVersatId).ToList();
            foreach (var per in periodosVersat)
            {
                if (!periodosImportados.Contains(per.Idperiodo))
                {
                    var nuevoPeriodo = new PeriodoContable { FechaInicio = per.Inicio, FechaFin = per.Fin, Activo = per.Enuso ?? per.Enuso.Value };
                    _cContext.Add(nuevoPeriodo);
                    _cContext.SaveChanges();
                    _enlaceContext.Add(new PeriodosContables { PeriodoId = nuevoPeriodo.Id, PeriodoVersatId = per.Idperiodo, Fecha = DateTime.Now });
                    _enlaceContext.SaveChanges();
                }
            }
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
                    Operaciones = c.Select(g => g)
                });
            var comprobantesImportados = _enlaceContext.Set<Asientos>().Select(c => c.ComprobanteId).ToList();
            foreach (var asi in operacionesVersat)
            {
                if (!comprobantesImportados.Contains(asi.Comprobante.Idcomprobante))
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
                    bool valid = true;
                    foreach (var op in asi.Operaciones)
                    {
                        var numero = GetNumeroCuenta(op.IdcuentaNavigation.Clave, op.IdcuentaNavigation.IdaperturaNavigation.IdmascaraNavigation.Posicion);
                        var cuenta = _cContext.Set<Cuenta>()
                            .Include(c => c.CuentaSuperior)
                            .ToList()
                            .SingleOrDefault(c => c.Numero == numero);
                        if (cuenta == null)
                        {
                            _logger.LogError($"Error cuenta {numero} no existe.");
                            valid = false;
                            break;
                        }
                        else
                        {
                            var tipoOperacion = TipoDeOperacion.Debito;
                            if (op.Importe < 0)
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
                    if (valid)
                    {
                        _cContext.Add(nuevoAsiento);
                        _cContext.SaveChanges();
                        _enlaceContext.Add(new Asientos { AsientoId = nuevoAsiento.Id, ComprobanteId = asi.Comprobante.Idcomprobante, Fecha = DateTime.Now });
                        _enlaceContext.SaveChanges();
                    }
                }
            }
        }

        public void ImportarTrabajadores()
        {
            //todo: revisar los periodos cuando ya existe uno en la BD
            var trabajadoresVersat = _vContext.Set<GenTrabajador>();
            var trabajadoresImportados = _enlaceContext.Set<ImportadorDatos.Models.EnlaceVersat.Trabajador>().Select(c => c.TrabajadorVersatId).ToList();
            foreach (var trabajador in trabajadoresVersat)
            {
                if (!trabajadoresImportados.Contains(trabajador.Idtrabajador))
                {
                    var estado = "Activo";
                    if (trabajador.Activo == null || !trabajador.Activo.Value)
                    {
                        estado = "No Activo";
                    }
                    //todo: agregar y modificar campos
                    var nuevoTrabajador = new RhWebApi.Models.Trabajador
                    {
                        CI = trabajador.Numident,
                        Nombre = trabajador.Nombres,
                        Apellidos = trabajador.Apellido1 + " " + trabajador.Apellido2,
                        Direccion = trabajador.Direccion,
                        EstadoTrabajador = estado,
                    };
                    _rhContext.Add(nuevoTrabajador);
                    _rhContext.SaveChanges();
                    _enlaceContext.Add(new ImportadorDatos.Models.EnlaceVersat.Trabajador
                    {
                        TrabajadorId = nuevoTrabajador.Id,
                        TrabajadorVersatId = trabajador.Idtrabajador,
                        Ci = trabajador.Numident,
                    });
                    _enlaceContext.SaveChanges();
                }
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