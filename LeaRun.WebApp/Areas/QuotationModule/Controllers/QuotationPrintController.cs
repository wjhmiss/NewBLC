using DNS.Business;
using DNS.Business.Utility.Enum;
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

namespace DNS.WebApp.Areas.QuotationModule.Controllers
{
    /// <summary>
    /// 列印
    /// </summary>
    public class QuotationPrintController : PublicController<BY_Quotation>
    {
        BY_QuotationBll by_quotationbll = new BY_QuotationBll();
        /// <summary>
        /// 状态变更
        /// </summary>
        /// <param name="KeyValue">主键</param>
        /// <returns></returns>
        public ActionResult StateModify(string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                BY_Quotation entity = by_quotationbll.Repository().FindEntity(KeyValue);
                entity.QuotationId = KeyValue;
                if (entity.BillStatus == QuotationBillStatus.已审核.ToString())
                {
                    entity.BillStatus = QuotationBillStatus.已列印.ToString();
                }
                entity.PrintDate = DateTime.Now;
                entity.PrintUserId = ManageProvider.Provider.Current().UserName;
                database.Update(entity, isOpenTrans);

                BY_OpearLog by_opearlog = new BY_OpearLog();
                by_opearlog.Create();
                by_opearlog.BillId = KeyValue;
                by_opearlog.OpearType = entity.BillStatus;
                database.Insert(by_opearlog, isOpenTrans);

                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "变更成功。" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "变更失败：" + ex.Message }.ToString());
            }
        }

        /// <summary>
        /// 报价单列表（返回表格JSON）
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
                List<BY_Quotation> ListData = by_quotationbll.GetQuotationPrintList(QueryParameter, ref jqgridparam);
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
        public ActionResult BJPrintPreviewLJ()
        {
            return View();
        }
        public ActionResult BJPrintPreviewLJ4()
        {
            return View();
        }

        public ActionResult BJPrintPreviewLJ2()
        {
            return View();
        }
        public ActionResult BJPrintPreviewLJ3()
        {
            return View();
        }
        
        public ActionResult BCPrintDataJsonLJ(string QuotationId, int rownum)
        {

            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(QuotationId);
            List<BY_QuotationDetail> quotationdetails = DataFactory.Database().FindList<BY_QuotationDetail>("QuotationId", QuotationId);
            Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>(by_quotation.CustomerId);

            foreach (var item in quotationdetails)
            {
                List<BY_DeliveryDetail> by_deliverydetails = DataFactory.Database().FindList<BY_DeliveryDetail>("QuotationDetailId", item.QuotationDetailId);
                if (by_deliverydetails.Count > 0)
                {
                    item.Qty = by_deliverydetails.Sum(p => p.Qty);
                    item.Weight = by_deliverydetails.Sum(p => p.Weight);
                    item.MaterialPrice = by_deliverydetails[0].MaterialPrice;
                    item.WorkAmount = by_deliverydetails.Sum(p => p.WorkAmount);
                    item.WorkPrice = by_deliverydetails[0].WorkPrice;
                    item.MaterialAmount = by_deliverydetails.Sum(p => p.MaterialAmount);
                    item.SalesAmount = by_deliverydetails.Sum(p => p.SalesAmount);
                }
            }

            List<BY_QuotationDetail> quotationdetailsadd = new List<BY_QuotationDetail>();
            quotationdetailsadd.AddRange(quotationdetails);
            int num3 = 999;

            if (quotationdetailsadd.Count() % rownum != 0 && quotationdetailsadd.Count() < rownum)
            {
                int j = (rownum - quotationdetailsadd.Count % rownum);
                for (int i = 0; i < j; i++)
                {
                    BY_QuotationDetail obj = BY_QuotationDetail.GetNewBY_OrderDetail();
                    obj.LineCode = num3;
                    //quotationdetailsadd.Add(obj);
                }
            }



            //quotationdetailsadd.Add(obj1);
            //quotationdetailsadd = quotationdetailsadd.OrderBy(p => p.LineCode).ToList();

            var grouped = quotationdetailsadd
                .GroupBy(p => new { p.LJtype1, p.LJtype2,p.LJsize })
                .Select(g => new
                {
                    LJtype1 = g.Key.LJtype1,
                    LJtype2 = g.Key.LJtype2,
                    LJsize = g.Key.LJsize,
                    count = g.ToList().Count,
                    QtySum = g.Sum(x => x.Qty),
                    WeightSum = g.Sum(x => x.Weight),
                    SalesAmountSum = g.Sum(x => x.SalesAmount)
                })
                .ToList();
            quotationdetailsadd = quotationdetailsadd.OrderBy(p=>p.LJtype1).ThenBy(p=>p.LJtype2).ThenBy(p => p.LJsize).ToList();

            List<BY_QuotationDetail> quotationdetailsaddok = new List<BY_QuotationDetail>();
            string ljtype12 = "";
            foreach (var item in quotationdetailsadd)
            {
                item.LineCode = item.LineCode * 10;
                if (ljtype12 != item.LJtype1+ item.LJtype2+ item.LJsize)
                {
                    ljtype12 = item.LJtype1 + item.LJtype2 + item.LJsize;
                    BY_QuotationDetail obj = BY_QuotationDetail.GetNewBY_OrderDetail();
                    obj.LJsize = item.LJsize;
                    obj.LJtype1 = item.LJtype1;
                    obj.LJtype2 = item.LJtype2;
                    obj.listcount = grouped.Where(p => p.LJtype1 == item.LJtype1 && p.LJtype2 == item.LJtype2 && p.LJsize == item.LJsize).FirstOrDefault().count;

                    obj.Qty = grouped.Where(p => p.LJtype1 == item.LJtype1 && p.LJtype2 == item.LJtype2 && p.LJsize == item.LJsize).FirstOrDefault().QtySum;
                    obj.Weight = grouped.Where(p => p.LJtype1 == item.LJtype1 && p.LJtype2 == item.LJtype2 && p.LJsize == item.LJsize).FirstOrDefault().WeightSum;
                    obj.SalesAmount = grouped.Where(p => p.LJtype1 == item.LJtype1 && p.LJtype2 == item.LJtype2 && p.LJsize == item.LJsize).FirstOrDefault().SalesAmountSum;
                    obj.LineCode = item.LineCode - 1;
                    obj.SteelCode = "zu";
                    if (item.SteelCode == "FY" || string.IsNullOrWhiteSpace(obj.LJsize))
                    {
                        obj.LJtype1 = item.ProductName;
                    }
                    quotationdetailsaddok.Add(obj);
                }
                //quotationdetailsaddok.Add(item);
            }

            BY_QuotationDetail obj2 = BY_QuotationDetail.GetNewBY_OrderDetail();
            obj2.LJtype1 = "其他费用";
            obj2.listcount = grouped.Where(p => string.IsNullOrWhiteSpace(p.LJtype1)).FirstOrDefault()?.count??0;
            obj2.Qty = grouped.Where(p => string.IsNullOrWhiteSpace(p.LJtype1)).FirstOrDefault()?.QtySum ?? 0;
            obj2.Weight = grouped.Where(p => string.IsNullOrWhiteSpace(p.LJtype1)).FirstOrDefault()?.WeightSum ?? 0;
            obj2.SalesAmount = grouped.Where(p => string.IsNullOrWhiteSpace(p.LJtype1)).FirstOrDefault()?.SalesAmountSum ?? 0;
            obj2.LineCode = quotationdetailsadd.Count * 10 ;
            obj2.SteelCode = "zu";
            if (obj2.Qty > 0 || obj2.Weight > 0 || obj2.SalesAmount > 0)
            {
                quotationdetailsaddok.Add(obj2);
            }


            BY_QuotationDetail obj1 = BY_QuotationDetail.GetNewBY_OrderDetail();
            obj1.LineCode = 99999;
            //obj1.Qty = quotationdetailsaddok.Count;
            obj1.Qty = grouped.Sum(p => p.QtySum);
            obj1.Weight = quotationdetails.Where(p => p.Length > 0).Sum<BY_QuotationDetail>(m => m.Weight);
            obj1.SalesAmount = quotationdetails.Sum<BY_QuotationDetail>(m => m.SalesAmount);
            obj1.WorkAmount = quotationdetails.Sum<BY_QuotationDetail>(m => m.WorkAmount);
            quotationdetailsaddok.Add(obj1);
            Base_Company company = DataFactory.Database().FindEntity<Base_Company>("FullName", string.IsNullOrEmpty(by_quotation.CompanyName) ? "" : by_quotation.CompanyName);

            quotationdetailsaddok = quotationdetailsaddok.OrderBy(p => p.LineCode).ToList();
            var JsonData = new
            {
                Quotation = by_quotation,//报价单主表
                QuotationDetail = quotationdetailsaddok,//报价单明细
                Customer = base_customer,//客户信息
                QtySum = quotationdetails.Sum<BY_QuotationDetail>(m => m.Qty),//总数量
                WeightSum = quotationdetails.Sum<BY_QuotationDetail>(m => m.Weight),//总重量
                //base_customerprocessJ = base_customerprocessJ,
                //base_customerprocessG = base_customerprocessG,
                UpperMaterialAmountSum = RMBHelper.CmycurD(CommonHelper.GetDecimal(obj1.SalesAmount)),//大写总金额
                company = company
            };
            return Content(JsonData.ToJson());
        }


        public ActionResult BCPrintDataJsonLJ2(string QuotationId, int rownum)
        {

            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(QuotationId);
            List<BY_QuotationDetail> quotationdetails = DataFactory.Database().FindList<BY_QuotationDetail>("QuotationId", QuotationId);
            Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>(by_quotation.CustomerId);

            //foreach (var item in quotationdetails)
            //{
            //    List<BY_DeliveryDetail> by_deliverydetails = DataFactory.Database().FindList<BY_DeliveryDetail>("QuotationDetailId", item.QuotationDetailId);
            //    if (by_deliverydetails.Count > 0)
            //    {
            //        item.Qty = by_deliverydetails.Sum(p => p.Qty);
            //        item.Weight = by_deliverydetails.Sum(p => p.Weight);
            //        item.MaterialPrice = by_deliverydetails[0].MaterialPrice;
            //        item.WorkAmount = by_deliverydetails.Sum(p => p.WorkAmount);
            //        item.WorkPrice = by_deliverydetails[0].WorkPrice;
            //        item.MaterialAmount = by_deliverydetails.Sum(p => p.MaterialAmount);
            //        item.SalesAmount = by_deliverydetails.Sum(p => p.SalesAmount);
            //    }
            //}

            List<BY_QuotationDetail> quotationdetailsadd = new List<BY_QuotationDetail>();
            

            BY_QuotationDetail obj1 = BY_QuotationDetail.GetNewBY_OrderDetail();
            obj1.LineCode = 1;
            obj1.Qty = quotationdetails.Where(p => p.Length > 0).Sum<BY_QuotationDetail>(m => m.Qty);
            obj1.Weight = quotationdetails.Where(p => p.Length > 0).Sum<BY_QuotationDetail>(m => m.Weight);
            obj1.SalesAmount = quotationdetails.Sum<BY_QuotationDetail>(m => m.SalesAmount);
            obj1.WorkAmount = quotationdetails.Sum<BY_QuotationDetail>(m => m.WorkAmount);
            obj1.SteelCode = quotationdetails.First().SteelCode;
            obj1.ProductName = quotationdetails.First().ProductName;
            obj1.Delivery = quotationdetails.Max(p=>p.Delivery);



            //quotationdetailsadd.Add(obj1);
            //quotationdetailsadd = quotationdetailsadd.OrderBy(p => p.LineCode).ToList();


            var grouped = quotationdetailsadd
                .GroupBy(p => new { p.LJtype1, p.LJtype2, p.LJsize })
                .Select(g => new
                {
                    LJtype1 = g.Key.LJtype1,
                    LJtype2 = g.Key.LJtype2,
                    LJsize = g.Key.LJsize,
                    count = g.ToList().Count,
                    QtySum = g.Sum(x => x.Qty),
                    WeightSum = g.Sum(x => x.Weight),
                    SalesAmountSum = g.Sum(x => x.SalesAmount)
                })
                .ToList();
            quotationdetailsadd = quotationdetailsadd.OrderBy(p => p.LJtype1).ThenBy(p => p.LJtype2).ThenBy(p => p.LJsize).ToList();

            List<BY_QuotationDetail> quotationdetailsaddok = new List<BY_QuotationDetail>();
            string ljtype12 = "";
            foreach (var item in quotationdetailsadd)
            {
                if (ljtype12 != item.LJtype1 + item.LJtype2 + item.LJsize)
                {
                    ljtype12 = item.LJtype1 + item.LJtype2 + item.LJsize;
                    BY_QuotationDetail obj = BY_QuotationDetail.GetNewBY_OrderDetail();
                    obj.LJsize = item.LJsize;
                    obj.LJtype1 = item.LJtype1;
                    obj.LJtype2 = item.LJtype2;
                    obj.listcount = grouped.Where(p => p.LJtype1 == item.LJtype1 && p.LJtype2 == item.LJtype2 && p.LJsize == item.LJsize).FirstOrDefault().count;

                    obj.Qty = grouped.Where(p => p.LJtype1 == item.LJtype1 && p.LJtype2 == item.LJtype2 && p.LJsize == item.LJsize).FirstOrDefault().QtySum;
                    obj.Weight = grouped.Where(p => p.LJtype1 == item.LJtype1 && p.LJtype2 == item.LJtype2 && p.LJsize == item.LJsize).FirstOrDefault().WeightSum;
                    obj.SalesAmount = grouped.Where(p => p.LJtype1 == item.LJtype1 && p.LJtype2 == item.LJtype2 && p.LJsize == item.LJsize).FirstOrDefault().SalesAmountSum;
                    quotationdetailsaddok.Add(obj);
                }
                quotationdetailsaddok.Add(item);
            }

            quotationdetailsaddok.Add(obj1);

            Base_Company company = DataFactory.Database().FindEntity<Base_Company>("FullName", string.IsNullOrEmpty(by_quotation.CompanyName) ? "" : by_quotation.CompanyName);


            var JsonData = new
            {
                Quotation = by_quotation,//报价单主表
                QuotationDetail = quotationdetailsaddok,//报价单明细
                Customer = base_customer,//客户信息
                QtySum = quotationdetails.Sum<BY_QuotationDetail>(m => m.Qty),//总数量
                WeightSum = quotationdetails.Sum<BY_QuotationDetail>(m => m.Weight),//总重量
                //base_customerprocessJ = base_customerprocessJ,
                //base_customerprocessG = base_customerprocessG,
                UpperMaterialAmountSum = RMBHelper.CmycurD(CommonHelper.GetDecimal(obj1.SalesAmount)),//大写总金额
                company = company
            };
            return Content(JsonData.ToJson());
        }

        public ActionResult BCPrintDataJsonLJ3(string QuotationId, int rownum)
        {

            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(QuotationId);
            List<BY_QuotationDetail> quotationdetails = DataFactory.Database().FindList<BY_QuotationDetail>("QuotationId", QuotationId);
            Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>(by_quotation.CustomerId);

            foreach (var item in quotationdetails)
            {
                //List<BY_DeliveryDetail> by_deliverydetails = DataFactory.Database().FindList<BY_DeliveryDetail>("QuotationDetailId", item.QuotationDetailId);
                //if (by_deliverydetails.Count > 0)
                //{
                //    item.Qty = by_deliverydetails.Sum(p => p.Qty);
                //    item.Weight = by_deliverydetails.Sum(p => p.Weight);
                //    item.MaterialPrice = by_deliverydetails[0].MaterialPrice;
                //    item.WorkAmount = by_deliverydetails.Sum(p => p.WorkAmount);
                //    item.WorkPrice = by_deliverydetails[0].WorkPrice;
                //    item.MaterialAmount = by_deliverydetails.Sum(p => p.MaterialAmount);
                //    item.SalesAmount = by_deliverydetails.Sum(p => p.SalesAmount);
                //}
            }

            List<BY_QuotationDetail> quotationdetailsadd = new List<BY_QuotationDetail>();
            quotationdetailsadd.AddRange(quotationdetails);
            int num3 = 999;

            if (quotationdetailsadd.Count() % rownum != 0 && quotationdetailsadd.Count() < rownum)
            {
                int j = (rownum - quotationdetailsadd.Count % rownum);
                for (int i = 0; i < j; i++)
                {
                    BY_QuotationDetail obj = BY_QuotationDetail.GetNewBY_OrderDetail();
                    obj.LineCode = num3;
                    //quotationdetailsadd.Add(obj);
                }
            }

            BY_QuotationDetail obj1 = BY_QuotationDetail.GetNewBY_OrderDetail();
            obj1.LineCode = 99999;
            obj1.Qty = quotationdetails.Where(p => p.Length > 0).Sum<BY_QuotationDetail>(m => m.Qty);
            obj1.Weight = quotationdetails.Where(p => p.Length > 0).Sum<BY_QuotationDetail>(m => m.Weight);
            obj1.SalesAmount = quotationdetails.Sum<BY_QuotationDetail>(m => m.SalesAmount);
            obj1.WorkAmount = quotationdetails.Sum<BY_QuotationDetail>(m => m.WorkAmount);

            //quotationdetailsadd.Add(obj1);
            //quotationdetailsadd = quotationdetailsadd.OrderBy(p => p.LineCode).ToList();


            var grouped = quotationdetailsadd
                .GroupBy(p => new { p.LJtype1, p.LJtype2, p.LJsize })
                .Select(g => new
                {
                    LJtype1 = g.Key.LJtype1,
                    LJtype2 = g.Key.LJtype2,
                    LJsize = g.Key.LJsize,
                    count = g.ToList().Count,
                    QtySum = g.Sum(x => x.Qty),
                    WeightSum = g.Sum(x => x.Weight),
                    SalesAmountSum = g.Sum(x => x.SalesAmount)
                })
                .ToList();
            quotationdetailsadd = quotationdetailsadd.OrderBy(p => p.LJtype1).ThenBy(p => p.LJtype2).ThenBy(p => p.LJsize).ThenBy(p => p.LineCode).ToList();

            List<BY_QuotationDetail> quotationdetailsaddok = new List<BY_QuotationDetail>();
            string ljtype12 = "";
            foreach (var item in quotationdetailsadd)
            {
                item.LineCode = item.LineCode * 10;
                if (ljtype12 != item.LJtype1 + item.LJtype2 + item.LJsize)
                {
                    ljtype12 = item.LJtype1 + item.LJtype2 + item.LJsize;
                    BY_QuotationDetail obj = BY_QuotationDetail.GetNewBY_OrderDetail();
                    obj.LJsize = item.LJsize;
                    obj.LJtype1 = item.LJtype1;
                    obj.LJtype2 = item.LJtype2;
                    obj.listcount = grouped.Where(p => p.LJtype1 == item.LJtype1 && p.LJtype2 == item.LJtype2 && p.LJsize == item.LJsize).FirstOrDefault().count;

                    obj.Qty = grouped.Where(p => p.LJtype1 == item.LJtype1 && p.LJtype2 == item.LJtype2 && p.LJsize == item.LJsize).FirstOrDefault().QtySum;
                    obj.Weight = grouped.Where(p => p.LJtype1 == item.LJtype1 && p.LJtype2 == item.LJtype2 && p.LJsize == item.LJsize).FirstOrDefault().WeightSum;
                    obj.SalesAmount = grouped.Where(p => p.LJtype1 == item.LJtype1 && p.LJtype2 == item.LJtype2 && p.LJsize == item.LJsize).FirstOrDefault().SalesAmountSum;
                    obj.LineCode = item.LineCode - 1;
                    obj.SteelCode = "zu";
                    quotationdetailsaddok.Add(obj);
                }
                quotationdetailsaddok.Add(item);
            }

            quotationdetailsaddok.Add(obj1);

            quotationdetailsaddok = quotationdetailsaddok.OrderBy(p => p.LineCode).ToList();
            List<BY_OrderDetailProcess> BY_OrderDetailProcesss = new List<BY_OrderDetailProcess>();
            List<TEMPorderdetails> TEMPorderdetailss = new List<TEMPorderdetails>();
            foreach (var item in quotationdetailsaddok)
            {
                
                BY_OrderDetailProcess clfeiyong = new BY_OrderDetailProcess();
                clfeiyong.SalePer = item.MaterialPrice;
                clfeiyong.SaleMc = item.Weight;
                clfeiyong.SaleTotal = item.MaterialAmount;
                clfeiyong.TechnologyName = "材料费用";
                clfeiyong.SortCode = 0;

                TEMPorderdetails temporderdetails = new TEMPorderdetails();
                temporderdetails.by_quotationdetail = item;
                if (!string.IsNullOrWhiteSpace(item.SteelCode))
                {
                    BY_OrderDetailProcesss = DataFactory.Database().FindList<BY_OrderDetailProcess>("QuotationDetailId", item.QuotationDetailId);
                    BY_OrderDetailProcesss.Add(clfeiyong);
                    temporderdetails.by_orderdetailprocesss = BY_OrderDetailProcesss.OrderBy(p => p.SortCode).ToList();
                }
                TEMPorderdetailss.Add(temporderdetails);
            }

            Base_Company company = DataFactory.Database().FindEntity<Base_Company>("FullName", string.IsNullOrEmpty(by_quotation.CompanyName) ? "" : by_quotation.CompanyName);


            var JsonData = new
            {
                Quotation = by_quotation,//报价单主表
                QuotationDetail = TEMPorderdetailss,//报价单明细
                Customer = base_customer,//客户信息
                QtySum = quotationdetails.Sum<BY_QuotationDetail>(m => m.Qty),//总数量
                WeightSum = quotationdetails.Sum<BY_QuotationDetail>(m => m.Weight),//总重量
                //base_customerprocessJ = base_customerprocessJ,
                //base_customerprocessG = base_customerprocessG,
                UpperMaterialAmountSum = RMBHelper.CmycurD(CommonHelper.GetDecimal(obj1.SalesAmount)),//大写总金额
                company = company
            };
            return Content(JsonData.ToJson());
        }


        public class TEMPorderdetails
        {
            public BY_QuotationDetail by_quotationdetail { get; set; }
            public List<BY_OrderDetailProcess> by_orderdetailprocesss { get; set; }
        }

        #region 粗料报价单
        /// <summary>
        /// 粗料报价单打印
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult BCPrint()
        {
            return View();
        }
        /// <summary>
        /// 粗料报价单打印浏览
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult BCPrintPreview()
        {
            return View();
        }
        /// <summary>
        /// 获取要打印数据（返回报价单信息）
        /// </summary>
        /// <param name="QuotationId"></param>
        /// <returns></returns>
        public ActionResult BCPrintDataJson(string QuotationId, int rownum)
        {

            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(QuotationId);
            List<BY_QuotationDetail> quotationdetails = DataFactory.Database().FindList<BY_QuotationDetail>("QuotationId", QuotationId);
            Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>(by_quotation.CustomerId);
            Base_Company company = DataFactory.Database().FindEntity<Base_Company>("FullName", string.IsNullOrEmpty(by_quotation.CompanyName) ? "" : by_quotation.CompanyName);
            foreach (var item in quotationdetails)
            {
                List<BY_DeliveryDetail> by_deliverydetails = DataFactory.Database().FindList<BY_DeliveryDetail>("QuotationDetailId", item.QuotationDetailId);
                if (by_deliverydetails.Count > 0)
                {
                    item.Qty = by_deliverydetails.Sum(p=>p.Qty);
                    item.Weight = by_deliverydetails.Sum(p=>p.Weight);
                    item.MaterialPrice = by_deliverydetails[0].MaterialPrice;
                    item.WorkAmount = by_deliverydetails.Sum(p=>p.WorkAmount);
                    item.WorkPrice = by_deliverydetails[0].WorkPrice;
                    item.MaterialAmount = by_deliverydetails.Sum(p=>p.MaterialAmount);
                    item.SalesAmount = by_deliverydetails.Sum(p=>p.SalesAmount);
                }
            }

            List<BY_QuotationDetail> quotationdetailsadd = new List<BY_QuotationDetail>();
            quotationdetailsadd.AddRange(quotationdetails);
            int num3 = 999;
             
            if (quotationdetailsadd.Count() % rownum != 0 && quotationdetailsadd.Count() < rownum)
            {
                int j = (rownum - quotationdetailsadd.Count % rownum);
                for (int i = 0; i < j; i++)
                {
                    BY_QuotationDetail obj = BY_QuotationDetail.GetNewBY_OrderDetail();
                    obj.LineCode = num3;
                    quotationdetailsadd.Add(obj);
                }
            }

            BY_QuotationDetail obj1 = BY_QuotationDetail.GetNewBY_OrderDetail();
            obj1.LineCode = 99999;
            obj1.Qty = quotationdetails.Where(p => p.Length > 0).Sum<BY_QuotationDetail>(m => m.Qty);
            obj1.Weight = quotationdetails.Where(p => p.Length > 0).Sum<BY_QuotationDetail>(m => m.Weight);
            obj1.SalesAmount = quotationdetails.Sum<BY_QuotationDetail>(m => m.SalesAmount);
            obj1.WorkAmount = quotationdetails.Sum<BY_QuotationDetail>(m => m.WorkAmount);

            quotationdetailsadd.Add(obj1);
            quotationdetailsadd = quotationdetailsadd.OrderBy(p => p.LineCode).ToList();

            Base_CustomerProcess base_customerprocessJ = DataFactory.Database().FindEntityBySql<Base_CustomerProcess>(@"SELECT TOP 1  *
FROM    Base_CustomerProcess
WHERE   WhereQty > 0
        AND JingSteel = 1
        AND CustomerId = '" + base_customer.CustomerId + "'");
            Base_CustomerProcess base_customerprocessG = DataFactory.Database().FindEntityBySql<Base_CustomerProcess>(@"SELECT TOP 1  *
FROM    Base_CustomerProcess
WHERE   WhereQty > 0
        AND GuangSteel = 1
        AND CustomerId = '" + base_customer.CustomerId + "'");
            var JsonData = new
            {
                Quotation = by_quotation,//报价单主表
                QuotationDetail = quotationdetailsadd,//报价单明细
                Customer = base_customer,//客户信息
                QtySum = quotationdetails.Sum<BY_QuotationDetail>(m => m.Qty),//总数量
                WeightSum = quotationdetails.Sum<BY_QuotationDetail>(m => m.Weight),//总重量
                base_customerprocessJ = base_customerprocessJ,
                base_customerprocessG = base_customerprocessG,
                UpperMaterialAmountSum = RMBHelper.CmycurD(CommonHelper.GetDecimal(obj1.SalesAmount)),//大写总金额
                company = company
            };
            return Content(JsonData.ToJson());
        }

        public ActionResult BCPrintDataJson1(string QuotationId)
        {

            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(QuotationId);
            List<BY_QuotationDetail> quotationdetails = DataFactory.Database().FindList<BY_QuotationDetail>("QuotationId", QuotationId);
            Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>(by_quotation.CustomerId);

            List<BY_QuotationDetail> quotationdetailsadd = new List<BY_QuotationDetail>();
            quotationdetailsadd.AddRange(quotationdetails);
            int num3 = 999;
            

            if (quotationdetailsadd.Count() % 22 != 0)
            {
                int j = (22 - quotationdetailsadd.Count % 22);
                for (int i = 0; i < j; i++)
                {
                    BY_QuotationDetail obj = BY_QuotationDetail.GetNewBY_OrderDetail();
                    obj.LineCode = num3;
                    quotationdetailsadd.Add(obj);
                }
            }





            BY_QuotationDetail obj1 = BY_QuotationDetail.GetNewBY_OrderDetail();
            obj1.LineCode = 99999;
            obj1.Qty = quotationdetails.Where(p => p.Length > 0).Sum<BY_QuotationDetail>(m => m.Qty);
            obj1.Weight = quotationdetails.Where(p => p.Length > 0).Sum<BY_QuotationDetail>(m => m.Weight);
            obj1.SalesAmount = quotationdetails.Sum<BY_QuotationDetail>(m => m.SalesAmount);

            quotationdetailsadd.Add(obj1);
            quotationdetailsadd = quotationdetailsadd.OrderBy(p => p.LineCode).ToList();

            var JsonData = new
            {
                Quotation = by_quotation,//报价单主表
                QuotationDetail = quotationdetailsadd,//报价单明细
                Customer = base_customer,//客户信息
                QtySum = quotationdetails.Sum<BY_QuotationDetail>(m => m.Qty),//总数量
                WeightSum = quotationdetails.Sum<BY_QuotationDetail>(m => m.Weight),//总重量
            };
            return Content(JsonData.ToJson());
        }
        #endregion

        #region 精料报价单
        /// <summary>
        /// 精料报价单打印
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult BJPrint()
        {
            return View();
        }
        /// <summary>
        /// 精料报价单打印浏览
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult BJPrintPreview()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult BJPrintPreview1()
        {
            return View();
        }
        #endregion
    }
}
