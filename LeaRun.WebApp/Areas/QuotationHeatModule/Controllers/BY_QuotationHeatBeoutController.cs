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


namespace DNS.WebApp.Areas.QuotationHeatModule.Controllers
{
    /// <summary>
    /// BY_QuotationHeatBeout控制器
    /// </summary>
    public class BY_QuotationHeatBeoutController : PublicController<BY_QuotationHeatBeout>
    {
        [LoginAuthorize]
        public virtual JsonResult GridPageJson1(string ParameterJson, string SteelCode, string Size, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                List<BY_QuotationHeatBeout> ListData = new List<BY_QuotationHeatBeout>();
                if (!string.IsNullOrEmpty(ParameterJson))
                {
                    List<DbParameter> parameter = new List<DbParameter>();
                    IList conditions = ParameterJson.JonsToList<Condition>();
                    string WhereSql = ConditionBuilder.GetWhereSql(conditions, out parameter);
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
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult Update(BY_QuotationHeatBeout entity, int? Status)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                BY_QuotationHeatBeout BY_QuotationHeatBeout = DataFactory.Database().FindEntity<BY_QuotationHeatBeout>(entity.QuotationHeatBeoutId);

                if (BY_QuotationHeatBeout != null && BY_QuotationHeatBeout.QuotationHeatBeoutId != null)
                {
                    if (Status == 1)
                    {
                        BY_QuotationHeatBeout.Status = BY_QuotationHeatBeout.Status == 1 ? 0 : 1;
                        BY_QuotationHeatBeout.ComfirmDate = DateTime.Now;
                        BY_QuotationHeatBeout.Modify(entity.QuotationHeatBeoutId);

                        DataFactory.Database().Update(BY_QuotationHeatBeout, isOpenTrans);
                        DataFactory.Database().Commit();

                        return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功！" }.ToString());

                    }
                    else
                    {
                        entity.Modify(entity.QuotationHeatBeoutId);
                        DataFactory.Database().Update(entity);
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
       
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult BeOutPrint()
        {
            return View();
        }
        
        public ActionResult BeOutListJson(string QuotationHeatBeoutId)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {

                BY_QuotationHeatBeout BY_QuotationHeatBeout = DataFactory.Database().FindEntity<BY_QuotationHeatBeout>(QuotationHeatBeoutId);
                IEnumerable<BY_QuotationHeatBeoutDetail> BY_QuotationHeatBeoutDetails = DataFactory.Database().FindList<BY_QuotationHeatBeoutDetail>("QuotationHeatBeoutCode", BY_QuotationHeatBeout.QuotationHeatBeoutCode);
                Base_Supplier Base_Supplier = DataFactory.Database().FindEntity<Base_Supplier>(BY_QuotationHeatBeout.CustomerId);
                var JsonData = new
                {
                    Base_Supplier = Base_Supplier,
                    BY_QuotationHeatBeout = BY_QuotationHeatBeout,
                    BY_QuotationHeatBeoutDetails = BY_QuotationHeatBeoutDetails.OrderBy(p => p.SortCode),
                    OrderDetailTotal = BY_QuotationHeatBeoutDetails.Sum(p => p.Qty),
                    WeightAll = BY_QuotationHeatBeoutDetails.Sum(p => p.Weight),
                    //Customer = base_customer,
                };

                database.Commit();
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
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
                    BY_QuotationHeatBeout BY_QuotationHeatBeout = DataFactory.Database().FindEntity<BY_QuotationHeatBeout>(KeyValue);
                    database.Delete(BY_QuotationHeatBeout, isOpenTrans);
                    database.Delete<BY_QuotationHeatBeoutDetail>("QuotationHeatBeoutCode", BY_QuotationHeatBeout.QuotationHeatBeoutCode, isOpenTrans);
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
        string ModuleId = "H";
        Base_CodeRuleBll base_coderulebll = new Base_CodeRuleBll();
        private string BillCode()
        {
            string UserId = ManageProvider.Provider.Current().UserId;
            return base_coderulebll.GetBillCode(UserId, ModuleId);
        }
        public ActionResult SubmitForm1(string KeyValue, BY_QuotationHeatBeout entity, string SteelListJson)
        {
            string QuotationHeatBeoutCode = BillCode();
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {

                List<BY_QuotationHeatBeoutDetail> Base_SteelDDPerchaseDetails = (SteelListJson.JonsToList<BY_QuotationHeatBeoutDetail>()).OrderBy(P => P.SortCode).ToList<BY_QuotationHeatBeoutDetail>();
                entity.TotalAmount = Base_SteelDDPerchaseDetails.Sum(P => P.HeatAmount);
                entity.Qty = CommonHelper.GetInt( Base_SteelDDPerchaseDetails.Sum(P => P.Qty));
                entity.Weight = Base_SteelDDPerchaseDetails.Sum(P => P.Weight);
                if (entity.TypeJC == "进仓")
                {
                    entity.TotalAmount = System.Math.Abs(CommonHelper.GetDecimal(entity.TotalAmount));
                }
                else
                {
                    entity.TotalAmount = -System.Math.Abs(CommonHelper.GetDecimal(entity.TotalAmount));
                }

                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    entity.Modify(KeyValue);
                    database.Update(entity, isOpenTrans);
                }
                else
                {
                    entity.Create();
                    entity.QuotationHeatBeoutCode = QuotationHeatBeoutCode;
                    base_coderulebll.OccupyBillCode(ManageProvider.Provider.Current().UserId, ModuleId, isOpenTrans);
                    database.Insert(entity, isOpenTrans);
                }

                foreach (BY_QuotationHeatBeoutDetail item in Base_SteelDDPerchaseDetails)
                {
                    item.QuotationHeatBeoutCode = entity.QuotationHeatBeoutCode;
                    item.Create();
                }

                database.Delete<BY_QuotationHeatBeoutDetail>("QuotationHeatBeoutCode", entity.QuotationHeatBeoutCode, isOpenTrans);
                database.Insert(Base_SteelDDPerchaseDetails, isOpenTrans);
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：录入的数据有错误，请检查" }.ToString());
            }
        }
    }
}