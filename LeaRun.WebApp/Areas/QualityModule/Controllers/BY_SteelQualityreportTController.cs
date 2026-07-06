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
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.QualityModule.Controllers
{
    /// <summary>
    /// BY_SteelQualityreport¿ØÖÆÆ÷
    /// </summary>
    public class BY_SteelQualityreportTController : PublicController<BY_SteelQualityreportT>
    {
        public  ActionResult SetForm1(string SteelQualityreportCode)
        {
            List<BY_SteelQualityreportT> entity = repositoryfactory.Repository().FindListBySql("select top 10 * from BY_SteelQualityreportT where  SteelQualityreportCode LIKE '%" + SteelQualityreportCode + "%'");
            return Content(entity.ToJson());
        }
    }
}