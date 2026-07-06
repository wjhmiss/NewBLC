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
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.StorageModule.Controllers
{
    /// <summary>
    /// 调质转型控制器
    /// </summary>
    public class AdjustStockController : PublicController<BY_InStock>
    {
        BY_InStockBll by_instockbll = new BY_InStockBll();
        #region 调质转型开料
        /// <summary>
        /// 调质转型开料视图
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult AdjustStockForm()
        {
            return View();
        }
        /// <summary>
        /// 调质转型开料视图
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult AdjustQuery()
        {
            return View();
        }
        /// <summary>
        /// 提交调质转型表单
        /// </summary>
        /// <param name="InStockListJson">入库列表</param>
        /// <param name="KeyValue">库存ID</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public ActionResult SubmitFormAdjust(string InStockListJson, string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            //取出所有碎料

            try
            {
                BY_Inventory parentby_inventory = database.FindEntity<BY_Inventory>(KeyValue);

                parentby_inventory.Enabled = -1;
                parentby_inventory.Modify(parentby_inventory.InventoryId);
                database.Update<BY_Inventory>(parentby_inventory, isOpenTrans);
                List<BY_InStock> BY_InStockList = InStockListJson.JonsToList<BY_InStock>();

                if (BY_InStockList.Where(p => string.IsNullOrWhiteSpace(p.BillCode)).Count() > 0)
                {
                    database.Rollback();
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：请填写钢材编号！！！" }.ToString());
                }


                //先写入入库单号，后面开启事务了，所以得先写入单号
                foreach (BY_InStock by_instock in BY_InStockList)
                {
                    by_instock.Create();
                    by_instock.CreateDate = DateTime.Now.AddSeconds(5);
                    by_instock.StoveCode = parentby_inventory.StoveCode;
                    by_instock.FactoryCode = parentby_inventory.FactoryCode;
                    by_instock.Price = parentby_inventory.Price;
                    by_instock.Remark = parentby_inventory.Remark;
                    by_instock.SteelType = parentby_inventory.SteelType;
                    by_instock.PlaceOrigin = parentby_inventory.PlaceOrigin;
                    by_instock.OriginalSize = parentby_inventory.OriginalSize;
                    //by_instock.CustomerId = parentby_inventory.CustomerId;
                    //by_instock.CustomerName = parentby_inventory.CustomerName;
                    //by_instock.CustomerCode = parentby_inventory.CustomerCode;
                    by_instock.InStockType = "转型入库";
                    by_instock.BatchCode = by_instock.BillCode;
                    by_instock.Qty = parentby_inventory.Qty;
                    by_instock.SteelPurchaseCode = parentby_inventory.SteelPurchaseCode;
                }
                database.Insert<BY_InStock>(BY_InStockList, isOpenTrans);

                BY_OutStock outstock = new BY_OutStock();

                CommonHelper.CopySameFieldsObject<BY_Inventory, BY_OutStock>(parentby_inventory, ref outstock);
                outstock.CustomerId = "";
                outstock.CustomerCode = "";
                outstock.CustomerName = "";
                outstock.StockId = parentby_inventory.InventoryId;
                outstock.OutStockType = "转型出库";
                outstock.BillCode = parentby_inventory.BatchCode;
                outstock.Olength = parentby_inventory.Length;
                outstock.Oweigh = parentby_inventory.Weigh;
                outstock.Othick = parentby_inventory.Thick;
                outstock.Oweight = parentby_inventory.Weight;
                outstock.OriginalWeight = parentby_inventory.Weight;
                outstock.OriginalSize = parentby_inventory.Length + "*" + parentby_inventory.Weigh + "*" + parentby_inventory.Thick;
                outstock.Opensize = parentby_inventory.Length;
                outstock.NowLength = 0;
                outstock.NowWeight = 0;
                outstock.NowSize = "0" + "*"+ parentby_inventory.Weigh + parentby_inventory.Thick;
                outstock.SteelPurchaseCode = parentby_inventory.SteelPurchaseCode;
                outstock.Enabled = 1;

                outstock.Create();
                database.Insert<BY_OutStock>(outstock, isOpenTrans);

                //BY_InStockList = by_instockbll.GetInStockCode(BY_InStockList);

                string Message = "调制转型成功。";
                int rowid = 1;
                foreach (BY_InStock by_instock in BY_InStockList)
                {

                    #region 条料写入库存表
                    BY_Inventory by_inventory = new BY_Inventory();
                    by_inventory.BatchCode = by_instock.BillCode;//继承父批次批号
                    rowid = rowid + 1;
                    by_inventory.Thick = by_instock.Thick;
                    by_inventory.Weigh = by_instock.Weigh;
                    by_inventory.Length = by_instock.Length;
                    by_inventory.DeleteMark = 0;
                    by_inventory.Enabled = 1;
                    by_inventory.MaterialType = 0;
                    by_inventory.Qty = parentby_inventory.Qty;
                    by_inventory.SteelId = by_instock.SteelId;
                    by_inventory.SteelStatus = by_instock.SteelStatus;
                    by_inventory.SteelType = parentby_inventory.SteelType;
                    by_inventory.SteelCode = by_instock.SteelCode;
                    by_inventory.SteelDescription = by_instock.SteelDescription;
                    by_inventory.SteelOtherDescription = parentby_inventory.SteelOtherDescription;
                    by_inventory.Weight = by_instock.Weight;
                    by_inventory.TheoryWeight = by_instock.TheoryWeight;
                    by_inventory.FactoryCode = parentby_inventory.FactoryCode;
                    by_inventory.InStockDate = DateTime.Now;
                    //by_inventory.InventoryId = parentby_inventory.InventoryId;
                    by_inventory.PlaceOrigin = parentby_inventory.PlaceOrigin;
                    by_inventory.Price = parentby_inventory.Price;
                    by_inventory.Remark = parentby_inventory.Remark;
                    by_inventory.StoveCode = parentby_inventory.StoveCode;
                    by_inventory.OriginalSize = parentby_inventory.OriginalSize;
                    by_inventory.CustomerId = parentby_inventory.CustomerId;
                    by_inventory.CustomerName = parentby_inventory.CustomerName;
                    by_inventory.CustomerCode = parentby_inventory.CustomerCode;
                    by_inventory.Density = parentby_inventory.Density;
                    by_inventory.QuotationCodeLine = "";
                    by_inventory.ParentInventoryId = parentby_inventory.InventoryId;
                    if (by_inventory.Weigh == 0)
                    {
                        by_inventory.ChildBatch = "Φ" + parentby_inventory.Thick + "*" + parentby_inventory.Length;
                        by_inventory.OriginalSize = "Φ" + parentby_inventory.Thick + "*" + parentby_inventory.Length;
                    }
                    else
                    {
                        by_inventory.ChildBatch = parentby_inventory.Length + "*" + parentby_inventory.Weigh + "*" + parentby_inventory.Thick;
                        by_inventory.OriginalSize = parentby_inventory.Length + "*" + parentby_inventory.Weigh + "*" + parentby_inventory.Thick;
                    }
                    by_inventory.OLength = by_inventory.Length;
                    by_inventory.OWeigh = by_inventory.Weigh;
                    by_inventory.OThick = by_inventory.Thick;
                    by_inventory.OWeight = by_inventory.Weight;
                    by_inventory.TypeName = "转型入库";
                    by_inventory.RemarkSteelperchase = parentby_inventory.RemarkSteelperchase;
                    by_inventory.position = by_instock.position;

                    by_inventory.Create();
                    //BY_AdjustQuery by_adjustquery = CommonHelper.CopySameFieldsObject<BY_AdjustQuery>(by_inventory);
                    //by_adjustquery.ParentInventoryId = parentby_inventory.InventoryId;
                    //database.Insert<BY_AdjustQuery>(by_adjustquery, isOpenTrans);

                    database.Insert<BY_Inventory>(by_inventory, isOpenTrans);
                    #endregion

                }

                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        #endregion




    }
}