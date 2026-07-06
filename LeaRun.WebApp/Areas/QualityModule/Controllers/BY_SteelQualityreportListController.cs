using DNS.Business;
using DNS.Entity;
using DNS.Repository;
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
    /// BY_SteelQualityreportList¿ØÖÆÆ÷
    /// </summary>
    public class BY_SteelQualityreportListController : PublicController<BY_SteelQualityreportList>
    {
        public ActionResult GetListJson1(string SteelQualityreportId)
        {
            try
            {
                List<BY_SteelQualityreportList> ListData = DataFactory.Database().FindList<BY_SteelQualityreportList>("SteelQualityreportId", SteelQualityreportId);

                return Content(ListData.OrderBy(p => p.SortCode).ToJson());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "Òì³£´íÎó£º" + ex.Message);
                return null;
            }
        }
    }
}