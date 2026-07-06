using System.Web.Mvc;

namespace DNS.WebApp.Areas.OfficialWebSite
{
    public class OfficialWebSiteAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "OfficialWebSite";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "OfficialWebSite_default",
                "OfficialWebSite/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
