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
using System.Text.RegularExpressions;

namespace cnrl.Controllers
{
    public class MailController : Controller
    {
        private socioculturalesEntities db = new socioculturalesEntities();

        // GET: Mail
        [HasPermission("Admin_Index")]
        public ActionResult Index()
        {
            try
            {
                return View(User.IsInRole("AdministradorEmpresas") == true ? db.plantillaEmail.Where(x => x.sector.Value == (int)Sectores.Empresas).ToList() : User.IsInRole("AdministradorGraduado") == true ? db.plantillaEmail.Where(x => x.sector.Value == (int)Sectores.Graduados).ToList() : db.plantillaEmail.Where(x => x.sector.Value == (int)Sectores.Idiomas).ToList());
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                ViewBag.ErrorMessage = string.Format(Strings.ErrorIntentarInsertarRegistro, e.Message.Replace(Strings.TheStatementHasBeenTerminated, ""));
                return View();
            }
        }

        // GET: Mail/Details/5
        [HasPermission("Admin_Index")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plantillaEmail plantillaEmail = db.plantillaEmail.Find(id);
            if (plantillaEmail == null)
            {
                return HttpNotFound();
            }
            return View(plantillaEmail);
        }

        // GET: Mail/Create
        [HasPermission("Admin_Index")]
        public ActionResult Create()
        {
            return View("Create");
        }

        // POST: Mail/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(FormCollection form)
        {
            plantillaEmail plantilla = new plantillaEmail();
            plantilla.asunto = form["asunto"];
            plantilla.descripcion = form["descripcion"];
            plantilla.cuerpo = form["cuerpo"];
            ViewBag.mensaje = plantilla.cuerpo;
            plantilla.sector = User.IsInRole("AdministradorEmpresas") == true ? (int)Sectores.Empresas : User.IsInRole("AdministradorGraduado") == true ? (int)Sectores.Graduados : (int)Sectores.Idiomas;
            try
            {
                if (ModelState.IsValid)
                {
                    #region Palabras aceptadas

                    List<string> listado = new List<string>();

                    listado.Add("%NOMBRE%");
                    listado.Add("%APELLIDO%");
                    listado.Add("%DOCUMENTO%");
                    listado.Add("%SEDE%");
                    listado.Add("%TIPO_CURSO%");
                    listado.Add("%CURSO%");
                    listado.Add("%DIA_HORARIO%");
                    listado.Add("%AULA%");
                    listado.Add("%FECHA_INICIO%");
                    listado.Add("%NRO_CUOTA%");
                    listado.Add("%TOTAL_CUOTAS%");
                    listado.Add("%IMPORTE%");
                    listado.Add("%IMPORTE2%");
                    listado.Add("%FECHA_VENCIMIENTO%");
                    listado.Add("%FECHA_VENCIMIENTO2%");

                    #endregion

                    //Patron
                    Regex rgx = new Regex(@"%([A-Z])\w+%", RegexOptions.IgnoreCase);
                    MatchCollection matches = rgx.Matches(form["cuerpo"]);
                    //Si encontro coincidencias
                    if (matches.Count > 0)
                    {
                        //Pregunto por cada una para chequear que solo se acepten las del listado
                        foreach (Match match in matches)
                        {
                            if (!listado.Contains(match.ToString()))
                            {
                                ViewBag.ErrorMessage += string.Format("Se detecto que " + match.ToString() + " no existe en el listado de palabras aceptadas, revise el cuerpo del correo e intente nuevamente.") + Environment.NewLine;
                            }
                        }
                    }

                    if (ViewBag.ErrorMessage == null)
                    {
                        db.plantillaEmail.Add(plantilla);
                        db.SaveChanges();
                        return Json(new { ok = true });
                    }
                }
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                ViewBag.ErrorMessage = string.Format(Strings.ErrorIntentarInsertarRegistro, e.Message.Replace(Strings.TheStatementHasBeenTerminated, ""));
            }

            return PartialView(plantilla);
        }

        // GET: Mail/Edit/5
        [ValidateInput(false)]
        [HasPermission("Admin_Index")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string callback = Request.Params["callback"];
            if (string.IsNullOrEmpty(callback))
                callback = "AjaxOk";
            //var curso = new curso();
            ViewBag.modo = "Edit";
            ViewBag.Callback = callback;

            plantillaEmail plantillaEmail = db.plantillaEmail.Find(id);

            if (plantillaEmail == null)
            {
                return HttpNotFound();
            }
            return View("Edit", plantillaEmail);
        }

        // POST: Mail/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection form)
        {
            plantillaEmail plantilla = null;

            try
            {
                if (ModelState.IsValid)
                {
                    plantilla = db.plantillaEmail.Find(Convert.ToInt32(form["codigo"]));

                    plantilla.asunto = form["asunto"];
                    plantilla.descripcion = form["descripcion"];
                    plantilla.cuerpo = form["cuerpo"];
                    plantilla.sector = User.IsInRole("AdministradorEmpresas") == true ? (int)Sectores.Empresas : User.IsInRole("AdministradorGraduado") == true ? (int)Sectores.Graduados : (int)Sectores.Idiomas;
                    db.SaveChanges();
                    return Json(new { ok = true });
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                ViewBag.ErrorMessage = string.Format(Strings.ErrorIntentarInsertarRegistro, e.Message.Replace(Strings.TheStatementHasBeenTerminated, ""));
                return View();
            }

            return View("Edit", plantilla);
        }

        // GET: Mail/Delete/5
        [HasPermission("Admin_Index")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            plantillaEmail plantillaEmail = db.plantillaEmail.Find(id);
            if (plantillaEmail == null)
            {
                return HttpNotFound();
            }
            return View("Delete", plantillaEmail);
        }

        // POST: Mail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                plantillaEmail plantilla = db.plantillaEmail.Find(id);

                db.plantillaEmail.Remove(plantilla);

                if (db.SaveChanges() > 0)
                    return Json(new { ok = true });
                else
                    return Json(new { error = true });
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                ViewBag.ErrorMessage = string.Format(Strings.ErrorIntentarInsertarRegistro, e.Message.Replace(Strings.TheStatementHasBeenTerminated, ""));
                return View();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HasPermission("Admin_Index")]
        public ActionResult PartialGrid()
        {
            List<plantillaEmail> tipos = db.plantillaEmail.ToList();

            if (User.IsInRole("AdministradorEmpresas"))
            {
                tipos = tipos.Where(x => x.sector == (int)Sectores.Empresas).ToList();
            }
            else
            {
                if (User.IsInRole("AdministradorGraduado"))
                {
                    tipos = tipos.Where(x => x.sector == (int)Sectores.Graduados).ToList();
                }
                else
                {
                    tipos = tipos.Where(x => x.sector == (int)Sectores.Idiomas).ToList();
                }

            }
            return PartialView("_Grid", tipos);
        }
    }
}
