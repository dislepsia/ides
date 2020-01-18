using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cnrl;
using System.Configuration;
using cnrl.Logica;

namespace cnrl.Controllers
{
    public class precioController : Controller
    {
        private socioculturalesEntities db = new socioculturalesEntities();

        [HasPermission("Precio_Index")]
        // GET: precio
        public ActionResult Index()
        {
            return View(db.precio.ToList());
        }

        [HasPermission("Precio_Index")]
        // GET: precio/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            precio precio = db.precio.Find(id);
            if (precio == null)
            {
                return HttpNotFound();
            }
            return View(precio);
        }

        [HasPermission("Precio_ABM")]
        // GET: precio/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: precio/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "codigo,alumno,noalumno,alumnoBandaNegativa,noAlumnoBandaNegativa,descripcion,mayorDe60,descuentoUnaCuota,diasSegundoVencimiento,recargoSegundoVencimiento")] precio precio)
        {
            if (ModelState.IsValid)
            {
                db.precio.Add(precio);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(precio);
        }

        [HasPermission("Precio_ABM")]
        // GET: precio/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            precio precio = db.precio.Find(id);
            if (precio == null)
            {
                return HttpNotFound();
            }
            return View(precio);
        }

        // POST: precio/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "codigo,alumno,noalumno,alumnoBandaNegativa,noAlumnoBandaNegativa,descripcion,mayorDe60,descuentoUnaCuota,diasSegundoVencimiento,recargoSegundoVencimiento")] precio precio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(precio).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(precio);
        }

        [HasPermission("Precio_ABM")]
        // GET: precio/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            precio precio = db.precio.Find(id);
            if (precio == null)
            {
                return HttpNotFound();
            }
            return View(precio);
        }

        // POST: precio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            precio precio = db.precio.Find(id);
            db.precio.Remove(precio);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #region ajax
        public JsonResult GetPrecioCurso(int IdCurso = -1)
        {
            var curso = db.curso.Find(IdCurso);
            var resultado = new Dictionary<String, String>();
            if (curso != null)
            {
                resultado.Add("alumno", curso.precio.alumno.ToString());
                resultado.Add("noAlumno", curso.precio.noalumno.ToString());
                //Banda Negativa
                resultado.Add("descuentoBandaNegativa", (curso.precio.descuentoBandaNegativa.HasValue) ? curso.precio.descuentoBandaNegativa.Value.ToString() : "0");
                resultado.Add("descuentoBandaNegativaNoAlumno", (curso.precio.descuentoBandaNegativaNoAlumno.HasValue) ? curso.precio.descuentoBandaNegativaNoAlumno.Value.ToString() : "0");
                //
                resultado.Add("precioEmpleado", curso.precio.empleado.ToString());
                resultado.Add("mayorDe60", curso.precio.mayorDe60.ToString());
                resultado.Add("graduado", curso.precio.graduado.ToString());
                resultado.Add("diasSegundoVencimiento", (curso.precio.diasSegundoVencimiento.HasValue) ? curso.precio.diasSegundoVencimiento.Value.ToString() : ConfigurationManager.AppSettings["DiasSegundoVencimientoDefault"]);
                resultado.Add("descuentoUnaCuota", (curso.precio.descuentoUnaCuota.HasValue) ? curso.precio.descuentoUnaCuota.Value.ToString() : ConfigurationManager.AppSettings["Descuento1CuotaDefault"]);
                resultado.Add("recargoSegundoVencimiento", (curso.precio.recargoSegundoVencimiento.HasValue) ? curso.precio.recargoSegundoVencimiento.Value.ToString() : ConfigurationManager.AppSettings["RecargoSegundoVencimientoDefault"]);
                resultado.Add("gratuito", (curso.precio.gratuito) ? curso.precio.gratuito.ToString() : "0");
            }
            return Json(resultado, JsonRequestBehavior.AllowGet);
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
    }
}
