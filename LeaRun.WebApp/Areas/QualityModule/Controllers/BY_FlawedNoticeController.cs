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
    /// 不合格品通知单控制器
    /// </summary>
    public class BY_FlawedNoticeController : PublicController<BY_FlawedNotice>
    {

        [HttpPost]
        [ManagerPermission(PermissionMode.Enforce)]
        public   ActionResult Delete1(string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                var Message = "删除失败。";
                int IsOk = 0;

                BY_FlawedNotice by_flawednotice = DataFactory.Database().FindEntity<BY_FlawedNotice>(KeyValue);
                IsOk = repositoryfactory.Repository().Delete(by_flawednotice, isOpenTrans);
                if (IsOk > 0)
                {
                    Message = "删除成功。";
                }
                if (!string.IsNullOrEmpty(by_flawednotice.AddOrderCode))
                {
                    BY_OrderDetail by_orderdetail = DataFactory.Database().FindEntity<BY_OrderDetail>("OrderCode", by_flawednotice.AddOrderCode);
                    DataFactory.Database().Delete(by_orderdetail, isOpenTrans);
                }
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }


        public ActionResult FlawedNoticeListJson(string flawednoticeid)
        {
            try
            {
                BY_FlawedNotice Data = DataFactory.Database().FindEntity<BY_FlawedNotice>(flawednoticeid);
                BY_OrderDetail BY_OrderDetail = DataFactory.Database().FindEntity<BY_OrderDetail>(Data.OrderDetailId);
                BY_Order BY_Order = DataFactory.Database().FindEntity<BY_Order>(BY_OrderDetail.OrderId);
                var JsonData = new
                {
                    BY_FlawedNotice = Data,
                    BY_OrderDetail = BY_OrderDetail,
                    BY_Order = BY_Order,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception)
            {
                return null;
            }
        }


        [ManagerPermission(PermissionMode.Enforce)]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public  ActionResult Print()
        {
            return View();
        }
        [ManagerPermission(PermissionMode.Enforce)]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult Print2()
        {
            return View();
        }
        BY_FlawedNoticeBll by_flawednoticebll = new BY_FlawedNoticeBll();
        public ActionResult FlawedNoticeInput()
        {
            string _ModuleId = DESEncrypt.Encrypt("477a2b75-3dce-4b0f-97a4-0ec7d561b228");
            CookieHelper.WriteCookie("ModuleId", _ModuleId);
            return View();
        }

        public ActionResult SubmitFlawedForm(BY_FlawedNotice BY_FlawedNotice, string KeyValue)
        {
            Base_CodeRuleBll base_coderulebll = new Base_CodeRuleBll();
            string OrderCode = "";
            string BillCode = "";
            if (BY_FlawedNotice.AddOrderId == "1")
            {
                OrderCode = base_coderulebll.GetBillCode(ManageProvider.Provider.Current().UserId, "B");
            }
            if (string.IsNullOrEmpty(KeyValue))
            {
                BillCode = base_coderulebll.GetBillCode(ManageProvider.Provider.Current().UserId, "IPQC");
            }
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                string[] billStr = BY_FlawedNotice.OrderBarcode.Split('-');
                //生成补料单
                //if (BY_FlawedNotice.AddOrderId == "1")
                if (false)
                {
                    BY_Order by_ordered = database.FindEntity<BY_Order>("Remark", "补料" + BY_FlawedNotice.OrderBarcode);
                    if (by_ordered != null && by_ordered.OrderId != null)
                    {
                        
                    }
                    else
                    {
                        BY_Order by_order = database.FindEntity<BY_Order>("OrderCode", billStr[0] + "-" + billStr[1]);
                        by_order.Qty = BY_FlawedNotice.Qty;
                        by_order.Amount = 0;
                        by_order.OrderStatus = "补料";
                        by_order.OrderCode = OrderCode;
                        //by_order.PrintDate = "";
                        by_order.PrintUserId = "";
                        by_order.Remark = "补料" + BY_FlawedNotice.OrderBarcode;
                        by_order.Create();
                        BY_OrderDetail by_orderdetail = new BY_OrderDetail();
                        //复制不合格订单
                        by_orderdetail = database.FindEntityBySql<BY_OrderDetail>(string.Format(@"SELECT * FROM BY_OrderDetail
                                                                                            WHERE   OrderId = ( SELECT  TOP 1
                                                                                                                        OrderId
                                                                                                                FROM    BY_Order
                                                                                                                WHERE   OrderCode ='{0}' and DeleteMark=0 
                                                                                                                )
                                                                                                    AND LineCode = {1}", billStr[0] + "-" + billStr[1], billStr[2]));
                        List<BY_OrderDetailProcess> BY_OrderDetailProcesss = DataFactory.Database().FindList<BY_OrderDetailProcess>("OrderDetailId", by_orderdetail.OrderDetailId);

                        by_order.OrderType = by_orderdetail.OrderCode.Substring(0, 1);//"BA";
                        if (by_order.OrderType == "B")
                        {
                            DataFactory.Database().Rollback();
                            return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：补料单不允许生成补料单" }.ToString());
                        }
                        by_orderdetail.OrderId = by_order.OrderId;
                        by_orderdetail.BillStatus = "补料";
                        by_orderdetail.OrderCode = OrderCode;
                        by_orderdetail.LineCode = 1;
                        by_orderdetail.Weight = by_orderdetail.Weight * BY_FlawedNotice.Qty / by_orderdetail.Qty;
                        by_orderdetail.OpenQty = 0;
                        by_orderdetail.CompleteQty = 0;
                        //by_orderdetail.QuotationDetailId = "";
                        //by_orderdetail.ScaleDate = "";
                        by_orderdetail.ScaleUser = "";
                        by_orderdetail.ScaleProcess = "";
                        by_orderdetail.Qty = BY_FlawedNotice.Qty;
                        by_orderdetail.Create();
                        database.Insert<BY_Order>(by_order, isOpenTrans);
                        database.Insert<BY_OrderDetail>(by_orderdetail, isOpenTrans);
                        base_coderulebll.OccupyBillCode(ManageProvider.Provider.Current().UserId, "B", isOpenTrans);
                        BY_FlawedNotice.AddOrderCode = by_order.OrderCode;

                        foreach (BY_OrderDetailProcess item in BY_OrderDetailProcesss)
                        {
                            item.OrderCode = OrderCode;
                            item.OrderDetailId = by_orderdetail.OrderDetailId;
                            item.LineCode = 1;
                            item.Qty = by_orderdetail.Qty;
                            item.Create();
                            DataFactory.Database().Insert<BY_OrderDetailProcess>(item, isOpenTrans);
                        }
                    }

                }
                else
                {
                    BY_Order by_ordered = database.FindEntity<BY_Order>("Remark", "补料" + BY_FlawedNotice.OrderBarcode);
                    if (by_ordered!= null && by_ordered.OrderId != null)
                    {
                        BY_OrderDetail by_orderdetail = DataFactory.Database().FindEntity<BY_OrderDetail>("OrderId", by_ordered.OrderId);
                        
                        List<BY_OrderDetailProcess> BY_OrderDetailProcesss = DataFactory.Database().FindList<BY_OrderDetailProcess>("OrderDetailId", by_orderdetail.OrderDetailId);

                        foreach (BY_OrderDetailProcess item in BY_OrderDetailProcesss)
                        {
                            DataFactory.Database().Delete<BY_OrderDetailProcess>(item, isOpenTrans);
                        }
                        DataFactory.Database().Delete<BY_Order>(by_ordered, isOpenTrans);
                        DataFactory.Database().Delete<BY_OrderDetail>(by_orderdetail, isOpenTrans);
                    }
                    BY_FlawedNotice.AddOrderCode = "";
                }


                if (string.IsNullOrEmpty(KeyValue))
                {
                    BY_FlawedNotice.Create();
                    BY_FlawedNotice.BillCode = BillCode;
                    base_coderulebll.OccupyBillCode(ManageProvider.Provider.Current().UserId, "IPQC", isOpenTrans);
                    database.Insert<BY_FlawedNotice>(BY_FlawedNotice, isOpenTrans);
                }
                else
                {
                    BY_FlawedNotice.FlawedNoticeId = KeyValue;
                    BY_FlawedNotice.Modify(KeyValue);
                    database.Update<BY_FlawedNotice>(BY_FlawedNotice, isOpenTrans);
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

        /// <summary>
        /// 不合格品列表
        /// </summary>
        /// <param name="StartTime">起始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="BillCode">不合格品单号</param>
        /// <param name="SteelCode">钢种</param>
        /// <param name="OrderCode">加工单号</param>
        /// <param name="CustomerName">客户名称</param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public ActionResult GridPageListJson(string StartTime, string EndTime, string BillCode, string SteelCode, string OrderCode, string CustomerName, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = by_flawednoticebll.GetPageList(StartTime, EndTime, BillCode, SteelCode, OrderCode, CustomerName, ref jqgridparam);
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

    }
}