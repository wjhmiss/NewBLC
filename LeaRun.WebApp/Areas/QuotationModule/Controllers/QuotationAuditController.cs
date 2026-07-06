using DNS.Business.Utility.Enum;
using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.QuotationModule.Controllers
{
    /// <summary>
    /// 报价单审核
    /// </summary>
    public class QuotationAuditController : PublicController<BY_Quotation>
    {
        /// <summary>
        /// 报价单详细页面
        /// </summary>
        /// <returns></returns>
        public override ActionResult Detail()
        {
            //CookieHelper.WriteCookie("ModuleId", DESEncrypt.Encrypt("65590e2c-f46c-41de-8cfb-9be9a6bf74bf"));
            return View();
        }
        /// <summary>
        /// 审核/驳回页面
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult Audit()
        {
            return View();
        }
        /// <summary>
        /// 审核/驳回
        /// </summary>
        /// <param name="KeyValue">报价单Id</param>
        /// <param name="CheckStatus">审核标识</param>
        /// <param name="CheckRemark">审核意见</param>
        /// <returns></returns>
        public ActionResult SubmitAudit(string KeyValue, int CheckStatus, string CheckRemark)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            string Message = "";
            try
            {
                BY_Quotation entity = new BY_Quotation();
                entity.QuotationId = KeyValue;
                if (CheckStatus == -1)
                {
                    entity.BillStatus = QuotationBillStatus.已驳回.ToString();
                }
                else
                {
                    entity.BillStatus = QuotationBillStatus.已审核.ToString();
                }
                entity.CheckStatus = CheckStatus;
                entity.CheckRemark = CheckRemark;
                entity.CheckDate = DateTime.Now;
                entity.CheckUserId = ManageProvider.Provider.Current().UserName;
                database.Update(entity, isOpenTrans);
                if (CheckStatus == -1)
                {
                    Message = "驳回成功。";
                }
                else
                {
                    Message = "审核成功,请通知接单员列印报价单";
                }

                BY_OpearLog by_opearlog = new BY_OpearLog();
                by_opearlog.Create();
                by_opearlog.BillId = KeyValue;
                by_opearlog.OpearType = entity.BillStatus;
                by_opearlog.Remark = entity.CheckRemark;
                database.Insert(by_opearlog, isOpenTrans);

                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = Message }.ToString());
            }
            catch (Exception)
            {
                database.Rollback();
                if (CheckStatus == -1)
                {
                    Message = "驳回失败。";
                }
                else
                {
                    Message = "审核失败。";
                }
                return Content(new JsonMessage { Success = true, Code = "-1", Message = Message }.ToString());
            }
        }
    }
}
