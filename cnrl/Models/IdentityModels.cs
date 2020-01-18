using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace cnrl.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {

            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public String passwordInicial { get; set; }

        public String Apellido { get; set; }
        public String Calle { get; set; }
        public String dpto { get; set; }

        public DateTime FechaNacimiento { get; set; }
        public int localidad { get; set; }

        public Nullable<int> NroCalle { get; set; }
        public int NroDocumento { get; set; }
        public String piso
        {
            get; set;
        }

        public int provincia { get; set; }
        public String Telefono { get; set; }
        public String Telefono2 { get; set; }
        public int TipoDocumento { get; set; }
        public String Nombre { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Strings), ErrorMessageResourceName = "ErrorCampoObligatorio")]
        public override String Email { get; set; }

    

        public static String GetRol()
        {
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var userManager = new UserManager<ApplicationUser>(store);
            ApplicationUser user = userManager.FindByNameAsync(HttpContext.Current.User.Identity.Name).Result;

            

            return ((user != null) ? user.Roles.First().RoleId : "-1");
        }

        public static String GetApellidoYNombre()
        {
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var userManager = new UserManager<ApplicationUser>(store);
            ApplicationUser user = userManager.FindByNameAsync(HttpContext.Current.User.Identity.Name).Result;
            return ((user != null) ? String.Format("{0}, {1}", user.Apellido, user.Nombre) : "");
        }

        public static String GetId()
        {
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var userManager = new UserManager<ApplicationUser>(store);
            ApplicationUser user = userManager.FindByNameAsync(HttpContext.Current.User.Identity.Name).Result;
            
            return ((user != null) ? user.Id : "");
        }

        public static bool userHasPermission(string permiso)
        {
            socioculturalesEntities db = new socioculturalesEntities();
            string rol = ApplicationUser.GetRol();
            return db.permiso.Where(x => x.nombre.Equals(permiso) && x.tipoUsuario.Equals(rol)).Any();
        }

        //    socioculturalesEntities db = new socioculturalesEntities();
        //    string rol = ApplicationUser.GetRol();
        //        if(!db.permiso.Where(x => x.nombre.Equals(this._permission) && x.tipoUsuario.Equals(rol)).Any())
        //        {
        //            RouteValueDictionary redirectTargetDictionary = new RouteValueDictionary();
        //    redirectTargetDictionary.Add("area", "");
        //            redirectTargetDictionary.Add("action", "NoAutorizado");
        //            redirectTargetDictionary.Add("controller", "Error");
        //            filterContext.Result = new RedirectToRouteResult(redirectTargetDictionary);
        //}

        public static int GetSede()
        {
            socioculturalesEntities db = new socioculturalesEntities();

            var usuario = db.AspNetUsers.Where(x => x.UserName == HttpContext.Current.User.Identity.Name).FirstOrDefault();

            if (usuario != null && usuario.sede.HasValue)
            {
                return usuario.sede.Value;
            }

            return -1;
        }
    }



    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("Cnrl", false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<cnrl.Models.Usuario> Usuarios { get; set; }
    }
}