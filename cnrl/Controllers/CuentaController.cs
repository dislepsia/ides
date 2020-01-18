using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using cnrl.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using cnrl.Logica;
using cnrl.Helpers;
using cnrl.ViewModels;
using System.Data.Entity;
using System.Activities.Statements;
using System.Data.Entity.Validation;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;
using System.IO;
using System.Collections.Generic;

namespace cnrl.Controllers
{

    [Authorize]
    public class CuentaController : Controller
    {
        [DataContract]
        class DatoGuarani
        {
            //[DataMember]
            //public string Name { get; set; }

            //[DataMember]
            //public string Description { get; set; }

            [DataMember]
            public string nombre { get; set; }

            [DataMember]
            public string fechaNacimiento { get; set; }

            [DataMember]
            public string regular { get; set; }

            [DataMember]
            public string calidad { get; set; }

            [DataMember]
            public string telefono { get; set; }

            [DataMember]
            public string domicilio { get; set; }

            [DataMember]
            public string carrera { get; set; }

            [DataMember]
            public string descripcionCarrera { get; set; }

            [DataMember]
            public string plan { get; set; }
        }

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public CuentaController()
        {
        }

        public CuentaController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private void PrepararViewBag(int IdProvinciaSeleccionada = 0)
        {
            socioculturalesEntities db = new socioculturalesEntities();
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
                        db.localidad.Where(x => x.idProvincia == IdProvinciaSeleccionada && x.codigo != 50267)
                            .ToSelectList(
                                x => x.localidad1,
                                x => x.codigo.ToString(),
                                Strings.SeleccionarLocalidad,
                                Constantes.ERROR.ToString(),
                                Constantes.ERROR.ToString()
                            )
                        , "Value", "Text");


        }

        //
        // GET: /Cuenta/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            var folder = Server.MapPath("~/Fichero2/");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);

                string filePath = System.IO.Path.Combine(folder, "fichero.txt");

                using (StreamWriter writer = System.IO.File.CreateText(filePath))
                {
                    writer.WriteLine(returnUrl);


                }
            }

            if (returnUrl != null)
            {
                if (returnUrl.Contains("%2Fnuevositio%2F") == true || returnUrl.Contains("/nuevositio/") == true || returnUrl.Contains("nuevositio") == true)
                {
                    returnUrl = "/nuevositio/";
                }
            }


            ViewBag.ReturnUrl = returnUrl;
            socioculturalesEntities db = new socioculturalesEntities();
            ViewBag.TiposDocumento = new SelectList(db.tipodocumento.ToList(), "codigo", "descripcion", "0");
            return View(new LoginViewModel());
        }

        //
        // POST: /Cuenta/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            socioculturalesEntities db = new socioculturalesEntities();
            ViewBag.TiposDocumento = new SelectList(db.tipodocumento, "codigo", "descripcion", "0");

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Credenciales incorrectas");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            int tipoDocumento = model.TipoDocumento;
            long nroDocumento = model.NroDocumento;
            var usuario = db.AspNetUsers.Where(u => u.NroDocumento == nroDocumento && u.TipoDocumento == tipoDocumento);
            SignInStatus result;
            if (usuario.Count() == 1)
                result = await SignInManager.PasswordSignInAsync(usuario.First().UserName, model.Password, model.RememberMe, shouldLockout: false);
            else
                result = await SignInManager.PasswordSignInAsync("123123", model.Password, model.RememberMe, shouldLockout: false);


            switch (result)
            {
                case SignInStatus.Success:
                    //if (String.IsNullOrEmpty(returnUrl))
                    //{
                    var perfil = usuario.First().AspNetRoles.FirstOrDefault();


                    if (perfil != null && perfil.Id == "6")
                    {
                        var folder = Server.MapPath("~/Fichero/");
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);

                            string filePath = System.IO.Path.Combine(folder, "fichero.txt");

                            using (StreamWriter writer = System.IO.File.CreateText(filePath))
                            {
                                writer.WriteLine(returnUrl);


                            }
                        }

                        if (returnUrl == "/nuevositio/" || returnUrl == "%2Fnuevositio%2F")
                        {
                            returnUrl = "/nuevositio/cursa/inscripcion";
                        }
                        else
                        {
                            returnUrl = "/cursa/inscripcion";
                        }

                        //ViewBag.categoria = true;

                        //////////////////////ACTUALIZO TIPO ALUMNO/////////////////////////////

                        var usuarioBase = db.AspNetUsers.Find(usuario.First().Id);

                        guarani.DatosAlumnosBecas s = new guarani.DatosAlumnosBecas();

                        string cadena = null;

                        try
                        {

                            cadena = s.consultaDatosAlumnosG3(usuario.First().NroDocumento.Value.ToString());

                            JavaScriptSerializer js = new JavaScriptSerializer();
                            List<DatoGuarani> cadenaDeserealizada = js.Deserialize<List<DatoGuarani>>(cadena);

                            if (cadenaDeserealizada != null)
                            {
                                string[] resultados = cadena.Split(new Char[] { ';' });
                                if (!cadenaDeserealizada[0].nombre.Equals("*"))
                                {
                                    if (!cadenaDeserealizada[0].nombre.Equals("*"))
                                    {
                                        if (!cadenaDeserealizada[0].calidad.Equals("E"))
                                        {
                                            usuarioBase.TipoAlumno = (byte)TiposAlumno.Alumno;
                                            usuarioBase.Carrera = cadenaDeserealizada[0].descripcionCarrera;
                                        }
                                        else
                                        {
                                            usuarioBase.TipoAlumno = (byte)TiposAlumno.Graduado;
                                            usuarioBase.Carrera = cadenaDeserealizada[0].descripcionCarrera;
                                        }

                                    }
                                    else
                                    {
                                        usuarioBase.TipoAlumno = (byte)TiposAlumno.NoAlumno;
                                    }
                                }
                                else
                                {
                                    usuarioBase.TipoAlumno = (byte)TiposAlumno.NoAlumno;
                                }
                            }
                            else
                            {
                                usuarioBase.TipoAlumno = (byte)TiposAlumno.NoAlumno;
                            }
                        }
                        catch (Exception ex)
                        {
                            usuarioBase.TipoAlumno = (byte)TiposAlumno.NoAlumno;

                        }

                        try
                        {

                            db.Entry(usuarioBase).State = EntityState.Modified;

                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            // UserManager.Delete(user);
                            //throw new Exception("Ha ocurrido un error con la bases de datos, por favor comuniquese con el administrador del sistema.");
                        }
                        ////////////////////////////////////////////////////////////////////////
                    }
                    else
                    {
                        if (perfil != null && (perfil.Id == "5" || perfil.Id == "7"))
                        {
                            if (returnUrl == "/nuevositio/" || returnUrl == "%2Fnuevositio%2F")
                            {
                                returnUrl = "/nuevositio/docente";
                            }
                            else
                            {
                                returnUrl = "/docente";
                            }
                        }
                        else
                        {
                            if (returnUrl == "/nuevositio/" || returnUrl == "%2Fnuevositio%2F")
                            {
                                returnUrl = "/nuevositio/Usuarios";
                            }
                            else
                            {
                                returnUrl = "/Usuarios";
                            }
                        }
                    }


                    //}
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, model.RememberMe });
                default: // SignInStatus.Failure
                    ModelState.AddModelError("", "Credenciales incorrectas");
                    return View(model);
            }
        }

        //
        // GET: /Cuenta/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Cuenta/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                default:
                    // SignInStatus.Failure
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Cuenta/Registro
        [AllowAnonymous]
        public ActionResult Registro()
        {
            PrepararViewBag();
            return View();
        }

        //
        // POST: /Cuenta/Registro
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(ViewModelRegistro model)//RegisterViewModel model)
        {
            if (model.usuario.provincia.HasValue && model.usuario.provincia.Value != -1 && model.usuario.localidad.Value != -1)
                PrepararViewBag(model.usuario.provincia.Value);
            else
            {
                PrepararViewBag();
                ModelState.AddModelError("Error", "Los campos Provincia y Localidad son obligatorios.");
            }

            if (model.usuario.NroDocumento == null)
                ModelState.AddModelError("Error", "El campo DNI es obligatorio.");

            if (model.usuario.piso != null && model.usuario.piso.Length > 4)
                ModelState.AddModelError("Error", "El campo Piso NO puede ser mayor a 4 caracteres.");

            if (model.usuario.dpto != null && model.usuario.dpto.Length > 4)
                ModelState.AddModelError("Error", "El campo Dpto NO puede ser mayor a 4 caracteres.");

            if (ModelState.IsValid)
            {
                //string pass = GenerarAleatorio(15);


                var user = new ApplicationUser
                {
                    UserName = model.usuario.TipoDocumento.ToString() + "_" + model.usuario.NroDocumento,
                    Email = model.usuario.Email,
                    Apellido = model.usuario.Apellido,
                    Calle = model.usuario.Calle,
                    dpto = model.usuario.dpto,
                    FechaNacimiento = model.usuario.FechaNacimiento.Value,
                    localidad = model.usuario.localidad.Value,
                    Nombre = model.usuario.Nombre,
                    NroCalle = model.usuario.NroCalle,
                    NroDocumento = model.usuario.NroDocumento.Value,
                    piso = model.usuario.piso,
                    provincia = model.usuario.provincia.Value,
                    Telefono = model.usuario.Telefono,
                    Telefono2 = model.usuario.Telefono2,
                    TipoDocumento = model.usuario.TipoDocumento.Value
                };
                var result = UserManager.Create(user, model.resetPassword.Password);
                if (result.Succeeded)
                {
                    //Logica.App.enviarMailUsuario(model.Email, "Bienvenido", Strings.emailBienvenida);
                    UserManager.AddToRole(user.Id, "Alumno");

                    socioculturalesEntities db = new socioculturalesEntities();
                    var usuarioBase = db.AspNetUsers.Find(user.Id);

                    guarani.DatosAlumnosBecas s = new guarani.DatosAlumnosBecas();

                    string cadena = null;

                    try
                    {

                        //cadena = s.consultaDatosAlumnos(model.usuario.NroDocumento.Value.ToString());

                        cadena = s.consultaDatosAlumnosG3(model.usuario.NroDocumento.Value.ToString());

                        JavaScriptSerializer js = new JavaScriptSerializer();
                        List<DatoGuarani> cadenaDeserealizada = js.Deserialize<List<DatoGuarani>>(cadena);
                        //string name = blogObject.Name;
                        //string description = blogObject.Description;

                        if (cadenaDeserealizada != null)
                        {
                            string[] resultados = cadena.Split(new Char[] { ';' });
                            if (/*!resultados[0].Split(new Char[] { '#' })[1].Equals("*")*/ !cadenaDeserealizada[0].nombre.Equals("*"))
                            {
                                if (/*!resultados[0].Split(new Char[] { '#' })[1].Equals("*")*/ !cadenaDeserealizada[0].nombre.Equals("*"))
                                {
                                    if (!cadenaDeserealizada[0].calidad.Equals("E"))
                                    {
                                        usuarioBase.TipoAlumno = (byte)TiposAlumno.Alumno;
                                        usuarioBase.Carrera = cadenaDeserealizada[0].descripcionCarrera;
                                    }
                                    else
                                    {
                                        usuarioBase.TipoAlumno = (byte)TiposAlumno.Graduado;
                                        usuarioBase.Carrera = cadenaDeserealizada[0].descripcionCarrera;
                                    }

                                }
                                else
                                {
                                    usuarioBase.TipoAlumno = (byte)TiposAlumno.NoAlumno;
                                }
                            }
                            else
                            {
                                usuarioBase.TipoAlumno = (byte)TiposAlumno.NoAlumno;
                            }
                        }
                        else
                        {
                            usuarioBase.TipoAlumno = (byte)TiposAlumno.NoAlumno;
                        }
                    }
                    catch (Exception ex)
                    {
                        usuarioBase.TipoAlumno = (byte)TiposAlumno.NoAlumno;

                    }

                    try
                    {
                        //usuarioBase.TipoAlumno = model.usuario.TipoAlumno;

                        //---------------------------------// AGREGADO EN USER

                        //usuarioBase.Apellido = model.usuario.Apellido;
                        //usuarioBase.Calle = model.usuario.Calle;
                        //usuarioBase.dpto = model.usuario.dpto;
                        //usuarioBase.Email = model.usuario.Email;
                        //usuarioBase.FechaNacimiento = model.usuario.FechaNacimiento;
                        //if (model.usuario.localidad.HasValue && !model.usuario.localidad.Value.Equals(Constantes.ERROR))
                        //    usuarioBase.localidad = model.usuario.localidad;
                        //usuarioBase.Nombre = model.usuario.Nombre;
                        //usuarioBase.NroCalle = model.usuario.NroCalle;
                        //usuarioBase.NroDocumento = model.usuario.NroDocumento;
                        //usuarioBase.piso = model.usuario.piso;
                        //if (model.usuario.provincia.HasValue && !model.usuario.provincia.Value.Equals(Constantes.ERROR))
                        //    usuarioBase.provincia = model.usuario.provincia;
                        //usuarioBase.Telefono = model.usuario.Telefono;
                        //usuarioBase.Telefono2 = model.usuario.Telefono2;
                        //usuarioBase.TipoDocumento = model.usuario.TipoDocumento;

                        //-----------------------------------//
                        //usuarioBase.TipoAlumno = (byte)TiposAlumno.NoAlumno; //TODO: Levantar este valor desde el web service


                        db.Entry(usuarioBase).State = EntityState.Modified;

                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        UserManager.Delete(user);
                        throw new Exception("Ha ocurrido un error con la bases de datos, por favor comuniquese con el administrador del sistema.");
                    }

                    ViewBag.Mayor60 = false;

                    var fecha = usuarioBase.FechaNacimiento.Value;
                    var today = DateTime.Today;
                    var age = today.Year - fecha.Year;
                    if (fecha > today.AddYears(-age)) age--;

                    if (age >= 60)
                        ViewBag.Mayor60 = true;

                    return View("RegistroConfirmado");

                    //return RedirectToAction("RegistroConfirmado");
                }


                //for (int i = 0; i<result.Errors.Count(); i++)
                //{
                //    if (result.Errors[i].)
                //}
                bool errorEmail = false;
                bool errorUserName = false;
                foreach (var err in result.Errors)
                {
                    if (err.Contains("Name") && err.Contains("taken"))
                    {
                        errorUserName = true;
                        //var error = result.Errors.Where(x => x.Contains("Name") && x.Contains("taken")).First();
                        //error = "El Tipo y Número de Documento que ingresó ya existen.";
                    }
                    if (err.Contains("Email") && err.Contains("taken"))
                    {
                        errorEmail = true;
                        //var error = result.Errors.Where(x => x.Contains("Name") && x.Contains("taken")).First();
                        //error = "El Tipo y Número de Documento que ingresó ya existen.";
                    }
                }

                if (!errorEmail && !errorUserName)
                    AddErrors(result);

                if (errorEmail)
                    ModelState.AddModelError(string.Empty, "El Email ingresado ya está en uso");
                if (errorUserName)
                    ModelState.AddModelError(string.Empty, "El Tipo y Nro de Documento ingresado ya está en uso");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Cuenta/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Cuenta/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            socioculturalesEntities db = new socioculturalesEntities();
            ViewBag.TiposDocumento = new SelectList(db.tipodocumento, "codigo", "descripcion", "0");

            return View(new ForgotPasswordViewModel());
        }

        //
        // POST: /Cuenta/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                socioculturalesEntities db = new socioculturalesEntities();
                ViewBag.TiposDocumento = new SelectList(db.tipodocumento, "codigo", "descripcion", "0");
                var user = db.AspNetUsers.Where(u => u.NroDocumento == model.NroDocumento && u.TipoDocumento == model.TipoDocumento);
                //var user = await UserManager.FindByIdAsync(usuarioBase.First().Id);
                if (user == null || user.Count() == 0)// || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    ViewBag.Mensaje = "El documento ingresado no se encuentra registrado. Por favor regístrese.";
                    return View("ForgotPasswordConfirmation");
                }

                if (!string.IsNullOrEmpty(user.First().Email))
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GeneratePasswordResetTokenAsync(user.First().Id);
                    var callbackUrl = Url.Action("ResetPassword", "Cuenta", new { userId = user.First(), code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    string texto = "Estimado. <BR><BR>Para reiniciar su contraseña haga click <a href=\"" + callbackUrl + "\">aquí</a>";

                    Logica.App.enviarMailUsuario(user.First().Email, "Reinicio de Contraseaña", texto);
                    // AppBuilderSecurityExtensions..

                    char[] mailCodificado;
                    var split = user.First().Email.Split('@');
                    mailCodificado = split.First().ToCharArray();
                    int mitad = mailCodificado.Length / 2;
                    for (int i = mitad; i < mailCodificado.Length; i++)
                    {
                        mailCodificado[i] = '*';
                    }
                    string mailCodificadoFinal = new string(mailCodificado) + "@" + split.Last();

                    ViewBag.email = mailCodificadoFinal;
                    ViewBag.Mensaje = "Por favor revise su casilla de Email para reiniciar su contraseña";
                }
                else
                {
                    ViewBag.email = "-";
                    ViewBag.Mensaje = "Usted no tiene cargada una cuenta de Email. No se puede recuperar su contraseña";

                }
                return View("ForgotPasswordConfirmation");
                //return RedirectToAction("ForgotPasswordConfirmation", "Cuenta");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> ForgotPasswordForUser(string idUsuario)
        {
            if (!string.IsNullOrEmpty(idUsuario))
            {
                socioculturalesEntities db = new socioculturalesEntities();

                var user = db.AspNetUsers.Find(idUsuario);

                if (user == null)// || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    return Json(new { ok = false });
                }

                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Cuenta", new { userId = user, code = code }, protocol: Request.Url.Scheme);
                //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                string texto = "Estimado. <BR><BR>Para reiniciar su contraseña haga click <a href=\"" + callbackUrl + "\">aquí</a>";

                Logica.App.enviarMailUsuario(user.Email, "Reinicio de Contraseaña", texto);
                // AppBuilderSecurityExtensions..

                return Json(new { ok = true });
                //return RedirectToAction("ForgotPasswordConfirmation", "Cuenta");
            }

            // If we got this far, something failed, redisplay form
            return Json(new { ok = false });
        }

        //
        // GET: /Cuenta/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Cuenta/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult RegistroConfirmado()
        {
            return View();
        }
        //
        // GET: /Cuenta/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            socioculturalesEntities db = new socioculturalesEntities();
            ViewBag.TiposDocumento = new SelectList(db.tipodocumento, "codigo", "descripcion", "0");
            return code == null ? View("Error") : View(new ResetPasswordViewModel());
        }

        //
        // POST: /Cuenta/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            socioculturalesEntities db = new socioculturalesEntities();
            ViewBag.TiposDocumento = new SelectList(db.tipodocumento, "codigo", "descripcion", "0");
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = db.AspNetUsers.Where(u => u.NroDocumento == model.NroDocumento && u.TipoDocumento == model.TipoDocumento);
            if (user == null || user.Count() == 0)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Cuenta");
            }
            var result = await UserManager.ResetPasswordAsync(user.First().Id, model.Code, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Cuenta");
            }
            AddErrors(result);
            return View(model);
        }

        //    CambiarPassword",
        //        data: { IdUsuario: IdUsuario, NuevoPassword: nuevoPassword
        //},
        [HttpPost]
        public async Task<JsonResult> CambiarPassword(string IdUsuario, string NuevoPassword)
        {
            if (string.IsNullOrEmpty(IdUsuario) || string.IsNullOrEmpty(NuevoPassword))
            {
                return Json(new { ok = false });
            }

            //return Json(new { ok = true });
            string code = await UserManager.GeneratePasswordResetTokenAsync(IdUsuario);


            var result = await UserManager.ResetPasswordAsync(IdUsuario, code, NuevoPassword);
            if (result.Succeeded)
            {
                return Json(new { ok = true });
                //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                //    if (user != null)
                //    {
                //        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                //    }
                //    return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            return Json(new { ok = false });
            //AddErrors(result);
            //return View(model);
        }

        //
        // GET: /Cuenta/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Cuenta/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Cuenta", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Cuenta/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Cuenta/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, model.ReturnUrl, model.RememberMe });
        }

        //
        // GET: /Cuenta/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                default: // SignInStatus.Failure
                         // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Cuenta/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Cuenta/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Remove("menu");
            return RedirectToAction("index", "Admin");
        }

        //
        // GET: /Cuenta/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XSRF_KEY = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XSRF_KEY] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        private string GenerarAleatorio(int longitudnuevacadena)
        {
            Random obj = new Random();
            const string posibles = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            int longitud = posibles.Length;
            char letra;
            string nuevacadena = "";
            for (int i = 0; i < longitudnuevacadena; i++)
            {
                letra = posibles[obj.Next(longitud)];
                nuevacadena += letra.ToString();
            }
            return nuevacadena;
        }
        #endregion


        //public async System.Threading.Tasks.Task<ActionResult> GenerarUsuariosBatch()
        //{
        //    socioculturalesEntities db = new socioculturalesEntities();
        //    IQueryable<XX_usuarios> usuariosParaGenerar = db.XX_usuarios;
        //    int i = 0;
        //    foreach (var usuarioParaGenerar in usuariosParaGenerar)
        //    {
        //        var usuarioEnBase = db.AspNetUsers.Where(x => x.TipoDocumento == usuarioParaGenerar.tipoDoc && x.NroDocumento.ToString() == usuarioParaGenerar.nroDoc);

        //        if (usuarioEnBase != null && usuarioEnBase.Count() == 0)//creo usuario si no existe
        //        {
        //            var user = new ApplicationUser
        //            {
        //                UserName = usuarioParaGenerar.tipoDoc.ToString() + "_" + usuarioParaGenerar.nroDoc,
        //                Email = (!string.IsNullOrEmpty(usuarioParaGenerar.email)) ? usuarioParaGenerar.email : usuarioParaGenerar.nroDoc + "@a.com"
        //            };
        //            var result = await UserManager.CreateAsync(user);
        //            i++;
        //            if (result.Succeeded)
        //            {
        //                i++;
        //                //Logica.App.enviarMailUsuario(model.Email, "Bienvenido", Strings.emailBienvenida);
        //                UserManager.AddToRole(user.Id, "Alumno");

        //                var usuarioBase = db.AspNetUsers.Find(user.Id);

        //                usuarioBase.Apellido = usuarioParaGenerar.apellido;
        //                //usuarioBase.Calle = usuarioParaGenerar.calle;
        //                //usuarioBase.dpto = usuarioParaGenerar.depto;
        //                usuarioBase.Email = usuarioParaGenerar.email;
        //                //usuarioBase.FechaNacimiento = usuarioParaGenerar.fechaNac;
        //                //usuarioBase.localidad = usuarioParaGenerar.localidad;
        //                usuarioBase.Nombre = usuarioParaGenerar.nombre;
        //                //usuarioBase.NroCalle = usuarioParaGenerar.nroCalle;
        //                usuarioBase.NroDocumento = int.Parse(usuarioParaGenerar.nroDoc);
        //                //usuarioBase.piso = usuarioParaGenerar.piso;
        //                //usuarioBase.provincia = usuarioParaGenerar.provincia;
        //                usuarioBase.Telefono = usuarioParaGenerar.telefono;
        //                //usuarioBase.Telefono2 = usuarioParaGenerar.telefono2;
        //                usuarioBase.TipoDocumento = usuarioParaGenerar.tipoDoc;
        //                usuarioBase.TipoAlumno = (byte)TiposAlumno.NoAlumno; //TODO: Levantar este valor desde el web service
        //                db.Entry(usuarioBase).State = EntityState.Modified;

        //            }

        //        }



        //    }
        //    db.SaveChanges();
        //    return View("Usuario");
        //}
    }
}