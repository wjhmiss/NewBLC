using System.Web.Mvc;

namespace DNS.WebApp.Areas.QualityModule
{
    public class QualityModuleAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "QualityModule";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "QualityModule_default",
                "QualityModule/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
