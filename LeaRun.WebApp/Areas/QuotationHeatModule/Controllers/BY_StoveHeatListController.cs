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

namespace DNS.WebApp.Areas.QuotationHeatModule.Controllers
{
    /// <summary>
    /// BY_StoveHeatList¿ØÖÆÆ÷
    /// </summary>
    public class BY_StoveHeatListController : PublicController<BY_StoveHeatList>
    {
        public ActionResult GetListJson1(string StoveId)
        {
            try
            {
                List<BY_StoveHeatList> ListData = DataFactory.Database().FindList<BY_StoveHeatList>("StoveId", StoveId);

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