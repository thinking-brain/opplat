using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ContabilidadWebApi.Data;
using ContabilidadWebApi.Models;
using ContratacionWebApi.Data;
using ContratacionWebApi.Models;
using ImportadorDatos.Models.EnlaceVersat;
using ImportadorDatos.Models.Versat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RhWebApi.Data;

namespace ImportadorDatos.Jobs {
    public class ImportarVersat {
        VersatDbContext _vContext;

        ContabilidadDbContext _cContext;

        RhWebApiDbContext _rhContext;
        ContratacionDbContext _contratacionContext;

        EnlaceVersatDbContext _enlaceContext;

        IConfiguration _config;

        ILogger _logger;

        public ImportarVersat (VersatDbContext vContext, ContabilidadDbContext cContext, RhWebApiDbContext rhContext, EnlaceVersatDbContext enlaceContext, ILogger<ImportarVersat> logger, IConfiguration config, ContratacionDbContext contratacionContext) {
            _vContext = vContext;
            _cContext = cContext;
            _rhContext = rhContext;
            _enlaceContext = enlaceContext;
            _logger = logger;
            _config = config;
            _contratacionContext = contratacionContext;
        }

        public void ImportarCuentasAsync () {
            //todo: revisar las cuentas que su padre es vacio            
            var cuentasImportadas = _enlaceContext.Set<Cuentas> ().Select (c => c.CuentaVersatId).ToList ();
            var cuentas = _vContext.Set<ConCuenta> ()
                .Include (c => c.IdaperturaNavigation.IdmascaraNavigation)
                .Where (c => !cuentasImportadas.Contains (c.Idcuenta))
                .OrderBy (c => c.Clave.Length);

            string baseUrl = _config.GetValue<string> ("ContabilidadApi") + "/cuentas";
            var handler = new HttpClientHandler ();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            using (HttpClient client = new HttpClient (handler)) {
                foreach (var cta in cuentas) {
                    var numero = GetNumeroCuenta (cta.Clave, cta.IdaperturaNavigation.IdmascaraNavigation.Posicion);
                    var descripcion = _vContext.Query<Con_Cuentadescrip> ().SingleOrDefault (c => c.Idcuenta == cta.Idcuenta).Descripcion;
                    var naturaleza = _vContext.Query<ConCuentanatur> ().SingleOrDefault (c => c.Idcuenta == cta.Idcuenta).Naturaleza;
                    if (!cuentasImportadas.Contains (cta.Idcuenta)) {
                        var datosLogin = new Dictionary<string, string> ();
                        datosLogin.Add ("numero", numero);
                        datosLogin.Add ("nombre", descripcion);
                        datosLogin.Add ("naturaleza", (naturaleza < 0 ? 0 : naturaleza).ToString ());
                        using (HttpResponseMessage res = client.PostAsJsonAsync (baseUrl, datosLogin).Result) {
                            if (res.StatusCode != HttpStatusCode.OK) {
                                var data = res.Content.ReadAsStringAsync ().Result;
                                _logger.LogError ($"error con cuenta {numero},  {data}");
                            }
                            if (res.StatusCode == HttpStatusCode.OK) {
                                var data = res.Content.ReadAsAsync<CuentaDto> ().Result;
                                var id = data.Id;
                                _enlaceContext.Add (new Cuentas { CuentaId = id, CuentaVersatId = cta.Idcuenta, Fecha = DateTime.Now });
                                _enlaceContext.SaveChanges ();
                            }
                        }
                    } else {
                        //todo: hacer PUT con cambios de cuentas
                    }
                }
            }

        }

        public void ImportarPeriodosContables () {
            //todo: revisar los periodos cuando ya existe uno en la BD
            var periodosVersat = _vContext.Set<GenPeriodo> ().OrderBy (p => p.Inicio);
            var periodosImportados = _enlaceContext.Set<PeriodosContables> ().Select (c => c.PeriodoVersatId).ToList ();
            foreach (var per in periodosVersat) {
                if (!periodosImportados.Contains (per.Idperiodo)) {
                    var nuevoPeriodo = new PeriodoContable { FechaInicio = per.Inicio, FechaFin = per.Fin, Activo = per.Enuso ?? per.Enuso.Value };
                    _cContext.Add (nuevoPeriodo);
                    _cContext.SaveChanges ();
                    _enlaceContext.Add (new PeriodosContables { PeriodoId = nuevoPeriodo.Id, PeriodoVersatId = per.Idperiodo, Fecha = DateTime.Now });
                    _enlaceContext.SaveChanges ();
                }
            }
        }

        public void ImportarAsientos () {
            //todo: controlar en DB independiente cuando importe un asiento
            var operacionesVersat = _vContext.Set<ConPase> ()
                .Include (c => c.IdcomprobanteNavigation.ConComprobanteoperacion.IdusuarioNavigation)
                .Include (c => c.IdcomprobanteNavigation.ConComprobanteoperacion.IdperiodoNavigation)
                .Include (c => c.IdcuentaNavigation.IdaperturaNavigation.IdmascaraNavigation)
                .Where (c => c.IdcomprobanteNavigation.ConComprobanteoperacion.Idestado == 5)
                .GroupBy (c => c.IdcomprobanteNavigation).Select (c => new {
                    Comprobante = c.Key,
                        Operaciones = c.Select (g => g)
                });
            var comprobantesImportados = _enlaceContext.Set<Asientos> ().Select (c => c.ComprobanteId).ToList ();
            foreach (var asi in operacionesVersat) {
                if (!comprobantesImportados.Contains (asi.Comprobante.Idcomprobante)) {
                    var diaContable = _cContext.Set<DiaContable> ().SingleOrDefault (d => d.Fecha.Date == asi.Comprobante.ConComprobanteoperacion.Fecha.Date);
                    if (diaContable == null) {
                        var periodo = _cContext.Set<PeriodoContable> ()
                            .SingleOrDefault (p => p.FechaInicio.Date == asi.Comprobante.ConComprobanteoperacion.IdperiodoNavigation.Inicio.Date &&
                                p.FechaFin.Date == asi.Comprobante.ConComprobanteoperacion.IdperiodoNavigation.Fin.Date);
                        diaContable = new DiaContable {
                            Fecha = asi.Comprobante.ConComprobanteoperacion.Fecha,
                            Abierto = false,
                            HoraEnQueCerro = asi.Comprobante.ConComprobanteoperacion.Fecha.Date.AddHours (23),
                            PeriodoContableId = periodo.Id
                        };
                    }
                    var nuevoAsiento = new Asiento {
                        Detalle = asi.Comprobante.Descripcion,
                        Fecha = asi.Comprobante.ConComprobanteoperacion.Fecha,
                        UsuarioId = asi.Comprobante.ConComprobanteoperacion.IdusuarioNavigation.Loginusuario,
                        DiaContable = diaContable,
                    };
                    bool valid = true;
                    foreach (var op in asi.Operaciones) {
                        var numero = GetNumeroCuenta (op.IdcuentaNavigation.Clave, op.IdcuentaNavigation.IdaperturaNavigation.IdmascaraNavigation.Posicion);
                        var cuenta = _cContext.Set<Cuenta> ()
                            .Include (c => c.CuentaSuperior)
                            .ToList ()
                            .SingleOrDefault (c => c.Numero == numero);
                        if (cuenta == null) {
                            _logger.LogError ($"Error cuenta {numero} no existe.");
                            valid = false;
                            break;
                        } else {
                            var tipoOperacion = TipoDeOperacion.Debito;
                            if (op.Importe < 0) {
                                tipoOperacion = TipoDeOperacion.Credito;
                            }
                            var mov = new Movimiento {
                                CuentaId = cuenta.Id,
                                Importe = Math.Abs (op.Importe),
                                TipoDeOperacion = tipoOperacion
                            };
                            nuevoAsiento.Movimientos.Add (mov);
                        }
                    }
                    if (valid) {
                        _cContext.Add (nuevoAsiento);
                        _cContext.SaveChanges ();
                        _enlaceContext.Add (new Asientos { AsientoId = nuevoAsiento.Id, ComprobanteId = asi.Comprobante.Idcomprobante, Fecha = DateTime.Now });
                        _enlaceContext.SaveChanges ();
                    }
                }
            }
        }

        public void ImportarTrabajadores () {
            var trabajadoresVersat = _vContext.Set<GenTrabajador> ();
            var trabajadoresImportados = _enlaceContext.Set<ImportadorDatos.Models.EnlaceVersat.Trabajador> ().Select (c => c.TrabajadorVersatId).ToList ();
            var sexo = new RhWebApi.Data.Sexo ();
            string fecha = "";
            DateTime fechaNac = new DateTime ();
            IFormatProvider culture = new System.Globalization.CultureInfo ("en-US", true);

            foreach (var trabajador in trabajadoresVersat) {
                if (!trabajadoresImportados.Contains (trabajador.Idtrabajador) && trabajador.Numident != null) {
                    var estado = Estados.Activo;
                    if (trabajador.Activo == null || !trabajador.Activo.Value) {
                        estado = Estados.Baja;
                    }
                    if (trabajador.Numident != null) {
                        var sexoCI = int.Parse (trabajador.Numident.Substring (9, 1));
                        if (sexoCI % 2 == 0) {
                            sexo = Sexo.M;
                        } else {
                            sexo = Sexo.F;
                        }
                        var siglo = int.Parse (trabajador.Numident.Substring (6, 1));
                        if (siglo >= 0 && siglo <= 5) {
                            fecha = "19" + trabajador.Numident.Substring (0, 6);
                        }
                        if (siglo >= 6 && siglo <= 8) {
                            fecha = "20" + trabajador.Numident.Substring (0, 6);
                        }
                        if (fecha != "19000000" && fecha != "20000000") {
                            fechaNac = DateTime.ParseExact (fecha, "yyyyMMdd", culture);
                        }
                    }

                    //todo: Tener en cuenta agregar un municipio vacio (ninguno), y un cargo vacio(ninguno)
                    var nuevoTrabajador = new RhWebApi.Models.Trabajador {
                        CI = trabajador.Numident,
                        Codigo = trabajador.Codigo,
                        Nombre = trabajador.Nombres,
                        Apellidos = trabajador.Apellido1 + " " + trabajador.Apellido2,
                        Direccion = trabajador.Direccion,
                        EstadoTrabajador = estado,
                        Sexo = sexo,
                        Fecha_Nac = fechaNac,
                        PerfilOcupacionalId = 1,
                        PuestoDeTrabajoId = 1
                    };

                    _rhContext.Add (nuevoTrabajador);
                    _rhContext.SaveChanges ();

                    var caractTrab = new RhWebApi.Models.CaracteristicasTrab {
                        TrabajadorId = nuevoTrabajador.Id,
                        ColorDePiel = 0,
                        ColorDeOjos = 0,
                        TallaCalzado = 0,
                        TallaDeCamisa = 0
                    };
                    _rhContext.Add (caractTrab);
                    _rhContext.SaveChanges ();

                    _enlaceContext.Add (new ImportadorDatos.Models.EnlaceVersat.Trabajador {
                        TrabajadorId = nuevoTrabajador.Id,
                            TrabajadorVersatId = trabajador.Idtrabajador,
                            Ci = trabajador.Numident,
                    });
                    _enlaceContext.SaveChanges ();
                }
            }
        }
        public void ImportarUnidadesOrganizativas () {
            var unidadesOrganizativasImportadas = _enlaceContext.Set<UnidadOrganizativa> ().Select (c => c.AreaVersatId).ToList ();
            var unidadesOrganizativas = _vContext.Set<GenArea> ()
                .Include (c => c.IdaperturaNavigation.IdmascaraNavigation)
                .Where (c => !unidadesOrganizativasImportadas.Contains (c.Idarea))
                .OrderBy (c => c.Clave.Length);

            if (!_rhContext.Set<RhWebApi.Models.TipoUnidadOrganizativa> ().Any (t => t.Nombre == "Área")) {
                _rhContext.Add (new RhWebApi.Models.TipoUnidadOrganizativa { Nombre = "Área", Prioridad = 1, });
                _rhContext.SaveChanges ();
            }
            if (!_rhContext.Set<RhWebApi.Models.TipoUnidadOrganizativa> ().Any (t => t.Nombre == "SubÁrea")) {
                _rhContext.Add (new RhWebApi.Models.TipoUnidadOrganizativa { Nombre = "SubÁrea", Prioridad = 2, });
                _rhContext.SaveChanges ();
            }
            if (!_rhContext.Set<RhWebApi.Models.TipoUnidadOrganizativa> ().Any (t => t.Nombre == "Departamento")) {
                _rhContext.Add (new RhWebApi.Models.TipoUnidadOrganizativa { Nombre = "Departamento", Prioridad = 3 });
                _rhContext.SaveChanges ();
            }
            var idTipoArea = _rhContext.Set<RhWebApi.Models.TipoUnidadOrganizativa> ().SingleOrDefault (t => t.Nombre == "Área").Id;
            var idTipoSubArea = _rhContext.Set<RhWebApi.Models.TipoUnidadOrganizativa> ().SingleOrDefault (t => t.Nombre == "SubÁrea").Id;
            var idTipoDepartamento = _rhContext.Set<RhWebApi.Models.TipoUnidadOrganizativa> ().SingleOrDefault (t => t.Nombre == "Departamento").Id;

            foreach (var uo in unidadesOrganizativas) {
                var idTipo = 0;
                if (uo.Clave.Length == 3) {
                    idTipo = idTipoArea;
                }
                if (uo.Clave.Length == 6) {
                    idTipo = idTipoSubArea;
                }
                if (uo.Clave.Length == 9) {
                    idTipo = idTipoDepartamento;
                }
                var nuevaUO = new RhWebApi.Models.UnidadOrganizativa () {
                    Codigo = uo.Clave,
                    Nombre = uo.Descripcion,
                    Activa = uo.Activa.Value,
                    TipoUnidadOrganizativaId = idTipo,
                };
                var codigoPadre = uo.Clave.Substring (0, uo.Clave.Length - 3);
                var padre = _rhContext.Set<RhWebApi.Models.UnidadOrganizativa> ().SingleOrDefault (u => u.Codigo == codigoPadre);
                if (padre != null) {
                    nuevaUO.PerteneceAId = padre.Id;
                }
                var nueva = _rhContext.Add (nuevaUO);
                _rhContext.SaveChanges ();
                _enlaceContext.Add (new ImportadorDatos.Models.EnlaceVersat.UnidadOrganizativa {
                    Fecha = DateTime.Now,
                        AreaVersatId = uo.Idarea,
                        UnidadOrganizativaId = nuevaUO.Id,
                });
                _enlaceContext.SaveChanges ();
            }
        }

        public void ImportarElementosDeGastos () {
            var importados = _enlaceContext.ElementoDeGastos.Select (e => e.ElementoVersatId).ToList ();
            foreach (var elem in _vContext.CosElementogasto.Where (e => !importados.Contains (e.Idelementogasto))) {
                var newElemento = new ContabilidadWebApi.Models.ElementoDeGasto {
                Activo = elem.Activo.HasValue ? elem.Activo.Value : false,
                Descripcion = elem.Descripcion,
                Codigo = elem.Codigo,
                };
                _cContext.Add (newElemento);
                _cContext.SaveChanges ();
                _enlaceContext.Add (new Models.EnlaceVersat.ElementoDeGasto {
                    ElementoId = newElemento.Id,
                        ElementoVersatId = elem.Idelementogasto,
                        Fecha = DateTime.Now,
                });
                _enlaceContext.SaveChanges ();
            }
        }
        public void ImportarPartidasDeGastos () {
            var importados = _enlaceContext.PartidaDeGastos.Select (e => e.PartidaVersatId).ToList ();
            foreach (var elem in _vContext.CosPartida.Where (e => !importados.Contains (e.Idpartida))) {
                var newElemento = new ContabilidadWebApi.Models.PartidaDeGasto {
                Activo = elem.Activo.HasValue ? elem.Activo.Value : false,
                Desripcion = elem.Descripcion,
                Codigo = elem.Codigo,
                };
                _cContext.Add (newElemento);
                _cContext.SaveChanges ();
                _enlaceContext.Add (new Models.EnlaceVersat.PartidaDeGasto {
                    PartidaId = newElemento.Id,
                        PartidaVersatId = elem.Idpartida,
                        Fecha = DateTime.Now,
                });
                _enlaceContext.SaveChanges ();
            }
        }
        public void ImportarSubElementosDeGastos () {
            var importados = _enlaceContext.SubElementoDeGastos.Select (e => e.SubElementoVersatId).ToList ();
            foreach (var elem in _vContext.CosSubelementogasto.Where (e => !importados.Contains (e.Idsubelemento))) {
                var newElemento = new ContabilidadWebApi.Models.SubElementoDeGasto {
                Activo = elem.Activo.HasValue ? elem.Activo.Value : false,
                Descripcion = elem.Descripcion,
                ElementoId = _enlaceContext.ElementoDeGastos.SingleOrDefault (e => e.ElementoVersatId == elem.Idelementogasto).ElementoId,
                PartidaId = _enlaceContext.PartidaDeGastos.SingleOrDefault (e => e.PartidaVersatId == elem.Idpartida).PartidaId,
                Codigo = elem.Codigo,
                };
                _cContext.Add (newElemento);
                _cContext.SaveChanges ();
                _enlaceContext.Add (new Models.EnlaceVersat.SubElementoDeGasto {
                    SubElementoId = newElemento.Id,
                        SubElementoVersatId = elem.Idsubelemento,
                        Fecha = DateTime.Now,
                });
                _enlaceContext.SaveChanges ();
            }
        }

        public void ImportarCentrosDeCostos () {
            var importados = _enlaceContext.CentrosDeCostos.Select (e => e.CentroVersatId).ToList ();
            foreach (var elem in _vContext.CosCentro.Where (e => !importados.Contains (e.Idcentro))) {
                var newElemento = new ContabilidadWebApi.Models.CentroDeCosto {
                Activo = elem.Activo.HasValue ? elem.Activo.Value : false,
                Nombre = elem.Descripcion,
                Codigo = elem.Clave,
                };
                _cContext.Add (newElemento);
                _cContext.SaveChanges ();
                _enlaceContext.Add (new Models.EnlaceVersat.CentroDeCosto {
                    CentroId = newElemento.Id,
                        CentroVersatId = elem.Idcentro,
                        Fecha = DateTime.Now,
                });
                _enlaceContext.SaveChanges ();
            }
        }

        public void ImportarRegistrosDeGastos () {
            var importados = _enlaceContext.RegistroDeGastos.Select (e => e.RegistroVersatId).ToList ();
            var registros = _vContext.CosPasesubelemento
                .Include (e => e.IdpaseNavigation.IdregistroNavigation.IdregistroNavigation.IdpaseNavigation)
                .Select (e => new {
                    Id = e.Idpase,
                        ComprobanteId = e.IdpaseNavigation.IdregistroNavigation.IdregistroNavigation.IdpaseNavigation.Idcomprobante,
                        SubElementoId = e.Idsubelemento,
                        Importe = e.IdpaseNavigation.IdregistroNavigation.Importe.HasValue ? e.IdpaseNavigation.IdregistroNavigation.Importe.Value : 0
                })
                .ToList ()
                .Where (e => !importados.Contains (e.Id));
            foreach (var elem in registros) {
                if (_enlaceContext.Asientos
                    .Any (a => a.ComprobanteId ==
                        elem.ComprobanteId)) {
                    var newElemento = new ContabilidadWebApi.Models.RegistroDeGasto {
                    AsientoId = _enlaceContext.Asientos
                    .SingleOrDefault (a => a.ComprobanteId ==
                    elem.ComprobanteId).AsientoId,
                    SubElementoId = _enlaceContext.SubElementoDeGastos.SingleOrDefault (se => se.SubElementoVersatId == elem.SubElementoId).SubElementoId,
                    Importe = elem.Importe,
                    };
                    _cContext.Add (newElemento);
                    _cContext.SaveChanges ();
                    _enlaceContext.Add (new Models.EnlaceVersat.RegistroDeGasto {
                        RegistroId = newElemento.Id,
                            RegistroVersatId = elem.Id,
                            Fecha = DateTime.Now,
                    });
                    _enlaceContext.SaveChanges ();
                } else {
                    _logger.LogError ("Error importando gasto. Falta por importar asiento contable.");
                }
            }
        }

        private string GetNumeroCuenta (string clave, int posicion) {
            var formatos = new int[6] { 0, 3, 4, 6, 6, 6 };
            var numero = "";
            var index = 0;
            var lenght = clave.Length;
            for (int i = 1; i <= posicion; i++) {
                var offset = index;
                for (int j = index; j < offset + formatos[i]; j++, index++) {
                    if (j >= lenght) {
                        break;
                    }
                    numero += clave[j];
                }
                numero += "-";
            }
            numero = numero.Remove (numero.Length - 1);
            return numero;
        }
        public void ImportarEntidades () {
            var entidadesVersat = _vContext.Set<GenEntidad> ();
            var entidadesImportadas = _enlaceContext.Set<ImportadorDatos.Models.EnlaceVersat.Entidad> ()
                .Select (c => c.EntidadVersatId).ToList ();
            var ctaBancoEntidadVersat = _vContext.Set<GenCtaBancoEntidad> ();
            var sucursalBancoVersat = _vContext.Set<GenSucursalBanco> ();
            var monedaVersat = _vContext.Set<GenMoneda> ();
            var clasifBancoVersat = _vContext.Set<GenClasifBanco> ();
            var cuentasBancariasImportadas = _enlaceContext.Set<ImportadorDatos.Models.EnlaceVersat.CuentaBancaria> ();

            foreach (var entidad in entidadesVersat) {
                if (!entidadesImportadas.Contains (entidad.IdEntidad)) {
                    var estado = Estados.Activo;
                    if (entidad.Activo == null || !entidad.Activo.Value) {
                        estado = Estados.Baja;
                    }
                    if (entidad.NIT == null) {
                        entidad.NIT = "Sin Definir";
                    }
                    var nuevaEntidad = new ContratacionWebApi.Models.Entidad {
                        Nit = entidad.NIT,
                        Codigo = entidad.Codigo,
                        Nombre = entidad.Nombre,
                        Direccion = entidad.Direccion + " " + entidad.Provincia + " " + entidad.Pais,
                        Correo = entidad.Email,
                    };

                    _contratacionContext.Add (nuevaEntidad);
                    if (entidad.Telefono != null) {
                        var telefonos = entidad.Telefono.Split ("Y");
                        foreach (var item in telefonos) {
                            if (item != "") {
                                var telefonoEntidad = new ContratacionWebApi.Models.Telefono {
                                Numero = item,
                                EntidadId = nuevaEntidad.Id,
                                };
                                _contratacionContext.Add (telefonoEntidad);
                            }
                        }
                    }
                    // if (ctaBancoEntidadVersat != null) {
                    //     foreach (var item in ctaBancoEntidadVersat) {
                    //         if (item.IdEntidad == entidad.IdEntidad) {
                    //             var Moneda = new ContratacionWebApi.Models.Moneda ();
                    //             var sucursalBanco = sucursalBancoVersat.Where (s => s.Idsucursal == item.Idsucursal);
                    //             var clasifBanco = clasifBancoVersat.Where (c => c.Idclasifbanco == sucursalBanco.Idclasifbanco);

                    //             var nuevaCtaBancoEntidad = new ContratacionWebApi.Models.CuentaBancaria {
                    //                 NumeroCuenta = item.NumeroCta,
                    //                 NumeroSucursal = sucursalBanco.Numero,
                    //                 NombreSucursal = clasifBanco.Nombre,
                    //                 Moneda = Moneda.CUP,
                    //                 EntidadId = entidad.IdEntidad
                    //             };
                    //         }
                    //     }
                    // }

                    _contratacionContext.SaveChanges ();

                    _enlaceContext.Add (new ImportadorDatos.Models.EnlaceVersat.Entidad {
                        EntidadId = nuevaEntidad.Id,
                            EntidadVersatId = entidad.IdEntidad,
                            Codigo = entidad.Codigo,
                            NIT = entidad.NIT,
                    });
                    _enlaceContext.SaveChanges ();
                }
            }
        }
    }
    class CuentaDto {
        public int Id { get; set; }

        public string Numero { get; set; }

        public string Descripcion { get; set; }

        public int Naturaleza { get; set; }
    }
}