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
using System.Globalization;
using cnrl.Models;
using System.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using cnrl.Helpers;
using DevExpress.Export;
using DevExpress.Web.Mvc;
using System.Web.UI.WebControls;
using cnrl.Repositories;
using System.Threading.Tasks;
using System.Data.Entity.Validation;

namespace cnrl.Controllers
{
    public class cursaController : Controller
    {
        private socioculturalesEntities db = new socioculturalesEntities();

        #region GridFiltro
        // GET: cursa
        [HasPermission("cursa_Index")]

        public ActionResult Index()
        {
            //var cursa = db.cursa.Include(c => c.EstadoCursa).Include(c => c.oferta1).Include(c => c.AspNetUsers);
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

            return View();// cursa.ToList());
        }

        private void PrepararViewBag()
        {
            int nroSede = ApplicationUser.GetSede();

            ViewBag.TiposCurso = new SelectList(
                db.tipocurso.Where(x => x.sector == (int)Sectores.Idiomas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosTiposCurso,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.Carreras = new SelectList(
               db.AspNetUsers.Where(x => x.Carrera.ToString().Length != 0).Distinct().ToList()
               .ToSelectList(
                   x => x.Carrera,
                   x => x.Carrera.ToString(),
                   "(Todas las Carreras)",
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

            //---Cambio de comisión sólo en la misma sede
            //ViewBag.sedeDeCambio = ApplicationUser.GetSede();

            ViewBag.periodosLectivosCandidatos = new SelectList(
                db.periodolectivo.Where(x => (x.anio == DateTime.Now.Year - 1 && x.periodo == 3) || x.anio == DateTime.Now.Year && x.tipocurso1.sector == (int)Sectores.Idiomas).OrderByDescending(x => x.fechaInscripcionHasta).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosLosPeriodos,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.OfertasFiltroDeSede = new SelectList(
                db.oferta.Where(x => x.sede1.codigo == nroSede && x.periodolectivo1.anio == DateTime.Now.Year).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosLosCursos,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            //---

            ViewBag.cursos = new SelectList(
                db.curso.Where(x => x.tipocurso1.sector == (int)Sectores.Idiomas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosLosCursos,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.periodosLectivos = new SelectList(
                db.periodolectivo.OrderByDescending(x => x.fechaInscripcionHasta).Where(x => x.tipocurso1.sector == (int)Sectores.Idiomas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosLosPeriodos,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.estados = new SelectList(
                db.EstadoCursa.ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosEstados,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.OfertasFiltro = new SelectList(
                db.oferta.Where(x => x.curso1.tipocurso1.sector == (int)Sectores.Idiomas).ToList()//new List<oferta>()
                .ToSelectList(
                    x => x.codigo + " - " + x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosLasOfertas,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

        }

        private void PrepararViewBagEmpresa()
        {
            int nroSede = ApplicationUser.GetSede();

            ViewBag.TiposCurso = new SelectList(
                db.tipocurso.Where(x => /*x.codigo.Equals(23)*/x.sector == (int)Sectores.Empresas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosTiposCurso,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.Carreras = new SelectList(
               db.AspNetUsers.Where(x => x.Carrera.ToString().Length != 0).Distinct().ToList()
               .ToSelectList(
                   x => x.Carrera,
                   x => x.Carrera.ToString(),
                   "(Todas las Carreras)",
                   Constantes.ERROR.ToString(),
                   Constantes.ERROR.ToString()
               )
           , "Value", "Text");

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

            //---Cambio de comisión sólo en la misma sede
            //ViewBag.sedeDeCambio = ApplicationUser.GetSede();

            ViewBag.periodosLectivosCandidatos = new SelectList(
                db.periodolectivo.Where(x => (x.anio == DateTime.Now.Year - 1 && x.periodo == 3) || x.anio == DateTime.Now.Year && /*x.tipocurso1.codigo.Equals(23)*/x.tipocurso1.sector == (int)Sectores.Empresas).OrderByDescending(x => x.fechaInscripcionHasta).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosLosPeriodos,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.OfertasFiltroDeSede = new SelectList(
                db.oferta.Where(x => x.sede1.codigo == nroSede && x.periodolectivo1.anio == DateTime.Now.Year && /*x.curso1.tipocurso1.codigo.Equals(23)*/x.curso1.tipocurso1.sector == (int)Sectores.Empresas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosLosCursos,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            //---

            ViewBag.cursos = new SelectList(
                db.curso.Where(x => /*x.tipocurso1.codigo.Equals(23)*/x.tipocurso1.sector == (int)Sectores.Empresas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosLosCursos,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.periodosLectivos = new SelectList(
                db.periodolectivo.OrderByDescending(x => x.fechaInscripcionHasta).Where(x => /*x.tipocurso1.codigo.Equals(23)*/x.tipocurso1.sector == (int)Sectores.Empresas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosLosPeriodos,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.estados = new SelectList(
                db.EstadoCursa.ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosEstados,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.OfertasFiltro = new SelectList(
                db.oferta.Where(x => /*x.curso1.tipocurso1.codigo.Equals(23)*/x.curso1.tipocurso1.sector == (int)Sectores.Empresas).ToList()//new List<oferta>()
                .ToSelectList(
                    x => x.codigo + " - " + x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosLasOfertas,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

        }

        private void PrepararViewBagGraduado()
        {
            int nroSede = ApplicationUser.GetSede();

            ViewBag.TiposCurso = new SelectList(
                db.tipocurso.Where(x => /*x.codigo.Equals(23)*/x.sector == (int)Sectores.Graduados).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosTiposCurso,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.Carreras = new SelectList(
               db.AspNetUsers.Where(x => x.Carrera.ToString().Length != 0).Distinct().ToList()
               .ToSelectList(
                   x => x.Carrera,
                   x => x.Carrera.ToString(),
                   "(Todas las Carreras)",
                   Constantes.ERROR.ToString(),
                   Constantes.ERROR.ToString()
               )
           , "Value", "Text");

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

            //---Cambio de comisión sólo en la misma sede
            //ViewBag.sedeDeCambio = ApplicationUser.GetSede();

            ViewBag.periodosLectivosCandidatos = new SelectList(
                db.periodolectivo.Where(x => ((x.anio == DateTime.Now.Year - 1 && x.periodo == 3) || x.anio == DateTime.Now.Year) && x.tipocurso1.sector == (int)Sectores.Graduados).OrderByDescending(x => x.fechaInscripcionHasta).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosLosPeriodos,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.OfertasFiltroDeSede = new SelectList(
                db.oferta.Where(x => x.sede1.codigo == nroSede && x.periodolectivo1.anio == DateTime.Now.Year && x.curso1.tipocurso1.sector == (int)Sectores.Graduados).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosLosCursos,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            //---

            ViewBag.cursos = new SelectList(
                db.curso.Where(x => x.tipocurso1.sector == (int)Sectores.Graduados).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosLosCursos,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.periodosLectivos = new SelectList(
                db.periodolectivo.OrderByDescending(x => x.fechaInscripcionHasta).Where(x => x.tipocurso1.sector == (int)Sectores.Graduados).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosLosPeriodos,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.estados = new SelectList(
                db.EstadoCursa.ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosEstados,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.OfertasFiltro = new SelectList(
                db.oferta.Where(x => x.curso1.tipocurso1.sector == (int)Sectores.Graduados).ToList()//new List<oferta>()
                .ToSelectList(
                    x => x.codigo + " - " + x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosLasOfertas,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

        }

        [HasPermission("cursa_Index")]
        public ActionResult PartialGridCursadas(int? IdSede = null, int? IdCurso = null, string IdUsuario = "", int? IdPeriodo = null, int? IdOferta = null, int? Estado = null, int? IdTipoCurso = null, string IdCarrera = null)
        {
            var cursadas = consultar(IdSede, IdCurso, IdUsuario, IdPeriodo, IdOferta, Estado, IdTipoCurso, IdCarrera);

            var aMostrar = cursadas;

            foreach (cursa cursada in cursadas)
            {
                if (aMostrar.Find(m => m.codigo.Equals(cursada.codigo)).aprobo == true)
                {
                    aMostrar.Find(x => x.codigo.Equals(cursada.codigo)).observacion = cursada.nota;
                }
            }

            return PartialView("_GridCurso", aMostrar.ToList());
        }

        [HasPermission("cursa_Index")]
        [HttpPost]
        public ActionResult Habilitar(int IdOferta)
        {
            ModelState.Remove("baja");

            var cursa = db.cursa.Find(IdOferta);
            cursa.aprobo = !cursa.aprobo;
            db.Configuration.ValidateOnSaveEnabled = false;
            db.SaveChanges();
            return Json(new { ok = true });
        }

        public List<cursa> consultar(int? IdSede = null, int? IdCurso = null, string IdUsuario = "", int? IdPeriodo = null, int? IdOferta = null, int? Estado = null, int? IdTipoCurso = null, string carrera = null, int? diasVencido = null)
        {
            if (User.IsInRole("Ventanilla"))
            {
                IdSede = ApplicationUser.GetSede();
            }


            IQueryable<cursa> cursadas = db.cursa;

            if (User.IsInRole("AdministradorEmpresas"))
            {
                cursadas = cursadas.Where(x => /*x.oferta1.curso1.tipocurso1.codigo.Equals(23)*/x.oferta1.curso1.tipocurso1.sector == (int)Sectores.Empresas);
            }
            else
            {
                if (User.IsInRole("AdministradorGraduado"))
                {
                    cursadas = cursadas.Where(x => x.oferta1.curso1.tipocurso1.sector == (int)Sectores.Graduados);
                }
                else
                {
                    cursadas = cursadas.Where(x => x.oferta1.curso1.tipocurso1.sector == (int)Sectores.Idiomas);
                }

            }


            bool algunFiltro = false;
            if (carrera != null && carrera != Constantes.ERROR.ToString())
            {
                cursadas = cursadas.Where(x => x.AspNetUsers.Carrera == carrera);
                algunFiltro = true;
            }
            if (Estado.HasValue && Estado.Value != Constantes.ERROR)
            {
                cursadas = cursadas.Where(x => x.estado == Estado.Value);
                algunFiltro = true;
            }

            if (IdTipoCurso.HasValue && IdTipoCurso.Value != Constantes.ERROR)
            {
                cursadas = cursadas.Where(x => x.oferta1.curso1.tipoCurso == IdTipoCurso.Value);
                algunFiltro = true;
            }

            if (IdSede.HasValue && IdSede.Value != Constantes.ERROR)
            {
                cursadas = cursadas.Where(x => x.oferta1.sede == IdSede.Value);
                //algunFiltro = false;
            }

            if (IdCurso.HasValue && IdCurso.Value != Constantes.ERROR)
            {
                cursadas = cursadas.Where(x => x.oferta1.curso == IdCurso.Value);
                algunFiltro = true;
            }

            if (IdPeriodo.HasValue && IdPeriodo.Value != Constantes.ERROR)
            {
                cursadas = cursadas.Where(x => x.oferta1.periodoLectivo == IdPeriodo.Value);
                algunFiltro = true;
            }

            if (IdOferta.HasValue && IdOferta.Value != Constantes.ERROR)
            {
                cursadas = cursadas.Where(x => x.oferta == IdOferta.Value);
                algunFiltro = true;
            }

            if (!String.IsNullOrEmpty(IdUsuario) && IdUsuario != "-1")
            {
                cursadas = cursadas.Where(x => x.alumno == IdUsuario);
                algunFiltro = true;
            }

            if (diasVencido.HasValue && diasVencido.Value != Constantes.ERROR)
            {
                int dias = int.Parse(ConfigurationManager.AppSettings["Dias1erCuota"].ToString());
                var fechaComparar = DateTime.Now.AddDays(-dias).AddDays(-diasVencido.Value);
                cursadas = cursadas.Where(x => x.estado == (int)EstadosCursada.FormaPago || x.estado == (int)EstadosCursada.Inscripto);
                cursadas = cursadas.Where(x => x.DateModified < fechaComparar);
                algunFiltro = true;
            }

            if (algunFiltro)
                return cursadas.ToList();
            else
                return new List<cursa>();
        }

        #endregion

        #region Exportar

        public ActionResult Exportar(int? IdSede = null, int? IdCurso = null, string hiddenUsuario = "", int? IdPeriodo = null, int? IdOferta = null, int? Estado = null, int? IdTipoCurso = null)
        {
            var cursadas = consultar(IdSede, IdCurso, hiddenUsuario, IdPeriodo, IdOferta, Estado, IdTipoCurso);

            ExportSettings.DefaultExportType = ExportType.WYSIWYG;
            return GridViewExtension.ExportToXlsx(CrearGridViewSettingsExportar("Cursadas"), cursadas, true);
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

            settings.Columns.Add(col => { col.FieldName = "fechaAlta"; col.Caption = "Fecha"; col.PropertiesEdit.DisplayFormatString = "d"; col.Width = Unit.Pixel(25); });

            settings.Columns.Add(col => { col.FieldName = "AspNetUsers.NroDocumento"; col.Caption = "N° Doc"; });
            settings.Columns.Add(col => { col.FieldName = "AspNetUsers.Nombre"; col.Caption = "Nombre"; });
            settings.Columns.Add(col => { col.FieldName = "AspNetUsers.Apellido"; col.Caption = "Apellido"; });
            settings.Columns.Add(col => { col.FieldName = "AspNetUsers.Telefono"; col.Caption = "Telefono"; });
            settings.Columns.Add(col => { col.FieldName = "AspNetUsers.Email"; col.Caption = "Email"; });

            settings.Columns.Add(col => { col.FieldName = "oferta1.curso1.tipocurso1.descripcion"; col.Caption = Strings.ColTipoCurso; });
            settings.Columns.Add(col => { col.FieldName = "oferta1.curso1.descripcion"; col.Caption = Strings.ColCurso; });
            settings.Columns.Add(col => { col.FieldName = "oferta1.sede1.nombre"; col.Caption = Strings.ColSede; });
            settings.Columns.Add(col => { col.FieldName = "oferta1.periodolectivo1.descripcion"; col.Caption = Strings.ColPeriodo; });
            settings.Columns.Add(col => { col.FieldName = "oferta1.horario"; col.Caption = "Horario"; });

            settings.Columns.Add(col => { col.FieldName = "descripcion"; col.Caption = "Aprobado"; col.Width = Unit.Pixel(25); });
            settings.Columns.Add(col => { col.FieldName = "EstadoCursa.descripcion"; col.Caption = "Estado"; col.Width = Unit.Pixel(25); });

            return settings;
        }

        #endregion

        #region Beca

        [HasPermission("Beca")]
        public ActionResult Beca(int? id)
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

            if (cursa.estado.Value.Equals(0))
            {
                //ModelState.AddModelError(string.Empty, "Ocurrió un error.");
                ViewBag.Bandera = false;
                ViewBag.ErrorMessage = string.Format("No se podrá asignar una beca para esta persona, es necesario generar un plan de pago primero, revise e intente nuevamente.") + Environment.NewLine;
            }
            else
                ViewBag.Bandera = true;

            return View(cursa);
        }

        [HasPermission("Beca")]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [HttpPost]
        public async Task<ActionResult> AsignarBeca(cursa modelo)
        {
            try
            {
                ModelState.Remove("baja");
                ModelState.Remove("DescripcionBaja");
                if (ModelState.IsValid)
                {
                    cursa cursadaConBeca = db.cursa.Find(modelo.codigo);

                    if (cursadaConBeca == null)
                    {
                        return HttpNotFound();
                    }

                    //Busco las cuotas de la persona a becar
                    var cuotas = CuotaData.consultarCuotas(cursadaConBeca.alumno, null, null, cursadaConBeca.oferta, ((int)EstadoCuota.Impaga).ToString());
                    //var cuotas = CuotaData.consultarCuotas(cursadaConBeca.codigo.ToString());

                    cursadaConBeca.porcentajeBeca = modelo.porcentajeBeca;
                    double precio1cuota = 0;
                    

                    //Porcentaje de Beca distinto de 100%
                    if (modelo.porcentajeBeca != 100)
                    {
                        //Calculo de cuotas
                        if (cursadaConBeca.cantidadCuotas == 1)
                            precio1cuota = CuotaData.ObtenerPrecioUnaCuota(cursadaConBeca);
                        
                        else
                            precio1cuota = CuotaData.ObtenerPrecioCuotas(cursadaConBeca);

                        //Descuento
                        double precioBecado = precio1cuota - precio1cuota * double.Parse(cursadaConBeca.porcentajeBeca.Value.ToString()) / 100;

                        //Actualizco las cuotas en el caso de que ya esten
                        foreach (var cuota in cuotas)
                        {
                            string conBeca = cuota.Importe.ToString("C2");
                            string sin = precioBecado.ToString("C2");
                            cuota.Motivo = string.Format("Beca del {0}%, valor anterior: {1}, valor becado: {2}", cursadaConBeca.porcentajeBeca.Value, conBeca, sin);
                            cuota.Importe = (decimal)precioBecado;
                            cuota.Importe2 = cuota.Importe2 -  cuota.Importe2 * decimal.Parse(cursadaConBeca.porcentajeBeca.Value.ToString()) / 100;
                            await CuotaData.actualizarCuota(cuota);
                        }
                    }
                    //Porcentaje de beca igual al 100%
                    else
                    {
                        //Pongo a 0 las cuotas creadas
                        foreach (var cuota in cuotas)
                        {
                            string conBeca = cuota.Importe.ToString("C2");
                            cuota.Motivo = string.Format("Beca del {0}%, valor anterior: {1}, valor becado: {2}", cursadaConBeca.porcentajeBeca.Value, conBeca, "0,00");
                            cuota.Importe = 0;
                            cuota.Importe2 = 0;
                            CuotaData.bajaCuota(cuota);
                        }
                    }

                    //Paso al estado forma de pago y guardo los cambios
                    if (cursadaConBeca.porcentajeBeca == 100)
                        cursadaConBeca.estado = 12;
                    else
                        cursadaConBeca.estado = 7;

                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();

                    return Json(new { ok = true });
                }
                else
                    ViewBag.ErrorMessage += string.Format("No se acepta el punto como separador decimal, utilice la coma. Revise e intente nuevamente.") + Environment.NewLine;
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                ViewBag.ErrorMessage = string.Format(Strings.ErrorIntentarInsertarRegistro, e.Message.Replace(Strings.TheStatementHasBeenTerminated, ""));
                return View();
            }

            return Json(new { error = true });
        }

        #endregion

        #region Cambio de Comision

        public ActionResult CambioComision(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cursa cursa = db.cursa.Find(id);
            //ViewBag.sedeDeCambio = cursa.oferta1.sede;
            if (cursa == null)
            {
                return HttpNotFound();
            }
            ViewBag.modo = "Edit";

            ViewBag.Ofertas = new SelectList(
                db.oferta.Where(x => x.codigo == cursa.oferta).ToList()
                .ToSelectList(
                    x => x.codigo + " - " + x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.SeleccionarOferta,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");
            ViewBag.estado = new SelectList(db.EstadoCursa, "codigo", "descripcion", cursa.estado);
            //ViewBag.oferta = new SelectList(db.oferta, "codigo", ".descripcion", cursa.oferta);
            if (User.IsInRole("AdministradorEmpresas"))
            {
                PrepararViewBagEmpresa();
            }
            else {
                if (User.IsInRole("AdministradorGraduado"))
                {
                    PrepararViewBagGraduado();
                }
                else
                {
                    PrepararViewBag();
                }

            }
            return View(cursa);
        }

        // POST: cursa/Edit/5
        [HttpPost]
        public async Task<ActionResult> CambioComision(cursa cursa)
        {
            cursa cursaOriginal = db.cursa.Find(cursa.codigo);

            try
            {
                ModelState.Remove("baja");
                ModelState.Remove("DescripcionBaja");

                if (ModelState.IsValid)
                {
                    decimal devolucion = 0;
                    cursa.AspNetUsers = cursaOriginal.AspNetUsers;
                    cursa.oferta1 = db.oferta.Find(cursa.oferta);

                    if (cursaOriginal.oferta != cursa.oferta)
                        devolucion = await CuotaData.cambioComisionCuota(cursaOriginal, cursa);

                    //cursaOriginal.aprobo = cursa.aprobo;
                    cursaOriginal.oferta = cursa.oferta;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    //db.Entry(cursa).State = EntityState.Modified;

                    db.SaveChanges();
                    return Json(new { ok = true, devolucion = devolucion });
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

            ViewBag.Ofertas = new SelectList(
                db.oferta.Where(x => x.codigo == cursa.oferta).ToList()
                .ToSelectList(
                    x => x.codigo + " - " + x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.SeleccionarOferta,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.estado = new SelectList(db.EstadoCursa, "codigo", "descripcion", cursa.estado);
            PrepararViewBag();
            ViewBag.modo = "Edit";
            return View(cursaOriginal);
        }

        #endregion

        #region nuevaCursada
        public ActionResult NuevaCursada()
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //cursa cursa = db.cursa.Find(id);
            //if (cursa == null)
            //{
            //    return HttpNotFound();
            //}
            //ViewBag.modo = "Edit";

            //ViewBag.Ofertas = new SelectList(
            //    db.oferta.Where(x => x.codigo == cursa.oferta).ToList()
            //    .ToSelectList(
            //        x => x.codigo + " - " + x.descripcion,
            //        x => x.codigo.ToString(),
            //        Strings.SeleccionarOferta,
            //        Constantes.ERROR.ToString(),
            //        Constantes.ERROR.ToString()
            //    )
            //, "Value", "Text");
            //ViewBag.estado = new SelectList(db.EstadoCursa, "codigo", "descripcion", cursa.estado);
            ViewBag.oferta = new SelectList(new List<oferta>(), "codigo", ".descripcion", "");

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

            return View(new cursa());
        }

        // POST: cursa/Edit/5
        [HttpPost]
        public ActionResult NuevaCursada(cursa cursa)
        {
            try
            {
                ModelState.Remove("baja");
                ModelState.Remove("codigo");
                ModelState.Remove("DescripcionBaja");
                if (ModelState.IsValid)
                {
                    cursa.estado = (int)EstadosCursada.Inscripto;

                    cursa.fechaAlta = DateTime.Now;
                    cursa.observacion = "";
                    cursa.baja = null;
                    cursa.resultadoCursada = 4;
                    cursa.ValidarNota = false;
                    db.cursa.Add(cursa);

                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();

                    return Json(new { ok = true, usuario = cursa.alumno });
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
            ViewBag.Ofertas = new SelectList(
                db.oferta.Where(x => x.codigo == cursa.oferta).ToList()
                .ToSelectList(
                    x => x.codigo + " - " + x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.SeleccionarOferta,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.estado = new SelectList(db.EstadoCursa, "codigo", "descripcion", cursa.estado);
            PrepararViewBag();
            ViewBag.modo = "Edit";
            return View(cursa);
        }
        #endregion

        #region inscripcion

        [HasPermission("inscripcion")]
        public ActionResult Inscripcion(string id) //id de usuario
        {
            if (id == null || string.IsNullOrEmpty(id)) // si es nulo entonces inscribo al usuario logueado
            {
                id = ApplicationUser.GetId();
            }

            var usuario = db.AspNetUsers.Find(id);

            ViewBag.TieneDeuda = CuotaData.TieneDeudaAlumnoVencida(usuario.NroDocumento.Value);

            ViewBag.Usuario = id;
            ViewBag.UsuarioCompleto = usuario;

            return View(consultarOfertaDisponible(id));
        }

        IQueryable<ofertaDisponiblePorAlumno4_Result> consultarOfertaDisponible(string idUsuario)
        {

            var ofertaDisponible = db.ofertaDisponiblePorAlumno4(idUsuario).ToList();
            var ofertaDisponibleSinCorrelativas = db.ofertaDisponiblePorAlumno4(idUsuario).ToList();


            foreach (var ofer in ofertaDisponible)
            {
                var cur = db.curso.Find(ofer.curso);

                eliminarCorrelativas(cur, ref ofertaDisponibleSinCorrelativas);


            }

            var ofertaDisponibleSinOfertaLLena = ofertaDisponibleSinCorrelativas.ToList();

            if (!ApplicationUser.userHasPermission("no_validar_cupo"))
            {
                foreach (var ofer in ofertaDisponibleSinCorrelativas)
                {
                    var oferta = db.oferta.Find(ofer.codigo);
                    if (ofer.cupo <= oferta.cantidadPreInscriptos || ofer.cupo <= oferta.cantidadInscriptos)
                    {
                        ofertaDisponibleSinOfertaLLena.Remove(ofer);
                        //ofer.selected = "selected";
                    }
                }
            }
            return ofertaDisponibleSinOfertaLLena.AsQueryable();
        }

        void eliminarCorrelativas(curso curso, ref List<ofertaDisponiblePorAlumno4_Result> lista)
        {
            if (curso.curso1.Count == 1)
            {
                foreach (var cur in curso.curso1)
                {
                    var ofertaCorrelativa = lista.Where(x => x.curso == cur.codigo).ToList();
                    foreach (var oferCor in ofertaCorrelativa)
                    {
                        lista.Remove(oferCor);
                    }
                    eliminarCorrelativas(cur, ref lista);
                }
            }
        }

        [HasPermission("Alumnos_Index")]
        public ActionResult InscribirAlumno(string id) //id de usuario
        {
            if (id == null || string.IsNullOrEmpty(id)) // si es nulo entonces inscribo al usuario logueado
            {
                id = "";
                ViewBag.UsuarioCompleto = new AspNetUsers { Nombre = "", Apellido = "", Id = "", NroDocumento = 0 };

            }
            else
            {
                ViewBag.UsuarioCompleto = db.AspNetUsers.Find(id);
            }

            //var ofertaDisponible = db.ofertaDisponiblePorAlumno4(id).ToList();
            ViewBag.Usuario = id;

            var ofertas = consultarOfertaDisponible(id);

            if (User.IsInRole("AdministradorEmpresas"))
            {
                ViewBag.Empresa = true;
            }
            else
            {
                if (User.IsInRole("AdministradorGraduado"))
                {
                    ViewBag.Graduado = true;
                }
                else
                {
                    ViewBag.Empresa = false;
                    ViewBag.Graduado = false;
                }

            }
            
            return View(ofertas);
        }

        [HasPermission("Alumnos_Index")]
        public ActionResult FormaDePagoAlumno(string id) //id de usuario
        {
            if (id == null || string.IsNullOrEmpty(id)) // si es nulo entonces inscribo al usuario logueado
            {
                id = "";
                ViewBag.UsuarioCompleto = new AspNetUsers { Nombre = "", Apellido = "", Id = "", NroDocumento = 0 };
            }
            else
            {
                ViewBag.UsuarioCompleto = db.AspNetUsers.Find(id);
            }

            //var ofertaDisponible = db.ofertaDisponiblePorAlumno4(id).ToList();
            ViewBag.Usuario = id;

            var cursos = db.cursa.Where(x => x.alumno == id && (x.estado == (int)EstadosCursada.Inscripto || x.estado == (int)EstadosCursada.FormaPago));
            if (User.IsInRole("AdministradorEmpresas"))
            {
                cursos = cursos.Where(x => /*x.oferta1.curso1.tipocurso1.codigo.Equals(23)*/x.oferta1.curso1.tipocurso1.sector == (int)Sectores.Empresas);
            }
            else
            {
                if (User.IsInRole("AdministradorGraduado"))
                {
                    cursos = cursos.Where(x => /*x.oferta1.curso1.tipocurso1.codigo.Equals(23)*/x.oferta1.curso1.tipocurso1.sector == (int)Sectores.Graduados);
                }
                else
                {
                    if (!User.IsInRole("Alumno"))
                    {
                        cursos = cursos.Where(x => /*!x.oferta1.curso1.tipocurso1.codigo.Equals(23)*/x.oferta1.curso1.tipocurso1.sector == (int)Sectores.Idiomas);
                    }
                }

            }


            return View(cursos);
        }
        [HttpPost]
        public ActionResult Inscripcion(ofertaDisponiblePorAlumno4_Result ofertaDisponible)
        {
            List<string> listaDeCursos = new List<string>();
            List<cursa> codOferta = new List<cursa>();
            List<string> listaDeCod = new List<string>();
            //
            string usuario = Request.Params["usuario"];
            string callBackView = Request.Params["callBackView"];
            string codCurso = Request.Params["codCurso"];
            string codigoOfertas = Request.Params["codigoOferta"];
            string selected = Request.Params["selected"];
            string cod = Request.Params["codigo"];

            ViewBag.Usuario = usuario;
            var usuarioBase = db.AspNetUsers.Find(usuario);
            ViewBag.UsuarioCompleto = usuarioBase;

            List<string> preInscripcion = new List<string>();
            string[] listaCodCursos = codCurso.Split(',');
            string[] listaOfertas = codigoOfertas.Split(',');
            string[] select = selected.Split(',');
            string[] c = new string[10];

            if (User.IsInRole("Alumno")) //VERIFICO SI EL ALUMNO TIENE DEUDA NO DEBERIA LLEGAR HASTA ACA, PERO SI LLEGO POR QUE SALTEO LA VALIDACIÓN JAVASCRIPT LO REBOTO
            {
                var tieneDeuda = CuotaData.TieneDeudaAlumnoVencida(usuarioBase.NroDocumento.Value);
                ViewBag.TieneDeuda = tieneDeuda;

                if (tieneDeuda)
                {
                    return View(consultarOfertaDisponible(usuario));//consultarOfertaDisponible
                }
            }

            ModelState.Remove("baja");

            if (ModelState.IsValid)
            {
                var ofertaA = 0;
                var codInt = 0;

                if (cod != null)
                {
                    c = cod.Split(',');
                }

                int i = 0, b = 0;

                if (cod == "-1" || cod == null)
                {
                    foreach (string s in select)
                    {
                        b++;
                        listaDeCod.Add(s);
                        i = b;
                    }

                    if (listaDeCod.Count != 0)
                    {
                        i = 0;

                        foreach (string data in listaDeCod)
                        {
                            i++;

                            if (data == "selected")
                            {
                                ofertaA = int.Parse(listaOfertas[i - 1]);
                                preInscripcion.Add("");
                                codInt = int.Parse(listaOfertas[i - 1]);
                                codOferta = db.cursa.Where(x => x.alumno.Equals(usuario) && x.oferta == codInt).ToList();
                            }
                            else
                            {
                                preInscripcion.Add("");
                            }
                        }
                    }
                    else
                    {
                        if (i == 0)
                        {
                            ViewBag.Usuario = usuario;
                            ViewBag.UsuarioCompleto = db.AspNetUsers.Find(usuario);
                            return View(consultarOfertaDisponible(usuario));
                        }
                    }
                }
                else
                {
                    foreach (string cc in c)
                    {
                        if (cc != "-1")
                            listaDeCod.Add(cc);
                    }

                    if (listaDeCod.Count != 0)
                    {
                        foreach (string coleccion in listaDeCod)
                        {
                            ofertaA = int.Parse(coleccion);
                            int cursoA = db.oferta.Where(x => x.codigo.Equals(ofertaA)).First().curso;
                            listaDeCursos.Add(cursoA.ToString());
                            if (db.oferta.Where(x => x.codigo.Equals(ofertaA)).First().RequierePreInscripcion == true)
                            {
                                preInscripcion.Add("True");
                                ViewBag.PreInscripcion = true;
                            }
                            else
                            {
                                preInscripcion.Add("False");
                                ViewBag.PreInscripcion = false;
                            }
                        }
                    }
                    else
                    {
                        ViewBag.Usuario = usuario;
                        ViewBag.UsuarioCompleto = db.AspNetUsers.Find(usuario);

                        if (User.IsInRole("AdministradorEmpresas"))
                        {
                            ViewBag.Empresa = true;
                        }
                        else
                        {
                            if (User.IsInRole("AdministradorGraduado"))
                            {
                                ViewBag.Empresa = false;
                            }
                            else
                            {
                                if (User.IsInRole("Alumno"))
                                {
                                    ViewBag.Empresa = false;
                                    return RedirectToAction("Inscripcion", new { id = usuario });
                                }
                                ViewBag.Empresa = false;
                            }

                        }

                        return RedirectToAction("InscribirAlumno", new { Id = usuario });

                        //return View(consultarOfertaDisponible(usuario));
                        //return View(consultarOfertaDisponible(usuario));
                    }
                }

                if (cod == "-1" || cod == null)
                {
                    int cuentaVuelta = 0;
                    string cadenaTrue = "";
                    int bandera = 0;

                    foreach (string lp in preInscripcion)
                    {
                        cuentaVuelta++;

                        if (preInscripcion.Count > 0 && lp == "True" && codOferta.First().estado == 9)
                        {
                            cadenaTrue += listaOfertas[cuentaVuelta - 1].ToString() + "-";
                            bandera = 1;
                        }

                        if ((preInscripcion.Count > 0 && lp == "False") || codOferta.First().estado == 0)
                        {
                            return RedirectToAction("FormaDePago", new { Id = usuario });
                        }
                    }
                    if (bandera == 1)
                    {
                        return RedirectToAction("Documentacion", new { Id = cadenaTrue, Us = usuario });
                    }
                }

                int cuentaCodigos = 0;

                foreach (var codigo in listaDeCod) //inserto cursadas nuevas
                {
                    cuentaCodigos++;

                    if (!string.IsNullOrEmpty(codigo) && codigo != Constantes.ERROR.ToString())
                    {
                        int codigoOferta = int.Parse(codigo);
                        var ofertaInscripcion = db.oferta.Find(codigoOferta);
                        if (ofertaInscripcion != null)
                        {
                            var cursadasInscripto = db.cursa.Where(x => x.alumno == usuario && x.oferta == codigoOferta && x.estado != (int)EstadosCursada.BajaAceptada);//verifico si es una cursada nueva o un update
                            cursa cursaInscripto;

                            if (cursadasInscripto.Count() == 0)
                                cursaInscripto = new cursa();
                            else
                                cursaInscripto = cursadasInscripto.First();

                            cursaInscripto.alumno = usuario;

                            if (preInscripcion.Count > 0 && preInscripcion[cuentaCodigos - 1].ToString() == "True")
                            {
                                cursaInscripto.estado = (int)EstadosCursada.PreInscripcion;
                            }
                            else
                            {
                                cursaInscripto.estado = (int)EstadosCursada.Inscripto;
                            }

                            cursaInscripto.fechaAlta = DateTime.Now;
                            cursaInscripto.oferta = codigoOferta;
                            cursaInscripto.observacion = "";
                            cursaInscripto.baja = null;
                            cursaInscripto.resultadoCursada = 4;
                            cursaInscripto.ValidarNota = false;

                            if (cursadasInscripto.Count() == 0)
                                db.cursa.Add(cursaInscripto);
                        }
                    }
                }

                //veo si hay cursadas para borrar
                //var cursadasUsuario = db.cursa.Where(x => x.alumno == usuario && x.estado == (int)EstadosCursada.Inscripto);

                //foreach (var cursadaDeUsuario in cursadasUsuario)
                //{
                //    bool encontrada = false;
                //    foreach (var codigo in codigos) //inserto cursadas nuevas
                //    {
                //        if (codigo == cursadaDeUsuario.oferta.ToString())
                //            encontrada = true;
                //    }

                //    if (!encontrada)
                //    {
                //        db.cursa.Remove(cursadaDeUsuario);
                //    }
                //}



                if (Request.Params["codigo"] != "-1")
                {
                    try
                    {
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }
                }

                var codlistado = listaDeCod[0];
                DateTime fech = DateTime.Now;

                cursa ultimoInsert = db.cursa.Where(x => x.alumno == usuario /*&& DbFunctions.TruncateTime(x.fechaAlta) == cursa*/ && x.oferta.ToString() == codlistado).First();

                string codigosInscrip = ultimoInsert.codigo.ToString();

                Session.Remove("CodigoInscripto");
                Session.Add("CodigoInscripto", codigosInscrip);

                string cadenaTru = "";
                int band = 0;
                int preCont = 0;

                foreach (string colec in listaDeCod)
                {
                    ofertaA = int.Parse(colec);
                    preCont++;
                    if (ofertaA != -1)
                    {
                        codOferta = db.cursa.Where(x => x.alumno.Equals(usuario) && x.oferta == ofertaA).ToList();
                        if (preInscripcion.Count > 0 && preInscripcion[preCont - 1].ToString() == "True" && codOferta.First().estado == 9)
                        {
                            if (i > 0 && listaOfertas.Count() == c.Count())
                            {
                                cadenaTru += listaOfertas[preCont - 1].ToString() + "-";
                                band = 1;
                            }
                            else
                            {
                                int conta = 0, contb = 0;
                                int cant = c.Count();
                                for (int mm = 0; mm < c.Count(); mm++)
                                {
                                    conta = 0;
                                    foreach (string dat in listaOfertas)
                                    {
                                        conta++;
                                        if (dat == c[mm].ToString())
                                        {
                                            contb = conta - 1;
                                            break;
                                        }
                                    }
                                }
                                return RedirectToAction("Documentacion", new { Id = (listaOfertas[contb].ToString() + "-"), Us = usuario });
                            }
                        }
                        if ((preInscripcion.Count > 0 && preInscripcion[preCont - 1].ToString() == "False") || (codOferta.First().estado == 0 && codOferta.First().oferta1.gratuito == false))
                        {
                            return RedirectToAction(callBackView, new { Id = usuario });
                        }

                        if ((preInscripcion.Count > 0 && preInscripcion[preCont - 1].ToString() == "False") || (codOferta.First().estado == 0 && codOferta.First().oferta1.gratuito == true))
                        {
                            return RedirectToAction(callBackView, new { Id = usuario });
                        }

                        if (!(preInscripcion.Count > 0 && preInscripcion[preCont - 1].ToString() == "True" && codOferta.First().estado == 9))
                        {
                            ViewBag.Usuario = usuario;
                            ViewBag.UsuarioCompleto = db.AspNetUsers.Find(usuario);
                            return View(consultarOfertaDisponible(usuario));
                        }
                    }
                }

                if (band == 1)
                {
                    return RedirectToAction("Documentacion", new { Id = cadenaTru, Us = usuario });
                }

                bool irAFormaDePago = false;

                foreach (var codi in select)
                {
                    if (codi == "selected")
                        irAFormaDePago = true;
                }

                if (irAFormaDePago)
                    return RedirectToAction("FormaDePago", new { Id = usuario });
            }

            return View(consultarOfertaDisponible(usuario));
        }

        [HasPermission("inscripcion")]
        public ActionResult FormaDePago(string Id) //id de usuario
        {
            if (Id == null || string.IsNullOrEmpty(Id)) // si es nulo entonces inscribo al usuario logueado
            {
                Id = ApplicationUser.GetId();
            }

            var cursos = db.cursa.Where(x => x.alumno == Id && (x.estado == (int)EstadosCursada.Inscripto || x.estado == (int)EstadosCursada.FormaPago));
            if (User.IsInRole("AdministradorEmpresas"))
            {
                cursos = cursos.Where(x => /*x.oferta1.curso1.tipocurso1.codigo.Equals(23)*/x.oferta1.curso1.tipocurso1.sector == (int)Sectores.Empresas);
            }
            else
            {
                if (User.IsInRole("AdministradorGraduado"))
                {
                    cursos = cursos.Where(x =>/* x.oferta1.curso1.tipocurso1.codigo.Equals(23)*/x.oferta1.curso1.tipocurso1.sector == (int)Sectores.Graduados);
                }
                else
                {
                    if (!User.IsInRole("Alumno"))
                    {
                        cursos = cursos.Where(x => /*!x.oferta1.curso1.tipocurso1.codigo.Equals(23)*/x.oferta1.curso1.tipocurso1.sector == (int)Sectores.Idiomas);
                    }
                }

            }
            var usuarioBase = db.AspNetUsers.Find(Id);

            //Hallo la edad para informar
            var edadPersona = Edad(DateTime.Parse(usuarioBase.FechaNacimiento.Value.ToShortDateString()));

            if (edadPersona >= 0 && edadPersona <= 15)
            {
                ViewBag.MenorDeQuince = true;
            }
            else
                ViewBag.MenorDeQuince = false;

            ////Hallo la preinscripcion para informar
            //List<cursa> aVerificar = db.cursa.Where(x => x.alumno == Id && (x.estado == (int)EstadosCursada.Inscripto || x.estado == (int)EstadosCursada.FormaPago)).ToList();
            //var oferta = traerPreInscripcion(aVerificar);

            //if (oferta != "")
            //{
            //    ViewBag.PreInscripcion = true;
            //    ViewBag.Informacion = oferta;
            //}
            //else
            //    ViewBag.PreInscripcion = false;
            var encuesta = db.Encuesta.Find(Id);
            if (encuesta == null)
            {
                ViewBag.Encuesta = false;
            }
            else
            {
                ViewBag.Encuesta = true;
            }
            if (User.IsInRole("AdministradorEmpresas") == false && User.IsInRole("AdministradorGraduado") == false)
            {
                if (cursos.Count() != 0)
                {
                    foreach (cursa item in cursos)
                    {
                        if (item.DateCreated.ToString("dd/MM/yyyy HH:mm") == DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
                        {
                            ViewBag.Rol = true;
                            ViewBag.mensajeEmpresa = false;
                        }
                    }

                    if (ViewBag.Rol != true)
                    {
                        ViewBag.Rol = false;
                        ViewBag.mensajeEmpresa = false;
                    }

                }
                else
                {
                    ViewBag.Rol = false;
                    ViewBag.mensajeEmpresa = false;
                }

            }
            else
            {
                if (User.IsInRole("AdministradorGraduado") == true)
                {
                    ViewBag.Rol = false;
                    ViewBag.mensajeEmpresa = false;
                }
                else
                {
                    ViewBag.Rol = false;
                    ViewBag.mensajeEmpresa = true;
                }

            }
            ViewBag.Id = Id;

            return View(cursos);
        }

        public ActionResult InsertarEncuesta(string radio = "", string id = "")
        {
            try
            {

                if (radio != "undefined")
                {
                    var codigos = Session["CodigoInscripto"].ToString();


                    Encuesta enc = new Encuesta();
                    enc.id = id;
                    enc.fecha = DateTime.Now;
                    enc.idRespuesta = int.Parse(radio);
                    enc.idCursa = int.Parse(codigos);

                    //if (radio == "1")
                    //{
                    //    enc.facebook = true;
                    //}
                    //if (radio == "2")
                    //{
                    //    enc.instagram = true;
                    //}
                    //if (radio == "3")
                    //{
                    //    enc.WebUnlam = true;
                    //}
                    //if (radio == "4")
                    //{
                    //    enc.BuscadorWeb = true;
                    //}
                    //if (radio == "5")
                    //{
                    //    enc.PeriodicoUnlam = true;
                    //}
                    //if (radio == "6")
                    //{
                    //    enc.Cartelera = true;
                    //}
                    //if (radio == "7")
                    //{
                    //    enc.ViaPublica = true;
                    //}
                    //if (radio == "8")
                    //{
                    //    enc.Radio = true;
                    //}
                    //if (radio == "9")
                    //{
                    //    enc.CurseAntes = true;
                    //}
                    //if (radio == "10")
                    //{
                    //    enc.Alumno = true;
                    //}
                    //if (radio == "11")
                    //{
                    //    enc.AmigosConocidos = true;
                    //}

                    db.Entry(enc).State = EntityState.Added;
                    db.SaveChanges();


                    //Url.Action("FormaDePago", "cursa", new { Id = id });


                    //return Json(new { ok = true }, JsonRequestBehavior.AllowGet);
                }



                return RedirectToAction("FormaDePago", new { Id = id });
            }
            catch (Exception ex)
            {

                //return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }
            return new EmptyResult();
            //return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
        }

        [HasPermission("inscripcion")]
        public ActionResult CursoGratuito(string Id) //id de usuario
        {
            if (Id == null || string.IsNullOrEmpty(Id)) // si es nulo entonces inscribo al usuario logueado
            {
                Id = ApplicationUser.GetId();
            }

            var cursos = db.cursa.Where(x => x.alumno == Id && (x.estado == (int)EstadosCursada.Inscripto || x.estado == (int)EstadosCursada.FormaPago));
            var usuarioBase = db.AspNetUsers.Find(Id);

            //Hallo la edad para informar
            var edadPersona = Edad(DateTime.Parse(usuarioBase.FechaNacimiento.Value.ToShortDateString()));

            if (edadPersona >= 0 && edadPersona <= 15)
            {
                ViewBag.MenorDeQuince = true;
            }
            else
                ViewBag.MenorDeQuince = false;

            return View(cursos);
        }

        [HttpPost]
        public async Task<ActionResult> FormaDePago(cursa cur)
        {
            DateTime fecha1er = DateTime.Now;
            Decimal importe1er = 0;
            var gratuito = db.cursa.Find(cur.codigo);

            if (gratuito.oferta1.gratuito == false)
            {
                try
                {
                    var cursada = db.cursa.Find(cur.codigo);

                    cursada.cantidadCuotas = cur.cantidadCuotas;
                    cursada.estado = (int)EstadosCursada.FormaPago;
                    db.Entry(cursada).State = EntityState.Modified;

                    decimal importe1erVencimiento = 0;
                    decimal importe2erVencimiento = 0;

                    //Muchas Cuotas
                    if (cur.cantidadCuotas > 1)
                    {
                        importe1erVencimiento = (decimal)CuotaData.ObtenerPrecioCuotas(cursada);
                    }
                    else//Una única Cuota
                    {
                        importe1erVencimiento = (decimal)CuotaData.ObtenerPrecioUnaCuota(cursada);
                    }

                    if (cursada.oferta1.recargoSegundoVencimiento.HasValue)
                        importe2erVencimiento = importe1erVencimiento + (importe1erVencimiento * (decimal)cursada.oferta1.recargoSegundoVencimiento.Value / 100);
                    else
                    {
                        int recargoDefault = int.Parse(ConfigurationManager.AppSettings["RecargoSegundoVencimientoDefault"].ToString());
                        importe2erVencimiento = importe1erVencimiento + (importe1erVencimiento * recargoDefault / 100);
                    }

                    for (int i = 0; i < cursada.cantidadCuotas; i++)
                    {
                        var cuota = new Cuota();
                        cuota.CodCon = cursada.oferta1.curso1.precio.codigo;
                        cuota.CodPlan = 1;
                        cuota.Dni = (long)cursada.AspNetUsers.NroDocumento;
                        cuota.Estado = "0";

                        var saltoMesesCuota = i;

                        //Mensual
                        if (cursada.oferta1.frecuenciaCuota.Value.Equals(1))
                        {
                            saltoMesesCuota = i;
                        }
                        //Bimestral
                        else if (cursada.oferta1.frecuenciaCuota.Value.Equals(2))
                        {
                            saltoMesesCuota = i * 2;
                        }
                        //Trimestral
                        else if (cursada.oferta1.frecuenciaCuota.Value.Equals(3))
                        {
                            saltoMesesCuota = i * 3;
                        }

                        //Generación de Cuotas dependiendo del salto entre las Cuotas
                        if (cursada.oferta1.fechaPrimerCuota.HasValue)
                        {
                            //Fecha de 1er Vencimiento
                            cuota.fechavto = cursada.oferta1.fechaPrimerCuota.Value.AddMonths(saltoMesesCuota);

                            //Fecha de 2do Vencimiento
                            if (cursada.oferta1.diasSegundoVencimiento.HasValue)

                                cuota.fechavto2 = cursada.oferta1.fechaPrimerCuota.Value.AddMonths(saltoMesesCuota).AddDays(cursada.oferta1.diasSegundoVencimiento.Value);
                            else
                                cuota.fechavto2 = cursada.oferta1.fechaPrimerCuota.Value.AddMonths(saltoMesesCuota).AddDays(10);
                        }
                        else
                        {
                            //Fecha de 1er Vencimiento
                            //cuota.fechavto = DateTime.Now.AddMonths(saltoMesesCuota);
                            cuota.fechavto = cursada.oferta1.periodolectivo1.desdeFecha.AddMonths(saltoMesesCuota);

                            //Fecha de 2do Vencimiento
                            if (cursada.oferta1.diasSegundoVencimiento.HasValue)
                                //cuota.fechavto2 = DateTime.Now.AddMonths(saltoMesesCuota).AddDays(cursada.oferta1.diasSegundoVencimiento.Value);
                                cuota.fechavto2 = cursada.oferta1.periodolectivo1.desdeFecha.AddMonths(saltoMesesCuota).AddDays(cursada.oferta1.diasSegundoVencimiento.Value);
                            else
                                //cuota.fechavto2 = DateTime.Now.AddMonths(saltoMesesCuota).AddMonths(saltoMesesCuota).AddDays(10);
                                cuota.fechavto2 = cursada.oferta1.periodolectivo1.desdeFecha.AddMonths(saltoMesesCuota).AddDays(10);
                        }

                        cuota.Importe = importe1erVencimiento;
                        cuota.Importe2 = importe2erVencimiento;
                        cuota.Motivo = "";
                        cuota.NroComision = cursada.oferta;
                        cuota.NroCuota = i + 1;
                        //if (cursada.oferta1.curso1.codCurso.HasValue)
                        //    cuota.NroCurso = cursada.oferta1.curso1.codCurso.Value;
                        //else
                        cuota.NroCurso = cursada.oferta1.curso1.codigo;
                        cuota.NroFactura = cursada.codigo.ToString() + "_" + (i + 1);
                        cuota.NroRec = 0;
                        cuota.Origen = ConfigurationManager.AppSettings["OrigenTeso"];
                        cuota.TotalCuota = cursada.cantidadCuotas.Value;

                        if (i == 0)
                        {
                            if (cuota.fechavto > DateTime.Now)
                            {
                                int dias = int.Parse(ConfigurationManager.AppSettings["Dias1erCuota"].ToString());
                                var fecha = DateTime.Now.AddDays(dias);
                                fecha1er = fecha;
                            }
                            else
                                fecha1er = cuota.fechavto.Value;
                            importe1er = cuota.Importe;
                        }

                        try
                        {
                            await CuotaData.generarCuota(cuota);
                        }
                        catch (Exception ex)
                        {

                        }

                    }

                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { ok = false, codigoCursa = cur.codigo });
                }
            }
            else
            {
                try
                {
                    gratuito.estado = (int)EstadosCursada.Confirmado;
                    db.Entry(gratuito).State = EntityState.Modified;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    return Json(new { ok = false, codigoCursa = cur.codigo });
                }

            }
            //return Json(new { ok = true, codigoCursa = cur.codigo, gratis = gratuito.oferta1.gratuito });

            cur = db.cursa.Find(cur.codigo);

            //return Json(new { ok = true, codigoCursa = cur.codigo, fechaInicio = cur.oferta1.periodolectivo1.desdeFecha.ToShortDateString() });

            return Json(new
            {
                ok = true,
                codigoCursa = cur.codigo,
                fechaInicio = cur.oferta1.fechaDesde.ToShortDateString(),
                fechaVencimiento = fecha1er.ToShortDateString(),
                importe = importe1er,
                sede = cur.oferta1.sede1.nombre,
                gratis = gratuito.oferta1.gratuito,
                preinscripcion = gratuito.oferta1.RequierePreInscripcion
            });
        }

        public ActionResult EliminarInscripcion(int? id, string callbackView = "", string usuario = "")
        {
            if (string.IsNullOrEmpty(callbackView))
                callbackView = "FormaDePago";
            try
            {
                var cursadaGuardada = db.cursa.Find(id);
                string alumno = cursadaGuardada.alumno;

                if (cursadaGuardada.estado == (int)EstadosCursada.Inscripto)
                {
                    var enc = db.Encuesta.Find(alumno);
                    db.Encuesta.Remove(enc);
                    db.cursa.Remove(cursadaGuardada);
                    db.SaveChanges();
                }

                var cursos = db.cursa.Where(x => x.alumno == alumno && (x.estado == (int)EstadosCursada.Inscripto));// || x.estado == (int)EstadosCursada.FormaPago));

                if (!string.IsNullOrEmpty(usuario))
                {
                    ViewBag.UsuarioCompleto = db.AspNetUsers.Find(usuario);
                    ViewBag.Usuario = usuario;
                }

                if (callbackView == "FormaDePago" && cursos.Count() == 0)
                //return RedirectToAction("inscripcion");
                {
                    if (User.IsInRole("AdministradorEmpresas"))
                    {
                        ViewBag.Empresa = true;
                    }
                    else
                    {
                        if (User.IsInRole("AdministradorGraduado"))
                        {
                            ViewBag.Empresa = false;
                        }
                        else
                        {
                            if (User.IsInRole("Alumno"))
                            {
                                return RedirectToAction("inscripcion");
                            }
                            ViewBag.Empresa = false;
                        }

                    }

                    return RedirectToAction("InscribirAlumno", new { Id = alumno });
                }
                else
                    return View(callbackView, cursos);


            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                ViewBag.ErrorMessage = string.Format(
                    Strings.ErrorIntentarInsertarRegistro,
                    e.Message.Replace(Strings.TheStatementHasBeenTerminated, "")
                );
            }
            return View(callbackView);

        }

        public ActionResult EliminarInscripcionCompleta(string id)
        {

            var ofertaDisponible = db.ofertaDisponiblePorAlumno4(id).ToList();
            var ofertasDelAlumno = ofertaDisponible.Where(x => x.selected == "selected");
            foreach (var oferta in ofertasDelAlumno)
            {
                var cursada = db.cursa.Where(x => x.alumno == id && x.oferta == oferta.codigo).ToList();
                foreach (cursa cursadas in cursada)
                {
                    if (cursadas.estado == (int)EstadosCursada.Inscripto || cursadas.estado == (int)EstadosCursada.PreInscripcion)
                    {
                        var enc = db.Encuesta.Find(id);
                        if (enc != null)
                        {
                            db.Encuesta.Remove(enc);
                           
                        }
                        db.cursa.Remove(cursadas);
                    }
                }

            }
            db.SaveChanges();

            return RedirectToAction("Inscripcion", new { id = id });
        }

        public int Edad(DateTime fechaNacimiento)
        {
            //Obtengo la diferencia en años.

            if (fechaNacimiento != null)
            {
                int edad = DateTime.Now.Year - fechaNacimiento.Year;

                //Obtengo la fecha de cumpleaños de este año.
                DateTime nacimientoAhora = fechaNacimiento.AddYears(edad);
                //Le resto un año si la fecha actual es anterior al día de nacimiento.
                if (DateTime.Now.CompareTo(nacimientoAhora) < 0)
                {
                    edad--;
                }

                return edad;

            }
            else
            {
                return 0;
            }
        }

        public string traerPreInscripcion(List<cursa> cursos)
        {
            var documentacion = "";
            foreach (cursa uno in cursos)
            {
                if (uno.oferta1.RequierePreInscripcion == true)
                    documentacion += "\n\nDocumentación a presentar: " + uno.oferta1.descripcion + "\nFecha de entrega: " + uno.oferta1.fechaDocumentacion + "\nHora de entrega: " + uno.oferta1.horaDocumentacion + "\n" + "\n" + uno.oferta1.documentacion + ".\n\n";
            }

            return documentacion;
        }

        //public ActionResult EliminarInscripcion(int? id)
        //{
        //    var cursadaGuardada = db.cursa.Find(id);
        //    db.cursa.Remove(cursadaGuardada);
        //    db.SaveChanges();

        //    var cursos = db.cursa.Where(x => x.alumno == cursadaGuardada.alumno && (x.estado == (int)EstadosCursada.Inscripto));// || x.estado == (int)EstadosCursada.FormaPago));
        //    return View("FormaDePago", cursos);
        //}

        #endregion

        #region preinscripcion
        [HasPermission("cursa_Index")]
        public ActionResult PartialGridPreInscripcion(int? dni = null, int? IdPeriodo = null)
        {
            IQueryable<cursa> cursadas = db.cursa.Where(x => x.estado == 9);


            if (dni != null)
            {
                cursadas = db.cursa.Where(x => x.estado == 9 && x.AspNetUsers.NroDocumento.Value.ToString().Contains(dni.ToString()));
            }

            if (IdPeriodo != null && IdPeriodo != -1)
            {
                if (dni != null)
                {
                    cursadas = db.cursa.Where(x => x.estado == 9 && x.oferta1.periodolectivo1.codigo == IdPeriodo.Value && x.AspNetUsers.NroDocumento == dni);
                }
                else
                {
                    cursadas = db.cursa.Where(x => x.estado == 9 && x.oferta1.periodolectivo1.codigo == IdPeriodo.Value);
                }
            }

            if (User.IsInRole("AdministradorEmpresas"))
            {
                cursadas = cursadas.Where(x => /*x.oferta1.curso1.tipocurso1.codigo.Equals(23)*/ x.oferta1.curso1.tipocurso1.sector == (int)Sectores.Empresas);
            }
            else
            {
                if (User.IsInRole("AdministradorGraduado"))
                {
                    cursadas = cursadas.Where(x => /*x.oferta1.curso1.tipocurso1.codigo.Equals(23)*/x.oferta1.curso1.tipocurso1.sector == (int)Sectores.Graduados);
                }
                else
                {
                    cursadas = cursadas.Where(x => /*!x.oferta1.curso1.tipocurso1.codigo.Equals(23)*/x.oferta1.curso1.tipocurso1.sector == (int)Sectores.Idiomas);
                }

            }

            //var cursadas = db.cursa.Where(x => x.alumno.Equals(dni.ToString())).ToList();
            return PartialView("_GridPreInscripcion", cursadas.ToList());
        }

        [HasPermission("cursa_Index")]
        public ActionResult ConfirmarInscripcion(int? id = null, string usuario = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cursa cursa = db.cursa.Where(x => x.codigo == id && x.alumno.Equals(usuario)).First();
            if (cursa == null)
            {
                return HttpNotFound();
            }

            try
            {
                ModelState.Remove("baja");
                db.Configuration.ValidateOnSaveEnabled = false;
                cursa.estado = 0;
                db.SaveChanges();
                return Json(new { ok = true });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false });

            }


        }

        [HasPermission("Alumnos_Index")]
        public ActionResult ConfirmarPreInscripcion()
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

            return View();
        }
        #endregion

        #region ABM
        // GET: cursa/Details/5
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

        // GET: cursa/Create
        public ActionResult Create()
        {
            ViewBag.estado = new SelectList(db.EstadoCursa, "codigo", "descripcion");
            ViewBag.oferta = new SelectList(db.oferta, "codigo", "codigo");
            ViewBag.alumno = new SelectList(db.AspNetUsers, "Id", "Email");
            return View();
        }

        // POST: cursa/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "oferta,fechaAlta,aprobo,observacion,alumno,codigo,estado")] cursa cursa)
        {
            if (ModelState.IsValid)
            {
                db.cursa.Add(cursa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.estado = new SelectList(db.EstadoCursa, "codigo", "descripcion", cursa.estado);
            ViewBag.oferta = new SelectList(db.oferta, "codigo", "codigo", cursa.oferta);
            ViewBag.alumno = new SelectList(db.AspNetUsers, "Id", "Email", cursa.alumno);
            return View(cursa);
        }

        // GET: cursa/Edit/5
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
            ViewBag.oferta = new SelectList(db.oferta, "codigo", "codigo", cursa.oferta);
            ViewBag.alumno = new SelectList(db.AspNetUsers, "Id", "Email", cursa.alumno);
            return View(cursa);
        }

        // POST: cursa/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "oferta,fechaAlta,aprobo,observacion,alumno,codigo,estado")] cursa cursa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cursa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.estado = new SelectList(db.EstadoCursa, "codigo", "descripcion", cursa.estado);
            ViewBag.oferta = new SelectList(db.oferta, "codigo", "codigo", cursa.oferta);
            ViewBag.alumno = new SelectList(db.AspNetUsers, "Id", "Email", cursa.alumno);
            return View(cursa);
        }

        // GET: cursa/Delete/5
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

        // POST: cursa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            cursa cursa = db.cursa.Find(id);
            db.cursa.Remove(cursa);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion

        #region nivelacion

        public ActionResult PartialGrid(string texto = "", int? IdCurso = null)
        {
            //List<AspNetUsers> usuarios = db.AspNetUsers.ToList();

            IQueryable<AspNetUsers> alumnos = db.AspNetUsers;

            List<AspNetUsers> user = null;

            if (!String.IsNullOrEmpty(texto))
            {
                alumnos = alumnos.Where(u => u.NroDocumento.Value.ToString().Contains(texto));
                user = alumnos.ToList();
            }
            if (user != null && user.Count() > 0)
            {
                var nivelaciones = user.First().nivelacion.ToList();

                if (User.IsInRole("AdministradorEmpresas"))
                {
                    nivelaciones = nivelaciones.Where(x => /*x.curso.tipocurso1.codigo.Equals(23)*/x.curso.tipocurso1.sector == (int)Sectores.Empresas).ToList();
                }
                else
                {
                    if (User.IsInRole("AdministradorGraduado"))
                    {
                        nivelaciones = nivelaciones.Where(x => /*x.curso.tipocurso1.codigo.Equals(23)*/x.curso.tipocurso1.sector == (int)Sectores.Graduados).ToList();
                    }
                    else
                    {
                        nivelaciones = nivelaciones.Where(x => /*!x.curso.tipocurso1.codigo.Equals(23)*/x.curso.tipocurso1.sector == (int)Sectores.Idiomas).ToList();
                    }

                }

                return PartialView("_Grid", nivelaciones);
            }
            else
                return PartialView("_Grid", new List<nivelacion>());
        }

        [HttpPost]
        public ActionResult EstablecerNivel(string texto, string IdIdCurso = "")
        {
            int ultimo = Request.Url.ToString().LastIndexOf('/');
            string usuario = Request.Url.ToString().Substring(ultimo + 1);


            if (ModelState.IsValid)
            {
                var codigos = Request.Params["codigo"].Split(','); //ofertas seleccionadas

                foreach (var codigo in codigos) //inserto cursadas nuevas
                {
                    if (!string.IsNullOrEmpty(codigo) && codigo != Constantes.ERROR.ToString())
                    {
                        int codigoOferta = int.Parse(codigo);


                        var cursadasInscripto = db.nivelacion.Where(x => x.codigo_usuario == usuario && x.codigo_curso == codigoOferta);//verifico si es una cursada nueva o un update
                        nivelacion cursaInscripto;
                        if (cursadasInscripto.Count() == 0)
                            cursaInscripto = new nivelacion();
                        else
                            cursaInscripto = cursadasInscripto.First();


                        cursaInscripto.codigo_usuario = usuario;
                        cursaInscripto.codigo_curso = codigoOferta;
                        cursaInscripto.fecha = DateTime.Now;



                        if (cursadasInscripto.Count() == 0)
                            db.nivelacion.Add(cursaInscripto);

                    }
                }

                db.SaveChanges();
                return RedirectToAction("FormaDePago", new { Id = usuario });
            }


            return View();
        }

        [HasPermission("Alumnos_Index")]
        public ActionResult EstablecerNivel(string Id) //id de usuario
        {
            if (Id == null || string.IsNullOrEmpty(Id)) // si es nulo entonces inscribo al usuario logueado
            {
                Id = ApplicationUser.GetId();
            }

            //var cursos = db.cursa.Where(x => x.alumno == Id && (x.estado == (int)EstadosCursada.Inscripto || x.estado == (int)EstadosCursada.FormaPago));


            return View();
        }

        [HasPermission("Alumnos_Index")]
        public ActionResult EstablecerNivelGrid(string texto, int? IdCurso = null)
        {

            if (ModelState.IsValid)
            {

                if (!string.IsNullOrEmpty(texto) && texto != Constantes.ERROR.ToString() && IdCurso.HasValue && IdCurso.Value != Constantes.ERROR)
                {
                    var cursadasInscripto = db.nivelacion.Where(x => x.codigo_curso == IdCurso && x.AspNetUsers.NroDocumento.Value.ToString().Contains(texto)).ToList();
                    nivelacion cursaInscripto;
                    if (cursadasInscripto.Count() == 0)
                        cursaInscripto = new nivelacion();
                    else
                        cursaInscripto = cursadasInscripto.First();


                    cursaInscripto.codigo_usuario = db.AspNetUsers.Where(x => x.NroDocumento.Value.ToString().Contains(texto)).ToList().First().Id;
                    cursaInscripto.codigo_curso = IdCurso.Value;
                    cursaInscripto.fecha = DateTime.Now;
                    cursaInscripto.AspNetUsers = db.AspNetUsers.Where(x => x.NroDocumento.Value.ToString().Contains(texto)).ToList().First();
                    //cursaInscripto.curso = db.curso.Find(IdCurso.Value);


                    if (cursadasInscripto.Count() == 0)
                        db.nivelacion.Add(cursaInscripto);

                    db.SaveChanges();
                    return Json(new { ok = true });
                }


                //return Json(new { ok = true });
                //return RedirectToAction("FormaDePago", new { Id = db.AspNetUsers.Where(x => x.NroDocumento.Value.ToString().Contains(texto)).ToList().First().Id });
            }


            return PartialView("EstablecerNivel");
        }

        [HasPermission("Alumnos_Index")]
        public ActionResult EliminarNivelGrid(string texto, int? IdCurso = null)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(texto) && texto != Constantes.ERROR.ToString() || IdCurso.HasValue && IdCurso.Value != Constantes.ERROR)
                {

                    var cursadasInscripto = db.nivelacion.Where(x => x.codigo_curso == IdCurso && x.AspNetUsers.NroDocumento.Value.ToString().Contains(texto));
                    nivelacion cursaInscripto;
                    if (cursadasInscripto.Count() == 0)
                        //cursaInscripto = new nivelacion();
                        return HttpNotFound();
                    else
                        cursaInscripto = cursadasInscripto.First();

                    if (cursadasInscripto.Count() != 0)
                        db.nivelacion.Remove(cursaInscripto);

                }

                db.SaveChanges();
                return Json(new { ok = true });
            }
            return View("EstablecerNivel");
        }

        #endregion

        #region AJAX
        [HttpGet]
        public JsonResult Buscar(string q)
        {
            IQueryable<curso> cursos = db.curso;


            var model = new ViewModels.ViewNivelar();

            if (User.IsInRole("AdministradorEmpresas"))
            {
                model.cursos = db.curso.Where(x => x.descripcion.Contains(q) && /*x.tipocurso1.codigo.Equals(23)*/x.tipocurso1.sector == (int)Sectores.Empresas).ToList();
            }
            else
            {
                if (User.IsInRole("AdministradorGraduado"))
                {
                    model.cursos = db.curso.Where(x => x.descripcion.Contains(q) &&/* x.tipocurso1.codigo.Equals(23)*/x.tipocurso1.sector == (int)Sectores.Graduados).ToList();
                }
                else
                {
                    model.cursos = db.curso.Where(x => x.descripcion.Contains(q) && /*!x.tipocurso1.codigo.Equals(23)*/x.tipocurso1.sector == (int)Sectores.Idiomas).ToList();
                }

            }

            //model.cursos = CursoData.Buscar(q).ToList();


            var rta = model.cursos.Select(i => new { id = i.codigo, text = i.descripcion }).ToList();
            return Json(rta, JsonRequestBehavior.AllowGet);
        }
        [HasPermission("Baja_Cursada")]
        public async Task<ActionResult> BajaCursada(int? IdCursa = null)
        {
            if (IdCursa.HasValue)
            {
                var cursada = db.cursa.Find(IdCursa);
                if (cursada != null)
                {
                    if (User.IsInRole("Administrador") || User.IsInRole("SuperAdministrador"))
                    {

                        if (cursada.estado == (int)EstadosCursada.BajaAceptada)  //si es administrador y estaba de baja la vuelve a poner al estado que estaba antes de la baja
                        {
                            db.Configuration.ValidateOnSaveEnabled = false;
                            if (cursada.EstadoAntBaja != null && cursada.EstadoAntBaja != (int)EstadosCursada.SolicitudBaja)
                                cursada.estado = cursada.EstadoAntBaja;
                            else
                            {
                                if (cursada.EstadoAntBaja == (int)EstadosCursada.SolicitudBaja)
                                {
                                    cursada.estado = cursada.EstadoAntBaja;
                                    cursada.EstadoAntBaja = (int)EstadosCursada.FormaPago;
                                }
                                else
                                {
                                    cursada.estado = (int)EstadosCursada.FormaPago;
                                }

                            }

                            db.SaveChanges();
                            await CuotaData.BajaCuotaPorCursadaRestaurar(IdCursa.Value);

                            return Json(new { ok = true });
                        }
                    }
                    db.Configuration.ValidateOnSaveEnabled = false;
                    if (cursada.EstadoAntBaja != (int)EstadosCursada.BajaAceptada /*&& cursada.estado != (int)EstadosCursada.SolicitudBaja*/)
                        cursada.EstadoAntBaja = cursada.estado;
                    cursada.estado = (int)EstadosCursada.BajaAceptada;
                    db.SaveChanges();
                    await CuotaData.BajaCuotaPorCursada(IdCursa.Value);
                }
            }

            return Json(new { ok = true });
        }

        [HasPermission("Baja_Cursada")]
        public ActionResult EliminarCursada(int? IdCursa = null)
        {
            if (User.IsInRole("Administrador") || User.IsInRole("SuperAdministrador"))
            {



                if (IdCursa.HasValue)
                {
                    var cursada = db.cursa.Find(IdCursa);

                    List<Cuota> cuotas = CuotaData.consultarCuotas(cursada.alumno.ToString(), null, cursada.oferta1.curso).Where(x => x.NroFactura.Contains(cursada.codigo.ToString()) && x.Estado.Equals("B")).ToList();


                    if (cuotas.Count != 0)
                    {
                        /*db.Configuration.ValidateOnSaveEnabled = false;
                        cursada.estado = (int)EstadosCursada.BajaAceptada;*/
                        var asistencia = db.asistencia.Where(x => x.cursa == IdCursa);
                        db.asistencia.RemoveRange(asistencia);
                        db.cursa.Remove(cursada);
                        db.SaveChanges();
                        //await CuotaData.BajaCuotaPorCursada(IdCursa.Value);
                    }
                    else
                    {
                        return Json(new { ok = false });
                    }
                }

                return Json(new { ok = true });
            }
            else
            {
                return Json(new { ok = "sin permiso" });
            }
        }
        #endregion

        #region Documentacion
        public ActionResult Documentacion(string Id, string Us)
        {

            //List<cursa> cursos = db.cursa.Where(x => x.oferta == Id && (x.estado == (int)EstadosCursada.Inscripto || x.estado == (int)EstadosCursada.FormaPago)).ToList();
            ViewBag.Id = Id;
            ViewBag.Usuario = Us;
            return View();
            //return View(cursos);

        }

        public ActionResult PartialGridDocumentacion(string Id, string Us)
        {
            string[] codOferta = Id.Split('-');

            List<cursa> cursos = new List<cursa>();
            for (int i = 0; i < codOferta.Count() - 1; i++)
            {
                int cod = int.Parse(codOferta[i].ToString());
                cursa[] dato = db.cursa.Where(x => x.oferta == cod && x.alumno.Equals(Us) && x.estado == (int)EstadosCursada.PreInscripcion/*(x.estado == (int)EstadosCursada.Inscripto || x.estado == (int)EstadosCursada.FormaPago)*/).ToArray();
                cursos.AddRange(dato);

            }



            return PartialView("_GridDocumentacion", cursos);
        }
        #endregion

        #region BajaCursada
        [HasPermission("cuota_Index")]
        public ActionResult SolicitarBajadeCursada()
        {
            return View();
        }

        [HasPermission("cuota_Index")]
        public ActionResult MisCursadas()
        {
            return View();
        }

        public ActionResult PartialGridBajaCursada(string id)
        {
            if (id == null || string.IsNullOrEmpty(id)) // si es nulo entonces inscribo al usuario logueado
            {
                id = ApplicationUser.GetId();
            }

            IQueryable<cursa> cursadas = db.cursa.Where(x => (x.estado != 5 && x.estado != 6) && x.alumno.Equals(id) /*&& x.oferta1.fechaHasta > DateTime.Today*/);
            // cursadas.First().oferta1.periodolectivo1.descripcion
            //var cursadas = db.cursa.Where(x => x.alumno.Equals(dni.ToString())).ToList();
            return PartialView("_GridBajaCursada", cursadas.ToList());
        }

        public ActionResult PartialGridMisCursos(string id)
        {
            if (id == null || string.IsNullOrEmpty(id)) // si es nulo entonces inscribo al usuario logueado
            {
                id = ApplicationUser.GetId();
            }

            List<cursa> cursadas = db.cursa.Where(x => x.alumno.Equals(id)).ToList();
            List<cursa> aMostrar = db.cursa.Where(x => x.alumno.Equals(id)).ToList();

            foreach (cursa cursada in cursadas)
            {
                if (aMostrar.Find(m => m.codigo.Equals(cursada.codigo)).ResultadoCursa.codigo == 0)
                {
                    aMostrar.Find(x => x.codigo.Equals(cursada.codigo)).observacion = cursada.nota;
                }


                ////if (aMostrar.Find(m => m.alumno.Equals(cursada.alumno)).aprobo == true)
                ////{
                //if (cursada.aprobo == true)
                //{
                //    aMostrar.Find(x => x.alumno == cursada.alumno).observacion = cursada.nota;
                //}
                ////aMostrar.Find(x => x.alumno == cursada.alumno).observacion = "todos putos";
                ////}
            }

            return PartialView("_GridMisCursos", aMostrar.ToList());
        }

        public ActionResult DarBajaCursada(int? codigo = null)
        {
            /*if (codigo == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            cursa cursa = db.cursa.Where(x => x.codigo == codigo).First();
            if (cursa == null)
            {
                return HttpNotFound();
            }

            try
            {
                cursa.estado = 5;
                db.SaveChanges();
                return Json(new { ok = true });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false });

            }*/

            if (codigo == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            cursa cursa = db.cursa.Find(codigo);
            if (cursa == null)
            {
                return HttpNotFound();
            }

            ViewBag.motivosBaja = new SelectList(
               db.motivoBaja.ToList()
               .ToSelectList(
                   x => x.codigo + " - " + x.descripcion,
                   x => x.codigo.ToString(),
                   Strings.SeleccionarMotivoBaja,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
               /*" ",
               "",
               ""*/
               )
           , "Value", "Text");

            return View("DetalleBajaCursada", cursa);
        }

        [HttpPost]
        public ActionResult DetalleBajaCursada(cursa curso)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //db.Entry(curso).State = EntityState.Modified;
                    cursa cursa = db.cursa.Where(x => x.codigo == curso.codigo).First();
                    if (cursa == null)
                    {
                        return HttpNotFound();
                    }
                    cursa.EstadoAntBaja = cursa.estado;
                    cursa.estado = 5;
                    cursa.resultadoCursada = 5;
                    cursa.baja = curso.baja;
                    cursa.DescripcionBaja = curso.DescripcionBaja;
                    db.SaveChanges();

                    int sede = cursa.oferta1.sede.Value;
                    string MailSede = "";
                    string linkBaja = "LinkAvisoBaja";
                    string adminCurso = cursa.oferta1.curso1.AdminCurso;
                    string adminCurso2 = cursa.oferta1.curso1.AdminCurso2;
                    string adminCurso3 = cursa.oferta1.curso1.AdminCurso3;
                    string adminCurso4 = cursa.oferta1.curso1.AdminCurso4;

                    int documeAdminCurso = 0;
                    int documeAdminCurso2 = 0;
                    int documeAdminCurso3 = 0;
                    int documeAdminCurso4 = 0;

                    if (!string.IsNullOrEmpty(adminCurso))
                    {
                        documeAdminCurso = db.AspNetUsers.Where(x => x.Id.Equals(adminCurso)).First().NroDocumento.Value;
                    }
                    if (!string.IsNullOrEmpty(adminCurso2))
                    {
                        documeAdminCurso2 = db.AspNetUsers.Where(x => x.Id.Equals(adminCurso2)).First().NroDocumento.Value;
                    }
                    if (!string.IsNullOrEmpty(adminCurso3))
                    {
                        documeAdminCurso3 = db.AspNetUsers.Where(x => x.Id.Equals(adminCurso3)).First().NroDocumento.Value;
                    }
                    if (!string.IsNullOrEmpty(adminCurso4))
                    {
                        documeAdminCurso4 = db.AspNetUsers.Where(x => x.Id.Equals(adminCurso4)).First().NroDocumento.Value;
                    }


                    if (sede == 19) //San justo
                    {
                        int tipoCurso = cursa.oferta1.curso1.tipoCurso;

                        if (tipoCurso == 14) // idiomas
                        {
                            MailSede = "MailAvisoBajaSanJustoIdiomas";
                        }
                        else
                        { //Extension
                            if (documeAdminCurso == 34454017)//magali
                            {
                                MailSede = "MailAvisoBajaSanJustoExtensionMagali";
                            }
                            else
                            {
                                if (documeAdminCurso == 33026376 || documeAdminCurso2 == 26539747 || documeAdminCurso == 26539747)
                                {
                                    MailSede = "MailAvisoBajaSanJustoExtensionGallegoCastro";
                                }
                            }


                        }

                    }
                    else
                    {
                        if (sede == 23) //congreso
                        {
                            MailSede = "MailAvisoBajaCongreso";

                        }
                        else
                        {
                            if (sede == 24) //ituzaingo
                            {
                                MailSede = "MailAvisoBajaItuzaingo";

                            }
                            else
                            {
                                if (sede == 25) //la ferrere
                                {
                                    MailSede = "MailAvisoBajaLaFerrere";

                                }
                                else
                                {
                                    if (sede == 28) //San Justo 

                                    {
                                        MailSede = "ComunicacionesMailGraduado";
                                        linkBaja = "LinkAvisoBajaGraduados";
                                    }
                                }
                            }

                        }
                    }

                    cnrl.Logica.App.enviarMailUsuario(ConfigurationManager.AppSettings[MailSede], "Solicitud de Baja", "El alumno "
         + cursa.AspNetUsers.Apellido + ", " + cursa.AspNetUsers.Nombre + " Dni: " + cursa.AspNetUsers.NroDocumento + "<br> "
         + "Solicitó la baja del curso " + cursa.oferta1.curso1.descripcion + ", codigo de curso " + cursa.oferta1.curso1.codCurso + "<br><br> "
         + "Presione el siguiente link para confirmar la solicitud de baja " + "<a href=" + ConfigurationManager.AppSettings[linkBaja] + ">" + ConfigurationManager.AppSettings[linkBaja] + "</a>");


                    string cod = curso.codigo.ToString();
                    string cod2 = "";
                    int longitud = cod.Length;
                    for (int i = 0; i < 10 - longitud; i++)
                    {
                        cod2 += "0";
                    }
                    cod2 = cod2 + cod;
                    return Json(new { ok = true, codigo = cod2 });

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


            ViewBag.Callback = "GuardadoOk";
            ViewBag.motivosBaja = new SelectList(
            db.motivoBaja.ToList()
            .ToSelectList(
                x => x.codigo + " - " + x.descripcion,
                x => x.codigo.ToString(),
                 Strings.SeleccionarMotivoBaja,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
            )
        , "Value", "Text");



            cursa cursos = db.cursa.Find(curso.codigo);
            return View("DetalleBajaCursada", cursos);
        }

        #endregion

        #region cursadasVencidas

        [HasPermission("cursaVencidas_Index")]
        public ActionResult CursadasVencidas()
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

            return View();// cursa.ToList());
        }


        [HasPermission("cursaVencidas_Index")]
        public ActionResult PartialGridCursadasVencidas(int? IdSede = null, int? IdCurso = null, string IdUsuario = "", int? IdPeriodo = null, int? IdOferta = null, int? Estado = null, int? IdTipoCurso = null, string IdCarrera = null, int? diasVencido = null)
        {
            List<cursa> cursadas = new List<cursa>();

            if (IdPeriodo.HasValue && IdPeriodo.Value != Constantes.ERROR)
                cursadas = consultar(IdSede, IdCurso, IdUsuario, IdPeriodo, IdOferta, Estado, IdTipoCurso, IdCarrera, diasVencido);
            ViewBag.Cantidad = cursadas.Count;
            return PartialView("_GridCursadasVencidas", cursadas);

        }

        [HasPermission("cursaVencidas_Index")]
        [HttpPost]
        public ActionResult consultarCantidadCursadas(int? IdSede = null, int? IdCurso = null, string IdUsuario = "", int? IdPeriodo = null, int? IdOferta = null, int? Estado = null, int? IdTipoCurso = null, string IdCarrera = null, int? diasVencido = null)
        {
            List<cursa> cursadas = new List<cursa>();

            if (IdPeriodo.HasValue && IdPeriodo.Value != Constantes.ERROR)
                cursadas = consultar(IdSede, IdCurso, IdUsuario, IdPeriodo, IdOferta, Estado, IdTipoCurso, IdCarrera, diasVencido);

            return Json(new { cantidad = cursadas.Count() });

        }

        [HasPermission("cursaVencidas_Index")]
        [HttpPost]
        public async Task<ActionResult> BajaCursadaMasiva(int? IdSede = null, int? IdCurso = null, string IdUsuario = "", int? IdPeriodo = null, int? IdOferta = null, int? Estado = null, int? IdTipoCurso = null, string IdCarrera = null, int? diasVencido = null)
        {

            try
            {
                List<cursa> cursadas = new List<cursa>();
                db.Configuration.ValidateOnSaveEnabled = false;

                if (IdPeriodo.HasValue && IdPeriodo.Value != Constantes.ERROR)
                    cursadas = consultar(IdSede, IdCurso, IdUsuario, IdPeriodo, IdOferta, Estado, IdTipoCurso, IdCarrera, diasVencido);
                cursadas.ForEach(x => x.estado = (int)EstadosCursada.BajaAceptada);

                db.SaveChanges();
                foreach (cursa item in cursadas)
                {
                    await CuotaData.BajaCuotaPorCursada(item.codigo);
                }


                return Json(new { ok = true });
            }
            catch
            {
                return Json(new { ok = false });
            }
        }

        public ActionResult ExportarVencidas(int? IdSede = null, int? IdCurso = null, string hiddenUsuario = "", int? IdPeriodo = null, int? IdOferta = null, int? Estado = null, int? IdTipoCurso = null, string IdCarrera = null, int? diasVencido = null)
        {
            List<cursa> cursadas = new List<cursa>();

            if (IdPeriodo.HasValue && IdPeriodo.Value != Constantes.ERROR)
                cursadas = consultar(IdSede, IdCurso, hiddenUsuario, IdPeriodo, IdOferta, Estado, IdTipoCurso, IdCarrera, diasVencido);


            ExportSettings.DefaultExportType = ExportType.WYSIWYG;
            return GridViewExtension.ExportToXlsx(CrearGridViewSettingsExportarVencidas("Cursadas"), cursadas, true);
        }

        private static GridViewSettings CrearGridViewSettingsExportarVencidas(string titulo)
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

            settings.Columns.Add(col => { col.FieldName = "fechaAlta"; col.Caption = "Fecha"; col.PropertiesEdit.DisplayFormatString = "d"; col.Width = Unit.Pixel(25); });
            settings.Columns.Add(col => { col.FieldName = "fechaVencimientoCupon"; col.Caption = "Fecha Vto Cupón"; col.PropertiesEdit.DisplayFormatString = "d"; col.Width = Unit.Pixel(25); });

            settings.Columns.Add(col => { col.FieldName = "AspNetUsers.NroDocumento"; col.Caption = "N° Doc"; });
            settings.Columns.Add(col => { col.FieldName = "AspNetUsers.Nombre"; col.Caption = "Nombre"; });
            settings.Columns.Add(col => { col.FieldName = "AspNetUsers.Apellido"; col.Caption = "Apellido"; });
            settings.Columns.Add(col => { col.FieldName = "AspNetUsers.Telefono"; col.Caption = "Telefono"; });
            settings.Columns.Add(col => { col.FieldName = "AspNetUsers.Email"; col.Caption = "Email"; });

            settings.Columns.Add(col => { col.FieldName = "oferta1.curso1.tipocurso1.descripcion"; col.Caption = Strings.ColTipoCurso; });
            settings.Columns.Add(col => { col.FieldName = "oferta1.curso1.descripcion"; col.Caption = Strings.ColCurso; });
            settings.Columns.Add(col => { col.FieldName = "oferta1.sede1.nombre"; col.Caption = Strings.ColSede; });
            settings.Columns.Add(col => { col.FieldName = "oferta1.periodolectivo1.descripcion"; col.Caption = Strings.ColPeriodo; });
            settings.Columns.Add(col => { col.FieldName = "oferta1.horario"; col.Caption = "Horario"; });

            settings.Columns.Add(col => { col.FieldName = "descripcion"; col.Caption = "Aprobado"; col.Width = Unit.Pixel(25); });
            settings.Columns.Add(col => { col.FieldName = "EstadoCursa.descripcion"; col.Caption = "Estado"; col.Width = Unit.Pixel(25); });

            return settings;
        }

        #endregion

        public ActionResult Alumnos(int? id)
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

            if (cursa.estado.Value.Equals(0))
            {
                //ModelState.AddModelError(string.Empty, "Ocurrió un error.");
                ViewBag.Bandera = false;
                ViewBag.ErrorMessage = string.Format("No se podrá asignar una beca para esta persona, es necesario generar un plan de pago primero, revise e intente nuevamente.") + Environment.NewLine;
            }
            else
                ViewBag.Bandera = true;

            return View(cursa);
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
