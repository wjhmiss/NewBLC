using System.Web.Mvc;

namespace DNS.WebApp.Areas.PaymentModule
{
    public class PaymentModuleAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PaymentModule";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PaymentModule_default",
                "PaymentModule/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
