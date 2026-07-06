using System.Web.Mvc;

namespace DNS.WebApp.Areas.ReportModule
{
    public class ReportModuleAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ReportModule";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ReportModule_default",
                "ReportModule/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
