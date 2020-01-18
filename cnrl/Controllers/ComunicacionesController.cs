using cnrl.Logica;
using cnrl.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using cnrl.Models;
using cnrl.Helpers;
using cnrl.ViewModels;
using System.Web;
using System.Configuration;
using System.Net;

namespace cnrl.Controllers
{
    [Authorize]
    public class ComunicacionesController : Controller
    {
        private socioculturalesEntities db = new socioculturalesEntities();

        [HasPermission("comunicacion_Index")]
        public ActionResult Index()
        {
            PrepararViewBag();
            ViewBag.modo = "Usuario";
            return View();
        }

        [HasPermission("comunicacion_Index")]
        public ActionResult PartialGrid(int? IdTipoCurso = null, int? IdSede = null, int? IdCurso = null, int? IdPeriodo = null, int? IdOferta = null, string IdUsuario = "", bool? CursosCerrados = null, bool? morosos = null)
        {
            //consultarCuotas(IdUsuario, IdSede, IdCurso, IdOferta, estado, IdPeriodo);
            IQueryable<AspNetUsers> usuarios = db.AspNetUsers;

            if (!string.IsNullOrEmpty(IdUsuario))//!= Constantes.ERROR)
            {
                usuarios = usuarios.Where(x => x.Id == IdUsuario);
            }
            if (IdTipoCurso.HasValue && IdTipoCurso.Value != Constantes.ERROR)
            {
                var tipo = IdTipoCurso.Value;
                usuarios = usuarios.Where(x => x.cursa.Any(c => c.oferta1.curso1.tipoCurso == tipo));
            }

            if (IdSede.HasValue && IdSede.Value != Constantes.ERROR)
            {
                var sede = IdSede.Value;
                usuarios = usuarios.Where(x => x.cursa.Any(c => c.oferta1.sede == sede));
            }

            if (IdCurso.HasValue && IdCurso.Value != Constantes.ERROR)
            {
                var curso = IdCurso.Value;
                usuarios = usuarios.Where(x => x.cursa.Any(c => c.oferta1.curso == curso));
            }

            if (IdOferta.HasValue && IdOferta.Value != Constantes.ERROR)
            {
                var oferta = IdOferta.Value;
                usuarios = usuarios.Where(x => x.cursa.Any(c => c.oferta == oferta));
            }






            string codigosUsuarios = "";
            //            ofertas.Select(x => x.codigo).ToList().ForEach(;
            usuarios.ToList().ForEach(x => codigosUsuarios += "|" + x.NroDocumento.ToString());

            Session.Remove("usuariosParaEnviarEmail");
            Session.Add("usuariosParaEnviarEmail", codigosUsuarios);


            return PartialView("_Grid", usuarios.ToList());
        }

        private void PrepararViewBag(bool filtro = false)
        {

            ViewBag.AsuntoPreFijado = "asunto";//Strings.AsuntoMailFijo + ApplicationUser.ApellidoYNombre();//User.Identity.Name;


            ViewBag.tiposCurso = new SelectList(
                db.tipocurso.ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosTiposCurso : Strings.SeleccionarTipoCurso,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.sedes = new SelectList(
                db.sede.Where(x => x.borradoLogico == false).ToList()
                .ToSelectList(
                    x => x.nombre,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodasSedes : Strings.SeleccionarSede,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.cursos = new SelectList(
                db.curso.ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosLosCursos : Strings.SeleccionarCurso,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.periodosLectivos = new SelectList(
                db.periodolectivo.OrderByDescending(x => x.fechaInscripcionHasta).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosLosPeriodos : Strings.SeleccionarPeriodoLectivo,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.ofertas = new SelectList(
                db.oferta.ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosLasOfertas : Strings.SeleccionarOferta,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");


            ViewBag.plantillas = new SelectList(
                db.plantillaEmail.ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.SeleccionarPlantilla,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");


        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Enviar(MailModel mail, HttpPostedFileBase adjunto1, HttpPostedFileBase adjunto2, HttpPostedFileBase adjunto3, HttpPostedFileBase adjunto4)
        {
            int Ok = 0;
            int Error = 0;
            if (adjunto1 != null)
            {
                //mail.ImageMimeType = adjunto.ContentType;
                mail.adjunto = new byte[adjunto1.ContentLength];
                adjunto1.InputStream.Read(mail.adjunto, 0, adjunto1.ContentLength);
            }

            List<MailModel> mails = new List<MailModel>();
            if (!string.IsNullOrEmpty(mail.modo) && mail.modo == "Oferta")
            {
                //var ofertas = (List<oferta>)Session["ofertasParaEnviarEmail"];
                var codigos = mail.codigos != null ? mail.codigos.Split('|') : null;


                if (codigos != null && codigos.Count() > 0)
                {
                    oferta ofertaAdministrador = null;

                    foreach (var cod in codigos)
                    {
                        int codigo = 0;
                        if (int.TryParse(cod, out codigo))
                        {
                            var oferta = db.oferta.Find(codigo);
                            ofertaAdministrador = oferta;
                            foreach (var cur in oferta.cursa)
                            {
                                var email = new MailModel();

                                email.asunto = reemplazarVariablesOferta(reemplazarVariablesUsuario(mail.asunto, cur.AspNetUsers), oferta);
                                email.editor = reemplazarVariablesOferta(reemplazarVariablesUsuario(mail.editor, cur.AspNetUsers), oferta);
                                email.destinatarios = cur.AspNetUsers.Email;

                                if (!mails.Where(x => x.destinatarios == email.destinatarios).Any()
                                    || (mails.Where(x => x.destinatarios == email.destinatarios).Any()
                                    && (!mails.Where(x => x.asunto == email.asunto).Any() || !mails.Where(x => x.editor == email.editor).Any()))) //evita enviar el mismo mail a una persona 2 veces (si es que no se usaron variables)
                                    mails.Add(email);
                            }
                        }
                    }

                    if (ofertaAdministrador != null)
                    {
                        var userAdministrador = db.AspNetUsers.Find(ApplicationUser.GetId());
                        var emailAdministrador = new MailModel();
                        emailAdministrador.asunto = reemplazarVariablesOferta(reemplazarVariablesUsuario(mail.asunto, userAdministrador), ofertaAdministrador);
                        emailAdministrador.editor = reemplazarVariablesOferta(reemplazarVariablesUsuario(mail.editor, userAdministrador), ofertaAdministrador);
                        emailAdministrador.destinatarios = userAdministrador.Email;
                    }
                }

            }
            else if (!string.IsNullOrEmpty(mail.modo) && mail.modo == "Deuda")
            {

                //var ofertas = (List<oferta>)Session["ofertasParaEnviarEmail"];
                //var codigos = mail.codigos.Split('|');
                var cuotas = CuotaData.consultarCuotas(mail.codigos);

                //asdas
                foreach (var cuota in cuotas)
                {
                    var oferta = db.oferta.Find(cuota.NroComision);
                    var usuario = db.AspNetUsers.Where(x => x.NroDocumento == cuota.Dni).First();

                    if (oferta != null && usuario != null)
                    {
                        var email = new MailModel();

                        email.asunto = reemplazarVariablesCuota(reemplazarVariablesOferta(reemplazarVariablesUsuario(mail.asunto, usuario), oferta), cuota);
                        email.editor = reemplazarVariablesCuota(reemplazarVariablesOferta(reemplazarVariablesUsuario(mail.editor, usuario), oferta), cuota);
                        email.destinatarios = usuario.Email;

                        if (!mails.Where(x => x.destinatarios == email.destinatarios).Any()
                            || (mails.Where(x => x.destinatarios == email.destinatarios).Any()
                            && (!mails.Where(x => x.asunto == email.asunto).Any() || !mails.Where(x => x.editor == email.editor).Any()))) //evita enviar el mismo mail a una persona 2 veces (si es que no se usaron variables)
                            mails.Add(email);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(mail.modo) && mail.modo == "Usuario")
            {

                var nrosDoc = Session["usuariosParaEnviarEmail"].ToString();
                var codigos = nrosDoc.Split('|');



                foreach (var cod in codigos)
                {
                    int codigo = 0;
                    if (int.TryParse(cod, out codigo))
                    {
                        var usuario = db.AspNetUsers.Where(x => x.NroDocumento == codigo).First();


                        var email = new MailModel();

                        email.asunto = reemplazarVariablesUsuario(mail.asunto, usuario);
                        email.editor = reemplazarVariablesUsuario(mail.editor, usuario);
                        email.destinatarios = usuario.Email;

                        if (!mails.Where(x => x.destinatarios == email.destinatarios).Any()
                            || (mails.Where(x => x.destinatarios == email.destinatarios).Any()
                            && (!mails.Where(x => x.asunto == email.asunto).Any() || !mails.Where(x => x.editor == email.editor).Any()))) //evita enviar el mismo mail a una persona 2 veces (si es que no se usaron variables)
                            mails.Add(email);

                    }
                }



                foreach (var email in mails)
                {

                    string resultado = cnrl.Logica.App.EnviarMailComunicacion(
                        email.destinatarios,
                        email.asunto,
                        email.editor,
                        false,
                        User.Identity.Name,
                        adjunto1,
                        adjunto2,
                        adjunto3,
                        adjunto4);

                    if (resultado == "Ok")
                    {
                        Ok++;
                    }
                    else
                    {
                        Error++;
                    }
                }

                ViewBag.Enviados = Ok;
                ViewBag.Errores = Error;
                ViewBag.Error = (Error > 0) ? true : false;
                ViewBag.Mensaje = "";

                return View("Enviados");
            }
            else
            {
                ViewBag.Enviados = 0;
                ViewBag.Errores = 0;
                ViewBag.Error = true;
                ViewBag.Mensaje = "No habia datos en sesión";
                return View("Enviados");
            }


            foreach (var email in mails)
            {

                string resultado = cnrl.Logica.App.EnviarMailComunicacion(
                    email.destinatarios,
                    email.asunto,
                    email.editor,
                    false,
                    User.Identity.Name,
                    adjunto1,
                    adjunto2,
                    adjunto3,
                    adjunto4);

                if (resultado == "Ok")
                {
                    Ok++;
                }
                else
                {
                    Error++;
                }
            }

            ViewBag.Enviados = Ok;
            ViewBag.Errores = Error;
            ViewBag.Error = (Error > 0) ? true : false;
            ViewBag.Mensaje = "";

            return Json(new { ok = (Ok > 0) ? true : false });

            //            return View("Enviados");
            //return GridViewExtension.ExportToXls(CrearGridViewSettingsExportar(letra), personas, true);
        }


        [HasPermission("comunicacion_Index")]
        private string reemplazarVariablesUsuario(string texto, AspNetUsers usuario)
        {
            //%NOMBRE%
            //%APELLIDO%
            //%DOCUMENTO%
            //%SEDE%
            //%TIPO_CURSO%
            //%CURSO%
            //%DIA_HORARIO%
            //%AULA%
            //%FECHA_INICIO%
            //%DETALLE_CUOTA_ADEUDADA%
            if (!string.IsNullOrEmpty(texto))
            {
                if (texto.Contains("%NOMBRE%"))
                {
                    texto = texto.Replace("%NOMBRE%", usuario.Nombre);
                }
                if (texto.Contains("%APELLIDO%"))
                {
                    texto = texto.Replace("%APELLIDO%", usuario.Apellido);
                }
                if (texto.Contains("%DOCUMENTO%"))
                {
                    texto = texto.Replace("%DOCUMENTO%", usuario.tipodocumento1.descripcion + " " + usuario.NroDocumento.ToString());
                }
            }
            return texto;
        }

        [HasPermission("comunicacion_Index")]
        private string reemplazarVariablesOferta(string texto, oferta oferta)
        {
            if (!string.IsNullOrEmpty(texto))
            {
                if (texto.Contains("%SEDE%"))
                {
                    texto = texto.Replace("%SEDE%", oferta.sede1.nombre);
                }
                if (texto.Contains("%TIPO_CURSO%"))
                {
                    texto = texto.Replace("%TIPO_CURSO%", oferta.curso1.tipocurso1.descripcion);
                }
                if (texto.Contains("%CURSO%"))
                {
                    texto = texto.Replace("%CURSO%", oferta.curso1.descripcion);
                }
                if (texto.Contains("%DIA_HORARIO%"))
                {
                    texto = texto.Replace("%DIA_HORARIO%", oferta.horario);
                }
                if (texto.Contains("%AULA%"))
                {
                    texto = texto.Replace("%AULA%", oferta.cantidadCuotas.ToString());
                }
                if (texto.Contains("%FECHA_INICIO%"))
                {
                    texto = texto.Replace("%FECHA_INICIO%", oferta.fechaDesde.ToString());
                }
            }

            return texto;
        }

        [HasPermission("comunicacion_Index")]
        private string reemplazarVariablesCuota(string texto, Cuota cuota)
        {
            if (!string.IsNullOrEmpty(texto))
            {
                if (texto.Contains("%NRO_CUOTA%"))
                {
                    texto = texto.Replace("%NRO_CUOTA%", cuota.NroCuota.ToString());
                }
                if (texto.Contains("%TOTAL_CUOTAS%"))
                {
                    texto = texto.Replace("%TOTAL_CUOTAS%", cuota.TotalCuota.ToString());
                }
                if (texto.Contains("%IMPORTE%"))
                {
                    texto = texto.Replace("%IMPORTE%", cuota.Importe.ToString());
                }
                if (texto.Contains("%IMPORTE2%"))
                {
                    texto = texto.Replace("%IMPORTE2%", cuota.Importe2.ToString());
                }
                if (texto.Contains("%FECHA_VENCIMIENTO%") && cuota.fechavto.HasValue)
                {
                    texto = texto.Replace("%FECHA_VENCIMIENTO%", cuota.fechavto.Value.ToString("dd/MM/yyyy"));
                }
                if (texto.Contains("%FECHA_VENCIMIENTO2%") && cuota.fechavto2.HasValue)
                {
                    texto = texto.Replace("%FECHA_VENCIMIENTO2%", cuota.fechavto2.Value.ToString("dd/MM/yyyy"));
                }

            }

            return texto;
        }

        #region popup
        [HasPermission("comunicacion_Index")]
        public ActionResult Oferta()
        {
            if (Session["ofertasParaEnviarEmail"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string callback = Request.Params["callback"];
            if (string.IsNullOrEmpty(callback))
                callback = "AjaxOk";

            ViewBag.Callback = callback;
            var codigosOferta = Session["ofertasParaEnviarEmail"].ToString().Split('|');
            int cantidad = 0;
            foreach (var cod in codigosOferta)
            {
                int codigo = 0;
                if (int.TryParse(cod, out codigo))
                    cantidad += db.oferta.Find(codigo).cursa.Count();
            }

            //            var ofertas = (List<oferta>)Session["ofertasParaEnviarEmail"];

            ViewBag.CantidadMailsParaEnviar = cantidad;
            //Session.Remove("ofertasParaEnviarEmail");
            PrepararViewBagMailer();
            var mail = new ViewModels.MailModel();
            mail.codigos = Session["ofertasParaEnviarEmail"].ToString();
            mail.modo = "Oferta";
            return View("mailer", mail);
        }

        [HasPermission("comunicacion_Index")]
        public ActionResult Deuda()
        {
            if (Session["cuotasParaEnviarEmail"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string callback = Request.Params["callback"];
            if (string.IsNullOrEmpty(callback))
                callback = "AjaxOk";

            ViewBag.Callback = callback;
            var codigosCuotas = Session["cuotasParaEnviarEmail"].ToString().Split('|');

            //int cantidad = 0;
            //foreach (var cod in codigosCuotas)
            //{
            //    int codigo = 0;
            //    if (int.TryParse(cod, out codigo))
            //        cantidad += db.oferta.Find(codigo).cursa.Count();
            //}

            //            var ofertas = (List<oferta>)Session["ofertasParaEnviarEmail"];

            ViewBag.CantidadMailsParaEnviar = codigosCuotas.Count() - 1;
            //Session.Remove("ofertasParaEnviarEmail");
            PrepararViewBagMailer();
            var mail = new ViewModels.MailModel();
            mail.codigos = Session["cuotasParaEnviarEmail"].ToString();
            mail.modo = "Deuda";
            return View("mailer", mail);
        }


        private void PrepararViewBagMailer()
        {

            ViewBag.AsuntoPreFijado = "asunto";//Strings.AsuntoMailFijo + ApplicationUser.ApellidoYNombre();//User.Identity.Name;


            ViewBag.plantillas = new SelectList(
                db.plantillaEmail.ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.SeleccionarPlantilla,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");






        }


        #endregion
    }
}