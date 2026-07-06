using DNS.Business;
using DNS.Entity;
using DNS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.QualityModule.Controllers
{
    /// <summary>
    /// BY_CSBReport¿ØÖÆÆ÷
    /// </summary>
    public class BY_CSBReportController : PublicController<BY_CSBReport>
    {
        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult Print()
        {
            return View();
        }
    }
}