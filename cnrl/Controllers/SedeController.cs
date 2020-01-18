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
    public class SedeController : Controller
    {
        private socioculturalesEntities db = new socioculturalesEntities();

        // GET: /Sede/
        [HasPermission("Admin_Index")]
        public ActionResult Index()
        {
            try
            {
                if (User.IsInRole("AdministradorEmpresas"))
                {
                    return View(db.sede.Where(m => m.borradoLogico.Value == false && m.sector == (int)Sectores.Empresas).ToList());
                }
                else
                {
                    if (User.IsInRole("AdministradorGraduado"))
                    {
                        return View(db.sede.Where(m => m.borradoLogico.Value == false && m.sector == (int)Sectores.Graduados).ToList());
                    }
                    else
                    {
                        return View(db.sede.Where(m => m.borradoLogico.Value == false && m.sector == (int)Sectores.Idiomas).ToList());
                    }
                    
                }
                
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                ViewBag.ErrorMessage = string.Format(Strings.ErrorIntentarInsertarRegistro, e.Message.Replace(Strings.TheStatementHasBeenTerminated, ""));
                return View();
            }
        }

        // GET: /Sede/Details/5
        [HasPermission("Admin_Index")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            sede sede = db.sede.Find(id);

            if (sede == null)
            {
                return HttpNotFound();
            }
            return View(sede);
        }

        // GET: /Sede/Create
        [HasPermission("Admin_Index")]
        public ActionResult Create()
        {
            return View("Create");
        }

        // POST: /Sede/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(sede sede)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    sede.borradoLogico = false;
                    sede.sector = User.IsInRole("AdministradorEmpresas") == true ? (int)Sectores.Empresas : User.IsInRole("AdministradorGraduado") == true ? (int)Sectores.Graduados : (int)Sectores.Idiomas;
                    db.sede.Add(sede);
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

            return PartialView(sede);
        }

        // GET: /Sede/Edit/5
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

            sede sede = db.sede.Find(id);

            if (sede == null)
            {
                return HttpNotFound();
            }

            return View("Edit", sede);
        }

        // POST: /Sede/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(sede sede)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    sede.borradoLogico = false;
                    sede.sector = User.IsInRole("AdministradorEmpresas") == true ? (int)Sectores.Empresas : User.IsInRole("AdministradorGraduado") == true ? (int)Sectores.Graduados : (int)Sectores.Idiomas;
                    db.Entry(sede).State = EntityState.Modified;
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

            return View(sede);
        }

        // GET: /Sede/Delete/5
        [HasPermission("Admin_Index")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            sede sede = db.sede.Find(id);

            if (sede == null)
            {
                return HttpNotFound();
            }
            return View("Delete", sede);
        }

        // POST: /Sede/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                sede sede = db.sede.Find(id);
                sede.borradoLogico = true;
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
            IQueryable<sede> sedes = db.sede.Where(m => m.borradoLogico.Value == false);
            if (User.IsInRole("AdministradorEmpresas"))
            {
                sedes = sedes.Where(x => x.sector.Value == (int)Sectores.Empresas);
            }
            else
            {
                if (User.IsInRole("AdministradorGraduado"))
                {
                    sedes = sedes.Where(x => x.sector.Value == (int)Sectores.Graduados);
                }
                else
                {
                    sedes = sedes.Where(x => x.sector.Value == (int)Sectores.Idiomas);
                }
                
            }

            return PartialView("_Grid", sedes.ToList());
        }
    }
}
