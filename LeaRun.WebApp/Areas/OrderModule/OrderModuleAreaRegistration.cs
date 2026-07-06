using System.Web.Mvc;

namespace DNS.WebApp.Areas.OrderModule
{
    public class OrderModuleAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "OrderModule";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "OrderModule_default",
                "OrderModule/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
