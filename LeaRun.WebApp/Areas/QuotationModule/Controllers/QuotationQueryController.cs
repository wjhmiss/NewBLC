using DNS.Business;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.QuotationModule.Controllers
{
    /// <summary>
    /// 报价单查询
    /// </summary>
    public class QuotationQueryController : PublicController<BY_Quotation>
    {
        BY_QuotationBll by_quotationbll = new BY_QuotationBll();
        BY_QuotationDetailBll by_quotationdetailbll = new BY_QuotationDetailBll();

        /// <summary>
        /// 报价单查询列表（返回表格JSON）
        /// </summary>
        /// <param name="QuotationCode">报价单号</param>
        /// <param name="StartTime">入单时间 开始时间</param>
        /// <param name="EndTime">入单时间 结束</param>
        /// <param name="?"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public ActionResult GridPageListJson(string QuotationCode, string StartTime, string EndTime, string CustomerName, string MnemonicCode, string CustomerCode,string CustomerPO, string ProcessingAlgorithm, string FreeBill, string LType, JqGridParam jqgridparam)
        {
            Stopwatch watch = CommonHelper.TimerStart();
            DataTable ListData = by_quotationbll.GetPageList(QuotationCode, StartTime, EndTime, CustomerName, MnemonicCode, CustomerCode, CustomerPO, ProcessingAlgorithm,FreeBill,  LType, ref jqgridparam);
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

        public ActionResult GridPageListJson1(string Size, string CustomerName, string MnemonicCode, string SteelCode, string LType, JqGridParam jqgridparam)
        {
            Stopwatch watch = CommonHelper.TimerStart();
            DataTable ListData = by_quotationbll.GetPageList1(Size, CustomerName, MnemonicCode,SteelCode, LType, ref jqgridparam);
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
        public ActionResult GridPageListJson11(string Size, string CustomerName, string MnemonicCode, string FreeBill, JqGridParam jqgridparam)
        {
            Stopwatch watch = CommonHelper.TimerStart();
            DataTable ListData = by_quotationbll.GetPageList11(Size, CustomerName, MnemonicCode,FreeBill, ref jqgridparam);
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
        /// <summary>
        /// 报价单明细列表
        /// </summary>
        /// <param name="QuotationId"></param>
        /// <returns></returns>
        public ActionResult QuotationDetailListJson(string QuotationId, JqGridParam jqgridparam)
        {
            //if (!string.IsNullOrEmpty(QuotationId))
            //{
            //    List<BY_QuotationDetail> ListData = by_quotationdetailbll.Repository().FindListPage(" and QuotationId = '" + QuotationId + "' ", ref jqgridparam);
            //    return Content(ListData.ToJson());
            //}
            //else
            //{
            //    return null;
            //}
            List<DbParameter> parameter = new List<DbParameter>();
            if (!string.IsNullOrEmpty(QuotationId))
            {
                string sql = string.Format(@"SELECT  qd.*,os.BillCode,i.SteelPurchaseCode,i.BatchCode
FROM    BY_QuotationDetail qd
LEFT JOIN BY_OrderDetail od ON qd.QuotationDetailId = od.QuotationDetailId
LEFT JOIN (select OrderDetailId, [BillCode]=stuff((select ','+BillCode from BY_OutStock t where os.OrderDetailId=t.OrderDetailId for xml path('')), 1, 1, '') 
from BY_OutStock  os
group by OrderDetailId ) os ON od.OrderDetailId = os.OrderDetailId
LEFT JOIN BY_Inventory i ON qd.QuotationCode + '-'+cast(qd.LineCode AS varchar(50)) = i.QuotationCodeLine

where qd.QuotationId = '{0}'
", QuotationId);
                DataTable ListData = by_quotationbll.QuotationDetailListJson(sql, ref jqgridparam);

                return Content(ListData.ToJson());
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 报价单操作记录页面
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult QuotationOpearLog()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult QueryQuotation()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult QueryQuotationForSteelP()
        {
            return View();
        }
        /// <summary>
        /// 报价单操作记录列表（返回表格JSON）
        /// </summary>
        /// <param name="BillId"></param>
        /// <returns></returns>
        public ActionResult QuotationOpearLogListJson(string BillId)
        {
            List<BY_OpearLog> ListData = DataFactory.Database().FindList<BY_OpearLog>("BillId", BillId);
            return Content(ListData.ToJson());
        }
    }
}
