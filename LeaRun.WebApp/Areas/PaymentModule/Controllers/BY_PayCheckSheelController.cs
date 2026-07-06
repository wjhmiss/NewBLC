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

namespace DNS.WebApp.Areas.PaymentModule.Controllers
{
    /// <summary>
    /// 付款对账单控制器
    /// </summary>
    public class BY_PayCheckSheelController : PublicController<BY_PayCheckSheel>
    {

        public ActionResult PrintDataJson(string PayCheckSheetCode)
        {
            try
            {
                List<DbParameter> parameter = new List<DbParameter>();
                string sql = @"SELECT  *
FROM    ( SELECT    '钢材购买' typename ,
                    PayCheckSheetCode ,
                    SteelPurchaseCode codecc ,
                    SteelPurchaseId idid ,
                    CustomerName ,
                    TotalAmount ,
                    GetedAmount ,
                    WorkerName ,
                    CreateDate ,
                    Remark,CustomerId
          FROM      Base_SteelPurchase
          UNION ALL
          SELECT    '物料购买' typename ,
                    PayCheckSheetCode ,
                    MaterielName codecc ,
                    MaterielPurchaseId idid ,
                    CustomerName ,
                    TotalAmount ,
                    GetedAmount ,
                    WorkerName ,
                    CreateDate ,
                    Remark,CustomerId
          FROM      Base_MaterielPurchase
          UNION ALL
          SELECT    '委外加工' typename ,
                    PayCheckSheetCode ,
                    Base_SteelDDBeoutCode codecc ,
                    Base_SteelDDBeoutId idid ,
                    CustomerName ,
                    TotalAmount ,
                    GetedAmount ,
                    CreateUserName ,
                    CreateDate ,
                    Remark,CustomerId
          FROM      Base_SteelDDBeout
          UNION ALL
          SELECT    '委外采购' typename ,
                    PayCheckSheetCode ,
                    Base_SteelDDPerchaseCode codecc ,
                    Base_SteelDDPerchaseId idid ,
                    CustomerName ,
                    TotalAmount ,
                    GetedAmount ,
                    CreateUserName ,
                    CreateDate ,
                    Remark,CustomerId
          FROM      Base_SteelDDPerchase
UNION ALL
            SELECT  '热处理委外' typename ,
                    PayCheckSheetCode ,
                    QuotationHeatBeoutCode codecc ,
                    QuotationHeatBeoutId idid ,
                    CustomerName ,
                    TotalAmount ,
                    GetedAmount ,
                    WorkerName ,
                    CreateDate ,
                    Remark ,
                    CustomerId
            FROM    BY_QuotationHeatBeout
UNION ALL
            SELECT  '其他付款' typename ,
                    PayCheckSheetCode ,
                    PayCompanyCode codecc ,
                    PayCompanyId idid ,
                    CustomerName ,
                    TotalAmount ,
                    GetedAmount ,
                    WorkerName ,
                    CreateDate ,
                    Remark ,
                    CustomerId
            FROM    Base_PayCompany
        ) a  where 1=1 ";

                string WhereSql = " and PayCheckSheetCode = '" + PayCheckSheetCode + "'";
                WhereSql += " order by  CreateDate asc ";
                
                DataTable ListData = DataFactory.Database().FindTableBySql(sql + WhereSql);
                BY_PayCheckSheel BY_PayReimburseSheet = DataFactory.Database().FindEntity<BY_PayCheckSheel>("PayCheckSheetCode", PayCheckSheetCode);
                Base_Supplier Base_Supplier = DataFactory.Database().FindEntity<Base_Supplier>(BY_PayReimburseSheet.CustomerId);

                var JsonData = new
                {
                    CheckEntity = BY_PayReimburseSheet,
                    CheckEntityList = ListData,
                    base_customer = Base_Supplier,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception)
            {
                return null;
            }
        }



        [LoginAuthorize]
        public ActionResult GetGridPageJson(string Condition, JqGridParam jqgridparam)
        {

            BY_DeliveryBll by_deliverybll = new Business.BY_DeliveryBll();
            Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
            DataTable by_orderdetail = by_deliverybll.GetGridPageJsonforPay(QueryParameter, ref jqgridparam);
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

        [HttpPost]
        [ManagerPermission(PermissionMode.Enforce)]
        public override ActionResult Delete(string KeyValue, string ParentId)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                BY_PayCheckSheel by_paychecksheel = database.FindEntity<BY_PayCheckSheel>(KeyValue);
                List<Base_SteelPurchase> base_steelpurchases = database.FindList<Base_SteelPurchase>("PayCheckSheetCode", by_paychecksheel.PayCheckSheetCode);
                foreach (Base_SteelPurchase item in base_steelpurchases)
                {
                    item.Modify(item.SteelPurchaseId);
                    item.PayCheckSheetCode = "";
                    database.Update(item, isOpenTrans);
                }
                List<BY_OrderDetailBeOut> by_orderdetailbeouts = database.FindList<BY_OrderDetailBeOut>("PayCheckSheetCode", by_paychecksheel.PayCheckSheetCode);
                foreach (BY_OrderDetailBeOut item in by_orderdetailbeouts)
                {
                    item.Modify(item.OrderDetailOutId);
                    item.PayCheckSheetCode = "";
                    database.Update(item, isOpenTrans);
                }
                List<Base_MaterielPurchase> base_materielpurchases = database.FindList<Base_MaterielPurchase>("PayCheckSheetCode", by_paychecksheel.PayCheckSheetCode);
                foreach (Base_MaterielPurchase item in base_materielpurchases)
                {
                    item.Modify(item.MaterielPurchaseId);
                    item.PayCheckSheetCode = "";
                    database.Update(item, isOpenTrans);
                }
                List<Base_SteelDDBeout> base_steelddbeouts = database.FindList<Base_SteelDDBeout>("PayCheckSheetCode", by_paychecksheel.PayCheckSheetCode);
                foreach (Base_SteelDDBeout item in base_steelddbeouts)
                {
                    item.Modify(item.Base_SteelDDBeoutId);
                    item.PayCheckSheetCode = "";
                    database.Update(item, isOpenTrans);
                }
                List<Base_SteelDDPerchase> base_steelddperchases = database.FindList<Base_SteelDDPerchase>("PayCheckSheetCode", by_paychecksheel.PayCheckSheetCode);
                foreach (Base_SteelDDPerchase item in base_steelddperchases)
                {
                    item.Modify(item.Base_SteelDDPerchaseId);
                    item.PayCheckSheetCode = "";
                    database.Update(item, isOpenTrans);
                }

                List<BY_QuotationHeatBeout> by_quotationheatbeouts = database.FindList<BY_QuotationHeatBeout>("PayCheckSheetCode", by_paychecksheel.PayCheckSheetCode);
                foreach (BY_QuotationHeatBeout item in by_quotationheatbeouts)
                {
                    item.Modify(item.QuotationHeatBeoutId);
                    item.PayCheckSheetCode = "";
                    database.Update(item, isOpenTrans);
                }
                List<Base_PayCompany> base_paycompanys = database.FindList<Base_PayCompany>("PayCheckSheetCode", by_paychecksheel.PayCheckSheetCode);
                foreach (Base_PayCompany item in base_paycompanys)
                {
                    item.Modify(item.PayCompanyId);
                    item.PayCheckSheetCode = "";
                    database.Update(item, isOpenTrans);
                }
                repositoryfactory.Repository().Delete(by_paychecksheel, isOpenTrans);
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功！" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        public virtual ActionResult BuildCheckSheet1(string Year, string Month, string CustomerId, string code,
          string PayCheckSheetId, string StartTime, string EndTime)
        {
            string Message = string.Empty;
            DateTime starttime = Convert.ToDateTime(StartTime);
            DateTime endtime = Convert.ToDateTime(EndTime);
            //string ModuleId = "PC";
            string YearMonth = Year + "-" + Month;
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            List<DbParameter> parameter = new List<DbParameter>();
            parameter.Add(DbFactory.CreateDbParameter("@CustomerId", CustomerId.Trim()));
            try
            {
                BY_PayCheckSheel by_paychecksheel = new BY_PayCheckSheel();
                if (!string.IsNullOrEmpty(PayCheckSheetId))
                {
                    by_paychecksheel = DataFactory.Database().FindEntity<BY_PayCheckSheel>(PayCheckSheetId);
                    parameter.Add(DbFactory.CreateDbParameter("@PayCheckSheetCode", by_paychecksheel.PayCheckSheetCode));
                }
                by_paychecksheel.BeginDate = starttime;
                by_paychecksheel.EndDate = endtime;
                by_paychecksheel.YearMonth = YearMonth;
                Base_CodeRuleBll base_coderulebll = new Base_CodeRuleBll();
                if (by_paychecksheel == null || by_paychecksheel.PayCheckSheetId == null)
                {
                    by_paychecksheel.PayCheckSheetCode = base_coderulebll.GetBillCode(ManageProvider.Provider.Current().UserId, "PC");
                }
                Base_Supplier base_supplier = DataFactory.Database().FindEntity<Base_Supplier>("SupplierId", CustomerId.Trim());
                Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>("CustomerId", CustomerId.Trim());
                if (base_supplier.SupplierId != null)
                {
                    by_paychecksheel.CustomerId = base_supplier.SupplierId;
                    by_paychecksheel.CustomerCode = base_supplier.Code;
                    by_paychecksheel.CustomerName = base_supplier.FullName;
                }
                else if (base_customer.CustomerId != null)
                {
                    by_paychecksheel.CustomerId = base_customer.CustomerId;
                    by_paychecksheel.CustomerCode = base_customer.Code;
                    by_paychecksheel.CustomerName = base_customer.FullName;
                }
                else
                {
                    database.Rollback();
                    return Content(new JsonMessage { Success = true, Code = "-1", Message = base_supplier.Code + "客户信息不存在！" }.ToString());
                }


                List<Base_SteelPurchase> base_steelpurchases = new List<Base_SteelPurchase>();
                List<Base_MaterielPurchase> base_materielpurchases = new List<Base_MaterielPurchase>();
                List<Base_SteelDDBeout> base_steelddbeouts = new List<Base_SteelDDBeout>();
                List<Base_SteelDDPerchase> Base_SteelDDPerchases = new List<Base_SteelDDPerchase>();
                List<BY_QuotationHeatBeout> BY_QuotationHeatBeouts = new List<BY_QuotationHeatBeout>();
                List<Base_PayCompany> Base_PayCompanys = new List<Base_PayCompany>();


                string[] ids = code.Trim(',').Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    Message = "对账单新增成功！";
                    Base_SteelPurchase base_steelpurchase = database.FindEntity<Base_SteelPurchase>(ids[i].Split(':')[1]);
                    Base_MaterielPurchase base_materielpurchase = database.FindEntity<Base_MaterielPurchase>(ids[i].Split(':')[1]);
                    Base_SteelDDBeout base_steelddbeout = database.FindEntity<Base_SteelDDBeout>(ids[i].Split(':')[1]);
                    Base_SteelDDPerchase base_steelddperchase = database.FindEntity<Base_SteelDDPerchase>(ids[i].Split(':')[1]);
                    BY_QuotationHeatBeout BY_QuotationHeatBeout = database.FindEntity<BY_QuotationHeatBeout>(ids[i].Split(':')[1]);
                    Base_PayCompany Base_PayCompany = database.FindEntity<Base_PayCompany>(ids[i].Split(':')[1]);

                    if (base_steelpurchase.SteelPurchaseId != null)
                    {
                        base_steelpurchases.Add(base_steelpurchase);
                    }
                    if (base_materielpurchase.MaterielPurchaseId != null)
                    {
                        base_materielpurchases.Add(base_materielpurchase);
                    }
                    if (base_steelddbeout.Base_SteelDDBeoutId != null)
                    {
                        base_steelddbeouts.Add(base_steelddbeout);
                    }
                    if (base_steelddperchase.Base_SteelDDPerchaseId != null)
                    {
                        Base_SteelDDPerchases.Add(base_steelddperchase);
                    }
                    if (BY_QuotationHeatBeout.QuotationHeatBeoutId != null)
                    {
                        BY_QuotationHeatBeouts.Add(BY_QuotationHeatBeout);
                    }
                    if (Base_PayCompany.PayCompanyId != null)
                    {
                        Base_PayCompanys.Add(Base_PayCompany);
                    }
                }
                by_paychecksheel.TotalAmount = 0;
                by_paychecksheel.InvoiceMoney = 0;
                by_paychecksheel.GetedAmount = 0;

                by_paychecksheel.TotalAmount = CommonHelper.GetDecimal(by_paychecksheel.TotalAmount) + base_steelpurchases.Sum(p => p.TotalAmount);
                by_paychecksheel.InvoiceMoney = CommonHelper.GetDecimal(by_paychecksheel.InvoiceMoney) + base_steelpurchases.Sum(p => p.TotalAmount);
                by_paychecksheel.GetedAmount = CommonHelper.GetDecimal(by_paychecksheel.GetedAmount) + base_steelpurchases.Sum(p => p.GetedAmount);
                List<Base_SteelPurchase> base_steelpurchasesBefore = new List<Base_SteelPurchase>();
                if (by_paychecksheel != null && by_paychecksheel.PayCheckSheetId != null)
                {
                    base_steelpurchasesBefore = DataFactory.Database().FindListBySql<Base_SteelPurchase>("SELECT * FROM Base_SteelPurchase WHERE PayCheckSheetCode=@PayCheckSheetCode ", parameter.ToArray());
                }
                foreach (Base_SteelPurchase item in base_steelpurchasesBefore)
                {
                    item.IsPay = 0;
                    item.PayCheckSheetCode = "";
                    item.Modify(item.SteelPurchaseId);
                    DataFactory.Database().Update(item, isOpenTrans);
                }
                foreach (Base_SteelPurchase item in base_steelpurchases)
                {
                    item.IsPay = 0;
                    item.PayCheckSheetCode = by_paychecksheel.PayCheckSheetCode;
                    item.Modify(item.SteelPurchaseId);
                    DataFactory.Database().Update(item, isOpenTrans);
                }


                List<Base_MaterielPurchase> base_materielpurchasesBefore = new List<Base_MaterielPurchase>();
                if (by_paychecksheel != null && by_paychecksheel.PayCheckSheetId != null)
                {
                    base_materielpurchasesBefore = DataFactory.Database().FindListBySql<Base_MaterielPurchase>("SELECT * FROM Base_MaterielPurchase WHERE PayCheckSheetCode=@PayCheckSheetCode ", parameter.ToArray());
                }
                by_paychecksheel.TotalAmount = CommonHelper.GetDecimal(by_paychecksheel.TotalAmount) + base_materielpurchases.Sum(p => p.TotalAmount);
                by_paychecksheel.InvoiceMoney = CommonHelper.GetDecimal(by_paychecksheel.InvoiceMoney) + base_materielpurchases.Sum(p => p.TotalAmount);
                by_paychecksheel.GetedAmount = CommonHelper.GetDecimal(by_paychecksheel.GetedAmount) + base_materielpurchases.Sum(p => p.GetedAmount);
                foreach (Base_MaterielPurchase item in base_materielpurchasesBefore)
                {
                    item.IsPay = 0;
                    item.PayCheckSheetCode = "";
                    item.Modify(item.MaterielPurchaseId);
                    DataFactory.Database().Update(item, isOpenTrans);
                }
                foreach (Base_MaterielPurchase item in base_materielpurchases)
                {
                    item.IsPay = 0;
                    item.PayCheckSheetCode = by_paychecksheel.PayCheckSheetCode;
                    item.Modify(item.MaterielPurchaseId);
                    DataFactory.Database().Update(item, isOpenTrans);
                }


                List<Base_SteelDDBeout> Base_SteelDDBeoutsBefore = new List<Base_SteelDDBeout>();
                if (by_paychecksheel != null && by_paychecksheel.PayCheckSheetId != null)
                {
                    Base_SteelDDBeoutsBefore = DataFactory.Database().FindListBySql<Base_SteelDDBeout>("SELECT * FROM Base_SteelDDBeout WHERE PayCheckSheetCode=@PayCheckSheetCode ", parameter.ToArray());
                }


                by_paychecksheel.TotalAmount = CommonHelper.GetDecimal(by_paychecksheel.TotalAmount) + base_steelddbeouts.Sum(p => p.TotalAmount);
                by_paychecksheel.InvoiceMoney = CommonHelper.GetDecimal(by_paychecksheel.InvoiceMoney) + base_steelddbeouts.Sum(p => p.TotalAmount);
                by_paychecksheel.GetedAmount = CommonHelper.GetDecimal(by_paychecksheel.GetedAmount) + base_steelddbeouts.Sum(p => p.GetedAmount);

                foreach (Base_SteelDDBeout item in Base_SteelDDBeoutsBefore)
                {
                    item.IsPay = 0;
                    item.PayCheckSheetCode = "";
                    item.Modify(item.Base_SteelDDBeoutId);
                    DataFactory.Database().Update(item, isOpenTrans);
                }
                foreach (Base_SteelDDBeout item in base_steelddbeouts)
                {
                    item.IsPay = 0;
                    item.PayCheckSheetCode = by_paychecksheel.PayCheckSheetCode;
                    item.Modify(item.Base_SteelDDBeoutId);
                    DataFactory.Database().Update(item, isOpenTrans);
                }

                List<Base_SteelDDPerchase> Base_SteelDDPerchasesBefore = new List<Base_SteelDDPerchase>();
                if (by_paychecksheel != null && by_paychecksheel.PayCheckSheetId != null)
                {
                    Base_SteelDDPerchasesBefore = DataFactory.Database().FindListBySql<Base_SteelDDPerchase>("SELECT * FROM Base_SteelDDPerchase WHERE PayCheckSheetCode=@PayCheckSheetCode ", parameter.ToArray());
                }


                by_paychecksheel.TotalAmount = CommonHelper.GetDecimal(by_paychecksheel.TotalAmount) + Base_SteelDDPerchases.Sum(p => p.TotalAmount);
                by_paychecksheel.InvoiceMoney = CommonHelper.GetDecimal(by_paychecksheel.InvoiceMoney) + Base_SteelDDPerchases.Sum(p => p.TotalAmount);
                by_paychecksheel.GetedAmount = CommonHelper.GetDecimal(by_paychecksheel.GetedAmount) + Base_SteelDDPerchases.Sum(p => p.GetedAmount);

                foreach (Base_SteelDDPerchase item in Base_SteelDDPerchasesBefore)
                {
                    item.IsPay = 0;
                    item.PayCheckSheetCode = "";
                    item.Modify(item.Base_SteelDDPerchaseId);
                    DataFactory.Database().Update(item, isOpenTrans);
                }
                foreach (Base_SteelDDPerchase item in Base_SteelDDPerchases)
                {
                    item.IsPay = 0;
                    item.PayCheckSheetCode = by_paychecksheel.PayCheckSheetCode;
                    item.Modify(item.Base_SteelDDPerchaseId);
                    DataFactory.Database().Update(item, isOpenTrans);
                }


                List<BY_QuotationHeatBeout> BY_QuotationHeatBeoutBefore = new List<BY_QuotationHeatBeout>();
                if (by_paychecksheel != null && by_paychecksheel.PayCheckSheetId != null)
                {
                    BY_QuotationHeatBeoutBefore = DataFactory.Database().FindListBySql<BY_QuotationHeatBeout>("SELECT * FROM BY_QuotationHeatBeout WHERE PayCheckSheetCode=@PayCheckSheetCode ", parameter.ToArray());
                }
                by_paychecksheel.TotalAmount = CommonHelper.GetDecimal(by_paychecksheel.TotalAmount) + BY_QuotationHeatBeouts.Sum(p => p.TotalAmount);
                by_paychecksheel.InvoiceMoney = CommonHelper.GetDecimal(by_paychecksheel.InvoiceMoney) + BY_QuotationHeatBeouts.Sum(p => p.TotalAmount);
                by_paychecksheel.GetedAmount = CommonHelper.GetDecimal(by_paychecksheel.GetedAmount) + BY_QuotationHeatBeouts.Sum(p => p.GetedAmount);
                foreach (BY_QuotationHeatBeout item in BY_QuotationHeatBeoutBefore)
                {
                    item.IsPay = 0;
                    item.PayCheckSheetCode = "";
                    item.Modify(item.QuotationHeatBeoutId);
                    DataFactory.Database().Update(item, isOpenTrans);
                }
                foreach (BY_QuotationHeatBeout item in BY_QuotationHeatBeouts)
                {
                    item.IsPay = 0;
                    item.PayCheckSheetCode = by_paychecksheel.PayCheckSheetCode;
                    item.Modify(item.QuotationHeatBeoutId);
                    DataFactory.Database().Update(item, isOpenTrans);
                }



                List<Base_PayCompany> Base_PayCompanyBefore = new List<Base_PayCompany>();
                if (by_paychecksheel != null && by_paychecksheel.PayCheckSheetId != null)
                {
                    Base_PayCompanyBefore = DataFactory.Database().FindListBySql<Base_PayCompany>("SELECT * FROM Base_PayCompany WHERE PayCheckSheetCode=@PayCheckSheetCode ", parameter.ToArray());
                }
                by_paychecksheel.TotalAmount = CommonHelper.GetDecimal(by_paychecksheel.TotalAmount) + Base_PayCompanys.Sum(p => p.TotalAmount);
                by_paychecksheel.InvoiceMoney = CommonHelper.GetDecimal(by_paychecksheel.InvoiceMoney) + Base_PayCompanys.Sum(p => p.TotalAmount);
                by_paychecksheel.GetedAmount = CommonHelper.GetDecimal(by_paychecksheel.GetedAmount) + Base_PayCompanys.Sum(p => p.GetedAmount);
                foreach (Base_PayCompany item in Base_PayCompanyBefore)
                {
                    item.IsPay = 0;
                    item.PayCheckSheetCode = "";
                    item.Modify(item.PayCompanyId);
                    DataFactory.Database().Update(item, isOpenTrans);
                }
                foreach (Base_PayCompany item in Base_PayCompanys)
                {
                    item.IsPay = 0;
                    item.PayCheckSheetCode = by_paychecksheel.PayCheckSheetCode;
                    item.Modify(item.PayCompanyId);
                    DataFactory.Database().Update(item, isOpenTrans);
                }



                if (by_paychecksheel == null || by_paychecksheel.PayCheckSheetId == null)
                {
                    by_paychecksheel.Create();
                    by_paychecksheel.IsComfirm = 0;
                    base_coderulebll.OccupyBillCode(ManageProvider.Provider.Current().UserId, "PC", isOpenTrans);
                    DataFactory.Database().Insert(by_paychecksheel, isOpenTrans);
                }
                else
                {
                    by_paychecksheel.IsComfirm = 0;
                    by_paychecksheel.Modify(by_paychecksheel.PayCheckSheetId);
                    DataFactory.Database().Update(by_paychecksheel, isOpenTrans);
                }
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                //Base_SysLogBll.Instance.WriteLog("checksheet", OperationType.Other, "-1", "生成对账单失败，" + ex.Message + "。时间：" + DateTime.Now.ToString());
                return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败！" }.ToString());
            }
        }
         
        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult Audit(string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            List<DbParameter> parameter = new List<DbParameter>();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    BY_PayCheckSheel by_paychecksheel = DataFactory.Database().FindEntity<BY_PayCheckSheel>(KeyValue);
                    by_paychecksheel.IsComfirm = by_paychecksheel.IsComfirm == 0 ? 1 : 0;
                    by_paychecksheel.Modify(KeyValue);
                    DataFactory.Database().Update(by_paychecksheel, isOpenTrans);
                    if (by_paychecksheel.PayType == "委外加工")
                    {
                        List<BY_OrderDetailBeOut> by_orderdetailbeouts = new List<BY_OrderDetailBeOut>();
                        if (by_paychecksheel != null && by_paychecksheel.PayCheckSheetId != null)
                        {
                            parameter.Add(DbFactory.CreateDbParameter("@PayCheckSheetCode", by_paychecksheel.PayCheckSheetCode));
                            by_orderdetailbeouts = DataFactory.Database().FindListBySql<BY_OrderDetailBeOut>("SELECT * FROM BY_OrderDetailBeOut WHERE PayCheckSheetCode=@PayCheckSheetCode ", parameter.ToArray());
                            foreach (BY_OrderDetailBeOut item in by_orderdetailbeouts)
                            {
                                item.IsPay = item.IsPay == 1 ? 0 : 1;
                                item.Modify(item.OrderDetailOutId);
                                DataFactory.Database().Update(item, isOpenTrans);
                            }
                        }
                    }
                    if (by_paychecksheel.PayType == "钢材申请单")
                    {
                        List<Base_SteelPurchase> base_steelpurchases = new List<Base_SteelPurchase>();
                        if (by_paychecksheel != null && by_paychecksheel.PayCheckSheetId != null)
                        {
                            parameter.Add(DbFactory.CreateDbParameter("@PayCheckSheetCode", by_paychecksheel.PayCheckSheetCode));
                            base_steelpurchases = DataFactory.Database().FindListBySql<Base_SteelPurchase>("SELECT * FROM Base_SteelPurchase WHERE PayCheckSheetCode=@PayCheckSheetCode ", parameter.ToArray());
                        }
                        foreach (Base_SteelPurchase item in base_steelpurchases)
                        {
                            item.IsPay = item.IsPay == 1 ? 0 : 1;
                            item.Modify(item.SteelPurchaseId);
                            DataFactory.Database().Update(item, isOpenTrans);
                        }
                    }
                }
                DataFactory.Database().Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功！" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult Print()
        {
            return View();
        }
    }
}