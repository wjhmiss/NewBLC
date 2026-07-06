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
    /// Base_Sheet控制器
    /// </summary>
    public class Base_SheetController : PublicController<Base_Sheet>
    {
        [LoginAuthorize]
        public JsonResult GridPageJson1(string ParameterJson, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                List<Base_Sheet> ListData = new List<Base_Sheet>();
                if (!string.IsNullOrEmpty(ParameterJson))
                {
                    List<DbParameter> parameter = new List<DbParameter>();
                    IList conditions = ParameterJson.JonsToList<Condition>();
                    string WhereSql = ConditionBuilder.GetWhereSql(conditions, out parameter);
                    if (ManageProvider.Provider.Current().ObjectId.IndexOf("9d04b362-c9fe-4c67-95a5-d24d6ad9177d") < 0 && ManageProvider.Provider.Current().UserName != "超级管理员")
                    {
                        WhereSql += @"  AND CreateUserName NOT IN (SELECT  b.RealName
FROM    Base_ObjectUserRelation o ,
        Base_User b
WHERE   ObjectId = '9d04b362-c9fe-4c67-95a5-d24d6ad9177d'
        AND o.UserId = b.UserId) ";
                    }
                    ListData = repositoryfactory.Repository().FindListPage(WhereSql, parameter.ToArray(), ref jqgridparam);
                }
                else
                {
                    string WhereSql = "";
                    if (ManageProvider.Provider.Current().ObjectId.IndexOf("9d04b362-c9fe-4c67-95a5-d24d6ad9177d") < 0 && ManageProvider.Provider.Current().UserName != "超级管理员")
                    {
                        WhereSql += @"  AND CreateUserName NOT IN (SELECT  b.RealName
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



        string ModuleId = "G";
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
        public override ActionResult SubmitForm(Base_Sheet entity, string KeyValue)
        {
            string PaySheetCode = BillCode();
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    Base_Sheet Oldentity = repositoryfactory.Repository().FindEntity(KeyValue);//获取没更新之前实体对象
                    entity.Modify(KeyValue);
                    IsOk = repositoryfactory.Repository().Update(entity);
                    this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }
                else
                {
                    entity.Create();
                    entity.PaySheetCode = PaySheetCode;
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
        public virtual ActionResult Update(Base_Sheet entity, string PaySheetIds)
        {
            try
            {
                int IsOk = 0;
                if (!string.IsNullOrEmpty(PaySheetIds))
                {
                    string[] ids = PaySheetIds.Split(',');
                    for (int i = 0; i < ids.Length; i++)
                    {
                        Base_Sheet Base_Sheet = DataFactory.Database().FindEntity<Base_Sheet>(ids[i]);
                        Base_Sheet.Status = Base_Sheet.Status == 1 ? 0 : 1;
                        Base_Sheet.Modify(ids[i]);
                        IsOk = DataFactory.Database().Update(Base_Sheet);
                    }

                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "操作成功！" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
    }
}