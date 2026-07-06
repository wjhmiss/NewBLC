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
    /// 物料退料控制器
    /// </summary>
    public class Base_MaterielReturnController : PublicController<Base_MaterielReturn>
    {
        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult Update(Base_MaterielReturn entity)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                int IsOk = 0;
                if (!string.IsNullOrEmpty(entity.MaterielReturnId))
                {
                    entity.Modify(entity.MaterielReturnId);
                    if (entity.Status == 1)
                    {
                        Base_MaterielReturn base_materielreturn = DataFactory.Database().FindEntity<Base_MaterielReturn>(entity.MaterielReturnId);
                        Base_MaterielStorage base_materielstorage = DataFactory.Database().FindEntity<Base_MaterielStorage>("MaterielId", base_materielreturn.MaterielId);
                        if (base_materielstorage != null && base_materielstorage.MaterielStorageId != null)
                        {
                            base_materielstorage.Modify(base_materielstorage.MaterielStorageId);
                            base_materielstorage.Qty += base_materielreturn.Qty;
                            DataFactory.Database().Update(base_materielstorage, isOpenTrans);
                        }
                        else
                        {
                            database.Rollback();
                            return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：仓库无此料" }.ToString());
                        }

                        base_materielreturn.Status = 1;
                        DataFactory.Database().Update(base_materielreturn, isOpenTrans);
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