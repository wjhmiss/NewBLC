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
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.CommonModule.Controllers
{
    /// <summary>
    /// 工厂日历控制器  
    /// </summary>
    public class WorkDayController : PublicController<Base_WorkDay>
    {
        Base_WorkDayBll base_workdaybll = new Base_WorkDayBll();
        /// <summary>
        /// 保存日历设置
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ActionResult SubmitWorkDateForm(Base_WorkDay entity)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                database.ExecuteBySql(new StringBuilder("DELETE FROM Base_WorkDay WHERE CompanyId ='" + entity.CompanyId + "' AND YEAR ='" + entity.Year + "' AND MONTH='" + entity.Month + "'"), isOpenTrans);
                entity.Create();
                database.Insert(entity, isOpenTrans);
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功。" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败。" }.ToString());
            }
        }
        /// <summary>
        /// 获取日历对象
        /// </summary>
        /// <param name="CompanyId">公司Id</param>
        /// <param name="Year">年</param>
        /// <param name="Month">月</param>
        /// <returns></returns>
        public ActionResult GetMonth(string CompanyId, string Year, string Month)
        {
            Base_WorkDay entity = base_workdaybll.GetList(CompanyId, Year, Month);
            return Content(entity.ToJson());
        }
    }
}