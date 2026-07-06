using System.Web.Mvc;

namespace DNS.WebApp.Areas.DeliveryModule
{
    public class DeliveryModuleAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "DeliveryModule";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "DeliveryModule_default",
                "DeliveryModule/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
