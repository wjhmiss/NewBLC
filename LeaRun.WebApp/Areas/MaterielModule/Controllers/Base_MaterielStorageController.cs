using DNS.Business;
using DNS.Entity;
using DNS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.MaterielModule.Controllers
{
    /// <summary>
    /// ¿â´æ¿ØÖÆÆ÷
    /// </summary>
    public class Base_MaterielStorageController : PublicController<Base_MaterielStorage>
    {
        public ActionResult GetEntity1(string MaterielCode)
        {
            Base_MaterielStorage ListData = repositoryfactory.Repository().FindEntity("MaterielCode", MaterielCode);
            return Content(ListData.ToJson());
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult Summary()
        {
            return View();
        }
        public ActionResult SummaryListJson(string Condition, JqGridParam jqgridparam)
        {
            try
            {
                Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = new Base_MaterielStorageBll().GetSummaryList(QueryParameter, ref jqgridparam);
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
        public ActionResult SummaryListJson1(string Condition, JqGridParam jqgridparam)
        {
            try
            {
                Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = new Base_MaterielStorageBll().GetSummaryList1(QueryParameter, ref jqgridparam);
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
        public ActionResult SummaryListJson2(string Condition, JqGridParam jqgridparam)
        {
            try
            {
                Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = new Base_MaterielStorageBll().GetSummaryList2(QueryParameter, ref jqgridparam);
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
        [LoginAuthorize]
        public virtual JsonResult GridPageJson1(string Condition, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                List<Base_MaterielStorage> ListData = new List<Base_MaterielStorage>();
                    Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
                    ListData = new Base_MaterielStorageBll().GetMaterielStorage(QueryParameter, ref jqgridparam);
                
                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = ListData,
                };
                return Json(JsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "Òì³£´íÎó£º" + ex.Message + "\r\nÌõ¼þ£º" + Condition);
                return null;
            }
        }
    }
}