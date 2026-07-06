using System.Web.Mvc;

namespace DNS.WebApp.Areas.MaterielModule
{
    public class MaterielModuleAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MaterielModule";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MaterielModule_default",
                "MaterielModule/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
