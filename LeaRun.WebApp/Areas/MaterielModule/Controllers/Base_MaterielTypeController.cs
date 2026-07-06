using DNS.Business;
using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.MaterielModule.Controllers
{
    /// <summary>
    /// 物料种类控制器
    /// </summary>
    public class Base_MaterielTypeController : PublicController<Base_MaterielType>
    {
        Base_MaterielTypeBll base_materieltypebll = new Base_MaterielTypeBll();
        public ActionResult GetAutoCompleJson(string keywords)
        {
            List<Base_MaterielType> ListData = base_materieltypebll.GetAutoComple(keywords);
            return Content(ListData.ToJson());
        }



        [HttpPost]
        [ValidateInput(false)]
        public override ActionResult SubmitForm(Base_MaterielType entity, string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    Base_MaterielType Oldentity = database.FindEntity<Base_MaterielType>(KeyValue);//获取没更新之前实体对象
                    Base_MaterielStorage ms = database.FindEntity<Base_MaterielStorage>("MaterielId", Oldentity.MaterielId);
                    ms.MaterielName = entity.MaterielName;
                    ms.MaterielCode = entity.MaterielCode;
                    ms.MaterielType = entity.MaterielType;
                    ms.MaterielSize = entity.MaterielSize;
                    ms.Unit = entity.Unit;
                    ms.CustomerId = entity.CustomerId;
                    ms.CustomerCode = entity.CustomerCode;
                    ms.CustomerName = entity.CustomerName;
                    ms.PricePer = entity.PricePer;

                    entity.Modify(KeyValue);
                    ms.Modify(ms.MaterielStorageId);

                    database.Update(ms, isOpenTrans);
                    database.Update(entity, isOpenTrans);
                    IsOk = 1;
                    this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }
                else
                {
                    entity.Create();
                    IsOk = repositoryfactory.Repository().Insert(entity);
                    this.WriteLog(IsOk, entity, null, KeyValue, Message);
                }
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                this.WriteLog(-1, entity, null, KeyValue, "操作失败：" + ex.Message);
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
    }
}