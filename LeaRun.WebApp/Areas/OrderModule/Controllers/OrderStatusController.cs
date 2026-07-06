using DNS.Business;
using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.OrderModule.Controllers
{
    /// <summary>
    /// 订单状态控制器
    /// </summary>
    public class OrderStatusController : PublicController<BY_Order>
    {
        public ActionResult UpdateStatus(string OrderCode)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                BY_Order by_order = DataFactory.Database().FindEntity<BY_Order>("OrderCode", OrderCode);
                List<BY_OrderDetail> by_orderdetails = DataFactory.Database().FindList<BY_OrderDetail>("OrderCode", OrderCode);
                foreach (BY_OrderDetail item in by_orderdetails)
                {
                    if (item.CompleteQty == item.Qty && item.BillStatus != "已发运" && item.BillStatus != "已完工")
                    {
                        BY_Delivery by_delivery = DataFactory.Database().FindEntity<BY_Delivery>("OrderCode", OrderCode);
                        if (by_delivery != null && by_delivery.DeliveryId != null)
                        {
                            item.BillStatus = "已发运";
                        }
                        else
                        {
                            item.BillStatus = "已完工";
                        }
                    }
                }

                if (by_orderdetails.Where(p => p.BillStatus == "已发运").Count() == by_orderdetails.Count)
                {
                    by_order.OrderStatus = "已发运";
                }
                else if (by_orderdetails.Where(p => p.BillStatus == "已完工").Count() == by_orderdetails.Count)
                {
                    by_order.OrderStatus = "已完工";
                }
                else if (by_orderdetails.Where(p => p.BillStatus == "已发运").Count() > 0)
                {
                    by_order.OrderStatus = "部分发运";
                }

                DataFactory.Database().Update(by_order, isOpenTrans);
                DataFactory.Database().Update(by_orderdetails, isOpenTrans);

                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "打印状态更改成功。" }.ToString());
            }
            catch (Exception)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = true, Code = "-1", Message = "打印状态更改失败。" }.ToString());
            }
        }
        BY_OrderBll by_orderbll = new BY_OrderBll();
        /// <summary>
        /// 订单列表（返回表格JSON）
        /// </summary>
        /// <param name="Condition">查询条件</param>
        /// <param name="jqgridparam">分页参数</param>
        /// <returns></returns>
        public ActionResult GridPageListJson(string Condition, JqGridParam jqgridparam)
        {
            try
            {
                Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
                Stopwatch watch = CommonHelper.TimerStart();
                List<BY_Order> ListData = by_orderbll.GetOrderStatusList(QueryParameter, ref jqgridparam);
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
        /// <summary>
        /// 整单变更状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ActionResult SubmitUpdata(BY_Order entity)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {

                database.Update(entity, isOpenTrans);

                Hashtable by_orderdetail = new Hashtable();
                by_orderdetail["OrderId"] = entity.OrderId;
                by_orderdetail["BillStatus"] = entity.OrderStatus;
                database.Update("by_orderdetail", by_orderdetail, "OrderId", isOpenTrans);

                BY_OpearLog by_opearlog = new BY_OpearLog();
                by_opearlog.Create();
                by_opearlog.BillId = entity.OrderId;
                by_opearlog.OpearType = entity.OrderStatus;
                database.Insert(by_opearlog, isOpenTrans);

                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "变更状态成功。" }.ToString());
            }
            catch (Exception)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "变更状态失败。" }.ToString());
            }
        }
        /// <summary>
        /// 修改行数量
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult UpdataNumber()
        {
            return View();
        }
        /// <summary>
        /// 提交修改行数量
        /// </summary>
        /// <returns></returns>
        public ActionResult SubmitUpdataNumber(BY_OrderDetail entity, decimal OQty, decimal Weight)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                entity.Weight = Weight * entity.Qty / OQty;
                //修改订单明细
                database.Update(entity, isOpenTrans);

                BY_OpearLog by_opearlog = new BY_OpearLog();
                by_opearlog.Create();
                by_opearlog.BillId = entity.OrderId;
                by_opearlog.OpearType = "修改数量";
                by_opearlog.Remark = entity.Qty.ToString();
                database.Insert(by_opearlog, isOpenTrans);

                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "修改数量成功。" }.ToString());
            }
            catch (Exception)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "修改数量失败。" }.ToString());
            }
        }
        /// <summary>
        /// 修改明细状态
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult OrderDetailStatus()
        {
            return View();
        }
        /// <summary>
        /// 提交修改行状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ActionResult SubmitOrderDetail(BY_OrderDetail entity)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                //修改订单状态
                BY_Order by_order = new BY_Order();
                by_order.OrderId = entity.OrderId;
                by_order.OrderStatus = by_orderbll.ReturnOrderStatus(entity.OrderId, entity.OrderDetailId, entity.BillStatus);
                database.Update(by_order, isOpenTrans);
                //修改行状态
                database.Update(entity, isOpenTrans);

                BY_OpearLog by_opearlog = new BY_OpearLog();
                by_opearlog.Create();
                by_opearlog.BillId = entity.OrderId;
                by_opearlog.OpearType = entity.BillStatus;
                database.Insert(by_opearlog, isOpenTrans);

                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "变更行状态成功。" }.ToString());
            }
            catch (Exception)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "变更行状态失败。" }.ToString());
            }
        }
    }
}
