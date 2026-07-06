using DNS.Business;
using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.OrderModule.Controllers
{
    /// <summary>
    /// Base_SteelDDBeoutDetail控制器
    /// </summary>
    public class Base_SteelDDBeoutDetailController : PublicController<Base_SteelDDBeoutDetail>
    {
        public ActionResult Update(Base_SteelDDBeoutDetail entity)
        {
            IDatabase database = DataFactory.Database();
            try
            {
                Base_SteelDDBeoutDetail detail = DataFactory.Database().FindEntity<Base_SteelDDBeoutDetail>(entity.Base_SteelDDBeoutDetailId);

                if (detail != null && detail.Base_SteelDDBeoutDetailId != null)
                {
                    detail.isback = detail.isback == 1 ? 0 : 1;
                    detail.Modify(entity.Base_SteelDDBeoutDetailId);
                    DataFactory.Database().Update(detail);
                }

                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功！" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }


        public ActionResult GetListJson1(string Base_SteelDDBeoutCode)
        {
            try
            {
                List<Base_SteelDDBeoutDetail> ListData = DataFactory.Database().FindList<Base_SteelDDBeoutDetail>("Base_SteelDDBeoutCode", Base_SteelDDBeoutCode);

                return Content(ListData.OrderBy(p => p.SortCode).ToJson());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }

        Base_SteelDDBeoutDetailBll base_steelddperchasedetailbll = new Base_SteelDDBeoutDetailBll();
        public ActionResult GridPageListJson(string Condition, JqGridParam jqgridparam)
        {
            try
            {
                Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = base_steelddperchasedetailbll.GetOrderQueryList(QueryParameter, ref jqgridparam);
                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = ListData,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}