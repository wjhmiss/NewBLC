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
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.MaterielModule.Controllers
{
    /// <summary>
    /// 物料领料控制器
    /// </summary>
    public class Base_MaterielGetController : PublicController<Base_MaterielGet>
    {
        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult SubmitForm1(Base_MaterielGet entity, string KeyValue)
        {
            try
            {
                Base_MaterielStorage base_materielstorage = DataFactory.Database().FindEntity<Base_MaterielStorage>("MaterielId", entity.MaterielId);
                if (base_materielstorage == null || base_materielstorage.MaterielStorageId == null)
                {
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：仓库无此料" }.ToString());
                }
                if (entity.Qty > base_materielstorage.Qty)
                {
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：仓库此料不足，剩余："+base_materielstorage.Qty.ToString() }.ToString());
                }
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    Base_MaterielGet Oldentity = repositoryfactory.Repository().FindEntity(KeyValue);//获取没更新之前实体对象
                    entity.Modify(KeyValue);
                    IsOk = repositoryfactory.Repository().Update(entity);
                    this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }
                else
                {
                    entity.Create();
                    IsOk = repositoryfactory.Repository().Insert(entity);
                    this.WriteLog(IsOk, entity, null, KeyValue, Message);
                }
                return Content(new JsonMessage { Success = true, Code = "1", Message = Message }.ToString());
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
        public virtual ActionResult Update(Base_MaterielGet entity)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                if (entity.MaterielGetId == "all")
                {
                    List<Base_MaterielGet> base_materielgets = DataFactory.Database().FindList<Base_MaterielGet>("Status", "0");
                    foreach (Base_MaterielGet item in base_materielgets)
                    {
                        Base_MaterielStorage base_materielstorage = DataFactory.Database().FindEntity<Base_MaterielStorage>("MaterielId", item.MaterielId);
                        if (base_materielstorage != null && base_materielstorage.MaterielStorageId != null)
                        {
                            base_materielstorage.Modify(base_materielstorage.MaterielStorageId);
                            base_materielstorage.Qty -= item.Qty;
                            DataFactory.Database().Update(base_materielstorage);
                        }
                        else
                        {
                            return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：仓库无此料" }.ToString());
                        }
                        item.Status = 1;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    DataFactory.Database().Commit();
                }
                else if (!string.IsNullOrEmpty(entity.MaterielGetId))
                {
                    entity.Modify(entity.MaterielGetId);
                    if (entity.Status == 1)
                    {
                        Base_MaterielGet base_materielget = DataFactory.Database().FindEntity<Base_MaterielGet>(entity.MaterielGetId);
                        Base_MaterielStorage base_materielstorage = DataFactory.Database().FindEntity<Base_MaterielStorage>("MaterielId", base_materielget.MaterielId);
                        if (base_materielstorage != null && base_materielstorage.MaterielStorageId != null)
                        {
                            base_materielstorage.Modify(base_materielstorage.MaterielStorageId);
                            base_materielstorage.Qty -= base_materielget.Qty;
                            DataFactory.Database().Update(base_materielstorage, isOpenTrans);
                        }
                        else
                        {
                            database.Rollback();
                            return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：仓库无此料" }.ToString());
                        }

                        base_materielget.Status = 1;
                        DataFactory.Database().Update(base_materielget, isOpenTrans);
                        DataFactory.Database().Commit();
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
    }
}