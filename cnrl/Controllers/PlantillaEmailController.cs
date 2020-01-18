using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace cnrl.Controllers
{
    public class PlantillaEmailController : Controller
    {
        private socioculturalesEntities db = new socioculturalesEntities();

        // GET: PlantillaEmail
        public ActionResult Index()
        {
            return View();
        }

      
        #region ajax
        public JsonResult GetPlantilla(int IdPlantilla = -1)
        {
            var plantilla = db.plantillaEmail.Find(IdPlantilla);
            var resultado = new Dictionary<String, String>();
            if (plantilla != null)
            {
                resultado.Add("cuerpo", plantilla.cuerpo.ToString());
                resultado.Add("asunto", plantilla.asunto.ToString());
            }
            return Json(resultado, JsonRequestBehavior.AllowGet);
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
    }
}