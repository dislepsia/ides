using cnrl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace cnrl.Logica
{
    public class HasPermissionAttribute : ActionFilterAttribute
    {
        private string _permission;

        public HasPermissionAttribute(string permission)
        {
            this._permission = permission;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            socioculturalesEntities db = new socioculturalesEntities();
            string rol = ApplicationUser.GetRol();
            if(!db.permiso.Where(x => x.nombre.Equals(this._permission) && x.tipoUsuario.Equals(rol)).Any())
            {
                RouteValueDictionary redirectTargetDictionary = new RouteValueDictionary();
                redirectTargetDictionary.Add("area", "");
                redirectTargetDictionary.Add("action", "NoAutorizado");
                redirectTargetDictionary.Add("controller", "Error");
                filterContext.Result = new RedirectToRouteResult(redirectTargetDictionary);
            }
        }
    }
}