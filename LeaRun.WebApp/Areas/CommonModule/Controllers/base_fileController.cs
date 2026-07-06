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
    /// base_file¿ØÖÆÆ÷
    /// </summary>
    public class base_fileController : PublicController<base_file>
    {
        [AllowAnonymous]
        public ActionResult CLCKCKC()
        {
            base_file baseFile = new base_file();
            baseFile.Create();
            baseFile.Type = "Í¼Æ¬";
            baseFile.Status = baseFile.Type == "Í¼Æ¬" ? "<div style='display: inline; '  ><img style='width:30px;' src='../../Resource/Document/LJ/test1/test2/test.png?" + new Random().Next() + "'  onMouseOver='toolTip(\"<img style=width:600px src=../../Resource/Document/LJ/test1/test2/test.png?" + new Random().Next() + ">\")' onMouseOut='toolTip()' /></div> " : "";
            DataFactory.Database().Insert(baseFile);
            return Content("1");
        }
    }
}