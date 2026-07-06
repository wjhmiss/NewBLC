using System.Web.Mvc;

namespace DNS.WebApp.Areas.StorageModule
{
    public class StorageModuleAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "StorageModule";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "StorageModule_default",
                "StorageModule/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
