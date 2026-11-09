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

namespace DNS.WebApp.Areas.OrderModule.Controllers
{
    /// <summary>
    /// Base_SteelDDPerchase控制器
    /// </summary>
    public class Base_SteelDDPerchaseController : PublicController<Base_SteelDDPerchase>
    {
        [LoginAuthorize]
        public virtual JsonResult GridPageJson1(string ParameterJson, string SteelCode, string Size, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                List<Base_SteelDDPerchase> ListData = new List<Base_SteelDDPerchase>();
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
        public virtual ActionResult Update(Base_SteelDDPerchase entity, int? Status)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                Base_SteelDDPerchase Base_SteelDDPerchase = DataFactory.Database().FindEntity<Base_SteelDDPerchase>(entity.Base_SteelDDPerchaseId);

                if (Base_SteelDDPerchase != null && Base_SteelDDPerchase.Base_SteelDDPerchaseId != null)
                {
                    if (Status == 1)
                    {
                        Base_SteelDDPerchase.Status = Base_SteelDDPerchase.Status == 1 ? 0 : 1;
                        Base_SteelDDPerchase.ComfirmDate = DateTime.Now;
                        Base_SteelDDPerchase.Modify(entity.Base_SteelDDPerchaseId);

                        DataFactory.Database().Update(Base_SteelDDPerchase, isOpenTrans);
                        DataFactory.Database().Commit();

                        return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功:待存仓物料已经入仓库，可在仓库中查询！" }.ToString());

                    }
                    else
                    {
                        entity.Modify(entity.Base_SteelDDPerchaseId);
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
        public virtual ActionResult BeOutPrintList()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult BeOutPrintList1()
        {
            return View();
        }
        public virtual ActionResult BeOutPrintList2()
        {
            return View();
        }
        public ActionResult BeOutListJson(string Base_SteelDDPerchaseId)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {

                IEnumerable<Base_SteelDDPerchaseDetail> Base_SteelDDPerchaseDetails = DataFactory.Database().FindList<Base_SteelDDPerchaseDetail>("Base_SteelDDPerchaseId", Base_SteelDDPerchaseId);
                Base_SteelDDPerchase Base_SteelDDPerchase = DataFactory.Database().FindEntity<Base_SteelDDPerchase>(Base_SteelDDPerchaseId);
                Base_Supplier Base_Supplier = DataFactory.Database().FindEntity<Base_Supplier>(Base_SteelDDPerchase.CustomerId);
                var JsonData = new
                {
                    Base_Supplier = Base_Supplier,
                    Base_SteelDDPerchase = Base_SteelDDPerchase,
                    OrderDetail = Base_SteelDDPerchaseDetails.OrderBy(p => p.SortCode),
                    OrderDetailTotal = Base_SteelDDPerchaseDetails.Sum(p => p.Qty),
                    OrderDetailCost = Base_SteelDDPerchaseDetails.Sum(p => p.CostAmount),
                    WeightAll = Base_SteelDDPerchaseDetails.Sum(p => p.Weight),
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
                    Base_SteelDDPerchase Base_SteelDDPerchase = DataFactory.Database().FindEntity<Base_SteelDDPerchase>(KeyValue);
                    database.Delete(Base_SteelDDPerchase, isOpenTrans);
                    database.Delete<Base_SteelDDPerchaseDetail>("Base_SteelDDPerchaseCode", Base_SteelDDPerchase.Base_SteelDDPerchaseCode, isOpenTrans);
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
        string ModuleId = "CG";
        Base_CodeRuleBll base_coderulebll = new Base_CodeRuleBll();
        private string BillCode()
        {
            string UserId = ManageProvider.Provider.Current().UserId;
            return base_coderulebll.GetBillCode(UserId, ModuleId);
        }
        [ValidateInput(false)]
        public ActionResult SubmitForm1(string KeyValue, Base_SteelDDPerchase entity, string SteelListJson)
        {
            string Base_SteelDDPerchaseCode = BillCode();
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {

                List<Base_SteelDDPerchaseDetail> Base_SteelDDPerchaseDetails = (SteelListJson.JonsToList<Base_SteelDDPerchaseDetail>()).OrderBy(P => P.SortCode).ToList<Base_SteelDDPerchaseDetail>();
                List<Base_SteelDDPerchaseDetail> Base_SteelDDPerchaseDetailsadd = new List<Base_SteelDDPerchaseDetail>();
                if (entity.TypeJC == "进仓")
                {
                    entity.TotalAmount = System.Math.Abs(CommonHelper.GetDecimal(Base_SteelDDPerchaseDetails.Sum(p => p.MaterialAmount)));
                }
                else
                {
                    entity.TotalAmount = -System.Math.Abs(CommonHelper.GetDecimal(Base_SteelDDPerchaseDetails.Sum(p => p.MaterialAmount)));
                }
                
                entity.Qty = Convert.ToInt32(Base_SteelDDPerchaseDetails.Sum(p => p.Qty));
                entity.Weight = Base_SteelDDPerchaseDetails.Sum(p => p.Weight);


                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    entity.Modify(KeyValue);
                    database.Update(entity, isOpenTrans);
                }
                else
                {
                    entity.Create();
                    entity.Base_SteelDDPerchaseCode = Base_SteelDDPerchaseCode;
                    base_coderulebll.OccupyBillCode(ManageProvider.Provider.Current().UserId, ModuleId, isOpenTrans);
                    database.Insert(entity, isOpenTrans);
                }


                Base_SteelDDPerchaseDetail Base_SteelDDPerchaseDetail;
                int rownum = 0;

                foreach (Base_SteelDDPerchaseDetail item in Base_SteelDDPerchaseDetails)
                {
                    Base_SteelDDPerchaseDetail = new Base_SteelDDPerchaseDetail();
                    BY_QuotationDetail by_quotationdetail = DataFactory.Database().FindEntity<BY_QuotationDetail>(item.QuotationDetailId);
                    CommonHelper.CopySameFieldsObject<Base_SteelDDPerchaseDetail, Base_SteelDDPerchaseDetail>(item, ref Base_SteelDDPerchaseDetail);
                    CommonHelper.CopySameFieldsObject<BY_QuotationDetail, Base_SteelDDPerchaseDetail>(by_quotationdetail, ref Base_SteelDDPerchaseDetail);
                    Base_SteelDDPerchaseDetail.Base_SteelDDPerchaseCode = entity.Base_SteelDDPerchaseCode;
                    Base_SteelDDPerchaseDetail.Base_SteelDDPerchaseId = entity.Base_SteelDDPerchaseId;
                    Base_SteelDDPerchaseDetail.SortCode = rownum;
                    Base_SteelDDPerchaseDetail.MaterialPrice = item.MaterialPrice;
                    Base_SteelDDPerchaseDetail.MaterialAmount = item.MaterialAmount;
                    Base_SteelDDPerchaseDetail.WorkCost = item.WorkCost;
                    Base_SteelDDPerchaseDetail.Remark = item.Remark;
                    if (!string.IsNullOrEmpty(item.Fang))
                    {
                        Base_SteelDDPerchaseDetail.Fang = item.Fang;
                    }
                    Base_SteelDDPerchaseDetail.Create();
                    Base_SteelDDPerchaseDetailsadd.Add(Base_SteelDDPerchaseDetail);
                    rownum = rownum + 1;

                    BY_DeliveryDetail by_deliverydetail = DataFactory.Database().FindEntity<BY_DeliveryDetail>("QuotationDetailId", item.QuotationDetailId);
                    if (by_deliverydetail != null && by_deliverydetail.DeliveryDetailId != null)
                    {
                        by_deliverydetail.GWeight = item.Weight;
                        by_deliverydetail.MaterialCost = by_deliverydetail.GWeight * by_deliverydetail.MaterialCostPer;
                        database.Update(by_deliverydetail);
                    }
                }

                database.Delete<Base_SteelDDPerchaseDetail>("Base_SteelDDPerchaseCode", entity.Base_SteelDDPerchaseCode, isOpenTrans);
                database.Insert(Base_SteelDDPerchaseDetailsadd, isOpenTrans);
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