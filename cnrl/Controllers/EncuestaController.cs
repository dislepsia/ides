using cnrl.Helpers;
using cnrl.Logica;
using DevExpress.Export;
using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Text;


namespace cnrl.Controllers
{
    public class EncuestaController : Controller
    {
        // GET: Encuesta
        private socioculturalesEntities db = new socioculturalesEntities();
        public ActionResult Index()
        {
            PrepararViewBag();
            return View();
        }

        private void PrepararViewBag()
        {
            ViewBag.Periodos = new SelectList(
                db.periodolectivo.OrderByDescending(x => x.fechaInscripcionHasta).Where(x => !x.tipocurso1.codigo.Equals(23)).ToList()
                .ToSelectList(
                    x => x.descripcion,
                    x => x.codigo.ToString(),
                    Strings.MsgTodosLosPeriodos,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            ViewBag.Zona = new SelectList(
                db.localidad
                .ToSelectList(
                    x => x.localidad1,
                    x => x.codigo.ToString(),
                    Strings.MsgTodasZonas,
                    Constantes.ERROR.ToString(),
                    Constantes.ERROR.ToString()
                )
            , "Value", "Text");

            IList<SelectListItem> list = Enum.GetValues(typeof(GrupoEtareo)).Cast<GrupoEtareo>().Select(x => new SelectListItem { Text = Logica.App.DisplayCamelCaseString(x.ToString()), Value = ((int)x).ToString() }).ToList().ToSelectList(
                         x => x.Text,
                         x => x.Value,
                         "(Todas las Edades)",
                         Constantes.ERROR.ToString(),
                         Constantes.ERROR.ToString()
                         );

            ViewBag.Edad = new SelectList(list, "Value", "Text");

        }

        [DbFunction("Socioculturales", "Edad")]
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

        public ActionResult PartialGrid(int? IdEdad = null, int? IdZona = null, int? IdPeriodo = null)
        {
            #region Consulta

            IQueryable<Encuesta> enc = db.Encuesta;

            if (IdEdad != null)
            {
                if (IdEdad == 1)
                {
                    enc = enc.Where(x => DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16);
                }
                if (IdEdad == 2)
                {
                    enc = enc.Where(x => DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25);
                }
                if (IdEdad == 3)
                {
                    enc = enc.Where(x => DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35);
                }
                if (IdEdad == 4)
                {
                    enc = enc.Where(x => DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36);
                }

            }

            if (IdZona != null && IdZona != -1)
            {
                enc = enc.Where(x => x.AspNetUsers.localidad.Value == IdZona);
            }

            if (IdPeriodo != null && IdPeriodo != -1)
            {
                enc = enc.Where(x => x.AspNetUsers.cursa.Where(c => c.oferta1.periodolectivo1.codigo == IdPeriodo && DbFunctions.TruncateTime(c.fechaAlta) == x.fecha && x.idCursa == c.codigo).Any());
                enc.Distinct();
            }

            //var lista = enc.Where(x => x.AspNetUsers.localidad1 != null).ToList();
            var lista = enc.ToList();
            var groups = lista.GroupBy(n => n.RespuestasEncuesta.id).Select(n => new
            {
                //idLocalidad = db.localidad.Where(x => x.codigo == n.Key).First().localidad1,
                idRespuesta = db.RespuestasEncuesta.Where(x => x.id == n.Key).First().respuesta.ToString().Normalize(),
                Cantidad = n.Count(),

                //facebook = (IdEdad == null || IdEdad == -1 ? enc.Where(x => x.facebook == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 1 ? enc.Where(x => x.facebook == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 2 ? enc.Where(x => x.facebook == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 3 ? enc.Where(x => x.facebook == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 4 ? enc.Where(x => x.facebook == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                ////facebook = enc.Where(x => x.facebook == true && x.AspNetUsers.localidad == n.Key).Count(),
                ////instagram = enc.Where(x => x.instagram == true && x.AspNetUsers.localidad == n.Key).Count(),
                ////WebUnlam = enc.Where(x => x.WebUnlam == true && x.AspNetUsers.localidad == n.Key).Count(),
                ////BuscadorWeb = enc.Where(x => x.BuscadorWeb == true && x.AspNetUsers.localidad == n.Key).Count(),
                ////PeriodicoUnlam = enc.Where(x => x.PeriodicoUnlam == true && x.AspNetUsers.localidad == n.Key).Count(),
                ////Cartelera = enc.Where(x => x.Cartelera == true && x.AspNetUsers.localidad == n.Key).Count(),
                ////ViaPublica = enc.Where(x => x.ViaPublica == true && x.AspNetUsers.localidad == n.Key).Count(),
                ////Radio = enc.Where(x => x.Radio == true && x.AspNetUsers.localidad == n.Key).Count(),
                ////CurseAntes = enc.Where(x => x.CurseAntes == true && x.AspNetUsers.localidad == n.Key).Count(),
                ////Alumno = enc.Where(x => x.Alumno == true && x.AspNetUsers.localidad == n.Key).Count(),
                ////AmigosConocidos = enc.Where(x => x.AmigosConocidos == true && x.AspNetUsers.localidad == n.Key).Count()

                //instagram = (IdEdad == null || IdEdad == -1 ? enc.Where(x => x.instagram == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 1 ? enc.Where(x => x.instagram == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 2 ? enc.Where(x => x.instagram == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 3 ? enc.Where(x => x.instagram == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 4 ? enc.Where(x => x.instagram == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                //WebUnlam = (IdEdad == null || IdEdad == -1 ? enc.Where(x => x.WebUnlam == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 1 ? enc.Where(x => x.WebUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 2 ? enc.Where(x => x.WebUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 3 ? enc.Where(x => x.WebUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 4 ? enc.Where(x => x.WebUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                //BuscadorWeb = (IdEdad == null || IdEdad == -1 ? enc.Where(x => x.BuscadorWeb == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 1 ? enc.Where(x => x.BuscadorWeb == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 2 ? enc.Where(x => x.BuscadorWeb == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 3 ? enc.Where(x => x.BuscadorWeb == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 4 ? enc.Where(x => x.BuscadorWeb == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                //PeriodicoUnlam = (IdEdad == null || IdEdad == -1 ? enc.Where(x => x.PeriodicoUnlam == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 1 ? enc.Where(x => x.PeriodicoUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 2 ? enc.Where(x => x.PeriodicoUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 3 ? enc.Where(x => x.PeriodicoUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 4 ? enc.Where(x => x.PeriodicoUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                //Cartelera = (IdEdad == null || IdEdad == -1 ? enc.Where(x => x.Cartelera == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 1 ? enc.Where(x => x.Cartelera == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 2 ? enc.Where(x => x.Cartelera == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 3 ? enc.Where(x => x.Cartelera == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 4 ? enc.Where(x => x.Cartelera == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                //ViaPublica = (IdEdad == null || IdEdad == -1 ? enc.Where(x => x.ViaPublica == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 1 ? enc.Where(x => x.ViaPublica == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 2 ? enc.Where(x => x.ViaPublica == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 3 ? enc.Where(x => x.ViaPublica == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 4 ? enc.Where(x => x.ViaPublica == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                //Radio = (IdEdad == null || IdEdad == -1 ? enc.Where(x => x.Radio == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 1 ? enc.Where(x => x.Radio == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 2 ? enc.Where(x => x.Radio == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 3 ? enc.Where(x => x.Radio == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 4 ? enc.Where(x => x.Radio == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                //CurseAntes = (IdEdad == null || IdEdad == -1 ? enc.Where(x => x.CurseAntes == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 1 ? enc.Where(x => x.CurseAntes == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 2 ? enc.Where(x => x.CurseAntes == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 3 ? enc.Where(x => x.CurseAntes == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 4 ? enc.Where(x => x.CurseAntes == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                //Alumno = (IdEdad == null || IdEdad == -1 ? enc.Where(x => x.Alumno == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 1 ? enc.Where(x => x.Alumno == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 2 ? enc.Where(x => x.Alumno == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 3 ? enc.Where(x => x.Alumno == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 4 ? enc.Where(x => x.Alumno == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                //AmigosConocidos = (IdEdad == null || IdEdad == -1 ? enc.Where(x => x.AmigosConocidos == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 1 ? enc.Where(x => x.AmigosConocidos == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 2 ? enc.Where(x => x.AmigosConocidos == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 3 ? enc.Where(x => x.AmigosConocidos == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(IdEdad == 4 ? enc.Where(x => x.AmigosConocidos == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0)))))
            }
            ).OrderBy(n => n.idRespuesta);

            #endregion Consulta

            return PartialView("_Grid", groups);
        }

        public JsonResult NewChart(int? Edad = null, int? Periodo = null, int? Zona = null)
        {
            #region Consulta

            IQueryable<Encuesta> enc = db.Encuesta;

            if (Edad != null)
            {
                if (Edad == 1)
                {
                    enc = enc.Where(x => DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16);
                }
                if (Edad == 2)
                {
                    enc = enc.Where(x => DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25);
                }
                if (Edad == 3)
                {
                    enc = enc.Where(x => DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35);
                }
                if (Edad == 4)
                {
                    enc = enc.Where(x => DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36);
                }

            }

            if (Zona != null && Zona != -1)
            {
                enc = enc.Where(x => x.AspNetUsers.localidad.Value == Zona);
            }

            if (Periodo != null && Periodo != -1)
            {
                enc = enc.Where(x => x.AspNetUsers.cursa.Where(c => c.oferta1.periodolectivo1.codigo == Periodo && DbFunctions.TruncateTime(c.fechaAlta) == x.fecha && x.idCursa == c.codigo).Any());
            }

            //var lista = enc.Where(x => x.AspNetUsers.localidad1 != null).ToList();
            var lista = enc.ToList();
            var groups = lista.GroupBy(n => n.RespuestasEncuesta.id).Select(n => new
            {
                //idLocalidad = db.localidad.Where(x => x.codigo == n.Key).First().localidad1,
                idRespuesta = db.RespuestasEncuesta.Where(x => x.id == n.Key).First().respuesta,
                Cantidad = n.Count(),

                //facebook = (Edad == null || Edad == -1 ? enc.Where(x => x.facebook == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 1 ? enc.Where(x => x.facebook == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 2 ? enc.Where(x => x.facebook == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 3 ? enc.Where(x => x.facebook == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 4 ? enc.Where(x => x.facebook == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                //instagram = (Edad == null || Edad == -1 ? enc.Where(x => x.instagram == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 1 ? enc.Where(x => x.instagram == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 2 ? enc.Where(x => x.instagram == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 3 ? enc.Where(x => x.instagram == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 4 ? enc.Where(x => x.instagram == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                //WebUnlam = (Edad == null || Edad == -1 ? enc.Where(x => x.WebUnlam == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 1 ? enc.Where(x => x.WebUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 2 ? enc.Where(x => x.WebUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 3 ? enc.Where(x => x.WebUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 4 ? enc.Where(x => x.WebUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                //BuscadorWeb = (Edad == null || Edad == -1 ? enc.Where(x => x.BuscadorWeb == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 1 ? enc.Where(x => x.BuscadorWeb == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 2 ? enc.Where(x => x.BuscadorWeb == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 3 ? enc.Where(x => x.BuscadorWeb == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 4 ? enc.Where(x => x.BuscadorWeb == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                //PeriodicoUnlam = (Edad == null || Edad == -1 ? enc.Where(x => x.PeriodicoUnlam == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 1 ? enc.Where(x => x.PeriodicoUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 2 ? enc.Where(x => x.PeriodicoUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 3 ? enc.Where(x => x.PeriodicoUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 4 ? enc.Where(x => x.PeriodicoUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                //Cartelera = (Edad == null || Edad == -1 ? enc.Where(x => x.Cartelera == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 1 ? enc.Where(x => x.Cartelera == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 2 ? enc.Where(x => x.Cartelera == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 3 ? enc.Where(x => x.Cartelera == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 4 ? enc.Where(x => x.Cartelera == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                //ViaPublica = (Edad == null || Edad == -1 ? enc.Where(x => x.ViaPublica == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 1 ? enc.Where(x => x.ViaPublica == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 2 ? enc.Where(x => x.ViaPublica == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 3 ? enc.Where(x => x.ViaPublica == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 4 ? enc.Where(x => x.ViaPublica == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                //Radio = (Edad == null || Edad == -1 ? enc.Where(x => x.Radio == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 1 ? enc.Where(x => x.Radio == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 2 ? enc.Where(x => x.Radio == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 3 ? enc.Where(x => x.Radio == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 4 ? enc.Where(x => x.Radio == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                //CurseAntes = (Edad == null || Edad == -1 ? enc.Where(x => x.CurseAntes == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 1 ? enc.Where(x => x.CurseAntes == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 2 ? enc.Where(x => x.CurseAntes == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 3 ? enc.Where(x => x.CurseAntes == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 4 ? enc.Where(x => x.CurseAntes == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                //Alumno = (Edad == null || Edad == -1 ? enc.Where(x => x.Alumno == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 1 ? enc.Where(x => x.Alumno == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 2 ? enc.Where(x => x.Alumno == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 3 ? enc.Where(x => x.Alumno == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 4 ? enc.Where(x => x.Alumno == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                //AmigosConocidos = (Edad == null || Edad == -1 ? enc.Where(x => x.AmigosConocidos == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 1 ? enc.Where(x => x.AmigosConocidos == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 2 ? enc.Where(x => x.AmigosConocidos == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 3 ? enc.Where(x => x.AmigosConocidos == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                //(Edad == 4 ? enc.Where(x => x.AmigosConocidos == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0)))))

                ////facebook = enc.Where(x => x.facebook == true && x.AspNetUsers.localidad == n.Key).Count(),
                ////instagram = enc.Where(x => x.instagram == true && x.AspNetUsers.localidad == n.Key).Count(),
                ////WebUnlam = enc.Where(x => x.WebUnlam == true && x.AspNetUsers.localidad == n.Key).Count(),
                ////BuscadorWeb = enc.Where(x => x.BuscadorWeb == true && x.AspNetUsers.localidad == n.Key).Count(),
                ////PeriodicoUnlam = enc.Where(x => x.PeriodicoUnlam == true && x.AspNetUsers.localidad == n.Key).Count(),
                ////Cartelera = enc.Where(x => x.Cartelera == true && x.AspNetUsers.localidad == n.Key).Count(),
                ////ViaPublica = enc.Where(x => x.ViaPublica == true && x.AspNetUsers.localidad == n.Key).Count(),
                ////Radio = enc.Where(x => x.Radio == true && x.AspNetUsers.localidad == n.Key).Count(),
                ////CurseAntes = enc.Where(x => x.CurseAntes == true && x.AspNetUsers.localidad == n.Key).Count(),
                ////Alumno = enc.Where(x => x.Alumno == true && x.AspNetUsers.localidad == n.Key).Count(),
                ////AmigosConocidos = enc.Where(x => x.AmigosConocidos == true && x.AspNetUsers.localidad == n.Key).Count()
            }
            ).OrderBy(n => n.idRespuesta);

            #endregion Consulta

            List<object> iData = new List<object>();
            DataTable dt = new DataTable();
            dt.Columns.Add("Employee", System.Type.GetType("System.String"));
            dt.Columns.Add("Credit", System.Type.GetType("System.Int32"));
            //dt.Columns.Add("Face", System.Type.GetType("System.Int32"));
            //dt.Columns.Add("Inst", System.Type.GetType("System.Int32"));
            //dt.Columns.Add("Web", System.Type.GetType("System.Int32"));
            //dt.Columns.Add("Buscador", System.Type.GetType("System.Int32"));
            //dt.Columns.Add("Periodico", System.Type.GetType("System.Int32"));
            //dt.Columns.Add("Cartel", System.Type.GetType("System.Int32"));
            //dt.Columns.Add("Via", System.Type.GetType("System.Int32"));
            //dt.Columns.Add("Radios", System.Type.GetType("System.Int32"));
            //dt.Columns.Add("CursoAntes", System.Type.GetType("System.Int32"));
            //dt.Columns.Add("Alumnos", System.Type.GetType("System.Int32"));
            //dt.Columns.Add("Amigos", System.Type.GetType("System.Int32"));

            foreach (var dato in groups)
            {
                DataRow dr = dt.NewRow();
                dr["Employee"] = dato.idRespuesta;
                dr["Credit"] = dato.Cantidad;
                //dr["Face"] = dato.facebook;
                //dr["Inst"] = dato.instagram;
                //dr["Web"] = dato.WebUnlam;
                //dr["Buscador"] = dato.BuscadorWeb;
                //dr["Periodico"] = dato.PeriodicoUnlam;
                //dr["Cartel"] = dato.Cartelera;
                //dr["Via"] = dato.ViaPublica;
                //dr["Radios"] = dato.Radio;
                //dr["CursoAntes"] = dato.CurseAntes;
                //dr["Alumnos"] = dato.Alumno;
                //dr["Amigos"] = dato.AmigosConocidos;

                dt.Rows.Add(dr);

            }

            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                iData.Add(x);
            }

            return Json(iData, JsonRequestBehavior.AllowGet);
        }

        #region Exportar
        [HttpPost]
        public ActionResult Exportar(int? IdEdad = null, int? IdZona = null, int? IdPeriodo = null)
        {
            #region Consulta
            var req = Request.Params["VistaCompleta"];

            if (req == "1")
            {



                IQueryable<Encuesta> enc = db.Encuesta;

                if (IdEdad != null)
                {
                    if (IdEdad == 1)
                    {
                        enc = enc.Where(x => DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16);
                    }
                    if (IdEdad == 2)
                    {
                        enc = enc.Where(x => DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25);
                    }
                    if (IdEdad == 3)
                    {
                        enc = enc.Where(x => DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35);
                    }
                    if (IdEdad == 4)
                    {
                        enc = enc.Where(x => DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36);
                    }

                }

                if (IdZona != null && IdZona != -1)
                {
                    enc = enc.Where(x => x.AspNetUsers.localidad.Value == IdZona);
                }

                if (IdPeriodo != null && IdPeriodo != -1)
                {
                    enc = enc.Where(x => x.AspNetUsers.cursa.Where(c => c.oferta1.periodolectivo1.codigo == IdPeriodo && DbFunctions.TruncateTime(c.fechaAlta) == x.fecha && x.idCursa == c.codigo).Any());
                    enc.Distinct();
                }

                //var lista = enc.Where(x => x.AspNetUsers.localidad1 != null).ToList();
                var lista = enc.ToList();
                var groups = lista.GroupBy(n => n.RespuestasEncuesta.id).Select(n => new
                {
                    //idLocalidad = db.localidad.Where(x => x.codigo == n.Key).First().localidad1,
                    idRespuesta = db.RespuestasEncuesta.Where(x => x.id == n.Key).First().respuesta,
                    Cantidad = n.Count(),

                    //facebook = (IdEdad == null || IdEdad == -1 ? enc.Where(x => x.facebook == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 1 ? enc.Where(x => x.facebook == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 2 ? enc.Where(x => x.facebook == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 3 ? enc.Where(x => x.facebook == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 4 ? enc.Where(x => x.facebook == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                    //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),



                    //instagram = (IdEdad == null || IdEdad == -1 ? enc.Where(x => x.instagram == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 1 ? enc.Where(x => x.instagram == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 2 ? enc.Where(x => x.instagram == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 3 ? enc.Where(x => x.instagram == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 4 ? enc.Where(x => x.instagram == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                    //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                    //WebUnlam = (IdEdad == null || IdEdad == -1 ? enc.Where(x => x.WebUnlam == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 1 ? enc.Where(x => x.WebUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 2 ? enc.Where(x => x.WebUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 3 ? enc.Where(x => x.WebUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 4 ? enc.Where(x => x.WebUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                    //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                    //BuscadorWeb = (IdEdad == null || IdEdad == -1 ? enc.Where(x => x.BuscadorWeb == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 1 ? enc.Where(x => x.BuscadorWeb == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 2 ? enc.Where(x => x.BuscadorWeb == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 3 ? enc.Where(x => x.BuscadorWeb == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 4 ? enc.Where(x => x.BuscadorWeb == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                    //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                    //PeriodicoUnlam = (IdEdad == null || IdEdad == -1 ? enc.Where(x => x.PeriodicoUnlam == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 1 ? enc.Where(x => x.PeriodicoUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 2 ? enc.Where(x => x.PeriodicoUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 3 ? enc.Where(x => x.PeriodicoUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 4 ? enc.Where(x => x.PeriodicoUnlam == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                    //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                    //Cartelera = (IdEdad == null || IdEdad == -1 ? enc.Where(x => x.Cartelera == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 1 ? enc.Where(x => x.Cartelera == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 2 ? enc.Where(x => x.Cartelera == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 3 ? enc.Where(x => x.Cartelera == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 4 ? enc.Where(x => x.Cartelera == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                    //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                    //ViaPublica = (IdEdad == null || IdEdad == -1 ? enc.Where(x => x.ViaPublica == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 1 ? enc.Where(x => x.ViaPublica == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 2 ? enc.Where(x => x.ViaPublica == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 3 ? enc.Where(x => x.ViaPublica == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 4 ? enc.Where(x => x.ViaPublica == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                    //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                    //Radio = (IdEdad == null || IdEdad == -1 ? enc.Where(x => x.Radio == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 1 ? enc.Where(x => x.Radio == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 2 ? enc.Where(x => x.Radio == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 3 ? enc.Where(x => x.Radio == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 4 ? enc.Where(x => x.Radio == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                    //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                    //CurseAntes = (IdEdad == null || IdEdad == -1 ? enc.Where(x => x.CurseAntes == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 1 ? enc.Where(x => x.CurseAntes == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 2 ? enc.Where(x => x.CurseAntes == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 3 ? enc.Where(x => x.CurseAntes == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 4 ? enc.Where(x => x.CurseAntes == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                    //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                    //Alumno = (IdEdad == null || IdEdad == -1 ? enc.Where(x => x.Alumno == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 1 ? enc.Where(x => x.Alumno == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 2 ? enc.Where(x => x.Alumno == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 3 ? enc.Where(x => x.Alumno == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 4 ? enc.Where(x => x.Alumno == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                    //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0))))),

                    //AmigosConocidos = (IdEdad == null || IdEdad == -1 ? enc.Where(x => x.AmigosConocidos == true && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 1 ? enc.Where(x => x.AmigosConocidos == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 10 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 16 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 2 ? enc.Where(x => x.AmigosConocidos == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 17 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 25 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 3 ? enc.Where(x => x.AmigosConocidos == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 26 &&
                    //DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) <= 35 && x.AspNetUsers.localidad.Value == n.Key).Count() :
                    //(IdEdad == 4 ? enc.Where(x => x.AmigosConocidos == true && DbFunctions.DiffYears(x.AspNetUsers.FechaNacimiento.Value, DateTime.Now) >= 36
                    //&& x.AspNetUsers.localidad.Value == n.Key).Count() : 0)))))
                }
                ).OrderBy(n => n.idRespuesta);

                #endregion Consulta

                ExportSettings.DefaultExportType = ExportType.WYSIWYG;
                return GridViewExtension.ExportToXlsx(CrearGridViewSettingsExportar("Informe de Encuesta"), groups, true);
            }
            else
            {
                return RedirectToAction("Index", "Encuesta");
            }
        }

        [HttpGet]
        public JsonResult Buscar(string q)
        {
            IQueryable<localidad> localidad = db.localidad;

            var split = q.Split(' ');

            foreach (var termino in split)
            {
                localidad = localidad.Where(x => x.localidad1.Contains(termino));
            }
            //  var institutos = InstitutoData.Buscar(q);
            var rta = localidad.Select(x => new { id = x.codigo, text = x.localidad1 }).ToList();


            return Json(rta, JsonRequestBehavior.AllowGet);
        }

        private static GridViewSettings CrearGridViewSettingsExportar(string titulo)
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
            settings.Columns.Add(col => { col.FieldName = "idRespuesta"; col.Caption = "Respuesta"; });
            settings.Columns.Add(col => { col.FieldName = "Cantidad"; col.Caption = "Cantidad Total"; });
            //settings.Columns.Add(col => { col.FieldName = "facebook"; col.Caption = "Facebook"; });
            //settings.Columns.Add(col => { col.FieldName = "instagram"; col.Caption = "Instagram"; });
            //settings.Columns.Add(col => { col.FieldName = "WebUnlam"; col.Caption = "Web UNLaM"; });
            //settings.Columns.Add(col => { col.FieldName = "BuscadorWeb"; col.Caption = "Buscador Web"; });
            //settings.Columns.Add(col => { col.FieldName = "PeriodicoUnlam"; col.Caption = "Periodico UNLaM"; });
            //settings.Columns.Add(col => { col.FieldName = "Cartelera"; col.Caption = "Cartelera"; });
            //settings.Columns.Add(col => { col.FieldName = "ViaPublica"; col.Caption = "Via Publica"; });
            //settings.Columns.Add(col => { col.FieldName = "Radio"; col.Caption = "Radio"; });
            //settings.Columns.Add(col => { col.FieldName = "CurseAntes"; col.Caption = "Curse Antes"; });
            //settings.Columns.Add(col => { col.FieldName = "Alumno"; col.Caption = "Alumno"; });
            //settings.Columns.Add(col => { col.FieldName = "AmigosConocidos"; col.Caption = "Amigos o Conocidos"; });


            settings.TotalSummary.Add(DevExpress.Data.SummaryItemType.Sum, "Cantidad");
            settings.Settings.ShowFooter = true;
            //settings.GroupSummary.Add(DevExpress.Data.SummaryItemType.Sum, "Total");

            //settings.Settings.ShowGroupPanel = true;

            return settings;
        }

        #endregion
    }
}