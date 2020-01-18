using cnrl.Models;
using cnrl.Repositories;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using cnrl.Logica;
using DevExpress.Export;
using Rotativa;

namespace cnrl.Controllers
{
    [Authorize]
    public class AsistenciaController : Controller
    {
        private socioculturalesEntities db = new socioculturalesEntities();

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Ficha(int id, bool imprimir = false)
        {
            var asistencia = db.oferta.Find(id);
            ViewBag.Imprimir = imprimir;
            //imprimir = true;

            //if (imprimir)
            //{
            //return new Rotativa.ViewAsPdf("Ficha", asistencia);
            try
            {
                //return new Rotativa.UrlAsPdf(Url.Content("~/Asistencia/FichaParaImprimir/" + id))
                //return new Rotativa.UrlAsPdf(("http://localhost:44308/Asistencia/FichaParaImprimir/" + id))
                return new Rotativa.UrlAsPdf(("http://10.10.201.29/Asistencia/FichaParaImprimir/" + id))
                {
                    FileName = String.Format("Asistencia_{0}.pdf", id),
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
            //}
            //else
            //return View("Ficha", asistencia);
            //FichaParaImprimir(id);
        }

        [AllowAnonymous]
        public ActionResult FichaParaImprimir(int id)
        {
            //var asistencia = db.oferta.Where(x => x.codigo == id);
            var asistencia = db.oferta.Find(id);

            ViewBag.Imprimir = true;
            if (asistencia.docente != null)
            {
                ViewBag.NombreDocente = db.AspNetUsers.Where(x => x.Id.Equals(asistencia.docente)).First().Apellido.ToUpper().ToString() + " " + db.AspNetUsers.Where(x => x.Id.Equals(asistencia.docente)).First().Nombre.ToUpper().ToString();
            }
            else
            {
                ViewBag.NombreDocente = "";
            }


            return View("Ficha", asistencia);
        }
    }
}
