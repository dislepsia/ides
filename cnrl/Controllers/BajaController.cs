using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cnrl;
using cnrl.Logica;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Configuration;
using cnrl.Models;
using cnrl.Repositories;
using cnrl.Helpers;
using DevExpress.Data.WcfLinq.Helpers;

namespace cnrl.Controllers
{
    public class BajaController : Controller
    {
        private socioculturalesEntities db = new socioculturalesEntities();

        // GET: Baja
        [HasPermission("Alumnos_Index")]
        public ActionResult Index()
        {
            try
            {
                var cursa = db.cursa.Include(c => c.EstadoCursa).Include(c => c.AspNetUsers).Include(c => c.oferta1);
                var idUsuario = (Request.Params["usuario"] != null) ? Request.Params["usuario"].ToString() : "";
                if (!string.IsNullOrEmpty(idUsuario))
                {
                    var usuario = db.AspNetUsers.Find(idUsuario);
                    ViewBag.IdUsuario = idUsuario;
                    ViewBag.NombreUsuario = usuario.descripcion;

                }

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
                return View(/*cursa.ToList()*/);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                ViewBag.ErrorMessage = string.Format(Strings.ErrorIntentarInsertarRegistro, e.Message.Replace(Strings.TheStatementHasBeenTerminated, ""));
                return View();
            }
        }

        private void PrepararViewBag()
        {


            if (User.IsInRole("Ventanilla"))
            {
                int sede = ApplicationUser.GetSede();
                ViewBag.sedes = new SelectList(
                        db.sede.Where(x => x.borradoLogico == false && x.codigo == sede && x.sector == (int)Sectores.Idiomas).ToList()
                        .ToSelectList(
                            x => x.nombre,
                            x => x.codigo.ToString(),
                            Strings.MsgTodasSedes,
                            Constantes.ERROR.ToString(),
                            Constantes.ERROR.ToString()
                        )
                    , "Value", "Text");

            }
            else
            {
                ViewBag.sedes = new SelectList(
                    db.sede.Where(x => x.borradoLogico == false && x.sector == (int)Sectores.Idiomas).ToList()
                    .ToSelectList(
                        x => x.nombre,
                        x => x.codigo.ToString(),
                        Strings.MsgTodasSedes,
                        Constantes.ERROR.ToString(),
                        Constantes.ERROR.ToString()
                    )
                , "Value", "Text");
            }

        }

        private void PrepararViewBagEmpresa()
        {


            if (User.IsInRole("Ventanilla"))
            {
                int sede = ApplicationUser.GetSede();
                ViewBag.sedes = new SelectList(
                        db.sede.Where(x => x.borradoLogico == false && x.codigo == sede && x.sector == (int)Sectores.Empresas).ToList()
                        .ToSelectList(
                            x => x.nombre,
                            x => x.codigo.ToString(),
                            Strings.MsgTodasSedes,
                            Constantes.ERROR.ToString(),
                            Constantes.ERROR.ToString()
                        )
                    , "Value", "Text");

            }
            else
            {
                ViewBag.sedes = new SelectList(
                    db.sede.Where(x => x.borradoLogico == false && x.sector == (int)Sectores.Empresas).ToList()
                    .ToSelectList(
                        x => x.nombre,
                        x => x.codigo.ToString(),
                        Strings.MsgTodasSedes,
                        Constantes.ERROR.ToString(),
                        Constantes.ERROR.ToString()
                    )
                , "Value", "Text");
            }

        }

        private void PrepararViewBagGraduado()
        {


            if (User.IsInRole("Ventanilla"))
            {
                int sede = ApplicationUser.GetSede();
                ViewBag.sedes = new SelectList(
                        db.sede.Where(x => x.borradoLogico == false && x.codigo == sede && x.sector == (int)Sectores.Graduados).ToList()
                        .ToSelectList(
                            x => x.nombre,
                            x => x.codigo.ToString(),
                            Strings.MsgTodasSedes,
                            Constantes.ERROR.ToString(),
                            Constantes.ERROR.ToString()
                        )
                    , "Value", "Text");

            }
            else
            {
                ViewBag.sedes = new SelectList(
                    db.sede.Where(x => x.borradoLogico == false && x.sector == (int)Sectores.Graduados).ToList()
                    .ToSelectList(
                        x => x.nombre,
                        x => x.codigo.ToString(),
                        Strings.MsgTodasSedes,
                        Constantes.ERROR.ToString(),
                        Constantes.ERROR.ToString()
                    )
                , "Value", "Text");
            }

        }

        // GET: Baja/Details/5
        [HasPermission("Alumnos_Index")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cursa cursa = db.cursa.Find(id);
            if (cursa == null)
            {
                return HttpNotFound();
            }
            return View(cursa);
        }

        // GET: Baja/Create
        [HasPermission("Alumnos_Index")]
        public ActionResult Create()
        {
            ViewBag.estado = new SelectList(db.EstadoCursa, "codigo", "descripcion");
            ViewBag.alumno = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.oferta = new SelectList(db.oferta, "codigo", "docente");
            return View();
        }

        // POST: Baja/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "oferta,fechaAlta,aprobo,observacion,alumno,codigo,estado,cantidadCuotas,RequierePreInscripcion")] cursa cursa)
        {
            if (ModelState.IsValid)
            {
                db.cursa.Add(cursa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.estado = new SelectList(db.EstadoCursa, "codigo", "descripcion", cursa.estado);
            ViewBag.alumno = new SelectList(db.AspNetUsers, "Id", "Email", cursa.alumno);
            ViewBag.oferta = new SelectList(db.oferta, "codigo", "docente", cursa.oferta);
            return View(cursa);
        }

        // GET: Baja/Edit/5
        [HasPermission("Alumnos_Index")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cursa cursa = db.cursa.Find(id);
            if (cursa == null)
            {
                return HttpNotFound();
            }
            ViewBag.estado = new SelectList(db.EstadoCursa, "codigo", "descripcion", cursa.estado);
            ViewBag.alumno = new SelectList(db.AspNetUsers, "Id", "Email", cursa.alumno);
            ViewBag.oferta = new SelectList(db.oferta, "codigo", "docente", cursa.oferta);
            return View(cursa);
        }

        // POST: Baja/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "oferta,fechaAlta,aprobo,observacion,alumno,codigo,estado,cantidadCuotas,RequierePreInscripcion")] cursa cursa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cursa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.estado = new SelectList(db.EstadoCursa, "codigo", "descripcion", cursa.estado);
            ViewBag.alumno = new SelectList(db.AspNetUsers, "Id", "Email", cursa.alumno);
            ViewBag.oferta = new SelectList(db.oferta, "codigo", "docente", cursa.oferta);
            return View(cursa);
        }

        // GET: Baja/Delete/5
        [HasPermission("Alumnos_Index")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cursa cursa = db.cursa.Find(id);
            if (cursa == null)
            {
                return HttpNotFound();
            }
            return View(cursa);
        }

        // POST: Baja/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            cursa cursa = db.cursa.Find(id);
            db.cursa.Remove(cursa);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HasPermission("Alumnos_Index")]
        public ActionResult PartialGrid(int? IdSede = null, string IdUsuario = "")
        {
            try
            {
                var solicitudCursa = consultar(IdSede, IdUsuario);
                //List<cursa> solicitudCursa = db.cursa.Where(m => m.estado.Value.Equals(5)).ToList();

                return PartialView("_Grid", solicitudCursa.ToList());
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                ViewBag.ErrorMessage = string.Format(Strings.ErrorIntentarInsertarRegistro, e.Message.Replace(Strings.TheStatementHasBeenTerminated, ""));
                return View();
            }
        }

        public List<cursa> consultar(int? IdSede = null, string IdUsuario = "")
        {
            IQueryable<cursa> cursadas;

            if (User.IsInRole("Ventanilla"))
            {
                IdSede = ApplicationUser.GetSede();
                cursadas = db.cursa.Where(m => m.estado.Value.Equals(5));
            }
            else
            {
                string usuario = ApplicationUser.GetId();
                cursadas = db.cursa.Where(m => m.estado.Value.Equals(5) && (m.oferta1.curso1.AdminCurso.Equals(usuario) || m.oferta1.curso1.AdminCurso2.Equals(usuario) || m.oferta1.curso1.AdminCurso3.Equals(usuario) || m.oferta1.curso1.AdminCurso4.Equals(usuario)));
            }



            //IQueryable<cursa> cursadas = db.cursa.Where(m => m.estado.Value.Equals(5));
            bool algunFiltro = false;

            if (IdSede.HasValue && IdSede.Value != Constantes.ERROR)
            {
                cursadas = cursadas.Where(x => x.oferta1.sede == IdSede.Value);
                algunFiltro = true;
            }

            if (!String.IsNullOrEmpty(IdUsuario) && IdUsuario != "-1")
            {
                cursadas = cursadas.Where(x => x.alumno == IdUsuario);
                algunFiltro = true;
            }

            if (algunFiltro)
                return cursadas.OrderBy(x => x.AspNetUsers.Apellido).ToList();
            else
                return cursadas.OrderBy(x => x.AspNetUsers.Apellido).ToList();
            //return new List<cursa>();
        }

        [HasPermission("Baja_Cursada")]
        public ActionResult VerMotivoBaja(int? codigo)
        {
            try
            {
                if (codigo == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                /*else
                    //Imputo las cuotas de las personas que se aceptaron la baja
                    imputarCuotas(codigo);*/

                cursa cursa = db.cursa.Find(codigo);
                //cursa.estado = 6;

                //ModelState.Remove("DescripcionBaja");
                //db.Configuration.ValidateOnSaveEnabled = false;
                //if (db.SaveChanges() > 0)
                //{

                //List<Cuota> cuota = CuotaData.consultarCuotas(cursa.alumno.ToString(), null, cursa.oferta1.curso, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
                List<Cuota> cuota = CuotaData.consultarCuotas(cursa.alumno.ToString(), null, cursa.oferta1.curso).Where(x => x.NroFactura.Contains(cursa.codigo.ToString()) && !x.Estado.Equals("B")).ToList();
                string cuotasMensaje = "";

                cuotasMensaje = "<div><table class=\"table\"><thead><tr><th><strong> Cuota </strong></th>";
                cuotasMensaje += "<th><strong> Importe </strong></th>";
                cuotasMensaje += "<th><strong> Estado </strong></th>";
                cuotasMensaje += "<th><strong> Fecha Vto </strong></th></tr></thead>";


                foreach (Cuota item in cuota)
                {
                    cuotasMensaje += "<tbody><tr><td>" + item.NroCuota + "</td><td>$" + decimal.Round(item.Importe, 2) + "</td><td>" + item.EstadoDescripcion + "</td><td>" + item.fechavto.Value.ToShortDateString() + "</td></tr>";
                }

                cuotasMensaje += "</tbody></table></div>";

                if (cursa.DescripcionBaja != null)
                {
                    return Json(new { ok = true, curso = cursa.oferta1.curso1.descripcion.ToString(), cuotas = cuotasMensaje, motivo = "El alumno/a <b>" + cursa.AspNetUsers.Apellido + ", " + cursa.AspNetUsers.Nombre + "</b> solicito la baja de cursada por el siguiente motivo: <br><b>" + cursa.DescripcionBaja + "</b><br> ¿Acepta la Baja?", fechaBaja = cursa.DateModified.ToShortDateString().ToString() });
                }
                else
                {
                    return Json(new { ok = true, curso = cursa.oferta1.curso1.descripcion.ToString(), cuotas = cuotasMensaje, motivo = "", fechaBaja = cursa.DateModified.ToShortDateString().ToString() });
                }

                //}
                //else
                //return Json(new { ok = false });

            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                ViewBag.ErrorMessage = string.Format(Strings.ErrorIntentarInsertarRegistro, e.Message.Replace(Strings.TheStatementHasBeenTerminated, ""));
                return View();
            }
        }

        [HasPermission("Baja_Cursada")]
        public ActionResult ConfirmarSolicitud(int? codigo)
        {
            try
            {
                if (codigo == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                // else
                //Imputo las cuotas de las personas que se aceptaron la baja
                //imputarCuotas(codigo);

                cursa cursa = db.cursa.Find(codigo);
                cursa.estado = 6;

                ModelState.Remove("DescripcionBaja");
                db.Configuration.ValidateOnSaveEnabled = false;
                if (db.SaveChanges() > 0)
                {
                    List<Cuota> cuotas = CuotaData.consultarCuotas(cursa.alumno.ToString(), null, cursa.oferta1.curso).Where(x => x.NroFactura.Contains(cursa.codigo.ToString()) && !x.Estado.Equals("B")).ToList();

                    BajasCuotas(cuotas);
                    //Enviar mail al alumno
                    string mail = db.AspNetUsers.Where(x => x.Id.Equals(cursa.alumno)).First().Email;

                    cnrl.Logica.App.enviarMailUsuario(mail, "Solicitud de Baja", "Se aprobó la solicitud de baja solicitada "
                  + "para el curso " + cursa.oferta1.curso1.descripcion + ", codigo de curso " + cursa.oferta1.curso1.codCurso + ".<br><br> ");

                    if (cursa.DescripcionBaja != null)
                    {
                        return Json(new { ok = true, motivo = "El alumno/a <b>" + cursa.AspNetUsers.Apellido + ", " + cursa.AspNetUsers.Nombre + "</b> solicito la baja de cursada por el siguiente motivo: <br><b>" + cursa.DescripcionBaja + "</b>" });
                    }
                    else
                    {
                        return Json(new { ok = true, motivo = "" });
                    }

                }
                else
                    return Json(new { ok = false });

            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                ViewBag.ErrorMessage = string.Format(Strings.ErrorIntentarInsertarRegistro, e.Message.Replace(Strings.TheStatementHasBeenTerminated, ""));
                return View();
            }
        }

        [HasPermission("Baja_Cursada")]
        public ActionResult RechazarSolicitud(int? codigo, string mensaje = null)
        {
            try
            {
                if (codigo == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                cursa cursa = db.cursa.Find(codigo);
                cursa.estado = cursa.EstadoAntBaja;
                cursa.resultadoCursada = 4;
                cursa.baja = null;
                cursa.DescripcionBaja = null;
                cursa.EstadoAntBaja = null;

                ModelState.Remove("DescripcionBaja");
                ModelState.Remove("MotivoBaja");
                db.Configuration.ValidateOnSaveEnabled = false;
                if (db.SaveChanges() > 0)
                {
                    //Enviar mail al alumno
                    string mail = db.AspNetUsers.Where(x => x.Id.Equals(cursa.alumno)).First().Email;

                    cnrl.Logica.App.enviarMailUsuario(mail, "Solicitud de Baja", "Se rechazo la solicitud de baja solicitada "
                  + "para el curso " + cursa.oferta1.curso1.descripcion + ", codigo de curso " + cursa.oferta1.curso1.codCurso + ".<br><br> "
                  + mensaje);


                    return Json(new { ok = true, motivo = "" });
                }
                else
                {
                    return Json(new { ok = false, motivo = "" });
                }

            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                ViewBag.ErrorMessage = string.Format(Strings.ErrorIntentarInsertarRegistro, e.Message.Replace(Strings.TheStatementHasBeenTerminated, ""));
                return View();
            }
        }

        [HasPermission("Baja_Cursada")]
        public ActionResult ConfirmarTodas()
        {
            try
            {
                List<cursa> solicitudesCursa = db.cursa.Where(m => m.estado.Value.Equals(5)).ToList();

                if (solicitudesCursa.Count > 0)
                {
                    foreach (cursa solicitud in solicitudesCursa)
                    {
                        solicitud.estado = 6;

                        //Imputo las cuotas de las personas que se aceptaron la baja
                        imputarCuotas(solicitud.codigo);
                        

                        db.SaveChanges();
                    }

                    return Json(new { ok = true });
                }
                else
                    return Json(new { ok = "error1" });
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                ViewBag.ErrorMessage = string.Format(Strings.ErrorIntentarInsertarRegistro, e.Message.Replace(Strings.TheStatementHasBeenTerminated, ""));
                return View();
            }
        }

        public void imputarCuotas(int? codigo)
        {
            Cuota cuota = new Cuota();
            string motivo = "Imputación manual - " + ApplicationUser.GetApellidoYNombre();
            cuota.Motivo = motivo;

            cursa cursa = db.cursa.Find(codigo);
            List<oferta> oferta = db.oferta.Where(z => z.codigo.Equals(cursa.oferta1.codigo)).ToList();
            AspNetUsers pibe = db.AspNetUsers.Where(x => x.Id.Equals(cursa.alumno)).First();

            //
            var cuotas = new List<Cuota>();
            cuotas = CuotaData.consultarCuotas(pibe.Id, null, oferta[0].curso, oferta[0].codigo, null, null, null, null,
                        null, null, null, null, null, null,
                        null, null, null, null, null, null, null, null
                        );
            //

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["ServicioTeso"]);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["ServicioTesoUsuario"])));

            List<KeyValuePair<string, object>> parametros = new List<KeyValuePair<string, object>>();

            foreach (Cuota parcial in cuotas)
            {
                parametros.Add(new KeyValuePair<string, object>("id", parcial.Id.ToString()));
                parametros.Add(new KeyValuePair<string, object>("motivo", motivo));

                var responseGet = httpClient.GetAsync("Cuota/ImputarPagoCuota" + Constantes.AsQueryString(parametros)).Result;
                var respuesta = responseGet.Content.ReadAsAsync<bool>().Result;
                if (responseGet.IsSuccessStatusCode == false)
                {
                    throw new Exception("Error en servicio Tesoreria");
                }

                parametros.Clear();
            }
        }

        public void BajasCuotas(List<Cuota> cuotas)
        {
            Cuota cuota = new Cuota();
            string motivo = "Baja Cursada - " + ApplicationUser.GetApellidoYNombre();
            cuota.Motivo = motivo;
            string[] nrof;


            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["ServicioTeso"]);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["ServicioTesoUsuario"])));

            List<KeyValuePair<string, object>> parametros = new List<KeyValuePair<string, object>>();

            foreach (Cuota parcial in cuotas)
            {
                nrof = parcial.NroFactura.Split('_');

                parametros.Add(new KeyValuePair<string, object>("origen", parcial.Origen.ToString()));
                parametros.Add(new KeyValuePair<string, object>("id", int.Parse(nrof[0])));
                parametros.Add(new KeyValuePair<string, object>("motivo", motivo));

                var responseGet = httpClient.GetAsync("Cuota/BajaCuotaPorCursada" + Constantes.AsQueryString(parametros)).Result;
                var respuesta = responseGet.Content.ReadAsAsync<bool>().Result;
                if (responseGet.IsSuccessStatusCode == false)
                {
                    throw new Exception("Error en servicio Tesoreria");
                }

                parametros.Clear();
            }
        }

    }
}
