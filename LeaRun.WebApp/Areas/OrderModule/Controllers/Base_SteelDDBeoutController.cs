using DNS.Business;
using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using DNS.WebApp.Areas.QuotationModule.Controllers;
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
    /// Base_SteelDDBeout控制器
    /// </summary>
    public class Base_SteelDDBeoutController : PublicController<Base_SteelDDBeout>
    {

        public virtual ActionResult BeOutPrintListLJ()
        {
            return View();
        }


        [LoginAuthorize]
        public virtual JsonResult GridPageJson1(string ParameterJson, string SteelCode, string Size, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                List<Base_SteelDDBeout> ListData = new List<Base_SteelDDBeout>();
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
        public virtual ActionResult Update(Base_SteelDDBeout entity, int? Status)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                Base_SteelDDBeout base_steelddbeout = DataFactory.Database().FindEntity<Base_SteelDDBeout>(entity.Base_SteelDDBeoutId);

                if (base_steelddbeout != null && base_steelddbeout.Base_SteelDDBeoutId != null)
                {
                    if (Status == 1)
                    {
                        base_steelddbeout.Status = base_steelddbeout.Status == 1 ? 0 : 1;
                        base_steelddbeout.ComfirmDate = DateTime.Now;
                        base_steelddbeout.Modify(entity.Base_SteelDDBeoutId);

                        DataFactory.Database().Update(base_steelddbeout, isOpenTrans);
                        DataFactory.Database().Commit();

                        return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功:待存仓物料已经入仓库，可在仓库中查询！" }.ToString());

                    }
                    else
                    {
                        entity.Modify(entity.Base_SteelDDBeoutId);
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
        public virtual ActionResult BeOutPrintList()
        {
            return View();
        }
        public ActionResult BeOutListJson(string Base_SteelDDBeoutId)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {

                IEnumerable<Base_SteelDDBeoutDetail> base_steelddbeoutdetails = DataFactory.Database().FindList<Base_SteelDDBeoutDetail>("Base_SteelDDBeoutId", Base_SteelDDBeoutId);
                Base_SteelDDBeout base_steelddbeout = DataFactory.Database().FindEntity<Base_SteelDDBeout>(Base_SteelDDBeoutId);
                Base_Supplier Base_Supplier = DataFactory.Database().FindEntity<Base_Supplier>(base_steelddbeout.CustomerId);
                Base_Customer Base_Customer = DataFactory.Database().FindEntity<Base_Customer>(base_steelddbeout.CustomerId);
                if (Base_Supplier == null || Base_Supplier.SupplierId == null)
                {
                    
                }
                var JsonData = new
                {
                    Base_Supplier = Base_Supplier,
                    base_steelddbeout = base_steelddbeout,
                    OrderDetail = base_steelddbeoutdetails.OrderBy(p => p.SortCode),
                    OrderDetailTotal = base_steelddbeoutdetails.Sum(p => p.Qty),
                    OrderDetailCost = base_steelddbeoutdetails.Sum(p => p.CostAmount),
                    WeightAll = base_steelddbeoutdetails.Sum(p => p.Weight),
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

        public ActionResult BeOutListJsonLJ(string Base_SteelDDBeoutId, string imgs)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                //img： base_files id  ;  by_quotation.ShortName + "：" + item.QuotationCode + "-" + item.LineCode + "*" + item.Base_SteelDDBeoutDetailId  
                List<string> ls = imgs.Split('!').Where(p => !string.IsNullOrEmpty(p)).ToList();
                List<Base_SteelDDBeoutDetail> base_steelddbeoutdetails = DataFactory.Database().FindList<Base_SteelDDBeoutDetail>("Base_SteelDDBeoutId", Base_SteelDDBeoutId);
                Base_SteelDDBeout base_steelddbeout = DataFactory.Database().FindEntity<Base_SteelDDBeout>(Base_SteelDDBeoutId);
                Base_Supplier Base_Supplier = DataFactory.Database().FindEntity<Base_Supplier>(base_steelddbeout.CustomerId);
                Base_Customer Base_Customer = DataFactory.Database().FindEntity<Base_Customer>(base_steelddbeout.CustomerId);

                for (int i = 0; i < base_steelddbeoutdetails.Count(); i++)
                {
                    base_steelddbeoutdetails[i].CustomerOrder = "";
                    foreach (string item2 in ls)
                    {
                        if (item2.Split(';')[1] == base_steelddbeoutdetails[i].Base_SteelDDBeoutDetailId)
                        {
                            base_file base_file = DataFactory.Database().FindEntity<base_file>(item2.Split(';')[0]);
                            base_steelddbeoutdetails[i].CustomerOrder += base_file.Status;
                        }
                    }
                }
                var JsonData = new
                {
                    Base_Supplier = Base_Supplier,
                    base_steelddbeout = base_steelddbeout,
                    OrderDetail = base_steelddbeoutdetails.OrderBy(p => p.SortCode),
                    OrderDetailTotal = base_steelddbeoutdetails.Sum(p => p.Qty),
                    OrderDetailCost = base_steelddbeoutdetails.Sum(p => p.CostAmount),
                    WeightAll = base_steelddbeoutdetails.Sum(p => p.Weight),
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
                    Base_SteelDDBeout Base_SteelDDBeout = DataFactory.Database().FindEntity<Base_SteelDDBeout>(KeyValue);
                    database.Delete(Base_SteelDDBeout, isOpenTrans);
                    database.Delete<Base_SteelDDBeoutDetail>("Base_SteelDDBeoutCode", Base_SteelDDBeout.Base_SteelDDBeoutCode, isOpenTrans);
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
        string ModuleId = "WF";
        Base_CodeRuleBll base_coderulebll = new Base_CodeRuleBll();
        private string BillCode()
        {
            string UserId = ManageProvider.Provider.Current().UserId;
            return base_coderulebll.GetBillCode(UserId, ModuleId);
        }
        [ValidateInput(false)]
        public ActionResult SubmitForm1(string KeyValue, Base_SteelDDBeout entity, string SteelListJson)
        {
            string Base_SteelDDBeoutCode = BillCode();
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                List<Base_SteelDDBeoutDetail> base_steelddbeoutdetails = (SteelListJson.JonsToList<Base_SteelDDBeoutDetail>()).OrderBy(P => P.SortCode).ToList<Base_SteelDDBeoutDetail>();
                List<Base_SteelDDBeoutDetail> base_steelddbeoutdetailsadd = new List<Base_SteelDDBeoutDetail>();
              
                entity.Qty = Convert.ToInt32(base_steelddbeoutdetails.Sum(p => p.Qty));

                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    entity.Modify(KeyValue);
                }
                else
                {
                    entity.Create();
                    entity.Base_SteelDDBeoutCode = Base_SteelDDBeoutCode;
                    base_coderulebll.OccupyBillCode(ManageProvider.Provider.Current().UserId, ModuleId, isOpenTrans);
                }
                
                Base_SteelDDBeoutDetail base_steelddbeoutdetail;
                Base_Supplier Base_Supplier = DataFactory.Database().FindEntity<Base_Supplier>(entity.CustomerId);
                int rownum = 0;
                BY_Order BY_Order = new BY_Order();
                foreach (Base_SteelDDBeoutDetail item in base_steelddbeoutdetails)
                {
                    base_steelddbeoutdetail = new Base_SteelDDBeoutDetail();
                    BY_QuotationDetail by_quotationdetail = DataFactory.Database().FindEntity<BY_QuotationDetail>(item.QuotationDetailId);
                    CommonHelper.CopySameFieldsObject<Base_SteelDDBeoutDetail, Base_SteelDDBeoutDetail>(item, ref base_steelddbeoutdetail);
                    CommonHelper.CopySameFieldsObject<BY_QuotationDetail, Base_SteelDDBeoutDetail>(by_quotationdetail, ref base_steelddbeoutdetail);
                    base_steelddbeoutdetail.Base_SteelDDBeoutCode = entity.Base_SteelDDBeoutCode;
                    base_steelddbeoutdetail.Base_SteelDDBeoutId = entity.Base_SteelDDBeoutId;
                    base_steelddbeoutdetail.MaterialPrice = item.MaterialPrice;
                    base_steelddbeoutdetail.MaterialAmount = item.MaterialAmount;
                    base_steelddbeoutdetail.Remark = item.Remark;
                    base_steelddbeoutdetail.SortCode = rownum;
                    base_steelddbeoutdetail.Create();
                    decimal? Area = new QuotationInputController().ReturnArea(base_steelddbeoutdetail.Thick, base_steelddbeoutdetail.Weigh, base_steelddbeoutdetail.Length, base_steelddbeoutdetail.OrderType);
                    base_steelddbeoutdetail.Area = Area * base_steelddbeoutdetail.Qty;
                    decimal MaterialPrice = 0;
                    decimal dcost = 0;
                    if (base_steelddbeoutdetail.MaterialAmount == 0 || base_steelddbeoutdetail.MaterialAmount == null)
                    {
                        //base_steelddbeoutdetail.MaterialAmount = new QuotationInputController().ReturnWorkCost(CommonHelper.GetDecimal(Area), CommonHelper.GetDecimal(base_steelddbeoutdetail.Weigh), base_steelddbeoutdetail.SteelType, base_steelddbeoutdetail.OrderType, Base_Supplier, 0, 0, ref MaterialPrice, ref dcost) * base_steelddbeoutdetail.Qty;
                        base_steelddbeoutdetail.MaterialPrice = MaterialPrice;
                    }
                    base_steelddbeoutdetailsadd.Add(base_steelddbeoutdetail);
                    rownum = rownum + 1;
                    BY_Order = DataFactory.Database().FindEntityByWhere<BY_Order>(" and CHARINDEX(QuotationCode,'" + base_steelddbeoutdetail.QuotationCode + "')>0");
                    BY_DeliveryDetail by_deliverydetail = DataFactory.Database().FindEntity<BY_DeliveryDetail>("QuotationDetailId", item.QuotationDetailId);
                    if (by_deliverydetail != null && by_deliverydetail.DeliveryDetailId != null)
                    {
                        if (!string.IsNullOrEmpty(item.Remark))
                        {
                            if ((item.Remark.IndexOf("热") >= 0 || item.Remark.IndexOf("真") >= 0 || item.Remark.IndexOf("氮") >= 0 || item.Remark.IndexOf("冷") >= 0
                            || item.Remark.IndexOf("调质") >= 0 || item.Remark.IndexOf("处理") >= 0 || item.Remark.IndexOf("火") >= 0))
                            {
                                by_deliverydetail.HeatWorkCost = item.MaterialAmount;
                            }
                            else
                            {
                                by_deliverydetail.BeOutWorkCost = item.MaterialAmount;
                            }
                        }
                        else
                        {
                            by_deliverydetail.BeOutWorkCost = item.MaterialAmount;
                        }
                        database.Update(by_deliverydetail);

                    }


                }
                entity.Remark = string.IsNullOrEmpty(entity.Remark.Replace("&nbsp;", "")) ? BY_Order.XMRequery : entity.Remark;

                if (entity.TypeJC == "进仓")
                {
                    entity.TotalAmount = System.Math.Abs(CommonHelper.GetDecimal(base_steelddbeoutdetailsadd.Sum(p => p.MaterialAmount)));
                }
                else
                {
                    entity.TotalAmount = -System.Math.Abs(CommonHelper.GetDecimal(base_steelddbeoutdetailsadd.Sum(p => p.MaterialAmount)));
                }

                if (!string.IsNullOrEmpty(KeyValue))
                {
                    database.Update(entity, isOpenTrans);
                }
                else
                {
                    database.Insert(entity, isOpenTrans);
                }

                database.Delete<Base_SteelDDBeoutDetail>("Base_SteelDDBeoutCode", entity.Base_SteelDDBeoutCode, isOpenTrans);
                database.Insert(base_steelddbeoutdetailsadd, isOpenTrans);
               
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：录入的数据重复，请检查" }.ToString());
            }
        }

       

    }
}