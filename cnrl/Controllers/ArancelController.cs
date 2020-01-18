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
using cnrl.Helpers;
using cnrl.Models;

namespace cnrl.Controllers
{
    public class ArancelController : Controller
    {
        private socioculturalesEntities db = new socioculturalesEntities();

        // GET: Arancel
        [HasPermission("Admin_Index")]
        public ActionResult Index()
        {
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


            return View(db.precio.ToList());
        }

        private void PrepararViewBag()
        {
            ViewBag.Descripcion = new SelectList(
                db.precio.Where(x => x.sector == (int)Sectores.Idiomas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.concepto.ToString(),
                    "(Todas las descripciones)",
                    ""/*Constantes.ERROR.ToString()*/,
                    ""/*Constantes.ERROR.ToString()*/
                )
                , "Value", "Text");

            ViewBag.Conceptos = new SelectList(
                db.precio.Where(x => x.sector == (int)Sectores.Idiomas).ToList()
                .ToSelectList(
                    x => x.concepto.ToString(),
                    x => x.concepto.ToString(),
                    "(Todos los conceptos)",
                    ""/*Constantes.ERROR.ToString()*/,
                    ""/*Constantes.ERROR.ToString()*/
                )
                , "Value", "Text");
        }

        private void PrepararViewBagEmpresa()
        {
            ViewBag.Descripcion = new SelectList(
                db.precio.Where(x => x.sector == (int)Sectores.Empresas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.concepto.ToString(),
                    "(Todas las descripciones)",
                    ""/*Constantes.ERROR.ToString()*/,
                    ""/*Constantes.ERROR.ToString()*/
                )
                , "Value", "Text");

            ViewBag.Conceptos = new SelectList(
                db.precio.Where(x => x.sector == (int)Sectores.Empresas).ToList()
                .ToSelectList(
                    x => x.concepto.ToString(),
                    x => x.concepto.ToString(),
                    "(Todos los conceptos)",
                    ""/*Constantes.ERROR.ToString()*/,
                    ""/*Constantes.ERROR.ToString()*/
                )
                , "Value", "Text");
        }

        private void PrepararViewBagGraduado()
        {
            ViewBag.Descripcion = new SelectList(
                db.precio.Where(x => x.sector == (int)Sectores.Graduados).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.concepto.ToString(),
                    "(Todas las descripciones)",
                    ""/*Constantes.ERROR.ToString()*/,
                    ""/*Constantes.ERROR.ToString()*/
                )
                , "Value", "Text");

            ViewBag.Conceptos = new SelectList(
                db.precio.Where(x => x.sector == (int)Sectores.Graduados).ToList()
                .ToSelectList(
                    x => x.concepto.ToString(),
                    x => x.concepto.ToString(),
                    "(Todos los conceptos)",
                    ""/*Constantes.ERROR.ToString()*/,
                    ""/*Constantes.ERROR.ToString()*/
                )
                , "Value", "Text");
        }

        [HasPermission("Admin_Index")]
        public ActionResult PartialGrid(string IdDescripcion = "", int? Concepto = null)
        {
            if (!string.IsNullOrEmpty(IdDescripcion.ToString()))
            {
                var descri = Convert.ToInt32(IdDescripcion);

                var arancel = db.precio.Where(x => x.concepto == descri).ToList();

                return PartialView("_Grid", arancel);
            }
            else if (Concepto != null)
            {
                var arancel = db.precio.Where(x => x.concepto == Concepto).ToList();

                return PartialView("_Grid", arancel);
            }
            else
            {
                var arancel = db.precio.ToList();

                if (User.IsInRole("AdministradorEmpresas"))
                {
                    arancel = arancel.Where(x => x.sector == (int)Sectores.Empresas).ToList();
                }
                else
                {
                    if (User.IsInRole("AdministradorGraduado"))
                    {
                        arancel = arancel.Where(x => x.sector == (int)Sectores.Graduados).ToList();
                    }
                    else
                    {
                        arancel = arancel.Where(x => x.sector == (int)Sectores.Idiomas).ToList();
                    }

                }


                return PartialView("_Grid", arancel);
            }
        }

        [HttpGet]
        public JsonResult BuscarDescripcion(string q)
        {
            IQueryable<precio> precio = db.precio;

            var model = new ViewModels.ViewPrecio();

            model.precios = db.precio.Where(x => x.descripcion.Contains(q)).ToList();

            var rta = model.precios.Select(i => new { id = i.concepto, text = i.descripcion }).ToList();
            return Json(rta, JsonRequestBehavior.AllowGet);
        }


        // GET: Arancel/Details/5
        [HasPermission("Admin_Index")]
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

        // GET: Arancel/Create
        [HasPermission("Admin_Index")]
        public ActionResult Create()
        {
            var precio = new precio();
            precio.alumno = 0;
            precio.noalumno = 0;
            precio.empleado = 0;
            precio.mayorDe60 = 0;
            precio.graduado = 0;
            precio.descuentoBandaNegativa = 0;
            precio.descuentoBandaNegativaNoAlumno = 0;
            precio.descuentoUnaCuota = 0;
            precio.diasSegundoVencimiento = 0;
            precio.recargoSegundoVencimiento = 0;

            return View("Create", precio);
        }

        // POST: Arancel/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(precio precio)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (precio.gratuito)
                    {
                        precio.alumno = 0;
                        precio.noalumno = 0;
                        precio.empleado = 0;
                        precio.mayorDe60 = 0;
                        precio.descuentoBandaNegativa = 0;
                        precio.descuentoBandaNegativaNoAlumno = 0;
                        precio.descuentoUnaCuota = 0;
                        precio.diasSegundoVencimiento = 0;
                        precio.recargoSegundoVencimiento = 0;
                        precio.graduado = 0;
                        precio.sector = User.IsInRole("AdministradorEmpresas") ? (int)Sectores.Empresas : User.IsInRole("AdministradorGraduado") ? (int)Sectores.Graduados : (int)Sectores.Idiomas;

                        db.precio.Add(precio);
                        db.SaveChanges();
                        return Json(new { ok = true });
                    }
                    else
                    {
                        if (precio.empleado == 0 && precio.mayorDe60 == 0 && precio.noalumno == 0 && precio.alumno == 0 && precio.graduado == 0)
                        {
                            ViewBag.ErrorMessage = string.Format("Recuerde que alguno de los aranceles deben ser mayor a $0.00, si desea crear un curso gratuito por favor tilde la casilla.") + Environment.NewLine;
                        }

                        //if (precio.noalumno == 0)
                        //    ViewBag.ErrorMessage += string.Format("El campo No Alumno es Obligatorio.") + Environment.NewLine;

                        //if (precio.empleado == 0)
                        //    ViewBag.ErrorMessage += string.Format("El campo Empleado es Obligatorio.") + Environment.NewLine;

                        //if (precio.mayorDe60 == 0)
                        //    ViewBag.ErrorMessage += string.Format("El campo Mayor de 60 es Obligatorio.") + Environment.NewLine;

                        //if (precio.alumno == 0)
                        //    ViewBag.ErrorMessage += string.Format("El campo Alumno es Obligatorio.") + Environment.NewLine;

                        //if (precio.graduado == 0)
                        //    ViewBag.ErrorMessage += string.Format("El campo Graduado es Obligatorio.") + Environment.NewLine;

                        if (string.IsNullOrEmpty(ViewBag.ErrorMessage))
                        {
                            precio.sector = User.IsInRole("AdministradorEmpresas") ? (int)Sectores.Empresas : User.IsInRole("AdministradorGraduado") ? (int)Sectores.Graduados : (int)Sectores.Idiomas;
                            db.precio.Add(precio);
                            db.SaveChanges();
                            return Json(new { ok = true });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return View();
            }

            return View(precio);


        }

        // GET: Arancel/Edit/5
        [HasPermission("Admin_Index")]
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

        // POST: Arancel/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(precio precio)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    precio.sector = User.IsInRole("AdministradorEmpresas") ? (int)Sectores.Empresas : User.IsInRole("AdministradorGraduado") ? (int)Sectores.Graduados : (int)Sectores.Idiomas;
                    db.Entry(precio).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { ok = true });
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return View();
            }

            return View(precio);
        }

        // GET: Arancel/Delete/5
        [HasPermission("Admin_Index")]
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

        // POST: Arancel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var cursos = db.curso.Where(x => x.concepto.Equals(id)).ToList();
            if (cursos.Count == 0)
            {
                precio precio = db.precio.Find(id);
                db.precio.Remove(precio);
                db.SaveChanges();
            }
            else
            {
                ViewBag.Mensaje = "El concepto que desea eliminar tiene Cursos asociado.";
            }

            return View("Index", db.precio.ToList());
            //return RedirectToAction("Index");
        }

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
