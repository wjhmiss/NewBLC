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
    /// 钢材购买申请控制器
    /// </summary>
    public class Base_SteelPurchaseController : PublicController<Base_SteelPurchase>
    {
        public ActionResult saveprice(string KeyValue, Base_SteelPurchase entity, string SteelListJson)
        {
            try
            {
                List<BY_Inventory> by_inventorys = (SteelListJson.JonsToList<BY_Inventory>()).OrderBy(P => P.SortCode).ToList<BY_Inventory>();
                foreach (BY_Inventory item in by_inventorys)
                {

                    BY_Inventory by_inventory = DataFactory.Database().FindEntity<BY_Inventory>(item.InventoryId);
                    by_inventory.BatchCode = item.QuotationCodeLine;
                    by_inventory.StoveCode = item.StoveCode;
                    by_inventory.position = item.position;
                    by_inventory.Modify(by_inventory.InventoryId);
                    DataFactory.Database().Update(by_inventory);
                }
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功!" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：录入的数据有错误，请检查" }.ToString());
            }
        }

        Base_SteelPurchaseBll base_steelpurchasebll = new Base_SteelPurchaseBll();
        public ActionResult GridPageListJson(int MaterialType, string SteelCode, string BatchCode, string Thick, string Weigh, string Length, string alarmck, string SteelPurchaseCode, string CustomerCode, string StartTime, string EndTime, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = base_steelpurchasebll.GetPageList(MaterialType, SteelCode, BatchCode, Thick, Weigh, Length, alarmck, SteelPurchaseCode, CustomerCode, StartTime, EndTime, ref jqgridparam);
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
        [LoginAuthorize]
        public ActionResult GridPageJson0(string ParameterJson, string SteelCode, string Size, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();

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
                    WorkerName ,
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
                    WorkerName ,
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
             SELECT '其他付款' typename ,
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
             FROM   Base_PayCompany
        ) a  where 1=1  ";

                string WhereSql = "";
                if (!string.IsNullOrEmpty(ParameterJson))
                {
                    IList conditions = ParameterJson.JonsToList<Condition>();
                    WhereSql = ConditionBuilder.GetWhereSql(conditions, out parameter);

                }
                DataTable ListData = repositoryfactory.Repository().FindTablePageBySql(sql + WhereSql, parameter.ToArray(), ref jqgridparam);

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
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message + "\r\n条件：" + ParameterJson);
                return null;
            }
        }


        [LoginAuthorize]
        public virtual JsonResult GridPageJson1(string ParameterJson, string SteelCode, string Size, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                List<Base_SteelPurchase> ListData = new List<Base_SteelPurchase>();
                if (!string.IsNullOrEmpty(ParameterJson))
                {
                    List<DbParameter> parameter = new List<DbParameter>();
                    IList conditions = ParameterJson.JonsToList<Condition>();
                    string WhereSql = ConditionBuilder.GetWhereSql(conditions, out parameter);

                    if (!string.IsNullOrEmpty(SteelCode))
                    {
                        WhereSql += @"  and SteelPurchaseCode IN ( SELECT   SteelPurchaseCode
                               FROM     BY_Inventory
                               WHERE    (SteelCode like @SteelCode or SteelDescription like @SteelCode)
                                        AND SteelPurchaseCode != ''
                                        AND SteelPurchaseCode IS NOT NULL )";
                        parameter.Add(DbFactory.CreateDbParameter("@SteelCode", SteelCode + "%"));
                    }
                    if (!string.IsNullOrEmpty(Size))
                    {
                        string[] Sizes = Size.Replace(" ", "").Split('*');
                        if (Size.IndexOf('Φ') > -1)
                        {
                            WhereSql += @" and SteelPurchaseCode IN ( SELECT   SteelPurchaseCode
                               FROM     BY_Inventory de
                               WHERE    SteelPurchaseCode != ''
                                        AND SteelPurchaseCode IS NOT NULL  AND de.Length like @Length AND de.Weigh=0 AND de.Thick like @Thick ) ";
                            parameter.Add(DbFactory.CreateDbParameter("@Length", Sizes[1].Replace("Φ", "") + "%"));
                            parameter.Add(DbFactory.CreateDbParameter("@Thick", Sizes[0].Replace("Φ", "") + "%"));
                        }
                        else
                        {
                            WhereSql += @" and SteelPurchaseCode IN ( SELECT   SteelPurchaseCode
                               FROM     BY_Inventory de
                               WHERE    SteelPurchaseCode != ''
                                         AND ((Length like @Length AND Weigh like @Weigh AND Thick like @Thick) or  
(Length like @Length AND Weigh like @Thick AND Thick like @Weigh) or 

(Length like @Weigh AND Weigh like @Length AND Thick like @Thick) or 

(Length like @Thick AND Weigh like @Weigh AND Thick like @Length) or 

(Length like @Weigh AND Weigh like @Thick AND Thick like @Length) or 

(Length like @Thick AND Weigh like @Weigh AND Thick like @Length)  ) ";
                            parameter.Add(DbFactory.CreateDbParameter("@Thick", Sizes[2] + "%"));
                            parameter.Add(DbFactory.CreateDbParameter("@Weigh", Sizes[1] + "%"));
                            parameter.Add(DbFactory.CreateDbParameter("@Length", Sizes[0] + "%"));
                        }
                    }

                    ListData = repositoryfactory.Repository().FindListPage(WhereSql, parameter.ToArray(), ref jqgridparam);
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
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message + "\r\n条件：" + ParameterJson);
                return null;
            }
        }

        [HttpPost]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult Delete1(string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                var Message = "删除失败。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    Base_SteelPurchase Base_SteelPurchase = DataFactory.Database().FindEntity<Base_SteelPurchase>(KeyValue);
                    database.Delete(Base_SteelPurchase, isOpenTrans);
                    database.Delete<BY_Inventory>("SteelPurchaseCode", Base_SteelPurchase.SteelPurchaseCode, isOpenTrans);
                    database.Delete<BY_InStock>("SteelPurchaseCode", Base_SteelPurchase.SteelPurchaseCode, isOpenTrans);
                }
                database.Commit();
                Message = "删除成功。";
                return Content(new JsonMessage { Success = true, Code = "1".ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        public ActionResult PrintDataJson(string SteelPurchaseId)
        {
            try
            {

                Base_SteelPurchase Base_SteelPurchase = DataFactory.Database().FindEntity<Base_SteelPurchase>(SteelPurchaseId);
                IEnumerable<BY_Inventory> BY_Inventorys = DataFactory.Database().FindList<BY_Inventory>("SteelPurchaseCode", Base_SteelPurchase.SteelPurchaseCode);
                Base_Supplier Base_Supplier = DataFactory.Database().FindEntity<Base_Supplier>("Code", Base_SteelPurchase.CustomerCode);

                var JsonData = new
                {
                    Base_SteelPurchase = Base_SteelPurchase,
                    BY_Inventorys = BY_Inventorys.OrderBy(p => p.SortCode),
                    Base_Supplier = Base_Supplier
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }

        }

        /// <summary>
        /// 保存客户信息
        /// </summary>
        /// <param name="KeyValue">主键</param>
        /// <param name="entity">实体对象</param>
        /// <param name="CustomerAddressListJson">客户地址</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult SubmitForm1(string KeyValue, Base_SteelPurchase entity, string SteelListJson)
        {
            string SteelPurchaseCode = BillCode();
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";

                if (entity.TypeJC == "进仓")
                {
                    entity.TotalAmount = Math.Abs(CommonHelper.GetDecimal(entity.TotalAmount));
                }
                else
                {
                    entity.TotalAmount = -Math.Abs(CommonHelper.GetDecimal(entity.TotalAmount));

                }
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    entity.Modify(KeyValue);
                    database.Update(entity, isOpenTrans);
                }
                else
                {
                    entity.Create();
                    entity.SteelPurchaseCode = SteelPurchaseCode;
                    base_coderulebll.OccupyBillCode(ManageProvider.Provider.Current().UserId, ModuleId, isOpenTrans);
                    database.Insert(entity, isOpenTrans);
                }


                List<BY_Inventory> by_inventorys = (SteelListJson.JonsToList<BY_Inventory>()).OrderBy(P => P.SortCode).ToList<BY_Inventory>();

                if (by_inventorys.Where(p=>string.IsNullOrWhiteSpace(p.QuotationCodeLine)).Count() > 0)
                {
                    database.Rollback();
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：请填写编号！！！" }.ToString());
                }

                List<BY_Inventory> by_inventorysadd = new List<BY_Inventory>();

                BY_Inventory by_inventory;

                foreach (BY_Inventory item in by_inventorys)
                {
                    Base_Steel Base_Steel = DataFactory.Database().FindEntity<Base_Steel>("SteelCode", item.SteelCode);
                    //if (item.Thick == 0)
                    //{
                    //    by_inventory = new BY_Inventory();
                    //    item.SteelId = Base_Steel.SteelId;
                    //    item.SteelCode = Base_Steel.SteelCode;
                    //    item.SteelDescription = Base_Steel.SteelName;
                    //    item.MaterialType = 0;
                    //    item.SteelType = item.Weigh == 0 && item.Length != 0 && item.Thick != 0 ? "R" : "F";
                    //    item.ChildBatch = item.Thick + "X" + item.Weigh + "X" + item.Length;
                    //    item.Enabled = 0;
                    //    item.OriginalSize = item.Thick + "X" + item.Weigh + "X" + item.Length;
                    //    item.CustomerId = entity.CustomerId;
                    //    item.CustomerCode = entity.CustomerCode;
                    //    item.CustomerName = entity.CustomerName;
                    //    item.SteelPurchaseCode = entity.SteelPurchaseCode;
                    //    item.BatchCode = item.QuotationCodeLine;
                    //    CommonHelper.CopySameFieldsObjectALL<BY_Inventory, BY_Inventory>(item, ref by_inventory);
                    //    by_inventory.Create();
                    //    by_inventorysadd.Add(by_inventory);
                    //    continue;
                    //}

                    by_inventory = new BY_Inventory();
                    item.ParentInventoryId = "0";
                    item.SteelId = Base_Steel.SteelId;
                    item.SteelCode = Base_Steel.SteelCode;
                    item.SteelDescription = Base_Steel.SteelName;
                    item.MaterialType = 0;
                    item.SteelType = Base_Steel.SteelType;
                    item.ChildBatch = item.Length + "*" + item.Weigh + "*" + item.Thick;
                    item.Enabled = 0;
                    item.OriginalSize = item.Length + "*" + item.Weigh + "*" + item.Thick;
                    item.CustomerId = entity.CustomerId;
                    item.CustomerCode = entity.CustomerCode;
                    item.CustomerName = entity.CustomerName;
                    item.SteelPurchaseCode = entity.SteelPurchaseCode;
                    item.BatchCode = item.QuotationCodeLine;
                    item.StoveCode = item.BatchCode;
                    item.RemarkSteelperchase = entity.SteelPurchaseCode;
                    CommonHelper.CopySameFieldsObjectALL<BY_Inventory, BY_Inventory>(item, ref by_inventory);
                    by_inventory.Create();
                    by_inventorysadd.Add(by_inventory);
                }

                database.Delete<BY_Inventory>("SteelPurchaseCode", entity.SteelPurchaseCode, isOpenTrans);
                database.Insert(by_inventorysadd, isOpenTrans);

                //foreach (BY_Inventory item in by_inventorysadd)
                //{
                //    BY_AdjustQuery by_adjustquery = CommonHelper.CopySameFieldsObject<BY_AdjustQuery>(item);
                //    by_adjustquery.ParentInventoryId = "0";
                //    database.Insert<BY_AdjustQuery>(by_adjustquery, isOpenTrans);
                //}

                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：录入的数据有错误，请检查" }.ToString());
            }
        }

        public List<BY_Inventory> ReturnCodeNow(List<BY_Inventory> by_inventorysNow, List<BY_Inventory> by_inventorysadd)
        {

            foreach (BY_Inventory itemadd in by_inventorysadd)
            {
                foreach (BY_Inventory itemNow in by_inventorysNow)
                {
                    if (itemadd.Length == itemNow.Length && itemadd.Weigh == itemNow.Weigh && itemadd.Thick == itemNow.Thick && itemadd.BoreSize == itemNow.BoreSize)
                    {
                        if (itemNow.BatchCode != "" && itemadd.SteelCode == itemNow.BatchCode.Split('-')[0])
                        {
                            itemadd.BatchCode = itemNow.BatchCode;
                            itemNow.BatchCode = "";
                            break;
                        }
                    }
                }
            }
            return by_inventorysadd;
        }

        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult Print()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult IndexDetail()
        {
            return View();
        }

        string ModuleId = "S";
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
        public override ActionResult SubmitForm(Base_SteelPurchase entity, string KeyValue)
        {
            string SteelPurchaseCode = BillCode();
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    Base_SteelPurchase Oldentity = repositoryfactory.Repository().FindEntity(KeyValue);//获取没更新之前实体对象
                    entity.Modify(KeyValue);
                    IsOk = repositoryfactory.Repository().Update(entity);
                    this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }
                else
                {
                    entity.Create();
                    entity.SteelPurchaseCode = SteelPurchaseCode;
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
        public virtual ActionResult Update(Base_SteelPurchase entity, int? Status)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                Base_SteelPurchase base_steelpurchase = DataFactory.Database().FindEntity<Base_SteelPurchase>(entity.SteelPurchaseId);

                List<BY_Inventory> by_inventorys = DataFactory.Database().FindList<BY_Inventory>("RemarkSteelperchase", base_steelpurchase.SteelPurchaseCode);
                if (by_inventorys.Where(p => p.ParentInventoryId != "0").Count() > 0 || by_inventorys.Where(p => p.OLength != p.Length).Count() > 0)
                {
                    return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败，请填写钢材编号！！！" }.ToString());
                }

                if (by_inventorys.Where(p => string.IsNullOrWhiteSpace(p.BatchCode)).Count() > 0)
                {
                    database.Rollback();
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：请填写钢材编号！！！" }.ToString());
                }

                DataFactory.Database().Delete<BY_InStock>("SteelPurchaseCode", base_steelpurchase.SteelPurchaseCode, isOpenTrans);
                if (base_steelpurchase != null && base_steelpurchase.SteelPurchaseId != null)
                {
                    if (Status == 1)
                    {
                        base_steelpurchase.Status = base_steelpurchase.Status == 1 ? 0 : 1;
                        base_steelpurchase.ComfirmDate = DateTime.Now;
                        base_steelpurchase.Modify(entity.SteelPurchaseId);

                        
                        foreach (var by_inventory in by_inventorys)
                        {
                            if (by_inventory.Thick == 0)
                            {
                                continue;
                            }
                            //if (by_inventory.Thick == 0 && by_inventory.Length == 0)
                            //{
                            //    DataFactory.Database().Rollback();
                            //    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：部分钢材长度未填写长度！" }.ToString());
                            //}


                            if (base_steelpurchase.Status == 1)
                            {
                                BY_InStock by_instock = new BY_InStock();
                                CommonHelper.CopySameFieldsObject<BY_Inventory, BY_InStock>(by_inventory, ref by_instock);
                                by_instock.BillCode = by_inventory.BatchCode;
                                by_instock.InStockType = "采购入库";
                                by_instock.InStockDate = DateTime.Now;
                                //by_instock.InStockType = 0;
                                by_instock.Create();
                                database.Insert(by_instock, isOpenTrans);
                                by_inventory.InStockDate = DateTime.Now;
                                by_inventory.Enabled = 1;
                            }
                            else
                            {
                                by_inventory.Enabled = 0;
                                by_inventory.InStockDate = Convert.ToDateTime("1991-01-01");
                            }

                            by_inventory.TypeName = "采购入库";
                            by_inventory.RemarkSteelperchase = base_steelpurchase.SteelPurchaseCode;
                            by_inventory.Modify(by_inventory.InventoryId);
                            database.Update(by_inventory, isOpenTrans);

                        }
                        DataFactory.Database().Update(base_steelpurchase, isOpenTrans);
                    }
                    else
                    {
                        entity.Modify(entity.SteelPurchaseId);
                        DataFactory.Database().Update(entity, isOpenTrans);
                    }

                }
                DataFactory.Database().Commit();

                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功！" }.ToString());
            }
            catch (Exception ex)
            {
                DataFactory.Database().Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
    }
}