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

namespace DNS.WebApp.Areas.CommonModule.Controllers
{
    /// <summary>
    /// 员工表控制器
    /// </summary>
    public class WorkerController : PublicController<BY_Worker>
    {
        WorkerBll workerbll = new WorkerBll();
        public ActionResult GridPageListJson(string DepartId, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = workerbll.GetPageList(DepartId, ref jqgridparam);
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
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }
        public ActionResult GetUsers(string DepartId)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = workerbll.GetPageList(DepartId);
                return Content(ListData.ToJson());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult WorkerBarCode()
        {

            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult AllWorkerBarCode()
        {

            return View();
        }
        public ActionResult GetAutoCompleJson(string keywords, string DepartmentName=null)
        {
            List<BY_Worker> ListData = workerbll.GetAutoComple(keywords, DepartmentName);
            return Content(ListData.ToJson());
        }
    }
}