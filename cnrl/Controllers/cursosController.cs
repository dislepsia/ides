using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cnrl;
using cnrl.Models;
using cnrl.Helpers;
using cnrl.Logica;

namespace cnrl.Controllers
{
    public class cursosController : Controller
    {
        private socioculturalesEntities db = new socioculturalesEntities();

        #region GridFiltro
        // GET: cursos
        //[HasPermission("prueba")]
        [HasPermission("Cursos_Index")]
        public ActionResult Index()
        {
            //var curso = db.curso.Include(c => c.precio).Include(c => c.tipocurso1);
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

            return View();
        }

        private void PrepararViewBag()
        {
            ViewBag.tiposCurso = new SelectList(
                db.tipocurso.Where(x => x.sector == (int)Sectores.Idiomas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosTiposCurso,
                    "",//Constantes.ERROR.ToString(),
                    ""//Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.conceptos = new SelectList(
                db.precio.Where(x => x.sector == (int)Sectores.Idiomas).ToList()
                .ToSelectList(
                    x => x.concepto + " - " + x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosLosConceptos,
                    "",
                    ""
                )
            , "Value", "Text");

        }

        private void PrepararViewBagEmpresa()
        {
            ViewBag.tiposCurso = new SelectList(
                db.tipocurso.Where(x => /*x.codigo.Equals(23)*/x.sector == (int)Sectores.Empresas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosTiposCurso,
                    "",//Constantes.ERROR.ToString(),
                    ""//Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.conceptos = new SelectList(
                db.precio.Where(x => x.sector == (int)Sectores.Empresas).ToList()
                .ToSelectList(
                    x => x.concepto + " - " + x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosLosConceptos,
                    "",
                    ""
                )
            , "Value", "Text");

        }

        private void PrepararViewBagGraduado()
        {
            ViewBag.tiposCurso = new SelectList(
                db.tipocurso.Where(x => x.sector == (int)Sectores.Graduados).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosTiposCurso,
                    "",//Constantes.ERROR.ToString(),
                    ""//Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.conceptos = new SelectList(
                db.precio.Where(x => x.sector == (int)Sectores.Graduados).ToList()
                .ToSelectList(
                    x => x.concepto + " - " + x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosLosConceptos,
                    "",
                    ""
                )
            , "Value", "Text");

        }

        [HasPermission("Cursos_Index")]
        public ActionResult PartialGrid(int? IdTipoCurso = null, int? IdConcepto = null, string texto = "")
        {
            IQueryable<curso> cursos = db.curso;

            if (User.IsInRole("AdministradorEmpresas"))
            {
                cursos = cursos.Where(x => /*x.tipocurso1.codigo.Equals(23)*/x.tipocurso1.sector == (int)Sectores.Empresas);
            }
            else
            {
                if (User.IsInRole("AdministradorGraduado"))
                {
                    cursos = cursos.Where(x => /*x.tipocurso1.codigo.Equals(23)*/x.tipocurso1.sector == (int)Sectores.Graduados);
                }
                else
                {
                    cursos = cursos.Where(x => /*!x.tipocurso1.codigo.Equals(23)*/x.tipocurso1.sector == (int)Sectores.Idiomas);
                }

            }

            if (IdTipoCurso.HasValue && IdTipoCurso.Value != Constantes.ERROR)
            {
                var tipo = IdTipoCurso.Value;
                cursos = cursos.Where(c => c.tipoCurso.Equals(tipo));
            }
            if (IdConcepto.HasValue && IdConcepto.Value != Constantes.ERROR)
            {
                var con = IdConcepto.Value;
                cursos = cursos.Where(c => c.concepto.Equals(con));
            }

            if (!String.IsNullOrEmpty(texto))
            {
                cursos = cursos.Where(u => u.codCurso.ToString().Contains(texto) || u.descripcion.ToString().ToUpper().Contains(texto.ToUpper()));
            }

            return PartialView("_Grid", cursos.ToList());
        }
        #endregion

        #region ABM
        // GET: cursos/Details/5
        [HasPermission("Cursos_Index")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string callback = Request.Params["callback"];
            if (string.IsNullOrEmpty(callback))
                callback = "AjaxOk";

            ViewBag.modo = "View";
            ViewBag.Callback = callback;

            curso curso = db.curso.Find(id);
            if (curso == null)
            {
                return HttpNotFound();
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
            return View("Curso", curso);

        }

        // GET: cursos/Create
        [HasPermission("Cursos_ABM")]
        public ActionResult Create()
        {
            string callback = Request.Params["callback"];
            if (string.IsNullOrEmpty(callback))
                callback = "AjaxOk";
            //var curso = new curso();
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

            return View("Curso");
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "codigo,descripcion,horasSemanales,concepto,tipoCurso,nivel,extraInfo,codCurso,RequierePreInscripcion,documentacion,fechaDocumentacion,horaDocumentacion")] curso curso)
        {
            try
            {
                string usuario = ApplicationUser.GetId();

                ModelState.Remove("codigo");
                if (ModelState.IsValid)
                {
                    curso.AdminCurso = usuario;

                    if (curso.RequierePreInscripcion == false)
                    {
                        db.curso.Add(curso);

                        db.SaveChanges();
                        return Json(new { ok = true });
                    }
                    else
                    {
                        if (curso.fechaDocumentacion.ToString() == "" || curso.documentacion == null)
                        {
                            ModelState.AddModelError(string.Empty, "El campo Fecha Documentación y/o Documentación no deben quedar vacío.");
                        }
                        else
                        {
                            db.curso.Add(curso);
                            db.SaveChanges();
                            return Json(new { ok = true });
                        }

                    }

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
            return PartialView("Curso", curso);
        }

        // GET: cursos/Edit/5
        [HasPermission("Cursos_ABM")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string callback = Request.Params["callback"];
            if (string.IsNullOrEmpty(callback))
                callback = "AjaxOk";

            ViewBag.modo = "Edit";
            ViewBag.Callback = callback;

            curso curso = db.curso.Find(id);
            if (curso == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdCurso = curso.codigo;
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
            return View("Curso", curso);
        }

        [HttpPost]
        public ActionResult Edit(curso curso)
        {
            try
            {
                string usuario = ApplicationUser.GetId();

                if (ModelState.IsValid)
                {
                    curso.AdminCurso = usuario;

                    if (curso.RequierePreInscripcion == false)
                    {
                        db.Entry(curso).State = EntityState.Modified;
                        db.SaveChanges();
                        return Json(new { ok = true });
                    }
                    else
                    {
                        if (curso.fechaDocumentacion.ToString() == "" || curso.documentacion == null)
                        {
                            ModelState.AddModelError(string.Empty, "El campo Fecha Documentación y/o Documentación no deben quedar vacío.");
                        }
                        else
                        {
                            db.Entry(curso).State = EntityState.Modified;
                            db.SaveChanges();
                            return Json(new { ok = true });
                        }

                    }
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
            PrepararViewBag();
            return View("Curso", curso);
        }

        // GET: cursos/Delete/5
        [HasPermission("Cursos_ABM")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string callback = Request.Params["callback"];
            if (string.IsNullOrEmpty(callback))
                callback = "AjaxOk";
            ViewBag.Callback = callback;
            ViewBag.modo = "Delete";

            curso curso = db.curso.Find(id);
            if (curso == null)
            {
                return HttpNotFound();
            }
            PrepararViewBag();

            return View("Curso", curso);
        }

        [HttpPost]
        public ActionResult Delete([Bind(Include = "codigo,descripcion,horasSemanales,concepto,tipoCurso,nivel,extraInfo,codCurso")] curso curso)
        {
            try
            {
                curso = db.curso.Find(curso.codigo);
                var correlativas = curso.curso1.ToList();
                foreach (var correlativa in correlativas)
                {
                    curso.curso1.Remove(correlativa);
                }

                db.curso.Remove(curso);
                db.SaveChanges();
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
            PrepararViewBag();
            return PartialView("Curso", curso);
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



        #region ajax
        public JsonResult GetCursos(int? IdSede = null, int? IdPeriodo = null, string IdUsuario = "", int? IdTipoCurso = null, int? IdConcepto = null)
        {
            IQueryable<curso> cursos = db.curso;

            //            var ofertas = db.oferta;

            if (IdSede.HasValue && IdSede.Value != Constantes.ERROR)
                cursos = cursos.Where(x => x.oferta.Any(o => o.sede == IdSede.Value));

            if (IdPeriodo.HasValue && IdPeriodo.Value != Constantes.ERROR)
                cursos = cursos.Where(x => x.oferta.Any(o => o.periodoLectivo == IdPeriodo.Value));

            if (!string.IsNullOrEmpty(IdUsuario) && IdUsuario != Constantes.ERROR.ToString())
                cursos = cursos.Where(x => x.oferta.Any(o => o.cursa.Any(c => c.alumno == IdUsuario)));

            if (IdTipoCurso.HasValue && IdTipoCurso.Value != Constantes.ERROR)
                cursos = cursos.Where(x => x.tipoCurso == IdTipoCurso.Value);

            if (IdConcepto.HasValue && IdConcepto.Value != Constantes.ERROR)
                cursos = cursos.Where(x => x.concepto == IdConcepto.Value);



            var listaCursos = cursos.ToList();

            var cursosSelect = new SelectList(
                listaCursos
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosLosCursos,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            return Json(cursosSelect, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ControlGratuito(int codigoCurso)
        {
            var curso = db.curso.Find(codigoCurso);

            if (curso.gratuito == true)
                return Json(new { Gratuito = true });
            else
                return Json(new { Gratuito = false });
        }

        [HttpGet]
        public JsonResult BuscarPosiblesCorrelativas(int Id, string q)
        {
            IQueryable<curso> cursos = db.curso;

            var curso = db.curso.Find(Id);
            cursos = cursos.Where(x => x.codigo != Id && x.tipoCurso == curso.tipoCurso);
            cursos = cursos.Where(x => x.tipocurso1.descripcion.Contains(q) || x.descripcion.Contains(q) || x.codCurso.ToString().Contains(q));

            //  var institutos = InstitutoData.Buscar(q);
            var rta = cursos.Select(x => new { id = x.codigo, text = x.codCurso + " - " + x.descripcion }).ToList();


            return Json(rta, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDocumentacionCurso(int IdCurso = -1)
        {
            var curso = db.curso.Find(IdCurso);
            var resultado = new Dictionary<String, String>();
            if (curso != null)
            {
                resultado.Add("RequierePreInscripcion", curso.RequierePreInscripcion.ToString());
                if (curso.documentacion == null)
                {
                    resultado.Add("documentacion", "");
                }
                else
                {
                    resultado.Add("documentacion", curso.documentacion.ToString());
                }

                if (curso.fechaDocumentacion == null)
                {
                    resultado.Add("fechaDocumentacion", "");
                }
                else
                {
                    resultado.Add("fechaDocumentacion", curso.fechaDocumentacion.ToString());
                }

                if (curso.horaDocumentacion == null)
                {
                    resultado.Add("horaDocumentacion", "");
                }
                else
                {
                    resultado.Add("horaDocumentacion", curso.horaDocumentacion.ToString());
                }



            }
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Buscar(string q)
        {
            IQueryable<curso> cursos = db.curso;

            cursos = cursos.Where(x => x.descripcion.Contains(q) || x.codCurso.Equals(q));

            //  var institutos = InstitutoData.Buscar(q);
            var rta = cursos.Select(x => new { id = x.codCurso, text = x.descripcion }).ToList();

            return Json(rta, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [ChildActionOnly]
        public ActionResult _Buscador(string SelectController, string SelectAction)
        {
            PrepararViewBag();
            ViewBag.SelectController = SelectController;
            ViewBag.SelectAction = SelectAction;
            return PartialView("_Buscador");
        }


        #region correlativas

        public ActionResult PartialGridCorrelativas(int? IdCurso = null)
        {


            List<curso> correlativas = new List<curso>();
            if (IdCurso.HasValue && IdCurso.Value != Constantes.ERROR)
            {
                var tipo = IdCurso.Value;
                curso curso = db.curso.Find(IdCurso);
                correlativas = curso.curso1.ToList();
            }
            ViewBag.IdCurso = IdCurso;

            return PartialView("_GridCorrelativas", correlativas);
        }

        [HasPermission("Cursos_Index")]
        public ActionResult CreateCorrelativa(int? id)
        {
            //if (string.IsNullOrEmpty(callback))
            //    callback = "AjaxOk2";
            var curso = db.curso.Find(id);
            ViewBag.modo = "CreateCorrelativa";
            ViewBag.Callback = "AjaxOk2";

            //PrepararViewBagCorrelativas();

            return View("Correlativa", curso);
        }

        [HttpPost]
        public ActionResult CreateCorrelativa(curso curso)
        {
            try
            {
                if (curso != null && curso.codCurso != null)
                {
                    var cur = db.curso.Find(curso.codigo);
                    cur.curso1.Add(db.curso.Find(curso.codCurso));
                    db.SaveChanges();
                    return Json(new { ok = true });
                }
                //           return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                ViewBag.ErrorMessage = string.Format(
                    Strings.ErrorIntentarInsertarRegistro,
                    e.Message.Replace(Strings.TheStatementHasBeenTerminated, "")
                );
            }
            ViewBag.modo = "CreateCorrelativa";
            ViewBag.Callback = "AjaxOk2";
            PrepararViewBagCorrelativas();
            return PartialView("Correlativa", curso);
        }

        [HasPermission("Cursos_Index")]
        public ActionResult DeleteCorrelativa(int? id, int? IdCurso)
        {
            if (id == null || IdCurso == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var cursoOrigen = db.curso.Find(IdCurso);
                cursoOrigen.curso1.Remove(db.curso.Find(id));
                db.SaveChanges();

                ViewBag.IdCurso = cursoOrigen.codigo;
                PrepararViewBagCorrelativas();
                return View("Correlativas", cursoOrigen);
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                ViewBag.ErrorMessage = string.Format(Strings.ErrorEliminarRegistroPorIntegridad);
            }




            //string callback = Request.Params["callback"];
            //if (string.IsNullOrEmpty(callback))
            //    callback = "AjaxOk2";
            ViewBag.Callback = "AjaxOk";
            ViewBag.modo = "DeleteCorrelativa";
            ViewBag.IdCurso = IdCurso;
            PrepararViewBagCorrelativas(id.ToString());
            return View("Correlativa", new curso());
        }


        [HasPermission("Cursos_Index")]
        public ActionResult Correlativas(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            curso curso = db.curso.Find(id);
            if (curso == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdCurso = curso.codigo;
            //PrepararViewBagCorrelativas();
            return View("Correlativas", curso);
        }

        private void PrepararViewBagCorrelativas(string IdCorrelativa = "")
        {
            ViewBag.codCurso = new SelectList(
                db.curso.ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.SeleccionarCorrelativa,
                    IdCorrelativa,//Constantes.ERROR.ToString(),
                    IdCorrelativa//Constantes.ERROR.ToString()
                )
            , "Value", "Text");
        }
        #endregion
    }
}
