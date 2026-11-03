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

namespace DNS.WebApp.Areas.CheckSheetModule.Controllers
{
    /// <summary>
    /// By_InvoiceForDetail控制器
    /// </summary>
    public class By_InvoiceForDetailController : PublicController<By_InvoiceForDetail>
    {
        public virtual ActionResult BJPrintPreview()
        {
            return View();
        }
        BY_DeliveryDetailBll by_deliverydetailbll = new BY_DeliveryDetailBll();
        public ActionResult GridPaasdf(string checksheetid, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                List<BY_DeliveryDetail> by_deliverydetails = by_deliverydetailbll.GetPageList1(checksheetid, ref jqgridparam);//  and d.Enabled > 0 and dd.Length > 0
                List<BY_DeliveryDetail> by_deliverydetails1 = by_deliverydetailbll.GetPageList11(checksheetid, ref jqgridparam);
                //热处理
                BY_DeliveryDetail by_deliverydetailRCL = new BY_DeliveryDetail();
                by_deliverydetailRCL.Qty = by_deliverydetails1.Where<BY_DeliveryDetail>(p => p.MaterialAmount != 0 && p.Enabled > 0 && p.Length == 0 && p.SteelCode != "FY").Sum(p => p.Qty);
                by_deliverydetailRCL.MaterialAmount = by_deliverydetails1.Where<BY_DeliveryDetail>(p => p.MaterialAmount != 0 && p.Enabled > 0 && p.Length == 0 && p.SteelCode != "FY").Sum(p => p.MaterialAmount);
                by_deliverydetailRCL.MaterialPrice = by_deliverydetailRCL.Qty == 0 ? 0 : by_deliverydetailRCL.MaterialAmount / by_deliverydetailRCL.Qty;
                by_deliverydetailRCL.ProductName = "热处理合计";
                by_deliverydetailRCL.Weight = by_deliverydetails1.Where<BY_DeliveryDetail>(p => p.MaterialAmount != 0 && p.Enabled > 0 && p.Length == 0 && p.SteelCode != "FY").Sum(p => p.Weight);
                if (CommonHelper.GetDecimal(by_deliverydetailRCL.MaterialAmount) != 0)
                {
                    by_deliverydetails.Add(by_deliverydetailRCL);
                }
                //其他
                BY_DeliveryDetail by_deliverydetailFY = new BY_DeliveryDetail();
                by_deliverydetailFY.Qty = by_deliverydetails1.Where<BY_DeliveryDetail>(p => p.SteelCode == "FY").Sum(p => p.Qty);
                by_deliverydetailFY.MaterialAmount = by_deliverydetails1.Where<BY_DeliveryDetail>(p =>  p.SteelCode == "FY").Sum(p => p.SalesAmount);
                by_deliverydetailFY.MaterialPrice = by_deliverydetailFY.Qty == 0 ? 0 : by_deliverydetailFY.MaterialAmount / by_deliverydetailFY.Qty;
                by_deliverydetailFY.ProductName = "其他费用";
                by_deliverydetailFY.Weight = by_deliverydetails1.Where<BY_DeliveryDetail>(p => p.SteelCode == "FY").Sum(p => p.Weight);
                if (CommonHelper.GetDecimal(by_deliverydetailFY.MaterialAmount) != 0)
                {
                    by_deliverydetails.Add(by_deliverydetailFY);
                }
                //切口费
                BY_DeliveryDetail by_deliverydetailworkJ = new BY_DeliveryDetail();
                by_deliverydetailworkJ.Qty = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.JKCost) != 0).Sum(p => p.Qty);
                by_deliverydetailworkJ.MaterialAmount = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.JKCost) != 0).Sum(p => p.JKCost);
                by_deliverydetailworkJ.MaterialPrice = by_deliverydetailworkJ.Qty == 0 ? 0 : by_deliverydetailworkJ.MaterialAmount / by_deliverydetailworkJ.Qty;
                by_deliverydetailworkJ.ProductName = "锯工费";
                by_deliverydetailworkJ.Weight = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.JKCost) != 0).Sum(p => p.Weight);
                if (CommonHelper.GetDecimal(by_deliverydetailworkJ.MaterialAmount) != 0)
                {
                    by_deliverydetails.Add(by_deliverydetailworkJ);
                }
                //切角
                BY_DeliveryDetail by_deliverydetailZJ = new BY_DeliveryDetail();
                by_deliverydetailZJ.Qty = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.DDCost) != 0).Sum(p => p.Qty);
                by_deliverydetailZJ.MaterialAmount = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.DDCost) != 0).Sum(p => p.DDCost);
                by_deliverydetailZJ.MaterialPrice = by_deliverydetailZJ.Qty == 0 ? 0 : by_deliverydetailZJ.MaterialAmount / by_deliverydetailZJ.Qty;
                by_deliverydetailZJ.ProductName = "切角";
                by_deliverydetailZJ.Weight = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.DDCost) != 0).Sum(p => p.Weight);
                if (CommonHelper.GetDecimal(by_deliverydetailZJ.MaterialAmount) != 0)
                {
                    by_deliverydetails.Add(by_deliverydetailZJ);
                }
                //加工
                BY_DeliveryDetail by_deliverydetailwork = new BY_DeliveryDetail();
                by_deliverydetailwork.Qty = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.WorkAmount) != 0).Sum(p => p.Qty);
                by_deliverydetailwork.MaterialAmount = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.WorkAmount) != 0).Sum(p => p.WorkAmount);
                by_deliverydetailwork.MaterialPrice = by_deliverydetailwork.Qty == 0 ? 0 : by_deliverydetailwork.MaterialAmount / by_deliverydetailwork.Qty;
                by_deliverydetailwork.ProductName = "加工费(件)";
                by_deliverydetailwork.Weight = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.WorkAmount) != 0).Sum(p => p.Weight);
                if (CommonHelper.GetDecimal(by_deliverydetailwork.MaterialAmount) != 0)
                {
                    by_deliverydetails.Add(by_deliverydetailwork);
                }

                //加工
                BY_DeliveryDetail by_deliverydetailwork1 = new BY_DeliveryDetail();
                by_deliverydetailwork1.Qty = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.SmallAmount) != 0).Sum(p => p.Qty);
                by_deliverydetailwork1.MaterialAmount = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.SmallAmount) != 0).Sum(p => p.SmallAmount);
                by_deliverydetailwork1.Weight = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.SmallAmount) != 0).Sum(p => p.Weight);
                by_deliverydetailwork1.MaterialPrice = by_deliverydetailwork1.Qty == 0 ? 0 : by_deliverydetailwork1.MaterialAmount / by_deliverydetailwork1.Weight;
                by_deliverydetailwork1.ProductName = "加工费(KG)";

                if (CommonHelper.GetDecimal(by_deliverydetailwork1.MaterialAmount) != 0)
                {
                    by_deliverydetails.Add(by_deliverydetailwork1);
                }

                BY_DeliveryDetail by_deliverydetailhot = new BY_DeliveryDetail();
                by_deliverydetailhot.Qty = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.HeatAmount) != 0).Sum(p => p.Qty);
                by_deliverydetailhot.MaterialAmount = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.HeatAmount) != 0).Sum(p => p.HeatAmount);
                by_deliverydetailhot.Weight = by_deliverydetails1.Where<BY_DeliveryDetail>(p => Convert.ToDecimal(p.HeatAmount) != 0).Sum(p => p.Weight);
                by_deliverydetailhot.MaterialPrice = by_deliverydetailhot.Qty == 0 ? 0 : by_deliverydetailhot.MaterialAmount / by_deliverydetailhot.Weight;
                by_deliverydetailhot.ProductName = "热处理费";
                if (CommonHelper.GetDecimal(by_deliverydetailhot.Qty) != 0)
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

        public JsonResult GridPageJson1(string checksheetid, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                List<By_InvoiceForDetail> ListData = new List<By_InvoiceForDetail>();
                if (!string.IsNullOrEmpty(checksheetid))
                {
                    List<DbParameter> parameter = new List<DbParameter>();
                    ListData = repositoryfactory.Repository().FindList("CheckSheetId", checksheetid);
                }
                
                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = ListData,
                };
                return Json(JsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public ActionResult SubmitForm1(By_InvoiceForDetail entity, string KeyValue, string checksheetid)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            BY_CheckSheet by_checksheet = new BY_CheckSheet();
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    By_InvoiceForDetail Oldentity = repositoryfactory.Repository().FindEntity(KeyValue);//获取没更新之前实体对象
                    by_checksheet = DataFactory.Database().FindEntity<BY_CheckSheet>(Oldentity.CheckSheetId);
                    entity.LType = by_checksheet.LType;
                    entity.CompanyName = by_checksheet.CompanyName;
                    entity.Modify(KeyValue);
                    IsOk = repositoryfactory.Repository().Update(entity, isOpenTrans);

                    if (!string.IsNullOrEmpty(by_checksheet.CheckSheetCode))
                    {
                        List<BY_ReimburseSheet> by_reimbursesheets = DataFactory.Database().FindList<BY_ReimburseSheet>("Code", by_checksheet.CheckSheetCode);
                        foreach (BY_ReimburseSheet item in by_reimbursesheets)
                        {
                            item.InvoiceNum = item.InvoiceNum.Replace(Oldentity.InvoiceNum, entity.InvoiceNum);
                            DataFactory.Database().Update(item);
                        }
                    }
                }
                else
                {
                    by_checksheet = DataFactory.Database().FindEntity<BY_CheckSheet>(checksheetid);
                    By_InvoiceForDetail by_invoicefordetail = new By_InvoiceForDetail();
                    CommonHelper.CopySameFieldsObjectALL<By_InvoiceForDetail, By_InvoiceForDetail>(entity, ref by_invoicefordetail);
                    by_invoicefordetail.CheckSheetId = by_checksheet.CheckSheetId;
                    by_invoicefordetail.CustomerId = by_checksheet.CustomerId;
                    by_invoicefordetail.CustomerName = by_checksheet.CustomerName;
                    by_invoicefordetail.CustomerCode = by_checksheet.CustomerCode;
                    by_invoicefordetail.InvoiceMoney = entity.InvoiceMoney;
                    by_invoicefordetail.LType = by_checksheet.LType;
                    by_invoicefordetail.CompanyName = by_checksheet.CompanyName;
                    by_invoicefordetail.CheckSheetCode = by_checksheet.CheckSheetCode;
                    by_invoicefordetail.Create();
                    repositoryfactory.Repository().Insert(by_invoicefordetail, isOpenTrans);
                    if (!string.IsNullOrEmpty(by_checksheet.CheckSheetCode))
                    {
                        List<BY_ReimburseSheet> by_reimbursesheets = DataFactory.Database().FindList<BY_ReimburseSheet>("Code", by_checksheet.CheckSheetCode);
                        foreach (BY_ReimburseSheet item in by_reimbursesheets)
                        {
                            item.InvoiceNum = item.InvoiceNum + entity.InvoiceNum;
                            DataFactory.Database().Update(item);
                        }
                    }
                }
                database.Commit();
                List<By_InvoiceForDetail> by_invoicefordetails = DataFactory.Database().FindList<By_InvoiceForDetail>("CheckSheetId", by_checksheet.CheckSheetId);
                by_checksheet.InvoiceMoney = by_invoicefordetails.Sum(p => p.InvoiceMoney);
                DataFactory.Database().Update(by_checksheet, isOpenTrans);

                return Content(new JsonMessage { Success = true, Code = "1", Message = "新增成功。" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
    }
}