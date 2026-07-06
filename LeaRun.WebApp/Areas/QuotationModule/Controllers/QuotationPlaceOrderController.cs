using DNS.Business;
using DNS.Business.Utility.Enum;
using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using Senparc.Weixin.MP.AdvancedAPIs.CustomService;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.QuotationModule.Controllers
{
    /// <summary>
    /// 报价单落单确认
    /// </summary>
    public class QuotationPlaceOrderController : PublicController<BY_Quotation>
    {
        string ModuleId = "";
        Base_CodeRuleBll base_coderulebll = new Base_CodeRuleBll();
        /// <summary>
        /// 获取自动单据编码
        /// </summary>
        /// <param name="OrderType">加工类型：精、粗</param>
        /// <returns></returns>
        private string BillCode(string OrderType)
        {
            ModuleId = OrderType;
            string UserId = ManageProvider.Provider.Current().UserId;
            return base_coderulebll.GetBillCode(UserId, ModuleId);
        }


        #region 自动落单
        /// <summary>
        /// 自动落单
        /// </summary>
        /// <param name="KeyValue">报价单主键</param>
        /// <returns></returns>
        public ActionResult AutoPlaceOrder(string KeyValue)
        {
            try
            {
                string OrderCode = "";
                SubmitAutoPlaceOrder(KeyValue, ref OrderCode);
                return Content(new JsonMessage { Success = true, Code = "1", Message = "自动落单成功,生成订单号为:" + OrderCode }.ToString());
            }
            catch (Exception)
            {
                return Content(new JsonMessage { Success = true, Code = "-1", Message = "自动落单失败。" }.ToString());
            }
        }
        /// <summary>
        /// 提交自动落单
        /// </summary>
        /// <param name="KeyValue"></param>
        public void SubmitAutoPlaceOrder(string KeyValue, ref string _OrderCode)
        {
            //报价单主表
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(KeyValue);
            //报价单明细列表
            List<BY_QuotationDetail> QuotationDetailList = DataFactory.Database().FindList<BY_QuotationDetail>("QuotationId", KeyValue);
            //查询产能信息
            List<BY_Capacity> by_capacitylistBJ = DataFactory.Database().FindListBySql<BY_Capacity>
                    (string.Format(" select * from BY_Capacity where CapacityDate BETWEEN '{0}' AND '{1}' AND CapacityType='{2}'",
                    CommonHelper.GetDateTime(by_quotation.CreateDate).ToString("yyyy-MM-dd"), CommonHelper.GetDateTime(by_quotation.CreateDate).AddDays(30).ToString("yyyy-MM-dd"),
                    "BJ"));//取出一段时间的产能
            List<BY_Capacity> by_capacitylistBC = DataFactory.Database().FindListBySql<BY_Capacity>
                    (string.Format(" select * from BY_Capacity where CapacityDate BETWEEN '{0}' AND '{1}' AND CapacityType='{2}'",
                    CommonHelper.GetDateTime(by_quotation.CreateDate).ToString("yyyy-MM-dd"), CommonHelper.GetDateTime(by_quotation.CreateDate).AddDays(30).ToString("yyyy-MM-dd"),
                    "BC"));//取出一段时间的产能


            //订单单号
            string OrderCode = BillCode("M");
            List<BY_OrderDetail> by_orderdetails = new List<BY_OrderDetail>();
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            List<BY_Booked> by_bookedList = new List<BY_Booked>();
            try
            {
                #region 执行自动落单操作

                string[] StrShipPart = { "A", "B", "C" };
                string[] MaxStrShipDay = new string[] { DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), "0" };//默认次日A段是最大落单时段
                foreach (BY_QuotationDetail QuotationDetail in QuotationDetailList)
                {
                    if (QuotationDetail.OrderType == "毛料生产")
                    {
                        string[] StrShipDay = PlaceOrder(by_quotation, QuotationDetail, ref by_capacitylistBC, ref by_bookedList);
                        if (CommonHelper.GetDateTime(StrShipDay[0]) > CommonHelper.GetDateTime(MaxStrShipDay[0]))
                        {
                            //当落单日期大于最大落单日期时将此次落单日期设为最大落单时段
                            MaxStrShipDay[0] = StrShipDay[0];
                            MaxStrShipDay[1] = StrShipDay[1];
                        }
                        else
                        {
                            //同一天时判断时段前后
                            if (int.Parse(StrShipDay[1]) > int.Parse(MaxStrShipDay[1]))
                            {
                                MaxStrShipDay[1] = StrShipDay[1];
                            }
                        }
                        QuotationDetail.PlaceOrderStatus = 1;
                        database.Update(QuotationDetail, isOpenTrans);
                    }
                    else
                    {
                        string[] StrShipDay = PlaceOrder(by_quotation, QuotationDetail, ref by_capacitylistBJ, ref by_bookedList);
                        if (CommonHelper.GetDateTime(StrShipDay[0]) > CommonHelper.GetDateTime(MaxStrShipDay[0]))
                        {
                            //当落单日期大于最大落单日期时将此次落单日期设为最大落单时段
                            MaxStrShipDay[0] = StrShipDay[0];
                            MaxStrShipDay[1] = StrShipDay[1];
                        }
                        else
                        {
                            //同一天时判断时段前后
                            if (int.Parse(StrShipDay[1]) > int.Parse(MaxStrShipDay[1]))
                            {
                                MaxStrShipDay[1] = StrShipDay[1];
                            }
                        }
                        QuotationDetail.PlaceOrderStatus = 1;
                        database.Update(QuotationDetail, isOpenTrans);
                    }
                    
                    
                }
                string Delivery = MaxStrShipDay[0] + " " + StrShipPart[int.Parse(MaxStrShipDay[1])];
                //落单完成后更新产能表
                foreach (BY_Capacity by_capacity in by_capacitylistBJ)
                {
                    database.Update<BY_Capacity>(by_capacity, isOpenTrans);
                }
                foreach (BY_Capacity by_capacity in by_capacitylistBC)
                {
                    database.Update<BY_Capacity>(by_capacity, isOpenTrans);
                }
                //落单完成后更新落单表
                foreach (BY_Booked by_booked in by_bookedList)
                {
                    database.Insert<BY_Booked>(by_booked, isOpenTrans);
                }
                #endregion

                #region 改变报价单落单 状态
                //BY_Quotation entity = new BY_Quotation();
                //entity.QuotationId = KeyValue;
                by_quotation.BillStatus = QuotationBillStatus.已落单.ToString();
                by_quotation.PlaceOrderDate = DateTime.Now;
                by_quotation.PlaceOrderUserId = ManageProvider.Provider.Current().UserName;
                //by_quotation.Delivery = Delivery;
                by_quotation.OrderCode = OrderCode;
                database.Update(by_quotation, isOpenTrans);

                BY_OpearLog by_opearlog = new BY_OpearLog();
                by_opearlog.Create();
                by_opearlog.BillId = KeyValue;
                by_opearlog.OpearType = by_quotation.BillStatus;
                database.Insert(by_opearlog, isOpenTrans);

                #endregion

                #region 生成订单主表
                BY_Order by_order = new BY_Order();
                by_order.Create();
                by_order.OrderStatus = OrderBillStatus.已审核.ToString();
                by_order.QuotationId = by_quotation.QuotationId;
                by_order.CustomerId = by_quotation.CustomerId;
                by_order.CustomerCode = by_quotation.CustomerCode;
                by_order.CustomerName = by_quotation.CustomerName;
                by_order.ShortName = by_quotation.ShortName;
                by_order.ContactName = by_quotation.ContactName;
                by_order.ContactTel = by_quotation.ContactTel;
                by_order.ContactFax = by_quotation.ContactFax;
                by_order.ContactMail = by_quotation.ContactMail;
                by_order.MobileCode = by_quotation.MobileCode;
                by_order.CustomerAddress = by_quotation.CustomerAddress;
                //by_order.OrderType = by_quotation.OrderType;
                by_order.OrderCode = OrderCode;
                by_order.Qty = by_quotation.Qty;
                by_order.Amount = by_quotation.TotalAmount;
                by_order.CustomerPO = by_quotation.CustomerPO;
                by_order.CheckStatus = by_quotation.CheckStatus;
                by_order.CheckUserId = by_quotation.CheckUserId;
                by_order.CheckDate = by_quotation.CheckDate;
                by_order.PlaceOrderDate = DateTime.Now;
                by_order.PlaceOrderUserId = ManageProvider.Provider.Current().UserName;
                by_order.SalesMan = by_quotation.SalesMan;
                //by_order.Delivery = Delivery;
                by_order.IssuedUserId = ManageProvider.Provider.Current().UserName;
                by_order.IssuedDate = DateTime.Now;
                by_order.MnemonicCode = by_quotation.MnemonicCode;
                by_order.DeleteMark = 0;
                database.Insert(by_order, isOpenTrans);
                base_coderulebll.OccupyBillCode(ManageProvider.Provider.Current().UserId, ModuleId, isOpenTrans);

                _OrderCode = OrderCode;
                #endregion

                #region 生成订单明细表
                foreach (BY_QuotationDetail QuotationDetail in QuotationDetailList)
                {
                    //气割，吊孔不生成订单
                    if (QuotationDetail.Length == 0)
                    {
                        continue;
                    }
                    #region 订单明细表
                    BY_OrderDetail by_orderdetail = new BY_OrderDetail();
                    by_orderdetail.Create();
                    by_orderdetail.OrderId = by_order.OrderId;
                    by_orderdetail.OrderCode = by_order.OrderCode;
                    //by_orderdetail.IsChamfer = QuotationDetail.IsChamfer;
                    by_orderdetail.IsGrindAngle = QuotationDetail.IsGrindAngle;
                    by_orderdetail.LineCode = QuotationDetail.LineCode;
                    by_orderdetail.BoardId = QuotationDetail.BoardId;
                    by_orderdetail.BoardDescription = QuotationDetail.BoardDescription;
                    by_orderdetail.Qty = QuotationDetail.Qty;
                    by_orderdetail.SteelId = QuotationDetail.SteelId;
                    by_orderdetail.SteelCode = QuotationDetail.SteelCode;
                    by_orderdetail.SteelDescription = QuotationDetail.SteelDescription;
                    by_orderdetail.SteelOtherDescription = QuotationDetail.SteelOtherDescription;
                    by_orderdetail.SteelState = QuotationDetail.SteelState;
                    by_orderdetail.DieCode = QuotationDetail.DieCode;
                    by_orderdetail.Thick = QuotationDetail.Thick;
                    by_orderdetail.Weigh = QuotationDetail.Weigh;
                    by_orderdetail.Length = QuotationDetail.Length;
                    by_orderdetail.BillStatus = OrderBillStatus.录入.ToString();
                    by_orderdetail.ExecRemark = QuotationDetail.Remark;
                    by_orderdetail.Weight = QuotationDetail.Weight;
                    by_orderdetail.QuotationDetailId = QuotationDetail.QuotationDetailId;
                    by_orderdetail.OrderType = QuotationDetail.OrderType;
                    by_orderdetail.RemarkKL = QuotationDetail.RemarkKL;

                    if (by_quotation.CustMaterial == 1)
                    {
                        by_orderdetail.OpenQty = QuotationDetail.Qty;
                        by_orderdetail.WeightFact = QuotationDetail.Weight;
                    }

                    by_orderdetail.RepalceMl = QuotationDetail.RepalceMl;
                    by_orderdetail.ShipDescription = QuotationDetail.ShipDescription;
                    by_orderdetail.CustomerProduct = QuotationDetail.CustomerProduct;
                    by_orderdetail.ProductName = QuotationDetail.ProductName;
                    by_orderdetail.LiftingHole = QuotationDetail.LiftingHole;
                    by_orderdetail.BoreSize = QuotationDetail.BoreSize;
                    by_orderdetail.SteelType = QuotationDetail.SteelType;
                    by_orderdetails.Add(by_orderdetail);
                    database.Insert(by_orderdetail, isOpenTrans);
                    #endregion
                }
                #endregion
                //GentOrderDetailProcess(by_orderdetails, OrderCode,  by_quotation,isOpenTrans);
                database.Commit();
            }
            catch (Exception ex)
            {
                database.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 自动落单
        /// </summary>
        /// <param name="by_quotation">报价单表头</param>
        /// <param name="quotationdetail">报价单明细</param>
        /// <returns></returns>
        public string[] PlaceOrder(BY_Quotation by_quotation, BY_QuotationDetail quotationdetail, ref List<BY_Capacity> by_capacitylist, ref List<BY_Booked> by_bookedList)
        {
            //获得报价时间
            //DateTime CreateDate = CommonHelper.GetDateTime(by_quotation.CreateDate);
            DateTime CreateDate = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));//从次日开始排程
            int ShipPart = 0;//1-A段，2-B段，3-C段
            string[] StrShipPart = { "A", "B", "C" };
            //string ShipDay = CreateDate.AddDays(1).ToString("yyyy-MM-dd");//从次日开始排程
            //当天12点前的报价单默认排第二天A段
            if (DateTime.Now < DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 12:00:00"))
            {
                ShipPart = 0;
            }
            else if (DateTime.Now < DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 18:00:00"))
            {
                ShipPart = 1;
            }
            else
            {
                ShipPart = 2;
            }
            decimal orderQty = CommonHelper.GetDecimal(quotationdetail.Weight);
            //进入死循环，直到产能满足时才退出循环
            while (true)
            {
                while (ShipPart <= 2)
                {
                    //获得系统默认产能时段
                    decimal bookedQty = 0;
                    BY_Capacity by_capacity = by_capacitylist.Find(
                        delegate(BY_Capacity t)
                        {
                            return t.CapacityDate == DateTime.Parse(CreateDate.ToString("yyyy-MM-dd")) && t.DayPart == StrShipPart[ShipPart];
                        });
                    //判断当前时段有无剩余产能
                    if (by_capacity.CapacityValue - by_capacity.BookedQty > 0)
                    {
                        if (orderQty <= (by_capacity.CapacityValue - by_capacity.BookedQty))
                        {
                            by_capacity.BookedQty = by_capacity.BookedQty + orderQty;
                            bookedQty = orderQty;
                        }
                        else
                        {
                            bookedQty = CommonHelper.GetDecimal(by_capacity.CapacityValue) - CommonHelper.GetDecimal(by_capacity.BookedQty);
                            by_capacity.BookedQty = by_capacity.CapacityValue;
                        }
                        #region 落单表
                        BY_Booked by_booked = new BY_Booked();
                        by_booked.Create();
                        by_booked.OrderDetailId = quotationdetail.QuotationDetailId;
                        by_booked.BillCode = by_quotation.QuotationCode + "-" + quotationdetail.LineCode;
                        by_booked.BookedType = 1;
                        by_booked.CapacityDate = by_capacity.CapacityDate;
                        by_booked.DayPart = by_capacity.DayPart;
                        by_booked.Qty = bookedQty;
                        by_bookedList.Add(by_booked);
                        #endregion
                        orderQty = orderQty - bookedQty;//扣减已落单数量
                        //当未完成落单数为0时，落单完成
                        if (orderQty == 0)
                        {
                            //database.Update(by_capacity, isOpenTrans);
                            return new string[] { CommonHelper.GetDateTime(by_capacity.CapacityDate).ToString("yyyy-MM-dd"), ShipPart.ToString() };
                            //当前时段产能足够，落单成功返回交货日期
                        }
                    }
                    ShipPart++;//当前时段已排满，转入下一时段
                }
                CreateDate = CreateDate.AddDays(1);//本天内产能不够，自动跳入到下一天
                ShipPart = 0;//新的一天从A段开始排程
            }
        }
        #endregion

        #region 手动落单
        /// <summary>
        /// 手动落单设置页面
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "CacheForParam")]
        public ActionResult NonauTomatic()
        {
            string KeyValue = Request["KeyValue"];
            decimal objweight = CommonHelper.GetDecimal( DataFactory.Database().FindSum<BY_QuotationDetail>("Weight", string.Format(" AND QuotationId='{0}'", KeyValue)))-
                CommonHelper.GetDecimal(DataFactory.Database().FindSum<BY_Booked>("Qty", string.Format(" AND BillCode IN (SELECT QuotationCode FROM BY_Quotation WHERE QuotationId='{0}'  AND DeleteMark = 0)", KeyValue)));
            ViewBag.PlaceOrder = objweight;
            return View();
        }
        /// <summary>
        /// 提交手动落单
        /// </summary>
        /// <param name="KeyValue">报价单主键</param>
        /// <param name="CapacityDate">交货期</param>
        /// <param name="DayPart">产能时段</param>
        /// <param name="PlaceOrder">落单数量</param>
        /// <returns></returns>
        public ActionResult SubmitNonauTomatic(string KeyValue, string CapacityDate, string DayPart, string PlaceOrder, string OrderType)
        {
            //求报价单各明细总重量
            object objweight = DataFactory.Database().FindSum<BY_QuotationDetail>("Weight", string.Format(" AND QuotationId='{0}'", KeyValue));
            decimal weight = (decimal)objweight;
          
            //报价单主表
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(KeyValue);
            if (CommonHelper.GetDecimal(PlaceOrder) > weight - by_quotation.BookedQty)//如果手动落单数量大于未落单数
            {
                return Content(new JsonMessage { Success = true, Code = "-1", Message = "手动落单失败,指定数据不能超过未落单数。" }.ToString());
            }
            //报价单明细列表
            List<BY_QuotationDetail> QuotationDetailList = DataFactory.Database().FindList<BY_QuotationDetail>("QuotationId", KeyValue);
            by_quotation.BookedQty = by_quotation.BookedQty + decimal.Parse(PlaceOrder);
            //获得产能时段
            BY_Capacity by_capacity = DataFactory.Database().FindEntityBySql<BY_Capacity>(string.Format(" select * from BY_Capacity where CapacityDate= '{0}' AND CapacityType='{1}' AND DayPart='{2}'", CapacityDate, OrderType, DayPart));//取出一段时间的产能
            string strMessage = "手动落单成功";
            //订单单号
            string OrderCode = BillCode("M");
            IDatabase database = DataFactory.Database();
            List<BY_OrderDetail> by_orderdetails = new List<BY_OrderDetail>();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                //更新已分配产能
                by_capacity.BookedQty = by_capacity.BookedQty + decimal.Parse(PlaceOrder);
                database.Update<BY_Capacity>(by_capacity, isOpenTrans);
                #region 落单表
                BY_Booked by_booked = new BY_Booked();
                by_booked.Create();
                by_booked.OrderDetailId = "";
                by_booked.BillCode = by_quotation.QuotationCode;
                by_booked.BookedType = 1;
                by_booked.CapacityDate = DateTime.Parse(CapacityDate);
                by_booked.DayPart = DayPart;
                by_booked.Qty = decimal.Parse(PlaceOrder);
                database.Insert<BY_Booked>(by_booked, isOpenTrans);
                #endregion
                //货期
                string Delivery = "";
                //落单完成后生成订单
                if (weight <= by_quotation.BookedQty)
                {
                    Delivery = CapacityDate + " " + DayPart;

                    #region 改变报价单落单 状态
                    by_quotation.BillStatus = QuotationBillStatus.已落单.ToString();
                    by_quotation.PlaceOrderDate = DateTime.Now;
                    by_quotation.PlaceOrderUserId = ManageProvider.Provider.Current().UserName;
                    //by_quotation.Delivery = Delivery;
                    by_quotation.OrderCode = OrderCode;

                    BY_OpearLog by_opearlog = new BY_OpearLog();
                    by_opearlog.Create();
                    by_opearlog.BillId = KeyValue;
                    by_opearlog.OpearType = by_quotation.BillStatus;
                    database.Insert(by_opearlog, isOpenTrans);
                    #endregion

                    #region 生成订单主表
                    BY_Order by_order = new BY_Order();
                    by_order.Create();
                    by_order.OrderStatus = OrderBillStatus.已审核.ToString();
                    by_order.QuotationId = by_quotation.QuotationId;
                    by_order.CustomerId = by_quotation.CustomerId;
                    by_order.CustomerCode = by_quotation.CustomerCode;
                    by_order.CustomerName = by_quotation.CustomerName;
                    by_order.ShortName = by_quotation.ShortName;
                    by_order.ContactName = by_quotation.ContactName;
                    by_order.ContactTel = by_quotation.ContactTel;
                    by_order.ContactFax = by_quotation.ContactFax;
                    by_order.ContactMail = by_quotation.ContactMail;
                    by_order.MobileCode = by_quotation.MobileCode;
                    by_order.CustomerAddress = by_quotation.CustomerAddress;
                    //by_order.OrderType = by_quotation.OrderType;
                    by_order.OrderCode = OrderCode;
                    by_order.Qty = by_quotation.Qty;
                    by_order.Amount = by_quotation.TotalAmount;
                    by_order.CustomerPO = by_quotation.CustomerPO;
                    by_order.CheckStatus = by_quotation.CheckStatus;
                    by_order.CheckUserId = by_quotation.CheckUserId;
                    by_order.CheckDate = by_quotation.CheckDate;
                    by_order.PlaceOrderDate = by_quotation.PlaceOrderDate;
                    by_order.PlaceOrderUserId = by_quotation.PlaceOrderUserId;
                    by_order.SalesMan = by_quotation.SalesMan;
                    //by_order.Delivery = Delivery;
                    by_order.IssuedUserId = ManageProvider.Provider.Current().UserName;
                    by_order.IssuedDate = DateTime.Now;
                    by_order.MnemonicCode = by_quotation.MnemonicCode;
                    by_order.DeleteMark = 0;
                    database.Insert(by_order, isOpenTrans);
                    base_coderulebll.OccupyBillCode(ManageProvider.Provider.Current().UserId, ModuleId, isOpenTrans);
                    #endregion

                    #region 生成订单明细表
                    foreach (BY_QuotationDetail QuotationDetail in QuotationDetailList)
                    {
                        //气割，吊孔不生成订单
                        if (QuotationDetail.Length == 0)
                        {
                            continue;
                        }
                        #region 订单明细表
                        BY_OrderDetail by_orderdetail = new BY_OrderDetail();
                        by_orderdetail.Create();
                        //by_orderdetail.IsChamfer = QuotationDetail.IsChamfer;
                        by_orderdetail.IsGrindAngle = QuotationDetail.IsGrindAngle;
                        by_orderdetail.OrderId = by_order.OrderId;
                        by_orderdetail.OrderCode = by_order.OrderCode;
                        by_orderdetail.LineCode = QuotationDetail.LineCode;
                        by_orderdetail.BoardId = QuotationDetail.BoardId;
                        by_orderdetail.BoardDescription = QuotationDetail.BoardDescription;
                        by_orderdetail.Qty = QuotationDetail.Qty;
                        by_orderdetail.SteelId = QuotationDetail.SteelId;
                        by_orderdetail.SteelCode = QuotationDetail.SteelCode;
                        by_orderdetail.SteelDescription = QuotationDetail.SteelDescription;
                        by_orderdetail.SteelOtherDescription = QuotationDetail.SteelOtherDescription;
                        by_orderdetail.SteelState = QuotationDetail.SteelState;
                        by_orderdetail.DieCode = QuotationDetail.DieCode;
                        by_orderdetail.Thick = QuotationDetail.Thick;
                        by_orderdetail.Weigh = QuotationDetail.Weigh;
                        by_orderdetail.Length = QuotationDetail.Length;
                        by_orderdetail.BillStatus = OrderBillStatus.录入.ToString();
                        by_orderdetail.ExecRemark = QuotationDetail.Remark;
                        by_orderdetail.Weight = QuotationDetail.Weight;
                        by_orderdetail.QuotationDetailId = QuotationDetail.QuotationDetailId;
                        by_orderdetail.OrderType = QuotationDetail.OrderType;
                        if (by_quotation.CustMaterial == 1)
                        {
                            by_orderdetail.OpenQty = QuotationDetail.Qty;
                            by_orderdetail.WeightFact = QuotationDetail.Weight;
                        }
                        by_orderdetail.RemarkKL = QuotationDetail.RemarkKL;

                        by_orderdetail.RepalceMl = QuotationDetail.RepalceMl;
                        by_orderdetail.ShipDescription = QuotationDetail.ShipDescription;
                        by_orderdetail.CustomerProduct = QuotationDetail.CustomerProduct;
                        by_orderdetail.ProductName = QuotationDetail.ProductName;
                        by_orderdetail.LiftingHole = QuotationDetail.LiftingHole;
                        by_orderdetail.BoreSize = QuotationDetail.BoreSize;
                        by_orderdetail.SteelType = QuotationDetail.SteelType;
                        by_orderdetails.Add(by_orderdetail);
                        database.Insert(by_orderdetail, isOpenTrans);
                        #endregion
                    }
                    #endregion
                    strMessage = "手动落单成功,生成订单号为:" + OrderCode;
                    //GentOrderDetailProcess(by_orderdetails, OrderCode,  by_quotation,isOpenTrans);
                }
                database.Update(by_quotation, isOpenTrans);
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = strMessage }.ToString());
            }
            catch (Exception)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = true, Code = "-1", Message = "手动落单失败。" }.ToString());
            }
        }
        #endregion

        public void GentOrderDetailProcess(BY_OrderDetail entity, string OrderCode, BY_Quotation by_quotation, decimal? WorkAmount)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                if (entity.Length == 0)
                {
                    return;
                }
                List<BY_OrderDetailProcess> processsadd = new List<BY_OrderDetailProcess>();
                List<BY_OrderDetailProcess> by_orderdetailprocesss = database.FindList<BY_OrderDetailProcess>("OrderDetailId", entity.OrderDetailId);
                foreach (BY_OrderDetailProcess item in by_orderdetailprocesss)
                {
                    database.Delete(item, isOpenTrans);
                }
                List<BY_OrderDetailProcessDetail> ProcessDetails = new List<BY_OrderDetailProcessDetail>();
                foreach (var item in by_orderdetailprocesss)
                {
                    ProcessDetails.AddRange(database.FindList<BY_OrderDetailProcessDetail>("OrderDetailProcessId", item.OrderDetailProcessId));
                }

                List<Base_ProcessList> base_ProcessLists = database.FindList<Base_ProcessList>("ProcessListName", entity.Kprocess);

                foreach (var process in base_ProcessLists)
                {
                    BY_OrderDetailProcess by_orderdetailprocess6 = new BY_OrderDetailProcess();
                    by_orderdetailprocess6.OrderDetailId = entity.OrderDetailId;
                    by_orderdetailprocess6.OrderCode = OrderCode;
                    by_orderdetailprocess6.LineCode = entity.LineCode;
                    by_orderdetailprocess6.Qty = entity.Qty;
                    by_orderdetailprocess6.Length = entity.Length;
                    by_orderdetailprocess6.Weigh = entity.Weigh;
                    by_orderdetailprocess6.Thick = entity.Thick;
                    by_orderdetailprocess6.SteelId = entity.SteelId;
                    by_orderdetailprocess6.SteelCode = entity.SteelCode;
                    by_orderdetailprocess6.SteelDescription = entity.SteelDescription;
                    by_orderdetailprocess6.SteelOtherDescription = entity.SteelOtherDescription;
                    by_orderdetailprocess6.SteelState = entity.SteelState;
                    by_orderdetailprocess6.OrderType = entity.OrderType;
                    by_orderdetailprocess6.MnemonicCode = by_quotation.MnemonicCode;
                    by_orderdetailprocess6.TechnologyId = process.ProcessValue;
                    by_orderdetailprocess6.TechnologyName = process.ProcessName;
                    by_orderdetailprocess6.SortCode = 3;
                    by_orderdetailprocess6.Create();
                    processsadd.Add(by_orderdetailprocess6);
                }

                database.Insert<BY_OrderDetailProcess>(processsadd, isOpenTrans);

                foreach (BY_OrderDetailProcess itemadd in processsadd)
                {
                    foreach (BY_OrderDetailProcess itemde in by_orderdetailprocesss)
                    {
                        if (itemadd.TechnologyId == itemde.TechnologyId)
                        {
                            List<BY_OrderDetailProcessDetail> ProcessDetails1 = ProcessDetails.Where(p => p.OrderDetailProcessId == itemde.OrderDetailProcessId).ToList();
                            foreach (BY_OrderDetailProcessDetail itemde2 in ProcessDetails1)
                            {
                                itemde2.OrderDetailProcessId = itemadd.OrderDetailProcessId;
                                itemde2.Modify(itemde2.OrderDetailProcessDetailId);
                            }
                            database.Update<BY_OrderDetailProcessDetail>(ProcessDetails1, isOpenTrans);
                        }
                    }
                }

                database.Commit();
            }
            catch (Exception)
            {
                database.Rollback();

            }

        }
        public void GentQuotationDetailProcess(BY_Quotation by_quotation, BY_QuotationDetail entity, List<BY_OrderDetailProcess> by_orderdetailprocesss, string OrderDetailId, DbTransaction isOpenTrans)
        {
            if (entity.Length == 0)
            {
                return;
            }
            List<Base_ProcessList> base_ProcessLists = DataFactory.Database().FindList<Base_ProcessList>("ProcessListName", entity.Kprocess);

            List<BY_OrderDetailProcess> processsadd = new List<BY_OrderDetailProcess>();
            List<BY_OrderDetailProcessDetail> BY_OrderDetailProcessDetail = new List<BY_OrderDetailProcessDetail>();
            if (by_orderdetailprocesss.Count > 0 && !string.IsNullOrWhiteSpace(by_orderdetailprocesss[0].OrderDetailId))
            {
                BY_OrderDetailProcessDetail = DataFactory.Database().FindList<BY_OrderDetailProcessDetail>("OrderDetailId", by_orderdetailprocesss[0].OrderDetailId);
            }

            foreach (BY_OrderDetailProcess item in by_orderdetailprocesss)
            {
                if (base_ProcessLists.Where(p => p.ProcessName == item.TechnologyName).Count() == 0)
                {
                    DataFactory.Database().Delete(item, isOpenTrans);
                    DataFactory.Database().Delete<BY_OrderDetailProcessDetail>("OrderDetailProcessId", item.OrderDetailProcessId, isOpenTrans);
                }
            }

            foreach (var process in base_ProcessLists)
            {
                if (by_orderdetailprocesss.Where(p => p.TechnologyName == process.ProcessName).Count() == 0)
                {
                    BY_OrderDetailProcess by_orderdetailprocess6 = new BY_OrderDetailProcess();
                    by_orderdetailprocess6.QuotationDetailId = entity.QuotationDetailId;
                    by_orderdetailprocess6.OrderCode = by_quotation.QuotationCode.Substring(2, 4) + by_quotation.QuotationCode.Substring(8);
                    by_orderdetailprocess6.LineCode = entity.LineCode;
                    by_orderdetailprocess6.Qty = entity.Qty;
                    by_orderdetailprocess6.Length = entity.Length;
                    by_orderdetailprocess6.Weigh = entity.Weigh;
                    by_orderdetailprocess6.Thick = entity.Thick;
                    by_orderdetailprocess6.SteelId = entity.SteelId;
                    by_orderdetailprocess6.SteelCode = entity.SteelCode;
                    by_orderdetailprocess6.SteelDescription = entity.SteelDescription;
                    by_orderdetailprocess6.SteelOtherDescription = entity.SteelOtherDescription;
                    by_orderdetailprocess6.SteelState = entity.SteelState;
                    by_orderdetailprocess6.OrderType = entity.OrderType;
                    by_orderdetailprocess6.MnemonicCode = by_quotation.MnemonicCode;
                    by_orderdetailprocess6.TechnologyId = process.ProcessValue;
                    by_orderdetailprocess6.TechnologyName = process.ProcessName;
                    by_orderdetailprocess6.SortCode = 3;
                    by_orderdetailprocess6.OrderDetailId = OrderDetailId;
                    by_orderdetailprocess6.Create();
                    processsadd.Add(by_orderdetailprocess6);
                }
            }

            //Base_Supplier Base_Supplier = DataFactory.Database().FindEntity<Base_Supplier>("Code", "ZDJX");
            //decimal WorkerPer = 0;
            //decimal? Area = new QuotationInputController().ReturnArea(entity.Thick, entity.Weigh, entity.Length,entity.OrderType) ;
            //decimal? WorkerTotal = new QuotationInputController().ReturnWorkCost(CommonHelper.GetDecimal(Area), CommonHelper.GetDecimal(entity.Weigh), entity.SteelType, entity.OrderType, new Base_Supplier(), 0, 0, ref WorkerPer, ref WorkerPer) * entity.Qty;

            //foreach (BY_OrderDetailProcess itemadd in processsadd)
            //{
            //    foreach (BY_OrderDetailProcess itemde in by_orderdetailprocesss)
            //    {
            //        if (itemadd.TechnologyId == itemde.TechnologyId)
            //        {
            //            itemadd.DeleteMark = itemde.DeleteMark;
            //        }
            //    }
            //    DataFactory.Database().Insert<BY_OrderDetailProcess>(itemadd);
            //}
            DataFactory.Database().Insert<BY_OrderDetailProcess>(processsadd, isOpenTrans);
        }

        /// <summary>
        /// 生成工序
        /// </summary>
        /// <param name="by_orderdetails"></param>
        /// <param name="OrderCode"></param>
        /// <param name="isOpenTrans"></param>
        //public void GentOrderDetailProcess(List<BY_OrderDetail> by_orderdetails, string OrderCode, BY_Quotation by_quotation, DbTransaction isOpenTrans)
        //{
        //    //工序生成
        //    foreach (BY_OrderDetail entity in by_orderdetails)
        //    {

        //       // GentOrderDetailProcess(entity,  OrderCode,  by_quotation,  isOpenTrans);
        //    }
        //}

    }
}
