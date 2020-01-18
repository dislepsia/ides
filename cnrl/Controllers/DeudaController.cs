using cnrl.Helpers;
using cnrl.Logica;
using cnrl.Models;
using cnrl.Repositories;
using DevExpress.Export;
using DevExpress.Web.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace cnrl.Controllers
{
    public class DeudaController : Controller
    {
        private socioculturalesEntities db = new socioculturalesEntities();

        // GET: Deuda
        [HasPermission("deuda_Index")]
        public ActionResult Index()
        {
            var idUsuario = (Request.Params["usuario"] != null) ? Request.Params["usuario"].ToString() : "";
            if (!string.IsNullOrEmpty(idUsuario))
            {
                var usuario = db.AspNetUsers.Find(idUsuario);
                ViewBag.IdUsuario = idUsuario;
                ViewBag.NombreUsuario = usuario.descripcion;

            }
            if (User.IsInRole("AdministradorEmpresas"))
            {
                PrepararViewBagEmpresa(true);
            }
            else
            {
                if (User.IsInRole("AdministradorGraduado"))
                {
                    PrepararViewBagGraduado(true);
                }
                else
                {
                    PrepararViewBag(true);
                }

            }


            return View();
        }

        private void PrepararViewBagGraduado(bool filtro = false, Cuota cuota = null)
        {
            if (cuota != null)
            {
                ViewBag.IdOferta = cuota.NroComision;
                var oferta = db.oferta.Find(cuota.NroComision);
                ViewBag.Oferta = (oferta != null) ? oferta.descripcion : "(Sin Oferta)";
                ViewBag.IdUsuario = cuota.Dni;
                ViewBag.Usuario = db.AspNetUsers.Where(x => x.NroDocumento == cuota.Dni).First().descripcion;
            }
            if (User.IsInRole("Ventanilla"))
            {
                int sede = ApplicationUser.GetSede();
                ViewBag.sedes = new SelectList(
                        db.sede.Where(x => x.borradoLogico == false && x.codigo == sede).ToList()
                        .ToSelectList(
                            x => x.nombre,
                            x => x.codigo.ToString(),
                            (filtro) ? Strings.MsgTodasSedes : Strings.SeleccionarSede,
                            Constantes.ERROR.ToString(),
                            Constantes.ERROR.ToString()
                        )
                    , "Value", "Text");

            }
            else
            {
                ViewBag.sedes = new SelectList(
                    db.sede.Where(x => x.borradoLogico == false && x.sector.Value == (int)Sectores.Graduados).ToList()
                    .ToSelectList(
                        x => x.nombre,
                        x => x.codigo.ToString(),
                        (filtro) ? Strings.MsgTodasSedes : Strings.SeleccionarSede,
                        Constantes.ERROR.ToString(),
                        Constantes.ERROR.ToString()
                    )
                , "Value", "Text");
            }

            ViewBag.cursos = new SelectList(
                db.curso.Where(x => x.tipocurso1.sector == (int)Sectores.Graduados).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosLosCursos : Strings.SeleccionarCurso,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.periodosLectivos = new SelectList(
                db.periodolectivo.OrderByDescending(x => x.fechaInscripcionHasta).Where(x => x.tipocurso1.sector == (int)Sectores.Graduados).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosLosPeriodos : Strings.SeleccionarPeriodoLectivo,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            //ViewBag.usuarios = new SelectList(
            //    db.AspNetUsers.ToList()
            //    .ToSelectList(
            //        x => x.descripcion,
            //        x => x.NroDocumento.ToString(),
            //        Strings.MsgTodosAlumnos,
            //        Constantes.ERROR.ToString(),
            //        Constantes.ERROR.ToString()
            //    )
            //, "Value", "Text");

            ViewBag.ofertas = new SelectList(
                db.oferta.Where(x => x.curso1.tipocurso1.sector == (int)Sectores.Graduados).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosLasOfertas : Strings.SeleccionarOferta,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            if (filtro)
            {
                IList<SelectListItem> list = Enum.GetValues(typeof(EstadoCuota)).Cast<EstadoCuota>().Select(x => new SelectListItem { Text = x.ToString(), Value = ((int)x).ToString() }).ToList().ToSelectList(
                        x => x.Text,
                        x => x.Value,
                        Strings.MsgTodosEstados,
                        Constantes.ERROR.ToString(),
                        Constantes.ERROR.ToString()
                        );

                ViewBag.estados = new SelectList(list, "Value", "Text");
            }
            else
            {
                IList<SelectListItem> list = Enum.GetValues(typeof(EstadoCuota)).Cast<EstadoCuota>().Select(x => new SelectListItem { Text = x.ToString(), Value = ((int)x).ToString() }).ToList().ToSelectList(
                        x => x.Text,
                        x => x.Value
                        );

                ViewBag.estados = new SelectList(list, "Value", "Text");
            }

            ViewBag.tiposCurso = new SelectList(
                db.tipocurso.Where(x => x.sector == (int)Sectores.Graduados).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosTiposCurso,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.conceptos = new SelectList(
                db.precio.Where(x => x.sector == (int)Sectores.Graduados).ToList()
                .ToSelectList(
                    x => x.concepto + " - " + x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosLosConceptos,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

        }

        private void PrepararViewBagEmpresa(bool filtro = false, Cuota cuota = null)
        {
            if (cuota != null)
            {
                ViewBag.IdOferta = cuota.NroComision;
                var oferta = db.oferta.Find(cuota.NroComision);
                ViewBag.Oferta = (oferta != null) ? oferta.descripcion : "(Sin Oferta)";
                ViewBag.IdUsuario = cuota.Dni;
                ViewBag.Usuario = db.AspNetUsers.Where(x => x.NroDocumento == cuota.Dni).First().descripcion;
            }
            if (User.IsInRole("Ventanilla"))
            {
                int sede = ApplicationUser.GetSede();
                ViewBag.sedes = new SelectList(
                        db.sede.Where(x => x.borradoLogico == false && x.codigo == sede).ToList()
                        .ToSelectList(
                            x => x.nombre,
                            x => x.codigo.ToString(),
                            (filtro) ? Strings.MsgTodasSedes : Strings.SeleccionarSede,
                            Constantes.ERROR.ToString(),
                            Constantes.ERROR.ToString()
                        )
                    , "Value", "Text");

            }
            else
            {
                ViewBag.sedes = new SelectList(
                    db.sede.Where(x => x.borradoLogico == false && x.sector.Value == (int)Sectores.Empresas).ToList()
                    .ToSelectList(
                        x => x.nombre,
                        x => x.codigo.ToString(),
                        (filtro) ? Strings.MsgTodasSedes : Strings.SeleccionarSede,
                        Constantes.ERROR.ToString(),
                        Constantes.ERROR.ToString()
                    )
                , "Value", "Text");
            }

            ViewBag.cursos = new SelectList(
                db.curso.Where(x => x.tipocurso1.sector == (int)Sectores.Empresas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosLosCursos : Strings.SeleccionarCurso,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.periodosLectivos = new SelectList(
                db.periodolectivo.OrderByDescending(x => x.fechaInscripcionHasta).Where(x => /*x.tipocurso1.codigo.Equals(23)*/x.tipocurso1.sector == (int)Sectores.Empresas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosLosPeriodos : Strings.SeleccionarPeriodoLectivo,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            //ViewBag.usuarios = new SelectList(
            //    db.AspNetUsers.ToList()
            //    .ToSelectList(
            //        x => x.descripcion,
            //        x => x.NroDocumento.ToString(),
            //        Strings.MsgTodosAlumnos,
            //        Constantes.ERROR.ToString(),
            //        Constantes.ERROR.ToString()
            //    )
            //, "Value", "Text");

            ViewBag.ofertas = new SelectList(
                db.oferta.Where(x => /*x.curso1.tipocurso1.codigo.Equals(23)*/x.curso1.tipocurso1.sector == (int)Sectores.Empresas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosLasOfertas : Strings.SeleccionarOferta,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            if (filtro)
            {
                IList<SelectListItem> list = Enum.GetValues(typeof(EstadoCuota)).Cast<EstadoCuota>().Select(x => new SelectListItem { Text = x.ToString(), Value = ((int)x).ToString() }).ToList().ToSelectList(
                        x => x.Text,
                        x => x.Value,
                        Strings.MsgTodosEstados,
                        Constantes.ERROR.ToString(),
                        Constantes.ERROR.ToString()
                        );

                ViewBag.estados = new SelectList(list, "Value", "Text");
            }
            else
            {
                IList<SelectListItem> list = Enum.GetValues(typeof(EstadoCuota)).Cast<EstadoCuota>().Select(x => new SelectListItem { Text = x.ToString(), Value = ((int)x).ToString() }).ToList().ToSelectList(
                        x => x.Text,
                        x => x.Value
                        );

                ViewBag.estados = new SelectList(list, "Value", "Text");
            }

            ViewBag.tiposCurso = new SelectList(
                db.tipocurso.Where(x => /*x.codigo.Equals(23)*/x.sector == (int)Sectores.Empresas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosTiposCurso,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.conceptos = new SelectList(
                db.precio.Where(x => x.sector == (int)Sectores.Empresas).ToList()
                .ToSelectList(
                    x => x.concepto + " - " + x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosLosConceptos,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

        }
        private void PrepararViewBag(bool filtro = false, Cuota cuota = null)
        {
            if (cuota != null)
            {
                ViewBag.IdOferta = cuota.NroComision;
                var oferta = db.oferta.Find(cuota.NroComision);
                ViewBag.Oferta = (oferta != null) ? oferta.descripcion : "(Sin Oferta)";
                ViewBag.IdUsuario = cuota.Dni;
                ViewBag.Usuario = db.AspNetUsers.Where(x => x.NroDocumento == cuota.Dni).First().descripcion;
            }
            if (User.IsInRole("Ventanilla"))
            {
                int sede = ApplicationUser.GetSede();
                ViewBag.sedes = new SelectList(
                        db.sede.Where(x => x.borradoLogico == false && x.codigo == sede && x.sector.Value == (int)Sectores.Idiomas).ToList()
                        .ToSelectList(
                            x => x.nombre,
                            x => x.codigo.ToString(),
                            (filtro) ? Strings.MsgTodasSedes : Strings.SeleccionarSede,
                            Constantes.ERROR.ToString(),
                            Constantes.ERROR.ToString()
                        )
                    , "Value", "Text");

            }
            else
            {
                ViewBag.sedes = new SelectList(
                    db.sede.Where(x => x.borradoLogico == false && x.sector.Value == (int)Sectores.Idiomas).ToList()
                    .ToSelectList(
                        x => x.nombre,
                        x => x.codigo.ToString(),
                        (filtro) ? Strings.MsgTodasSedes : Strings.SeleccionarSede,
                        Constantes.ERROR.ToString(),
                        Constantes.ERROR.ToString()
                    )
                , "Value", "Text");
            }

            ViewBag.cursos = new SelectList(
                db.curso.Where(x => x.tipocurso1.sector == (int)Sectores.Idiomas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosLosCursos : Strings.SeleccionarCurso,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.periodosLectivos = new SelectList(
                db.periodolectivo.OrderByDescending(x => x.fechaInscripcionHasta).Where(x => x.tipocurso1.sector == (int)Sectores.Idiomas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosLosPeriodos : Strings.SeleccionarPeriodoLectivo,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            //ViewBag.usuarios = new SelectList(
            //    db.AspNetUsers.ToList()
            //    .ToSelectList(
            //        x => x.descripcion,
            //        x => x.NroDocumento.ToString(),
            //        Strings.MsgTodosAlumnos,
            //        Constantes.ERROR.ToString(),
            //        Constantes.ERROR.ToString()
            //    )
            //, "Value", "Text");

            ViewBag.ofertas = new SelectList(
                db.oferta.Where(x => x.curso1.tipocurso1.sector == (int)Sectores.Idiomas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosLasOfertas : Strings.SeleccionarOferta,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            if (filtro)
            {
                IList<SelectListItem> list = Enum.GetValues(typeof(EstadoCuota)).Cast<EstadoCuota>().Select(x => new SelectListItem { Text = x.ToString(), Value = ((int)x).ToString() }).ToList().ToSelectList(
                        x => x.Text,
                        x => x.Value,
                        Strings.MsgTodosEstados,
                        Constantes.ERROR.ToString(),
                        Constantes.ERROR.ToString()
                        );

                ViewBag.estados = new SelectList(list, "Value", "Text");
            }
            else
            {
                IList<SelectListItem> list = Enum.GetValues(typeof(EstadoCuota)).Cast<EstadoCuota>().Select(x => new SelectListItem { Text = x.ToString(), Value = ((int)x).ToString() }).ToList().ToSelectList(
                        x => x.Text,
                        x => x.Value
                        );

                ViewBag.estados = new SelectList(list, "Value", "Text");
            }

            ViewBag.tiposCurso = new SelectList(
                db.tipocurso.Where(x => x.sector == (int)Sectores.Idiomas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosTiposCurso,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.conceptos = new SelectList(
                db.precio.Where(x => x.sector == (int)Sectores.Idiomas).ToList()
                .ToSelectList(
                    x => x.concepto + " - " + x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosLosConceptos,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

        }

        [HasPermission("deuda_Index")]
        public ActionResult PartialGrid(string IdUsuario = "", int? IdSede = null, int? IdCurso = null, int? IdOferta = null, string estado = "", int? IdPeriodo = null, int? IdConcepto = null, int? IdTipoCurso = null, int? NroCuotaDesde = null, int? NroCuotaHasta = null, decimal? ImporteDesde = null, decimal? ImporteHasta = null, decimal? Importe2Desde = null, decimal? Importe2Hasta = null, DateTime? FechaVtoDesde = null, DateTime? FechaVtoHasta = null, DateTime? FechaVto2Desde = null, DateTime? FechaVto2Hasta = null, DateTime? FechaPagoDesde = null, DateTime? FechaPagoHasta = null, DateTime? FechaBajaDesde = null, DateTime? FechaBajaHasta = null
            )
        {

            if (User.IsInRole("Ventanilla") && ApplicationUser.GetSede() != 19)
            {
                IdSede = ApplicationUser.GetSede();
            }

            var cuotas = new List<Cuota>();
            cuotas = CuotaData.consultarCuotas(IdUsuario, IdSede, IdCurso, IdOferta, estado, IdPeriodo, IdConcepto, IdTipoCurso,
                        NroCuotaDesde, NroCuotaHasta, ImporteDesde, ImporteHasta, Importe2Desde, Importe2Hasta,
                        FechaVtoDesde, FechaVtoHasta, FechaVto2Desde, FechaVto2Hasta, FechaPagoDesde, FechaPagoHasta, FechaBajaDesde, FechaBajaHasta
                        );


            string codigosCuotas = "";
            //            ofertas.Select(x => x.codigo).ToList().ForEach(;
            cuotas.Where(x => x.Estado == "0").ToList().ForEach(x => codigosCuotas += "|" + x.Id.ToString());

            Session.Remove("cuotasParaEnviarEmail");
            Session.Add("cuotasParaEnviarEmail", codigosCuotas);

            return PartialView("_Grid", cuotas);
        }

        [HasPermission("cuota_Index")]
        public ActionResult PartialGridAlumno(int? IdUsuario = null)
        {
            var cuotas = new List<Cuota>();

            if (!IdUsuario.HasValue)
            {
                IdUsuario = db.AspNetUsers.Find(ApplicationUser.GetId()).NroDocumento;
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["ServicioTeso"]);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["ServicioTesoUsuario"])));

                List<KeyValuePair<string, object>> parametros = new List<KeyValuePair<string, object>>();
                parametros.Add(new KeyValuePair<string, object>("origen", ConfigurationManager.AppSettings["OrigenTeso"]));
                if (IdUsuario != null && IdUsuario.HasValue && IdUsuario.Value != Constantes.ERROR)
                    parametros.Add(new KeyValuePair<string, object>("Dni", IdUsuario.ToString()));


                var responseGet = httpClient.GetAsync("Cuota/ObtenerCuotas" + Constantes.AsQueryString(parametros)).Result;
                var respuesta = responseGet.Content.ReadAsAsync<IEnumerable<Cuota>>().Result;
                if (responseGet.IsSuccessStatusCode == false)
                {
                    throw new Exception("Error en servicio Tesoreria");
                }
                else
                {
                    cuotas = respuesta.ToList();
                    int dias = int.Parse(ConfigurationManager.AppSettings["Dias1erCuota"].ToString());

                    foreach (var cuota in cuotas)
                    {
                        var cursoCuota = db.curso.Find(cuota.NroCurso);
                        if (cursoCuota != null)
                        {
                            cuota.Curso = cursoCuota.descripcion;
                            cuota.Concepto = cursoCuota.precio.descripcion;
                        }

                        if (cuota.NroCuota == 1 && cuota.Estado == Constantes.ESTADO_CUOTA_IMPAGA && cuota.fechavto > DateTime.Now)
                        {
                            var fecha = DateTime.Now.AddDays(dias);
                            cuota.fechavto = fecha;
                            cuota.fechavto2 = fecha.AddDays(1);
                            cuota.Importe2 = cuota.Importe;
                        }
                    }
                }

                return PartialView("_GridAlumno", cuotas.OrderByDescending(x => x.fechavto));
            }
        }

        [HasPermission("cuota_Index")]
        public ActionResult CuotasAlumno()
        {
            var usuario = db.AspNetUsers.Find(ApplicationUser.GetId());
            ViewBag.IdUsuario = usuario.NroDocumento;
            ViewBag.NombreUsuario = usuario.Apellido + ", " + usuario.Nombre + "(" + usuario.NroDocumento + ")";
            return View();
        }

        #region ABM
        // GET: cursos/Details/5
        [HasPermission("deuda_Index")]
        public ActionResult Details(int? id)
        {
            if (id == null || !id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string callback = Request.Params["callback"];
            if (string.IsNullOrEmpty(callback))
                callback = "AjaxOk";

            ViewBag.modo = "View";
            ViewBag.Callback = callback;
            Cuota cuota = CuotaData.consultarCuota(id.Value);



            if (cuota == null)
            {
                return HttpNotFound();
            }
            PrepararViewBag(false, cuota);
            return View("Cuota", cuota);

        }

        // GET: cursos/Create
        [HasPermission("Deuda_ABM")]
        public ActionResult Create()
        {
            string callback = Request.Params["callback"];
            if (string.IsNullOrEmpty(callback))
                callback = "AjaxOk";

            ViewBag.modo = "Create";
            ViewBag.Callback = callback;

            if (User.IsInRole("AdministradorEmpresas"))
            {
                PrepararViewBagEmpresa();
            }
            else
            {
                if (User.IsInRole("AdministradorGraduado"))
                {
                    PrepararViewBagGraduado();
                }
                else
                {
                    PrepararViewBag();
                }

            }


            return View("Cuota");
        }

        [HttpPost]
        public async Task<ActionResult> Create(Cuota cuota)
        {
            try
            {
                ModelState.Remove("Id");
                if (ModelState.IsValid)
                {

                    var oferta = db.oferta.Find(cuota.NroComision);
                    cuota.NroCurso = oferta.curso;
                    cuota.CodCon = oferta.curso1.concepto;
                    cuota.Motivo = "";
                    cuota.Estado = Constantes.ESTADO_CUOTA_IMPAGA;
                    cuota.Origen = ConfigurationManager.AppSettings["OrigenTeso"];
                    var cursa = db.cursa.Where(x => x.oferta == (cuota.NroComision) && x.AspNetUsers.NroDocumento == (cuota.Dni));
                    if (cursa != null && cursa.Count() > 0)
                    {
                        List<Cuota> cu = CuotaData.consultarCuotas(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, cursa.First().codigo.ToString());

                        List<Cuota> cuo = cu.Where(x => x.NroFactura.Contains("_1_")).ToList().OrderByDescending(x => x.NroFactura).ToList();
                        int sumaCuota = 0;
                        if (cuo.Count() > 0)
                        {
                            string[] split = cuo.First().NroFactura.Split('_');
                            sumaCuota = int.Parse(split[2]);
                        }


                        if (cuota.TotalCuota == 1)
                            cuota.NroFactura = cursa.First().codigo.ToString() + "_" + cuota.NroCuota.ToString() + "_" + (cuota.TotalCuota + sumaCuota).ToString();
                        else
                            cuota.NroFactura = cursa.First().codigo.ToString() + "_" + cuota.NroCuota.ToString();
                    }
                    else
                    {
                        cuota.NroFactura = oferta.codigo.ToString() + cuota.Dni + "_" + cuota.NroCuota.ToString();
                    }

                    await CuotaData.generarCuota(cuota);
                    //var content = new StringContent(JsonConvert.SerializeObject(cuota));
                    //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    //var response = await client.PostAsync(ConfigurationManager.AppSettings["ServicioTeso"] + "Cuota/GenerarCuota", content);
                    //if (response.IsSuccessStatusCode == false)//error!
                    //{
                    //    var error = await response.Content.ReadAsStringAsync();
                    //    throw new Exception(error);
                    //}
                    return Json(new { ok = true });

                    //           return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                ViewBag.ErrorMessage = string.Format(
                    Strings.ErrorIntentarInsertarRegistro,
                    e.Message.Replace(Strings.TheStatementHasBeenTerminated, "")
                );
            }
            ViewBag.modo = "Create";
            ViewBag.Callback = "AjaxOk";
            PrepararViewBag(false, cuota);
            return PartialView("Cuota", cuota);
        }

        // GET: cursos/Edit/5
        [HasPermission("Deuda_ABM")]
        public ActionResult Edit(int? id)
        {
            if (id == null || !id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string callback = Request.Params["callback"];
            if (string.IsNullOrEmpty(callback))
                callback = "AjaxOk";
            //var curso = new curso();
            ViewBag.modo = "Edit";
            ViewBag.Callback = callback;

            Cuota cuota = CuotaData.consultarCuota(id.Value);
            if (cuota == null)
            {
                return HttpNotFound();
            }
            Session.Add("cuota_" + cuota.Id.ToString(), cuota);
            PrepararViewBag(false, cuota);
            return View("Cuota", cuota);
        }



        [HttpPost]
        public async Task<ActionResult> Edit(Cuota cuota)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //LLAMAR WEBSERVICE POST

                    var oferta = db.oferta.Find(cuota.NroComision);
                    if (oferta != null)
                    {
                        cuota.NroCurso = oferta.curso;
                        cuota.CodCon = oferta.curso1.concepto;
                    }
                    var cuotaOriginal = (Cuota)Session["cuota_" + cuota.Id.ToString()];

                    if (oferta == null)
                    {
                        cuota.NroCurso = cuotaOriginal.NroCurso;
                        cuota.CodCon = cuotaOriginal.CodCon;
                    }
                    cuota.Motivo = cuotaOriginal.Motivo;
                    cuota.fechaBaja = cuotaOriginal.fechaBaja;
                    cuota.fechaPago = cuotaOriginal.fechaPago;
                    cuota.NroFactura = cuotaOriginal.NroFactura;
                    cuota.Estado = cuotaOriginal.Estado;
                    cuota.Origen = cuotaOriginal.Origen;

                    await CuotaData.actualizarCuota(cuota);

                    Session.Remove("cuota_" + cuota.Id.ToString());
                    return Json(new { ok = true });
                }
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                ViewBag.ErrorMessage = string.Format(
                    Strings.ErrorIntentarInsertarRegistro,
                    e.Message.Replace(Strings.TheStatementHasBeenTerminated, "")
                );
            }

            ViewBag.modo = "Edit";
            ViewBag.Callback = "AjaxOk";
            PrepararViewBag(false, cuota);
            return View("Cuota", cuota);
        }

        // GET: cursos/Delete/5
        [HasPermission("Deuda_ABM")]
        public ActionResult Delete(int? id)
        {
            if (id == null || !id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string callback = Request.Params["callback"];
            if (string.IsNullOrEmpty(callback))
                callback = "AjaxOk";
            ViewBag.Callback = callback;
            ViewBag.modo = "Delete";

            Cuota cuota = CuotaData.consultarCuota(id.Value);
            if (cuota == null)
            {
                return HttpNotFound();
            }
            PrepararViewBag(false, cuota);

            return View("Cuota", cuota);
        }

        [HttpPost]
        public ActionResult Delete(Cuota cuota)
        {
            try
            {
                cuota.Motivo = "Delete Cuota";
                CuotaData.bajaCuota(cuota);

                return Json(new { ok = true });
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                ViewBag.ErrorMessage = string.Format(Strings.ErrorEliminarRegistroPorIntegridad);
            }
            ViewBag.modo = "Delete";
            string callback = Request.Params["callback"];
            if (string.IsNullOrEmpty(callback))
                callback = "AjaxOk";
            ViewBag.Callback = callback;
            PrepararViewBag(false, cuota);
            return PartialView("Cuota", cuota);
        }

        #endregion

        #region tesoreria

        public async System.Threading.Tasks.Task<ActionResult> EmitirCupon(int? id, int? nroCuota = 1)
        {
            var cursa = db.cursa.Find(id);
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["ServicioTeso"]);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["ServicioTesoUsuario"])));

                List<KeyValuePair<string, object>> parametros = new List<KeyValuePair<string, object>>();
                parametros.Add(new KeyValuePair<string, object>("origen", ConfigurationManager.AppSettings["OrigenTeso"]));
                parametros.Add(new KeyValuePair<string, object>("nroFactura", id + "_" + nroCuota));
                parametros.Add(new KeyValuePair<string, object>("nombre", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cursa.AspNetUsers.descripcion.ToLower())));
                parametros.Add(new KeyValuePair<string, object>("curso", cursa.oferta1.curso1.descripcionCompleja));

                //if (nroCuota == 1)
                //{
                if (cursa.oferta1.curso1.tipocurso1.sector != (int)Sectores.Graduados)
                {
                    int dias = int.Parse(ConfigurationManager.AppSettings["Dias1erCuota"].ToString());
                    parametros.Add(new KeyValuePair<string, object>("vencimientoRelativo", true));
                    parametros.Add(new KeyValuePair<string, object>("diasVencimientoRelativo", dias));
                    parametros.Add(new KeyValuePair<string, object>("tipoCurso", true));
                    //}
                }
                var responseGet = await httpClient.GetAsync("Cuota.pdf" + Constantes.AsQueryString(parametros));
                var cuponPdf = await responseGet.Content.ReadAsByteArrayAsync();

                return File(cuponPdf, "application/pdf");
            }

        }
        public async System.Threading.Tasks.Task<ActionResult> EmitirCuponCuota(int? id)
        {
            if (id != null && id.HasValue)
            {
                var cuota = CuotaData.consultarCuota(id.Value);

                var usuario = db.AspNetUsers.Where(x => x.NroDocumento == cuota.Dni).FirstOrDefault();
                var oferta = db.oferta.Find(cuota.NroComision);
                //cursa cursa = null;
                //int NroCursa = 0;
                //var SNroCursa = cuota.NroFactura.Split('_').First();
                //if (int.TryParse(SNroCursa, out NroCursa))
                //{
                //    cursa = db.cursa.Where(x => x.codigo == NroCursa).FirstOrDefault();
                //}

                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["ServicioTeso"]);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["ServicioTesoUsuario"])));

                    List<KeyValuePair<string, object>> parametros = new List<KeyValuePair<string, object>>();
                    parametros.Add(new KeyValuePair<string, object>("origen", ConfigurationManager.AppSettings["OrigenTeso"]));
                    parametros.Add(new KeyValuePair<string, object>("nroFactura", cuota.NroFactura));
                    if (usuario != null)
                    {
                        parametros.Add(new KeyValuePair<string, object>("nombre", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(usuario.descripcion.ToLower())));
                    }
                    if (oferta != null && oferta.curso1 != null)
                    {
                        parametros.Add(new KeyValuePair<string, object>("curso", oferta.curso1.descripcion));
                    }
                    else
                    {
                        if (cuota.NroCurso > 0)
                        {
                            var curso = db.curso.Where(x => x.codCurso == cuota.NroCurso).FirstOrDefault();

                            if (curso != null)
                            {
                                parametros.Add(new KeyValuePair<string, object>("curso", curso.descripcion));
                            }
                        }
                    }

                    //if (cuota.NroCuota == 1)
                    //{
                    if (oferta != null && oferta.curso1 != null && oferta.curso1.tipocurso1.sector != (int)Sectores.Graduados)
                    {
                        int dias = int.Parse(ConfigurationManager.AppSettings["Dias1erCuota"].ToString());
                        parametros.Add(new KeyValuePair<string, object>("vencimientoRelativo", true));
                        parametros.Add(new KeyValuePair<string, object>("diasVencimientoRelativo", dias));
                        parametros.Add(new KeyValuePair<string, object>("tipoCurso", true));
                    }

                    //}

                    var responseGet = await httpClient.GetAsync("Cuota.pdf" + Constantes.AsQueryString(parametros));
                    var cuponPdf = await responseGet.Content.ReadAsByteArrayAsync();

                    return File(cuponPdf, "application/pdf", string.Format("{0}-{1}.pdf", cuota.Dni, DateTime.Now.ToShortDateString()));
                }

            }
            else
            {
                throw new Exception("Error en llamada a Servicio");
            }

        }

        #endregion

        #region AJAX

        [HasPermission("Deuda_ABM")]
        [HttpPost]
        public ActionResult impaga(int IdCuota)
        {
            Cuota cuota = new Cuota();
            cuota.Motivo = "Desvinculación manual - " + ApplicationUser.GetApellidoYNombre();
            cuota.Id = IdCuota;

            CuotaData.DesvincularPagoCuota(cuota);

            return Json(new { ok = true });
        }

        [HasPermission("Deuda_ABM")]
        public ActionResult baja(int IdCuota)
        {
            Cuota cuota = new Cuota();
            cuota.Motivo = "Baja manual - " + ApplicationUser.GetApellidoYNombre();
            cuota.Id = IdCuota;

            CuotaData.bajaCuota(cuota);

            return Json(new { ok = true });
        }

        [HasPermission("Deuda_ABM")]
        public ActionResult imputar(int IdCuota)
        {
            Cuota cuota = new Cuota();
            cuota.Motivo = "Imputación manual - " + ApplicationUser.GetApellidoYNombre();
            cuota.Id = IdCuota;

            CuotaData.ImputarPagoCuota(cuota);

            return Json(new { ok = true });
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Exportar

        public ActionResult Exportar(string hiddenUsuario = "", int? IdSede = null, int? IdCurso = null, int? IdOferta = null, string estado = "", int? IdPeriodo = null, int? IdConcepto = null, int? IdTipoCurso = null, int? NroCuotaDesde = null, int? NroCuotaHasta = null, decimal? ImporteDesde = null, decimal? ImporteHasta = null, decimal? Importe2Desde = null, decimal? Importe2Hasta = null, DateTime? FechaVtoDesde = null, DateTime? FechaVtoHasta = null, DateTime? FechaVto2Desde = null, DateTime? FechaVto2Hasta = null, DateTime? FechaPagoDesde = null, DateTime? FechaPagoHasta = null, DateTime? FechaBajaDesde = null, DateTime? FechaBajaHasta = null)
        {
            var cuotas = new List<Cuota>();
            if (User.IsInRole("Ventanilla"))
            {
                IdSede = ApplicationUser.GetSede();
            }

            cuotas = CuotaData.consultarCuotas(hiddenUsuario, IdSede, IdCurso, IdOferta, estado, IdPeriodo, IdConcepto, IdTipoCurso,
                       NroCuotaDesde, NroCuotaHasta, ImporteDesde, ImporteHasta, Importe2Desde, Importe2Hasta,
                       FechaVtoDesde, FechaVtoHasta, FechaVto2Desde, FechaVto2Hasta, FechaPagoDesde, FechaPagoHasta, FechaBajaDesde, FechaBajaHasta
                       );

            ExportSettings.DefaultExportType = ExportType.WYSIWYG;
            return GridViewExtension.ExportToXlsx(CrearGridViewSettingsExportar("Cuotas"), cuotas, true);
        }

        private static GridViewSettings CrearGridViewSettingsExportar(string titulo)
        {
            var settings = new GridViewSettings
            {
                Name = "Exportar",
                KeyFieldName = "Id",
                Caption = Strings.TituloRedesJur,
                CallbackRouteValues = new { Controller = "RedesJur", Action = "PartialGrid" }
            };
            // Settings
            settings.SettingsExport.FileName = string.Format("{0}.xlsx", titulo);
            settings.SettingsExport.Styles.Default.Font.Name = "Segoe UI";
            settings.SettingsExport.Styles.Default.Font.Size = 10;
            settings.SettingsPager.Visible = false;
            settings.ControlStyle.Paddings.Padding = Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = Unit.Pixel(0);
            settings.Styles.Header.HorizontalAlign = HorizontalAlign.Center;
            // Columnas
            settings.Columns.Add(col => { col.FieldName = "Dni"; col.Caption = Strings.ColDocumento; });
            settings.Columns.Add(col => { col.FieldName = "Apellido"; col.Caption = "Apellido"; });
            settings.Columns.Add(col => { col.FieldName = "Nombre"; col.Caption = "Nombre"; });
            settings.Columns.Add(col => { col.FieldName = "Concepto"; col.Caption = Strings.ColConcepto; });
            settings.Columns.Add(col => { col.FieldName = "Curso"; col.Caption = Strings.ColCurso; });
            settings.Columns.Add(col => { col.FieldName = "NroCuota"; col.Caption = Strings.ColNroCuota; });
            settings.Columns.Add(col => { col.FieldName = "TotalCuota"; col.Caption = Strings.ColTotalCuota; });
            settings.Columns.Add(col => { col.FieldName = "Importe"; col.Caption = Strings.ColImporte; col.PropertiesEdit.DisplayFormatString = "c"; });
            settings.Columns.Add(col => { col.FieldName = "fechavto"; col.Caption = Strings.ColFechaVto; col.PropertiesEdit.DisplayFormatString = "d"; });
            settings.Columns.Add(col => { col.FieldName = "Importe2"; col.Caption = Strings.ColImporte2; col.PropertiesEdit.DisplayFormatString = "c"; });
            settings.Columns.Add(col => { col.FieldName = "fechavto2"; col.Caption = Strings.ColFechaVto2; col.PropertiesEdit.DisplayFormatString = "d"; });
            settings.Columns.Add(col => { col.FieldName = "fechaPago"; col.Caption = Strings.ColFechaPago; col.PropertiesEdit.DisplayFormatString = "d"; });
            settings.Columns.Add(col => { col.FieldName = "EstadoDescripcion"; col.Caption = Strings.ColEstado; });
            settings.Columns.Add(col => { col.FieldName = "fechaBaja"; col.Caption = Strings.ColFechaBaja; col.PropertiesEdit.DisplayFormatString = "d"; });

            return settings;
        }

        #endregion
    }
}