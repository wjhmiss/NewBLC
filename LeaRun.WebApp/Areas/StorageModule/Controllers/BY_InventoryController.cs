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

namespace DNS.WebApp.Areas.StorageModule.Controllers
{
    /// <summary>
    /// 库存表控制器
    /// </summary>
    public class BY_InventoryController : PublicController<BY_Inventory>
    {

        [HttpPost]
        [ManagerPermission(PermissionMode.Enforce)]
        public virtual ActionResult Delete1(string KeyValue, string ParentId)
        {
            try
            {
                try
                {
                    BY_Inventory Inventory = DataFactory.Database().FindEntity<BY_Inventory>(KeyValue);
                    if (Inventory.MaterialType == 1)
                    {
                        return Content(new JsonMessage { Success = true, Code = "1", Message = "碎料不能删除！" }.ToString());

                    }
                    Inventory.Enabled = -1;
                    Inventory.Modify(Inventory.InventoryId);
                    string Message = "删除成功。";
                    DataFactory.Database().Update<BY_Inventory>(Inventory);
                    return Content(new JsonMessage { Success = true, Code = "1", Message = Message }.ToString());
                }
                catch (Exception ex)
                {
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
                }
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }



        BY_InventoryBll by_inventorybll = new BY_InventoryBll();
        /// <summary>
        /// 根据批号返回列表JONS
        /// </summary>
        /// <param name="BatchCode">批号</param>
        /// <returns></returns>
        public ActionResult GetListJson(string BatchCode,string InventoryType)
        {
            try
            {
                List<BY_Inventory> ListData = by_inventorybll.GetList(BatchCode, InventoryType);
                return Content(ListData.ToJson());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }
        public ActionResult GetListJson1(string SteelPurchaseCode)
        {
            try
            {
                List<BY_Inventory> ListData = DataFactory.Database().FindList<BY_Inventory>("SteelPurchaseCode", SteelPurchaseCode);

                //var entitys = from entity in ListData
                //              group entity by new { entity.OLength, entity.OWeigh, entity.OThick, entity.BoreSize, entity.SteelDescription, entity.Density, entity.QuotationCodeLine, entity.PlaceOrigin, entity.StoveCode }
                //                  into g
                //                  select new
                //                  {
                //                      Key = g.Key,
                //                      TotalAmount = g.Sum(item => item.TotalAmount),
                //                      Qty = g.Sum(item => item.Qty),
                //                      Weight = g.Sum(item => item.Weight),
                //                      OWeight = g.Sum(item => item.OWeight),
                //                      TheoryWeight = g.Sum(item => item.TheoryWeight),
                //                      Weightcheng = g.Sum(item => item.Weightcheng),
                //                  };
                //IList<BY_Inventory> by_inventorys = new List<BY_Inventory>();
                //BY_Inventory by_inventory;
                //foreach (var item in entitys)
                //{
                //    by_inventory = new BY_Inventory();
                //    by_inventory.Qty = item.Qty;
                //    by_inventory.TotalAmount = item.TotalAmount;
                //    by_inventory.Length = item.Key.OLength;
                //    by_inventory.Thick = item.Key.OThick;
                //    by_inventory.Weigh = item.Key.OWeigh;
                //    by_inventory.OLength = item.Key.OLength;
                //    by_inventory.OThick = item.Key.OThick;
                //    by_inventory.OWeigh = item.Key.OWeigh;
                //    by_inventory.OWeight = item.OWeight;
                //    by_inventory.BoreSize = item.Key.BoreSize;
                //    by_inventory.Weight = item.Weight;
                //    by_inventory.TheoryWeight = item.TheoryWeight;
                //    by_inventory.Weightcheng = item.Weightcheng;
                //    by_inventory.Price = ListData.Where(p => CommonHelper.GetDecimal(p.OLength) == CommonHelper.GetDecimal(item.Key.OLength) && CommonHelper.GetDecimal(p.OWeigh) == CommonHelper.GetDecimal(item.Key.OWeigh) && CommonHelper.GetDecimal(p.OThick) == CommonHelper.GetDecimal(item.Key.OThick) && p.SteelDescription == item.Key.SteelDescription).FirstOrDefault().Price;
                //    by_inventory.Remark = ListData.Where(p => CommonHelper.GetDecimal(p.OLength) == CommonHelper.GetDecimal(item.Key.OLength) && CommonHelper.GetDecimal(p.OWeigh) == CommonHelper.GetDecimal(item.Key.OWeigh) && CommonHelper.GetDecimal(p.OThick) == CommonHelper.GetDecimal(item.Key.OThick) && p.SteelDescription == item.Key.SteelDescription).FirstOrDefault().Remark;
                //    by_inventory.SortCode = ListData.Where(p => CommonHelper.GetDecimal(p.OLength) == CommonHelper.GetDecimal(item.Key.OLength) && CommonHelper.GetDecimal(p.OWeigh) == CommonHelper.GetDecimal(item.Key.OWeigh) && CommonHelper.GetDecimal(p.OThick) == CommonHelper.GetDecimal(item.Key.OThick) && p.SteelDescription == item.Key.SteelDescription).Min(P => P.SortCode);
                //    by_inventory.SteelDescription = item.Key.SteelDescription;
                //    by_inventory.TypeName = ListData.Where(p => CommonHelper.GetDecimal(p.OLength) == CommonHelper.GetDecimal(item.Key.OLength) && CommonHelper.GetDecimal(p.OWeigh) == CommonHelper.GetDecimal(item.Key.OWeigh) && CommonHelper.GetDecimal(p.OThick) == CommonHelper.GetDecimal(item.Key.OThick) && p.SteelDescription == item.Key.SteelDescription).FirstOrDefault().TypeName;
                //    by_inventory.SteelCode = ListData.Where(p => CommonHelper.GetDecimal(p.OLength) == CommonHelper.GetDecimal(item.Key.OLength) && CommonHelper.GetDecimal(p.OWeigh) == CommonHelper.GetDecimal(item.Key.OWeigh) && CommonHelper.GetDecimal(p.OThick) == CommonHelper.GetDecimal(item.Key.OThick) && p.SteelDescription == item.Key.SteelDescription).FirstOrDefault().SteelCode;
                //    by_inventory.SteelStatus = ListData.Where(p => CommonHelper.GetDecimal(p.OLength) == CommonHelper.GetDecimal(item.Key.OLength) && CommonHelper.GetDecimal(p.OWeigh) == CommonHelper.GetDecimal(item.Key.OWeigh) && CommonHelper.GetDecimal(p.OThick) == CommonHelper.GetDecimal(item.Key.OThick) && p.SteelDescription == item.Key.SteelDescription).FirstOrDefault().SteelStatus;
                //    by_inventory.PlaceOrigin = ListData.Where(p => CommonHelper.GetDecimal(p.OLength) == CommonHelper.GetDecimal(item.Key.OLength) && CommonHelper.GetDecimal(p.OWeigh) == CommonHelper.GetDecimal(item.Key.OWeigh) && CommonHelper.GetDecimal(p.OThick) == CommonHelper.GetDecimal(item.Key.OThick) && p.SteelDescription == item.Key.SteelDescription).FirstOrDefault().PlaceOrigin;
                //    by_inventory.StoveCode = ListData.Where(p => CommonHelper.GetDecimal(p.OLength) == CommonHelper.GetDecimal(item.Key.OLength) && CommonHelper.GetDecimal(p.OWeigh) == CommonHelper.GetDecimal(item.Key.OWeigh) && CommonHelper.GetDecimal(p.OThick) == CommonHelper.GetDecimal(item.Key.OThick) && p.SteelDescription == item.Key.SteelDescription).FirstOrDefault().StoveCode;
                    
                //    by_inventory.Density = item.Key.Density;
                //    by_inventory.QuotationCodeLine = item.Key.QuotationCodeLine;
                    
                    
                //    by_inventorys.Add(by_inventory);
                //}

                //return Content(by_inventorys.OrderBy(p=>p.SortCode).ToJson());
                return Content(ListData.OrderBy(p=>p.SortCode).ToJson());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }
        public ActionResult GetListJsonforReplace(string BatchCode, string InventoryType,string batchcodereplace)
        {
            try
            {
                List<BY_Inventory> ListData = by_inventorybll.GetListforReplace(BatchCode, InventoryType, batchcodereplace);
                return Content(ListData.ToJson());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }
        public ActionResult GetDistinctBatchCode(string BatchCode, string Enabled, string InventoryType="",string ParentInventoryId = "")
        {
            try
            {
                DataTable ListData = by_inventorybll.GetDistinctBatchCode(BatchCode, InventoryType, ParentInventoryId, Enabled);
                return Content(ListData.ToJson());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }
        public ActionResult GetSmoney()
        {
            try
            {
                DataTable ListData = by_inventorybll.GetSmoney();
                return Content(ListData.ToJson());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }
        public ActionResult GridPageListJson(int MaterialType, string SteelCode, string BatchCode, string Thick, string Weigh, string Length,string alarmck,string SteelPurchaseCode, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = by_inventorybll.GetPageList(MaterialType, SteelCode, BatchCode,  Thick,  Weigh,  Length,alarmck,SteelPurchaseCode, ref jqgridparam);
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
        /// 根据批号和子批号返回实体JONS
        /// </summary>
        /// <param name="BatchCode">批号</param>
        /// <param name="ChildBatch">子批号</param>
        /// <returns></returns>
        public ActionResult GetEntityJson(string BatchCode, string ChildBatch)
        {
            try
            {
                BY_Inventory entity = by_inventorybll.GetEntity(BatchCode, ChildBatch);
                return Content(entity.ToJson());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }
        public ActionResult SubmitInventoryForm(BY_Inventory by_inventory, string KeyValue)
        {
            try
            {
                IDatabase database = DataFactory.Database();
                BY_Inventory Inventory = new BY_Inventory();
                Inventory = database.FindEntity<BY_Inventory>(KeyValue);
                Inventory.Thick = by_inventory.Thick;
                Inventory.Weigh = by_inventory.Weigh;
                Inventory.Length = by_inventory.Length;
                Inventory.Weight = by_inventory.Weight;
                Inventory.BatchCode = by_inventory.BatchCode;

                Inventory.StoveCode = by_inventory.StoveCode;
                Inventory.Remark = by_inventory.Remark;
                Inventory.PlaceOrigin = by_inventory.PlaceOrigin;
                Inventory.SteelStatus = by_inventory.SteelStatus;
                Inventory.position = by_inventory.position;


                Inventory.Modify(Inventory.InventoryId);
                string Message = "修改成功。";
                DataFactory.Database().Update<BY_Inventory>(Inventory);
                return Content(new JsonMessage { Success = true, Code = "1", Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：编号重复！" + ex.Message }.ToString());
            }
        }
    }
}