using System.Web.Mvc;

namespace DNS.WebApp.Areas.QuotationModule
{
    public class QuotationModuleAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "QuotationModule";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "QuotationModule_default",
                "QuotationModule/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
