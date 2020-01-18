using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using cnrl.Models;
using DevExpress.Web.Mvc;
using cnrl.Repositories;
using cnrl.Logica;
using System.Data.Common;
using cnrl.Helpers;
using DevExpress.Export;
using System.Web.UI.WebControls;

namespace cnrl.Controllers
{
    public class DocenteController : Controller
    {
        private socioculturalesEntities db = new socioculturalesEntities();

        private static string fecha = "";
        private static int codOferta;

        // GET: Docente
        public ActionResult Index()
        {
            var aspNetUsers = db.AspNetUsers.Include(a => a.localidad1).Include(a => a.tipodocumento1).Include(a => a.sede1);
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
            return View(aspNetUsers.ToList());
        }

        // GET: Docente/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            if (aspNetUsers == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUsers);
        }

        // GET: Docente/Create
        public ActionResult Create()
        {
            ViewBag.localidad = new SelectList(db.localidad, "codigo", "localidad1");
            ViewBag.TipoDocumento = new SelectList(db.tipodocumento, "codigo", "descripcion");
            ViewBag.sede = new SelectList(db.sede, "codigo", "nombre");
            return View();
        }

        // POST: Docente/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,Activo,passwordInicial,NroDocumento,Apellido,Nombre,Calle,NroCalle,FechaNacimiento,Telefono,Telefono2,piso,dpto,localidad,provincia,TipoDocumento,TipoAlumno,DateCreated,DateModified,UserCreated,UserModified,sede")] AspNetUsers aspNetUsers)
        {
            if (ModelState.IsValid)
            {
                db.AspNetUsers.Add(aspNetUsers);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.localidad = new SelectList(db.localidad, "codigo", "localidad1", aspNetUsers.localidad);
            ViewBag.TipoDocumento = new SelectList(db.tipodocumento, "codigo", "descripcion", aspNetUsers.TipoDocumento);
            ViewBag.sede = new SelectList(db.sede, "codigo", "nombre", aspNetUsers.sede);
            return View(aspNetUsers);
        }

        // GET: Docente/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            if (aspNetUsers == null)
            {
                return HttpNotFound();
            }
            ViewBag.localidad = new SelectList(db.localidad, "codigo", "localidad1", aspNetUsers.localidad);
            ViewBag.TipoDocumento = new SelectList(db.tipodocumento, "codigo", "descripcion", aspNetUsers.TipoDocumento);
            ViewBag.sede = new SelectList(db.sede, "codigo", "nombre", aspNetUsers.sede);
            return View(aspNetUsers);
        }

        // POST: Docente/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,Activo,passwordInicial,NroDocumento,Apellido,Nombre,Calle,NroCalle,FechaNacimiento,Telefono,Telefono2,piso,dpto,localidad,provincia,TipoDocumento,TipoAlumno,DateCreated,DateModified,UserCreated,UserModified,sede")] AspNetUsers aspNetUsers)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetUsers).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.localidad = new SelectList(db.localidad, "codigo", "localidad1", aspNetUsers.localidad);
            ViewBag.TipoDocumento = new SelectList(db.tipodocumento, "codigo", "descripcion", aspNetUsers.TipoDocumento);
            ViewBag.sede = new SelectList(db.sede, "codigo", "nombre", aspNetUsers.sede);
            return View(aspNetUsers);
        }

        // GET: Docente/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            if (aspNetUsers == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUsers);
        }

        // POST: Docente/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspNetUsers);
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

        public ActionResult PartialGrid(string IdDocente = "", int? idPeriodoLectivo = null)
        {
            string profesor = ApplicationUser.GetId();
            IQueryable<oferta> misCursos = Enumerable.Empty<oferta>().AsQueryable();

            if (User.IsInRole("CoordinadorDocente"))
            {
                if (idPeriodoLectivo == null || idPeriodoLectivo == -1)
                {
                    misCursos = db.oferta.Where(m => m.coordinadorDocente.Equals(profesor));
                }
                else
                {
                    misCursos = db.oferta.Where(m => m.coordinadorDocente.Equals(profesor) && m.periodoLectivo == idPeriodoLectivo);
                }

            }
            else
            {
                if (User.IsInRole("Docente"))
                {
                    if (idPeriodoLectivo == null || idPeriodoLectivo == -1)
                    {
                        misCursos = db.oferta.Where(m => m.docente.Equals(profesor));
                    }
                    else
                    {
                        misCursos = db.oferta.Where(m => m.docente.Equals(profesor) && m.periodoLectivo == idPeriodoLectivo);
                    }

                }
                else
                {
                    if (User.IsInRole("AdministradorEmpresas"))
                    {
                        if (!String.IsNullOrEmpty(IdDocente) && IdDocente != "-1")
                        {
                            if (idPeriodoLectivo == null || idPeriodoLectivo == -1)
                            {
                                misCursos = db.oferta.Where(x => x.docente == IdDocente && /*x.curso1.tipocurso1.codigo.Equals(23)*/x.curso1.tipocurso1.sector == (int)Sectores.Empresas);
                            }
                            else
                            {
                                misCursos = db.oferta.Where(x => x.docente == IdDocente && x.curso1.tipocurso1.sector == (int)Sectores.Empresas && x.periodoLectivo == idPeriodoLectivo);
                            }

                        }
                        else
                        {
                            if (idPeriodoLectivo == null || idPeriodoLectivo == -1)
                            {
                                misCursos = db.oferta.Where(x => x.habilitada == true && x.fechaDesde.Year == DateTime.Now.Year && x.curso1.tipocurso1.sector == (int)Sectores.Empresas);
                            }
                            else
                            {
                                misCursos = db.oferta.Where(x => x.habilitada == true && x.fechaDesde.Year == DateTime.Now.Year && x.curso1.tipocurso1.sector == (int)Sectores.Empresas && x.periodoLectivo == idPeriodoLectivo);
                            }

                        }
                    }
                    else
                    {
                        if (User.IsInRole("AdministradorGraduado"))
                        {
                            if (!String.IsNullOrEmpty(IdDocente) && IdDocente != "-1")
                            {
                                if (idPeriodoLectivo == null || idPeriodoLectivo == -1)
                                {
                                    misCursos = db.oferta.Where(x => x.docente == IdDocente && x.curso1.tipocurso1.sector == (int)Sectores.Graduados);
                                }
                                else
                                {
                                    misCursos = db.oferta.Where(x => x.docente == IdDocente && x.curso1.tipocurso1.sector == (int)Sectores.Graduados && x.periodoLectivo == idPeriodoLectivo);
                                }

                            }
                            else
                            {
                                if (idPeriodoLectivo == null || idPeriodoLectivo == -1)
                                {
                                    misCursos = db.oferta.Where(x => x.habilitada == true && x.fechaDesde.Year == DateTime.Now.Year && x.curso1.tipocurso1.sector == (int)Sectores.Graduados);
                                }
                                else
                                {
                                    misCursos = db.oferta.Where(x => x.habilitada == true && x.fechaDesde.Year == DateTime.Now.Year && x.curso1.tipocurso1.sector == (int)Sectores.Graduados && x.periodoLectivo == idPeriodoLectivo);
                                }

                            }
                        }
                        else
                        {


                            if (User.IsInRole("Administrador") || User.IsInRole("SuperAdministrador"))
                            {
                                if (!String.IsNullOrEmpty(IdDocente) && IdDocente != "-1")
                                {
                                    if (idPeriodoLectivo == null || idPeriodoLectivo == -1)
                                    {
                                        misCursos = db.oferta.Where(x => x.docente == IdDocente && x.periodolectivo1.tipocurso1.sector == (int)Sectores.Idiomas);
                                    }
                                    else
                                    {
                                        misCursos = db.oferta.Where(x => x.docente == IdDocente && x.periodoLectivo == idPeriodoLectivo && x.periodolectivo1.tipocurso1.sector == (int)Sectores.Idiomas);
                                    }


                                }
                                else
                                {
                                    if (idPeriodoLectivo == null || idPeriodoLectivo == -1)
                                    {
                                        misCursos = db.oferta.Where(x => x.habilitada == true && x.fechaDesde.Year == DateTime.Now.Year && x.periodolectivo1.tipocurso1.sector == (int)Sectores.Idiomas);
                                    }
                                    else
                                    {
                                        misCursos = db.oferta.Where(x => x.habilitada == true && x.fechaDesde.Year == DateTime.Now.Year && x.periodoLectivo == idPeriodoLectivo && x.periodolectivo1.tipocurso1.sector == (int)Sectores.Idiomas);
                                    }

                                }
                                //misCursos = db.oferta;
                            }
                            else
                            {
                                misCursos = Enumerable.Empty<oferta>().AsQueryable();
                            }
                        }
                    }
                }
            }

            //Logica para saber si un curso fue validado
            //Si un alumno dentro del curso esta validado entonces todos lo estan
            List<oferta> cursosValidados = new List<oferta>();

            foreach (oferta verValidacionCurso in misCursos)
            {
                cursa auxiliar;

                try
                {
                    //auxiliar = verValidacionCurso.cursa.First();
                    //Se modifico la consulta de alumnos a los cuales se le pregunta si ya fueron validadas las notas, 
                    //debido a que en ocasiones se preguntaba por este campo a alumnos que no debian validarse
                    //generando que los cursos aparezcan como no validados
                    auxiliar = verValidacionCurso.cursa.Where(x => x.estado.Value != 5 && x.estado.Value != 6 && x.estado.Value != 3).First();


                    IQueryable<cursa> aula = db.cursa.Where(x => x.oferta == verValidacionCurso.codigo && x.ValidarNota == true && x.estado.Value != 5 && x.estado.Value != 6 && x.estado.Value != 3);

                    var total = db.cursa.Where(x => x.oferta == verValidacionCurso.codigo && x.estado.Value != 5 && x.estado.Value != 6 && x.estado.Value != 3).Count();
                    auxiliar.ValidarNota = false;
                    if (total != 0 && aula.ToList().Count == total)
                    {
                        auxiliar.ValidarNota = true;
                    }
                }
                catch (Exception e)
                {
                    auxiliar = null;
                }

                if (auxiliar != null)
                {
                    if (auxiliar.ValidarNota == true)
                        verValidacionCurso.documentacion = "Si";
                    else
                        verValidacionCurso.documentacion = "No";
                }
                else
                    verValidacionCurso.documentacion = "No";

                cursosValidados.Add(verValidacionCurso);
            }

            var listado = cursosValidados.AsQueryable();

            return PartialView("_Grid", listado.OrderBy(x => x.fechaDesde).ToList());
        }

        [HttpPost]
        public ActionResult Habilitar(int IdCursa)
        {
            ModelState.Remove("baja");

            var cursa = db.cursa.Find(IdCursa);
            cursa.aprobo = !cursa.aprobo;
            db.Configuration.ValidateOnSaveEnabled = false;
            db.SaveChanges();
            return Json(new { ok = true });
        }

        #region ImprimirActa
        public ActionResult IndexImprimirActa()
        {
            //PrepararViewBag(true);

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

        private void PrepararViewBag(bool filtro = false)
        {
            ViewBag.tiposCurso = new SelectList(
                db.tipocurso.Where(x => x.sector == (int)Sectores.Idiomas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosTiposCurso : Strings.SeleccionarTipoCurso,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");


            if (User.IsInRole("Ventanilla"))
            {
                int sede = ApplicationUser.GetSede();
                ViewBag.sedes = new SelectList(
                        db.sede.Where(x => x.borradoLogico == false && x.codigo == sede && x.sector == (int)Sectores.Idiomas).ToList()
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
                    db.sede.Where(x => x.borradoLogico == false && x.sector == (int)Sectores.Idiomas).ToList()
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
                db.curso.Where(x => /*!x.tipocurso1.codigo.Equals(23)*/x.tipocurso1.sector == (int)Sectores.Idiomas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosLosCursos : Strings.SeleccionarCurso,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.periodosLectivos = new SelectList(
                db.periodolectivo.Where(x => /*!x.tipocurso1.codigo.Equals(23)*/x.tipocurso1.sector == (int)Sectores.Idiomas).OrderByDescending(x => x.fechaInscripcionHasta).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosLosPeriodos : Strings.SeleccionarPeriodoLectivo,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");


            IList<SelectListItem> list = Enum.GetValues(typeof(Dias)).Cast<Dias>().Select(x => new SelectListItem { Text = x.ToString(), Value = ((int)x).ToString() }).ToList().ToSelectList(
                    x => x.Text,
                    x => x.Value,
                    Strings.MsgTodosLosDias,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                    );

            ViewBag.dias = new SelectList(list, "Value", "Text");

            IList<SelectListItem> listEstadosOferta = Enum.GetValues(typeof(EstadoOferta)).Cast<EstadoOferta>().Select(x => new SelectListItem { Text = x.ToString(), Value = ((int)x).ToString() }).ToList().ToSelectList(
                    x => x.Text,
                    x => x.Value,
                    Strings.MsgTodosEstados,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                    );

            ViewBag.Docentes = new SelectList(
           db.AspNetRoles.Find(((int)Roles.Docente).ToString()).AspNetUsers.ToList()
           .ToSelectList(
               x => x.descripcion,
               x => x.Id.ToString(),
               (filtro) ? Strings.MsgTodosDocentes : Strings.SeleccionarDocente,
                   Constantes.ERROR.ToString(),
                   Constantes.ERROR.ToString()
           )
           , "Value", "Text");
        }

        private void PrepararViewBagGraduado(bool filtro = false)
        {
            ViewBag.tiposCurso = new SelectList(
                db.tipocurso.Where(x => x.sector == (int)Sectores.Graduados).
                ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosTiposCurso : Strings.SeleccionarTipoCurso,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");


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


            IList<SelectListItem> list = Enum.GetValues(typeof(Dias)).Cast<Dias>().Select(x => new SelectListItem { Text = x.ToString(), Value = ((int)x).ToString() }).ToList().ToSelectList(
                    x => x.Text,
                    x => x.Value,
                    Strings.MsgTodosLosDias,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                    );

            ViewBag.dias = new SelectList(list, "Value", "Text");

            IList<SelectListItem> listEstadosOferta = Enum.GetValues(typeof(EstadoOferta)).Cast<EstadoOferta>().Select(x => new SelectListItem { Text = x.ToString(), Value = ((int)x).ToString() }).ToList().ToSelectList(
                    x => x.Text,
                    x => x.Value,
                    Strings.MsgTodosEstados,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                    );

            ViewBag.Docentes = new SelectList(
           db.AspNetRoles.Find(((int)Roles.Docente).ToString()).AspNetUsers.ToList()
           .ToSelectList(
               x => x.descripcion,
               x => x.Id.ToString(),
               (filtro) ? Strings.MsgTodosDocentes : Strings.SeleccionarDocente,
                   "",//Constantes.ERROR.ToString(),
                   ""//Constantes.ERROR.ToString()
           )
           , "Value", "Text");
        }

        private void PrepararViewBagEmpresa(bool filtro = false)
        {
            ViewBag.tiposCurso = new SelectList(
                db.tipocurso.Where(x => x.sector == (int)Sectores.Empresas).
                ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosTiposCurso : Strings.SeleccionarTipoCurso,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");


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
                db.periodolectivo.OrderByDescending(x => x.fechaInscripcionHasta).Where(x => x.tipocurso1.sector == (int)Sectores.Empresas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosLosPeriodos : Strings.SeleccionarPeriodoLectivo,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");


            IList<SelectListItem> list = Enum.GetValues(typeof(Dias)).Cast<Dias>().Select(x => new SelectListItem { Text = x.ToString(), Value = ((int)x).ToString() }).ToList().ToSelectList(
                    x => x.Text,
                    x => x.Value,
                    Strings.MsgTodosLosDias,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                    );

            ViewBag.dias = new SelectList(list, "Value", "Text");

            IList<SelectListItem> listEstadosOferta = Enum.GetValues(typeof(EstadoOferta)).Cast<EstadoOferta>().Select(x => new SelectListItem { Text = x.ToString(), Value = ((int)x).ToString() }).ToList().ToSelectList(
                    x => x.Text,
                    x => x.Value,
                    Strings.MsgTodosEstados,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                    );

            ViewBag.Docentes = new SelectList(
           db.AspNetRoles.Find(((int)Roles.Docente).ToString()).AspNetUsers.ToList()
           .ToSelectList(
               x => x.descripcion,
               x => x.Id.ToString(),
               (filtro) ? Strings.MsgTodosDocentes : Strings.SeleccionarDocente,
                   "",//Constantes.ERROR.ToString(),
                   ""//Constantes.ERROR.ToString()
           )
           , "Value", "Text");
        }

        public ActionResult PartialGridImprimir(int? IdTipoCurso = null, int? IdSede = null, int? IdCurso = null, int? IdPeriodo = null)
        {
            List<oferta> aMostrar = new List<oferta>();
            IQueryable<oferta> misCursos = null;
            if (User.IsInRole("AdministradorEmpresas"))
            {
                misCursos = db.oferta.Where(x => x.habilitada == true && (x.fechaDesde.Year >= DateTime.Now.Year - 1 && x.fechaDesde.Year <= DateTime.Now.Year) && /*x.curso1.tipocurso1.codigo.Equals(23)*/x.curso1.tipocurso1.sector == (int)Sectores.Empresas).OrderBy(x => x.curso);
            }
            else
            {
                if (User.IsInRole("AdministradorGraduado"))
                {
                    misCursos = db.oferta.Where(x => x.habilitada == true && (x.fechaDesde.Year >= DateTime.Now.Year - 1 && x.fechaDesde.Year <= DateTime.Now.Year) && x.curso1.tipocurso1.sector == (int)Sectores.Graduados).OrderBy(x => x.curso);
                }
                else
                {
                    misCursos = db.oferta.Where(x => x.habilitada == true && (x.fechaDesde.Year >= DateTime.Now.Year - 1 && x.fechaDesde.Year <= DateTime.Now.Year) && x.curso1.tipocurso1.sector == (int)Sectores.Idiomas).OrderBy(x => x.curso);
                }

            }

            bool algunFiltro = false;
            if (IdTipoCurso.HasValue && IdTipoCurso.Value != Constantes.ERROR)
            {
                var tipo = IdTipoCurso.Value;
                misCursos = misCursos.Where(x => x.curso1.tipoCurso == (tipo));
            }

            if (IdSede.HasValue && IdSede.Value != Constantes.ERROR)
            {
                var sede = IdSede.Value;
                misCursos = misCursos.Where(x => x.sede == (sede));
            }

            if (IdCurso.HasValue && IdCurso.Value != Constantes.ERROR)
            {
                var curso = IdCurso.Value;
                misCursos = misCursos.Where(x => x.curso == (curso));
                algunFiltro = true;
            }

            if (IdPeriodo.HasValue && IdPeriodo.Value != Constantes.ERROR)
            {
                var periodo = IdPeriodo.Value;
                misCursos = misCursos.Where(x => x.periodoLectivo == (periodo));
                algunFiltro = true;
            }

            foreach (oferta curso in misCursos)
            {
                IQueryable<cursa> aula = db.cursa.Where(x => x.oferta == curso.codigo && x.ValidarNota == true && x.estado.Value != 5 && x.estado.Value != 6 && x.estado.Value != 3);

                var total = db.cursa.Where(x => x.oferta == curso.codigo && x.estado.Value != 5 && x.estado.Value != 6 && x.estado.Value != 3).Count();

                if (total != 0 && aula.ToList().Count == total)
                {
                    aMostrar.Add(curso);
                }
            }
            return PartialView("_GridImprimirActa", aMostrar.ToList());
        }

        [AllowAnonymous]
        public ActionResult ImprimirActa(int codOferta)
        {
            var asistencia = db.oferta.Find(codOferta);

            try
            {
                return new Rotativa.UrlAsPdf(("http://10.10.201.29/Docente/FichaParaImprimir/" + codOferta))
                //return new Rotativa.UrlAsPdf(Url.Content("~/Docente/FichaParaImprimir/" + codOferta))
                {
                    FileName = String.Format("Acta_{0}.pdf", codOferta),
                    CustomSwitches =
                        "--footer-center \"Fecha: " +
                        DateTime.Now.Date.ToString("MM/dd/yyyy") +
                        " Pag: [page]/[toPage]\"" +
                        " --footer-line --footer-font-size \"9\"" +
                        " --footer-spacing 6 --footer-font-name \"calibri light\""
                };


            }
            catch (Exception e)
            {
                throw e;
            }
            //return FichaParaImprimir(codOferta);
        }

        [AllowAnonymous]
        public ActionResult ImprimirActaExcel(int codOferta)
        {
            List<oferta> asistencia = db.oferta.Where(x => x.codigo == codOferta).ToList();

            try
            {
                IQueryable<cursa> alumnos = db.cursa.Where(m => m.oferta.Equals(codOferta)).OrderByDescending(m => m.oferta1.periodolectivo1.anio);

                IQueryable<cursa> noVan = db.cursa.Where(m => m.oferta.Equals(codOferta) && m.DateModified < m.oferta1.fechaDesde && m.estado == (int)EstadosCursada.BajaAceptada);

                List<cursa> resul = alumnos.Except(noVan).ToList();

                alumnos = resul.AsQueryable();

                //return new Rotativa.UrlAsPdf(("http://10.10.201.29/Docente/FichaParaImprimir/" + codOferta))
                //{
                //    FileName = String.Format("Acta_{0}.pdf", codOferta),
                //    CustomSwitches =
                //        "--footer-center \"Fecha: " +
                //        DateTime.Now.Date.ToString("MM/dd/yyyy") +
                //        " Pag: [page]/[toPage]\"" +
                //        " --footer-line --footer-font-size \"9\"" +
                //        " --footer-spacing 6 --footer-font-name \"calibri light\""
                //};

                ExportSettings.DefaultExportType = ExportType.WYSIWYG;
                return GridViewExtension.ExportToXlsx(CrearGridViewSettingsExportarExcel("Acta_" + codOferta.ToString(), codOferta), alumnos, true);


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private GridViewSettings CrearGridViewSettingsExportarExcel(string titulo, int cod)
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



            IQueryable<cursa> alumnos = db.cursa.Where(m => m.oferta.Equals(cod)).OrderByDescending(m => m.oferta1.periodolectivo1.anio);

            IQueryable<cursa> noVan = db.cursa.Where(m => m.oferta.Equals(cod) && m.DateModified < m.oferta1.fechaDesde && m.estado == (int)EstadosCursada.BajaAceptada);

            List<cursa> resul = alumnos.Except(noVan).ToList();

            alumnos = resul.AsQueryable();

            //Columnas
            settings.Columns.Add(col => { col.FieldName = "oferta1.curso1.descripcion"; col.Caption = "Curso"; });
            settings.Columns.Add(col => { col.FieldName = "oferta1.horario"; col.Caption = "Horario"; });
            settings.Columns.Add(col => { col.FieldName = "AspNetUsers.tipodocumento1.descripcion"; col.Caption = "Tipo"; });
            settings.Columns.Add(col => { col.FieldName = "AspNetUsers.NroDocumento"; col.Caption = "Nro"; });



            foreach (cursa item in alumnos.Where(x => x.estado == (int)EstadosCursada.FormaPago || x.estado == (int)EstadosCursada.Confirmado || x.estado == (int)EstadosCursada.PlanGenerado || x.estado == (int)EstadosCursada.Inscripto || x.estado == (int)EstadosCursada.SolicitudBaja || x.estado == (int)EstadosCursada.BajaAceptada).OrderBy(f => f.AspNetUsers.Apellido))
            {
                if (item.EstadoCursa.descripcion == "SolicitudBaja")
                {
                    item.observacion = "Baja";
                }
                else
                {
                    if (item.EstadoCursa.descripcion == "BajaAceptada")
                    {
                        item.observacion = "Baja Aceptada";
                    }
                }

                if (item.nota != null)
                {
                    item.nota = item.nota.ToString();
                }
                else
                {
                    item.nota = "";
                }

                item.AspNetUsers.Apellido = item.AspNetUsers.Apellido.ToUpper();
                item.AspNetUsers.Nombre = item.AspNetUsers.Nombre.ToUpper();
                alumnos.First(x => x.alumno == item.alumno).observacion = item.observacion;
            }

            settings.Columns.Add(col => { col.FieldName = "AspNetUsers.Apellido"; col.Caption = "Apellido"; });
            settings.Columns.Add(col => { col.FieldName = "AspNetUsers.Nombre"; col.Caption = "Nombre"; });
            settings.Columns.Add(col => { col.FieldName = "observacion"; col.Caption = "Estado"; });
            settings.Columns.Add(col => { col.FieldName = "ResultadoCursa.descripcion"; col.Caption = "Condición"; });
            settings.Columns.Add(col => { col.FieldName = "nota"; col.Caption = "Nota"; col.PropertiesEdit.DisplayFormatString = "N"; });

            return settings;
        }

        [AllowAnonymous]
        public ActionResult FichaParaImprimir(int id)
        {
            //var asistencia = db.oferta.Where(x => x.codigo == id);
            var asistencia = db.oferta.Find(id);
            ViewBag.Imprimir = true;
            return View("Acta", asistencia);
        }
        #endregion

        #region Notas

        public ActionResult CargarNotas(string codOferta, string mensaje = "")
        {
            DocenteController.codOferta = int.Parse(codOferta);
            oferta info = db.oferta.Where(x => x.codigo.Equals(DocenteController.codOferta)).First();
            ViewBag.InfoCurso = info.descripcion.Trim();
            ViewBag.CodOferta = codOferta;

            if (User.IsInRole("CoordinadorDocente") || User.IsInRole("Administrador") || User.IsInRole("SuperAdministrador") || User.IsInRole("AdministradorGraduado"))
            {
                ViewBag.Coordinador = true;
            }
            else
            {
                ViewBag.Coordinador = false;
            }

            //if (!string.IsNullOrEmpty(error))
            //{
            //    ViewBag.ErrorMessage = error;
            //}
            if (!string.IsNullOrEmpty(mensaje))
            {
                if (mensaje.Contains("Error: ") || mensaje.Contains("Recordar"))
                    ViewBag.MensajeError = mensaje;
                else
                    ViewBag.MensajeExito = mensaje;
            }

            return View("IndexAlumnoCurso");
        }

        public ActionResult PartialGridCursadas(string codOferta)
        {
            if (codOferta == null && DocenteController.codOferta == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.CodOferta = DocenteController.codOferta;

            int oferta = Convert.ToInt32(DocenteController.codOferta);
            IQueryable<cursa> alumnos = db.cursa.Where(m => m.oferta.Equals(oferta) && m.alumno != null).OrderByDescending(m => m.oferta1.periodolectivo1.anio);

            IQueryable<cursa> noVan = db.cursa.Where(m => m.oferta.Equals(oferta) && m.DateModified < m.oferta1.fechaDesde && m.estado == (int)EstadosCursada.BajaAceptada);

            List<cursa> resul = alumnos.Except(noVan).ToList();

            alumnos = resul.AsQueryable();

            var cuotasOferta = CuotaData.consultarCuotas(null, null, null, DocenteController.codOferta);

            foreach (cursa pibe in alumnos)
            {
                if (pibe.estado == 5)
                    pibe.observacion += "Solicitud de Baja en proceso \n";
                else if (pibe.estado == 6)
                    pibe.observacion += "Baja Aceptada \n";
                else
                {
                    var cuotas = cuotasOferta.Where(x => x.Dni == pibe.AspNetUsers.NroDocumento).OrderBy(x => x.NroCuota).OrderBy(x => x.NroFactura);

                    foreach (Cuota cuota in cuotas)
                    {
                        pibe.observacion += "- C" + cuota.NroCuota + ": " + cuota.EstadoDescripcion + " " + Environment.NewLine;
                    }
                    //Cargo el estado de las cuotas de cada alumno que conforma el curso
                    alumnos.First(x => x.alumno == pibe.alumno).observacion = pibe.observacion;
                }
                //Cantidad de inasistencias
                pibe.DescripcionBaja = db.asistencia.Where(x => x.cursa == pibe.codigo && x.asistio == false).Count().ToString();
            }

            if (alumnos == null)
            {
                return HttpNotFound();
            }

            return PartialView("_GridAlumnoCurso", alumnos.OrderBy(x => x.AspNetUsers.Apellido).ToList());
        }

        #endregion

        #region Asistencias

        public ActionResult IndexAsistencias(int codOferta, string mensaje = "")
        {
            DocenteController.codOferta = codOferta;
            oferta info = db.oferta.Where(x => x.codigo.Equals(DocenteController.codOferta)).First();
            ViewBag.InfoCurso = info.descripcion.Trim();
            ViewBag.codOferta = codOferta;

            if (!string.IsNullOrEmpty(mensaje))
            {
                if (mensaje.Contains("error"))
                    ViewBag.MensajeError = mensaje;
                else
                    ViewBag.MensajeExito = mensaje;
            }
            return View();
        }

        public ActionResult guardarFecha(string fecha)
        {
            DocenteController.fecha = fecha;
            return Json(new { ok = true });
        }

        public ActionResult PartialGridAsistencias(int? codOferta = null, DateTime? fecha = null)
        {
            if (!fecha.HasValue)
            {
                DocenteController.fecha = DateTime.Now.ToShortDateString();
            }
            else
                DocenteController.fecha = fecha.Value.ToShortDateString();

            var valorFecha = DateTime.Parse(DocenteController.fecha);

            if (codOferta == null)
            {
                codOferta = DocenteController.codOferta;
            }

            //var codigo = Request.Params["codOferta"];
            IQueryable<cursa> alumnos = db.cursa.Where(m => m.oferta.Equals(codOferta.Value)).OrderByDescending(m => m.oferta1.periodolectivo1.anio);

            IQueryable<cursa> noVan = db.cursa.Where(m => m.oferta.Equals(codOferta.Value) && m.DateModified < m.oferta1.fechaDesde && m.estado == (int)EstadosCursada.BajaAceptada);

            List<cursa> resul = alumnos.Except(noVan).ToList();

            alumnos = resul.AsQueryable();
            //Listado de todas las cuotas para esa oferta
            var cuotasOferta = CuotaData.consultarCuotas(null, null, null, DocenteController.codOferta);

            //Listado de los estados de baja para toda esa oferta
            var baja = db.cursa.Where(x => (x.estado == 5 || x.estado == 6) && x.oferta == codOferta).ToList();

            foreach (cursa pibe in alumnos)
            {
                var baja_o_nobaja = baja.Where(x => x.alumno == pibe.alumno);

                if (baja_o_nobaja.Count() == 0)
                {
                    var cuotas = cuotasOferta.Where(x => x.Dni == pibe.AspNetUsers.NroDocumento);

                    foreach (Cuota cuota in cuotas)
                    {
                        pibe.observacion += "- Cuota " + cuota.NroCuota + ": " + cuota.EstadoDescripcion + " - ";
                    }
                    //Cargo el estado de las cuotas de cada alumno que conforma el curso
                    alumnos.First(x => x.alumno == pibe.alumno).observacion = pibe.observacion;
                }
                else
                {
                    if (pibe.estado == 5)
                        pibe.observacion += "Solicitud de Baja en proceso";
                    else if (pibe.estado == 6)
                        pibe.observacion += "Baja Aceptada";
                }

                //Cantidad de inasistencias
                pibe.DescripcionBaja = db.asistencia.Where(x => x.cursa == pibe.codigo && x.asistio == false).Count().ToString();

                // Cargo a partir de la fecha seleccionada si el alumno estuvo o no presente con la fecha actual
                var asistencia = db.asistencia.Where(x => x.asistencia1 == valorFecha && x.cursa1.codigo == pibe.codigo);

                var cantAsistencia = asistencia.Count();

                if (cantAsistencia > 0)
                {
                    pibe.ValidarNota = asistencia.First().asistio;
                }
                else if (cantAsistencia == 0)
                    pibe.ValidarNota = false;

                alumnos.First(x => x.alumno == pibe.alumno).ValidarNota = pibe.ValidarNota;
            }

            if (alumnos == null)
            {
                return HttpNotFound();
            }

            IQueryable<cursa> inscriptos = alumnos.Where(x => x.estado != (int)EstadosCursada.BajaAceptada).OrderBy(x => x.AspNetUsers.Apellido);
            IQueryable<cursa> Bajas = alumnos.Where(x => x.estado == (int)EstadosCursada.BajaAceptada).OrderBy(x => x.AspNetUsers.Apellido);
            List<cursa> resultante = new List<cursa>();

            resultante = inscriptos.ToList();
            if (resultante.Count() == 0)
            {
                resultante = Bajas.ToList();
            }
            else
            {
                //alumnos.ToList().AddRange(Bajas);
                resultante.AddRange(Bajas);
            }

            alumnos = resultante.AsQueryable();
            return PartialView("_GridAsistencias", alumnos/*.OrderBy(x => x.AspNetUsers.Apellido)*/.ToList());
        }

        public ActionResult Exportar()
        {
            List<cursa> alumnos = db.cursa.Where(m => m.oferta.Equals(codOferta)).ToList();


            List<cursa> noVan = db.cursa.Where(m => m.oferta.Equals(codOferta) && m.DateModified < m.oferta1.fechaDesde && m.estado == (int)EstadosCursada.BajaAceptada).ToList();

            List<cursa> resul = alumnos.Except(noVan).ToList();

            alumnos = resul;

            ExportSettings.DefaultExportType = ExportType.WYSIWYG;
            return GridViewExtension.ExportToXlsx(CrearGridViewSettingsExportar("ListadoAsistencia"), alumnos, true);
        }

        private GridViewSettings CrearGridViewSettingsExportar(string titulo)
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




            //Columnas
            settings.Columns.Add(col => { col.FieldName = "AspNetUsers.NroDocumento"; col.Caption = "Documento"; });
            settings.Columns.Add(col => { col.FieldName = "AspNetUsers.Nombre"; col.Caption = "Nombre"; });
            settings.Columns.Add(col => { col.FieldName = "AspNetUsers.Apellido"; col.Caption = "Apellido"; });

            IQueryable<cursa> alumnos = db.cursa.Where(m => m.oferta.Equals(codOferta)).OrderByDescending(m => m.oferta1.periodolectivo1.anio);

            IQueryable<cursa> noVan = db.cursa.Where(m => m.oferta.Equals(codOferta) && m.DateModified < m.oferta1.fechaDesde && m.estado == (int)EstadosCursada.BajaAceptada);

            List<cursa> resul = alumnos.Except(noVan).ToList();

            alumnos = resul.AsQueryable();


            var totalAsistenciaCurso = db.asistencia.ToList();
            bool marca = true;
            string fechaBaja = "";

            foreach (cursa pibe in alumnos)
            {
                var totalFaltasPorAlumno = totalAsistenciaCurso.Where(x => x.cursa == pibe.codigo).OrderBy(m => m.asistencia1.Date);

                int baja = db.cursa.Where(x => x.codigo == pibe.codigo).First().EstadoCursa.codigo;

                foreach (asistencia faltaPorAlumno in totalFaltasPorAlumno)
                {
                    if (baja != 5 && baja != 6)
                    {
                        if (faltaPorAlumno.asistio)
                            pibe.observacion += faltaPorAlumno.asistencia1.ToShortDateString() + " Si" + Environment.NewLine;
                        else
                            pibe.observacion += faltaPorAlumno.asistencia1.ToShortDateString() + " No" + Environment.NewLine;
                    }
                    else
                    {
                        if (faltaPorAlumno.asistio && DateTime.Parse(faltaPorAlumno.asistencia1.ToShortDateString()) < DateTime.Parse(faltaPorAlumno.cursa1.DateModified.ToShortDateString()))
                        {
                            pibe.observacion += faltaPorAlumno.asistencia1.ToShortDateString() + " Si" + Environment.NewLine;
                        }
                        else
                        {
                            if (faltaPorAlumno.asistio == false && DateTime.Parse(faltaPorAlumno.asistencia1.ToShortDateString()) < DateTime.Parse(faltaPorAlumno.cursa1.DateModified.ToShortDateString()))
                            {
                                pibe.observacion += faltaPorAlumno.asistencia1.ToShortDateString() + " No" + Environment.NewLine;
                            }

                        }

                        if (marca)
                        {
                            //pibe.observacion += " Baja " + faltaPorAlumno.cursa1.DateModified.ToShortDateString() + Environment.NewLine;
                            fechaBaja += " Baja " + faltaPorAlumno.cursa1.DateModified.ToShortDateString() + Environment.NewLine;
                            marca = false;
                        }
                    }
                }

                if (marca == false)
                {
                    pibe.observacion += fechaBaja;
                }

                marca = true;
                //Cargo el estado de las cuotas de cada alumno que conforma el curso
                alumnos.First(x => x.alumno == pibe.alumno).observacion = pibe.observacion;
            }

            settings.Columns.Add(col => { col.FieldName = "observacion"; col.Caption = "Asistencia"; });

            return settings;
        }

        #endregion

        #region Batch Edit

        [HttpPost, ValidateInput(false)]
        public ActionResult BatchEditing_Notas(MVCxGridViewBatchUpdateValues<cursa, object> lista)
        {
            int codOferta;
            var actualizar = db.cursa.Find(lista.Update[0].codigo);
            codOferta = actualizar.oferta;
            bool bandera = false;
            List<cursa> cursadas = db.cursa.Where(x => x.oferta == codOferta).ToList();

            IQueryable<cursa> noVan = db.cursa.Where(m => m.oferta.Equals(codOferta) && m.DateModified < m.oferta1.fechaDesde && m.estado == (int)EstadosCursada.BajaAceptada);

            List<cursa> resul = cursadas.Except(noVan).ToList();

            cursadas = resul;

            bool marcaError = false;
            string mensaje = "";

            if (User.IsInRole("Docente"))
            {
                ModelState.Remove("baja");
                ModelState.Remove("DescripcionBaja");
                // EDITS
                var baja = db.cursa.Where(x => x.oferta == codOferta).ToList();
                foreach (cursa alumno in lista.Update)
                {
                    lista.EditorErrors.Remove(alumno);

                    double nota;
                    if (alumno.nota == null)
                    {
                        alumno.nota = "0";
                    }
                    if (Double.TryParse(alumno.nota.Replace(".", ","), out nota))
                    {
                        var baja_o_no_baja = baja.Where(x => x.codigo == alumno.codigo).First();

                        //Cantidad de inasistencias
                        //db.asistencia.Where(x => x.cursa == alumno.codigo && x.asistio == false).Count().ToString();

                        if (baja_o_no_baja.estado != 5 && baja_o_no_baja.estado != 6 && baja_o_no_baja.resultadoCursada != 3)
                        {
                            if (baja_o_no_baja.ValidarNota == false)
                            {
                                if (nota >= 0 && nota <= 10)
                                {
                                    if (nota >= 7)
                                        actualizar.aprobo = true;
                                    else
                                        actualizar.aprobo = false;

                                    //Resultado de cursada
                                    actualizar = db.cursa.Find(alumno.codigo);
                                    actualizar.nota = nota.ToString();
                                    actualizar.resultadoCursada = alumno.resultadoCursada;
                                    db.Configuration.ValidateOnSaveEnabled = false;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    ViewBag.MensajeError += "Error: Las notas se deben encontrar entre 0 y 10 para los siguientes alumnos: " + actualizar.AspNetUsers.Apellido + ", " + actualizar.AspNetUsers.Nombre + Environment.NewLine;
                                }
                            }
                            else
                            {
                                bandera = true;
                            }
                        }
                        else
                        {
                            //El alumno esta dado de baja
                            actualizar = db.cursa.Find(alumno.codigo);
                            actualizar.nota = "0";
                            actualizar.resultadoCursada = 5;
                            db.Configuration.ValidateOnSaveEnabled = false;
                            db.SaveChanges();
                            ViewBag.MensajeError += "Recordar que para aquellos alumnos que tengan una Solicitud de Baja en proceso o bien una Baja Aceptada el campo Nota tendra valor 0 y el Estado de Cursada sera Baja.";
                        }

                    }
                    else
                    {
                        ViewBag.MensajeError += "Error: No se admiten el ingreso de letras en el campo Nota";
                        marcaError = true;
                    }
                }

                if (bandera)
                {
                    ViewBag.MensajeError += "Error: No se admiten cambios de Nota ya que estas fueran validadas por el Coordinador Docente";
                    marcaError = true;
                }

            }
            else
            {
                if (User.IsInRole("CoordinadorDocente") || User.IsInRole("Administrador") || User.IsInRole("SuperAdministrador") || User.IsInRole("AdministradorGraduado"))
                {
                    ModelState.Remove("baja");
                    ModelState.Remove("DescripcionBaja");
                    // EDITS
                    //int listaFalse = lista.Update.Where(x => x.ValidarNota == false).Count();
                    int cursadasFalse = cursadas.Where(x => x.ValidarNota == false).Count();

                    if (lista.Update.Count != cursadasFalse)
                    {
                        foreach (cursa item in cursadas)
                        {
                            try
                            {
                                int i = 0;
                                foreach (cursa item2 in lista.Update)
                                {
                                    if (item.codigo == item2.codigo)
                                    {
                                        i++;
                                    }

                                }

                                if (i == 0)
                                {
                                    marcaError = true;
                                    throw new Exception("Error: No validó la nota de lo/s alumno/s " + item.AspNetUsers.NroDocumento);
                                }
                            }
                            catch (Exception ex)
                            {
                                marcaError = true;
                                ViewBag.MensajeError += ex.Message.ToString() + Environment.NewLine;
                            }

                        }
                    }

                    var baja = db.cursa.Where(x => x.oferta == codOferta).ToList();


                    foreach (cursa alumno in lista.Update)
                    {
                        var baja_o_no_baja = baja.Where(x => x.codigo == alumno.codigo).First();

                        if (baja_o_no_baja.ValidarNota == false)
                        {
                            lista.EditorErrors.Remove(alumno);
                            if (lista.IsValid(alumno))
                            {
                                try
                                {
                                    if (alumno.ValidarNota == false)
                                    {
                                        actualizar = db.cursa.Find(alumno.codigo);
                                        throw new Exception("Error: No validó la nota de lo/s alumno/s " + actualizar.AspNetUsers.NroDocumento);
                                    }


                                    double nota;
                                    if (alumno.nota == null)
                                    {
                                        alumno.nota = "0";
                                    }
                                    if (Double.TryParse(alumno.nota.Replace(".", ","), out nota))
                                    {
                                        if (nota >= 0 && nota <= 10)
                                        {
                                            if (nota >= 7)
                                                actualizar.aprobo = true;
                                            else
                                                actualizar.aprobo = false;

                                            //Resultado de cursada
                                            actualizar = db.cursa.Find(alumno.codigo);
                                            actualizar.nota = alumno.nota;
                                            actualizar.resultadoCursada = alumno.resultadoCursada;
                                            actualizar.ValidarNota = alumno.ValidarNota;
                                            db.Configuration.ValidateOnSaveEnabled = false;
                                            db.SaveChanges();
                                        }
                                        else
                                        {
                                            ViewBag.MensajeError += "Error: Las notas se deben encontrar entre 0 y 10 para los siguientes alumnos: " + actualizar.AspNetUsers.Apellido + ", " + actualizar.AspNetUsers.Nombre + Environment.NewLine;
                                            marcaError = true;
                                        }
                                    }
                                    else
                                    {
                                        ViewBag.MensajeError += "Error: No se admiten el ingreso de letras en el campo Nota";
                                        marcaError = true;
                                    }



                                    //db.Configuration.ValidateOnSaveEnabled = false;
                                    //db.SaveChanges();
                                }
                                catch (Exception e)
                                {
                                    ViewBag.MensajeError += e.Message.ToString() + Environment.NewLine;
                                    marcaError = true;
                                }
                            }
                        }
                    }
                }
            }

            if (!marcaError)
            {
                return RedirectToAction("CargarNotas", "Docente", new { codOferta = DocenteController.codOferta, mensaje = "Las notas se cargaron correctamente." });
            }
            else
                return RedirectToAction("CargarNotas", "Docente", new { codOferta = DocenteController.codOferta, mensaje = (string)ViewBag.MensajeError });

            //return RedirectToAction("CargarNotas", "Docente", new { codOferta = codOferta, error = (string)ViewBag.ErrorMessage });

            //return RedirectToAction("CargarNotas", "Docente", new { codOferta = DocenteController.codOferta, mensaje = "Se cargaron correctamente las notas." });
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BatchEditing_Asistencia(MVCxGridViewBatchUpdateValues<cursa, object> asistencia)
        {
            DateTime cadena = DateTime.Parse(DocenteController.fecha);

            ModelState.Remove("baja");
            ModelState.Remove("DescripcionBaja");

            //Cargo la comisión en una lista
            List<cursa> aula = db.cursa.Where(x => x.oferta == DocenteController.codOferta).ToList();

            try
            {
                foreach (cursa alumno in aula)
                {
                    var elemento = asistencia.Update.Where(x => x.codigo == alumno.codigo);

                    if (elemento.Count() == 0)
                    {
                        ponerAsistencia(false, alumno.codigo, cadena);
                    }
                    else
                    {
                        if (ponerAsistencia(true, alumno.codigo, cadena) == false)
                        {
                            int baja = db.cursa.Where(x => x.codigo == alumno.codigo).First().EstadoCursa.codigo;
                            int ausente = db.cursa.Where(x => x.codigo == alumno.codigo).First().ResultadoCursa.codigo;

                            if (baja != 5 && baja != 6 && ausente != 3)
                            {
                                db.asistencia.Where(x => x.cursa == alumno.codigo && DbFunctions.TruncateTime(x.asistencia1) == DbFunctions.TruncateTime(cadena)).First().asistio = !db.asistencia.Where(x => x.cursa == alumno.codigo && DbFunctions.TruncateTime(x.asistencia1) == DbFunctions.TruncateTime(cadena)).First().asistio;
                                db.asistencia.Where(x => x.cursa == alumno.codigo && DbFunctions.TruncateTime(x.asistencia1) == DbFunctions.TruncateTime(cadena)).First().asistencia1 = cadena;
                                db.asistencia.Where(x => x.cursa == alumno.codigo && DbFunctions.TruncateTime(x.asistencia1) == DbFunctions.TruncateTime(cadena)).First().cursa = alumno.codigo;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("IndexAsistencias", "Docente", new { codOferta = DocenteController.codOferta, mensaje = "Ocurrió un error al cargar la asistencia para el día " + DocenteController.fecha.ToString() });
            }

            db.Configuration.ValidateOnSaveEnabled = false;
            db.SaveChanges();

            return RedirectToAction("IndexAsistencias", "Docente", new { codOferta = DocenteController.codOferta, mensaje = "Se cargo correctamente la asistencia del día " + DocenteController.fecha.ToString() });
        }

        private bool ponerAsistencia(bool presente, int aula, DateTime dia)
        {
            List<asistencia> yaEstaba = db.asistencia.Where(x => x.cursa == aula && DbFunctions.TruncateTime(x.asistencia1) == DbFunctions.TruncateTime(dia)).ToList();

            if (yaEstaba.Count() == 0)
            {
                int baja = db.cursa.Where(x => x.codigo == aula).First().EstadoCursa.codigo;
                int ausente = db.cursa.Where(x => x.codigo == aula).First().ResultadoCursa.codigo;

                if (baja != 5 && baja != 6 && ausente != 3)
                {
                    asistencia alta = new asistencia();
                    alta.asistio = presente;
                    alta.asistencia1 = dia;
                    alta.cursa = aula;
                    db.asistencia.Add(alta);
                }
                return true;
            }
            else
                return false;
        }

        #endregion
    }
}
