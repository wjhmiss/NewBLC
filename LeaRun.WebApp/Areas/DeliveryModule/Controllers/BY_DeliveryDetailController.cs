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

namespace DNS.WebApp.Areas.DeliveryModule.Controllers
{
    /// <summary>
    /// 发运单明细控制器
    /// </summary>
    public class BY_DeliveryDetailController : PublicController<BY_DeliveryDetail>
    {
        public ActionResult GridPageListJson2(string deliverycode, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = by_deliverydetailbll.GetPageList2(deliverycode, ref jqgridparam);
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
        private bool CanNotEdit1(string DeliveryCode)
        {
            BY_Delivery by_delivery = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryCode", DeliveryCode);
            if (by_delivery != null && by_delivery.DeliveryId != null)
            {
                if (!string.IsNullOrEmpty(by_delivery.CheckSheetCode))
                {
                    return true;
                }
                if (by_delivery.GetedAmount > 0)
                {
                    return true;
                }
            }
            return false;
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult SetCostForm()
        {
            return View();
        }

        public virtual ActionResult SetForm1(string deliveryid, string linecode, string KeyValue)
        {
            BY_DeliveryDetail entity = new BY_DeliveryDetail();
            if (string.IsNullOrEmpty(linecode))
            {
                entity = repositoryfactory.Repository().FindEntity(KeyValue);
            }
            else
            {
                 entity = repositoryfactory.Repository().FindList(string.Format(" and deliveryid='{0}' and linecode >=  {1} ", deliveryid, linecode)).OrderBy(P => P.LineCode).FirstOrDefault();
            }

            return Content(entity.ToJson());
        }

        BY_DeliveryDetailBll by_deliverydetailbll = new BY_DeliveryDetailBll();
        public ActionResult GridPageListJson(string LType, string DeliveryCode, string MnemonicCode, string CustomerName, string ProductName, string Size, string StartTime, string EndTime, string SteelCode, string Technology, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = by_deliverydetailbll.GetPageList(LType,DeliveryCode, MnemonicCode, CustomerName, ProductName, Size, StartTime, EndTime,  SteelCode,  Technology, ref jqgridparam);
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

        public ActionResult GridPageListJson1(string CheckSheetId, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                List<BY_DeliveryDetail> by_deliverydetails = by_deliverydetailbll.GetPageList1(CheckSheetId, ref jqgridparam);
                List<BY_DeliveryDetail> by_deliverydetails1 = by_deliverydetailbll.GetPageList11(CheckSheetId, ref jqgridparam);


                BY_DeliveryDetail by_deliverydetailfy = new BY_DeliveryDetail();
                by_deliverydetailfy.Qty = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.SalesAmount) != 0 && p.Length == 0).Sum(p => p.Qty);
                by_deliverydetailfy.MaterialAmount = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.SalesAmount) != 0 && p.Length == 0).Sum(p => p.SalesAmount);
                by_deliverydetailfy.Weight = 0;
                by_deliverydetailfy.MaterialPrice = 0;
                by_deliverydetailfy.ProductName = "运费或其他";
                if (CommonHelper.GetDecimal(by_deliverydetailfy.MaterialAmount) != 0)
                {
                    by_deliverydetails.Add(by_deliverydetailfy);
                }

                //锯弓费
                BY_DeliveryDetail by_deliverydetailworkJ = new BY_DeliveryDetail();
                by_deliverydetailworkJ.Qty = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.JKCost) != 0 && p.Length > 0).Sum(p => p.Qty);
                by_deliverydetailworkJ.MaterialAmount = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.JKCost) != 0 && p.Length > 0).Sum(p => p.JKCost);
                by_deliverydetailworkJ.Weight = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.JKCost) != 0 && p.Length > 0).Sum(p => p.Weight);
                by_deliverydetailworkJ.MaterialPrice = by_deliverydetailworkJ.Qty == 0 ? 0 : by_deliverydetailworkJ.MaterialAmount / by_deliverydetailworkJ.Qty;
                by_deliverydetailworkJ.ProductName = "锯工费";
                if (CommonHelper.GetDecimal(by_deliverydetailworkJ.MaterialAmount) != 0)
                {
                    by_deliverydetails.Add(by_deliverydetailworkJ);
                }
                //加工
                BY_DeliveryDetail by_deliverydetailwork = new BY_DeliveryDetail();
                by_deliverydetailwork.Qty = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.WorkAmount) != 0 && p.Length > 0).Sum(p => p.Qty);
                by_deliverydetailwork.MaterialAmount = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.WorkAmount) != 0 && p.Length > 0).Sum(p => p.WorkAmount);
                by_deliverydetailwork.Weight = 0;
                by_deliverydetailwork.MaterialPrice = by_deliverydetailwork.Qty == 0 ? 0 : by_deliverydetailwork.MaterialAmount / by_deliverydetailwork.Qty;
                by_deliverydetailwork.ProductName = "加工费(件)";
                if (CommonHelper.GetDecimal(by_deliverydetailwork.MaterialAmount) != 0)
                {
                    by_deliverydetails.Add(by_deliverydetailwork);
                }

                //加工
                BY_DeliveryDetail by_deliverydetailwork1 = new BY_DeliveryDetail();
                by_deliverydetailwork1.Qty = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.SmallAmount) != 0 && p.Length > 0).Sum(p => p.Qty);
                by_deliverydetailwork1.MaterialAmount = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.SmallAmount) != 0 && p.Length > 0).Sum(p => p.SmallAmount);
                by_deliverydetailwork1.Weight = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.SmallAmount) != 0 && p.Length > 0).Sum(p => p.Weight);
                by_deliverydetailwork1.MaterialPrice = by_deliverydetailwork1.Qty == 0 ? 0 : by_deliverydetailwork1.MaterialAmount / by_deliverydetailwork1.Weight;
                by_deliverydetailwork1.ProductName = "加工费";
                if (CommonHelper.GetDecimal(by_deliverydetailwork1.MaterialAmount) != 0)
                {
                    by_deliverydetails.Add(by_deliverydetailwork1);
                }

                BY_DeliveryDetail by_deliverydetaildd = new BY_DeliveryDetail();
                by_deliverydetaildd.Qty = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.DDCost) != 0 && p.Length > 0).Sum(p => p.Qty);
                by_deliverydetaildd.MaterialAmount = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.DDCost) != 0 && p.Length > 0).Sum(p => p.DDCost);
                by_deliverydetaildd.Weight = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.DDCost) != 0 && p.Length > 0).Sum(p => p.Weight);
                by_deliverydetaildd.MaterialPrice = by_deliverydetaildd.Qty == 0 ? 0 : by_deliverydetaildd.MaterialAmount / by_deliverydetaildd.Weight;
                by_deliverydetaildd.ProductName = "切角";
                if (CommonHelper.GetDecimal(by_deliverydetaildd.MaterialAmount) != 0)
                {
                    by_deliverydetails.Add(by_deliverydetaildd);
                }

                BY_DeliveryDetail by_deliverydetailhot = new BY_DeliveryDetail();
                by_deliverydetailhot.Qty = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.HeatAmount) != 0 && p.Length > 0).Sum(p => p.Qty);
                by_deliverydetailhot.MaterialAmount = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.HeatAmount) != 0 && p.Length > 0).Sum(p => p.HeatAmount);
                by_deliverydetailhot.Weight = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.HeatAmount) != 0 && p.Length > 0).Sum(p => p.Weight);
                by_deliverydetailhot.MaterialPrice = by_deliverydetailhot.Qty == 0 ? 0 : by_deliverydetailhot.MaterialAmount / by_deliverydetailhot.Weight;
                by_deliverydetailhot.ProductName = "热处理费";
                if (CommonHelper.GetDecimal(by_deliverydetailhot.MaterialAmount) != 0)
                {
                    by_deliverydetails.Add(by_deliverydetailhot);
                }



                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = by_deliverydetails.Where(p => p.MaterialAmount != 0),
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }


        public ActionResult SubmitDeliveryForm(BY_DeliveryDetail BY_DeliveryDetail, string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            try
            {
                BY_DeliveryDetail deliverydetail = database.FindEntity<BY_DeliveryDetail>(KeyValue);
                BY_Delivery by_delivery = database.FindEntity<BY_Delivery>(deliverydetail.DeliveryId);
                BY_CheckSheet by_checksheet = new BY_CheckSheet();
                if (CanNotEdit1(by_delivery.DeliveryCode))
                {
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：已生成发运单或已收款！" }.ToString());
                }
                BY_DeliveryDetail.Modify(KeyValue);
                DataFactory.Database().Update<BY_DeliveryDetail>(BY_DeliveryDetail);
                List<BY_DeliveryDetail> by_deliverydetails = database.FindList<BY_DeliveryDetail>("DeliveryId", by_delivery.DeliveryId);

                by_delivery.Weight = by_deliverydetails.Sum(p=>p.Weight);
                by_delivery.TotalAmount = by_deliverydetails.Sum(p => p.SalesAmount);
                if (by_delivery.CheckSheetCode != null)
                {
                    by_checksheet = database.FindEntity<BY_CheckSheet>("CheckSheetCode", by_delivery.CheckSheetCode);
                    by_checksheet.TotalAmount = by_deliverydetails.Sum(p => p.SalesAmount);
                    by_checksheet.Modify(by_checksheet.CheckSheetId);
                    DataFactory.Database().Update<BY_CheckSheet>(by_checksheet);
                }

                by_delivery.Modify(by_delivery.DeliveryId);
                DataFactory.Database().Update<BY_Delivery>(by_delivery);

                string Message = "修改成功。";
                return Content(new JsonMessage { Success = true, Code = "1", Message = Message, Attribute = deliverydetail.LineCode.ToString() }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        public ActionResult SubmitDeliveryCostFormPer(BY_DeliveryDetail BY_DeliveryDetail, string KeyValue)
        {
            try
            {
                BY_DeliveryDetail deliverydetail = DataFactory.Database().FindEntity<BY_DeliveryDetail>(KeyValue);
                deliverydetail.GWeight = BY_DeliveryDetail.GWeight;
                deliverydetail.Setself = BY_DeliveryDetail.Setself;

                deliverydetail.MaterialCostPer = BY_DeliveryDetail.MaterialCostPer;
                deliverydetail.WorkCostPer = BY_DeliveryDetail.WorkCostPer;
                deliverydetail.HeatCostPer = BY_DeliveryDetail.HeatCostPer;
                deliverydetail.LimiteCost = BY_DeliveryDetail.LimiteCost;
                deliverydetail.MianCostArea = BY_DeliveryDetail.MianCostArea;
                deliverydetail.NBworkcost = BY_DeliveryDetail.NBworkcost;
                deliverydetail.BeOutWorkCost = BY_DeliveryDetail.BeOutWorkCost;
                deliverydetail.HeatWorkCost = BY_DeliveryDetail.HeatWorkCost;

                BY_Delivery by_delivery = DataFactory.Database().FindEntity<BY_Delivery>(deliverydetail.DeliveryId);
                BY_OrderDetail by_orderdetail = DataFactory.Database().FindEntity<BY_OrderDetail>("QuotationDetailId", deliverydetail.QuotationDetailId);

                deliverydetail = new InventoryComfirmController().ReturnCost(deliverydetail, by_delivery.CustomerId, by_delivery.OrderCode, by_delivery.CustMaterial, by_delivery.LType, DataFactory.Database());
                DataFactory.Database().Update(deliverydetail);
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功" }.ToString());
            }
            catch (Exception)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败" }.ToString());
            }


        }

        public ActionResult IndexLJ()
        {
            return View();
        }
        
    }
}