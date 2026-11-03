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

namespace DNS.WebApp.Areas.OrderModule.Controllers
{
    /// <summary>
    /// 订单查询
    /// </summary>
    public class OrderQueryController : PublicController<BY_Order>
    {
        public ActionResult AutocompleteJson(string keywords)
        {
            List<BY_OrderDetail> by_orders = DataFactory.Database().FindListBySql<BY_OrderDetail>(@"  SELECT o.QuotationCode + '-' + cast(od.linecode as varchar(50)) as OrderCode
      ,od.LineCode
      ,od.Qty
      ,od.SteelId
      ,od.SteelCode
      ,od.SteelDescription
      ,od.Thick
      ,od.Weigh
      ,od.Length,od.Weight,od.ShipDescription
FROM dbo.BY_Order o, BY_OrderDetail od where o.orderid = od.orderid and  o.QuotationCode like '%" + keywords + "%'");


            BY_OrderDetail dd = new BY_OrderDetail();
            dd.OrderCode = "*";
            dd.Qty = 0;
            dd.SteelCode = "";
            dd.SteelId = "";
            dd.SteelDescription = "";
            dd.Weight = 0;
            dd.ShipDescription = "*";
            by_orders.Add(dd);
            return Content(by_orders.ToJson());
        }



        public  void UpdateStatus()
        {

            string dt = DNS.Utilities.DateTimeHelper.GetDate(-60) ;

            List<BY_Order> by_orders = DataFactory.Database().FindListBySql<BY_Order>(" select * FROM dbo.BY_Order  WHERE OrderStatus != '已发运' and CreateDate > '" + dt + "'");
            
            foreach (BY_Order by_order in by_orders)
            {
                SetOrderStatus(by_order.OrderCode);
            }
        }

        public ActionResult SetOrderStatus(string OrderCode)
        {
            if (!string.IsNullOrEmpty(OrderCode))
            {
                BY_Order by_order = DataFactory.Database().FindEntity<BY_Order>("OrderCode", OrderCode);

                List<BY_OrderDetail> by_orderdetails = DataFactory.Database().FindList<BY_OrderDetail>("OrderCode", by_order.OrderCode);
                if (by_orderdetails.Count == 0)
                {
                    by_order.OrderStatus = "已取消";
                    DataFactory.Database().Update(by_order);
                    return Content(null);
                }

                BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>("OrderCode", by_order.OrderCode);
                if (by_quotation != null && by_quotation.QuotationId != null)
                {
                    if (by_quotation.BillStatus == "合同取消")
                    {
                        by_order.OrderStatus = "已取消";
                        DataFactory.Database().Update(by_order);
                        foreach (BY_OrderDetail item in by_orderdetails)
                        {
                            item.BillStatus = "已取消";
                            DataFactory.Database().Update(item);
                        }
                        return Content(null);

                    }
                    if (by_order.OrderStatus == "已取消")
                    {
                        by_quotation.BillStatus = "合同取消";
                        DataFactory.Database().Update(by_quotation);
                        foreach (BY_OrderDetail item in by_orderdetails)
                        {
                            item.BillStatus = "已取消";
                            DataFactory.Database().Update(item);
                        }
                        return Content(null);

                    }
                }

                foreach (BY_OrderDetail item in by_orderdetails)
                {
                    if (item.SteelCode == "FY")
                    {
                        item.BillStatus = "已完工";
                        continue;
                    }
                    if (item.BillStatus != "已取消")
                    {
                        List<BY_DeliveryDetail> by_deliverydetails = DataFactory.Database().FindList<BY_DeliveryDetail>("OrderDetailId", item.OrderDetailId);
                        if (by_deliverydetails.Sum(p => p.Qty) == item.Qty)
                        {
                            item.BillStatus = "已发运";
                        }
                        else if (item.Qty == item.CompleteQty)
                        {
                            item.BillStatus = "已完工";
                        }
                        else if (by_deliverydetails.Sum(p => p.Qty) > 0)
                        {
                            item.BillStatus = "部分发运";
                        }
                        else if (item.CompleteQty > 0)
                        {
                            item.BillStatus = "部分完工";
                        }
                        DataFactory.Database().Update(item);
                    }
                }

                int YWG = by_orderdetails.Where(p => p.BillStatus == "已完工" && p.SteelCode != "FY").Count();
                int YFY = by_orderdetails.Where(p => p.BillStatus == "已发运" && p.SteelCode != "FY").Count();
                int BFWG = by_orderdetails.Where(p => p.BillStatus == "部分完工" && p.SteelCode != "FY").Count();
                int BFFY = by_orderdetails.Where(p => p.BillStatus == "部分发运" && p.SteelCode != "FY").Count();
                int YQX = by_orderdetails.Where(p => p.BillStatus == "已取消" && p.SteelCode != "FY").Count();
                int ZCSC = by_orderdetails.Where(p => p.BillStatus == "正常生产" && p.SteelCode != "FY").Count();
                int totals = by_orderdetails.Where(p => p.SteelCode != "FY").Count();
                if (YWG == totals)
                {
                    by_order.OrderStatus = "已完工";
                }
                else if (YFY == totals)
                {
                    by_order.OrderStatus = "已发运";
                }
                else if (BFWG == totals)
                {
                    by_order.OrderStatus = "部分完工";
                }
                else if (BFFY == totals)
                {
                    by_order.OrderStatus = "部分发运";
                }
                else if (YQX == totals)
                {
                    by_order.OrderStatus = "已取消";
                }
                else if (ZCSC == totals)
                {
                    by_order.OrderStatus = "正常生产";
                }
                else if (YQX > 0)
                {
                    by_order.OrderStatus = "部分取消";
                }
                else if (YFY > 0)
                {
                    by_order.OrderStatus = "部分发运";
                }
                else if (YWG > 0)
                {
                    by_order.OrderStatus = "部分完工";
                }
                else
                {
                    by_order.OrderStatus = "正常生产";
                }

                DataFactory.Database().Update(by_order);
            }
            return Content(null);
        }

        public void DownLoadExcel(string Condition)
        {
            Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
            DataTable ListData = by_orderbll.GetQuotDetailList(QueryParameter);
            //string[] DataColumn = { "下单日期", "助记码", "交期", "客户名称", "产品名称", "规格", "数量", "重量", "加工类型", "特殊工序", "公差备注" };
            string[] DataColumn = { "客户编码", "客户简称", "客户名称", "下单日期","交期", "整单", "行状态",  "助记码", "产品名称", "加工类型", "规格", "数量", "重量", "公差备注", "模号","送货地址","业务员" };
            string fileName = "明细汇总";
            DeriveExcel.DataTableToExcel(ListData, DataColumn, fileName);
        }
        public void DownLoadExcel1(string Condition)
        {
            Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
            QueryParameter.Add("WorkCostForWork", 3);
            DataTable ListData = by_orderbll.GetQuotDetailList(QueryParameter);
            string[] DataColumn = { "下单日期", "助记码", "交期", "客户名称", "产品名称", "规格", "数量", "重量", "加工类型", "特殊工序", "公差备注", "模号" };
            string fileName = "加工单汇总";
            DeriveExcel.DataTableToExcel(ListData, DataColumn, fileName);
        }
        BY_OrderBll by_orderbll = new BY_OrderBll();
        BY_OrderDetailBll by_orderdetailbll = new BY_OrderDetailBll();
        /// <summary>
        /// 订单查询列表（返回表格JSON）
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
                DataTable ListData = by_orderbll.GetOrderQueryList(QueryParameter, ref jqgridparam);
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
                return Content(ex.Message);
            }
        }

        public ActionResult GetGQOrder()
        {
            try
            {
                //报警物料
                int A = DataFactory.Database().FindCountBySql(@" SELECT SUM(1)
 FROM   BY_Order O
 WHERE   OrderStatus != '已取消' and o.DeleteMark=0  and  EXISTS (SELECT  1
FROM    dbo.BY_OrderDetail od
WHERE   (od.BillStatus = '正常生产'
        OR od.BillStatus = '部分完工'
        OR od.BillStatus = '部分发运')
        AND DATEADD(day, 1, o.Delivery) < SYSDATETIME()    AND od.OrderId=o.OrderId  )
            
 ");
                var JsonData = new
                {
                    A = A
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception)
            {
                return Content(null);
                throw;
            }
        }

        public  ActionResult UpdateOrderDetail(BY_OrderDetail entity)
        {
            try
            {
                int IsOk = 0;
                if (!string.IsNullOrEmpty(entity.OrderDetailId))
                {
                    entity.Modify(entity.OrderDetailId);
                    IsOk = DataFactory.Database().Update(entity);
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        /// <summary>
        /// 订单明细列表（返回表格JSON）
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public ActionResult GridOrderDetailListJson(string OrderId, JqGridParam jqgridparam)
        {
            if (!string.IsNullOrEmpty(OrderId))
            {
                DataTable ListData = by_orderdetailbll.Repository().FindTablePageBySql(@" SELECT  * ,
        ( SELECT TOP 1
                    '已' + TechnologyName
          FROM      BY_OrderDetailProcessDetail opd
          WHERE     opd.OrderDetailId = od.OrderDetailId
          ORDER BY  CreateDate DESC
        ) BillStatus1  --当前状态 
,
        ISNULL(( SELECT SUM(odd.Qty)
                 FROM   BY_OrderDetailProcessDetail odd
                 WHERE  odd.OrderDetailId = od.OrderDetailId
                        AND odd.LineCode = od.LineCode
                        AND (odd.TechnologyId = 'K001' or odd.TechnologyName = '锯床' or odd.TechnologyId = 'FG' or odd.TechnologyName = '风割' )
               ), 0) Oqty ,
        ISNULL(( SELECT SUM(odd.weight)
                 FROM   BY_OrderDetailProcessDetail odd
                 WHERE  odd.OrderDetailId = od.OrderDetailId
                        AND odd.LineCode = od.LineCode
                        AND (odd.TechnologyId = 'K001' or odd.TechnologyName = '锯床' or odd.TechnologyId = 'FG' or odd.TechnologyName = '风割' )
               ), 0) Oweight,
 ISNULL(( SELECT SUM(dd.Qty)
                 FROM   dbo.BY_DeliveryDetail dd
                 WHERE  dd.OrderDetailId = od.OrderDetailId
               ), 0) deliveryqty ,
(SELECT SUM(QTY) FROM Base_SteelDDBeoutDetail OB WHERE OB.QuotationDetailId = od.QuotationDetailId) BEOUTQTY
, (SELECT STUFF((SELECT '，'+bf.Status FROM base_file bf where bf.AttachId = od.QuotationDetailId for xml path('')),1,1,'') ) [Fang1]

FROM    BY_OrderDetail od
WHERE   od.OrderId = '" + OrderId + "' AND OD.Length > 0 ", ref jqgridparam);
                return Content(ListData.ToJson());
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 根据 订单号+行号 获取明细对象
        /// </summary>
        /// <param name="BarCode">条码</param>
        /// <returns></returns>
        public ActionResult GetOrderDetailEntityJson(string BarCode)
        {
            DataTable orderTable = by_orderbll.GetOrderDetailEntity(BarCode);
            if (orderTable==null||orderTable.Rows.Count<1)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：请输入正确的工单号+行号！" }.ToString());
            }
            return Content(orderTable.ToJson());
        }
        /// <summary>
        /// 根据 订单号+行号 获取明细对象过磅才能获取
        /// </summary>
        /// <param name="BarCode">条码</param>
        /// <returns></returns>
        public ActionResult GetOrderDetailEntityJsonforGB(string BarCode)
        {
            string[] arr = BarCode.Split('-');
            BY_Order by_order = by_orderbll.Repository().FindEntityByWhere(" AND OrderCode = '" + arr[0] + "-" + arr[1] + "'");
            if (by_order == null || by_order.OrderId == null)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：请输入正确的工单号+行号！" }.ToString());
            }
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>("QuotationId", by_order.QuotationId);
            //如果客来料
            if (by_quotation.CustMaterial == 1)
            {
                DataTable orderTable1 = by_orderbll.GetOrderDetailEntity(BarCode);
                if (orderTable1 == null || orderTable1.Rows.Count < 1)
                {
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：请输入正确的工单号+行号！" }.ToString());
                }
                return Content(orderTable1.ToJson());
            }
            DataTable orderTable = by_orderbll.GetOrderDetailEntityForGB(BarCode);
            if (orderTable == null || orderTable.Rows.Count < 1)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：此工单未消数！" }.ToString());
            }
            return Content(orderTable.ToJson());
        }
        /// <summary>
        /// 根据 订单号+行号 获取明细对象
        /// </summary>
        /// <param name="BarCode">条码</param>
        /// <returns></returns>
        public ActionResult GetOrderDetailScaleJson(string BarCode)
        {
            string[] arr = BarCode.Split('-');
            BY_Order by_order = by_orderbll.Repository().FindEntityByWhere(" AND OrderCode = '" + arr[0] + "-" + arr[1] + "'");
            if (by_order == null || by_order.OrderId == null)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：请输入正确的工单号+行号！" }.ToString());
            }
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>("QuotationId", by_order.QuotationId);
            //如果客来料
            if (by_quotation.CustMaterial == 1)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "客来料不用过磅！" }.ToString());
            }

            DataTable orderTable = by_orderbll.GetOrderDetailScaleJson(BarCode);
            if (orderTable.Rows.Count <1)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：工单号+行号错误，或此单已发运！" }.ToString());
            }
            return Content(orderTable.ToJson());
        }
    }
}
