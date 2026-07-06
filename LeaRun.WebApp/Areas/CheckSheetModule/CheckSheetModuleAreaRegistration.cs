using System.Web.Mvc;

namespace DNS.WebApp.Areas.CheckSheetModule
{
    public class CheckSheetModuleAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "CheckSheetModule";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "CheckSheetModule_default",
                "CheckSheetModule/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
