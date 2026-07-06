using DNS.Business;
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
    /// 报价单回传确认
    /// </summary>
    public class QuotationSureController : PublicController<BY_Quotation>
    {
        Base_CodeRuleBll base_coderulebll = new Base_CodeRuleBll();
        //private string BillCode(string ModuleId)
        //{
        //    string UserId = ManageProvider.Provider.Current().UserId;
        //    return base_coderulebll.GetBillCode(UserId, ModuleId);
        //}
        /// <summary>
        /// 回传确认
        /// </summary>
        /// <param name="KeyValue">主键</param>
        /// <returns></returns>
        public ActionResult SureConfirm(string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(KeyValue);
                if (by_quotation.BillStatus == "已落单")
                {
                    database.Rollback();
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "提交失败：已经提交，不能多次提交！" }.ToString());
                }
                by_quotation.BillStatus = QuotationBillStatus.已落单.ToString();
                by_quotation.CheckStatus = 1;
                by_quotation.CheckDate = DateTime.Now;
                by_quotation.CheckUserId = ManageProvider.Provider.Current().UserName;
                by_quotation.SureDate = DateTime.Now;
                by_quotation.SureUserId = ManageProvider.Provider.Current().UserName;

                //报价单明细列表
                List<BY_QuotationDetail> QuotationDetailList = DataFactory.Database().FindList<BY_QuotationDetail>("QuotationId", KeyValue);
                //订单单号
                //string OrderCode = BillCode("M");
                List<BY_OrderDetail> by_orderdetails = new List<BY_OrderDetail>();

                by_quotation.BillStatus = QuotationBillStatus.已落单.ToString();
                by_quotation.PlaceOrderDate = DateTime.Now;
                by_quotation.PlaceOrderUserId = ManageProvider.Provider.Current().UserName;
                by_quotation.OrderCode = by_quotation.QuotationCode.Substring(2, 4) + "-" + by_quotation.QuotationCode.Substring(9, 4);
                database.Update(by_quotation, isOpenTrans);

                BY_Order by_order = new BY_Order();
                by_order.Create();
                by_order.OrderStatus = OrderBillStatus.正常生产.ToString();
                by_order.QuotationId = by_quotation.QuotationId;
                by_order.CustomerId = by_quotation.CustomerId;
                by_order.CustomerCode = by_quotation.CustomerCode;
                by_order.CustomerName = by_quotation.CustomerName;
                by_order.ShortName = by_quotation.ShortName;
                by_order.ContactName = by_quotation.ContactName;
                by_order.ContactTel = by_quotation.ContactTel;
                by_order.ContactFax = by_quotation.ContactFax;
                by_order.ContactMail = by_quotation.ContactMail;
                by_order.MobileCode = by_quotation.MobileCode;
                by_order.CustomerAddress = by_quotation.CustomerAddress;
                //by_order.OrderType = by_quotation.OrderType;
                by_order.OrderCode = by_quotation.OrderCode;
                by_order.Qty = by_quotation.Qty;
                by_order.Amount = by_quotation.TotalAmount;
                by_order.CustomerPO = by_quotation.CustomerPO;
                by_order.CheckStatus = by_quotation.CheckStatus;
                by_order.CheckUserId = by_quotation.CheckUserId;
                by_order.CheckDate = by_quotation.CheckDate;
                by_order.PlaceOrderDate = DateTime.Now;
                by_order.PlaceOrderUserId = ManageProvider.Provider.Current().UserName;
                by_order.SalesMan = by_quotation.SalesMan;
                by_order.IssuedUserId = ManageProvider.Provider.Current().UserName;
                by_order.IssuedDate = DateTime.Now;
                by_order.MnemonicCode = by_quotation.MnemonicCode;
                by_order.Delivery = by_quotation.Delivery;

                by_order.DepartName = by_quotation.DepartName;
                by_order.KLRequery = by_quotation.KLRequery;
                by_order.XMRequery = by_quotation.XMRequery;
                by_order.QuotationCode = by_quotation.QuotationCode;
                by_order.Remark = by_quotation.Remark;
                by_order.DeleteMark = 0;
                database.Insert(by_order, isOpenTrans);
                //base_coderulebll.OccupyBillCode(ManageProvider.Provider.Current().UserId, "M", isOpenTrans);

                foreach (BY_QuotationDetail QuotationDetail in QuotationDetailList)
                {
                    BY_OrderDetail by_orderdetail = new BY_OrderDetail();
                    //气割，吊孔不生成订单
                    if (QuotationDetail.Length == 0)
                    {
                        by_orderdetail.CompleteQty = QuotationDetail.Qty;
                        by_orderdetail.BillStatus = "已完工";
                    }
                    else
                    {
                        by_orderdetail.BillStatus = "正常生产";
                    }

                    by_orderdetail.Create();
                    by_orderdetail.OrderId = by_order.OrderId;
                    by_orderdetail.OrderCode = by_order.OrderCode;
                    //by_orderdetail.IsChamfer = QuotationDetail.IsChamfer;
                    by_orderdetail.IsGrindAngle = QuotationDetail.IsGrindAngle;
                    by_orderdetail.LineCode = QuotationDetail.LineCode;
                    by_orderdetail.BoardId = QuotationDetail.BoardId;
                    by_orderdetail.BoardDescription = QuotationDetail.BoardDescription;
                    by_orderdetail.Qty = QuotationDetail.Qty;
                    by_orderdetail.SteelId = QuotationDetail.SteelId;
                    by_orderdetail.SteelCode = QuotationDetail.SteelCode;
                    by_orderdetail.SteelDescription = QuotationDetail.SteelDescription;
                    by_orderdetail.SteelOtherDescription = QuotationDetail.SteelOtherDescription;
                    by_orderdetail.SteelState = QuotationDetail.SteelState;
                    by_orderdetail.DieCode = QuotationDetail.DieCode;
                    by_orderdetail.Thick = QuotationDetail.Thick;
                    by_orderdetail.Weigh = QuotationDetail.Weigh;
                    by_orderdetail.Length = QuotationDetail.Length;
                    by_orderdetail.ExecRemark = QuotationDetail.Remark;
                    by_orderdetail.Weight = QuotationDetail.Weight;
                    by_orderdetail.QuotationDetailId = QuotationDetail.QuotationDetailId;
                    by_orderdetail.OrderType = QuotationDetail.OrderType;
                    by_orderdetail.RemarkKL = QuotationDetail.RemarkKL;

                    if (by_quotation.CustMaterial == 1 || QuotationDetail.Length == 0)
                    {
                        by_orderdetail.OpenQty = QuotationDetail.Qty;
                        by_orderdetail.WeightFact = QuotationDetail.Weight;
                    }

                    by_orderdetail.RepalceMl = QuotationDetail.RepalceMl;
                    by_orderdetail.ShipDescription = QuotationDetail.ShipDescription;
                    by_orderdetail.CustomerProduct = QuotationDetail.CustomerProduct;
                    by_orderdetail.ProductName = QuotationDetail.ProductName;
                    by_orderdetail.LiftingHole = QuotationDetail.LiftingHole;
                    by_orderdetail.BoreSize = QuotationDetail.BoreSize;
                    by_orderdetail.SteelType = QuotationDetail.SteelType;

                    by_orderdetail.MandXSteel = QuotationDetail.MandXSteel;
                    by_orderdetail.MorXSteel = QuotationDetail.MorXSteel;
                    by_orderdetail.BSteel = QuotationDetail.BSteel;
                    by_orderdetail.MianCostArea = QuotationDetail.MianCostArea;
                    by_orderdetail.MianCostPer = QuotationDetail.MianCostPer;
                    by_orderdetail.MianCost = QuotationDetail.MianCost;
                    by_orderdetail.BianCostLong = QuotationDetail.BianCostLong;
                    by_orderdetail.BianCostPer = QuotationDetail.BianCostPer;
                    by_orderdetail.BianCost = QuotationDetail.BianCost;
                    by_orderdetail.JKCost = QuotationDetail.JKCost;
                    by_orderdetail.SmallCost = QuotationDetail.SmallCost;
                    by_orderdetail.WorkCostForWork = QuotationDetail.WorkCostForWork;
                    by_orderdetail.AddLength = QuotationDetail.AddLength;
                    by_orderdetail.AddThick = QuotationDetail.AddThick;
                    by_orderdetail.AddWeigh = QuotationDetail.AddWeigh;
                    by_orderdetail.JBSteel = QuotationDetail.JBSteel;
                    by_orderdetail.MianCostUnit = QuotationDetail.MianCostUnit;
                    by_orderdetail.BianCostUnit = QuotationDetail.BianCostUnit;
                    by_orderdetail.WorkUnit = QuotationDetail.WorkUnit;
                    by_orderdetail.XorMSteel = QuotationDetail.XorMSteel;

                    by_orderdetail.MianType = QuotationDetail.MianType;
                    by_orderdetail.BianType = QuotationDetail.BianType;
                    by_orderdetail.CountShow = QuotationDetail.CountShow;

                    by_orderdetail.MianCostWork = QuotationDetail.MianCostWork;
                    by_orderdetail.BianCostWork = QuotationDetail.BianCostWork;

                    by_orderdetail.CNC = QuotationDetail.CNC;
                    by_orderdetail.YM = QuotationDetail.YM;
                    by_orderdetail.JQ = QuotationDetail.JQ;
                    by_orderdetail.Kprocess = QuotationDetail.Kprocess;

                    by_orderdetail.JKCostPer = QuotationDetail.JKCostPer;
                    by_orderdetail.JKCostQty = QuotationDetail.JKCostQty;

                    by_orderdetail.Technology = QuotationDetail.Technology;

                    by_orderdetail.SmallAmount = QuotationDetail.SmallAmount;
                    by_orderdetail.WorkWeight = QuotationDetail.WorkWeight;
                    by_orderdetail.Fang = QuotationDetail.Fang;
                    by_orderdetail.Brand = QuotationDetail.Brand;

                    by_orderdetail.BYDUnit = QuotationDetail.BYDUnit;
                    by_orderdetail.BYDNum = QuotationDetail.BYDNum;
                    by_orderdetail.ProductName1 = QuotationDetail.ProductName1;


                    by_orderdetails.Add(by_orderdetail);
                    database.Insert(by_orderdetail, isOpenTrans);


                    if (by_orderdetail.Length == 0)
                    {
                        BY_OrderComplete entity = DataFactory.Database().FindEntity<BY_OrderComplete>("OrderDetailId", by_orderdetail.OrderDetailId);
                        if (entity != null && entity.OrderCompleteId != null)
                        {
                            DataFactory.Database().Delete(entity, isOpenTrans);
                        }
                        entity.Create();
                        entity.CompleteDate = DateTime.Now;
                        entity.CompleteUserId = ManageProvider.Provider.Current().UserName;
                        entity.OrderDetailId = by_orderdetail.OrderDetailId;
                        entity.OrderId = by_orderdetail.OrderId;
                        entity.Qty = Convert.ToInt32(by_orderdetail.Qty);
                        entity.TheoryWeight = by_orderdetail.Weight;
                        entity.Weight = by_orderdetail.Weight;
                        database.Insert<BY_OrderComplete>(entity, isOpenTrans);
                    }

                    new QuotationPlaceOrderController().GentOrderDetailProcess(by_orderdetail, by_orderdetail.OrderCode, by_quotation, QuotationDetail.WorkAmount);
                }
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "确认成功,此报价单等待落单。" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "确认失败：" + ex.Message }.ToString());
            }
        }
    }
}
