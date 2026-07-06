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

namespace DNS.WebApp.Areas.OrderModule.Controllers
{
    /// <summary>
    /// 产能表控制器
    /// </summary>
    public class CapacityController : PublicController<BY_Capacity>
    {
        BY_CapacityBll by_capacitybll = new BY_CapacityBll();
        BY_QuotationBll by_quotationbll = new BY_QuotationBll();
        /// <summary>
        /// 获取产能
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCapacityListJson()
        {
            List<BY_Capacity> list = by_capacitybll.GetList();
            return Content(list.ToJson());
        }
        /// <summary>
        /// 提交表单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ActionResult SubmitCapacityForm(BY_Capacity entity)
        {
            if (entity.CapacityType == "精加工")
            {
                entity.CapacityType = "BJ";
            }
            else if (entity.CapacityType == "粗加工")
            {
                entity.CapacityType = "BC";
            }
            int IsOk = by_capacitybll.SetCapacity(entity);
            string Message = "";
            if (IsOk > 0)
            {
                Message = "操作成功。";
            }
            else
            {
                Message = "操作失败。";
            }
            return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
        }
        /// <summary>
        /// 获取产能数量
        /// </summary>
        /// <param name="CapacityDate">日期</param>
        /// <param name="DayPart">时段</param>
        /// <returns></returns>
        public ActionResult GetCapacityValue(string CapacityDate, string DayPart, string QuotationId, string OrderType)
        {
            //string CapacityType = by_quotationbll.Repository().FindEntity(QuotationId).OrderType;
            BY_Capacity entity = by_capacitybll.Repository().FindEntityByWhere(string.Format(" AND CapacityDate = '{0}' AND DayPart = '{1}' AND  CapacityType='{2}'", CapacityDate, DayPart, OrderType));
            return Content(entity.ToJson());
        }

        #region 产能看板
        /// <summary>
        /// 产能看板页面
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult CapacityReport()
        {
            return View();
        }
        /// <summary>
        /// 产能看板数据列表 （返回JSON）
        /// </summary>
        /// <param name="StartTime">日期开始</param>
        /// <param name="EndTime">日期介绍</param>
        /// <returns></returns>
        public ActionResult CapacityReportListJson(string StartTime, string EndTime)
        {
            DataTable dt = by_capacitybll.GetCapacityReportList(StartTime, EndTime);
            return Content(dt.ToJson());
        }
        #endregion
    }
}