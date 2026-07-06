using DNS.Business.Utility.Enum;
using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.QuotationModule.Controllers
{
    /// <summary>
    /// 报价单管理
    /// </summary>
    public class QuotationManageController : PublicController<BY_Quotation>
    {
        /// <summary>
        /// 暂挂/取消暂挂
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <param name="Mode">方式：1：暂挂、2：取消暂挂</param>
        /// <returns></returns>
        public ActionResult SubmitSuspendMode(string KeyValue, int Mode)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                BY_Quotation entity = new BY_Quotation();
                entity.QuotationId = KeyValue;
                if (Mode == 1)
                {
                    entity.PauseDate = DateTime.Now;
                    entity.PauseUserId = ManageProvider.Provider.Current().UserName;
                }
                else
                {
                    entity.PauseDate = CommonHelper.GetDateTime("1900-01-01");
                    entity.PauseUserId = "";
                }
                database.Update(entity, isOpenTrans);

                BY_OpearLog by_opearlog = new BY_OpearLog();
                by_opearlog.Create();
                by_opearlog.BillId = KeyValue;
                if (Mode==1)
                {
                    by_opearlog.OpearType = QuotationBillStatus.暂挂.ToString();
                }
                else
                {
                    by_opearlog.OpearType = QuotationBillStatus.取消暂挂.ToString();
                }
                database.Insert(by_opearlog, isOpenTrans);


                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功。" }.ToString());
            }
            catch (Exception)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败。" }.ToString());
            }
        }
        /// <summary>
        /// 关闭单据
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult CloseBills()
        {
            return View();
        }
        /// <summary>
        /// 提交关闭单据
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult SubmitCloseBills(string KeyValue, string CancleReason)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                BY_Quotation entity = new BY_Quotation();
                entity.QuotationId = KeyValue;
                entity.BillStatus = QuotationBillStatus.合同取消.ToString();
                entity.CancleReason = CancleReason;
                entity.CancleDate = DateTime.Now;
                entity.CancleUserId = ManageProvider.Provider.Current().UserName;
                database.Update(entity, isOpenTrans);

                BY_OpearLog by_opearlog = new BY_OpearLog();
                by_opearlog.Create();
                by_opearlog.BillId = KeyValue;
                by_opearlog.OpearType = entity.BillStatus;
                database.Insert(by_opearlog, isOpenTrans);

                if (!string.IsNullOrEmpty(entity.OrderCode))
                {
                    BY_Order by_order = DataFactory.Database().FindEntity<BY_Order>("OrderCode", entity.OrderCode);
                    if (by_order != null && by_order.OrderId != null)
                    {
                        if (by_order.OrderStatus == "已完工" || by_order.OrderStatus == "部分完工" || by_order.OrderStatus == "已发运" || by_order.OrderStatus == "部分发运")
                        {
                            database.Rollback();
                            return Content(new JsonMessage { Success = false, Code = "-1", Message = "取消失败:订单状态" + by_order.OrderStatus + ",请去订单取消！" }.ToString());
                        }
                        by_order.OrderStatus = "已取消";
                        database.Update(by_order, isOpenTrans);
                        List<BY_OrderDetail> by_orderdetails = DataFactory.Database().FindList<BY_OrderDetail>("OrderCode", entity.OrderCode);
                        foreach (BY_OrderDetail item in by_orderdetails)
                        {
                            item.BillStatus = "已取消";
                            item.Modify(item.OrderDetailId);
                            database.Update(item, isOpenTrans);
                        }
                    }
                }

                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "合同取消成功。" }.ToString());
            }
            catch (Exception)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "合同取消失败。" }.ToString());
            }
        }
    }
}
