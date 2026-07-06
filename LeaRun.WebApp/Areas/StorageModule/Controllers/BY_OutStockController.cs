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
    /// 出仓表控制器
    /// </summary>
    public class BY_OutStockController : PublicController<BY_OutStock>
    {
        class tempclass
        {
            public string orderdetailid { get; set; }
            public string ordercode { get; set; }
            public int? linecode { get; set; }
            public string steelcode { get; set; }
            public string steeldescription { get; set; }
            public string customername { get; set; }
            public string customercode { get; set; }
            public string position { get; set; }

            public decimal? thick { get; set; }
            public decimal? weigh { get; set; }
            public decimal? length { get; set; }
            public decimal? weight { get; set; }

            public int? qty { get; set; }
            public int? openqty { get; set; }
            public string batchcode { get; set; }
            public decimal? density { get; set; }

            public decimal? olength { get; set; }
            public decimal? oweigh { get; set; }
            public decimal? othick { get; set; }
            public decimal? oweight { get; set; }

            public decimal? opensize { get; set; }
            public decimal? nowlength { get; set; }
            public decimal? nowweight { get; set; }
            public decimal? openthick { get; set; }


        }

        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public ActionResult SubmitScaleForm2(string postData)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                string BatchCodes = "";
                List<tempclass> tempclasss = (postData.JonsToList<tempclass>()).ToList<tempclass>();
                foreach (var item in tempclasss)
                {
                    BatchCodes += "'"+item.batchcode+"',";
                }
                foreach (var item in tempclasss)
                {
                    BatchCodes += "'"+item.steelcode+"',";
                }
                BatchCodes = BatchCodes.Trim(',');
                List<BY_Inventory> by_inventorys = database.FindListBySql<BY_Inventory>("select * from BY_Inventory where batchcode in ("+BatchCodes+") ");
                List<BY_OutStock> by_outstockdeletes = new List<BY_OutStock>();

                List<BY_OrderDetailProcessDetail> by_orderdetailprocessdetails = new List<BY_OrderDetailProcessDetail>();

                List<BY_OrderDetail> by_orderdetails = new List<BY_OrderDetail>();
                foreach (tempclass tempclass in tempclasss)
                {
                    if (by_orderdetails.Count == 0)
                    {
                        by_orderdetails = DataFactory.Database().FindList<BY_OrderDetail>("OrderCode", tempclass.ordercode);
                        by_outstockdeletes = DataFactory.Database().FindListBySql<BY_OutStock>(string.Format("select * from BY_OutStock where OrderCode = '{0}'", tempclass.ordercode));
                        by_orderdetailprocessdetails = DataFactory.Database().FindListBySql<BY_OrderDetailProcessDetail>(string.Format("select * from BY_OrderDetailProcessDetail where OrderCode = '{0}'", tempclass.ordercode));
                    }
                    if (tempclass.nowlength < -100 || tempclass.nowweight < -50)
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：此钢材料号剩余长度和重量过小，请检查！" }.ToString());
                    }
                    BY_OrderDetail by_orderdetail = new BY_OrderDetail();
                    if (!string.IsNullOrEmpty(tempclass.orderdetailid))
                    {
                        by_orderdetail = by_orderdetails.Where(p => p.OrderDetailId == tempclass.orderdetailid).FirstOrDefault();
                        by_orderdetail.OpenQty = by_orderdetail.OpenQty + tempclass.openqty;
                        by_orderdetail.Modify(by_orderdetail.OrderDetailId);
                        database.Update<BY_OrderDetail>(by_orderdetail, isOpenTrans);

                        //BY_OrderDetailProcessDetail BY_OrderDetailProcessDetail = by_orderdetailprocessdetails.Where(p => p.OrderDetailId == tempclass.orderdetailid).FirstOrDefault();//DataFactory.Database().FindEntity<BY_OrderDetailProcessDetail>("OrderDetailId", tempclass.orderdetailid);
                        //if (BY_OrderDetailProcessDetail != null && BY_OrderDetailProcessDetail.OrderDetailProcessDetailId != null)
                        //{
                        //    BY_OutStock by_outstockdelete = by_outstockdeletes.Where(p => p.OrderDetailProcessDetailId == BY_OrderDetailProcessDetail.OrderDetailProcessDetailId).FirstOrDefault() ;//DataFactory.Database().FindEntity<BY_OutStock>("OrderDetailProcessDetailId", BY_OrderDetailProcessDetail.OrderDetailProcessDetailId);
                        //    if (by_outstockdelete != null && by_outstockdelete.OutStockId != null)
                        //    {
                        //        by_outstockdelete.Enabled = -1;
                        //        by_outstockdelete.Modify(by_outstockdelete.OutStockId);
                        //        database.Update<BY_OutStock>(by_outstockdelete, isOpenTrans);
                        //    }
                        //}
                    }
                    else
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：请输入订单号" }.ToString());
                    }

                    decimal? _Weight = 0;
                    if (tempclass.thick != 0 && tempclass.length != 0 && tempclass.weigh == 0)
                    {
                        _Weight = (((((tempclass.thick ) / 2) * ((tempclass.thick ) / 2) * ((tempclass.length))) * CommonHelper.GetDecimal(7.85) * CommonHelper.GetDecimal(3.14)) / 1000000);
                    }
                    else
                    {
                        _Weight = ((((tempclass.thick ) * (tempclass.weigh) * (tempclass.length)) * CommonHelper.GetDecimal(7.85)) / 1000000);
                    }


                    BY_OutStock outstock = new BY_OutStock()
                    {
                        OrderDetailId = tempclass.orderdetailid,
                        Qty = tempclass.openqty,
                        Length = tempclass.length,
                        Weigh = tempclass.weigh,
                        Thick = tempclass.thick,
                        Weight = tempclass.oweight - tempclass.nowweight,
                        TheoryWeight = _Weight,
                        CustomerCode = tempclass.customercode,
                        CustomerName = tempclass.customername,
                        OrderCode = tempclass.ordercode,
                        LineCode = tempclass.linecode,
                        OpenQty = tempclass.openqty,
                        Density = tempclass.density,
                        Olength = tempclass.olength,
                        Oweigh = tempclass.oweigh,
                        Othick = tempclass.othick,
                        Oweight = tempclass.oweight,
                        Opensize = tempclass.opensize,
                        NowLength = tempclass.nowlength,
                        NowWeight = tempclass.nowweight,
                        OpenThick = tempclass.openthick
                    };
                    string BatchCode = tempclass.batchcode;
                    decimal? OpenSize = tempclass.opensize;



                    //List<DbParameter> parameter = new List<DbParameter>();
                    //parameter.Add(DbFactory.CreateDbParameter("@OrderCode", by_orderdetail.OrderCode));
                    //parameter.Add(DbFactory.CreateDbParameter("@LineCode", by_orderdetail.LineCode));
                    //                    IList<BY_OrderDetailProcessDetail> by_orderdetailprocessdetails = database.FindListBySql<BY_OrderDetailProcessDetail>(@"SELECT * FROM dbo.BY_OrderDetailProcessDetail odp
                    //WHERE odp.OrderCode = @OrderCode AND odp.LineCode = @LineCode AND odp.TechnologyId='K001' ", parameter.ToArray());
                    BY_Inventory by_inventory = by_inventorys.Where(p => p.BatchCode == BatchCode).FirstOrDefault();//database.FindEntity<BY_Inventory>("BatchCode", BatchCode);
                    string outCode = by_inventory.BatchCode;
                    string Message = "开料成功。";
                    outstock.Create();
                    outstock.SteelId = by_inventory.SteelId;
                    outstock.SteelStatus = by_inventory.SteelStatus;
                    outstock.SteelType = by_inventory.SteelType;
                    outstock.SteelCode = by_inventory.SteelCode;
                    outstock.SteelDescription = by_inventory.SteelDescription;
                    outstock.SteelOtherDescription = by_inventory.SteelOtherDescription;
                    outstock.StockId = by_inventory.InventoryId;
                    outstock.BillCode = outCode;
                    outstock.BatchCode = outCode;
                    outstock.SteelPurchaseCode = by_inventory.SteelPurchaseCode;
                    outstock.StoveCode = by_inventory.StoveCode;
                    outstock.Enabled = 1;
                    outstock.OutStockType = "订单出仓";
                    if (by_inventory.Weigh > 0)
                    {
                        outstock.OriginalSize = CommonHelper.GetDecimal(by_inventory.Length) + "*" + CommonHelper.GetDecimal(by_inventory.Weigh) + "*" + CommonHelper.GetDecimal(by_inventory.Thick);
                        outstock.NowSize = CommonHelper.GetDecimal(tempclass.nowlength) + "*" + CommonHelper.GetDecimal(by_inventory.Weigh) + "*" + CommonHelper.GetDecimal(by_inventory.Thick);
                    }
                    else
                    {
                        outstock.OriginalSize = "Φ" + CommonHelper.GetDecimal(by_inventory.Thick) + "*" + CommonHelper.GetDecimal(by_inventory.Length);
                        outstock.NowSize = "Φ" + CommonHelper.GetDecimal(by_inventory.Thick) + "*" + CommonHelper.GetDecimal(tempclass.nowlength);
                    }


                    if (tempclass.opensize  == 0)
                    {
                        outstock.NowWeight = 0;
                    }
                    else
                    {
                        outstock.OriginalWeight = by_inventory.Weight;
                    }


                    by_inventory.Length = tempclass.nowlength;
                    by_inventory.Weight = tempclass.nowweight;
                    by_inventory.position = tempclass.position;
                    by_inventory.Modify(by_inventory.InventoryId);
                    database.Update<BY_Inventory>(by_inventory, isOpenTrans);

                    //
                    if (outstock.Othick != 0 && outstock.Oweigh != 0 && tempclass.opensize != 0)
                    {
                        

                        //BY_Inventory by_inventorytype = by_inventorys.Where(p => p.BatchCode == outstock.SteelCode).FirstOrDefault(); //database.FindEntity<BY_Inventory>("BatchCode", outstock.SteelCode);
                        //by_inventorytype.Weight = by_inventorytype.Weight + tempclass.oweight - tempclass.nowweight - outstock.TheoryWeight;
                        //by_inventorytype.Modify(by_inventorytype.InventoryId);
                        //database.Update<BY_Inventory>(by_inventorytype, isOpenTrans);
                    }
                    else if (outstock.Othick != 0 && outstock.Oweigh != 0 && tempclass.opensize == 0)
                    {
                        //BY_Inventory by_inventorytype = by_inventorys.Where(p => p.BatchCode == outstock.SteelCode).FirstOrDefault(); //database.FindEntity<BY_Inventory>("BatchCode", outstock.SteelCode);
                        //by_inventorytype.Weight = by_inventorytype.Weight - outstock.TheoryWeight;
                        //by_inventorytype.Modify(by_inventorytype.InventoryId);
                        //database.Update<BY_Inventory>(by_inventorytype, isOpenTrans);
                    }
                    
                    database.Insert<BY_OutStock>(outstock, isOpenTrans);

                }
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功！" }.ToString());

            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }



        BY_OrderCompleteBll by_ordercompletebll = new BY_OrderCompleteBll();
        public ActionResult WriteWeihtdo(string Condition, JqGridParam jqgridparam)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = by_ordercompletebll.WriteWeihtdo(QueryParameter, ref jqgridparam);

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
                database.Rollback();
                return null;
            }
        }


        BY_InStockBll by_instockbll = new BY_InStockBll();
        BY_OutStockBll by_outstockbll = new BY_OutStockBll();

        #region 订单条料出仓
        /// <summary>
        /// 订单条料出仓列表页面
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult BoardIndex()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult OutStockIndex()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult ChangeOuter()
        {
            return View();
        }

        /// <summary>
        /// 修改材料出仓
        /// </summary>
        /// <param name="OutStockId"></param>
        /// <param name="Qty"></param>
        /// <param name="Weight"></param>
        /// <param name="steelcode1"></param>
        /// <param name="steelcode2"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeOuterSubmit(string OutStockId, string Qty, string Weight, string steelcode1, string steelcode2)
        {
            BY_OutStock by_outstock = by_outstockbll.Repository().FindEntity(OutStockId);
            BY_Inventory by_inventory1 = DataFactory.Database().FindEntity<BY_Inventory>("BatchCode", steelcode1);
            BY_Inventory by_inventory2 = DataFactory.Database().FindEntity<BY_Inventory>("BatchCode", steelcode2);
            Base_Steel base_steel = DataFactory.Database().FindEntity<Base_Steel>("SteelCode", steelcode2);
            BY_OrderDetailProcessDetail by_orderdetailprocessdetail = DataFactory.Database().FindEntityByWhere<BY_OrderDetailProcessDetail>(string.Format(" AND TechnologyId='K001' AND OrderDetailId='{0}'  AND Qty='{1}' AND Weight='{2}'", by_outstock.OrderDetailId, Convert.ToInt32(by_outstock.Qty), by_outstock.Weight));
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                if (by_outstock != null && by_outstock.OutStockId != null && by_inventory1 != null && by_inventory1.InventoryId != null && by_inventory2 != null && by_inventory2.InventoryId != null && base_steel != null && base_steel.SteelId != null)
                {
                    by_inventory1.Modify(by_inventory1.InventoryId);
                    by_inventory1.Weight = by_inventory1.Weight + by_outstock.Weight; // - Convert.ToDecimal(Weight)
                    by_inventory2.Modify(by_inventory2.InventoryId);
                    by_inventory2.Weight = by_inventory2.Weight - Convert.ToDecimal(Weight);
                    if (by_inventory1.InventoryId == by_inventory2.InventoryId)
                    {
                        by_inventory2.Weight = by_inventory2.Weight + by_outstock.Weight;
                    }
                    by_orderdetailprocessdetail.Modify(by_orderdetailprocessdetail.OrderDetailProcessDetailId);
                    by_orderdetailprocessdetail.Qty = Convert.ToDecimal(Qty);
                    by_orderdetailprocessdetail.Weight = Convert.ToDecimal(Weight);

                    by_outstock.Modify(OutStockId);
                    by_outstock.Qty = Convert.ToDecimal(Qty);
                    by_outstock.Weight = Convert.ToDecimal(Weight);
                    by_outstock.SteelCode = base_steel.SteelCode;
                    by_outstock.BillCode = base_steel.SteelCode;
                    by_outstock.SteelId = base_steel.SteelId;
                    by_outstock.SteelDescription = base_steel.SteelName;
                    by_outstock.SteelOtherDescription = base_steel.OtherDescription;
                    by_outstock.SteelType = base_steel.SteelType;
                    DataFactory.Database().Update<BY_OutStock>(by_outstock, isOpenTrans);
                    DataFactory.Database().Update<BY_Inventory>(by_inventory1, isOpenTrans);
                    DataFactory.Database().Update<BY_Inventory>(by_inventory2, isOpenTrans);
                    DataFactory.Database().Update<BY_OrderDetailProcessDetail>(by_orderdetailprocessdetail, isOpenTrans);
                    database.Commit();
                    return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功！" }.ToString());
                }
                return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败!" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }

        }
        /// <summary>
        /// 提交订单条料出仓表单
        /// </summary>
        /// <param name="InStockListJson">入库列表</param>
        /// <param name="KeyValue">库存ID</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public ActionResult SubmitBoardForm(string InStockListJson, BY_OutStock by_outstock, string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            //取出所有碎料
            List<BY_Inventory> by_inventorys = database.FindList<BY_Inventory>("MaterialType", "1");
            BY_Inventory parentby_inventory = database.FindEntity<BY_Inventory>(KeyValue);
            List<BY_InStock> BY_InStockList = new List<BY_InStock>();
            BY_OrderDetail by_orderdetail = new BY_OrderDetail();
            string outCode = parentby_inventory.BatchCode;//by_outstockbll.GetOutStockCode(parentby_inventory.SteelCode, parentby_inventory.SteelType, parentby_inventory.SteelStatus);
            //更新订单已开料数
            if (!string.IsNullOrEmpty(by_outstock.OrderDetailId))
            {
                by_orderdetail = database.FindEntity<BY_OrderDetail>(by_outstock.OrderDetailId);
                if (by_orderdetail.OpenQty + by_outstock.Qty > by_orderdetail.Qty)
                {
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：现此订单行最多开料" + (by_orderdetail.Qty - by_orderdetail.OpenQty).ToString() + "件！" }.ToString());
                }
                by_orderdetail.OpenQty = CommonHelper.GetDecimal(by_orderdetail.OpenQty) + by_outstock.Qty;
            }
            if (!string.IsNullOrEmpty(InStockListJson))
            {
                BY_InStockList = InStockListJson.JonsToList<BY_InStock>();
            }
            //先写入入库单号，后面开启事务了，所以得先写入单号
            foreach (BY_InStock by_instock in BY_InStockList)
            {
                by_instock.Remark = parentby_inventory.Remark;
                by_instock.FactoryCode = parentby_inventory.FactoryCode;
                by_instock.Price = parentby_inventory.Price;
                by_instock.StoveCode = parentby_inventory.StoveCode;
                //by_instock.InStockType = 2;
                by_instock.SteelStatus = by_instock.SteelStatus.Substring(0, 1);
                by_instock.SteelType = parentby_inventory.SteelType;
                by_instock.SteelId = parentby_inventory.SteelId;
                by_instock.SteelDescription = parentby_inventory.SteelDescription;
                by_instock.SteelCode = parentby_inventory.SteelCode;
                by_instock.PlaceOrigin = parentby_inventory.PlaceOrigin;
                by_instock.Qty = 1;
                by_instock.InStockDate = DateTime.Now;
                by_instock.BillCode = parentby_inventory.BatchCode;
            }
            //BY_InStockList = by_instockbll.GetInStockCode(BY_InStockList);


            try
            {
                DbTransaction isOpenTrans = database.BeginTrans();
                string Message = "出仓成功。";
                foreach (BY_InStock by_instock in BY_InStockList)
                {
                    by_instock.Create();
                    database.Insert<BY_InStock>(by_instock, isOpenTrans);
                    //判断是条料还是碎料
                    if (by_instock.Weigh > 200 && by_instock.Length > 200)
                    {
                        #region 条料写入库存表
                        BY_Inventory by_inventory = new BY_Inventory();
                        by_inventory.BatchCode = parentby_inventory.BatchCode;//继承父批次批号
                        by_inventory.ChildBatch = by_instock.Length.ToString() + "×" + by_instock.Weigh.ToString() + "×" + by_instock.Thick.ToString();
                        by_inventory.Thick = by_instock.Thick;
                        by_inventory.Weigh = by_instock.Weigh;
                        by_inventory.Length = by_instock.Length;
                        by_inventory.DeleteMark = 0;
                        by_inventory.Enabled = 1;
                        by_inventory.MaterialType = 0;
                        by_inventory.Qty = 1;
                        by_inventory.SteelId = parentby_inventory.SteelId;
                        by_inventory.SteelStatus = parentby_inventory.SteelStatus;
                        by_inventory.SteelType = parentby_inventory.SteelType;
                        by_inventory.SteelCode = parentby_inventory.SteelCode;
                        by_inventory.SteelDescription = parentby_inventory.SteelDescription;
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
                        by_inventory.Create();
                        database.Insert<BY_Inventory>(by_inventory, isOpenTrans);
                        #endregion
                    }
                    else
                    {
                        //碎料入库
                        #region 碎料写入库存表
                        string batchCode = parentby_inventory.SteelCode;//by_instock.SteelCode + "-" + by_instock.SteelType + by_instock.SteelStatus;
                        //BY_Inventory by_inventory = database.FindEntity<BY_Inventory>("BatchCode", batchCode);
                        BY_Inventory by_inventory = (from entity in by_inventorys
                                                     where entity.BatchCode == batchCode
                                                     select entity).FirstOrDefault<BY_Inventory>();
                        if (by_inventory == null)
                        {
                            by_inventory = new BY_Inventory();
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
                            by_inventory.TheoryWeight = by_instock.TheoryWeight;
                            by_inventory.Price = by_instock.Price;
                            by_inventory.InStockDate = DateTime.Now;
                            by_inventory.Create();
                            database.Insert<BY_Inventory>(by_inventory, isOpenTrans);
                        }
                        else
                        {
                            by_inventory.Weight = by_inventory.Weight + by_instock.Weight;
                            by_inventory.TheoryWeight = by_inventory.TheoryWeight + by_instock.TheoryWeight;
                            by_inventory.Modify(by_inventory.InventoryId);
                            database.Update<BY_Inventory>(by_inventory, isOpenTrans);
                        }
                        #endregion
                    }
                }
                decimal? totalweight = BY_InStockList.Sum<BY_InStock>(i => i.Weight);
                InputdetailprocessDetail(by_orderdetail, "K001", "开料", parentby_inventory.Weight - totalweight, by_outstock.Qty, isOpenTrans);
                //入库完成后作出库操作
                #region 出库操作
                by_outstock.Create();

                //by_outstock.OutStockType = 0;
                by_outstock.Remark = "";
                by_outstock.Qty = parentby_inventory.Qty;
                by_outstock.SteelId = parentby_inventory.SteelId;
                by_outstock.SteelCode = parentby_inventory.SteelCode;
                by_outstock.SteelDescription = parentby_inventory.SteelDescription;
                by_outstock.SteelOtherDescription = parentby_inventory.SteelOtherDescription;
                by_outstock.TheoryWeight = parentby_inventory.TheoryWeight;
                by_outstock.SteelStatus = parentby_inventory.SteelStatus;
                by_outstock.SteelType = parentby_inventory.SteelType;
                by_outstock.Weight = parentby_inventory.Weight;
                by_outstock.Length = parentby_inventory.Length;
                by_outstock.Weigh = parentby_inventory.Weigh;
                by_outstock.Thick = parentby_inventory.Thick;
                by_outstock.StockId = parentby_inventory.InventoryId;
                by_outstock.BillCode = outCode;
                database.Insert<BY_OutStock>(by_outstock, isOpenTrans);
                database.Delete<BY_Inventory>(parentby_inventory, isOpenTrans);
                #endregion
                #region 回写已开料数
                if (!string.IsNullOrEmpty(by_orderdetail.OrderDetailId))
                {
                    database.Update<BY_OrderDetail>(by_orderdetail, isOpenTrans);
                }
                #endregion


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

        #region 订单碎料出仓
        /// <summary>
        /// 订单碎料出仓列表页面
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult BrokenIndex()
        {
            return View();
        }
        /// <summary>
        /// 提交订单碎料出仓表单
        /// </summary>
        /// <param name="InStockListJson">入库列表</param>
        /// <param name="KeyValue">库存ID</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public ActionResult SubmitBrokenForm(string OutStockListJson, BY_OutStock outstock, string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            List<BY_OutStock> BY_OutStockList = OutStockListJson.JonsToList<BY_OutStock>();
            BY_Inventory by_inventory = database.FindEntity<BY_Inventory>(KeyValue);
            string outCode = by_inventory.BatchCode;//by_outstockbll.GetOutStockCode(by_inventory.SteelCode, by_inventory.SteelType, by_inventory.SteelStatus);
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                BY_OrderDetail by_orderdetail = new BY_OrderDetail();
                //更新订单已开料数
                if (!string.IsNullOrEmpty(outstock.OrderDetailId))
                {
                    by_orderdetail = database.FindEntity<BY_OrderDetail>(outstock.OrderDetailId);

                    if (by_orderdetail.OpenQty + outstock.Qty > by_orderdetail.Qty)
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：现此订单行最多开料" + (by_orderdetail.Qty - by_orderdetail.OpenQty).ToString() + "件！" }.ToString());
                    }
                    by_orderdetail.OpenQty = CommonHelper.GetDecimal(by_orderdetail.OpenQty) + outstock.Qty;
                }
                database.Update<BY_OrderDetail>(by_orderdetail, isOpenTrans);
                string Message = "出仓成功。";

                //入库完成后作出库操作
                #region 出库操作
                foreach (BY_OutStock by_outstock in BY_OutStockList)
                {
                    by_outstock.Create();

                    //by_outstock.OutStockType = 1;
                    //by_outstock.Qty = 1;
                    by_outstock.OrderDetailId = outstock.OrderDetailId;
                    by_outstock.SteelCode = by_inventory.SteelCode;
                    by_outstock.SteelDescription = by_inventory.SteelDescription;
                    by_outstock.StockId = by_inventory.InventoryId;
                    by_outstock.Remark = "";
                    by_outstock.SteelId = by_inventory.SteelId;
                    by_outstock.SteelStatus = by_inventory.SteelStatus;
                    by_outstock.SteelType = by_inventory.SteelType;
                    by_outstock.BillCode = outCode;
                    database.Insert<BY_OutStock>(by_outstock, isOpenTrans);
                    by_inventory.Weight -= by_outstock.Weight;
                }
                if (by_inventory.Weight < 0)
                {
                    database.Rollback();
                    return Content(new JsonMessage { Success = true, Code = "1", Message = "出仓重量大于库存重量，请重新输入！" }.ToString());
                }
                decimal? totalweight = BY_OutStockList.Sum<BY_OutStock>(i => i.Weight);
                InputdetailprocessDetail(by_orderdetail, "K001", "开料", totalweight, outstock.Qty, isOpenTrans);
                database.Update<BY_Inventory>(by_inventory, isOpenTrans);
                #endregion
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

        #region 订单过磅
        /// <summary>
        /// 过磅界面
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult ScaleIndex()
        {
            IManageUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<IManageUser>(DESEncrypt.Decrypt(CookieHelper.GetCookie("LoginUserKey")));
            if (user == null)
            {
                string outmsg = "";
                Base_UserBll base_userbll = new Base_UserBll();
                Base_ObjectUserRelationBll base_objectuserrelationbll = new Base_ObjectUserRelationBll();
                DNS.WebApp.Controllers.LoginController logincontroller = new WebApp.Controllers.LoginController();
                //logincontroller.CheckLogin("Common","d13bc5b68b2bd9e18f29777db17cc563",null);
                Base_User base_user = base_userbll.UserLogin("Common", "d13bc5b68b2bd9e18f29777db17cc563", out outmsg);
                IManageUser imanageuser = new IManageUser();
                imanageuser.UserId = base_user.UserId;
                imanageuser.Account = base_user.Account;
                imanageuser.UserName = base_user.RealName;
                imanageuser.Gender = base_user.Gender;
                imanageuser.Password = base_user.Password;
                imanageuser.Code = base_user.Code;
                imanageuser.Secretkey = base_user.Secretkey;
                imanageuser.LogTime = DateTime.Now;
                imanageuser.CompanyId = base_user.CompanyId;
                imanageuser.CompanyName = DataFactory.Database().FindEntity<Base_Company>(base_user.CompanyId).FullName;
                imanageuser.DepartmentId = base_user.DepartmentId;
                imanageuser.ObjectId = base_objectuserrelationbll.GetObjectId(imanageuser.UserId);
                imanageuser.IPAddress = "";
                imanageuser.IPAddressName = "";
                imanageuser.IsSystem = false;
                ManageProvider.Provider.AddCurrent(imanageuser);
            }
            string _ModuleId = DESEncrypt.Encrypt("81e540e5-34f0-45d2-bf80-189812fd73f4");
            CookieHelper.WriteCookie("ModuleId", _ModuleId);
            return View();
        }

        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult ScaleIndex2()
        {
            return View();
        }


        /// <summary>
        /// 提交订单碎料出仓表单
        /// </summary>
        /// <param name="InStockListJson">入库列表</param>
        /// <param name="KeyValue">库存ID</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public ActionResult SubmitScaleForm(BY_OutStock outstock, string BatchCode, decimal OpenSize)
        {


            List<DbParameter> parameter = new List<DbParameter>();
            IDatabase database = DataFactory.Database();
            BY_Inventory by_inventory = database.FindEntity<BY_Inventory>("BatchCode", BatchCode);
            BY_OrderDetail by_orderdetail = new BY_OrderDetail();
            string outCode = by_inventory.BatchCode; //by_outstockbll.GetOutStockCode(by_inventory.SteelCode, by_inventory.SteelType, by_inventory.SteelStatus);
            //更新订单已开料数
            if (!string.IsNullOrEmpty(outstock.OrderDetailId))
            {
                by_orderdetail = database.FindEntity<BY_OrderDetail>(outstock.OrderDetailId);
                if (by_orderdetail.OpenQty + outstock.Qty > by_orderdetail.Qty)
                {
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：现此订单行最多开料" + (by_orderdetail.Qty - by_orderdetail.OpenQty).ToString() + "件！" }.ToString());
                }
                by_orderdetail.OpenQty = CommonHelper.GetDecimal(by_orderdetail.OpenQty) + outstock.Qty;
                by_orderdetail.WeightFact += outstock.Weight;
            }
            else
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：请输入订单号" }.ToString());
            }

            BY_Order by_order = database.FindEntity<BY_Order>(by_orderdetail.OrderId);
            BY_Inventory by_inventoryTEMP = database.FindEntity<BY_Inventory>("QuotationCodeLine", by_order.QuotationCode + "-" + by_orderdetail.LineCode);
            if (by_inventoryTEMP.InventoryId != null)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：规格料不用消数！" }.ToString());
            }

            if (CommonHelper.GetDecimal(by_inventory.Weight) < outstock.Weight)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：库存不够" }.ToString());
            }

            if (outstock.Length > 0 && outstock.Thick > 0 && outstock.Weigh > 0)
            {
                if (by_inventory.Length > 0 && by_inventory.Thick > 0 && by_inventory.Weigh == 0)
                {
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：不能使用圆钢，请使用条钢！" }.ToString());
                }
            }
            if (outstock.Length > 0 && outstock.Thick > 0 && outstock.Weigh == 0)
            {
                if (by_inventory.Length > 0 && by_inventory.Thick > 0 && by_inventory.Weigh > 0)
                {
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：不能使用条钢，请使用圆钢！" }.ToString());
                }
            }

            parameter.Add(DbFactory.CreateDbParameter("@OrderCode", by_orderdetail.OrderCode));
            parameter.Add(DbFactory.CreateDbParameter("@LineCode", by_orderdetail.LineCode));
            IList<BY_OrderDetailProcessDetail> by_orderdetailprocessdetails = database.FindListBySql<BY_OrderDetailProcessDetail>(@"SELECT * FROM dbo.BY_OrderDetailProcessDetail odp
WHERE odp.OrderCode = @OrderCode AND odp.LineCode = @LineCode AND (odp.TechnologyId = 'K001' or odp.TechnologyName = '锯床' or odp.TechnologyId = 'FG' or odp.TechnologyName = '风割' ) AND odp.IsCKXS = 0", parameter.ToArray());


            DbTransaction isOpenTrans = database.BeginTrans();
            database.Update<BY_OrderDetail>(by_orderdetail, isOpenTrans);
            try
            {
                string Message = "开料成功。";
                //入库完成后作出库操作
                #region 出库操作
                outstock.Create();
                if (by_orderdetailprocessdetails.Count > 0)
                {
                    outstock.CreateUserId = by_orderdetailprocessdetails.FirstOrDefault<BY_OrderDetailProcessDetail>().OperatorId;
                    outstock.CreateUserName = by_orderdetailprocessdetails.FirstOrDefault<BY_OrderDetailProcessDetail>().OperatorName;
                }
                //outstock.BillCode = by_inventory.BatchCode + DateTime.Now.ToString("yyyyMMdd");
                //outstock.OutStockType = 4;

                //outstock.Qty = 1;
                outstock.SteelId = by_inventory.SteelId;
                outstock.SteelStatus = by_inventory.SteelStatus;
                outstock.SteelType = by_inventory.SteelType;
                outstock.SteelCode = by_inventory.SteelCode;
                outstock.SteelDescription = by_inventory.SteelDescription;
                outstock.SteelOtherDescription = by_inventory.SteelOtherDescription;
                outstock.StockId = by_inventory.InventoryId;
                //outstock.TheoryWeight = outstock.Thick * outstock.Length * outstock.Weigh * 0 / 1000000;
                outstock.BillCode = outCode;

                outstock.CustomerId = by_inventory.CustomerId;
                outstock.CustomerCode = by_inventory.CustomerCode;
                outstock.CustomerName = by_inventory.CustomerName;



                if (by_inventory.MaterialType == 1)
                {
                    by_inventory.Weight -= outstock.Weight;
                    outstock.OriginalSize = "";
                    outstock.Remark = "";
                }
                else if ((by_inventory.Thick != 0 && by_inventory.Weigh == 0 && by_inventory.Length != 0))
                {
                    outstock.OriginalSize = "Φ" + by_inventory.Thick + " * " + by_inventory.Length;
                    by_inventory.Weight -= outstock.Weight;
                    by_inventory.Length -= OpenSize;
                    outstock.Remark = "Φ" + by_inventory.Thick + " * " + by_inventory.Length;
                }
                else
                {
                    decimal OpenWeight = CommonHelper.GetDecimal(by_inventory.Thick) * CommonHelper.GetDecimal(by_inventory.Weigh) * CommonHelper.GetDecimal(OpenSize) * CommonHelper.GetDecimal(by_inventory.Density) / CommonHelper.GetDecimal(1000000);
                    decimal NewLength = CommonHelper.GetDecimal(by_inventory.Length) - CommonHelper.GetDecimal(OpenSize);

                    outstock.OriginalSize = by_inventory.Thick + " * " + by_inventory.Weigh + " * " + by_inventory.Length;
                    by_inventory.Length = NewLength;
                    outstock.Remark = by_inventory.Thick + " * " + by_inventory.Weigh + " * " + by_inventory.Length;
                    by_inventory.Weight -= OpenWeight;
                    by_inventory.TheoryWeight -= OpenWeight;

                    //if (OpenWeight - outstock.Weight < 0)
                    //{
                    //    database.Rollback();
                    //    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：所开尺寸重量小于工件重量！"   }.ToString());
                    //}

                    decimal? Openww = OpenWeight - outstock.Weight > 0 ? OpenWeight - outstock.Weight : 0;

                    BY_Inventory by_inventoryReturn = database.FindEntity<BY_Inventory>("BatchCode", by_inventory.SteelCode);
                    if (by_inventoryReturn != null && by_inventoryReturn.InventoryId != null)
                    {
                        by_inventoryReturn.Weight += Openww;
                        database.Update<BY_Inventory>(by_inventoryReturn, isOpenTrans);
                    }
                    else
                    {
                        by_inventoryReturn.BatchCode = by_inventory.SteelCode;
                        by_inventoryReturn.ChildBatch = "0×0×0";
                        by_inventoryReturn.DeleteMark = 0;
                        by_inventoryReturn.Enabled = 1;
                        by_inventoryReturn.MaterialType = 1;
                        by_inventoryReturn.SteelId = by_inventory.SteelId;
                        by_inventoryReturn.SteelStatus = by_inventory.SteelStatus;
                        by_inventoryReturn.SteelType = "S";
                        by_inventoryReturn.SteelCode = by_inventory.SteelCode;
                        by_inventoryReturn.SteelDescription = by_inventory.SteelDescription;
                        by_inventoryReturn.SteelOtherDescription = by_inventory.SteelOtherDescription;
                        by_inventoryReturn.Weight = Openww;
                        by_inventoryReturn.InStockDate = DateTime.Now;
                        by_inventoryReturn.Create();
                        if (Openww > 0)
                        {
                            database.Insert<BY_Inventory>(by_inventoryReturn, isOpenTrans);
                        }
                    }
                    //碎料入仓
                    BY_InStock by_instock = new BY_InStock();
                    //by_instock.InStockType = 2;
                    by_instock.SteelStatus = by_inventory.SteelStatus;
                    by_instock.SteelType = "S";
                    by_instock.SteelId = by_inventory.SteelId;
                    by_instock.SteelDescription = by_inventory.SteelDescription;
                    by_instock.SteelCode = by_inventory.SteelCode;
                    by_instock.PlaceOrigin = by_inventory.PlaceOrigin;
                    by_instock.Qty = 1;
                    by_instock.InStockDate = DateTime.Now;
                    by_instock.BillCode = by_inventory.BatchCode;
                    by_instock.Remark = by_orderdetail.OrderCode + "-" + by_orderdetail.LineCode.ToString();
                    by_instock.Create();
                    by_instock.Weight = Openww;
                    if (Openww > 0)
                    {
                        database.Insert<BY_InStock>(by_instock, isOpenTrans);
                    }
                    //条料入仓
                    BY_InStock by_instockBoard = new BY_InStock();
                    //by_instockBoard.InStockType = 2;
                    by_instockBoard.SteelStatus = by_inventory.SteelStatus;
                    by_instockBoard.SteelType = by_inventory.Weigh == 0 ? "R" : "F";
                    by_instockBoard.SteelId = by_inventory.SteelId;
                    by_instockBoard.SteelDescription = by_inventory.SteelDescription;
                    by_instockBoard.SteelCode = by_inventory.SteelCode;
                    by_instockBoard.PlaceOrigin = by_inventory.PlaceOrigin;
                    by_instockBoard.Qty = 1;
                    by_instockBoard.InStockDate = DateTime.Now;
                    by_instockBoard.BillCode = by_inventory.BatchCode;
                    by_instockBoard.Remark = by_orderdetail.OrderCode + "-" + by_orderdetail.LineCode.ToString();
                    by_instockBoard.Create();
                    by_instockBoard.Weight = by_inventory.Weight;
                    by_instockBoard.CustomerId = by_inventory.CustomerId;
                    by_instockBoard.CustomerName = by_inventory.CustomerName;
                    by_instockBoard.CustomerCode = by_inventory.CustomerCode;
                    by_instockBoard.Thick = by_inventory.Thick;
                    by_instockBoard.Weigh = by_inventory.Weigh;
                    by_instockBoard.Length = by_inventory.Length;
                    by_instockBoard.OriginalSize = outstock.OriginalSize;
                    by_instockBoard.StoveCode = by_inventory.StoveCode;
                    database.Insert<BY_InStock>(by_instockBoard, isOpenTrans);
                }
                database.Insert<BY_OutStock>(outstock, isOpenTrans);
                database.Update<BY_Inventory>(by_inventory, isOpenTrans);

                foreach (BY_OrderDetailProcessDetail item in by_orderdetailprocessdetails)
                {
                    item.IsCKXS = 1;
                    database.Update<BY_OrderDetailProcessDetail>(item, isOpenTrans);
                }

                #endregion
                //InputdetailprocessDetail(by_orderdetail, "K001", "开料", outstock.Weight, outstock.Qty, by_worker, isOpenTrans);

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

        public void InputdetailprocessDetail(BY_OrderDetail by_orderdetail, string TechnologyId, string TechnologyName, decimal? Weight, decimal? Qty, BY_Worker by_worker, DbTransaction isOpenTrans)
        {
            #region detailprocess操作
            BY_OrderDetailProcessDetail by_orderdetailprocessdetail = new Entity.BY_OrderDetailProcessDetail();
            by_orderdetailprocessdetail.Create();
            List<DbParameter> parameter = new List<DbParameter>();
            parameter.Add(DbFactory.CreateDbParameter("@OrderCode", by_orderdetail.OrderCode));
            parameter.Add(DbFactory.CreateDbParameter("@LineCode", by_orderdetail.LineCode));
            parameter.Add(DbFactory.CreateDbParameter("@TechnologyId", TechnologyId));
            //检查工单工序
            BY_OrderDetailProcess by_orderdetailprocess = DataFactory.Database().FindEntityByWhere<BY_OrderDetailProcess>(" AND OrderCode=@OrderCode AND LineCode=@LineCode AND TechnologyId=@TechnologyId ", parameter.ToArray());
            if (by_orderdetailprocess != null && by_orderdetailprocess.OrderDetailProcessId != null)
            {
                by_orderdetailprocessdetail.OrderDetailProcessId = by_orderdetailprocess.OrderDetailProcessId;
            }
            by_orderdetailprocessdetail.OrderDetailId = by_orderdetail.OrderDetailId;
            by_orderdetailprocessdetail.OrderCode = by_orderdetail.OrderCode;
            by_orderdetailprocessdetail.LineCode = by_orderdetail.LineCode;
            by_orderdetailprocessdetail.TechnologyId = TechnologyId;
            by_orderdetailprocessdetail.TechnologyName = TechnologyName;
            by_orderdetailprocessdetail.MachineId = "";
            by_orderdetailprocessdetail.MachineName = "";
            by_orderdetailprocessdetail.OperatorId = by_worker.WorkerCode;
            by_orderdetailprocessdetail.OperatorName = by_worker.WorkerName;
            by_orderdetailprocessdetail.OperatorDate = DateTime.Now;
            by_orderdetailprocessdetail.Weight = Weight;
            by_orderdetailprocessdetail.Area = Math.Round(Convert.ToDecimal((by_orderdetail.Thick * by_orderdetail.Weigh + by_orderdetail.Thick * by_orderdetail.Length) * by_orderdetail.Qty / 1000000), 4, MidpointRounding.AwayFromZero);
            by_orderdetailprocessdetail.CalcLength = 0;
            by_orderdetailprocessdetail.Qty = Qty;
            by_orderdetailprocessdetail.Thick = by_orderdetail.Thick;
            by_orderdetailprocessdetail.Length = by_orderdetail.Length;
            by_orderdetailprocessdetail.Weigh = by_orderdetail.Weigh;
            by_orderdetailprocessdetail.SteelId = by_orderdetail.SteelId;
            by_orderdetailprocessdetail.SteelCode = by_orderdetail.SteelCode;
            by_orderdetailprocessdetail.SteelDescription = by_orderdetail.SteelDescription;
            by_orderdetailprocessdetail.SteelOtherDescription = by_orderdetail.SteelOtherDescription;
            by_orderdetailprocessdetail.SteelState = by_orderdetail.SteelState;
            BY_Order by_order = DataFactory.Database().FindEntity<BY_Order>(by_orderdetail.OrderId);
            by_orderdetailprocessdetail.OrderType = by_order.OrderType;
            by_orderdetailprocessdetail = new BY_OrderDetailProcessDetailBll().GetClassesDateClassesShift(by_orderdetailprocessdetail);
            DataFactory.Database().Insert<BY_OrderDetailProcessDetail>(by_orderdetailprocessdetail, isOpenTrans);
            #endregion
        }

        public void InputdetailprocessDetail(BY_OrderDetail by_orderdetail, string TechnologyId, string TechnologyName, decimal? Weight, decimal? Qty, DbTransaction isOpenTrans)
        {
            #region detailprocess操作
            BY_OrderDetailProcessDetail by_orderdetailprocessdetail = new Entity.BY_OrderDetailProcessDetail();
            by_orderdetailprocessdetail.Create();
            List<DbParameter> parameter = new List<DbParameter>();
            parameter.Add(DbFactory.CreateDbParameter("@OrderCode", by_orderdetail.OrderCode));
            parameter.Add(DbFactory.CreateDbParameter("@LineCode", by_orderdetail.LineCode));
            parameter.Add(DbFactory.CreateDbParameter("@TechnologyId", TechnologyId));
            //检查工单工序
            BY_OrderDetailProcess by_orderdetailprocess = DataFactory.Database().FindEntityByWhere<BY_OrderDetailProcess>(" AND OrderCode=@OrderCode AND LineCode=@LineCode AND TechnologyId=@TechnologyId ", parameter.ToArray());
            if (by_orderdetailprocess != null && by_orderdetailprocess.OrderDetailProcessId != null)
            {
                by_orderdetailprocessdetail.OrderDetailProcessId = by_orderdetailprocess.OrderDetailProcessId;
            }
            by_orderdetailprocessdetail.OrderDetailId = by_orderdetail.OrderDetailId;
            by_orderdetailprocessdetail.OrderCode = by_orderdetail.OrderCode;
            by_orderdetailprocessdetail.LineCode = by_orderdetail.LineCode;
            by_orderdetailprocessdetail.TechnologyId = TechnologyId;
            by_orderdetailprocessdetail.TechnologyName = TechnologyName;
            by_orderdetailprocessdetail.MachineId = "";
            by_orderdetailprocessdetail.MachineName = "";
            by_orderdetailprocessdetail.OperatorId = ManageProvider.Provider.Current().UserId;
            by_orderdetailprocessdetail.OperatorName = ManageProvider.Provider.Current().UserName;
            by_orderdetailprocessdetail.OperatorDate = DateTime.Now;
            by_orderdetailprocessdetail.Weight = Weight;
            by_orderdetailprocessdetail.Area = Math.Round(Convert.ToDecimal((by_orderdetail.Thick * by_orderdetail.Weigh + by_orderdetail.Thick * by_orderdetail.Length) / 1000000), 4, MidpointRounding.AwayFromZero);
            by_orderdetailprocessdetail.CalcLength = 0;
            by_orderdetailprocessdetail.Qty = Qty;
            by_orderdetailprocessdetail.Thick = by_orderdetail.Thick;
            by_orderdetailprocessdetail.Length = by_orderdetail.Length;
            by_orderdetailprocessdetail.Weigh = by_orderdetail.Weigh;
            by_orderdetailprocessdetail.SteelId = by_orderdetail.SteelId;
            by_orderdetailprocessdetail.SteelCode = by_orderdetail.SteelCode;
            by_orderdetailprocessdetail.SteelDescription = by_orderdetail.SteelDescription;
            by_orderdetailprocessdetail.SteelOtherDescription = by_orderdetail.SteelOtherDescription;
            by_orderdetailprocessdetail.SteelState = by_orderdetail.SteelState;
            BY_Order by_order = DataFactory.Database().FindEntity<BY_Order>(by_orderdetail.OrderId);
            by_orderdetailprocessdetail.OrderType = by_order.OrderType;
            by_orderdetailprocessdetail = new BY_OrderDetailProcessDetailBll().GetClassesDateClassesShift(by_orderdetailprocessdetail);
            DataFactory.Database().Insert<BY_OrderDetailProcessDetail>(by_orderdetailprocessdetail, isOpenTrans);
            #endregion
        }




        #region 出库列表
        public ActionResult GridPageListJson(DateTime StartTime, DateTime EndTime, string SteelCode, string BillCode, string BatchCode, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = by_outstockbll.GetPageList(StartTime, EndTime, SteelCode, BillCode, BatchCode, ref jqgridparam);
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
        #endregion
        #region 物料出库列表
        public ActionResult GridPageListJson1(string StartTime, string EndTime, string SteelCode, string BillCode, string BatchCode, string CustomerCode,string CreateUserId, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = by_outstockbll.GetPageList1(StartTime, EndTime, SteelCode, BillCode, BatchCode, CustomerCode, CreateUserId, ref jqgridparam);
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
        
        public ActionResult TJB(DateTime StartTime, DateTime EndTime, string SteelCode, string BillCode, string BatchCode, string CustomerCode)
        {
            try
            {
                JqGridParam jqgridparam = new JqGridParam() { page = 1, records = 99999, rows = 99999, sidx = "" };
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = by_outstockbll.GetPageList2(StartTime, EndTime, SteelCode, BillCode, BatchCode, CustomerCode,"", ref jqgridparam);
                var JsonData = new
                {
                    A = ListData
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }
        #endregion
    }
}