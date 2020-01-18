using cnrl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace cnrl.Controllers
{
    public class MenuController : Controller
    {
        private socioculturalesEntities db = new socioculturalesEntities();

        public JsonResult CargarMenu()
        {
            var rolUsuario = ApplicationUser.GetRol();

            var elementosMenu = (from r in db.AspNetRoles
                                 from p in r.permiso
                                 where r.Id.Equals(rolUsuario) && p.menu.codPadre.HasValue.Equals(false)
                                 select p.menu).OrderBy(x => x.orden);

            string menu = "<ul class='sidebar-menu'> ";
            foreach (var elemento in elementosMenu)
            {
                if (elemento != null)
                {


                    if (elemento.menu1.Count == 0)
                    {
                        menu += string.Format("<li><a href = '{0}' ><i class='fa {1}'></i> <span>{2}</span></a></li> ",
                            Url.Action(elemento.accion, elemento.vista),
                            elemento.icono,
                            elemento.descripcion);
                    }
                    else
                    {
                        menu += "<li class='treeview'>";
                        menu += string.Format("<a href = '#' ><i class='fa {0}'></i> <span>{1}</span> <i class='fa fa-angle-left pull-right'></i></a>",
                                elemento.icono,
                                elemento.descripcion);
                        menu += "<ul class='treeview-menu'>";

                        var elementosHijo = (from p in db.permiso
                                             where p.AspNetRoles.Id.Equals(rolUsuario)
                                             && p.menu.codPadre.Value.Equals(elemento.codigo)
                                             select p.menu);

                        foreach (var elementoHijo in elementosHijo)
                        {
                            menu += string.Format("<li><a href = '{0}' > {1} </a></li>",
                                Url.Action(elementoHijo.accion, elementoHijo.vista),
                                elementoHijo.descripcion
                                );
                        }
                        menu += "</ul> </li>";
                    }
                }
            }

            menu += " </ul>";

            Session.Add("menu", menu);

            return Json(menu, JsonRequestBehavior.AllowGet);
        }


        public void CargarMenuStatic()
        {

            socioculturalesEntities db = new socioculturalesEntities();

            var rolUsuario = ApplicationUser.GetRol();

            var elementosMenu = (from r in db.AspNetRoles
                                 from p in r.permiso
                                 where r.Id.Equals(rolUsuario)
                                 select p.menu);

            string menu = "<ul class='sidebar-menu'> ";
            foreach (var elemento in elementosMenu)
            {
                if (elemento.menu1.Count == 0)
                {
                    menu += string.Format("<li><a href = '{0}' ><i class='fa {1}'></i> <span>{2}</span></a></li> ",
                        Url.Action(elemento.accion, elemento.vista),
                        elemento.icono,
                        elemento.descripcion);
                }
                else
                {
                    menu += "<li class='treeview'>";
                    menu += string.Format("<a href = '#' ><i class='fa {0}'></i> <span>{1}</span> <i class='fa fa-angle-left pull-right'></i></a>",
                            elemento.icono,
                            elemento.descripcion);
                    menu += "<ul class='treeview-menu'>";

                    var elementosHijo = (from p in db.permiso
                                         where p.AspNetRoles.Id.Equals(rolUsuario)
                                         && p.menu.codPadre.Value.Equals(elemento.codigo)
                                         select p.menu);

                    foreach (var elementoHijo in elementosHijo)
                    {
                        menu += string.Format("<li><a href = '{0}' > {1} </a></li>",
                            Url.Action(elementoHijo.accion, elementoHijo.vista),
                            elementoHijo.descripcion
                            );
                    }
                    menu += "</ul> </li>";
                }
            }

            menu += " </ul>";

            Session.Add("menu", menu);


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