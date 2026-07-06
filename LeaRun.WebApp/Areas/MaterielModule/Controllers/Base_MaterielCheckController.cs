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
    /// 物料盘点控制器
    /// </summary>
    public class Base_MaterielCheckController : PublicController<Base_MaterielCheck>
    {
        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult Update(Base_MaterielCheck entity)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                int IsOk = 0;
                if (!string.IsNullOrEmpty(entity.MaterielCheckId))
                {
                    entity.Modify(entity.MaterielCheckId);
                    if (entity.Status == 1)
                    {
                        Base_MaterielCheck base_materielcheck = DataFactory.Database().FindEntity<Base_MaterielCheck>(entity.MaterielCheckId);
                        Base_MaterielStorage base_materielstorage = DataFactory.Database().FindEntity<Base_MaterielStorage>("MaterielId", base_materielcheck.MaterielId);
                        if (base_materielstorage != null && base_materielstorage.MaterielStorageId != null)
                        {
                            base_materielstorage.Modify(base_materielstorage.MaterielStorageId);
                            base_materielstorage.Qty = base_materielcheck.NowQty;
                            DataFactory.Database().Update(base_materielstorage, isOpenTrans);
                        }
                        else
                        {
                            Base_MaterielType base_materieltype = DataFactory.Database().FindEntity<Base_MaterielType>(base_materielcheck.MaterielId);
                            CommonHelper.CopySameFieldsObject<Base_MaterielCheck, Base_MaterielStorage>(base_materielcheck, ref base_materielstorage);
                            base_materielstorage.Create();
                            base_materielstorage.Qty = base_materielcheck.NowQty;
                            base_materielstorage.AlarmNumber = base_materieltype.AlarmNumber;
                            DataFactory.Database().Insert(base_materielstorage, isOpenTrans);
                        }

                        base_materielcheck.Status = 1;
                        DataFactory.Database().Update(base_materielcheck, isOpenTrans);
                        
                    }
                }
                DataFactory.Database().Commit();
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