using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using cnrl.Helpers;
using cnrl.Logica;
using DevExpress.Export;
using DevExpress.Web.Mvc;
using System.Web.UI.WebControls;
using cnrl.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace cnrl.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private socioculturalesEntities db = new socioculturalesEntities();

        //public JsonResult IsUserExists(string UserName)
        //{
        //    //check if any of the UserName matches the UserName specified in the Parameter using the ANY extension method.  
        //    return Json(!db.AspNetUsers.Any(x => x.UserName == UserName), JsonRequestBehavior.AllowGet);
        //}

        //[HasPermission("inscripcion")]
        //public ActionResult Categoria(string id) //id de usuario
        //{
        //    //if (id == null || string.IsNullOrEmpty(id)) // si es nulo entonces inscribo al usuario logueado
        //    //{
        //    //    id = ApplicationUser.GetId();
        //    //}
        //    //
        //    //var usuario = db.AspNetUsers.Find(id);

        //    //ViewBag.TieneDeuda = CuotaData.TieneDeudaAlumnoVencida(usuario.NroDocumento.Value);

        //    ViewBag.Usuario = id;
        //    ViewBag.categoria = true;
        //    //ViewBag.UsuarioCompleto = usuario;
        //    return View("Categoria");
        //}

        // GET: cursos
        //[HasPermission("prueba")]
        [HasPermission("Usuarios_Index")]
        public ActionResult Index()
        {
            //var curso = db.curso.Include(c => c.precio).Include(c => c.tipocurso1);

            PrepararViewBag();
            return View();
        }

        private void PrepararViewBag(int IdProvinciaSeleccionada = 0)
        {
            ViewBag.Roles = new SelectList(
                db.AspNetRoles.ToList()
                .ToSelectList(
                    x => x.Name,
                    x => x.Id.ToString(),
                    Strings.MsgTodosRoles,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.RolesDocentes = new SelectList(
               db.AspNetRoles.ToList().Where(x => x.Name == "Ventanilla" || x.Name == "Docente" || x.Name == "CoordinadorDocente")
               .ToSelectList(
                   x => x.Name,
                   x => x.Id.ToString(),
                   "Seleccionar Rol",
                   Constantes.ERROR.ToString(),
                   Constantes.ERROR.ToString()
               )
           , "Value", "Text");

            ViewBag.TiposDocumento = new SelectList(db.tipodocumento, "codigo", "descripcion", "0");
            ViewBag.Provincias = new SelectList(
                db.provincia.ToList()
                .ToSelectList(
                    x => x.provincia1,
                    x => x.codigo.ToString(),
                    Strings.SeleccionarProvincia,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");


            ViewBag.Localidades = new SelectList(
                        db.localidad.Where(x => x.idProvincia == IdProvinciaSeleccionada && x.codigo != 50267).ToList()
                        .ToSelectList(
                            x => x.localidad1,
                            x => x.codigo.ToString(),
                            Strings.SeleccionarLocalidad,
                            Constantes.ERROR.ToString(),
                            Constantes.ERROR.ToString()
                        )
                    , "Value", "Text");



            IList<SelectListItem> list = Enum.GetValues(typeof(TiposAlumno)).Cast<TiposAlumno>().Select(x => new SelectListItem { Text = Logica.App.DisplayCamelCaseString(x.ToString()), Value = ((int)x).ToString() }).ToList().ToSelectList(
                x => x.Text,
                x => x.Value,
                Strings.SeleccionarTipoAlumno,
                Constantes.ERROR.ToString(),
                Constantes.ERROR.ToString()
                );


            ViewBag.TiposAlumno = new SelectList(list, "Value", "Text");
        }

        [ChildActionOnly]
        public ActionResult _Buscador(string SelectController, string SelectAction)
        {
            PrepararViewBag();
            ViewBag.SelectController = SelectController;
            ViewBag.SelectAction = SelectAction;
            return PartialView("_Buscador");
        }

        [HasPermission("Usuarios_Index")]
        public ActionResult PartialGrid(string IdRol = null, int? TipoAlumno = null, int? IdProvincia = null, int? IdLocalidad = null, string texto = "", string SelectController = "", string SelectAction = "")
        {

            ViewBag.SelectController = SelectController;
            ViewBag.SelectAction = SelectAction;

            List<AspNetUsers> usuarios = null;

            usuarios = consultar(IdRol, TipoAlumno, IdProvincia, IdLocalidad, texto);

            //if (!string.IsNullOrEmpty(SelectController))
            //{
            //    usuarios = new List<AspNetUsers>();
            //}

            if (usuarios.Count == 1 && !string.IsNullOrEmpty(SelectController))
            {
                return RedirectToAction(SelectAction, SelectController, new { id = usuarios.First().Id });
            }

            return PartialView("_Grid", usuarios);
        }

        #region exportar
        public ActionResult Exportar(string IdRol = null, int? TipoAlumno = null, int? IdProvincia = null, int? IdLocalidad = null, string texto = "")
        {
            List<AspNetUsers> usuarios = null;

            usuarios = consultar(IdRol, TipoAlumno, IdProvincia, IdLocalidad, texto);

            ExportSettings.DefaultExportType = ExportType.WYSIWYG;
            return GridViewExtension.ExportToXlsx(CrearGridViewSettingsExportar("Alumnos"), usuarios, true);
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

            settings.Columns.Add(col => { col.FieldName = "Id"; col.Caption = Strings.ColId; col.Visible = false; });
            settings.Columns.Add(col => { col.FieldName = "tipodocumento1.descripcion"; col.Caption = "Tipo Doc"; col.Width = Unit.Pixel(30); });
            settings.Columns.Add(col => { col.FieldName = "NroDocumento"; col.Caption = "N° Doc"; });
            settings.Columns.Add(col => { col.FieldName = "Nombre"; col.Caption = "Nombre"; });
            settings.Columns.Add(col => { col.FieldName = "Apellido"; col.Caption = "Apellido"; });
            settings.Columns.Add(col => { col.FieldName = "Email"; col.Caption = "Email"; });
            settings.Columns.Add(col => { col.FieldName = "Telefono"; col.Caption = "Telefono"; });
            settings.Columns.Add(col => { col.FieldName = "Telefono2"; col.Caption = "Telefono 2"; });
            settings.Columns.Add(col => { col.FieldName = "TipoAlumnoDescripcion"; col.Caption = "Tipo"; });
            settings.Columns.Add(col => { col.FieldName = "localidad1.localidad1"; col.Caption = "Localidad"; });
            settings.Columns.Add(col => { col.FieldName = "localidad1.provincia.provincia1"; col.Caption = "Provincia"; });
            settings.Columns.Add(col => { col.FieldName = "FechaNacimiento"; col.Caption = "Fec Nac"; col.PropertiesEdit.DisplayFormatString = "d"; });


            return settings;
        }
        #endregion
        public List<AspNetUsers> consultar(string IdRol = null, int? TipoAlumno = null, int? IdProvincia = null, int? IdLocalidad = null, string texto = "")
        {
            IQueryable<AspNetUsers> usuarios = db.AspNetUsers;
            bool algunFiltro = false;
            if (!String.IsNullOrEmpty(IdRol) && IdRol != Constantes.ERROR.ToString())
            {
                algunFiltro = true;
                usuarios = usuarios.Where(u => u.AspNetRoles.Any(r => r.Id == IdRol));
            }
            if (TipoAlumno.HasValue && TipoAlumno.Value != Constantes.ERROR)
            {
                algunFiltro = true;
                usuarios = usuarios.Where(u => u.TipoAlumno == TipoAlumno.Value);
            }
            if (IdProvincia.HasValue && IdProvincia.Value != Constantes.ERROR)
            {
                algunFiltro = true;
                usuarios = usuarios.Where(u => u.provincia == IdProvincia.Value);
            }
            if (IdLocalidad.HasValue && IdLocalidad.Value != Constantes.ERROR)
            {
                algunFiltro = true;
                usuarios = usuarios.Where(u => u.localidad == IdLocalidad.Value);
            }
            if (!String.IsNullOrEmpty(texto))
            {
                algunFiltro = true;
                usuarios = usuarios.Where(u => u.NroDocumento.Value.ToString().Contains(texto)
                || u.Apellido.ToString().ToUpper().Contains(texto.ToUpper()) || u.Email.ToUpper().Contains(texto.ToUpper()) || u.Nombre.ToUpper().Contains(texto.ToUpper())
                );
            }

            if (algunFiltro)
                return usuarios.ToList();
            else
                return new List<AspNetUsers>();
        }

        #region abm
        // GET: cursos/Details/5
        [HasPermission("Usuarios_Index")]
        public ActionResult Details(string id)
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

            AspNetUsers usuario = db.AspNetUsers.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            if (usuario.provincia.HasValue)
                PrepararViewBag(usuario.provincia.Value);
            else
                PrepararViewBag();

            return View("Usuario", usuario);

        }

        // GET: cursos/Create
        //public ActionResult Create()
        //{
        //    string callback = Request.Params["callback"];
        //    if (string.IsNullOrEmpty(callback))
        //        callback = "AjaxOk";
        //    //var curso = new curso();
        //    ViewBag.modo = "Create";
        //    ViewBag.Callback = callback;

        //    PrepararViewBag();
        //    return View("Usuario");
        //}

        //[HttpPost]
        //public ActionResult Create(AspNetUsers usuario)
        //{
        //    try
        //    {
        //        ModelState.Remove("Id");
        //        if (ModelState.IsValid)
        //        {
        //            db.curso.Add(curso);
        //            db.SaveChanges();
        //            return Json(new { ok = true });

        //            //           return RedirectToAction("Index");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        while (e.InnerException != null) e = e.InnerException;
        //        ViewBag.ErrorMessage = string.Format(
        //            Strings.ErrorIntentarInsertarRegistro,
        //            e.Message.Replace(Strings.TheStatementHasBeenTerminated, "")
        //        );
        //    }
        //    ViewBag.modo = "Create";
        //    ViewBag.Callback = "AjaxOk";
        //    PrepararViewBag();
        //    return PartialView("Curso", curso);
        //}

        // GET: cursos/Edit/5
        [HasPermission("perfil_index")]
        public ActionResult Edit(string id)
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

            AspNetUsers usuario = db.AspNetUsers.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            if (usuario.provincia.HasValue)
                PrepararViewBag(usuario.provincia.Value);
            else
                PrepararViewBag();

            return View("Usuario", usuario);
        }



        [HttpPost]
        public ActionResult Edit(AspNetUsers usuario)
        {
            try
            {
                if (usuario.provincia.HasValue && usuario.provincia == -1)
                {
                    ModelState.AddModelError("Error", "Debe ingresar la Provincia.");

                }

                if (usuario.localidad.HasValue && usuario.localidad == -1)
                {
                    ModelState.AddModelError("Error", "Debe ingresar la Localidad.");
                }

                ModelState.Remove("AspNetRoles");
                db.Configuration.ValidateOnSaveEnabled = false;
                if (ModelState.IsValid)
                {
                    string id = ApplicationUser.GetId();
                    string seleccion = Request.Params["selected"];
                    var usuarioActual = db.AspNetUsers.Find(usuario.Id);
                    usuarioActual.Apellido = usuario.Apellido;
                    usuarioActual.Calle = usuario.Calle;
                    usuarioActual.dpto = usuario.dpto;
                    usuarioActual.Email = usuario.Email;
                    usuarioActual.FechaNacimiento = usuario.FechaNacimiento;
                    usuarioActual.localidad = usuario.localidad;
                    usuarioActual.Nombre = usuario.Nombre;
                    usuarioActual.NroCalle = usuario.NroCalle;
                    //usuarioActual.NroDocumento = usuario.NroDocumento;
                    usuarioActual.piso = usuario.piso;
                    usuarioActual.provincia = usuario.provincia;
                    usuarioActual.Telefono = usuario.Telefono;
                    usuarioActual.Telefono2 = usuario.Telefono2;
                    //usuarioActual.TipoDocumento = usuario.TipoDocumento;


                    if (id != usuarioActual.Id && !string.IsNullOrEmpty(seleccion))
                    {
                        var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
                        var userManager = new UserManager<ApplicationUser>(store);
                        var result = userManager.RemoveFromRole(usuarioActual.Id, usuarioActual.AspNetRoles.First().Name.ToString());
                        if (result.Succeeded)
                        {
                            userManager.AddToRole(usuarioActual.Id, seleccion);
                        }

                    }


                    if (App.UsuarioTienePermiso("edit_tipo_usuario"))
                    {
                        usuarioActual.TipoAlumno = usuario.TipoAlumno;
                    }

                    db.Entry(usuarioActual).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { ok = true });
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

            if (usuario.provincia.HasValue)
                PrepararViewBag(usuario.provincia.Value);
            else
                PrepararViewBag();


            return View("Usuario", usuario);
        }

        // GET: cursos/Delete/5
        [HasPermission("Admin_Index")]
        public ActionResult Delete(string id)
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

            AspNetUsers usuario = db.AspNetUsers.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            if (usuario.provincia.HasValue)
                PrepararViewBag(usuario.provincia.Value);
            else
                PrepararViewBag();

            return View("Usuario", usuario);
        }

        [HttpPost]
        public ActionResult Delete(AspNetUsers usuario)
        {
            try
            {
                usuario = db.AspNetUsers.Find(usuario.Id);
                db.AspNetUsers.Remove(usuario);
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
            if (usuario.provincia.HasValue)
                PrepararViewBag(usuario.provincia.Value);
            else
                PrepararViewBag();

            return PartialView("Usuario", usuario);
        }


        [AllowAnonymous]
        public JsonResult GetLocalidades(int? IdProvincia = null)
        {
            var localidades = new SelectList(
                            db.localidad.Where(x => x.idProvincia == IdProvincia.Value && x.codigo != 50267).ToList()
                            .ToSelectList(
                                x => x.localidad1,
                                x => x.codigo.ToString(),
                                Strings.SeleccionarLocalidad,
                                Constantes.ERROR.ToString(),
                                Constantes.ERROR.ToString()
                            )
                        , "Value", "Text");

            return Json(localidades, JsonRequestBehavior.AllowGet);



        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region AJAX
        [HttpGet]
        public JsonResult Buscar(string q)
        {
            IQueryable<AspNetUsers> usuarios = db.AspNetUsers;

            var split = q.Split(' ');

            foreach (var termino in split)
            {
                usuarios = usuarios.Where(x => x.Nombre.Contains(termino) || x.Apellido.Contains(termino) || x.NroDocumento.ToString().Contains(termino));
            }
            //  var institutos = InstitutoData.Buscar(q);
            var rta = usuarios.Select(x => new { id = x.Id, text = x.Apellido + ", " + x.Nombre + "(" + x.NroDocumento + ")" }).ToList();


            return Json(rta, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult BuscarPorDNI(string q)
        {
            IQueryable<AspNetUsers> usuarios = db.AspNetUsers;

            var split = q.Split(' ');

            foreach (var termino in split)
            {
                usuarios = usuarios.Where(x => x.Nombre.Contains(termino) || x.Apellido.Contains(termino) || x.NroDocumento.ToString().Contains(termino));
            }
            //  var institutos = InstitutoData.Buscar(q);
            var rta = usuarios.Select(x => new { id = x.NroDocumento, text = x.Apellido + ", " + x.Nombre + "(" + x.NroDocumento + ")" }).ToList();


            return Json(rta, JsonRequestBehavior.AllowGet);
        }

        #endregion


    }
}
