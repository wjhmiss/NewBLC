using DNS.Business;
using DNS.Business.Utility.Enum;
using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using DNS.WebApp.Areas.CheckSheetModule.Controllers;
using DNS.WebApp.Areas.OrderModule.Controllers;
using DNS.WebApp.Areas.QuotationModule.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.DeliveryModule.Controllers
{
    public class InventoryComfirmController : PublicController<BY_Delivery>
    {
        public virtual ActionResult DeteletComplete(string Orderid, string ordercompleteids)
        {
            string[] idcs = ordercompleteids.Split(',');
            string ids = "";
            for (int i = 0; i < idcs.Length; i++)
            {
                ids = ids + "'" + idcs[i] + "',";
            }
            ids = ids.TrimEnd(',');
            BY_Order by_order = DataFactory.Database().FindEntity<BY_Order>(Orderid);
            List<BY_OrderComplete> by_ordercompletes = DataFactory.Database().FindList<BY_OrderComplete>(string.Format(" and (DeliveryCode is null or DeliveryCode = '') and OrderCompleteId in ({0})", ids));
            if (by_ordercompletes.Count == 0 || by_order.OrderId == null)
            {
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作失败：已发运，请回退送货单后删除或使用盘点单！" }.ToString());
            }
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                foreach (BY_OrderComplete by_ordercomplete in by_ordercompletes)
                {
                    DataFactory.Database().Delete(by_ordercomplete, isOpenTrans);
                    BY_OrderDetail by_orderdetail = DataFactory.Database().FindEntity<BY_OrderDetail>(by_ordercomplete.OrderDetailId);
                    by_orderdetail.CompleteQty = by_orderdetail.CompleteQty - by_ordercomplete.Qty;

                    if (by_orderdetail.CompleteQty == 0)
                    {
                        by_orderdetail.BillStatus = "正常生产";
                    }
                    if (by_orderdetail.CompleteQty != 0)
                    {
                        by_orderdetail.BillStatus = "部分完工";
                    }

                    if (by_orderdetail.Length == 0)
                    {
                        BY_QuotationDetail BY_QuotationDetail = DataFactory.Database().FindEntity<BY_QuotationDetail>(by_orderdetail.QuotationDetailId);
                        DataFactory.Database().Delete(BY_QuotationDetail, isOpenTrans);

                        BY_Quotation BY_Quotation = DataFactory.Database().FindEntity<BY_Quotation>(BY_QuotationDetail.QuotationId);
                        BY_Quotation.TotalAmount = BY_Quotation.TotalAmount - BY_QuotationDetail.SalesAmount;
                        DataFactory.Database().Update(BY_Quotation, isOpenTrans);
                    }
                    IList<BY_OrderDetailProcessDetail> by_orderdetailprocessdetails = database.FindListBySql<BY_OrderDetailProcessDetail>(@"SELECT  *
FROM    dbo.BY_OrderDetailProcessDetail odp
WHERE   (odp.TechnologyId = 'K001' or odp.TechnologyName = '锯床' or odp.TechnologyId = 'FG' or odp.TechnologyName = '风割' )
        AND odp.isclear = 0 and OrderDetailId='" + by_orderdetail.OrderDetailId+"' ");
                    foreach (BY_OrderDetailProcessDetail item in by_orderdetailprocessdetails)
                    {
                        item.IsClear = 0;
                        database.Update<BY_OrderDetailProcessDetail>(item, isOpenTrans);
                    }

                    if (by_orderdetail.Length == 0)
                    {
                        DataFactory.Database().Delete(by_orderdetail, isOpenTrans);
                        
                    }
                    else
                    {
                        DataFactory.Database().Update(by_orderdetail, isOpenTrans);

                    }
                }
                by_order.OrderStatus = "部分完工";
                DataFactory.Database().Update(by_order, isOpenTrans);

                DataFactory.Database().Commit();
                new OrderQueryController().SetOrderStatus(by_order.OrderCode);
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功！" }.ToString());
            }

            catch (Exception ex)
            {
                DataFactory.Database().Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        Base_CodeRuleBll base_coderulebll = new Base_CodeRuleBll();
        //
        // GET: /DeliveryModule/InventoryComfirm/
        private string BillCode(string ModuleId)
        {
            string UserId = ManageProvider.Provider.Current().UserId;
            return base_coderulebll.GetBillCode(UserId, ModuleId);
        }

        

        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public override ActionResult Index()
        {
            return View();
        }

        //
        // GET: /DeliveryModule/InventoryComfirm/Details/5
        [OutputCache(CacheProfile = "CacheForParam")]
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /DeliveryModule/InventoryComfirm/Create
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /DeliveryModule/InventoryComfirm/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /DeliveryModule/InventoryComfirm/Edit/5
        [OutputCache(CacheProfile = "CacheForParam")]
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /DeliveryModule/InventoryComfirm/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /DeliveryModule/InventoryComfirm/Delete/5
        [OutputCache(CacheProfile = "CacheForParam")]
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /DeliveryModule/InventoryComfirm/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        

        /// <summary>
        /// 绑定表格
        /// </summary>
        /// <param name="ParameterJson">查询条件</param>
        /// <param name="Gridpage">分页条件</param>
        /// <returns></returns>
        [LoginAuthorize]
        [ValidateInput(false)]
        public ActionResult OrderGridPageJson(string Condition, JqGridParam jqgridparam)
        {

            BY_OrderBll by_orderbll = new Business.BY_OrderBll();
            Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
            DataTable by_orderdetail = by_orderbll.GetNoCompleteOrders(QueryParameter,ref jqgridparam);
            Stopwatch watch = CommonHelper.TimerStart();
            var JsonData = new
            {
                total = jqgridparam.total,
                page = jqgridparam.page,
                records = jqgridparam.records,
                costtime = CommonHelper.TimerEnd(watch),
                rows = by_orderdetail,
            };
            return Content(JsonData.ToJson());

        }
        /// <summary>
        /// 获取完成但没有发运的订单详情
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        [LoginAuthorize]
        public  ActionResult DeliveryGridPageJson(string OrderId)
        {
            BY_OrderBll by_orderbll = new Business.BY_OrderBll();
            DataTable by_orderdetail = by_orderbll.GetNoCompleteOrdersDetails(OrderId);
            Stopwatch watch = CommonHelper.TimerStart(); 
            var JsonData = new
            {
                //total = jqgridparam.total,
                //page = jqgridparam.page,
                //records = jqgridparam.records,
                //costtime = CommonHelper.TimerEnd(watch),
                rows = by_orderdetail,
            };
            return Content(JsonData.ToJson());
        }
        /// <summary>
        /// 获取没有完成的订单详情
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        [LoginAuthorize]
        public  ActionResult NoCompleteGridPageJson(string OrderId, JqGridParam jqgridparam)
        {
            BY_OrderBll by_orderbll = new Business.BY_OrderBll();
            DataTable by_orderdetail = by_orderbll.GetCompleteOrdersDetails(OrderId);
            Stopwatch watch = CommonHelper.TimerStart();
            var JsonData = new
            {
                //total = jqgridparam.total,
                //page = jqgridparam.page,
                //records = jqgridparam.records,
                //costtime = CommonHelper.TimerEnd(watch),
                rows = by_orderdetail,
            };
            return Content(JsonData.ToJson());
        }

        private string GetDeliverCode(string Quocode,string OrderCode,int? CompeletqtyAll)
        {
            List<BY_OrderDetail> by_orderdetails = DataFactory.Database().FindList<BY_OrderDetail>("OrderCode", OrderCode);

            if (by_orderdetails.Sum(p=>p.Qty) == CompeletqtyAll)
            {
                return Quocode;
            }

            List<BY_Delivery> by_deliverys = DataFactory.Database().FindListBySql<BY_Delivery>(" select * from BY_Delivery where DeliveryCode like '" + Quocode + "%'   and DeleteMark = 0 ");
            BY_Delivery by_delivery = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryCode", Quocode + "-" + (by_deliverys.Count + 1));
            if (by_delivery == null || by_delivery.DeliveryId == null)
            {
                return Quocode + "-" + (by_deliverys.Count + 1);
            }
            else
            {
                BY_Delivery by_delivery2 = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryCode", Quocode + "-" + (by_deliverys.Count + 2));
                if (by_delivery2 == null || by_delivery2.DeliveryId == null)
                {
                    return Quocode + "-" + (by_deliverys.Count + 2);
                }
                else
                {
                    return Quocode + "-" + (by_deliverys.Count + 3);
                }
            }
        }


        /// <summary>
        /// 生成发运单
        /// </summary>
        /// <param name="Orderid"></param>
        /// <returns></returns>
        [LoginAuthorize]
        public ActionResult SureConfirm(string Orderid, string ordercompleteids)
        {
            
            

            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                
                //订单
                BY_Order by_order = database.FindEntity<BY_Order>(Orderid);
                Base_Customer base_customer = database.FindEntity<Base_Customer>(by_order.CustomerId);
                //订单明细
                List<BY_OrderDetail> by_orderdetails = database.FindList<BY_OrderDetail>("OrderId", Orderid);
                //报价单
                BY_Quotation by_quotation = database.FindEntity<BY_Quotation>(by_order.QuotationId);
                //报价单明细
                //List<BY_QuotationDetail> by_quotationdetailsQGorDK = database.FindList<BY_QuotationDetail>(string.Format(" AND QuotationId = '{0}' and (SteelCode='DK' or SteelCode='QG' )", by_order.QuotationId));
                //如果订单明细存在已取消或部分取消，则不修改状态。订单状态即为部分取消或已取消
                //var isCancelorderdetail = (from entity in by_orderdetails
                //                           where entity.BillStatus == "已取消" || entity.BillStatus == "已暂停"
                //                           select entity).ToList();
                
                //1，更新订单状态
                string[] idcs = ordercompleteids.Split(',');
                string ids = "";
                for (int i = 0; i < idcs.Length; i++)
                {
                    ids = ids + "'" + idcs[i] + "',";
                }
                ids = ids.TrimEnd(',');
                List<BY_OrderComplete> by_ordercompletesOk = database.FindList<BY_OrderComplete>(string.Format(" and DeliveryCode is not null and DeliveryCode != '' and Orderid ='{0}'", Orderid));
                List<BY_OrderComplete> by_ordercompletes = database.FindList<BY_OrderComplete>(string.Format(" and (DeliveryCode is null or DeliveryCode = '') and OrderCompleteId in ({0})", ids));
                if (by_ordercompletesOk.Sum<BY_OrderComplete>(p => p.Qty) + by_ordercompletes.Sum<BY_OrderComplete>(p => p.Qty) >= by_order.Qty)
                {
                    by_order.OrderStatus = "已发运";
                }
                else
                {
                    by_order.OrderStatus = "部分发运";
                }
                by_order.Modify(by_order.OrderId);
                database.Update(by_order, isOpenTrans);
                //2，新增发运单
                string DeliveryCode = GetDeliverCode(by_order.QuotationCode, by_order.OrderCode, by_ordercompletes.Sum(P => P.Qty));
                if (string.IsNullOrWhiteSpace(DeliveryCode))
                {
                    database.Rollback();
                    return Content(new JsonMessage { Success = true, Code = "-1", Message = "获取发运单号失败。" }.ToString());
                }
                BY_Delivery by_delivery = new Entity.BY_Delivery();
                Base_CodeRuleBll base_coderulebll = new Base_CodeRuleBll();
                //string ModuleId = DESEncrypt.Decrypt(CookieHelper.GetCookie("ModuleId"));
                by_delivery.DeliveryCode = DeliveryCode;
                by_delivery.QuotationCode = by_quotation.QuotationCode;
                by_delivery.LType = by_quotation.LType;
                by_delivery.CompanyName = by_quotation.CompanyName;
                by_delivery.IsReturn = by_quotation.IsReturn;
                

        by_delivery.Create();

                //3，新增发运单详情
                
                List<BY_DeliveryDetail> by_deliverydetails = new List<BY_DeliveryDetail>();
                BY_OrderComplete by_ordercomplete;
                BY_DeliveryDetail by_deliverydetail;
                BY_QuotationDetail by_quotationdetail;


                foreach (BY_OrderDetail item in by_orderdetails)
                {
                    by_ordercomplete = (from entity in by_ordercompletes
                                       where entity.OrderDetailId == item.OrderDetailId
                                        select entity).FirstOrDefault<BY_OrderComplete>();
                    by_quotationdetail = database.FindEntity<BY_QuotationDetail>(item.QuotationDetailId);
                    if (by_ordercomplete == null)
                    {
                        continue;
                    }
                    //if (by_quotationdetail.QuotationDetailId == null)
                    //{
                    //    return Content(new JsonMessage { Success = true, Code = "-1", Message = "获取报价单详情失败。" }.ToString());
                    //}
                    //新增发运单详情
                    by_deliverydetail = new BY_DeliveryDetail();
                    if (item.SteelCode == "FY")
                    {
                        CommonHelper.CopySameFieldsObjectALL<BY_QuotationDetail, BY_DeliveryDetail>(by_quotationdetail, ref by_deliverydetail);
                        by_deliverydetail.DeliveryCode = by_delivery.DeliveryCode;
                        by_deliverydetail.DeliveryId = by_delivery.DeliveryId;
                        by_deliverydetail.Create();
                    }
                    else
                    {
                        by_deliverydetail.Create();
                        by_deliverydetail.DeliveryId = by_delivery.DeliveryId;
                        by_deliverydetail.LineCode = item.LineCode;
                        by_deliverydetail.BoardId = item.BoardId;
                        by_deliverydetail.Qty = by_ordercomplete.Qty;
                        by_deliverydetail.ProjectCode = by_quotationdetail.ProjectCode;
                        by_deliverydetail.IsGrindAngle = item.IsGrindAngle;
                        by_deliverydetail.IsChamfer = item.IsChamfer;
                        by_deliverydetail.SteelId = item.SteelId;
                        by_deliverydetail.SteelCode = item.SteelCode;
                        by_deliverydetail.SteelDescription = item.SteelDescription;
                        by_deliverydetail.SteelOtherDescription = item.SteelOtherDescription;
                        by_deliverydetail.SteelState = item.SteelState;
                        by_deliverydetail.DieCode = item.DieCode;
                        by_deliverydetail.Thick = item.Thick;
                        by_deliverydetail.Weigh = item.Weigh;
                        by_deliverydetail.Length = item.Length;
                        //实际重量
                        //材料
                        by_deliverydetail.Weight = by_ordercomplete.Weight;
                        by_deliverydetail.TheoryWeight = by_quotationdetail.TheoryWeight * by_ordercomplete.Qty / (item.Qty == 0 ? 1 : item.Qty);
                        by_deliverydetail.WorkWeight = by_quotationdetail.WorkWeight * by_ordercomplete.Qty / (item.Qty == 0 ? 1 : item.Qty);
                        by_deliverydetail.JJtype = by_quotationdetail.JJtype;

                        by_deliverydetail.MaterialPrice = by_quotationdetail.MaterialPrice;
                        by_deliverydetail.MaterialLess = by_quotationdetail.MaterialLess;
                        if (by_ordercomplete.Weight == by_quotationdetail.Weight)
                        {
                            by_deliverydetail.MaterialAmount = Math.Round(Convert.ToDecimal(by_quotationdetail.MaterialAmount * by_ordercomplete.Qty / by_quotationdetail.Qty), 2, MidpointRounding.AwayFromZero);//*((item.CompleteQty - item.PrintQty) / item.Qty)
                        }
                        else
                        {
                            by_deliverydetail.MaterialAmount = Math.Round(Convert.ToDecimal(by_ordercomplete.Weight * by_quotationdetail.MaterialPrice), 2, MidpointRounding.AwayFromZero);
                        }



                        if (by_deliverydetail.JJtype != "按件数")
                        {
                            by_deliverydetail.JJtype = "按公斤";
                        }
                        else
                        {
                            by_deliverydetail.JJtype = "按件数";
                        }

                        if (by_deliverydetail.MaterialAmount == null)
                        {
                            by_deliverydetail.MaterialAmount = 0;
                        }

                        ////加工
                        //by_deliverydetail.WorkUnit = item.WorkUnit;
                        //if (by_deliverydetail.WorkUnit == 1)
                        //{
                        ////小件
                        by_deliverydetail.WorkPrice = by_quotationdetail.WorkPrice;
                        by_deliverydetail.WorkLess = by_quotationdetail.WorkLess;

                        by_deliverydetail.WorkAmount = Math.Round(Convert.ToDecimal(by_quotationdetail.WorkAmount * by_ordercomplete.Qty / by_quotationdetail.Qty), 2, MidpointRounding.AwayFromZero);
                       
                        //}
                        //else
                        //{
                        //    by_deliverydetail.WorkPrice = by_quotationdetail.WorkPrice;
                        //    by_deliverydetail.WorkLess = by_quotationdetail.WorkLess;
                        //    by_deliverydetail.WorkAmount =  Math.Round(Convert.ToDecimal(by_deliverydetail.WorkPrice * by_deliverydetail.TheoryWeight), 2, MidpointRounding.AwayFromZero);
                        //}

                        //热处理
                        by_deliverydetail.HeatPrice = by_quotationdetail.HeatPrice;
                        by_deliverydetail.HeatLess = by_quotationdetail.HeatLess;
                        if (by_ordercomplete.Weight == by_quotationdetail.Weight)
                        {
                            by_deliverydetail.HeatAmount = Math.Round(Convert.ToDecimal(by_quotationdetail.HeatAmount * by_ordercomplete.Qty / by_quotationdetail.Qty), 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            by_deliverydetail.HeatAmount = Math.Round(Convert.ToDecimal(by_quotationdetail.HeatPrice * by_ordercomplete.Weight), 2, MidpointRounding.AwayFromZero);

                        }


                        by_deliverydetail.JKCostPer = by_quotationdetail.JKCostPer;
                        by_deliverydetail.JKCost = Math.Round(Convert.ToDecimal(by_quotationdetail.JKCost * by_ordercomplete.Qty / by_quotationdetail.Qty), 2, MidpointRounding.AwayFromZero);
                       
                        //加工
                        by_deliverydetail.SmallCost = by_quotationdetail.SmallCost;
                        if (by_ordercomplete.Weight == by_quotationdetail.Weight)
                        {
                            by_deliverydetail.SmallAmount = Math.Round(Convert.ToDecimal(by_quotationdetail.SmallAmount * by_ordercomplete.Qty / by_quotationdetail.Qty), 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            by_deliverydetail.SmallAmount = Math.Round(Convert.ToDecimal(by_quotationdetail.SmallCost * by_ordercomplete.Weight), 2, MidpointRounding.AwayFromZero);

                        }
                        //if (by_quotationdetail.WorkWeight > by_quotationdetail.TheoryWeight)
                        //{
                        //    by_deliverydetail.SmallAmount = Math.Round(Convert.ToDecimal(by_deliverydetail.WorkWeight * by_quotationdetail.SmallCost), 2, MidpointRounding.AwayFromZero);
                        //}
                        //else
                        //{
                        //    by_deliverydetail.SmallAmount = Math.Round(Convert.ToDecimal(by_deliverydetail.TheoryWeight * by_quotationdetail.SmallCost), 2, MidpointRounding.AwayFromZero);
                        //}


                        by_deliverydetail.DDCostPer = by_quotationdetail.DDCostPer;
                        by_deliverydetail.DDCost = Math.Round(Convert.ToDecimal(by_quotationdetail.DDCost * by_ordercomplete.Qty / by_quotationdetail.Qty), 2, MidpointRounding.AwayFromZero);


                        by_deliverydetail.MianCostArea = by_quotationdetail.MianCostArea;
                        by_deliverydetail.MianCostPer = item.MianCostPer;
                        by_deliverydetail.MianCost = Math.Round(Convert.ToDecimal(by_quotationdetail.MianCost * by_ordercomplete.Qty / by_quotationdetail.Qty), 2, MidpointRounding.AwayFromZero);


                        by_deliverydetail.BianCostLong = by_quotationdetail.BianCostLong;
                        by_deliverydetail.BianCostPer = item.BianCostPer;
                        by_deliverydetail.BianCost = Math.Round(Convert.ToDecimal(by_quotationdetail.BianCost * by_ordercomplete.Qty / by_quotationdetail.Qty), 2, MidpointRounding.AwayFromZero);

                        //其他
                        //获取费用
                        //by_deliverydetail.MandXSteel = item.MandXSteel;
                        //by_deliverydetail.MorXSteel = item.MorXSteel;
                        //by_deliverydetail.BSteel = item.BSteel;

                        ////面费用
                        //by_deliverydetail.MianCostUnit = item.MianCostUnit;
                        //if (by_deliverydetail.MianCostUnit == 1)
                        //{
                        //    by_deliverydetail.MianCostArea =  Math.Round(Convert.ToDecimal(item.MianCostArea * by_ordercomplete.Qty / item.Qty), 2, MidpointRounding.AwayFromZero);
                        //    by_deliverydetail.MianCostPer = item.MianCostPer;
                        //    by_deliverydetail.MianCost =  Math.Round(Convert.ToDecimal(item.MianCostPer * by_ordercomplete.Qty), 2, MidpointRounding.AwayFromZero);
                        //}
                        //else
                        //{
                        //    by_deliverydetail.MianCostArea =  Math.Round(Convert.ToDecimal(item.MianCostArea * by_ordercomplete.Qty / item.Qty), 2, MidpointRounding.AwayFromZero);
                        //    by_deliverydetail.MianCostPer = item.MianCostPer;
                        //    by_deliverydetail.MianCost =  Math.Round(Convert.ToDecimal(item.MianCost * by_ordercomplete.Qty / item.Qty), 2, MidpointRounding.AwayFromZero);
                        //}
                        ////边费用
                        //by_deliverydetail.BianCostUnit = item.BianCostUnit;
                        //if (by_deliverydetail.BianCostUnit  == 1)
                        //{
                        //    by_deliverydetail.BianCostLong =  Math.Round(Convert.ToDecimal(item.BianCostLong * by_ordercomplete.Qty / item.Qty), 2, MidpointRounding.AwayFromZero);
                        //    by_deliverydetail.BianCostPer = item.BianCostPer;
                        //    by_deliverydetail.BianCost =  Math.Round(Convert.ToDecimal(item.BianCostPer * by_ordercomplete.Qty), 2, MidpointRounding.AwayFromZero);
                        //}
                        //else
                        //{
                        //    by_deliverydetail.BianCostLong =  Math.Round(Convert.ToDecimal(item.BianCostLong * by_ordercomplete.Qty / item.Qty), 2, MidpointRounding.AwayFromZero);
                        //    by_deliverydetail.BianCostPer = item.BianCostPer;
                        //    by_deliverydetail.BianCost =  Math.Round(Convert.ToDecimal(item.BianCost * by_ordercomplete.Qty / item.Qty), 2, MidpointRounding.AwayFromZero);
                        //}


                        //by_deliverydetail.JKCost =  Math.Round(Convert.ToDecimal(item.JKCost * by_ordercomplete.Qty / item.Qty), 2, MidpointRounding.AwayFromZero);
                        //by_deliverydetail.SmallCost =  Math.Round(Convert.ToDecimal(item.SmallCost * by_ordercomplete.Qty / item.Qty), 2, MidpointRounding.AwayFromZero);
                        //by_deliverydetail.WorkCostForWork = Math.Round(Convert.ToDecimal(item.WorkCostForWork * by_ordercomplete.Qty / item.Qty), 2, MidpointRounding.AwayFromZero);


                        by_deliverydetail.Remark = item.ExecRemark;
                        by_deliverydetail.Enabled = 1;
                        by_deliverydetail.BoardDescription = item.BoardDescription;
                        if (item.OrderCode.Substring(0, 1) == "B")
                        {
                            by_deliverydetail.MaterialPrice = 0;
                            by_deliverydetail.MaterialAmount = 0;
                            by_deliverydetail.WorkPrice = 0;
                            by_deliverydetail.WorkAmount = 0;
                            by_deliverydetail.HeatPrice = 0;
                            by_deliverydetail.HeatAmount = 0;
                            by_deliverydetail.SalesAmount = 0;
                            by_deliverydetail.MianCost = 0;
                            by_deliverydetail.BianCost = 0;
                            by_deliverydetail.JKCost = 0;
                            by_deliverydetail.SmallCost = 0;
                            by_deliverydetail.SmallAmount = 0;
                            by_deliverydetail.WorkCostForWork = 0;
                            by_deliverydetail.DDCost = 0;
                        }
                        by_deliverydetail.OrderType = item.OrderType;
                        by_deliverydetail.CustomerOrder = by_quotationdetail.CustomerOrder;
                        by_deliverydetail.CustomerProduct = item.CustomerProduct;

                        by_deliverydetail.ShipDescription = item.ShipDescription;
                        by_deliverydetail.QuotationDetailId = item.QuotationDetailId;

                        by_deliverydetail.ProductName = item.ProductName;
                        by_deliverydetail.LiftingHole = item.LiftingHole;
                        by_deliverydetail.BoreSize = item.BoreSize;
                        by_deliverydetail.SteelType = item.SteelType;

                        by_deliverydetail.SortCode = item.LineCode;



                        //by_deliverydetail.SalesAmount = by_deliverydetail.MaterialAmount + by_deliverydetail.WorkAmount + by_deliverydetail.HeatAmount + by_deliverydetail.MianCost + by_deliverydetail.BianCost
                        //    + by_deliverydetail.JKCost + by_deliverydetail.SmallCost;
                        by_deliverydetail.SalesAmount = by_deliverydetail.MianCost + by_deliverydetail.BianCost + by_deliverydetail.DDCost + by_deliverydetail.MaterialAmount + by_deliverydetail.WorkAmount + by_deliverydetail.HeatAmount + by_deliverydetail.JKCost + by_deliverydetail.SmallAmount;

                        if (by_quotationdetail.Length == 0)
                        {
                            by_deliverydetail.Weight = by_ordercomplete.Weight;
                            by_deliverydetail.MaterialPrice = by_quotationdetail.MaterialPrice;
                            by_deliverydetail.MaterialAmount = Math.Round(Convert.ToDecimal(by_quotationdetail.MaterialAmount * by_ordercomplete.Qty / by_quotationdetail.Qty), 2, MidpointRounding.AwayFromZero);//*((item.CompleteQty - item.PrintQty) / item.Qty)
                            by_deliverydetail.SalesAmount = Math.Round(Convert.ToDecimal(by_quotationdetail.SalesAmount * by_ordercomplete.Qty / by_quotationdetail.Qty), 2, MidpointRounding.AwayFromZero); ;
                        }

                        by_deliverydetail.JBSteel = item.JBSteel;

                        by_deliverydetail.XorMSteel = item.XorMSteel;
                        by_deliverydetail.MianType = item.MianType;
                        by_deliverydetail.BianType = item.BianType;

                        by_deliverydetail.CNC = by_quotationdetail.CNC;
                        by_deliverydetail.YM = by_quotationdetail.YM;
                        by_deliverydetail.JQ = by_quotationdetail.JQ;
                        by_deliverydetail.Technology = by_quotationdetail.Technology;
                        by_deliverydetail.OrderDetailId = item.OrderDetailId;
                        by_deliverydetail = ReturnCost(by_deliverydetail, by_quotation.CustomerId, by_order.OrderCode, by_quotation.CustMaterial, by_delivery.LType, database);

                        by_deliverydetail.BYDUnit = by_quotationdetail.BYDUnit;
                        by_deliverydetail.BYDNum = by_quotationdetail.BYDNum;
                        by_deliverydetail.DeliveryCode = by_delivery.DeliveryCode;
                        by_deliverydetail.ProductName1 = by_quotationdetail.ProductName1;
                        by_deliverydetail.DingCode = by_quotationdetail.DingCode;


                        by_deliverydetail.LJtype1 = by_quotationdetail.LJtype1;
                        by_deliverydetail.LJtype2 = by_quotationdetail.LJtype2;
                        by_deliverydetail.LJsize = by_quotationdetail.LJsize;

                    }

                    database.Insert<BY_DeliveryDetail>(by_deliverydetail, isOpenTrans);
                    //4，更新订单详情确认发运数
                    item.PrintQty = item.CompleteQty;
                    if (item.CompleteQty == item.Qty)
                    {
                        item.BillStatus = OrderBillStatus.已发运.ToString();
                    }
                    else
                    {
                        item.BillStatus = OrderBillStatus.部分发运.ToString();
                    }
                    database.Update<BY_OrderDetail>(item, isOpenTrans);
                    by_deliverydetails.Add(by_deliverydetail);
                    //5，更新订单完工表发运单号
                    by_ordercomplete.DeliveryCode = by_delivery.DeliveryCode;
                    database.Update<BY_OrderComplete>(by_ordercomplete, isOpenTrans);
                }

                by_delivery.OrderId = by_order.OrderId;
                by_delivery.OrderCode = by_order.OrderCode;
                by_delivery.CustomerPO = by_order.CustomerPO;
                by_delivery.CustomerId = by_order.CustomerId;
                by_delivery.CustomerCode = by_order.CustomerCode;
                by_delivery.CustomerName = by_order.CustomerName;
                by_delivery.ShortName = by_order.ShortName;
                by_delivery.ContactName = by_order.ContactName;
                by_delivery.ContactTel = by_order.ContactTel;
                by_delivery.ContactFax = by_order.ContactFax;
                by_delivery.ContactMail = by_order.ContactMail;
                by_delivery.MobileCode = by_order.MobileCode;
                by_delivery.CustomerAddress = by_order.CustomerAddress;
                by_delivery.OrderType = by_order.OrderType;
                by_delivery.Qty = by_deliverydetails.Where(p => p.Length > 0).Sum(m => m.Qty);
                by_delivery.TotalAmount = by_deliverydetails.Sum<BY_DeliveryDetail>(m => m.SalesAmount);
                by_delivery.LessRate = by_quotation.LessRate;
                by_delivery.LessedAmount = by_deliverydetails.Sum<BY_DeliveryDetail>(m => m.SalesAmount) * (1 - by_quotation.LessRate/100);
                by_delivery.PaymentMode = by_quotation.PaymentMode;
                by_delivery.ShipMode = by_quotation.ShipMode;
                by_delivery.ShipGroup = by_quotation.ShipGroup;
                by_delivery.SalesMode = by_quotation.SalesMode;
                by_delivery.SalesMan = by_quotation.SalesMan;
                by_delivery.SalesArea = by_quotation.SalesArea;
                by_delivery.ShipRoute = by_quotation.ShipRoute;
                by_delivery.IsEmergency = by_quotation.IsEmergency;
                by_delivery.CollectAddress = by_quotation.CollectAddress;
                by_delivery.ShipAddress = by_quotation.ShipAddress;
                by_delivery.PrintDate = by_quotation.PrintDate;
                by_delivery.PrintUserId = by_quotation.PrintUserId;
                //打印时更新
                //by_delivery.PrintDate = DateTime.Now;
                //by_delivery.PrintUserId = ManageProvider.Provider.Current().UserId;
                //by_delivery.LastPrintDate = DateTime.Now;
                //by_delivery.LastPrintUserId = ManageProvider.Provider.Current().UserId;
                by_delivery.ShipDescription = by_quotation.ShipDescription;
                by_delivery.Enabled = 1;
                by_delivery.DeleteMark = 0;
                by_delivery.ShipClause = by_quotation.ShipClause;
                by_delivery.SteelQualityCert = by_quotation.SteelQualityCert;
                by_delivery.QualityCert = by_quotation.QualityCert;
                by_delivery.FactoryCert = by_quotation.FactoryCert;
                by_delivery.ByPiece = by_quotation.ByPiece;
                by_delivery.CustMaterial = by_quotation.CustMaterial;
                by_delivery.FreeBill = by_quotation.FreeBill;
                by_delivery.Weight = by_deliverydetails.Where(p => p.Length > 0).Sum<BY_DeliveryDetail>(m => m.Weight);
                by_delivery.MnemonicCode = by_order.MnemonicCode;

                by_delivery.DepartName = by_order.DepartName;
                by_delivery.KLRequery = by_order.KLRequery;
                by_delivery.XMRequery = by_order.XMRequery;
                by_delivery.LType = by_quotation.LType;
                by_delivery.CompanyName = by_quotation.CompanyName;


                by_delivery.BuildNum = by_quotation.BuildNum;
                by_delivery.ProductCode = by_quotation.ProductCode;
                by_delivery.BuildName = by_quotation.BuildName;
                by_delivery.DieName = by_quotation.DieName;
                by_delivery.PicCode = by_quotation.PicCode;
                by_delivery.DieCode = by_quotation.DieCode;
                
                by_delivery.Remark = by_quotation.Remark;
                by_delivery.IsInvoice = by_quotation.IsInvoice;
                by_delivery.IsReturn = by_quotation.IsReturn;
                

                database.Insert<BY_Delivery>(by_delivery, isOpenTrans);
                base_coderulebll.OccupyBillCode(ManageProvider.Provider.Current().UserId, "D", isOpenTrans);
                database.Commit();

                new OrderQueryController().SetOrderStatus(by_order.OrderCode);
                //取消自动生成
                //_thread = new System.Threading.Thread(ExcuteBlack);
                //_thread.IsBackground = true;
                //_thread.Start();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功。" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败:" + ex.Message}.ToString());
            }
        }

        public BY_DeliveryDetail ReturnCost(BY_DeliveryDetail by_deliverydetail, string CustomerID, string OrderCode, int? CustMaterial,string LType, IDatabase database)
        {
            #region 成本计算
            //Base_Customer base_customer = database.FindEntity<Base_Customer>(CustomerID);
            //if (base_customer != null && base_customer.CustomerId != null)
            //{
            //    by_deliverydetail.ManagerCost = by_deliverydetail.SalesAmount * CommonHelper.GetDecimal(base_customer.ManagementCostRate);
            //    by_deliverydetail.InvoiceCost = by_deliverydetail.SalesAmount * CommonHelper.GetDecimal(base_customer.TaxRate);
            //}
       
            by_deliverydetail.ManagerCost = 0;
            by_deliverydetail.InvoiceCost = 0;
            //decimal? Weight5 = 0;
            //decimal? AddThick = 5;
            //decimal? AddWeigh = 5;
            //decimal? AddLength = 5;

            Base_Steel base_steel = database.FindEntity<Base_Steel>("SteelCode", by_deliverydetail.SteelCode);
            by_deliverydetail.Weight5 = by_deliverydetail.Weight;
            if (CustMaterial == 1)
            {
                by_deliverydetail.Weight5 = 0;
                by_deliverydetail.GWeight = 0;

            }
            else
            {
                by_deliverydetail.Weight5 = by_deliverydetail.Weight;
                if (LType == "RCL")
                {
                    by_deliverydetail.GWeight = by_deliverydetail.Weight;
                }
                else
                {
                    by_deliverydetail.GWeight = by_deliverydetail.Weight5 * CommonHelper.GetDecimal(1.05);
                }

            }

            // 按过磅算实际重量
            //                if (!string.IsNullOrEmpty(OrderCode))
            //                {
            //                    List<BY_OrderDetailProcessDetail> by_orderdetailprocessdetails = DataFactory.Database().FindListBySql<BY_OrderDetailProcessDetail>(string.Format(@" 
            //SELECT * FROM BY_OrderDetailProcessDetail WHERE OrderCode='{0}' AND LineCode={1} ", OrderCode, by_deliverydetail.LineCode));
            //                    //45直接算理重
            //                    by_deliverydetail.GWeight = by_orderdetailprocessdetails.Where(p => p.TechnologyId == "K001").Sum(P => P.Weight) * CommonHelper.GetDecimal(1.08);
            //                    Base_SteelDDPerchaseDetail base_steelddperchasedetail = DataFactory.Database().FindEntity<Base_SteelDDPerchaseDetail>("QuotationDetailId", by_deliverydetail.QuotationDetailId);
            //                    if (base_steelddperchasedetail != null && base_steelddperchasedetail.Base_SteelDDPerchaseDetailId != null)
            //                    {
            //                        by_deliverydetail.GWeight = base_steelddperchasedetail.Weight;
            //                        by_deliverydetail.MaterialCostPer = base_steelddperchasedetail.MaterialPrice;
            //                    }
            //                }
            //                else
            //                {
            //                    by_deliverydetail.GWeight = 0;
            //                }
            
            if (by_deliverydetail.Technology != "&nbsp;" && !string.IsNullOrWhiteSpace(by_deliverydetail.Technology) && LType == "RCL")
            {
                //外发加工
                BY_QuotationHeatBeoutDetail by_quotationheatbeoutdetail = DataFactory.Database().FindEntity<BY_QuotationHeatBeoutDetail>("QuotationHeatCode", by_deliverydetail.OrderDetailId);
                if (by_quotationheatbeoutdetail != null && by_quotationheatbeoutdetail.QuotationHeatCode != null)
                {
                    by_deliverydetail.HeatCostPer = by_quotationheatbeoutdetail.HeatPrice;
                    by_deliverydetail.HeatCost = by_deliverydetail.HeatCostPer * by_deliverydetail.GWeight;
                }
                else
                {
                    Base_DataDictionaryDetail Base_DataDictionaryDetail = DataFactory.Database().FindEntityByWhere<Base_DataDictionaryDetail>(" and  DataDictionaryId='da5eb34c-3f97-4a84-a855-c3d42b5d712e' and  CHARINDEX(Code,'" + by_deliverydetail.Technology + "') > 0");
                    if (Base_DataDictionaryDetail != null && Base_DataDictionaryDetail.DataDictionaryDetailId != null)
                    {
                        by_deliverydetail.HeatCostPer = CommonHelper.GetDecimal(Base_DataDictionaryDetail.Money);
                        by_deliverydetail.HeatCost = by_deliverydetail.HeatCostPer * by_deliverydetail.GWeight;

                        if (by_deliverydetail.HeatCostPer < 0)
                        {
                            by_deliverydetail.HeatCostPer = by_deliverydetail.HeatPrice;
                            by_deliverydetail.HeatCost = by_deliverydetail.HeatAmount;
                        }
                    }
                    else
                    {
                        by_deliverydetail.HeatCostPer = by_deliverydetail.HeatPrice;
                        by_deliverydetail.HeatCost = by_deliverydetail.HeatAmount;
                    }
                }
            }

            //外发加工
            if ((by_deliverydetail.Technology == "&nbsp;" || string.IsNullOrWhiteSpace(by_deliverydetail.Technology)) && LType != "RCL")
            {
                Base_SteelDDPerchaseDetail base_steelddperchasedetail = DataFactory.Database().FindEntity<Base_SteelDDPerchaseDetail>("QuotationDetailId", by_deliverydetail.QuotationDetailId);
                if (base_steelddperchasedetail != null && base_steelddperchasedetail.Base_SteelDDPerchaseDetailId != null)
                {
                    by_deliverydetail.GWeight = base_steelddperchasedetail.Weight;
                    by_deliverydetail.MaterialCostPer = base_steelddperchasedetail.MaterialPrice;
                }
                else
                {
                    int daay = 1;
                    List<BY_Inventory> by_inventorys = new List<BY_Inventory>();
                    do
                    {
                        by_inventorys = database.FindListBySql<BY_Inventory>("select * from BY_Inventory where SteelCode = '" + by_deliverydetail.SteelCode + "' and Price > 0 and CreateDate < DATEADD(DAY,10, convert(datetime,'" + by_deliverydetail.CreateDate + "', 20)) and CreateDate > DATEADD(DAY," + (daay * (-30)) + ", convert(datetime,'" + by_deliverydetail.CreateDate + "', 20))");
                        daay = daay + 1;
                        by_inventorys.Remove(by_inventorys.Where(c => c.Price == by_inventorys.Max(p => p.Price)).FirstOrDefault());
                        by_inventorys.Remove(by_inventorys.Where(c => c.Price == by_inventorys.Min(p => p.Price)).FirstOrDefault());
                    } while (by_inventorys.Count < 5 && daay < 3);

                    if (by_inventorys.Count > 0)
                    {
                        by_deliverydetail.MaterialCostPer = by_inventorys.Sum(p => p.Price) / by_inventorys.Count;
                        if (by_deliverydetail.MaterialCostPer < 0)
                        {
                            Base_Steel basesteel = DataFactory.Database().FindEntity<Base_Steel>("SteelCode", by_deliverydetail.SteelCode);
                            by_deliverydetail.MaterialCostPer = basesteel.Price;
                        }
                    }
                    else
                    {
                        Base_Steel basesteel = DataFactory.Database().FindEntity<Base_Steel>("SteelCode", by_deliverydetail.SteelCode);
                        by_deliverydetail.MaterialCostPer = basesteel.Price;
                    }
                }
                by_deliverydetail.MaterialCost = by_deliverydetail.GWeight * by_deliverydetail.MaterialCostPer;
            }

            //by_deliverydetail.MianCostArea = new QuotationInputController().ReturnArea(by_deliverydetail.Thick, by_deliverydetail.Weigh, by_deliverydetail.Length, by_deliverydetail.OrderType);
            //
            //by_deliverydetail.WorkCost = new QuotationInputController().ReturnWorkCost(CommonHelper.GetDecimal(by_deliverydetail.MianCostArea), CommonHelper.GetDecimal(by_deliverydetail.Weigh), by_deliverydetail.SteelType, by_deliverydetail.OrderType, new Base_Supplier(), 0, 0, ref WorkCostPer, ref LimiteCost) * by_deliverydetail.Qty;
            //by_deliverydetail.WorkCostPer = WorkCostPer;
            //by_deliverydetail.LimiteCost = LimiteCost;
            //by_deliverydetail.NBworkcost = new QuotationInputController().ReturnWorkCost1(CommonHelper.GetDecimal(by_deliverydetail.MianCostArea), CommonHelper.GetDecimal(by_deliverydetail.Weigh), by_deliverydetail.SteelType, by_deliverydetail.OrderType, new Base_Supplier(), ref WorkCostPer) * by_deliverydetail.Qty;
            #endregion

            //管理费，税费
            return by_deliverydetail;
        }


        [LoginAuthorize]
        public ActionResult CostUpdate()
        {
            IDatabase database = DataFactory.Database();
            try
            {
                int num = 0;
                List<BY_Delivery> by_deliverys = DataFactory.Database().FindList<BY_Delivery>();
                foreach (BY_Delivery item in by_deliverys)
                {
                    List<BY_DeliveryDetail> by_deliverydetails = DataFactory.Database().FindList<BY_DeliveryDetail>("DeliveryId",item.DeliveryId);
                    BY_DeliveryDetail by_deliverydetail;
                    foreach (BY_DeliveryDetail entity in by_deliverydetails)
                    {
                        by_deliverydetail = entity;
                        by_deliverydetail = this.ReturnCost(entity, item.CustomerId,item.OrderCode,item.CustMaterial, item.LType, database);
                        database.Update(by_deliverydetail);
                    }
                    num = num + 1;
                }
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功。" }.ToString());
            }
            catch (Exception)
            {
                return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败。" }.ToString());
            }
        }

        public ActionResult CostUpdate1(string deliveryid, string linecode)
        {
            IDatabase database = DataFactory.Database();
            try
            {
                BY_Delivery by_delivery = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryId", deliveryid);
                BY_DeliveryDetail by_deliverydetail = DataFactory.Database().FindEntityByWhere<BY_DeliveryDetail>(string.Format(" AND deliveryid='{0}' AND linecode={1}", deliveryid, linecode));
                BY_OrderDetail by_orderdetail = DataFactory.Database().FindEntity<BY_OrderDetail>("QuotationDetailId", by_deliverydetail.QuotationDetailId);
                by_deliverydetail = this.ReturnCost(by_deliverydetail, by_delivery.CustomerId, by_delivery.OrderCode,  by_delivery.CustMaterial, by_delivery.LType, database);
                database.Update(by_deliverydetail);
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功。" }.ToString());
            }
            catch (Exception)
            {
                return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败。" }.ToString());
            }
        }

        //System.Threading.Thread _thread;
        //private void ExcuteBlack()
        //{
        //    try
        //    {
        //        BY_CheckSheetController by_checksheetcontroller = new BY_CheckSheetController();
        //        DateTime dt = DateTime.Now;
        //        by_checksheetcontroller.BuildCheckSheet(dt.Year.ToString(), dt.Month.ToString());
        //    }
        //    catch (Exception)
        //    {
        //        _thread.Abort();
        //    }
        //    _thread.Abort();
        //}

    }
}
