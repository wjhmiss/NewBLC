using DNS.Business;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.CommonModule.Controllers
{
    /// <summary>
    /// 设置绩效控制器
    /// </summary>
    public class Base_SetPerformanceController : PublicController<Base_SetPerformance>
    {
        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public override ActionResult SubmitForm(Base_SetPerformance entity, string KeyValue)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    Base_SetPerformance Oldentity = repositoryfactory.Repository().FindEntity(KeyValue);//获取没更新之前实体对象
                    Base_DataDictionaryDetail base_datadictionarydetail = DataFactory.Database().FindEntityByWhere<Base_DataDictionaryDetail>(string.Format(@" and FullName='{0}' AND DataDictionaryId='21d16fdf-780d-43ac-9bab-2c5142ba6a7c'", entity.TechnologyName));
                    entity.TechnologyId = base_datadictionarydetail.Code;
                    entity.Modify(KeyValue);
                    IsOk = repositoryfactory.Repository().Update(entity);
                    this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }
                else
                {
                    entity.Create();
                    Base_DataDictionaryDetail base_datadictionarydetail = DataFactory.Database().FindEntityByWhere<Base_DataDictionaryDetail>(string.Format(@" and FullName='{0}' AND DataDictionaryId='21d16fdf-780d-43ac-9bab-2c5142ba6a7c'", entity.TechnologyName));
                    entity.TechnologyId = base_datadictionarydetail.Code;
                    IsOk = repositoryfactory.Repository().Insert(entity);
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
    }
}