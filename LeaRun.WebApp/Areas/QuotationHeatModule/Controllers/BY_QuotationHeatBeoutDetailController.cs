using DNS.Business;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.QuotationHeatModule.Controllers
{
    /// <summary>
    /// BY_QuotationHeatBeoutDetail¿ØÖÆÆ÷
    /// </summary>
    public class BY_QuotationHeatBeoutDetailController : PublicController<BY_QuotationHeatBeoutDetail>
    {
        public ActionResult GetListJson1(string QuotationHeatBeoutCode)
        {
            try
            {
                List<BY_QuotationHeatBeoutDetail> ListData = DataFactory.Database().FindList<BY_QuotationHeatBeoutDetail>("QuotationHeatBeoutCode", QuotationHeatBeoutCode);

                return Content(ListData.OrderBy(p => p.SortCode).ToJson());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "Òì³£´íÎó£º" + ex.Message);
                return null;
            }
        }

        BY_QuotationHeatBeoutDetailBll base_steelddperchasedetailbll = new BY_QuotationHeatBeoutDetailBll();
        public ActionResult GridPageListJson(string Condition, JqGridParam jqgridparam)
        {
            try
            {
                Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = base_steelddperchasedetailbll.GetOrderQueryList(QueryParameter, ref jqgridparam);
                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = ListData,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}