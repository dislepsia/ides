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
using System.Web.Mvc.Html;
using System.Configuration;
using System.Net.Http;
using DevExpress.Export;
using DevExpress.Web.Mvc;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Headers;
using DevExpress.XtraPrinting.Localization;

namespace cnrl.Controllers
{
    public class ofertaController : Controller
    {
        private socioculturalesEntities db = new socioculturalesEntities();

        [HasPermission("Oferta_Index")]
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

            return View();
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

            ViewBag.Horario = new SelectList(
               db.oferta.Where(x => x.periodolectivo1.tipocurso1.sector == (int)Sectores.Idiomas).ToList()
               .ToSelectList(
                   x => x.horaDesde + " a " + x.horaHasta,
                   x => x.codigo.ToString(),
                   (filtro) ? "Seleccionar Todos los Horarios" : "Seleccione un Horario",
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
                db.curso.Where(x => x.tipocurso1.sector == (int)Sectores.Idiomas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosLosCursos : Strings.SeleccionarCurso,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.periodosLectivos = new SelectList(
                db.periodolectivo.OrderByDescending(x => x.fechaInscripcionHasta).Where(x => x.tipocurso1.sector == (int)Sectores.Idiomas).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro) ? Strings.MsgTodosLosPeriodos : Strings.SeleccionarPeriodoLectivo,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");


            ViewBag.docentes = new SelectList(
            db.AspNetRoles.Find(((int)Roles.Docente).ToString()).AspNetUsers.ToList()
            .ToSelectList(
                x => x.descripcion,
                x => x.Id.ToString(),
                (filtro) ? Strings.MsgTodosDocentes : Strings.SeleccionarDocente,
                    "",//Constantes.ERROR.ToString(),
                    ""//Constantes.ERROR.ToString()
            )
            , "Value", "Text");

            ViewBag.coordinadorDocentes = new SelectList(
           db.AspNetRoles.Find(((int)Roles.coordinadorDocente).ToString()).AspNetUsers.ToList()
           .ToSelectList(
               x => x.descripcion,
               x => x.Id.ToString(),
               (filtro) ? Strings.MsgTodosDocentes : Strings.SeleccionarDocente,
                   "",//Constantes.ERROR.ToString(),
                   ""//Constantes.ERROR.ToString()
           )
           , "Value", "Text");

            ViewBag.frecuenciasCuota = new SelectList(
                db.frecuenciaCuota.Where(x => x.codigo != 4).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    "(Seleccione la frecuencia de las cuotas)",
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


            ViewBag.EstadosOferta = new SelectList(listEstadosOferta, "Value", "Text");

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

            ViewBag.Horario = new SelectList(
               db.oferta.Where(x => x.periodolectivo1.tipocurso1.sector == (int)Sectores.Graduados).ToList()
               .ToSelectList(
                   x => x.horaDesde + " a " + x.horaHasta,
                   x => x.codigo.ToString(),
                   (filtro) ? "Seleccionar Todos los Horarios" : "Seleccione un Horario",
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


            ViewBag.docentes = new SelectList(
            db.AspNetRoles.Find(((int)Roles.Docente).ToString()).AspNetUsers.ToList()
            .ToSelectList(
                x => x.descripcion,
                x => x.Id.ToString(),
                (filtro) ? Strings.MsgTodosDocentes : Strings.SeleccionarDocente,
                    "",//Constantes.ERROR.ToString(),
                    ""//Constantes.ERROR.ToString()
            )
            , "Value", "Text");

            ViewBag.coordinadorDocentes = new SelectList(
           db.AspNetRoles.Find(((int)Roles.coordinadorDocente).ToString()).AspNetUsers.ToList()
           .ToSelectList(
               x => x.descripcion,
               x => x.Id.ToString(),
               (filtro) ? Strings.MsgTodosDocentes : Strings.SeleccionarDocente,
                   "",//Constantes.ERROR.ToString(),
                   ""//Constantes.ERROR.ToString()
           )
           , "Value", "Text");

            ViewBag.frecuenciasCuota = new SelectList(
                db.frecuenciaCuota.Where(x => x.codigo != 4).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    "(Seleccione la frecuencia de las cuotas)",
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


            ViewBag.EstadosOferta = new SelectList(listEstadosOferta, "Value", "Text");

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

            ViewBag.Horario = new SelectList(
               db.oferta.Where(x => x.periodolectivo1.tipocurso1.sector == (int)Sectores.Empresas).ToList()
               .ToSelectList(
                   x => x.horaDesde + " a " + x.horaHasta,
                   x => x.codigo.ToString(),
                   (filtro) ? "Seleccionar Todos los Horarios" : "Seleccione un Horario",
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


            ViewBag.docentes = new SelectList(
            db.AspNetRoles.Find(((int)Roles.Docente).ToString()).AspNetUsers.ToList()
            .ToSelectList(
                x => x.descripcion,
                x => x.Id.ToString(),
                (filtro) ? Strings.MsgTodosDocentes : Strings.SeleccionarDocente,
                    "",//Constantes.ERROR.ToString(),
                    ""//Constantes.ERROR.ToString()
            )
            , "Value", "Text");

            ViewBag.coordinadorDocentes = new SelectList(
           db.AspNetRoles.Find(((int)Roles.coordinadorDocente).ToString()).AspNetUsers.ToList()
           .ToSelectList(
               x => x.descripcion,
               x => x.Id.ToString(),
               (filtro) ? Strings.MsgTodosDocentes : Strings.SeleccionarDocente,
                   "",//Constantes.ERROR.ToString(),
                   ""//Constantes.ERROR.ToString()
           )
           , "Value", "Text");

            ViewBag.frecuenciasCuota = new SelectList(
                db.frecuenciaCuota.Where(x => x.codigo != 4).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    "(Seleccione la frecuencia de las cuotas)",
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


            ViewBag.EstadosOferta = new SelectList(listEstadosOferta, "Value", "Text");

        }

        [HasPermission("Oferta_Index")]
        public ActionResult PartialGrid(int? IdTipoCurso = null, int? IdSede = null, int? IdCurso = null, int? IdPeriodo = null, int? IdDia = null, string IdDocente = "", bool? VistaCompleta = null, decimal? montoDesde = null, decimal? montoHasta = null, int? inscriptosDesde = null, int? inscriptosHasta = null, bool? inscriptosMayorMinimo = null, bool? montoMayorMinimo = null, int? EstadoOferta = null, int? IdHorario = null)
        {
            List<oferta> ofertas = null;

            ofertas = consultarOfertas(IdTipoCurso, IdSede, IdCurso, IdPeriodo, IdDia, IdDocente, VistaCompleta, montoDesde, montoHasta, inscriptosDesde, inscriptosHasta, inscriptosMayorMinimo, montoMayorMinimo, EstadoOferta, IdHorario);
            ViewBag.VistaCompleta = VistaCompleta;
            ViewBag.CantidadOfertas = ofertas.Count;
            return PartialView("_Grid", ofertas);
        }

        public List<oferta> consultarOfertas(int? IdTipoCurso = null, int? IdSede = null, int? IdCurso = null, int? IdPeriodo = null, int? IdDia = null, string IdDocente = "", bool? VistaCompleta = null, decimal? montoDesde = null, decimal? montoHasta = null, int? inscriptosDesde = null, int? inscriptosHasta = null, bool? inscriptosMayorMinimo = null, bool? montoMayorMinimo = null, int? EstadoOferta = null, int? IdHorario = null)
        {
            IQueryable<oferta> ofertas = db.oferta;
            if (User.IsInRole("AdministradorEmpresas"))
            {
                ofertas = ofertas.Where(x => x.periodolectivo1.tipocurso1.sector == (int)Sectores.Empresas);
            }
            else
            {
                if (User.IsInRole("AdministradorGraduado"))
                {
                    ofertas = ofertas.Where(x => x.periodolectivo1.tipocurso1.sector == (int)Sectores.Graduados);
                }
                else
                {
                    ofertas = ofertas.Where(x => x.periodolectivo1.tipocurso1.sector == (int)Sectores.Idiomas);
                }
            }

            bool algunFiltro = false;
            if (User.IsInRole("Ventanilla"))
            {
                IdSede = ApplicationUser.GetSede();
            }

            if (EstadoOferta.HasValue && EstadoOferta.Value != Constantes.ERROR)
            {
                bool habilitada = (EstadoOferta.Value == 1) ? true : false;
                ofertas = ofertas.Where(x => x.habilitada == habilitada);
            }

            if (IdTipoCurso.HasValue && IdTipoCurso.Value != Constantes.ERROR)
            {
                var tipo = IdTipoCurso.Value;
                ofertas = ofertas.Where(x => x.curso1.tipoCurso == (tipo));
            }

            if (IdSede.HasValue && IdSede.Value != Constantes.ERROR)
            {
                var sede = IdSede.Value;
                ofertas = ofertas.Where(x => x.sede == (sede));
            }

            if (IdCurso.HasValue && IdCurso.Value != Constantes.ERROR)
            {
                var curso = IdCurso.Value;
                ofertas = ofertas.Where(x => x.curso == (curso));
                algunFiltro = true;
            }

            if (IdPeriodo.HasValue && IdPeriodo.Value != Constantes.ERROR)
            {
                var periodo = IdPeriodo.Value;
                ofertas = ofertas.Where(x => x.periodoLectivo == (periodo));
                algunFiltro = true;
            }

            if (!String.IsNullOrEmpty(IdDocente) && IdDocente != "-1")
            {
                ofertas = ofertas.Where(x => x.docente == IdDocente);
                algunFiltro = true;
            }

            if (IdHorario.HasValue && IdHorario.Value != Constantes.ERROR)
            {
                var horario = IdHorario.Value;
                ofertas = ofertas.Where(x => x.codigo == (horario));
                algunFiltro = true;
            }

            if (IdDia.HasValue && IdDia.Value != Constantes.ERROR)
            {
                var dia = IdDia.Value;
                switch (dia)
                {
                    case (int)Dias.Lunes:
                        ofertas = ofertas.Where(x => x.lunes == (true));
                        break;
                    case (int)Dias.Martes:
                        ofertas = ofertas.Where(x => x.martes == (true));
                        break;
                    case (int)Dias.Miercoles:
                        ofertas = ofertas.Where(x => x.miercoles == (true));
                        break;
                    case (int)Dias.Jueves:
                        ofertas = ofertas.Where(x => x.jueves == (true));
                        break;
                    case (int)Dias.Viernes:
                        ofertas = ofertas.Where(x => x.viernes == (true));
                        break;
                    case (int)Dias.Sabado:
                        ofertas = ofertas.Where(x => x.sabado == (true));
                        break;
                }
            }

            if (!algunFiltro)
            {
                if (User.IsInRole("AdministradorEmpresas"))
                {
                    ofertas = db.oferta.Where(x => x.sede == -1 && /*x.periodolectivo1.tipocurso1.codigo.Equals(23)*/x.periodolectivo1.tipocurso1.sector == (int)Sectores.Empresas);
                }
                else
                {
                    if (User.IsInRole("AdministradorGraduado"))
                    {
                        ofertas = db.oferta.Where(x => x.sede == -1 && x.periodolectivo1.tipocurso1.sector == (int)Sectores.Graduados);
                    }
                    else
                    {
                        ofertas = db.oferta.Where(x => x.sede == -1 && x.periodolectivo1.tipocurso1.sector == (int)Sectores.Idiomas);
                    }

                }

            }

            if (VistaCompleta.HasValue && VistaCompleta.Value //&&
                                                              //(montoDesde.HasValue || montoHasta.HasValue
                                                              //|| (montoMayorMinimo.HasValue && montoMayorMinimo.Value) || (inscriptosMayorMinimo.HasValue && inscriptosMayorMinimo.Value)
                                                              //|| inscriptosDesde.HasValue || inscriptosHasta.HasValue)
                )

            {
                var ofertasFiltradas = new List<oferta>();
                foreach (var ofer in ofertas)
                {
                    var recaudacion = recaudacionPorComision(ofer.codigo);
                    ofer.importeInscritos = recaudacion;
                    bool cumple = true;

                    if (montoDesde.HasValue)
                    {
                        if (ofer.importeInscritos < montoDesde.Value)
                            cumple = false;
                    }
                    if (montoHasta.HasValue)
                    {
                        if (ofer.importeInscritos > montoHasta.Value)
                            cumple = false;
                    }
                    if (montoMayorMinimo.HasValue && montoMayorMinimo.Value)
                    {
                        if ((double)ofer.importeInscritos < ofer.recaudacionMinima)
                            cumple = false;
                    }

                    if (inscriptosDesde.HasValue)
                    {
                        if (ofer.cantidadInscriptos < inscriptosDesde.Value)
                            cumple = false;
                    }
                    if (inscriptosHasta.HasValue)
                    {
                        if (ofer.cantidadInscriptos > inscriptosHasta.Value)
                            cumple = false;
                    }

                    if (inscriptosMayorMinimo.HasValue && inscriptosMayorMinimo.Value)
                    {
                        if (ofer.cantidadInscriptos < ofer.cupoMinimo)
                            cumple = false;
                    }
                    //if (ofer.cupo <= ofer.cantidadInscriptos)
                    //    cumple = false;

                    if (cumple)
                    {
                        ofertasFiltradas.Add(ofer);
                    }
                }
                ofertas = ofertasFiltradas.AsQueryable();
            }
            //Session.Remove("ofertasParaEnviarEmail");
            //Session.Add("ofertasParaEnviarEmail", ofertas.ToList());

            string codigosOferta = "";
            //            ofertas.Select(x => x.codigo).ToList().ForEach(;
            if (ofertas != null && ofertas.Count() > 0)
                ofertas.ToList().ForEach(x => codigosOferta += "|" + x.codigo.ToString());

            Session.Remove("ofertasParaEnviarEmail");
            Session.Add("ofertasParaEnviarEmail", codigosOferta);

            return ofertas.ToList();
        }

        #region ABM
        // GET: cursos/Details/5
        [HasPermission("Oferta_Index")]
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

            oferta oferta = db.oferta.Find(id);

            if (oferta == null)
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


            if (oferta.descuentoBandaNegativaNoAlumno.HasValue)
                oferta.descuentoBandaNegativaNoAlumno.Value.ToString();
            else
                oferta.descuentoBandaNegativaNoAlumno = 0;

            return View("Oferta", oferta);

        }

        public JsonResult GetHorarios(int? IdCurso = null, int? IdSede = null, int? IdPeriodo = null)
        {
            var curso = db.curso.Find(IdCurso.Value);

            var periodos = new SelectList(
                            db.oferta.Where(x => x.curso == IdCurso && x.habilitada == true && x.sede == IdSede && x.periodoLectivo == IdPeriodo).ToList()
                            .ToSelectList(
                                x => x.horaDesde + " a " + x.horaHasta,
                                x => x.codigo.ToString(),
                                "Seleccione un Horario",
                                Constantes.ERROR.ToString(),
                                Constantes.ERROR.ToString()
                            )
                        , "Value", "Text");

            return Json(periodos, JsonRequestBehavior.AllowGet);
        }

        // GET: cursos/Create
        [HasPermission("Oferta_ABM")]
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


            var oferta = new oferta();
            oferta.horaInscripcionDesde = oferta.horaDesde = new TimeSpan(10, 00, 00);
            oferta.horaInscripcionHasta = oferta.horaHasta = new TimeSpan(22, 00, 00);
            oferta.habilitada = true;
            oferta.cupo = int.Parse(ConfigurationManager.AppSettings["CupoDefault"]);
            oferta.esBandaNegativa = false;


            return View("Oferta", oferta);
        }

        [HttpPost]
        public ActionResult Create(oferta oferta)
        {
            try
            {
                ModelState.Remove("codigo");
                if (ModelState.IsValid)
                {
                    if (oferta.gratuito == true)
                        oferta.frecuenciaCuota = 4;

                    if (oferta.RequierePreInscripcion == false)
                    {
                        db.oferta.Add(oferta);
                        db.SaveChanges();
                        return Json(new { ok = true });
                    }
                    else
                    {
                        if (oferta.fechaDocumentacion.ToString() == "" || oferta.documentacion == null)
                        {
                            ModelState.AddModelError(string.Empty, "El campo Fecha Documentación y/o Documentación no deben quedar vacío.");
                        }
                        else
                        {
                            db.oferta.Add(oferta);
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
            return PartialView("Oferta", oferta);
        }

        // GET: cursos/Edit/5
        [HasPermission("Oferta_ABM")]
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

            oferta oferta = db.oferta.Find(id);

            if (oferta == null)
            {
                return HttpNotFound();
            }

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

            if (oferta.descuentoBandaNegativaNoAlumno.HasValue)
                oferta.descuentoBandaNegativaNoAlumno.Value.ToString();
            else
                oferta.descuentoBandaNegativaNoAlumno = 0;

            return View("Oferta", oferta);
        }

        [HttpPost]
        public ActionResult Edit(oferta oferta)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (oferta.gratuito == true)
                        oferta.frecuenciaCuota = 4;

                    if (oferta.RequierePreInscripcion == false)
                    {
                        db.Entry(oferta).State = EntityState.Modified;
                        db.SaveChanges();
                        return Json(new { ok = true });
                    }
                    else
                    {
                        if (oferta.fechaDocumentacion.ToString() == "" || oferta.documentacion == null)
                        {
                            ModelState.AddModelError(string.Empty, "El campo Fecha Documentación y/o Documentación no deben quedar vacío.");
                        }
                        else
                        {
                            db.Entry(oferta).State = EntityState.Modified;
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
            return View("Oferta", oferta);
        }

        // GET: cursos/Delete/5
        [HasPermission("Oferta_ABM")]
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

            oferta oferta = db.oferta.Find(id);
            if (oferta == null)
            {
                return HttpNotFound();
            }
            PrepararViewBag();

            return View("Oferta", oferta);
        }

        [HttpPost]
        public ActionResult Delete(oferta oferta)
        {
            try
            {
                oferta = db.oferta.Find(oferta.codigo);
                db.oferta.Remove(oferta);
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
            return PartialView("Oferta", oferta);
        }

        [HasPermission("Oferta_ABM")]
        [HttpPost]
        public ActionResult Habilitar(int IdOferta)
        {
            try
            {
                var oferta = db.oferta.Find(IdOferta);
                oferta.habilitada = !oferta.habilitada;
                if (oferta.descuentoBandaNegativaNoAlumno == null)
                {
                    oferta.descuentoBandaNegativaNoAlumno = oferta.descuentoBandaNegativa;
                }
                db.Entry(oferta).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { ok = true });
            }
            catch (Exception ex)
            {

                return Json(new { ok = false });
            }


        }

        [HttpPost]
        public ActionResult consultarCantidadOfertas(int? IdTipoCurso = null, int? IdSede = null, int? IdCurso = null, int? IdPeriodo = null, int? IdDia = null, string IdDocente = "", bool? VistaCompleta = null, decimal? montoDesde = null, decimal? montoHasta = null, int? inscriptosDesde = null, int? inscriptosHasta = null, bool? inscriptosMayorMinimo = null, bool? montoMayorMinimo = null, int? EstadoOferta = null)
        {
            IQueryable<oferta> ofertas = db.oferta;

            if (EstadoOferta.HasValue && EstadoOferta.Value != Constantes.ERROR)
            {
                bool habilitada = (EstadoOferta.Value == 1) ? true : false;
                ofertas = ofertas.Where(x => x.habilitada == habilitada);
            }

            if (IdTipoCurso.HasValue && IdTipoCurso.Value != Constantes.ERROR)
            {
                var tipo = IdTipoCurso.Value;
                ofertas = ofertas.Where(x => x.curso1.tipoCurso == (tipo));
            }

            if (IdSede.HasValue && IdSede.Value != Constantes.ERROR)
            {
                var sede = IdSede.Value;
                ofertas = ofertas.Where(x => x.sede == (sede));
            }

            if (IdCurso.HasValue && IdCurso.Value != Constantes.ERROR)
            {
                var curso = IdCurso.Value;
                ofertas = ofertas.Where(x => x.curso == (curso));
            }

            if (IdPeriodo.HasValue && IdPeriodo.Value != Constantes.ERROR)
            {
                var periodo = IdPeriodo.Value;
                ofertas = ofertas.Where(x => x.periodoLectivo == (periodo));
            }

            if (!String.IsNullOrEmpty(IdDocente) && IdDocente != "-1")
            {
                ofertas = ofertas.Where(x => x.docente == IdDocente);
            }

            if (IdDia.HasValue && IdDia.Value != Constantes.ERROR)
            {
                var dia = IdDia.Value;
                switch (dia)
                {
                    case (int)Dias.Lunes:
                        ofertas = ofertas.Where(x => x.lunes == (true));
                        break;
                    case (int)Dias.Martes:
                        ofertas = ofertas.Where(x => x.martes == (true));
                        break;
                    case (int)Dias.Miercoles:
                        ofertas = ofertas.Where(x => x.miercoles == (true));
                        break;
                    case (int)Dias.Jueves:
                        ofertas = ofertas.Where(x => x.jueves == (true));
                        break;
                    case (int)Dias.Viernes:
                        ofertas = ofertas.Where(x => x.viernes == (true));
                        break;
                    case (int)Dias.Sabado:
                        ofertas = ofertas.Where(x => x.sabado == (true));
                        break;
                }
            }



            if (VistaCompleta.HasValue && VistaCompleta.Value)
            {
                var ofertasFiltradas = new List<oferta>();
                foreach (var ofer in ofertas)
                {
                    var recaudacion = recaudacionPorComision(ofer.codigo);
                    ofer.importeInscritos = recaudacion;
                    bool cumple = true;
                    if (montoDesde.HasValue)
                    {
                        if (ofer.importeInscritos < montoDesde.Value)
                            cumple = false;
                    }
                    if (montoHasta.HasValue)
                    {
                        if (ofer.importeInscritos > montoHasta.Value)
                            cumple = false;
                    }
                    if (montoMayorMinimo.HasValue && montoMayorMinimo.Value)
                    {
                        if ((double)ofer.importeInscritos < ofer.recaudacionMinima)
                            cumple = false;
                    }

                    if (inscriptosDesde.HasValue)
                    {
                        if (ofer.cantidadInscriptos < inscriptosDesde.Value)
                            cumple = false;
                    }
                    if (inscriptosHasta.HasValue)
                    {
                        if (ofer.cantidadInscriptos > inscriptosHasta.Value)
                            cumple = false;
                    }
                    if (inscriptosMayorMinimo.HasValue && inscriptosMayorMinimo.Value)
                    {
                        if (ofer.cupoMinimo > ofer.cantidadInscriptos)
                            cumple = false;
                    }

                    if (ofer.cupo > ofer.cantidadPreInscriptos)
                    {
                        cumple = false;
                    }


                    if (cumple)
                    {
                        ofertasFiltradas.Add(ofer);
                    }
                }
                ofertas = ofertasFiltradas.AsQueryable();
            }

            return Json(new { cantidad = ofertas.Count() });
        }

        [HasPermission("Oferta_ABM")]
        [HttpPost]
        public ActionResult HabilitarTodo(bool habilitar, int? IdTipoCurso = null, int? IdSede = null, int? IdCurso = null, int? IdPeriodo = null, int? IdDia = null, string IdDocente = "", bool? VistaCompleta = null, decimal? montoDesde = null, decimal? montoHasta = null, int? inscriptosDesde = null, int? inscriptosHasta = null, bool? inscriptosMayorMinimo = null, bool? montoMayorMinimo = null)
        {
            List<oferta> ofertas = null;

            ofertas = consultarOfertas(IdTipoCurso, IdSede, IdCurso, IdPeriodo, IdDia, IdDocente, VistaCompleta, montoDesde, montoHasta, inscriptosDesde, inscriptosHasta, inscriptosMayorMinimo, montoMayorMinimo);

            ofertas.ForEach(x => x.habilitada = habilitar);

            db.SaveChanges();

            return Json(new { ok = true });
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

        public JsonResult GetOfertas(int? IdSede = null, int? IdPeriodo = null, string IdUsuario = "", int? IdCurso = null, int? IdTipoCurso = null, int? IdConcepto = null, bool? filtro = null)
        {
            IQueryable<oferta> ofertas = db.oferta;
            bool hayFiltro = false;
            if (User.IsInRole("AdministradorEmpresas"))
            {
                ofertas = ofertas.Where(x => /*x.curso1.tipocurso1.codigo.Equals(23)*/x.curso1.tipocurso1.sector == (int)Sectores.Empresas);
            }
            else
            {
                if (User.IsInRole("AdministradorGraduado"))
                {
                    ofertas = ofertas.Where(x => x.curso1.tipocurso1.sector == (int)Sectores.Graduados);
                }
                else
                {
                    ofertas = ofertas.Where(x => x.curso1.tipocurso1.sector == (int)Sectores.Idiomas);
                }

            }

            if (IdSede.HasValue && IdSede.Value != Constantes.ERROR)
            {
                ofertas = ofertas.Where(x => x.sede == IdSede.Value);
                hayFiltro = true;
            }
            if (IdPeriodo.HasValue && IdPeriodo.Value != Constantes.ERROR)
            {
                ofertas = ofertas.Where(x => x.periodoLectivo == IdPeriodo.Value);
                hayFiltro = true;
            }
            if (IdCurso.HasValue && IdCurso.Value != Constantes.ERROR)
            {
                ofertas = ofertas.Where(x => x.curso == IdCurso.Value);
                hayFiltro = true;
            }
            if (IdTipoCurso.HasValue && IdTipoCurso.Value != Constantes.ERROR)
            {
                ofertas = ofertas.Where(x => x.curso1.tipoCurso == IdTipoCurso.Value);
                hayFiltro = true;
            }
            if (IdConcepto.HasValue && IdConcepto.Value != Constantes.ERROR)
            {
                ofertas = ofertas.Where(x => x.curso1.concepto == IdConcepto.Value);
                hayFiltro = true;
            }
            if (!string.IsNullOrEmpty(IdUsuario) && IdUsuario != Constantes.ERROR.ToString())
            {
                ofertas = ofertas.Where(x => x.cursa.Any(c => c.alumno == IdUsuario));
                hayFiltro = true;
            }

            var ofertasSelect = new SelectList(
                ((hayFiltro) ? ofertas.ToList() : new List<oferta>())
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    (filtro.HasValue && filtro.Value) ? Strings.SeleccionarOferta : Strings.MsgTodosLasOfertas,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            return Json(ofertasSelect, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Buscar(string q)
        {
            IQueryable<oferta> ofertas = db.oferta;

            if (User.IsInRole("AdministradorEmpresas"))
            {
                ofertas = ofertas.Where(x => /*x.curso1.tipocurso1.codigo.Equals(23)*/x.curso1.tipocurso1.sector == (int)Sectores.Empresas);
            }
            else
            {
                if (User.IsInRole("AdministradorGraduado"))
                {
                    ofertas = ofertas.Where(x => x.curso1.tipocurso1.sector == (int)Sectores.Graduados);
                }
                else
                {
                    ofertas = ofertas.Where(x => x.curso1.tipocurso1.sector == (int)Sectores.Idiomas);
                }

            }

            ofertas = ofertas.Where(x => x.sede1.nombre.Contains(q)
                                    || x.periodolectivo1.anio.ToString().Contains(q)
                                    || x.curso1.descripcion.Contains(q)
                                    || x.curso1.tipocurso1.descripcion.Contains(q)
                                    || x.curso1.precio.descripcion.Contains(q)
                                  );


            var rta = ofertas.OrderByDescending(x => x.periodolectivo1.anio).ThenByDescending(x => x.periodolectivo1.periodo).Select(x => new
            {
                id = x.codigo,
                text =
                       "(" + x.periodolectivo1.anio + " - " + x.periodolectivo1.periodo + ") " +
                       x.curso1.tipocurso1.descripcion + " - " + x.curso1.descripcion + " - " +
                       x.sede1.nombre + " ["
                       + ((x.lunes) ? "LU " : "")
                       + ((x.martes) ? "MA " : "")
                       + ((x.miercoles) ? "MI " : "")
                       + ((x.jueves) ? "JU " : "")
                       + ((x.viernes) ? "VI " : "")
                       + ((x.sabado) ? "SA " : "")
                       + " " + x.horaDesde.Value.ToString().Substring(0, 5)
                       + " a " + x.horaHasta.Value.ToString().Substring(0, 5) + "]"
                //+ "] " + x.horaDesde.Value.ToString(@"hh\:mm") + " a " + x.horaHasta.Value.ToString(@"hh\:mm")
            }).ToList();


            return Json(rta, JsonRequestBehavior.AllowGet);
        }

        public decimal recaudacionPorComision(int NroComision)
        {
            decimal recaudacion;
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["ServicioTeso"]);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["ServicioTesoUsuario"])));

                List<KeyValuePair<string, object>> parametros = new List<KeyValuePair<string, object>>();
                parametros.Add(new KeyValuePair<string, object>("origen", ConfigurationManager.AppSettings["OrigenTeso"]));
                parametros.Add(new KeyValuePair<string, object>("comision", NroComision.ToString()));


                var responseGet = httpClient.GetAsync("Cuota/RecaudacionPorComision" + Constantes.AsQueryString(parametros)).Result;
                var respuesta = responseGet.Content.ReadAsAsync<decimal>().Result;
                if (responseGet.IsSuccessStatusCode == false)
                {
                    throw new Exception("Error en servicio Tesoreria");
                }
                else
                {
                    recaudacion = respuesta;
                }
            }

            return recaudacion;
        }

        #region Exportar

        public ActionResult Exportar(int? IdTipoCurso = null, int? IdSede = null, int? IdCurso = null, int? IdPeriodo = null, int? IdDia = null, string IdDocente = "", bool? VistaCompleta = null, decimal? montoDesde = null, decimal? montoHasta = null, int? inscriptosDesde = null, int? inscriptosHasta = null, bool? inscriptosMayorMinimo = null, bool? montoMayorMinimo = null)
        {

            List<oferta> ofertas = null;

            ofertas = consultarOfertas(IdTipoCurso, IdSede, IdCurso, IdPeriodo, IdDia, IdDocente, VistaCompleta, montoDesde, montoHasta, inscriptosDesde, inscriptosHasta, inscriptosMayorMinimo, montoMayorMinimo);

            ExportSettings.DefaultExportType = ExportType.WYSIWYG;
            return GridViewExtension.ExportToXlsx(CrearGridViewSettingsExportar("Oferta", VistaCompleta), ofertas, true);
        }

        private static GridViewSettings CrearGridViewSettingsExportar(string titulo, bool? vistaCompleta)
        {
            GridViewSettings settings = new GridViewSettings
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


            settings.ControlStyle.Paddings.Padding = Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = Unit.Pixel(0);
            settings.Styles.Header.HorizontalAlign = HorizontalAlign.Center;
            // Columnas
            if (vistaCompleta.HasValue && vistaCompleta.Value)
            {
                settings.Columns.Add(col => { col.FieldName = "curso1.tipocurso1.descripcion"; col.Caption = Strings.ColTipoCurso; });
                settings.Columns.Add(col => { col.FieldName = "curso1.descripcion"; col.Caption = Strings.ColCurso; });
                settings.Columns.Add(col => { col.FieldName = "sede1.nombre"; col.Caption = Strings.ColSede; });
                settings.Columns.Add(col => { col.FieldName = "periodolectivo1.descripcion"; col.Caption = Strings.ColPeriodo; });
                settings.Columns.Add(col => { col.FieldName = "horario"; col.Caption = "Horario"; });
                settings.Columns.Add(col => { col.FieldName = "cantidadInscriptos"; col.Caption = "Inscriptos"; });
                settings.Columns.Add(col => { col.FieldName = "cupo"; col.Caption = "Cupo"; });
                settings.Columns.Add(col => { col.FieldName = "cupoMinimo"; col.Caption = "Cupo Mínimo"; });
                settings.Columns.Add(col => { col.FieldName = "recaudacionMinima"; col.Caption = "Recaudacion Mínima"; col.PropertiesEdit.DisplayFormatString = "c"; });
                settings.Columns.Add(col => { col.FieldName = "importeInscritos"; col.Caption = "Recaudación Parcial"; col.PropertiesEdit.DisplayFormatString = "c"; });
                settings.Columns.Add(col => { col.FieldName = "AspNetUsers.descripcion"; col.Caption = Strings.ColDocente; });


                settings.Columns.Add(col => { col.FieldName = "habilitada"; col.Caption = "Habilitada"; col.ColumnType = MVCxGridViewColumnType.CheckBox; });
                settings.Columns.Add(col => { col.FieldName = "fechaDesde"; col.Caption = "Desde"; col.PropertiesEdit.DisplayFormatString = "d"; });
                settings.Columns.Add(col => { col.FieldName = "fechaHasta"; col.Caption = "Hasta"; col.PropertiesEdit.DisplayFormatString = "d"; });
                settings.Columns.Add(col => { col.FieldName = "inscripcionDesde"; col.Caption = "Inscripcion Desde"; col.PropertiesEdit.DisplayFormatString = "d"; });
                settings.Columns.Add(col => { col.FieldName = "inscripcionHasta"; col.Caption = "Inscripcion Hasta"; col.PropertiesEdit.DisplayFormatString = "d"; });

            }
            else
            {
                settings.Columns.Add(col => { col.FieldName = "codigo"; col.Caption = Strings.ColId; col.Visible = false; });
                settings.Columns.Add(col => { col.FieldName = "curso1.tipocurso1.descripcion"; col.Caption = Strings.ColTipoCurso; });
                settings.Columns.Add(col => { col.FieldName = "curso1.descripcion"; col.Caption = Strings.ColCurso; });
                settings.Columns.Add(col => { col.FieldName = "sede1.nombre"; col.Caption = Strings.ColSede; });
                settings.Columns.Add(col => { col.FieldName = "fechaDesde"; col.Caption = "Desde"; col.PropertiesEdit.DisplayFormatString = "d"; });
                settings.Columns.Add(col => { col.FieldName = "fechaHasta"; col.Caption = "Hasta"; col.PropertiesEdit.DisplayFormatString = "d"; });
                settings.Columns.Add(col => { col.FieldName = "habilitada"; col.Caption = "Habilitada"; col.ColumnType = MVCxGridViewColumnType.CheckBox; });
                settings.Columns.Add(col => { col.FieldName = "inscripcionDesde"; col.Caption = "Inscripcion Desde"; col.PropertiesEdit.DisplayFormatString = "d"; });
                settings.Columns.Add(col => { col.FieldName = "inscripcionHasta"; col.Caption = "Inscripcion Hasta"; col.PropertiesEdit.DisplayFormatString = "d"; });

            }
            return settings;
        }

        #endregion
    }
}
