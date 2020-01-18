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

namespace cnrl.Controllers
{
    public class TipoCursoController : Controller
    {
        private socioculturalesEntities db = new socioculturalesEntities();

        // GET: /TipoCurso/
        [HasPermission("TipoCursos_Index")]
        public ActionResult Index()
        {
            try
            {
                return View(db.tipocurso.ToList());
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                ViewBag.ErrorMessage = string.Format(Strings.ErrorIntentarInsertarRegistro, e.Message.Replace(Strings.TheStatementHasBeenTerminated, ""));
                return View();
            }
        }

        // GET: /TipoCurso/Details/5
        [HasPermission("TipoCursos_Index")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipocurso tipocurso = db.tipocurso.Find(id);
            if (tipocurso == null)
            {
                return HttpNotFound();
            }
            return View(tipocurso);
        }

        // GET: /TipoCurso/Create
        [HasPermission("TipoCursos_ABM")]
        public ActionResult Create()
        {
            return View("Create");
        }

        // POST: /TipoCurso/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public ActionResult Create(tipocurso tipocurso)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    tipocurso.abreviado = "";
                    tipocurso.sector = User.IsInRole("AdministradorEmpresas") == true ? (int)Sectores.Empresas : User.IsInRole("AdministradorGraduado") == true ? (int)Sectores.Graduados : (int)Sectores.Idiomas;
                    db.tipocurso.Add(tipocurso);
                    db.SaveChanges();
                    return Json(new { ok = true });
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                ViewBag.ErrorMessage = string.Format(Strings.ErrorIntentarInsertarRegistro, e.Message.Replace(Strings.TheStatementHasBeenTerminated, ""));
            }

            //return View(tipocurso);
            return PartialView(tipocurso);
        }

        // GET: /TipoCurso/Edit/5
        [HasPermission("TipoCursos_ABM")]
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

            tipocurso tipoCurso = db.tipocurso.Find(id);

            if (tipoCurso == null)
            {
                return HttpNotFound();
            }
            return View("Edit", tipoCurso);

            //tipocurso tipocurso = db.tipocurso.Find(id);
            //if (tipocurso == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(tipocurso);
        }

        // POST: /TipoCurso/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tipocurso tipocurso)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    tipocurso.abreviado = "";
                    tipocurso.sector = User.IsInRole("AdministradorEmpresas") == true ? (int)Sectores.Empresas : User.IsInRole("AdministradorGraduado") == true ? (int)Sectores.Graduados : (int)Sectores.Idiomas;
                    db.Entry(tipocurso).State = EntityState.Modified;
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

            return View(tipocurso);

            //if (ModelState.IsValid)
            //{
            //    db.Entry(tipocurso).State = EntityState.Modified;
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}
            //return View(tipocurso);
        }

        // GET: /TipoCurso/Delete/5
        [HasPermission("TipoCursos_ABM")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tipocurso tipocurso = db.tipocurso.Find(id);
            if (tipocurso == null)
            {
                return HttpNotFound();
            }
            return View("Delete", tipocurso);
        }

        // POST: /TipoCurso/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                tipocurso tipocurso = db.tipocurso.Find(id);
                db.tipocurso.Remove(tipocurso);
                if (db.SaveChanges() > 0)
                    return Json(new { ok = true });
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                ViewBag.ErrorMessage = string.Format(Strings.ErrorIntentarInsertarRegistro, e.Message.Replace(Strings.TheStatementHasBeenTerminated, ""));
            }

            return Json(new { error = true });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HasPermission("Cursos_Index")]
        public ActionResult PartialGrid()
        {
            List<tipocurso> tipoCursos = db.tipocurso.ToList();
            if (User.IsInRole("AdministradorEmpresas"))
            {
                tipoCursos = tipoCursos.Where(x => x.sector == (int)Sectores.Empresas).ToList();
            }
            else
            {
                if (User.IsInRole("AdministradorGraduado"))
                {
                    tipoCursos = tipoCursos.Where(x => x.sector == (int)Sectores.Graduados).ToList();
                }
                else
                {
                    tipoCursos = tipoCursos.Where(x => x.sector == (int)Sectores.Idiomas).ToList();
                }

            }
            return PartialView("_Grid", tipoCursos);
        }
    }
}
