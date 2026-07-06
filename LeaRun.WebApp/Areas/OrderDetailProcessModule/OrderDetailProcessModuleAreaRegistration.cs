using System.Web.Mvc;

namespace DNS.WebApp.Areas.OrderDetailProcessModule
{
    public class OrderDetailProcessModuleAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "OrderDetailProcessModule";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "OrderDetailProcessModule_default",
                "OrderDetailProcessModule/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
