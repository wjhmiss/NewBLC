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
    /// 应收款管理控制器
    /// </summary>
    public class BY_CheckCompanyController : PublicController<BY_CheckCompany>
    {
        [LoginAuthorize]
        public JsonResult GridPageJson1(string ParameterJson,string CustomerName,string Type0,string Type1,string Type2, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                List<BY_CheckCompany> ListData = new List<BY_CheckCompany>();
                if (!string.IsNullOrEmpty(ParameterJson))
                {
                    List<DbParameter> parameter = new List<DbParameter>();
                    IList conditions = ParameterJson.JonsToList<Condition>();
                    string sql = @"SELECT  cc.*
FROM    BY_CheckCompany cc ,
        dbo.Base_Customer c
WHERE   cc.CustomerId = c.CustomerId ";
                    //string WhereSql = ConditionBuilder.GetWhereSql(conditions, out parameter);
                    string WhereSql = "";
                    if (ManageProvider.Provider.Current().ObjectId.IndexOf("9d04b362-c9fe-4c67-95a5-d24d6ad9177d") < 0 && ManageProvider.Provider.Current().UserName != "超级管理员")
                    {
                        WhereSql += @"  AND cc.CreateUserName NOT IN (SELECT  b.RealName
FROM    Base_ObjectUserRelation o ,
        Base_User b
WHERE   ObjectId = '9d04b362-c9fe-4c67-95a5-d24d6ad9177d'
        AND o.UserId = b.UserId) ";
                    }
                    if (!string.IsNullOrEmpty(CustomerName))
                    {
                        WhereSql += @" AND (c.ShortName LIKE '%" + CustomerName + "%' OR c.FullName LIKE  '%" + CustomerName + "%' OR c.Code LIKE '%" + CustomerName + "%' OR cc.CheckCompanyCode LIKE '%" + CustomerName + "%')  ";

                    }
                    if (!string.IsNullOrEmpty(Type0))
                    {
                        WhereSql += @" AND Type0 = '" + Type0 + "'  ";
                    }
                    if (!string.IsNullOrEmpty(Type1))
                    {
                        WhereSql += @" AND Type1 = '" + Type1 + "'  ";
                    }
                    if (!string.IsNullOrEmpty(Type2) && Type2 != null && Type2 != "null")
                    {
                        WhereSql += @" AND Type2 = '" + Type2 + "'  ";
                    }
                    ListData = repositoryfactory.Repository().FindListPageBySql(sql+WhereSql, parameter.ToArray(), ref jqgridparam);
                }
                else
                {
                    string WhereSql = "";
                    if (ManageProvider.Provider.Current().ObjectId.IndexOf("9d04b362-c9fe-4c67-95a5-d24d6ad9177d") < 0 && ManageProvider.Provider.Current().UserName != "超级管理员")
                    {
                        WhereSql += @"  AND cc.CreateUserName NOT IN (SELECT  b.RealName
FROM    Base_ObjectUserRelation o ,
        Base_User b
WHERE   ObjectId = '9d04b362-c9fe-4c67-95a5-d24d6ad9177d'
        AND o.UserId = b.UserId) ";
                    }
                    ListData = repositoryfactory.Repository().FindListPage(WhereSql, ref jqgridparam);
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
        public virtual ActionResult SubmitForm1(BY_CheckCompany entity, string KeyValue)
        {
            Base_CodeRuleBll base_coderulebll = new Base_CodeRuleBll();
            string billcode = base_coderulebll.GetBillCode(ManageProvider.Provider.Current().UserId, "Y");
            int IsOk = 0;
            string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    entity.Modify(KeyValue);
                    IsOk = repositoryfactory.Repository().Update(entity, isOpenTrans);
                }
                else
                {
                    entity.Create();
                    entity.CheckCompanyCode = billcode;
                    IsOk = repositoryfactory.Repository().Insert(entity, isOpenTrans);
                    base_coderulebll.OccupyBillCode(ManageProvider.Provider.Current().UserId, "Y", isOpenTrans);
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

        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult Audit(string KeyValue)
        {
            try
            {
                //BY_CheckCompany by_checkcompany = repositoryfactory.Repository().FindEntity(KeyValue);
                //int IsOk = 0;
                //if (!string.IsNullOrEmpty(by_checkcompany.CheckCompanyId))
                //{
                //    by_checkcompany.Modify(KeyValue);
                //    by_checkcompany.IsAudit = 1;
                //    IsOk = repositoryfactory.Repository().Update(by_checkcompany);
                //}
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    string[] ids = KeyValue.Split(',');
                    for (int i = 0; i < ids.Length; i++)
                    {
                        BY_CheckCompany entity = DataFactory.Database().FindEntity<BY_CheckCompany>(ids[i]); ;
                        entity.IsAudit = entity.IsAudit == 0 ? 1 : 0; ;
                        entity.Modify(ids[i]);
                        DataFactory.Database().Update(entity);
                    }

                }
                return Content(new JsonMessage { Success = true, Code = "1", Message = "审核通过" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult Invoice(string KeyValue)
        {
            try
            {
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    BY_CheckCompany entity = new BY_CheckCompany();
                    string[] ids = KeyValue.Split(',');
                    for (int i = 0; i < ids.Length; i++)
                    {
                        entity.IsInvoice = 1;
                        entity.IsAudit = 1;
                        entity.Modify(ids[i]);
                        DataFactory.Database().Update(entity);
                    }

                }
                
                return Content(new JsonMessage { Success = true, Code = "1", Message = "已开票" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
    }
}