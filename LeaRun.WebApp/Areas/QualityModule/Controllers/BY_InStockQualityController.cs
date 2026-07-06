using DNS.Business;
using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.QualityModule.Controllers
{
    /// <summary>
    /// 原材料入仓品质信息控制器
    /// </summary>
    public class BY_InStockQualityController : PublicController<BY_InStockQuality>
    {
        BY_InStockQualityBll by_instockqualitybll = new BY_InStockQualityBll();
        public ActionResult InStockQualityInput()
        {
            string _ModuleId = DESEncrypt.Encrypt("1e5c744a-b373-4124-99d0-dd5f1ff0cf6f");
            CookieHelper.WriteCookie("ModuleId", _ModuleId);
            return View();
        }
        /// <summary>
        /// 原材料入仓品质列表
        /// </summary>
        /// <param name="StartTime">起始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="BillCode">不合格品单号</param>
        /// <param name="SteelCode">钢种</param>
        /// <param name="OrderCode">加工单号</param>
        /// <param name="CustomerName">客户名称</param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public ActionResult GridPageListJson(string StartTime, string EndTime, string BillCode, string SteelCode, string InStockQualityCode, string CreateUserName, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = by_instockqualitybll.GetPageList(StartTime, EndTime, BillCode, SteelCode, InStockQualityCode, CreateUserName, ref jqgridparam);
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

        public ActionResult SubmitQualityForm(BY_InStockQuality BY_InStockQuality, string KeyValue)
        {
            Base_CodeRuleBll base_coderulebll = new Base_CodeRuleBll();
            string InStockQualityCode = "";
            if (string.IsNullOrEmpty(KeyValue))
            {
               InStockQualityCode = base_coderulebll.GetBillCode(ManageProvider.Provider.Current().UserId, "IQC-1");
            }
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                if (string.IsNullOrEmpty(KeyValue))
                {
                    BY_InStockQuality.Create();
                    BY_InStockQuality.InStockQualityCode = InStockQualityCode;
                    base_coderulebll.OccupyBillCode(ManageProvider.Provider.Current().UserId, "IQC-1", isOpenTrans);
                    database.Insert<BY_InStockQuality>(BY_InStockQuality, isOpenTrans);
                }
                else
                {
                    BY_InStockQuality.InStockQualityId = KeyValue;
                    BY_InStockQuality.Modify(KeyValue);
                    database.Update<BY_InStockQuality>(BY_InStockQuality, isOpenTrans);
                }
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

    }
}