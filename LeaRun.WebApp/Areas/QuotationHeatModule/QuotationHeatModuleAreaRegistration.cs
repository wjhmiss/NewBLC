using System.Web.Mvc;

namespace DNS.WebApp.Areas.QuotationHeatModule
{
    public class QuotationHeatModuleAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "QuotationHeatModule";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "QuotationHeatModule_default",
                "QuotationHeatModule/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
