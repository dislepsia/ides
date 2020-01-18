using System.Web.Mvc;

namespace cnrl.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Mostrar(
            string titulo = "Lo sentimos",
            string msg = "Página no encontrada"
        )
        {
            TempData["titulo"] = titulo;
            TempData["msg"] = msg;
            return RedirectToAction("Index");
        }
        public ActionResult Index()
        {
            ViewBag.titulo = TempData["titulo"];
            ViewBag.msg = TempData["msg"];
            return View("Error");
        }

        public ActionResult NoAutorizado()
        {
            ViewBag.titulo = "Lo sentimos";
            ViewBag.msg = "Usted no posee permisos para acceder a la página o acción solicitada";
            return View("Error");
        }
    }
}