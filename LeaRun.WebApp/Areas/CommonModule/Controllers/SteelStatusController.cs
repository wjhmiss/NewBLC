using DNS.Business;
using DNS.Entity;
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
    /// 钢材状态设置表控制器
    /// </summary>
    public class SteelStatusController : PublicController<Base_SteelStatus>
    {
        Base_SteelStatusBll base_steelstatusbll = new Base_SteelStatusBll();

        /// <summary>
        /// 根据钢材编码获取 状态对象
        /// </summary>
        /// <param name="SteelCode">钢材编码</param>
        /// <returns></returns>
        public ActionResult GetSteelStatusJson(string SteelCode)
        {
            Base_SteelStatus entity = base_steelstatusbll.GetEntityBySteelCode(SteelCode);
            return Content(entity.ToJson());
        }
    }
}