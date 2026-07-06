using System.Web.Mvc;

namespace DNS.WebApp.Areas.WeiWebSite
{
    public class WeiWebSiteAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "WeiWebSite";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "WeiWebSite_default",
                "WeiWebSite/{controller}/{action}/{id}",
                new { action = "WeiLogin", id = UrlParameter.Optional }
            );
        }
    }
}
