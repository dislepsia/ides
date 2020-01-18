using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using cnrl.Helpers;
using cnrl.Logica;

namespace cnrl.Controllers
{
    public class periodolectivoesController : Controller
    {
        private socioculturalesEntities db = new socioculturalesEntities();

        // GET: periodolectivoes
        [HasPermission("Admin_Index")]
        public ActionResult Index()
        {
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


            try
            {
                return View();
            }
            catch (Exception)
            {
                return View();
            }
        }

        // GET: periodolectivoes/Details/5
        [HasPermission("Admin_Index")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            periodolectivo periodolectivo = db.periodolectivo.Find(id);
            if (periodolectivo == null)
            {
                return HttpNotFound();
            }
            return View(periodolectivo);
        }

        // GET: periodolectivoes/Create
        [HasPermission("Admin_Index")]
        public ActionResult Create()
        {
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


            return View("Create");
        }

        // POST: periodolectivoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(periodolectivo periodolectivo)
        {
            TimeSpan inicioI = new TimeSpan(09, 00, 00);
            TimeSpan finI = new TimeSpan(18, 00, 00);
            TimeSpan inicioP = new TimeSpan(08, 00, 00);
            TimeSpan finP = new TimeSpan(23, 00, 00);

            if (periodolectivo.tipoCurso.Equals(0) || periodolectivo.tipoCurso.Equals(-1))
                ViewBag.ErrorMessage += string.Format("Debe seleccionar un Tipo de Curso.") + Environment.NewLine;

            if (periodolectivo.desdeFecha > periodolectivo.hastaFecha)
                ViewBag.ErrorMessage += string.Format("La fecha de inicio del periodo lectivo es superior a la de fin del mismo.") + Environment.NewLine;

            if (periodolectivo.fechaInscripcionDesde > periodolectivo.fechaInscripcionHasta)
                ViewBag.ErrorMessage += string.Format("La fecha de inicio de la inscripción es superior a la de fin de la misma.") + Environment.NewLine;

            //if (periodolectivo.fechaPrimeraCuota > periodolectivo.desdeFecha)
            //    ViewBag.ErrorMessage += string.Format("La fecha de la primera cuota del periodo lectivo es superior a la de inicio del curso.") + Environment.NewLine;

            if (periodolectivo.fechaPrimeraCuota < periodolectivo.fechaInscripcionDesde)
                ViewBag.ErrorMessage += string.Format("La fecha de la primera cuota del periodo lectivo es inferior a la de inscripcion.") + Environment.NewLine;

            //if (periodolectivo.fechaInscripcionHasta > periodolectivo.desdeFecha)
            //    ViewBag.ErrorMessage += string.Format("La fecha de fin de la inscripcion es superior a la de inicio del periodo lectivo.") + Environment.NewLine;

            //if (periodolectivo.inscripcionHoraDesde >= periodolectivo.inscripcionHoraHasta)
            //    ViewBag.ErrorMessage += string.Format("La hora de inicio de la inscripción es mayor al horario de cierre de la inscripción.") + Environment.NewLine;

            //if (periodolectivo.periodoHoraDesde >= periodolectivo.periodoHoraHasta)
            //    ViewBag.ErrorMessage += string.Format("La hora de inicio del periodo es mayor al horario de cierre del mismo.") + Environment.NewLine;

            //if (periodolectivo.inscripcionHoraDesde < inicioI)
            //    ViewBag.ErrorMessage += string.Format("La hora de inicio de la inscripción al curso es menor al horario de apertura de la inscripcion.") + Environment.NewLine;

            //if (periodolectivo.inscripcionHoraHasta > finI)
            //    ViewBag.ErrorMessage += string.Format("La hora de fin de la inscripción al curso es mayor al horario de cierre de la misma.") + Environment.NewLine;

            //if (periodolectivo.periodoHoraDesde < inicioP)
            //    ViewBag.ErrorMessage += string.Format("La hora de inicio del periodo lectivo es menor al horario de apertura de la universidad.") + Environment.NewLine;

            //if (periodolectivo.periodoHoraHasta > finP)
            //    ViewBag.ErrorMessage += string.Format("'La hora de fin del periodo lectivo es mayor al horario de cierre de la universidad.") + Environment.NewLine;

            if (ModelState.IsValid && string.IsNullOrEmpty(ViewBag.ErrorMessage))
            {


                db.periodolectivo.Add(periodolectivo);
                db.SaveChanges();
                return Json(new { ok = true });
            }

            //PrepararViewBag();
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
            return PartialView(periodolectivo);
        }

        // GET: periodolectivoes/Edit/5
        [HasPermission("Admin_Index")]
        public ActionResult Edit(int? id)
        {
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

            periodolectivo periodolectivo = db.periodolectivo.Find(id);

            if (periodolectivo == null)
            {
                return HttpNotFound();
            }

            return View("Edit", periodolectivo);

            //periodolectivo periodolectivo = db.periodolectivo.Find(id);
            //if (periodolectivo == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(periodolectivo);
        }

        // POST: periodolectivoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(periodolectivo periodolectivo)
        {
            try
            {
                TimeSpan inicioI = new TimeSpan(09, 00, 00);
                TimeSpan finI = new TimeSpan(18, 00, 00);
                TimeSpan inicioP = new TimeSpan(08, 00, 00);
                TimeSpan finP = new TimeSpan(23, 00, 00);

                if (periodolectivo.tipoCurso.Equals(0) || periodolectivo.tipoCurso.Equals(-1))
                    ViewBag.ErrorMessage += string.Format("Debe seleccionar un Tipo de Curso.") + Environment.NewLine;

                if (periodolectivo.desdeFecha > periodolectivo.hastaFecha)
                    ViewBag.ErrorMessage += string.Format("La fecha de inicio del periodo lectivo es superior a la de fin del mismo.") + Environment.NewLine;

                if (periodolectivo.fechaInscripcionDesde > periodolectivo.fechaInscripcionHasta)
                    ViewBag.ErrorMessage += string.Format("La fecha de inicio de la inscripción es superior a la de fin de la misma.") + Environment.NewLine;

                //if (periodolectivo.fechaPrimeraCuota > periodolectivo.desdeFecha)
                //    ViewBag.ErrorMessage += string.Format("La fecha de la primera cuota del periodo lectivo es superior a la de inicio del curso.") + Environment.NewLine;

                if (periodolectivo.fechaPrimeraCuota < periodolectivo.fechaInscripcionDesde)
                    ViewBag.ErrorMessage += string.Format("La fecha de la primera cuota del periodo lectivo es inferior a la de inscripcion.") + Environment.NewLine;

                //if (periodolectivo.fechaInscripcionHasta > periodolectivo.desdeFecha)
                //    ViewBag.ErrorMessage += string.Format("La fecha de fin de la inscripcion es superior a la de inicio del periodo lectivo.") + Environment.NewLine;

                //if (periodolectivo.inscripcionHoraDesde >= periodolectivo.inscripcionHoraHasta)
                //    ViewBag.ErrorMessage += string.Format("La hora de inicio de la inscripción es mayor al horario de cierre de la inscripción.") + Environment.NewLine;

                //if (periodolectivo.periodoHoraDesde >= periodolectivo.periodoHoraHasta)
                //    ViewBag.ErrorMessage += string.Format("La hora de inicio del periodo es mayor al horario de cierre del mismo.") + Environment.NewLine;

                //if (periodolectivo.inscripcionHoraDesde < inicioI)
                //    ViewBag.ErrorMessage += string.Format("La hora de inicio de la inscripción al curso es menor al horario de apertura de la inscripcion.") + Environment.NewLine;

                //if (periodolectivo.inscripcionHoraHasta > finI)
                //    ViewBag.ErrorMessage += string.Format("La hora de fin de la inscripción al curso es mayor al horario de cierre de la misma.") + Environment.NewLine;

                //if (periodolectivo.periodoHoraDesde < inicioP)
                //    ViewBag.ErrorMessage += string.Format("La hora de inicio del periodo lectivo es menor al horario de apertura de la universidad.") + Environment.NewLine;

                //if (periodolectivo.periodoHoraHasta > finP)
                //    ViewBag.ErrorMessage += string.Format("'La hora de fin del periodo lectivo es mayor al horario de cierre de la universidad.") + Environment.NewLine;

                if (ModelState.IsValid && string.IsNullOrEmpty(ViewBag.ErrorMessage))
                {
                    db.Entry(periodolectivo).State = EntityState.Modified;
                    if (db.SaveChanges() > 0)
                        return Json(new { ok = true });
                    else
                        return Json(new { error = true });
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                return View();
            }

            //PrepararViewBag();
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
            return PartialView(periodolectivo);
        }

        // GET: periodolectivoes/Delete/5
        [HasPermission("Admin_Index")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            periodolectivo periodolectivo = db.periodolectivo.Find(id);
            if (periodolectivo == null)
            {
                return HttpNotFound();
            }

            return View(periodolectivo);
        }

        // POST: periodolectivoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            periodolectivo periodolectivo = db.periodolectivo.Find(id);

            List<oferta> chequeo = db.oferta.Where(m => m.periodoLectivo == id).ToList();

            if (chequeo.Count.Equals(0))
            {
                db.periodolectivo.Remove(periodolectivo);

                if (db.SaveChanges() > 0)
                    return Json(new { ok = true });
            }

            return Json(new { error = true });
        }

        #region ajax
        public JsonResult GetPeriodosLectivos(int? IdCurso = null)
        {
            var curso = db.curso.Find(IdCurso.Value);
            int idTipoCurso = curso.tipoCurso;
            var periodos = new SelectList(
                            db.periodolectivo.Where(x => x.tipoCurso == idTipoCurso).OrderByDescending(x => x.inscripcionHoraHasta).ToList()
                            .ToSelectList(
                                x => x.anio + " - " + x.periodo + "(" + x.tipocurso1.descripcion + ")",
                                x => x.codigo.ToString(),
                                Strings.SeleccionarPeriodoLectivo,
                                Constantes.ERROR.ToString(),
                                Constantes.ERROR.ToString()
                            )
                        , "Value", "Text");

            return Json(periodos, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPeriodoLectivo(int IdPeriodo = -1)
        {
            var periodo = db.periodolectivo.Find(IdPeriodo);
            var resultado = new Dictionary<String, String>();
            if (periodo != null)
            {
                resultado.Add("fechaDesde", periodo.desdeFecha.ToShortDateString());
                resultado.Add("fechaHasta", periodo.hastaFecha.ToShortDateString());
                resultado.Add("inscripcionDesde", (periodo.fechaInscripcionDesde.HasValue) ? periodo.fechaInscripcionDesde.Value.ToShortDateString() : DateTime.Now.ToShortDateString());
                resultado.Add("inscripcionHasta", (periodo.fechaInscripcionHasta.HasValue) ? periodo.fechaInscripcionHasta.Value.ToShortDateString() : DateTime.Now.ToShortDateString());
                resultado.Add("fechaPrimerCuota", (periodo.fechaPrimeraCuota.HasValue) ? periodo.fechaPrimeraCuota.Value.ToShortDateString() : DateTime.Now.ToShortDateString());
                //Hora del periodo lectivo e inscripcion
                resultado.Add("horaInscripcionDesde", (periodo.inscripcionHoraDesde.HasValue) ? periodo.inscripcionHoraDesde.Value.ToString(@"hh\:mm") : DateTime.Now.ToString("hh:mm"));
                resultado.Add("horaInscripcionHasta", (periodo.inscripcionHoraHasta.HasValue) ? periodo.inscripcionHoraHasta.Value.ToString(@"hh\:mm") : DateTime.Now.ToString("hh:mm"));
                resultado.Add("horaDesde", (periodo.periodoHoraDesde.HasValue) ? periodo.periodoHoraDesde.Value.ToString(@"hh\:mm") : DateTime.Now.ToString("hh:mm"));
                resultado.Add("horaHasta", (periodo.periodoHoraHasta.HasValue) ? periodo.periodoHoraHasta.Value.ToString(@"hh\:mm") : DateTime.Now.ToString("hh:mm"));
                //
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


        [HasPermission("Admin_Index")]
        public ActionResult PartialGrid(int? IdTipoCurso = null, int? IdPeriodo = null, int? IdAnio = null)
        {
            IQueryable<periodolectivo> periodosLectivos = db.periodolectivo;
            if (User.IsInRole("AdministradorEmpresas"))
            {
                periodosLectivos = periodosLectivos.Where(x => x.tipocurso1.sector == (int)Sectores.Empresas);
            }
            else
            {
                if (User.IsInRole("AdministradorGraduado"))
                {
                    periodosLectivos = periodosLectivos.Where(x => x.tipocurso1.sector == (int)Sectores.Graduados);
                }
                else
                {
                    periodosLectivos = periodosLectivos.Where(x => x.tipocurso1.sector == (int)Sectores.Idiomas);
                }
            }

            if (IdTipoCurso.HasValue)
            {
                if (IdTipoCurso.Value != Constantes.ERROR)
                {
                    var tipo = IdTipoCurso.Value;
                    periodosLectivos = periodosLectivos.Where(c => c.tipoCurso.Equals(tipo));
                }
            }

            if (IdAnio.HasValue)
                if (IdAnio.Value != Constantes.ERROR)
                {
                    var anio = IdAnio.Value;
                    periodosLectivos = periodosLectivos.Where(x => x.anio.Equals(anio));
                }

            if (IdPeriodo.HasValue)
                if (IdPeriodo.Value != Constantes.ERROR)
                {
                    // var periodo = IdPeriodo.Value;
                    periodosLectivos = periodosLectivos.Where(x => x.periodo.Equals(IdPeriodo.Value));
                }

            return PartialView("_Grid", periodosLectivos.ToList());
        }

        private void PrepararViewBag(bool filtro = false)
        {
            ViewBag.tiposCurso = new SelectList(
                db.tipocurso.Where(x => /*!x.codigo.Equals(23)*/x.sector == (int)Sectores.Idiomas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosTiposCurso : Strings.SeleccionarTipoCurso,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            //Periodo
            var itemsPeriodo = (from a in db.periodolectivo where /*!a.tipocurso1.codigo.Equals(23)*/a.tipocurso1.sector == (int)Sectores.Idiomas select a.periodo).Distinct();

            List<SelectListItem> listaPeriodos = new List<SelectListItem>();
            listaPeriodos.Add(new SelectListItem() { Value = null, Text = " ", Selected = false });

            foreach (var per in itemsPeriodo)
                listaPeriodos.Add(new
                SelectListItem()
                {
                    Value = per.ToString(),
                    Text = per.ToString(),
                    Selected = true
                });

            ViewBag.periodos = new SelectList(listaPeriodos, "Value", "Text");

            //Año
            var itemsAnio = (from a in db.periodolectivo where /*!a.tipocurso1.codigo.Equals(23)*/a.tipocurso1.sector == (int)Sectores.Idiomas select a.anio).Distinct();

            List<SelectListItem> listaAnios = new List<SelectListItem>();
            listaAnios.Add(new SelectListItem() { Value = null, Text = " ", Selected = false });

            foreach (var per in itemsAnio)
                listaAnios.Add(new
                SelectListItem()
                {
                    Value = per.ToString(),
                    Text = per.ToString(),
                    Selected = true
                });

            ViewBag.anios = new SelectList(listaAnios, "Value", "Text");
        }

        private void PrepararViewBagEmpresa(bool filtro = false)
        {
            ViewBag.tiposCurso = new SelectList(
                db.tipocurso.Where(x => /*x.codigo.Equals(23)*/x.sector == (int)Sectores.Empresas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosTiposCurso : Strings.SeleccionarTipoCurso,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            //Periodo
            var itemsPeriodo = (from a in db.periodolectivo where /*a.tipocurso1.codigo.Equals(23)*/ a.tipocurso1.sector == (int)Sectores.Empresas select a.periodo).Distinct();

            List<SelectListItem> listaPeriodos = new List<SelectListItem>();
            listaPeriodos.Add(new SelectListItem() { Value = null, Text = " ", Selected = false });

            foreach (var per in itemsPeriodo)
                listaPeriodos.Add(new
                SelectListItem()
                {
                    Value = per.ToString(),
                    Text = per.ToString(),
                    Selected = true
                });

            ViewBag.periodos = new SelectList(listaPeriodos, "Value", "Text");

            //Año
            var itemsAnio = (from a in db.periodolectivo where /*a.tipocurso1.codigo.Equals(23)*/a.tipocurso1.sector == (int)Sectores.Empresas select a.anio).Distinct();

            List<SelectListItem> listaAnios = new List<SelectListItem>();
            listaAnios.Add(new SelectListItem() { Value = null, Text = " ", Selected = false });

            foreach (var per in itemsAnio)
                listaAnios.Add(new
                SelectListItem()
                {
                    Value = per.ToString(),
                    Text = per.ToString(),
                    Selected = true
                });

            ViewBag.anios = new SelectList(listaAnios, "Value", "Text");
        }

        private void PrepararViewBagGraduado(bool filtro = false)
        {
            ViewBag.tiposCurso = new SelectList(
                db.tipocurso.Where(x => x.sector == (int)Sectores.Graduados).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosTiposCurso : Strings.SeleccionarTipoCurso,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            //Periodo
            var itemsPeriodo = (from a in db.periodolectivo where a.tipocurso1.sector == (int)Sectores.Graduados select a.periodo).Distinct();

            List<SelectListItem> listaPeriodos = new List<SelectListItem>();
            listaPeriodos.Add(new SelectListItem() { Value = null, Text = " ", Selected = false });

            foreach (var per in itemsPeriodo)
                listaPeriodos.Add(new
                SelectListItem()
                {
                    Value = per.ToString(),
                    Text = per.ToString(),
                    Selected = true
                });

            ViewBag.periodos = new SelectList(listaPeriodos, "Value", "Text");

            //Año
            var itemsAnio = (from a in db.periodolectivo where a.tipocurso1.sector == (int)Sectores.Graduados select a.anio).Distinct();

            List<SelectListItem> listaAnios = new List<SelectListItem>();
            listaAnios.Add(new SelectListItem() { Value = null, Text = " ", Selected = false });

            foreach (var per in itemsAnio)
                listaAnios.Add(new
                SelectListItem()
                {
                    Value = per.ToString(),
                    Text = per.ToString(),
                    Selected = true
                });

            ViewBag.anios = new SelectList(listaAnios, "Value", "Text");
        }
    }
}
