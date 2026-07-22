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
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.PaymentModule.Controllers
{
    /// <summary>
    /// 物料购买申请控制器
    /// </summary>
    public class Base_MaterielPurchaseController : PublicController<Base_MaterielPurchase>
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
                List<Base_MaterielPurchase> base_materielpurchases = DataFactory.Database().FindListBySql<Base_MaterielPurchase>(" select * from  Base_MaterielPurchase where MaterielPurchaseId in ('" + KeyValue.Replace(",", "','") + "')");
                foreach (var base_materielpurchase in base_materielpurchases)
                {
                    IsOk = repositoryfactory.Repository().Delete(base_materielpurchase, isOpenTrans);
                    if (base_materielpurchase.Status == 1)
                    {
                        Base_MaterielStorage base_materielstorage = DataFactory.Database().FindEntity<Base_MaterielStorage>("MaterielId", base_materielpurchase.MaterielId);
                        if (base_materielstorage != null && base_materielstorage.MaterielStorageId != null)
                        {
                            base_materielstorage.Modify(base_materielstorage.MaterielStorageId);
                            base_materielstorage.Qty -= base_materielpurchase.Qty;
                            DataFactory.Database().Update(base_materielstorage, isOpenTrans);
                        }
                    }
                }
                
                if (IsOk > 0)
                {
                    Message = "删除成功。";
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



        public DataTable BuildSummaryTable1(string StartTime, string EndTime)
        {
            
            List<DbParameter> parameter = new List<DbParameter>();
            StringBuilder strSql1 = new StringBuilder();
            strSql1.Append(@" SELECT  
                   sum( TotalAmount ) TotalMoney1
          FROM      Base_PayCompany WHERE 1=1 ");
            
            if (!HashtableHelper.IsNullOrDBNull(StartTime))
            {
                strSql1.Append(" AND CreateDate > @StartTime ");
                
                parameter.Add(DbFactory.CreateDbParameter("@StartTime", CommonHelper.GetDateTime(StartTime + " 00:00")));
            }
            //创建日期
            if (!HashtableHelper.IsNullOrDBNull(EndTime))
            {
                strSql1.Append(" AND CreateDate < @EndTime ");
                
                parameter.Add(DbFactory.CreateDbParameter("@EndTime", CommonHelper.GetDateTime(EndTime + " 23:59")));
            }
            DataTable dt1 = DataFactory.Database().FindTableBySql(strSql1.ToString(), parameter.ToArray());
            return dt1;
        }
        public DataTable BuildSummaryTable2(string StartTime, string EndTime)
        {

            List<DbParameter> parameter = new List<DbParameter>();
            
            StringBuilder strSql2 = new StringBuilder();
            strSql2.Append(@" SELECT  
                   sum( TotalAmount ) TotalMoney2
          FROM      Base_PaySheet WHERE 1=1 ");
            
            if (!HashtableHelper.IsNullOrDBNull(StartTime))
            {
                strSql2.Append(" AND CreateDate > @StartTime ");
                
                parameter.Add(DbFactory.CreateDbParameter("@StartTime", CommonHelper.GetDateTime(StartTime + " 00:00")));
            }
            //创建日期
            if (!HashtableHelper.IsNullOrDBNull(EndTime))
            {
                strSql2.Append(" AND CreateDate < @EndTime ");
                
                parameter.Add(DbFactory.CreateDbParameter("@EndTime", CommonHelper.GetDateTime(EndTime + " 23:59")));
            }
            DataTable dt2 = DataFactory.Database().FindTableBySql(strSql2.ToString(), parameter.ToArray());
            return dt2;
        }
        public DataTable BuildSummaryTable3(string StartTime, string EndTime)
        {

            List<DbParameter> parameter = new List<DbParameter>();
            
            StringBuilder strSql3 = new StringBuilder();
            strSql3.Append(@" SELECT  
                   sum( TotalAmount )  TotalMoney3
          FROM      Base_MaterielPurchase WHERE 1=1 ");
            
            if (!HashtableHelper.IsNullOrDBNull(StartTime))
            {
                strSql3.Append(" AND CreateDate > @StartTime ");
                parameter.Add(DbFactory.CreateDbParameter("@StartTime", CommonHelper.GetDateTime(StartTime + " 00:00")));
            }
            //创建日期
            if (!HashtableHelper.IsNullOrDBNull(EndTime))
            {
                strSql3.Append(" AND CreateDate < @EndTime ");
                parameter.Add(DbFactory.CreateDbParameter("@EndTime", CommonHelper.GetDateTime(EndTime + " 23:59")));
            }
            DataTable dt3 = DataFactory.Database().FindTableBySql(strSql3.ToString(), parameter.ToArray());
            return dt3;
        }
        public DataTable BuildSummaryTable4(string StartTime, string EndTime)
        {

            List<DbParameter> parameter = new List<DbParameter>();
            
            StringBuilder strSql4 = new StringBuilder();
            strSql4.Append(@" SELECT  
                   sum( TotalAmount )  TotalMoney4
          FROM      Base_SteelPurchase WHERE 1=1 ");
            
            if (!HashtableHelper.IsNullOrDBNull(StartTime))
            {
                strSql4.Append(" AND CreateDate > @StartTime ");
                parameter.Add(DbFactory.CreateDbParameter("@StartTime", CommonHelper.GetDateTime(StartTime + " 00:00")));
            }
            //创建日期
            if (!HashtableHelper.IsNullOrDBNull(EndTime))
            {
                strSql4.Append(" AND CreateDate < @EndTime ");
                parameter.Add(DbFactory.CreateDbParameter("@EndTime", CommonHelper.GetDateTime(EndTime + " 23:59")));
            }
            DataTable dt4 = DataFactory.Database().FindTableBySql(strSql4.ToString(), parameter.ToArray());
            return dt4;
        }
        public DataTable BuildSummaryTable5(string StartTime, string EndTime)
        {

            List<DbParameter> parameter = new List<DbParameter>();
           
            StringBuilder strSql5 = new StringBuilder();
            strSql5.Append(@" SELECT  
                   sum( CostAmount )  TotalMoney5
          FROM      BY_OrderDetailBeOut WHERE 1=1 ");
            if (!HashtableHelper.IsNullOrDBNull(StartTime))
            {
                strSql5.Append(" AND CreateDate > @StartTime ");
                parameter.Add(DbFactory.CreateDbParameter("@StartTime", CommonHelper.GetDateTime(StartTime + " 00:00")));
            }
            //创建日期
            if (!HashtableHelper.IsNullOrDBNull(EndTime))
            {
                strSql5.Append(" AND CreateDate < @EndTime ");
                parameter.Add(DbFactory.CreateDbParameter("@EndTime", CommonHelper.GetDateTime(EndTime + " 23:59")));
            }
            DataTable dt5 = DataFactory.Database().FindTableBySql(strSql5.ToString(), parameter.ToArray());
            return dt5;
        }
        public ActionResult BuildSummary(string StartTime, string EndTime)
        {
            try
            {
                DataTable ListData1 = this.BuildSummaryTable1(StartTime, EndTime);
                DataTable ListData2 = this.BuildSummaryTable2(StartTime, EndTime);
                DataTable ListData3 = this.BuildSummaryTable3(StartTime, EndTime);
                DataTable ListData4 = this.BuildSummaryTable4(StartTime, EndTime);
                DataTable ListData5 = this.BuildSummaryTable5(StartTime, EndTime);
                var JsonData = new
                {
                    ListData1 = ListData1.Rows[0]["TotalMoney1"],
                    ListData2 = ListData2.Rows[0]["TotalMoney2"],
                    ListData3 = ListData3.Rows[0]["TotalMoney3"],
                    ListData4 = ListData4.Rows[0]["TotalMoney4"],
                    ListData5 = ListData5.Rows[0]["TotalMoney5"],
                    ListData6 = Convert.ToDecimal(ListData1.Rows[0]["TotalMoney1"])+Convert.ToDecimal(ListData2.Rows[0]["TotalMoney2"])+Convert.ToDecimal(ListData3.Rows[0]["TotalMoney3"])+Convert.ToDecimal(ListData4.Rows[0]["TotalMoney4"])+Convert.ToDecimal(ListData5.Rows[0]["TotalMoney5"])

                };
                return Content(JsonData.ToJson());
            }
            catch (Exception)
            {
                return null;
            }
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult FormSummary()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult Print()
        {
            return View();
        }
        public ActionResult PrintDataJson(string MaterielPurchaseIds)
        {
            try
            {
                Base_Supplier base_supplier = new Entity.Base_Supplier();
                DataSet ds = new  DataSet();
                if (!string.IsNullOrEmpty(MaterielPurchaseIds))
                {
                    string[] materielpurchaseids = MaterielPurchaseIds.Trim(',').Split(',') ;
                    string WhereSql = "";
                    for (int i = 0; i < materielpurchaseids.Length; i++)
                    {
                        WhereSql += "'"+materielpurchaseids[i] + "',";
                    }
                    WhereSql = WhereSql.TrimEnd(',');
                    ds = DataFactory.Database().FindDataSetBySql(@"SELECT  mp.* ,
        s.Fax ,
        s.Sellpeople ,
        s.SellpeoplePhone
FROM    Base_MaterielPurchase mp
        LEFT JOIN Base_Supplier s ON mp.CustomerCode = s.Code
WHERE   mp.MaterielPurchaseId IN (" + WhereSql + ") ");
                }
                if (ds.Tables[0].Rows.Count >0)
                {
                    base_supplier = DataFactory.Database().FindEntityByWhere<Base_Supplier>(" and code like '%" + ds.Tables[0].Rows[0]["CustomerCode"].ToString() + "%' or FullName like '%" + ds.Tables[0].Rows[0]["CustomerCode"].ToString() + "%'");
                }
                var JsonData = new
                {
                    ListData = ds.Tables[0],
                    Base_Supplier = base_supplier,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }

        }

        [LoginAuthorize]
        public virtual ActionResult GridPageJsonCost1(string Condition, JqGridParam jqgridparam)
        {
            try
            {
                Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = new Base_MaterielPurchaseBll().GridPageJsonCost1(QueryParameter, ref jqgridparam);
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
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message + "\r\n条件：" + Condition);
                return null;
            }
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult GridPageJsonCost()
        {
            return View();
        }
        [LoginAuthorize]
        public virtual ActionResult GridPageJsonCostPerYear1(string Year, string Customer,string type ,JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = new Base_MaterielPurchaseBll().GridPageJsonCostPerYear1(Year, Customer,type, ref jqgridparam);
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
                return null;
            }
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult GridPageJsonCostPerYear()
        {
            return View();
        }
        string ModuleId = "P";
        Base_CodeRuleBll base_coderulebll = new Base_CodeRuleBll();
        /// <summary>
        /// 获取自动单据编码
        /// </summary>
        /// <param name="OrderType">加工类型：精、粗</param>
        /// <returns></returns>
        private string BillCode()
        {
            string UserId = ManageProvider.Provider.Current().UserId;
            return base_coderulebll.GetBillCode(UserId, ModuleId);
        }

        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public override ActionResult SubmitForm(Base_MaterielPurchase entity, string KeyValue)
        {
            string MaterielPurchaseCode = BillCode();
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    if (entity.Status == 1)
                    {
                        Base_MaterielPurchase Oldentity = repositoryfactory.Repository().FindEntity(KeyValue);//获取没更新之前实体对象
                        Oldentity.PricePer = entity.PricePer;
                        Oldentity.TotalAmount = entity.TotalAmount;
                        this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                    }
                    else
                    {
                        Base_MaterielPurchase Oldentity = repositoryfactory.Repository().FindEntity(KeyValue);//获取没更新之前实体对象
                        entity.Modify(KeyValue);
                        IsOk = repositoryfactory.Repository().Update(entity);
                        this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                    }
                    
                }
                else
                {
                    entity.Create();
                    entity.MaterielPurchaseCode = MaterielPurchaseCode;
                    IsOk = repositoryfactory.Repository().Insert(entity, isOpenTrans);
                    base_coderulebll.OccupyBillCode(ManageProvider.Provider.Current().UserId, ModuleId, isOpenTrans);
                    database.Commit();
                    this.WriteLog(IsOk, entity, null, KeyValue, Message);
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                this.WriteLog(-1, entity, null, KeyValue, "操作失败：" + ex.Message);
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }


        

        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult Update(Base_MaterielPurchase entity, string MaterielPurchaseIds)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                int IsOk = 0;
                if (!string.IsNullOrEmpty(MaterielPurchaseIds))
                {
                    string[] ids = MaterielPurchaseIds.Split(',');
                    for (int i = 0; i < ids.Length; i++)
                    {
                        Base_MaterielPurchase base_materielpurchase = repositoryfactory.Repository().FindEntity(ids[i]);

                        if (base_materielpurchase.Status != 1 && entity.IsPay == 1)
                        {
                            Base_MaterielStorage base_materielstorage = DataFactory.Database().FindEntity<Base_MaterielStorage>("MaterielId", base_materielpurchase.MaterielId);
                            Base_MaterielType base_materieltype = DataFactory.Database().FindEntity<Base_MaterielType>(base_materielpurchase.MaterielId);
                            if (base_materielstorage != null && base_materielstorage.MaterielStorageId != null)
                            {
                                base_materielstorage.Modify(base_materielstorage.MaterielStorageId);
                                base_materielstorage.Qty += base_materielpurchase.Qty;
                                base_materielstorage.PricePer = base_materielpurchase.PricePer;
                                base_materielstorage.AlarmNumber = base_materieltype.AlarmNumber;
                                DataFactory.Database().Update(base_materielstorage, isOpenTrans);
                            }
                            else
                            {
                                CommonHelper.CopySameFieldsObject<Base_MaterielPurchase, Base_MaterielStorage>(base_materielpurchase, ref base_materielstorage);
                                base_materielstorage.Create();
                                base_materielstorage.PricePer = base_materielpurchase.PricePer;
                                base_materielstorage.AlarmNumber = base_materieltype.AlarmNumber;
                                DataFactory.Database().Insert(base_materielstorage, isOpenTrans);
                            }

                            base_materielpurchase.Status = 1;
                            base_materielpurchase.IsPay = entity.IsPay;
                            DataFactory.Database().Update(base_materielpurchase, isOpenTrans);
                            DataFactory.Database().Commit();
                        }
                        else if (base_materielpurchase.Status == 1 && entity.IsPay == 1)
                        {
                            base_materielpurchase.Status = base_materielpurchase.Status==1?0:1;
                            base_materielpurchase.IsPay = entity.IsPay;
                            DataFactory.Database().Update(base_materielpurchase, isOpenTrans);
                            DataFactory.Database().Commit();
                        }
                        else if (entity.Status == 1 && entity.IsPay != 1)
                        {
                            Base_MaterielStorage base_materielstorage = DataFactory.Database().FindEntity<Base_MaterielStorage>("MaterielId", base_materielpurchase.MaterielId);
                            Base_MaterielType base_materieltype = DataFactory.Database().FindEntity<Base_MaterielType>(base_materielpurchase.MaterielId);
                            if (base_materielstorage != null && base_materielstorage.MaterielStorageId != null)
                            {
                                base_materielstorage.Modify(base_materielstorage.MaterielStorageId);
                                base_materielstorage.Qty += base_materielpurchase.Qty;
                                base_materielstorage.PricePer = base_materielpurchase.PricePer;
                                base_materielstorage.AlarmNumber = base_materieltype.AlarmNumber;
                                DataFactory.Database().Update(base_materielstorage, isOpenTrans);
                            }
                            else
                            {
                                CommonHelper.CopySameFieldsObject<Base_MaterielPurchase, Base_MaterielStorage>(base_materielpurchase, ref base_materielstorage);
                                base_materielstorage.Create();
                                base_materielstorage.PricePer = base_materielpurchase.PricePer;
                                base_materielstorage.AlarmNumber = base_materieltype.AlarmNumber;
                                DataFactory.Database().Insert(base_materielstorage, isOpenTrans);
                            }

                            base_materielpurchase.Status = base_materielpurchase.Status == 1 ? 0 : 1;
                            base_materielpurchase.IsPay = entity.IsPay;
                            DataFactory.Database().Update(base_materielpurchase, isOpenTrans);
                            DataFactory.Database().Commit();
                        }
                        else
                        {
                            entity.Modify(ids[i]);
                            DataFactory.Database().Update(entity, isOpenTrans);
                            DataFactory.Database().Commit();
                        }
                    }
                }
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功！" }.ToString());
            }
            catch (Exception ex)
            {
                DataFactory.Database().Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        [LoginAuthorize]
        public virtual JsonResult GridPageJson1(string Condition, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                List<Base_MaterielPurchase> ListData = new List<Base_MaterielPurchase>();
                if (!string.IsNullOrEmpty(Condition))
                {
                    Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
                    ListData = new Base_MaterielPurchaseBll().GetMaterielStorage(QueryParameter, ref jqgridparam);
                }
                else
                {
                    ListData = repositoryfactory.Repository().FindListPage(ref jqgridparam);
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
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message + "\r\n条件：" + Condition);
                return null;
            }
        }

        
    }
}