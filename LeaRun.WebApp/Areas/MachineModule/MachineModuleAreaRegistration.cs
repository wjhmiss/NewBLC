using System.Web.Mvc;

namespace DNS.WebApp.Areas.MachineModule
{
    public class MachineModuleAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MachineModule";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MachineModule_default",
                "MachineModule/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
