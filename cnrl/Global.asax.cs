using cnrl.Logica;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace cnrl
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ModelBinders.Binders.DefaultBinder = new DevExpress.Web.Mvc.DevExpressEditorsBinder();
            //ModelBinders.Binders.Add(typeof(double), new DoubleModelBinder());
            //ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            //ClientDataTypeModelValidatorProvider.ResourceClassKey = "ErroresPersonalizados";
            //DefaultModelBinder.ResourceClassKey = "ErroresPersonalizados";
            //MyErrorMessageProvider.SetResourceClassKey("ErroresPersonalizados");


        }
    }
}
