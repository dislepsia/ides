using System.Web.Mvc;
using System.Collections.Generic;
using cnrl.Models;
using System.Linq;
using Microsoft.AspNet.Identity;

namespace cnrl.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private socioculturalesEntities db = new socioculturalesEntities();
        public ActionResult Index()
        {
            //var controlador = new MenuController();
            //controlador.CargarMenuStatic();
            //var rolUsuario = ApplicationUser.GetRol();

            //var elementosMenu = (from r in db.AspNetRoles
            //                     from p in r.permiso
            //                     where r.Id.Equals(rolUsuario)
            //                     select p.menu);

            //string menu = "<ul class='sidebar-menu'> ";
            //foreach(var elemento in elementosMenu)
            //{
            //    if (elemento.menu1.Count == 0)
            //    {
            //        menu += string.Format("<li><a href = '{0}' ><i class='fa {1}'></i> <span>{2}</span></a></li> ",
            //            Url.Action(elemento.accion, elemento.vista),
            //            elemento.icono,
            //            elemento.descripcion);
            //    }
            //    else
            //    {
            //        menu += "<li class='treeview'>";
            //        menu += string.Format("<a href = '#' ><i class='fa {0}'></i> <span>{1}</span> <i class='fa fa-angle-left pull-right'></i></a>",
            //                elemento.icono,
            //                elemento.descripcion);
            //        menu += "<ul class='treeview-menu'>";

            //        var elementosHijo = (from p in db.permiso
            //             where p.AspNetRoles.Id.Equals(rolUsuario)
            //             && p.menu.codPadre.Value.Equals(elemento.codigo)
            //             select p.menu);

            //        foreach (var elementoHijo in elementosHijo)
            //        {
            //            menu += string.Format("<li><a href = '{0}' > {1} </a></li>",
            //                Url.Action(elementoHijo.accion, elementoHijo.vista),
            //                elementoHijo.descripcion
            //                );
            //        }
            //        menu += "</ul> </li>";
            //    }    
            //}

            //menu += " </ul>";

            //Session.Add("menu", menu);
            //ViewBag.ElementosMenu = elementosMenu.ToList();
            //ViewBag.RolUsuario = rolUsuario;
            //           ViewBag.menu = menu;
            return View();
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