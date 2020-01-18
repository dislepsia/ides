using cnrl.Logica;
using cnrl.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace cnrl.Repositories
{
    public static class CuotaData
    {


        public static Cuota consultarCuota(int id)
        {
            Cuota cuota = null;
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["ServicioTeso"]);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["ServicioTesoUsuario"])));

                List<KeyValuePair<string, object>> parametros = new List<KeyValuePair<string, object>>();
                parametros.Add(new KeyValuePair<string, object>("id", id.ToString()));

                var responseGet = httpClient.GetAsync("Cuota/Get" + Constantes.AsQueryString(parametros)).Result;
                var respuesta = responseGet.Content.ReadAsAsync<Cuota>().Result;
                if (responseGet.IsSuccessStatusCode == false)
                {
                    throw new Exception("Error en servicio Tesoreria");
                }
                else
                {
                    cuota = respuesta;
                }
            }

            return cuota;
        }

        public static List<Cuota> consultarCuotas(string IdUsuario = "", int? IdSede = null, int? IdCurso = null, int? IdOferta = null, string estado = "", int? IdPeriodo = null, int? IdConcepto = null, int? IdTipoCurso = null,
            int? NroCuotaDesde = null, int? NroCuotaHasta = null, decimal? ImporteDesde = null, decimal? ImporteHasta = null, decimal? Importe2Desde = null, decimal? Importe2Hasta = null,
            DateTime? FechaVtoDesde = null, DateTime? FechaVtoHasta = null, DateTime? FechaVto2Desde = null, DateTime? FechaVto2Hasta = null, DateTime? FechaPagoDesde = null, DateTime? FechaPagoHasta = null, DateTime? FechaBajaDesde = null, DateTime? FechaBajaHasta = null,
            string NroFactura = "")
        {
            var db = new socioculturalesEntities();
            var cuotas = new List<Cuota>();

            //if ((IdUsuario.HasValue && IdUsuario.Value != Constantes.ERROR) || (IdSede.HasValue && IdSede.Value != Constantes.ERROR) || (IdCurso.HasValue && IdCurso.Value != Constantes.ERROR) || (IdOferta.HasValue && IdOferta.Value != Constantes.ERROR) || (IdPeriodo.HasValue && IdPeriodo.Value != Constantes.ERROR) || (!string.IsNullOrEmpty(estado) && estado != Constantes.ERROR.ToString()))
            //{
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["ServicioTesoUsuario"])));

                var curso = db.curso.Find(IdCurso);
                var oferta = db.oferta.Find(IdOferta);
                httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["ServicioTeso"]);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["ServicioTesoUsuario"])));

                List<KeyValuePair<string, object>> parametros = new List<KeyValuePair<string, object>>();
                parametros.Add(new KeyValuePair<string, object>("origen", ConfigurationManager.AppSettings["OrigenTeso"]));
                if (!string.IsNullOrEmpty(IdUsuario))
                {
                    var user = db.AspNetUsers.Find(IdUsuario);
                    if (user != null)
                        parametros.Add(new KeyValuePair<string, object>("Dni", user.NroDocumento.ToString()));
                }
                if (curso != null)
                    //
                    //
                    parametros.Add(new KeyValuePair<string, object>("NroCurso", curso.codigo.ToString()));
                //
                //
                if (oferta != null)
                    parametros.Add(new KeyValuePair<string, object>("NroComision", oferta.codigo.ToString()));
                if (!string.IsNullOrEmpty(estado) && estado != Constantes.ERROR.ToString())
                    parametros.Add(new KeyValuePair<string, object>("Estado", ((char)int.Parse(estado)).ToString()));
                //if (NroCuotaDesde.HasValue)
                //{
                //    parametros.Add(new KeyValuePair<string, object>("NroCuotaDesde", NroCuotaDesde.Value.ToString()));
                //}
                if (NroCuotaDesde.HasValue) { parametros.Add(new KeyValuePair<string, object>("NroCuotaDesde", NroCuotaDesde.Value.ToString())); }
                if (NroCuotaHasta.HasValue) { parametros.Add(new KeyValuePair<string, object>("NroCuotaHasta", NroCuotaHasta.Value.ToString())); }
                if (ImporteDesde.HasValue) { parametros.Add(new KeyValuePair<string, object>("ImporteDesde", ImporteDesde.Value.ToString())); }
                if (ImporteHasta.HasValue) { parametros.Add(new KeyValuePair<string, object>("ImporteHasta", ImporteHasta.Value.ToString())); }
                if (Importe2Desde.HasValue) { parametros.Add(new KeyValuePair<string, object>("Importe2Desde", Importe2Desde.Value.ToString())); }
                if (Importe2Hasta.HasValue) { parametros.Add(new KeyValuePair<string, object>("Importe2Hasta", Importe2Hasta.Value.ToString())); }
                if (FechaVtoDesde.HasValue) { parametros.Add(new KeyValuePair<string, object>("FechaVtoDesde", FechaVtoDesde.Value.ToString("MM/dd/yy"))); }
                if (FechaVtoHasta.HasValue) { parametros.Add(new KeyValuePair<string, object>("FechaVtoHasta", FechaVtoHasta.Value.ToString("MM/dd/yy"))); }
                if (FechaVto2Desde.HasValue) { parametros.Add(new KeyValuePair<string, object>("FechaVto2Desde", FechaVto2Desde.Value.ToString("MM/dd/yy"))); }
                if (FechaVto2Hasta.HasValue) { parametros.Add(new KeyValuePair<string, object>("FechaVto2Hasta", FechaVto2Hasta.Value.ToString("MM/dd/yy"))); }
                if (FechaPagoDesde.HasValue) { parametros.Add(new KeyValuePair<string, object>("FechaPagoDesde", FechaPagoDesde.Value.ToString("MM/dd/yy"))); }
                if (FechaPagoHasta.HasValue) { parametros.Add(new KeyValuePair<string, object>("FechaPagoHasta", FechaPagoHasta.Value.ToString("MM/dd/yy"))); }
                if (FechaBajaDesde.HasValue) { parametros.Add(new KeyValuePair<string, object>("FechaBajaDesde", FechaBajaDesde.Value.ToString("MM/dd/yy"))); }
                if (FechaBajaHasta.HasValue) { parametros.Add(new KeyValuePair<string, object>("FechaBajaHasta", FechaBajaHasta.Value.ToString("MM/dd/yy"))); }

                if (!string.IsNullOrEmpty(NroFactura))
                    parametros.Add(new KeyValuePair<string, object>("NroFactura", (NroFactura)));

                //int?          NroCuotaDesde               = null
                // int?         NroCuotaHasta               = null
                // decimal?     ImporteDesde                =               null
                // decimal?     ImporteHasta                =               null
                // decimal?     Importe2Desde               = null
                // decimal?     Importe2Hasta               = null
                // DateTime?    FechaVtoDesde               = null
                // DateTime?    FechaVtoHasta               = null
                // DateTime?    FechaVto2Desde               = null
                // DateTime?    FechaVto2Hasta               = null
                // DateTime?    FechaPagoDesde               = null
                // DateTime?    FechaPagoHasta               = null
                // DateTime?    FechaBajaDesde               = null
                // DateTime?    FechaBajaHasta               = null

                IQueryable<oferta> ofertas = null;// db.periodolectivo.Find(IdPeriodo.Value).oferta.Select(x => x.codigo);

                if (IdPeriodo.HasValue && IdPeriodo.Value != Constantes.ERROR)
                {
                    if (ofertas != null)
                        ofertas = ofertas.Where(x => x.periodoLectivo == IdPeriodo.Value);//.Select(x => x.codigo);
                    else
                        ofertas = db.periodolectivo.Find(IdPeriodo.Value).oferta.AsQueryable();//.Select(x => x.codigo);

                }
                if (IdSede.HasValue && IdSede.Value != Constantes.ERROR)
                {
                    if (ofertas != null)
                        ofertas = ofertas.Where(x => x.sede == IdSede.Value);//.Select(x => x.codigo);
                    else
                        ofertas = db.oferta.Where(x => x.sede == IdSede.Value);//.Select(x => x.codigo);
                }
                if (IdTipoCurso.HasValue && IdTipoCurso.Value != Constantes.ERROR)
                {
                    if (ofertas != null)
                        ofertas = ofertas.Where(x => x.curso1.tipoCurso == IdTipoCurso.Value);//.Select(x => x.codigo);
                    else
                        ofertas = db.oferta.Where(x => x.curso1.tipoCurso == IdTipoCurso.Value);//.Select(x => x.codigo);
                }
                if (IdConcepto.HasValue && IdConcepto.Value != Constantes.ERROR)
                {
                    if (ofertas != null)
                        ofertas = ofertas.Where(x => x.curso1.concepto == IdConcepto.Value);//.Select(x => x.codigo);
                    else
                        ofertas = db.oferta.Where(x => x.curso1.concepto == IdConcepto.Value);//.Select(x => x.codigo);
                }

                if (ofertas != null)
                {
                    //    var ofertas = db.periodolectivo.Find(IdPeriodo.Value).oferta.Select(x => x.codigo);
                    var of = ofertas.ToList();
                    string arrayOfertas = "";
                    if (of.Count > 0)
                    {
                        int i = 0;

                        foreach (var ofer in ofertas)
                        {
                            i++;
                            arrayOfertas += ofer.codigo;
                            if (of.Last() != ofer)
                            {
                                arrayOfertas += "|";
                            }
                            //if (i < 100)
                            //{
                                
                            //    //if (of.Last() != ofer && i != 99)
                            //    //{
                            //    //    arrayOfertas += "|";
                            //    //}
                            //}
                        }
                    }
                    else
                    {
                        arrayOfertas = "-1";
                    }
                    parametros.Add(new KeyValuePair<string, object>("listadoComisiones", arrayOfertas));
                }

                if (parametros.Count > 1)
                {
                    var responseGet = httpClient.GetAsync("Cuota/ObtenerCuotas" + Constantes.AsQueryString(parametros)).Result;
                    var respuesta = responseGet.Content.ReadAsAsync<IEnumerable<Cuota>>().Result;
                    if (responseGet.IsSuccessStatusCode == false)
                    {
                        throw new Exception("Error en servicio Tesoreria");
                    }
                    else
                    {
                        //if (IdSede.HasValue && IdSede.Value != Constantes.ERROR)
                        //{
                        //    var ofertas = db.oferta.Where(x => x.sede == IdSede.Value);
                        //    foreach (var ofer in ofertas)
                        //    {
                        //        var cuotasOferta = respuesta.Where(x => x.NroComision == ofer.codigo);
                        //        cuotas.AddRange(cuotasOferta);
                        //    }
                        //}
                        //else
                        //{
                        cuotas = respuesta.OrderBy(x => x.Dni).ToList();
                        //}

                        var agrupado = cuotas.GroupBy(x => x.NroCurso);

                        foreach (var grupo in agrupado)
                        {
                            var nroCurso = grupo.First().NroCurso;
                            var cursoCuota = db.curso.Where(x => x.codigo == nroCurso || x.codCurso == nroCurso).FirstOrDefault();
                            if (cursoCuota != null)
                            {
                                foreach (var cuota in grupo)
                                {
                                    cuota.Curso = cursoCuota.descripcion;
                                    cuota.Concepto = cursoCuota.precio.descripcion;
                                }
                            }
                        }

                        var cantidadDni = cuotas.GroupBy(x => x.Dni);//.Count();



                        var alumnos = db.AspNetUsers;
                        AspNetUsers alumno = null;
                        Int64 dniAnterior = 0;
                        string nombreAnterior = "";
                        string apellidoAnterior = "";
                        foreach (var cuota in cuotas)
                        {
                            if (dniAnterior != cuota.Dni)
                            {
                                alumno = alumnos.Where(x => x.NroDocumento == cuota.Dni).FirstOrDefault();
                                if (alumno != null)
                                {
                                    dniAnterior = alumno.NroDocumento.Value;
                                    nombreAnterior = alumno.Nombre;
                                    apellidoAnterior = alumno.Apellido;
                                }
                                else
                                {
                                    dniAnterior = 0;
                                    nombreAnterior = "";
                                    apellidoAnterior = "";
                                }
                            }
                            cuota.Nombre = nombreAnterior;
                            cuota.Apellido = apellidoAnterior;

                        }

                        //foreach (var cuota in cuotas)
                        //{
                        //    var cursoCuota = db.curso.Where(x => x.codigo == cuota.NroCurso).FirstOrDefault();
                        //    if (cursoCuota != null)
                        //    {
                        //        cuota.Curso = cursoCuota.descripcion;
                        //        cuota.Concepto = cursoCuota.precio.descripcion;
                        //    }
                        //}
                    }
                }
            }
            return cuotas;
        }

        public static List<Cuota> consultarCuotas(string codigosCuota = ""
            )
        {
            var db = new socioculturalesEntities();
            var cuotas = new List<Cuota>();
            //if ((IdUsuario.HasValue && IdUsuario.Value != Constantes.ERROR) || (IdSede.HasValue && IdSede.Value != Constantes.ERROR) || (IdCurso.HasValue && IdCurso.Value != Constantes.ERROR) || (IdOferta.HasValue && IdOferta.Value != Constantes.ERROR) || (IdPeriodo.HasValue && IdPeriodo.Value != Constantes.ERROR) || (!string.IsNullOrEmpty(estado) && estado != Constantes.ERROR.ToString()))
            //{
            if (codigosCuota.Count() > 0)

                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["ServicioTeso"]);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["ServicioTesoUsuario"])));

                    List<KeyValuePair<string, object>> parametros = new List<KeyValuePair<string, object>>();
                    parametros.Add(new KeyValuePair<string, object>("origen", ConfigurationManager.AppSettings["OrigenTeso"]));

                    parametros.Add(new KeyValuePair<string, object>("listadoCuotas", codigosCuota));


                    if (parametros.Count > 1)
                    {
                        var responseGet = httpClient.GetAsync("Cuota/ObtenerCuotas" + Constantes.AsQueryString(parametros)).Result;
                        var respuesta = responseGet.Content.ReadAsAsync<IEnumerable<Cuota>>().Result;
                        if (responseGet.IsSuccessStatusCode == false)
                        {
                            throw new Exception("Error en servicio Tesoreria");
                        }
                        else
                        {
                            //if (IdSede.HasValue && IdSede.Value != Constantes.ERROR)
                            //{
                            //    var ofertas = db.oferta.Where(x => x.sede == IdSede.Value);
                            //    foreach (var ofer in ofertas)
                            //    {
                            //        var cuotasOferta = respuesta.Where(x => x.NroComision == ofer.codigo);
                            //        cuotas.AddRange(cuotasOferta);
                            //    }
                            //}
                            //else
                            //{
                            cuotas = respuesta.ToList();
                            //}

                            foreach (var cuota in cuotas)
                            {
                                var cursoCuota = db.curso.Where(x => x.codCurso == cuota.NroCurso).FirstOrDefault();
                                if (cursoCuota != null)
                                {
                                    cuota.Curso = cursoCuota.descripcion;
                                    if (cursoCuota.precio != null)
                                        cuota.Concepto = cursoCuota.precio.descripcion;
                                }
                            }
                        }
                    }
                }
            return cuotas;
        }
        //(long id, string origen = "", DateTime? fecha = null, bool? SegundoVencimiento = null)
        public static bool TieneDeudaAlumnoVencida(long dni)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["ServicioTeso"]);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["ServicioTesoUsuario"])));

                List<KeyValuePair<string, object>> parametros = new List<KeyValuePair<string, object>>();
                parametros.Add(new KeyValuePair<string, object>("id", dni));
                parametros.Add(new KeyValuePair<string, object>("origen", ConfigurationManager.AppSettings["OrigenTeso"]));

                try
                {
                    var responseGet = httpClient.GetAsync("Alumno/TieneDeudaAlumnoVencida" + Constantes.AsQueryString(parametros)).Result;
                    var respuesta = responseGet.Content.ReadAsAsync<bool>();
                    if (responseGet.IsSuccessStatusCode == false)
                    {
                        return false;
                        //throw new Exception("Error en servicio Tesoreria");
                    }
                    else
                    {
                        return respuesta.Result;
                    }

                }
                catch (Exception ex)
                {
                    return false;
                }


            }

        }


        public static async Task<bool> generarCuota(Cuota cuota)
        {
            cuota.Usuario = HttpContext.Current.User.Identity.Name;
            //var client = new HttpClient();
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            //        Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["ServicioTesoUsuario"])));
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ServicioTeso"]);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["ServicioTesoUsuario"])));

                var content = new StringContent(JsonConvert.SerializeObject(cuota));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(ConfigurationManager.AppSettings["ServicioTeso"] + "Cuota/GenerarCuota", content);
                if (response.IsSuccessStatusCode == false)//error!
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception(error);
                }
                return true;
            }
          
        }

        public static async Task<bool> actualizarCuota(Cuota cuota)
        {
            cuota.Usuario = HttpContext.Current.User.Identity.Name;
            var client = new HttpClient();
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            //        Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["ServicioTesoUsuario"])));

            var content = new StringContent(JsonConvert.SerializeObject(cuota));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(ConfigurationManager.AppSettings["ServicioTeso"] + "Cuota/UpdateCuota", content);
            if (response.IsSuccessStatusCode == false)//error!
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
            return true;
        }

        public static void bajaCuota(Cuota cuota)
        {
            using (var httpClient = new HttpClient())
            {
                cuota.Usuario = HttpContext.Current.User.Identity.Name;
                httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["ServicioTeso"]);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["ServicioTesoUsuario"])));

                List<KeyValuePair<string, object>> parametros = new List<KeyValuePair<string, object>>();
                parametros.Add(new KeyValuePair<string, object>("id", cuota.Id));
                parametros.Add(new KeyValuePair<string, object>("motivo", cuota.Motivo));

                var responseGet = httpClient.GetAsync("Cuota/BajaCuota" + Constantes.AsQueryString(parametros)).Result;
                //var respuesta = responseGet.Content.ReadAsAsync<Cuota>().Result;
                if (responseGet.IsSuccessStatusCode == false)
                {
                    throw new Exception("Error en servicio Tesoreria");
                }
            }

        }

        public static void DesvincularPagoCuota(Cuota cuota)
        {
            using (var httpClient = new HttpClient())
            {
                cuota.Usuario = HttpContext.Current.User.Identity.Name;
                httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["ServicioTeso"]);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["ServicioTesoUsuario"])));

                List<KeyValuePair<string, object>> parametros = new List<KeyValuePair<string, object>>();
                parametros.Add(new KeyValuePair<string, object>("id", cuota.Id.ToString()));
                parametros.Add(new KeyValuePair<string, object>("motivo", cuota.Motivo));

                var responseGet = httpClient.GetAsync("Cuota/DesvincularPagoCuota" + Constantes.AsQueryString(parametros)).Result;
                var respuesta = responseGet.Content.ReadAsAsync<bool>().Result;
                if (responseGet.IsSuccessStatusCode == false)
                {
                    throw new Exception("Error en servicio Tesoreria");
                }
                //else
                //{
                //    if(respuesta == false)

                //}
            }
        }

        public static void ImputarPagoCuota(Cuota cuota)
        {
            using (var httpClient = new HttpClient())
            {
                cuota.Usuario = HttpContext.Current.User.Identity.Name;
                httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["ServicioTeso"]);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["ServicioTesoUsuario"])));

                List<KeyValuePair<string, object>> parametros = new List<KeyValuePair<string, object>>();
                parametros.Add(new KeyValuePair<string, object>("id", cuota.Id.ToString()));
                parametros.Add(new KeyValuePair<string, object>("motivo", cuota.Motivo));

                var responseGet = httpClient.GetAsync("Cuota/ImputarPagoCuota" + Constantes.AsQueryString(parametros)).Result;
                var respuesta = responseGet.Content.ReadAsAsync<bool>().Result;
                if (responseGet.IsSuccessStatusCode == false)
                {
                    throw new Exception("Error en servicio Tesoreria");
                }
                //else
                //{
                //    if(respuesta == false)

                //}
            }
        }


        public static async Task<bool> BajaCuotaPorCursadaRestaurar(int IdCursa)
        {
            using (var httpClient = new HttpClient())
            {

                httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["ServicioTeso"]);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["ServicioTesoUsuario"])));

                List<KeyValuePair<string, object>> parametros = new List<KeyValuePair<string, object>>();
                parametros.Add(new KeyValuePair<string, object>("origen", ConfigurationManager.AppSettings["OrigenTeso"]));
                parametros.Add(new KeyValuePair<string, object>("id", IdCursa.ToString()));
                parametros.Add(new KeyValuePair<string, object>("usuario", HttpContext.Current.User.Identity.Name));

                var responseGet = await httpClient.GetAsync("Cuota/BajaCuotaPorCursadaRestaurar" + Constantes.AsQueryString(parametros));
                var respuesta = await responseGet.Content.ReadAsAsync<bool>();
                if (responseGet.IsSuccessStatusCode == false)
                {
                    throw new Exception("Error en servicio Tesoreria");
                }
                else if (respuesta == false)
                {
                    throw new Exception("El servicio respondio false");
                }
            }
            return true;
        }

        public static async Task<bool> BajaCuotaPorCursada(int IdCursa)
        {
            using (var httpClient = new HttpClient())
            {

                httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["ServicioTeso"]);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["ServicioTesoUsuario"])));

                List<KeyValuePair<string, object>> parametros = new List<KeyValuePair<string, object>>();
                parametros.Add(new KeyValuePair<string, object>("origen", ConfigurationManager.AppSettings["OrigenTeso"]));
                parametros.Add(new KeyValuePair<string, object>("id", IdCursa.ToString()));
                parametros.Add(new KeyValuePair<string, object>("usuario", HttpContext.Current.User.Identity.Name));

                var responseGet = await httpClient.GetAsync("Cuota/BajaCuotaPorCursada" + Constantes.AsQueryString(parametros));
                var respuesta = await responseGet.Content.ReadAsAsync<bool>();
                if (responseGet.IsSuccessStatusCode == false)
                {
                    throw new Exception("Error en servicio Tesoreria");
                }
                else if (respuesta == false)
                {
                    throw new Exception("El servicio respondio false");
                }
            }
            return true;
        }

        #region FuncionesDeuda
        public static decimal ObtenerImporte2doVencimiento(cursa cursada, Cuota cuota)
        {
            decimal importe2vencimiento = 0;
            if (cursada.oferta1.recargoSegundoVencimiento.HasValue)
                importe2vencimiento = cuota.Importe + (cuota.Importe * (decimal)cursada.oferta1.recargoSegundoVencimiento.Value / 100);
            else
            {
                int recargoDefault = int.Parse(ConfigurationManager.AppSettings["RecargoSegundoVencimientoDefault"].ToString());
                importe2vencimiento = cuota.Importe + (cuota.Importe * recargoDefault / 100);
            }

            return Math.Round(importe2vencimiento, 2);
        }

        public static double ObtenerPrecioBase(cursa cursada)
        {
            double precio = 0;
            if (cursada.AspNetUsers.TipoAlumno == (byte)TiposAlumno.Alumno)
            {
                if (cursada.oferta1.esBandaNegativa == true && cursada.oferta1.descuentoBandaNegativa.HasValue)
                {
                    precio = cursada.oferta1.precioAlumno - cursada.oferta1.descuentoBandaNegativa.Value;
                }
                else
                {
                    precio = cursada.oferta1.precioAlumno;
                }
            }
            //Banda Negativa No Alumno
            else if (cursada.AspNetUsers.TipoAlumno == (byte)TiposAlumno.NoAlumno)
            {
                if (cursada.oferta1.esBandaNegativa == true && cursada.oferta1.descuentoBandaNegativaNoAlumno.HasValue)
                {
                    precio = cursada.oferta1.precioNoAlumno - cursada.oferta1.descuentoBandaNegativaNoAlumno.Value;
                }
                else
                {
                    precio = cursada.oferta1.precioNoAlumno;
                }
            }
            //
            else if (cursada.AspNetUsers.TipoAlumno == (byte)TiposAlumno.Empleado)
            {
                if (cursada.oferta1.esBandaNegativa == true && cursada.oferta1.descuentoBandaNegativa.HasValue)
                {
                    precio = cursada.oferta1.precioEmpleado - cursada.oferta1.descuentoBandaNegativa.Value;
                }
                else
                {
                    precio = cursada.oferta1.precioEmpleado;
                }
            }
            else if (cursada.AspNetUsers.TipoAlumno == (byte)TiposAlumno.Mayor60)
            {
                if (cursada.oferta1.esBandaNegativa == true && cursada.oferta1.descuentoBandaNegativa.HasValue)
                {
                    precio = cursada.oferta1.precioMayorDe60 - cursada.oferta1.descuentoBandaNegativa.Value;
                }
                else
                {
                    precio = cursada.oferta1.precioMayorDe60;
                }
            }
            else if (cursada.AspNetUsers.TipoAlumno == (byte)TiposAlumno.Graduado)
            {
                if (cursada.oferta1.esBandaNegativa == true && cursada.oferta1.descuentoBandaNegativa.HasValue)
                {
                    precio = cursada.oferta1.precioGraduado - cursada.oferta1.descuentoBandaNegativa.Value;
                }
                else
                {
                    precio = cursada.oferta1.precioGraduado;
                }
            }
            else
            {
                precio = cursada.oferta1.precioNoAlumno;
            }
            return precio;
        }
        public static double ObtenerPrecioUnaCuota(double precioBase, cursa cursada)
        {
            double precio1Cuota = 0;
            if (cursada.oferta1.descuentoUnaCuota.HasValue && cursada.oferta1.descuentoUnaCuota.Value > 0)
            {
                precio1Cuota = precioBase * cursada.oferta1.descuentoUnaCuota.Value / 100;
            }
            else
            {
                precio1Cuota = precioBase;
            }
            return Math.Round(precio1Cuota, 2);

        }
        public static double ObtenerPrecioUnaCuota(cursa cursada)
        {
            double precio1Cuota = 0;
            if (cursada.oferta1.descuentoUnaCuota.HasValue && cursada.oferta1.descuentoUnaCuota.Value > 0)
            {
                precio1Cuota = ObtenerPrecioBase(cursada) - (ObtenerPrecioBase(cursada) * cursada.oferta1.descuentoUnaCuota.Value / 100);
            }
            else
            {
                precio1Cuota = ObtenerPrecioBase(cursada);
            }
            return Math.Round(precio1Cuota, 2);
        }
        public static double ObtenerPrecioCuotas(double precioBase, cursa cursada)
        {
            double precioCuota = 0;
            if (cursada.oferta1.cantidadCuotas.HasValue && cursada.oferta1.cantidadCuotas.Value > 1)
            {
                precioCuota = precioBase / cursada.oferta1.cantidadCuotas.Value;
            }
            else
            {
                precioCuota = ObtenerPrecioUnaCuota(precioBase, cursada);
            }
            return Math.Round(precioCuota, 2);
        }
        public static double ObtenerPrecioCuotas(cursa cursada)
        {
            double precioCuota = 0;
            if (cursada.oferta1.cantidadCuotas.HasValue && cursada.oferta1.cantidadCuotas.Value > 1)
            {
                precioCuota = ObtenerPrecioBase(cursada) / cursada.oferta1.cantidadCuotas.Value;
            }
            else
            {
                precioCuota = ObtenerPrecioUnaCuota(ObtenerPrecioBase(cursada), cursada);
            }
            return Math.Round(precioCuota, 2);
        }
        #endregion


        public static async Task<decimal> cambioComisionCuota(cursa original, cursa nueva)
        {
            var precioBaseOriginal = ObtenerPrecioBase(original);
            var precioBaseNueva = ObtenerPrecioBase(nueva);
            decimal devolucion = 0;
            if (original.estado.Value == (int)EstadosCursada.FormaPago || original.estado.Value == (int)EstadosCursada.PlanGenerado || original.estado.Value == (int)EstadosCursada.Confirmado)
            {
                if (precioBaseNueva != precioBaseOriginal && original.cantidadCuotas.HasValue)
                {
                    var cuotas = consultarCuotas(original.alumno, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, original.codigo.ToString());

                    if (cuotas != null && cuotas.Count > 0)
                    {
                        double diferencia = precioBaseNueva - precioBaseOriginal;
                        var cuotasImpagas = cuotas.Where(x => x.Estado == "0");
                        var cuotasPagas = cuotas.Where(x => x.Estado == "P");
                        int cantidadCuotasImpagas = cuotasImpagas.Count();
                        int cantidadCuotasPagas = cuotasPagas.Count();

                        if (diferencia > 0)  //LA NUEVA COMISION ES MAS CARA QUE LA ANTERIOR
                        {
                            if (cantidadCuotasImpagas > 0) //QUEDAN CUOTAS PARA INCREMENTAR EL VALOR
                            {
                                double diferenciaPorCuota = diferencia / cantidadCuotasImpagas; //INCREMENTO EL VALOR REPARTIDO ENTRE LAS CUOTAS RESTANTES

                                foreach (var cuota in cuotasImpagas)
                                {
                                    if (cantidadCuotasImpagas.Equals(1))
                                    {
                                        cuota.Importe = (decimal)precioBaseNueva;//SI ES SOLO UNA CUOTA PISO EL VALORA
                                    }
                                    else
                                    {
                                        cuota.Importe += (decimal)diferenciaPorCuota;//SI SON VARIAS LO SUMO
                                    }

                                    cuota.Importe2 = ObtenerImporte2doVencimiento(nueva, cuota);
                                    cuota.Motivo = "Ajuste por cambio de comision. Diferencia por cuota para con respecto al nuevo curso: " + diferenciaPorCuota.ToString("C2");
                                    cuota.NroComision = nueva.oferta;
                                    cuota.NroCurso = nueva.oferta1.curso;

                                    await actualizarCuota(cuota);
                                }
                            }
                            else //NO QUEDAN MAS CUOTAS PARA INCREMENTAR VALOR => GENERO UNA CUOTA MAS POR LA DIFERENCIA
                            {
                                var cuota = cuotas.First();
                                cuota.Importe = (decimal)diferencia;
                                cuota.Importe2 = ObtenerImporte2doVencimiento(nueva, cuota);

                                cuota.NroCuota = original.cantidadCuotas.Value + 1;
                                cuota.TotalCuota = original.cantidadCuotas.Value + 1;
                                cuota.Estado = "0";

                                int diasNuevaCuota = int.Parse(ConfigurationManager.AppSettings["DiasNuevaCuotaPorCambioDeComision"].ToString());
                                cuota.fechavto = DateTime.Now.AddDays(diasNuevaCuota);

                                int dias2doVto = (original.oferta1.diasSegundoVencimiento.HasValue) ? original.oferta1.diasSegundoVencimiento.Value : int.Parse(ConfigurationManager.AppSettings["DiasSegundoVencimientoDefault"].ToString());
                                cuota.fechavto2 = cuota.fechavto.Value.AddDays(dias2doVto);

                                cuota.Motivo = "Nueva cuota por cambio de comision. Diferencia por el nuevo curso: " + diferencia.ToString("C2");
                                cuota.NroFactura = nueva.codigo + "_" + cuota.NroCuota;

                                await generarCuota(cuota);
                            }
                        }
                        else //LA NUEVA COMISION ES MAS BARATA QUE LA ANTERIOR
                        {
                            decimal difCuota = 0;
                            bool marca = true;

                            if (cantidadCuotasPagas > 0)
                            {
                                if (cantidadCuotasImpagas > 0)
                                {
                                    var sumPago = cuotasPagas.Sum(x => x.Importe);
                                    var sumImpago = cuotasImpagas.Sum(x => x.Importe);

                                    difCuota = sumPago;//(decimal)(precioBaseOriginal / cuotas.Count);

                                    foreach (var cuota in cuotasImpagas)
                                    {
                                        difCuota -= (decimal)(precioBaseNueva / cuotas.Count);

                                        if (difCuota < 0 && Math.Abs(difCuota) > ((decimal)(precioBaseNueva / cuotas.Count)))
                                            difCuota = -(decimal)(precioBaseNueva / cuotas.Count);

                                        if (difCuota >= 0) //ANULO CUOTAS MIENTRAS SIGA TENIENDO SALDO A FAVOR
                                        {
                                            cuota.Importe = 0;
                                            cuota.Importe2 = 0;
                                            cuota.Estado = "B";
                                            cuota.Motivo = "Baja por cambio de comision. ";
                                        }
                                        else //SI QUEDO UN RESTO ACTUALIZO 1 CUOTA CON ESE RESTO
                                        {
                                            cuota.Importe = -difCuota;
                                            cuota.Importe2 = ObtenerImporte2doVencimiento(nueva, cuota);
                                            cuota.Motivo = "Ajuste por cambio de comision. ";
                                        }

                                        cuota.Motivo += "Curso Anterior: " + precioBaseOriginal.ToString("C2") + " - Nuevo Curso: " + precioBaseNueva.ToString("C2") + " - Diferencia por cuota: " + difCuota.ToString("C2");
                                        cuota.NroComision = nueva.oferta;
                                        cuota.NroCurso = nueva.oferta1.curso;

                                        await actualizarCuota(cuota);
                                    }

                                    //SI ABONE LAS SUFICIENTES CUOTAS DEL VIEJO CURSO PARA CUBRIR LAS RESTANTES DEL NUEVO DEBO 
                                    //SABER SI DEVOLVER UN REINTEGRO O BIEN GENERAR UNA NUEVA CUOTA POR LA DIFERENCIA ENTRE LO YA PAGADO Y EL NUEVO CURSO
                                    if ((double)sumPago > precioBaseNueva)
                                    {
                                        diferencia = (double)difCuota - precioBaseNueva;
                                    }
                                    else
                                    {
                                        if (difCuota > 0)
                                        {
                                            diferencia = precioBaseNueva - (double)sumPago;
                                            marca = false;
                                            var cuota = cuotas.First();
                                            cuota.Importe = (decimal)diferencia;
                                            cuota.Importe2 = ObtenerImporte2doVencimiento(nueva, cuota);

                                            cuota.NroCuota = original.cantidadCuotas.Value + 1;
                                            cuota.TotalCuota = original.cantidadCuotas.Value + 1;
                                            cuota.Estado = "0";

                                            int diasNuevaCuota = int.Parse(ConfigurationManager.AppSettings["DiasNuevaCuotaPorCambioDeComision"].ToString());
                                            cuota.fechavto = DateTime.Now.AddDays(diasNuevaCuota);

                                            int dias2doVto = (original.oferta1.diasSegundoVencimiento.HasValue) ? original.oferta1.diasSegundoVencimiento.Value : int.Parse(ConfigurationManager.AppSettings["DiasSegundoVencimientoDefault"].ToString());
                                            cuota.fechavto2 = cuota.fechavto.Value.AddDays(dias2doVto);

                                            cuota.Motivo = "Nueva cuota por cambio de comision. Diferencia por el nuevo curso: " + diferencia.ToString("C2");
                                            cuota.NroFactura = nueva.codigo + "_" + cuota.NroCuota;

                                            await generarCuota(cuota);
                                        }
                                    }

                                    if (cuotas.Count == 1)
                                    {
                                        if (sumPago < (decimal)precioBaseNueva)
                                        {
                                            marca = false;

                                            var cuota = cuotasPagas.First();
                                            cuota.Importe = Math.Abs(sumPago - (decimal)precioBaseNueva);
                                            cuota.Importe2 = ObtenerImporte2doVencimiento(nueva, cuota);
                                            cuota.Estado = "0";
                                            cuota.Motivo = "Nueva cuota por cambio de comision. Diferencia con el nuevo curso: " + cuota.Importe.ToString("C2");
                                            cuota.NroComision = nueva.oferta;
                                            cuota.NroCurso = nueva.oferta1.curso;
                                            cuota.NroCuota = original.cantidadCuotas.Value + 1;
                                            cuota.TotalCuota = original.cantidadCuotas.Value + 1;

                                            await generarCuota(cuota);
                                        }
                                    }
                                    else
                                    {
                                        if (diferencia > 0 && marca)
                                            marca = true;
                                        else
                                            marca = false;
                                    }
                                }
                                else
                                {
                                    //SI TIENE TODAS LAS CUOTAS PAGAS HABRIA QUE DEVOLVER LA DIFERENCIA ENTRE EL CURSO CARO - CURSO BARATO
                                    diferencia = Math.Abs(diferencia);
                                }

                                if (diferencia > 0 && marca) //GENERO UN REINTEGRO PARA LA PERSONA
                                {
                                    devolucion = (decimal)diferencia;
                                }
                            }
                            else
                            {
                                foreach (var cuota in cuotasImpagas)
                                {
                                    difCuota = cuota.Importe - (decimal)(precioBaseNueva / cantidadCuotasImpagas);

                                    if (difCuota >= 0)
                                    {
                                        cuota.Importe = (decimal)(precioBaseNueva / cantidadCuotasImpagas);
                                        cuota.Importe2 = ObtenerImporte2doVencimiento(nueva, cuota);
                                    }
                                    else //SI QUEDO UN RESTO ACTUALIZO 1 CUOTA CON ESE RESTO
                                    {
                                        cuota.Importe = -difCuota;
                                        cuota.Importe2 = ObtenerImporte2doVencimiento(nueva, cuota);
                                    }

                                    cuota.Motivo = "Ajuste por cambio de comision. Diferencia entre un curso y otro por cuota: " + difCuota.ToString("C2");
                                    cuota.NroComision = nueva.oferta;
                                    cuota.NroCurso = nueva.oferta1.curso;
                                    await actualizarCuota(cuota);
                                }
                            }
                        }
                    }
                }
            }
            //ACTUALIZO NRO COMISION Y NRO CURSO
            var cuotasCambioOferta = consultarCuotas(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, original.codigo.ToString());
            foreach (var cuota in cuotasCambioOferta)
            {
                if (cuota.NroComision != nueva.oferta || cuota.NroCurso != nueva.oferta1.curso)
                {
                    cuota.NroComision = nueva.oferta;
                    cuota.NroCurso = nueva.oferta1.curso;
                    await actualizarCuota(cuota);
                }
            }

            return Math.Round(devolucion, 2);
        }
    }
}