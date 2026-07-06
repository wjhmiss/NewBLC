using DNS.Business;
using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using DNS.WebApp.Areas.CommonModule.Controllers;
using DNS.WebApp.Areas.DeliveryModule.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.QuotationHeatModule.Controllers
{
    /// <summary>
    /// BY_QuotationHeat控制器
    /// </summary>
    public class BY_QuotationHeatController : PublicController<BY_QuotationHeat>
    {
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult BJPrintPreview()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult DetailIndex()
        {
            return View();
        }
        public ActionResult BCPrintDataJson(string QuotationHeatIds, int rownum)
        {
            string[] ids = QuotationHeatIds.Split(',');
            string iddss = "";
            for (int i = 0; i < ids.Length; i++)
            {
                iddss += "'" + ids[i] + "',";
            }
            iddss = iddss.Trim(',');
            List<BY_QuotationHeat> by_quotationheats = DataFactory.Database().FindListBySql<BY_QuotationHeat>(" select * from BY_QuotationHeat where QuotationHeatId in (" + iddss + ") ");
            foreach (var item in by_quotationheats)
            {
                string manageSql = string.Format(@"SELECT  *
                                                FROM    Base_CustomerManage
                                                WHERE   (CustomerId = '{0}' )
                                                        AND CraftTypeCode='{1}'  ORDER BY SortCode 
                                                         ", item.CustomerId, item.Technology);
                Base_CustomerManage base_customermanage = DataFactory.Database().FindEntityBySql<Base_CustomerManage>(manageSql);// .FindListBySql<Base_CustomerManage>(manageSql, manageParameter.ToArray());
                if (base_customermanage != null && base_customermanage.CustomerManageId != null)
                {
                    item.HeatPrice = base_customermanage.Price;
                    item.HeatAmount = item.HeatPrice * item.Weight;
                    item.SalesAmount = item.HeatAmount;
                }
            }
            Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>(by_quotationheats[0].CustomerId);

            List<BY_QuotationHeat> BY_QuotationHeatadd = new List<BY_QuotationHeat>();
            BY_QuotationHeatadd.AddRange(by_quotationheats);
            int num3 = 999;

            if (BY_QuotationHeatadd.Count() % rownum != 0 && BY_QuotationHeatadd.Count() < rownum)
            {
                int j = (rownum - BY_QuotationHeatadd.Count % rownum);
                for (int i = 0; i < j; i++)
                {
                    BY_QuotationHeat obj = BY_QuotationHeat.GetNewBY_OrderDetail();
                    obj.QuotationHeatCode = 999999;
                    BY_QuotationHeatadd.Add(obj);
                }
            }

            BY_QuotationHeat obj1 = BY_QuotationHeat.GetNewBY_OrderDetail();
            obj1.QuotationHeatCode = 9999999; ;
            obj1.Qty = by_quotationheats.Sum<BY_QuotationHeat>(m => m.Qty);
            obj1.Weight = by_quotationheats.Sum<BY_QuotationHeat>(m => m.Weight);
            obj1.SalesAmount = by_quotationheats.Sum<BY_QuotationHeat>(m => m.SalesAmount);

            BY_QuotationHeatadd = BY_QuotationHeatadd.OrderByDescending(p => p.QuotationHeatCode).ToList();
            BY_QuotationHeatadd.Add(obj1);

            var JsonData = new
            {
                QuotationDetail = BY_QuotationHeatadd,//报价单明细
                Customer = base_customer,//客户信息
                QtySum = by_quotationheats.Sum<BY_QuotationHeat>(m => m.Qty),//总数量
                WeightSum = by_quotationheats.Sum<BY_QuotationHeat>(m => m.Weight),//总重量
            };
            return Content(JsonData.ToJson());
        }

        public virtual ActionResult Delete1(string KeyValue)
        {
            try
            {
                BY_QuotationHeat BY_QuotationHeat = DataFactory.Database().FindEntity<BY_QuotationHeat>(KeyValue);
                List<BY_StoveHeatList> BY_QuotationHeats = DataFactory.Database().FindList<BY_StoveHeatList>("QuotationHeatCode", BY_QuotationHeat.QuotationHeatCode.ToString());
                if (BY_QuotationHeats.Count > 0)
                {
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：已经进炉子，请先删除进炉信息！"  }.ToString());
                }
                var Message = "删除成功。";
                repositoryfactory.Repository().Delete(KeyValue);
                
                return Content(new JsonMessage { Success = true, Code = "1".ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult Update(BY_QuotationHeat entity, string quotationheatids)
        {
            try
            {
                int IsOk = 0;
                if (!string.IsNullOrEmpty(quotationheatids))
                {
                    string[] ids = quotationheatids.Split(',');
                    for (int i = 0; i < ids.Length; i++)
                    {
                        BY_QuotationHeat Base_PaySheet = DataFactory.Database().FindEntity<BY_QuotationHeat>(ids[i]);
                        if (Base_PaySheet.BillStatus != "已发运")
                        {
                            Base_PaySheet.BillStatus = Base_PaySheet.BillStatus == "检验通过" ? "正常生产" : "检验通过";
                            //Base_PaySheet.BillStatus = "检验通过";
                            Base_PaySheet.Modify(ids[i]);
                            IsOk = DataFactory.Database().Update(Base_PaySheet);
                        }
                    }
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "操作成功！" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }


        public ActionResult GetListJsonhistory(string QuotationHeatCode)
        {
            List<BY_StoveHeatList> ListData = DataFactory.Database().FindListBySql<BY_StoveHeatList>(" select * from BY_StoveHeatList where QuotationHeatCode = '" + QuotationHeatCode + "' ORDER BY SDDateTime ASC");
            return Content(ListData.ToJson());
        }


        public virtual ActionResult SubmitForm2(BY_QuotationHeat entity, string KeyValue, string CreateDate)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                entity.CreateDate = CommonHelper.GetDateTime(CreateDate);
                //entity.imgbase64 = entity.imgbase64.Replace("，",",");
                entity.BillStatus = "正常生产";  //检验通过
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    BY_QuotationHeat Oldentity = repositoryfactory.Repository().FindEntity(KeyValue);//获取没更新之前实体对象
                    entity.Modify(KeyValue);
                    entity.QuotationHeatCode = Oldentity.QuotationHeatCode;
                    IsOk = repositoryfactory.Repository().Update(entity, isOpenTrans);
                    List<BY_DeliveryDetail> by_deliverydetails = DataFactory.Database().FindList<BY_DeliveryDetail>("OrderDetailId", entity.QuotationHeatCode.ToString());
                    if (by_deliverydetails.Count > 0)
                    {
                        DataFactory.Database().Rollback();
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：已发运，请删除送货单！"  }.ToString());
                    }
                    List<BY_StoveHeatList> by_stoveheatlists = DataFactory.Database().FindList<BY_StoveHeatList>("QuotationHeatCode", entity.QuotationHeatCode.ToString());
                    foreach (var item in by_stoveheatlists)
                    {
                        item.SteelCode = entity.SteelCode;
                        item.SteelId = entity.SteelId;
                        item.SteelDescription = entity.SteelDescription;
                        item.ProductName = entity.ProductName;
                        item.Remark = entity.Remark;
                        item.Technology = entity.Technology;
                        item.TechnologQuery = entity.TechnologQuery;
                        item.Qty = entity.Qty;
                        item.Weight = entity.Weight;
                        item.CustomerId = entity.CustomerId;
                        item.CustomerName = entity.CustomerName;
                        item.CustomerCode = entity.CustomerCode;
                        database.Update(item, isOpenTrans);
                    }
                    List<BY_QuotationHeatBeoutDetail> by_quotationheatbeoutdetails = DataFactory.Database().FindList<BY_QuotationHeatBeoutDetail>("QuotationHeatCode", entity.QuotationHeatCode.ToString());
                    foreach (var item in by_quotationheatbeoutdetails)
                    {
                        item.SteelCode = entity.SteelCode;
                        item.SteelId = entity.SteelId;
                        item.SteelDescription = entity.SteelDescription;
                        item.ProductName = entity.ProductName;
                        item.Remark = entity.Remark;
                        item.Technology = entity.Technology;
                        item.TechnologQuery = entity.TechnologQuery;
                        item.Qty = entity.Qty;
                        item.Weight = entity.Weight;
                        item.CustomerId = entity.CustomerId;
                        item.CustomerName = entity.CustomerName;
                        item.CustomerCode = entity.CustomerCode;
                        database.Update(item, isOpenTrans);
                    }
                    //this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }
                else
                {
                    string toto = DateTime.Now.ToString("yyMMdd");
                    long seriousCode = CommonHelper.Getlong(toto + "00001");
                    long QuotationHeatCodemax = CommonHelper.Getlong(DataFactory.Database().FindMax<BY_QuotationHeat>("QuotationHeatCode"));
                    if (QuotationHeatCodemax >= seriousCode)
                    {
                        seriousCode = QuotationHeatCodemax + 1;
                    }

                    entity.Create();
                    entity.QuotationHeatCode = seriousCode;
                    IsOk = repositoryfactory.Repository().Insert(entity, isOpenTrans);
                    //this.WriteLog(IsOk, entity, null, KeyValue, Message);
                }
                DataFactory.Database().Commit();
                return Content(new JsonMessage { Success = true, HeatCode = entity.QuotationHeatCode.ToString(),Code= IsOk.ToString(), Message = Message,Attribute = entity.QuotationHeatId }.ToString());
            }
            catch (Exception ex)
            {
                DataFactory.Database().Rollback();
                this.WriteLog(-1, entity, null, KeyValue, "操作失败：" + ex.Message);
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        public ActionResult GetAutoCompleJson(string QuotationHeatCode)
        {
            List<BY_QuotationHeat> ListData = DataFactory.Database().FindListBySql<BY_QuotationHeat>(" select top 10 * from BY_QuotationHeat where QuotationHeatCode like '%" + QuotationHeatCode + "%'");
            return Content(ListData.ToJson());
        }

        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult Return(string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    BY_Delivery by_delivery = DataFactory.Database().FindEntity<BY_Delivery>(KeyValue);
                    List<BY_DeliveryDetail> by_deliverydetails = DataFactory.Database().FindList<BY_DeliveryDetail>("DeliveryId", by_delivery.DeliveryId);
                    List<BY_QuotationHeat> by_quotationheats = database.FindList<BY_QuotationHeat>(string.Format("  and DeliveryCode = '{0}'", by_delivery.DeliveryCode));
                    if (!string.IsNullOrEmpty(by_delivery.CheckSheetCode))
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：已生成对账单，无法退回！" }.ToString());
                    }
                    if (by_delivery.GetedAmount > 0)
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：已收款，无法退回！" }.ToString());
                    }

                    database.Delete(by_delivery, isOpenTrans);
                    foreach (var item in by_deliverydetails)
                    {
                        database.Delete(item, isOpenTrans);
                    }
                    for (int i = 0; i < by_quotationheats.Count; i++)
                    {
                        by_quotationheats[i].DeliveryCode = "";
                        by_quotationheats[i].BillStatus = "检验通过";
                        by_quotationheats[i].Modify(by_quotationheats[i].QuotationHeatId);
                        database.Update(by_quotationheats[i], isOpenTrans);
                    }

                }
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功！" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        [LoginAuthorize]
        public ActionResult GetGridPageJson(string Condition, JqGridParam jqgridparam)
        {
            Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
            DataTable by_orderdetail = new BY_QuotationHeatBll().GetGridPageJson(QueryParameter, ref jqgridparam);
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

        Base_CodeRuleBll base_coderulebll = new Base_CodeRuleBll();
        private string BillCode(string ModuleId)
        {
            string UserId = ManageProvider.Provider.Current().UserId;
            return base_coderulebll.GetBillCode(UserId, ModuleId);
        }
        [LoginAuthorize]
        public ActionResult SureConfirm(string quotationheatids)
        {
            string ModuleId = "RC";
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                string[] idcs = quotationheatids.Split(',');
                string ids = "";
                for (int i = 0; i < idcs.Length; i++)
                {
                    ids = ids + "'" + idcs[i] + "',";
                }
                ids = ids.TrimEnd(',');
                List<BY_QuotationHeat> by_quotationheats = database.FindList<BY_QuotationHeat>(string.Format("  and quotationheatid in ({0})", ids));
                //2，新增发运单
                BY_Delivery by_delivery = new Entity.BY_Delivery();
                Base_Customer base_customer = new Base_Customer();
                Base_CustomerAddress base_customeraddress = new Base_CustomerAddress();
                Base_CodeRuleBll base_coderulebll = new Base_CodeRuleBll();
                //string ModuleId = DESEncrypt.Decrypt(CookieHelper.GetCookie("ModuleId"));
                by_delivery.DeliveryCode = this.BillCode(ModuleId);
                by_delivery.Create();
                //3，新增发运单详情

                List<BY_DeliveryDetail> by_deliverydetails = new List<BY_DeliveryDetail>();
                BY_DeliveryDetail by_deliverydetail;
                foreach (BY_QuotationHeat item in by_quotationheats)
                {
                    by_deliverydetail = new BY_DeliveryDetail();
                    CommonHelper.CopySameFieldsObject<BY_QuotationHeat, BY_DeliveryDetail>(item, ref by_deliverydetail);
                    if (base_customer == null || base_customer.CustomerId == null)
                    {
                        base_customer = DataFactory.Database().FindEntity<Base_Customer>("Code", item.CustomerCode);
                        base_customeraddress = DataFactory.Database().FindEntity<Base_CustomerAddress>("CustomerId", base_customer.CustomerId);
                        CommonHelper.CopySameFieldsObject<BY_QuotationHeat, BY_DeliveryDetail>(item, ref by_deliverydetail);
                        CommonHelper.CopySameFieldsObject<Base_Customer, BY_Delivery>(base_customer, ref by_delivery);
                        by_delivery.CustomerCode = base_customer.Code;
                        by_delivery.CustomerName = base_customer.FullName;
                        by_delivery.ShipMode = base_customer.TransitMode;
                        by_delivery.SalesMan = base_customer.Sellpeople;
                        by_delivery.CustomerAddress = base_customeraddress.DetailAddress;
                        by_delivery.ShipAddress = base_customeraddress.DetailAddress;
                        by_delivery.ContactName = base_customer.Leader;
                        by_delivery.ContactTel = base_customer.LeaderTele + "" + base_customer.LeaderMobile;
                        by_delivery.PaymentMode = base_customer.PaymentMode;
                        by_delivery.IsInvoice = base_customer.IsInvoice;

                        CommonHelper.CopySameFieldsObject<Base_CustomerAddress, BY_Delivery>(base_customeraddress, ref by_delivery);
                    }
                    by_deliverydetail.Create();


                    Hashtable HashtableData = new Base_SteelPriceBll().GetSteelPrice("", base_customer.CustomerId, item.SteelCode, 0, 0, 0, 0, 0, 0, 0, 0, item.Technology, 0, 0, 0, 0, "");
                    by_deliverydetail.HeatPrice = CommonHelper.GetDecimal(HashtableData["HeatPrice"]);


//                    BY_DeliveryDetail by_deliverydetailprice = DataFactory.Database().FindEntityBySql<BY_DeliveryDetail>(string.Format(@"
//SELECT top 1 dd.*
//FROM    BY_Delivery d ,
//        BY_DeliveryDetail dd
//WHERE   d.DeliveryId = dd.DeliveryId
//        AND d.CustomerId = '{0}'
//        AND dd.SteelCode = '{1}'
//        AND dd.Technology = '{2}'  order by dd.CreateDate desc 
//", item.CustomerId, item.SteelCode, item.Technology));

//                    if (by_deliverydetailprice == null || by_deliverydetailprice.DeliveryDetailId == null)
//                    {
//                        Hashtable HashtableData = new Base_SteelPriceBll().GetSteelPrice("", base_customer.CustomerId, item.SteelCode, 0, 0, 0, 0, 0, 0, 0, 0, item.Technology, 0, 0, 0, 0, "");
//                        by_deliverydetail.HeatPrice = CommonHelper.GetDecimal(HashtableData["HeatPrice"]);
//                    }
//                    else
//                    {
//                        by_deliverydetail.HeatPrice = by_deliverydetailprice.HeatPrice;

//                    }


                    by_deliverydetail.Weight = item.Weight;
                    by_deliverydetail.HeatAmount = Math.Round(Convert.ToDecimal(by_deliverydetail.Weight * by_deliverydetail.HeatPrice), 2, MidpointRounding.AwayFromZero);
                    by_deliverydetail.Remark = item.Remark;
                    by_deliverydetail.ProductName = item.ProductName;
                    by_deliverydetail.Technology = item.Technology;
                    by_deliverydetail.OrderType = "毛料生产";
                    by_deliverydetail.CustomerProduct = item.CustomerProduct;
                    by_deliverydetail.Enabled = 1;
                    by_deliverydetail.BoardDescription = item.BoardDescription;
                    by_deliverydetail.SalesAmount = by_deliverydetail.HeatAmount;
                    by_deliverydetail.HeatCost = 0;
                    by_deliverydetail.HeatCostPer = 0;
                    by_deliverydetail.OrderDetailId = item.QuotationHeatCode.ToString() ;
                    by_deliverydetail.DeliveryCode = by_delivery.DeliveryCode;
                    by_deliverydetail.DeliveryId = by_delivery.DeliveryId;
                    by_deliverydetail.DeliveryCode = by_delivery.DeliveryCode;
                    by_deliverydetail.TechnologQuery = item.TechnologQuery;
                    by_deliverydetail.Qty = item.Qty;

                    item.BillStatus = "已发运";
                    item.DeliveryCode = by_delivery.DeliveryCode;
                    item.Modify(item.QuotationHeatId);
                    database.Update<BY_QuotationHeat>(item, isOpenTrans);
                    by_deliverydetails.Add(by_deliverydetail);
                }

                List<Base_CustomerManage> Base_CustomerManages = database.FindListBySql<Base_CustomerManage>(string.Format(" select * from [dbo].[Base_CustomerManage] where CustomerId='{0}'", base_customer.CustomerId));
                foreach (var item in Base_CustomerManages)
                {
                    var totoamount = by_deliverydetails.Where(p => p.Technology == item.CraftTypeCode).Sum(p => p.SalesAmount);
                    var totoweight = by_deliverydetails.Where(p => p.Technology == item.CraftTypeCode).Sum(p => p.Weight);
                    if (totoamount == null || totoweight == null)
                    {
                        continue;
                    }
                    if (item.HeatLimitMoney > 0 && totoamount < item.HeatLimitMoney)
                    {
                        foreach (var item2 in by_deliverydetails.Where(p => p.Technology == item.CraftTypeCode))
                        {
                            item2.HeatAmount = Math.Round(Convert.ToDecimal(item.HeatLimitMoney * item2.Weight / totoweight), 2, MidpointRounding.AwayFromZero);
                            // Math.Round(Convert.ToDecimal(by_ordercomplete.Weight * by_quotationdetail.HeatPrice), 2, MidpointRounding.AwayFromZero);
                            item2.SalesAmount = item2.HeatAmount;
                            item2.ShipDescription = item2.ShipDescription + "标收";
                        }
                    }
                }

                by_delivery.Qty = by_deliverydetails.Sum(m => m.Qty);

                by_delivery.TotalAmount = by_deliverydetails.Sum<BY_DeliveryDetail>(m => m.SalesAmount);

                by_delivery.Enabled = 1;
                by_delivery.DeleteMark = 0;
                by_delivery.LType = "RCL";
                by_delivery.Weight = by_deliverydetails.Sum<BY_DeliveryDetail>(m => m.Weight);
                database.Insert<BY_DeliveryDetail>(by_deliverydetails, isOpenTrans);
                database.Insert<BY_Delivery>(by_delivery, isOpenTrans);
                base_coderulebll.OccupyBillCode(ManageProvider.Provider.Current().UserId, ModuleId, isOpenTrans);
                database.Commit();

                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功。" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败:" + ex.Message }.ToString());
            }
        }

        public ActionResult GridPageListJson(string Condition, JqGridParam jqgridparam)
        {
            try
            {
                Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = new BY_QuotationHeatBll().GetQueryList(QueryParameter, ref jqgridparam);
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

        [ValidateInput(false)]
        public ActionResult SubmitForm1(string QuotationHeatList)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                string Message = "新增成功。";
                string toto = DateTime.Now.ToString("yyMM");
                string seriousCode = "00001";
                int QuotationHeatCodemax = CommonHelper.GetInt(DataFactory.Database().FindMax<BY_QuotationHeat>("QuotationHeatCode"));
                if (QuotationHeatCodemax < 100000000)
                {
                    seriousCode =  "0";
                }
                else
                {
                    seriousCode = DataFactory.Database().FindMax<BY_QuotationHeat>("QuotationHeatCode").ToString().Substring(4, 5);
                    //流水号+1
                }
                int seritemp = int.Parse(seriousCode);
                List<BY_QuotationHeat> by_quotationheats = (QuotationHeatList.JonsToList<BY_QuotationHeat>()).OrderBy(P => P.SortCode).ToList<BY_QuotationHeat>();
                string cates = "";
                foreach (BY_QuotationHeat item in by_quotationheats)
                {
                    Base_Customer custor = DataFactory.Database().FindEntity<Base_Customer>("ShortName", item.CustomerName);
                    if (custor.Category == "冻结")
                    {
                        cates = cates + custor.FullName + ",";
                        continue;
                    }
                    seriousCode = (seritemp + 1).ToString().PadLeft(5, '0').Substring((seritemp + 1).ToString().PadLeft(5, '0').Length - 5, 5);
                    seritemp = seritemp + 1;
                    item.Create();
                    item.CustomerId = custor.CustomerId;
                    item.CustomerCode = custor.Code;
                    item.CustomerName = custor.ShortName;
                    item.QuotationHeatCode = Convert.ToInt32(toto + seriousCode);
                    if (item.SteelCode == "FY")
                    {
                        item.BillStatus = "检验通过";
                    }
                    else
                    {
                        item.BillStatus = "正常生产";

                    }
                    database.Insert(item, isOpenTrans);
                }
                database.Commit();
                if (cates != "")
                {
                    return Content(new JsonMessage { Success = true, Code = "1", Message = "部分成功：" + cates+"被冻结无法录入！" }.ToString());
                    
                }
                else
                {
                    return Content(new JsonMessage { Success = true, Code = "1", Message = Message }.ToString());
                }
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：录入的数据有错误，请检查"+ ex.Message + ex.StackTrace }.ToString());
            }
        }


        [OutputCache(CacheProfile = "Cache1Hour")]
        public  ActionResult FormAll()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult Complete()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult Delivery()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult Inventory()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult IndexSelect()
        {
            return View();
        }

        
[OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult WorkPrint()
        {
            return View();
        }

        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult Print()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult HeatReport()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult HeatReportList()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult HeatReportPrintCG()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult HeatReportPrintZK()
        {
            return View();
        }
        public ActionResult PrintDataJson(string starttime, string endtime)
        {

            List<BY_QuotationHeat> quotationdetails = DataFactory.Database().FindListBySql<BY_QuotationHeat>(string.Format(@"SELECT  *
FROM    BY_QuotationHeat
WHERE   CreateDate > '{0}'
        AND CreateDate < '{1}' order by QuotationHeatCode DESC
", starttime, endtime));



            

            BY_QuotationHeat obj1 = new BY_QuotationHeat();
            obj1.Technology = "总计";
            obj1.Remark = "9999";
            obj1.Qty = quotationdetails.Sum<BY_QuotationHeat>(m => m.Qty);
            obj1.Weight = quotationdetails.Sum<BY_QuotationHeat>(m => m.Weight);
            quotationdetails.Add(obj1);


            List<BY_QuotationHeat> quotagroups = DataFactory.Database().FindListBySql<BY_QuotationHeat>(string.Format(@"SELECT  Technology
FROM    BY_QuotationHeat
WHERE   CreateDate > '{0}'
        AND CreateDate < '{1}' group by Technology
", starttime, endtime));
            foreach (var item in quotagroups)
            {
                BY_QuotationHeat obj = new BY_QuotationHeat();
                obj.Remark = "99999";
                obj.Technology = item.Technology;
                obj.Qty = quotationdetails.Where(p => p.Technology == item.Technology).Sum<BY_QuotationHeat>(m => m.Qty);
                obj.Weight = quotationdetails.Where(p => p.Technology == item.Technology).Sum<BY_QuotationHeat>(m => m.Weight);
                quotationdetails.Add(obj);
            }
            quotationdetails = quotationdetails.OrderBy(p => p.Technology).ToList();
            var JsonData = new
            {
                BY_QuotationHeats = quotationdetails,//报价单主表
            };
            return Content(JsonData.ToJson());
        }


        public ActionResult PrintDataJsonData(string Condition)
        {
            Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
            List<DbParameter> parameter = new List<DbParameter>();

            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT * FROM BY_QuotationHeat qh
                            WHERE 1=1 ");
            if (!HashtableHelper.IsNullOrDBNull(QueryParameter["QuotationHeatCode"]))
            {
                strSql.Append(" AND qh.QuotationHeatCode LIKE @QuotationHeatCode");
                parameter.Add(DbFactory.CreateDbParameter("@QuotationHeatCode", '%' + QueryParameter["QuotationHeatCode"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(QueryParameter["CustomerCode"]))
            {
                strSql.Append(" AND ( qh.CustomerName LIKE @CustomerCode or qh.CustomerCode LIKE @CustomerCode ) ");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerCode", '%' + QueryParameter["CustomerCode"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(QueryParameter["SteelDescription"]))
            {
                strSql.Append(" AND qh.SteelDescription LIKE @SteelDescription");
                parameter.Add(DbFactory.CreateDbParameter("@SteelDescription", '%' + QueryParameter["SteelDescription"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(QueryParameter["ProductName"]))
            {
                strSql.Append(" AND qh.ProductName LIKE @ProductName");
                parameter.Add(DbFactory.CreateDbParameter("@ProductName", '%' + QueryParameter["ProductName"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(QueryParameter["Remark"]))
            {
                strSql.Append(" AND qh.Remark LIKE @Remark");
                parameter.Add(DbFactory.CreateDbParameter("@Remark", '%' + QueryParameter["Remark"].ToString() + '%'));
            }

            if (!HashtableHelper.IsNullOrDBNull(QueryParameter["Technology"]))
            {
                strSql.Append(" AND qh.Technology = @Technology");
                parameter.Add(DbFactory.CreateDbParameter("@Technology", QueryParameter["Technology"].ToString()));
            }

            if (!HashtableHelper.IsNullOrDBNull(QueryParameter["CreateDate_StartTime"]))
            {
                strSql.Append(" AND qh.CreateDate >= @CreateDate_StartTime  ");
                parameter.Add(DbFactory.CreateDbParameter("@CreateDate_StartTime", CommonHelper.GetDateTime(QueryParameter["CreateDate_StartTime"] + ":00")));
            }
            if (!HashtableHelper.IsNullOrDBNull(QueryParameter["CreateDate_EndTime"]))
            {
                strSql.Append(" AND qh.CreateDate <= @CreateDate_EndTime  ");
                parameter.Add(DbFactory.CreateDbParameter("@CreateDate_EndTime", CommonHelper.GetDateTime(QueryParameter["CreateDate_EndTime"] + ":59")));
            }
            if (!HashtableHelper.IsNullOrDBNull(QueryParameter["BillStatus"]))
            {
                strSql.Append(" AND qh.BillStatus = @BillStatus");
                parameter.Add(DbFactory.CreateDbParameter("@BillStatus", QueryParameter["BillStatus"].ToString()));
            }
            strSql.Append("order by CustomerName,Technology,QuotationHeatCode DESC");
            
            List<BY_QuotationHeat> quotationdetails = DataFactory.Database().FindListBySql<BY_QuotationHeat>(strSql.ToString(), parameter.ToArray());


            List<BY_QuotationHeat> quotationdetailsadd = new List<BY_QuotationHeat>();

            int num3 = 1;
            string customers = "";
            foreach (BY_QuotationHeat item in quotationdetails)
            {
                if (num3 == 1)
                {
                    customers = item.CustomerCode;
                }
                if (item.CustomerCode != customers)
                {
                    int j = (8 - quotationdetailsadd.Count % 8);
                    for (int i = 0; i < j; i++)
                    {
                        quotationdetailsadd.Add(BY_QuotationHeat.GetNewBY_OrderDetail());
                    }
                }
                quotationdetailsadd.Add(item);
                customers = item.CustomerCode;
                if (num3 == quotationdetails.Count() && quotationdetailsadd.Count() % 8 != 0)
                {
                    int j = (8 - quotationdetailsadd.Count % 8);
                    for (int i = 0; i < j; i++)
                    {
                        quotationdetailsadd.Add(BY_QuotationHeat.GetNewBY_OrderDetail());
                    }
                }
                num3 += 1;
            }

            if (quotationdetailsadd.Count() % 8 != 0)
            {
                int j = (8 - quotationdetailsadd.Count % 8);
                for (int i = 0; i < j; i++)
                {
                    quotationdetailsadd.Add(BY_QuotationHeat.GetNewBY_OrderDetail());
                }
            }

            var JsonData = new
            {
                BY_QuotationHeats = quotationdetailsadd,//报价单主表
            };
            return Content(JsonData.ToJson());
        }


        public ActionResult PrintDataJson1(string KeyValue)
        {

            BY_QuotationHeat by_quotationheat = DataFactory.Database().FindEntity<BY_QuotationHeat>(KeyValue);
            List<BY_StoveHeatList> BY_StoveHeatLists = DataFactory.Database().FindList<BY_StoveHeatList>("QuotationHeatCode", by_quotationheat.QuotationHeatCode.ToString()).OrderBy(p => p.SDDateTime).ToList();

            var JsonData = new
            {
                BY_QuotationHeat = by_quotationheat,
                BY_StoveHeatLists = BY_StoveHeatLists,//报价单主表
            };
            return Content(JsonData.ToJson());
        }
    }
}