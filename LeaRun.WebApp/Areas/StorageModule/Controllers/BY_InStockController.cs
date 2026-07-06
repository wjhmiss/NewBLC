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
    /// 入仓表控制器
    /// </summary>
    public class BY_InStockController : PublicController<BY_InStock>
    {
        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public ActionResult SubmitBoardForm1(string ids)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                string[] InventoryIds = ids.Trim(',').Split(',');
                for (int i = 0; i < InventoryIds.Length; i++)
                {
                    BY_Inventory by_inventory = DataFactory.Database().FindEntity<BY_Inventory>(InventoryIds[i]);
                    by_inventory.Enabled = 1;
                    by_inventory.InStockDate = DateTime.Now;
                    BY_InStock by_instock = new BY_InStock();
                    CommonHelper.CopySameFieldsObject<BY_Inventory, BY_InStock>(by_inventory, ref by_instock);
                    by_instock.BillCode = by_inventory.BatchCode;
                    //by_instock.InStockType = 0;
                    by_instock.Create();
                    
                    BY_AdjustQuery by_adjustquery = CommonHelper.CopySameFieldsObject<BY_AdjustQuery>(by_inventory);
                    by_inventory.Modify(by_inventory.InventoryId);
                    by_adjustquery.ParentInventoryId = "0";
                    database.Update(by_inventory, isOpenTrans);
                    database.Insert<BY_AdjustQuery>(by_adjustquery, isOpenTrans);
                    database.Insert(by_instock, isOpenTrans);
                }
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "进仓成功！" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        BY_InStockBll by_instockbll = new BY_InStockBll();
        #region 板件入仓
        /// <summary>
        /// 板件入仓列表页面
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult BoardIndex()
        {
            return View();
        }
        /// <summary>
        /// 板件新增页面
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult BoardForm()
        {
            return View();
        }
        /// <summary>
        /// 板件明细页面
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult BoardDetail()
        {
            return View();
        }


        /// <summary>
        /// 提交板件入仓表单
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="InStockType">入库类型</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public ActionResult SubmitBoardForm(BY_InStock by_instock, string InStockType, string BatchCode)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                int IsOk = 0;
                string Message = "新增成功。";
                by_instock.InStockType = "手工入库";
                by_instock.BillCode = BatchCode;
                //by_instock.InStockType = int.Parse(InStockType);
                by_instock.Create();
                IsOk = database.Insert(by_instock, isOpenTrans);
                #region 写入库存表

                Base_Steel base_steel = database.FindEntity<Base_Steel>("SteelCode", by_instock.SteelCode);

                BY_Inventory by_inventory = new BY_Inventory();
                by_inventory.BatchCode = BatchCode;
                by_inventory.Density = base_steel.Density;
                by_inventory.ChildBatch = by_instock.Thick.ToString() + "×" + by_instock.Weigh.ToString() + "×" + by_instock.Length.ToString();
                by_inventory.StoveCode = by_instock.StoveCode;
                by_inventory.TheoryWeight = by_instock.TheoryWeight;
                by_inventory.Thick = by_instock.Thick;
                by_inventory.Weigh = by_instock.Weigh;
                by_inventory.DeleteMark = 0;
                by_inventory.Enabled = 1;
                by_inventory.PlaceOrigin = by_instock.PlaceOrigin;
                by_inventory.FactoryCode = by_instock.FactoryCode;
                by_inventory.InStockDate = by_instock.InStockDate;
                by_inventory.Length = by_instock.Length;
                by_inventory.MaterialType = 0;
                by_inventory.Price = by_instock.Price;
                by_inventory.Qty = by_instock.Qty;
                by_inventory.Remark = by_instock.Remark;
                by_inventory.SteelId = by_instock.SteelId;
                by_inventory.SteelStatus = by_instock.SteelStatus;
                by_inventory.SteelType = by_instock.SteelType;
                by_inventory.SteelCode = by_instock.SteelCode;
                by_inventory.SteelDescription = by_instock.SteelDescription;
                by_inventory.SteelOtherDescription = by_instock.SteelOtherDescription;
                by_inventory.Weight = by_instock.Weight;
                by_inventory.OriginalSize = by_instock.Thick.ToString() + "×" + by_instock.Weigh.ToString() + "×" + by_instock.Length.ToString();
                by_inventory.CustomerId = by_instock.CustomerId;
                by_inventory.CustomerName = by_instock.CustomerName;
                by_inventory.CustomerCode = by_instock.CustomerCode;
                by_inventory.BoreSize = by_instock.BoreSize;

                by_inventory.OLength =by_inventory.Length;
        by_inventory.OWeigh =by_inventory.OWeigh;
        by_inventory.OThick =by_inventory.OThick;
        by_inventory.OWeight = by_inventory.OWeight;
        by_inventory.TypeName = "手工入库";


                by_inventory.Create();
                BY_AdjustQuery by_adjustquery = CommonHelper.CopySameFieldsObject<BY_AdjustQuery>(by_inventory);
                by_adjustquery.ParentInventoryId = "0";
                database.Insert<BY_AdjustQuery>(by_adjustquery, isOpenTrans);
                database.Insert<BY_Inventory>(by_inventory, isOpenTrans);
                #endregion
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：编码重复！" }.ToString());
            }
        }
        #endregion

        #region 碎料入仓
        /// <summary>
        /// 碎料入仓列表页面
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult BrokenIndex()
        {
            return View();
        }
        /// <summary>
        /// 碎料新增页面
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult BrokenForm()
        {
            return View();
        }
        /// <summary>
        /// 碎料明细页面
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult BrokenDetail()
        {
            return View();
        }
        /// <summary>
        /// 提交碎料入仓表单
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="InStockType">入库类型</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public ActionResult SubmitBrokenForm(BY_InStock by_instock, string InStockType)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                int IsOk = 0;
                string Message = "新增成功。";
                by_instock.BillCode = by_instock.SteelCode;
                //by_instock.InStockType = int.Parse(InStockType);
                by_instock.Create();
                IsOk = database.Insert(by_instock, isOpenTrans);
                #region 处理库存
                string batchCode = by_instock.SteelCode;
                BY_Inventory by_inventory = database.FindEntity<BY_Inventory>("BatchCode", batchCode);
                if (by_inventory.InventoryId == null)
                {
                    by_inventory.BatchCode = batchCode;
                    by_inventory.ChildBatch = "0×0×0";
                    by_inventory.DeleteMark = 0;
                    by_inventory.Enabled = 1;
                    by_inventory.MaterialType = 1;
                    by_inventory.Remark = by_instock.Remark;
                    by_inventory.SteelId = by_instock.SteelId;
                    by_inventory.SteelStatus = by_instock.SteelStatus;
                    by_inventory.SteelType = by_instock.SteelType;
                    by_inventory.SteelCode = by_instock.SteelCode;
                    by_inventory.SteelDescription = by_instock.SteelDescription;
                    by_inventory.SteelOtherDescription = by_instock.SteelOtherDescription;
                    by_inventory.Weight = by_instock.Weight;
                    by_inventory.InStockDate = DateTime.Now;
                    by_inventory.Price = by_instock.Price;
                    by_inventory.Create();
                    database.Insert<BY_Inventory>(by_inventory, isOpenTrans);
                }
                else
                {
                    by_inventory.Weight = by_inventory.Weight + by_instock.Weight;
                    by_inventory.Modify(by_inventory.InventoryId);
                    database.Update<BY_Inventory>(by_inventory, isOpenTrans);
                }
                #endregion
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                this.WriteLog(-1, by_instock, null, "", "操作失败：" + ex.Message);
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        #endregion

        #region 公共

        /// <summary>
        /// 【入仓管理】返回列表JONS
        /// </summary>
        /// <param name="InStockType">入库类型</param>
        /// <param name="jqgridparam">JqGrid表格参数</param>
        /// <returns></returns>
        public ActionResult GridPageBroJson(string InStockType, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = by_instockbll.GetPageListInventory(InStockType,  ref jqgridparam);
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
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 入仓材料列表
        /// </summary>
        /// <param name="StartTime">起始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="SteelCode">钢种</param>
        /// <param name="SteelStatus">状态</param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public ActionResult GridPageListJson(string InStockType, DateTime StartTime, DateTime EndTime, string SteelCode, string CustomerCode,string BatchCode, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = by_instockbll.GetPageList(InStockType, StartTime, EndTime, SteelCode, CustomerCode,BatchCode, ref jqgridparam);
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
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 获得入库列表
        /// </summary>
        /// <param name="BillCode"></param>
        /// <returns></returns>
        public ActionResult GetInStockList(string BillCode)
        {
            return Content(by_instockbll.GetInStockList(BillCode).ToJson());
        }
        /// <summary>
        /// 根据IP返回称重重量
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public ActionResult GetScale(string ip)
        {
            return  Content(by_instockbll.GetScale(ip).ToJson());
        }
        #endregion

    }
}