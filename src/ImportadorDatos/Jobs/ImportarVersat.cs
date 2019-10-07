
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ImportadorDatos.Models.Versat;
using Microsoft.EntityFrameworkCore;

namespace ImportadorDatos.Jobs
{
    public class ImportarVersat
    {
        public static void ImportarCuentasAsync()
        {
            var options = new DbContextOptionsBuilder<VersatDbContext>()
                        .UseSqlServer("Server=localhost;Database=concordia;User Id=sa;Password=Admin123*;").Options;
            using (var context = new VersatDbContext(options))
            {
                var formatos = new int[6] { 0, 3, 4, 6, 6, 6 };

                var cuentas = context.Set<ConCuenta>()
                    .Include(c => c.IdaperturaNavigation.IdmascaraNavigation)
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
                        var posicion = cta.IdaperturaNavigation.IdmascaraNavigation.Posicion;
                        var numero = "";
                        var index = 0;
                        var lenght = cta.Clave.Length;
                        for (int i = 1; i <= posicion; i++)
                        {
                            var offset = index;
                            for (int j = index; j < offset + formatos[i]; j++, index++)
                            {
                                if (j >= lenght)
                                {
                                    break;
                                }
                                numero += cta.Clave[j];
                            }
                            numero += "-";
                        }
                        numero = numero.Remove(numero.Length - 1);
                        var descripcion = context.Query<Con_Cuentadescrip>().SingleOrDefault(c => c.Idcuenta == cta.Idcuenta).Descripcion;
                        var naturaleza = context.Query<ConCuentanatur>().SingleOrDefault(c => c.Idcuenta == cta.Idcuenta).Naturaleza;
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
                                    Console.WriteLine($"error con cuenta {numero},  {data}");
                                }
                            }
                        }
                        else
                        {

                        }
                    }
                }
            }
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