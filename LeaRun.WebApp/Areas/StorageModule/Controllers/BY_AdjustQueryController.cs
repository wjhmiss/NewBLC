using DNS.Business;
using DNS.Entity;
using DNS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.StorageModule.Controllers
{
    /// <summary>
    /// BY_AdjustQuery¿ØÖÆÆ÷
    /// </summary>
    public class BY_AdjustQueryController : PublicController<BY_Inventory>
    {
        BY_AdjustQueryBll by_adjustquerybll = new BY_AdjustQueryBll();



        public ActionResult GridPageListJson2(string BatchCode, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = by_adjustquerybll.GetPageList1(BatchCode, ref jqgridparam);
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
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "Òì³£´íÎó£º" + ex.Message);
                return null;
            }
        }



        public ActionResult GridPageListJson(string BatchCode)
        {
            try
            {
                if (string.IsNullOrEmpty(BatchCode))
                {
                    return null;

                }
                StringBuilder sb = new StringBuilder();
                BY_Inventory by_inventory = by_adjustquerybll.Repository().FindEntityByWhere(" and BatchCode='" + BatchCode + "' AND ParentInventoryId='0'   ");
                List<BY_Inventory> ListData = by_adjustquerybll.GetPageList(by_inventory.StoveCode);
                //ListData.Add(by_adjustquery);
                sb.Append("{ \"rows\": [");
                sb.Append(TreeGridJson(ListData, -1));
                sb.Append("]}");
                return Content(sb.ToString());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "Òì³£´íÎó£º" + ex.Message);
                return null;
            }
        }
        int lft = 1, rgt = 1000000;
        public string TreeGridJson(List<BY_Inventory> ListData, int index, string ParentId = "0")
        {
            StringBuilder sb = new StringBuilder();
            List<BY_Inventory> ChildNodeList = ListData.FindAll(t => t.ParentInventoryId == ParentId);
            if (ChildNodeList.Count > 0) { index++; }
            foreach (BY_Inventory entity in ChildNodeList)
            {
                string strJson = entity.ToJson();
                strJson = strJson.Insert(1, "\"BatchCode\":\"" + entity.BatchCode + "\",");
                strJson = strJson.Insert(1, "\"level\":" + index + ",");
                strJson = strJson.Insert(1, "\"isLeaf\":" + (ListData.Count<BY_Inventory>(t => t.ParentInventoryId == entity.InventoryId) == 0 ? true : false).ToString().ToLower() + ",");
                strJson = strJson.Insert(1, "\"expanded\":true,");
                strJson = strJson.Insert(1, "\"lft\":" + lft++ + ",");
                strJson = strJson.Insert(1, "\"rgt\":" + rgt-- + ",");
                sb.Append(strJson);
                sb.Append(TreeGridJson(ListData, index, entity.InventoryId));
            }
            return sb.ToString().Replace("}{", "},{");
        }
    }
}