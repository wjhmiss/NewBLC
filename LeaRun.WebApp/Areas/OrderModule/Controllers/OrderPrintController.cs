using DNS.Business;
using DNS.Business.Utility.Enum;
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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.OrderModule.Controllers
{
    /// <summary>
    /// 订单打印
    /// </summary>
    public class OrderPrintController : Controller
    {

        public ActionResult PrintForSigle(string OrderId, string OrderDetailIds, string QuotationId)
        {
            BY_Order by_order = new BY_Order();
            List<BY_OrderDetail> orderdetail_List = new List<BY_OrderDetail>();
            if (string.IsNullOrEmpty(OrderDetailIds))
            {
                orderdetail_List = (from items in DataFactory.Database().FindList<BY_OrderDetail>("OrderId", OrderId) where items.Length > 0 select items).ToList<BY_OrderDetail>();
                by_order = DataFactory.Database().FindEntity<BY_Order>(OrderId);

            }
            else
            {
                string[] OrderDetailids = OrderDetailIds.Trim(',').Split(',');
                for (int i = 0; i < OrderDetailids.Length; i++)
                {
                    BY_OrderDetail by_orderdetail = DataFactory.Database().FindEntity<BY_OrderDetail>(OrderDetailids[i]);
                    if (by_order == null || by_order.OrderId == null)
                    {
                        by_order = DataFactory.Database().FindEntity<BY_Order>(by_orderdetail.OrderId);
                    }
                    orderdetail_List.Add(by_orderdetail);
                }
            }
            if (!string.IsNullOrEmpty(QuotationId))
            {
                by_order = DataFactory.Database().FindEntity<BY_Order>("QuotationId", QuotationId);
                if (by_order == null || by_order.OrderId == null)
                {
                    return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：请先提交报价单。" }.ToString());
                }
                orderdetail_List = (from items in DataFactory.Database().FindList<BY_OrderDetail>("OrderId", by_order.OrderId) where items.Length > 0 select items).ToList<BY_OrderDetail>();
            }

            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(by_order.QuotationId);
            Base_Customer Base_Customer = DataFactory.Database().FindEntity<Base_Customer>(by_quotation.CustomerId);
            Base_Company company = DataFactory.Database().FindEntity<Base_Company>("FullName", string.IsNullOrEmpty(by_quotation.CompanyName) ? "" : by_quotation.CompanyName);

            foreach (BY_OrderDetail item in orderdetail_List)
            {
                IEnumerable<BY_OrderDetailProcess> by_orderdetailprocesss = from items in DataFactory.Database().FindList<BY_OrderDetailProcess>("OrderDetailId", item.OrderDetailId) orderby items.SortCode select items;
                string strProcess = string.Empty;
                foreach (BY_OrderDetailProcess entity in by_orderdetailprocesss)
                {
                    strProcess = strProcess + entity.TechnologyName + " ("+ entity.Remark + ")(&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;) " + " ——> ";
                }
                strProcess = strProcess + "完结";
                item.ScaleProcess = strProcess;
            }

            var JsonData = new
            {
                OrderEntiy = by_order,
                OrderDetail = orderdetail_List.OrderBy(p => p.LineCode),
                OrderDetailQty = orderdetail_List.Sum(p => p.Qty),
                QuotationEntiy = by_quotation,
                Base_Customer = Base_Customer,
                company = company
            };
            return Content(JsonData.ToJson());
        }



        public class TEMPorderdetails
        {
            public BY_OrderDetail by_orderdetail { get; set; }
            public List<BY_OrderDetailProcess> by_orderdetailprocesss { get; set; }
        }
        public ActionResult YCPrintListForOne(string OrderId, string OrderDetails, string QuotationId, int RowNum)
        {

            BY_Order by_order = new BY_Order();
            List<BY_OrderDetail> orderdetail_List = new List<BY_OrderDetail>();
            if (string.IsNullOrEmpty(OrderDetails))
            {
                orderdetail_List = (from items in DataFactory.Database().FindList<BY_OrderDetail>("OrderId", OrderId) where items.Length > 0 select items).ToList<BY_OrderDetail>();
                by_order = DataFactory.Database().FindEntity<BY_Order>(OrderId);

            }
            else
            {
                string[] OrderDetailids = OrderDetails.Trim(',').Split(',');
                for (int i = 0; i < OrderDetailids.Length; i++)
                {
                    BY_OrderDetail by_orderdetail = DataFactory.Database().FindEntity<BY_OrderDetail>(OrderDetailids[i]);
                    if (by_order == null || by_order.OrderId == null)
                    {
                        by_order = DataFactory.Database().FindEntity<BY_Order>(by_orderdetail.OrderId);
                    }
                    orderdetail_List.Add(by_orderdetail);
                }
            }
            if (!string.IsNullOrEmpty(QuotationId))
            {
                by_order = DataFactory.Database().FindEntity<BY_Order>("QuotationId", QuotationId);
                if (by_order == null || by_order.OrderId == null)
                {
                    return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：请先提交报价单。" }.ToString());
                }
                orderdetail_List = (from items in DataFactory.Database().FindList<BY_OrderDetail>("OrderId", by_order.OrderId) where items.Length > 0 select items).ToList<BY_OrderDetail>();
            }
            Base_Customer Base_Customer = DataFactory.Database().FindEntity<Base_Customer>(by_order.CustomerId);
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(by_order.QuotationId);
            if (by_quotation.CustMaterial == 1)
            {
                return Content(null);
            }


            orderdetail_List = (from items in orderdetail_List orderby items.SteelCode, items.LineCode ascending select items).ToList<BY_OrderDetail>();
            List<BY_OrderDetail> by_orderdetails = new List<BY_OrderDetail>();
            List<BY_OrderDetail> by_orderdetailsa = new List<BY_OrderDetail>();
            by_orderdetailsa = orderdetail_List;

            BY_OrderDetail obj = new BY_OrderDetail();
            Type t = typeof(BY_OrderDetail);
            PropertyInfo[] myPropertyInfo;
            // Get the properties of 'Type' class object.             
            myPropertyInfo = t.GetProperties();
            for (int i = 0; i < myPropertyInfo.Length; i++)
            {
                switch (myPropertyInfo[i].PropertyType.ToString())
                {
                    case "System.Nullable`1[System.Int32]":
                        myPropertyInfo[i].SetValue(obj, 0, null);
                        break;
                    case "System.Nullable`1[System.Decimal]":
                        myPropertyInfo[i].SetValue(obj, Convert.ToDecimal(0), null);
                        break;
                    case "System.Nullable`1[System.DateTime]":
                        //myPropertyInfo[i].SetValue(obj, "", null);       
                        break;
                    case "System.DateTime":
                        //myPropertyInfo[i].SetValue(obj, "", null);       
                        break;

                }
            }

            int num3 = 1;
            string steelcode = "";
            int pageNum = 1;
            //精
            foreach (BY_OrderDetail item in by_orderdetailsa)
            {
                if (num3 == 1)
                {
                    steelcode = item.SteelCode;
                }
                if (item.SteelCode != steelcode)
                {
                    pageNum += 1;
                    int j = (RowNum - by_orderdetails.Count % RowNum);
                    for (int i = 0; i < j; i++)
                    {
                        obj.SortCode = num3;
                        by_orderdetails.Add(obj);
                    }
                }
                item.SortCode = num3;
                by_orderdetails.Add(item);
                steelcode = item.SteelCode;
                if (num3 == by_orderdetailsa.Count() && by_orderdetails.Count() % RowNum != 0)
                {
                    int j = (RowNum - by_orderdetails.Count % RowNum);
                    for (int i = 0; i < j; i++)
                    {
                        obj.SortCode = num3;
                        by_orderdetails.Add(obj);
                    }
                }
                num3 += 1;
            }

            if (by_orderdetails.Count() % RowNum != 0)
            {
                int j = (RowNum - by_orderdetails.Count % RowNum);
                for (int i = 0; i < j; i++)
                {
                    obj.SortCode = num3;
                    by_orderdetails.Add(obj);
                }
            }


            List<BY_OrderDetailPrint> by_orderdetailprints = new List<BY_OrderDetailPrint>();
            foreach (BY_OrderDetail item in by_orderdetails)
            {
                if (!string.IsNullOrEmpty(item.SteelCode))
                {
                    BY_OrderDetailPrint by_orderdetailprint = new Entity.BY_OrderDetailPrint();
                    CommonHelper.CopySameFieldsObjectALL<BY_OrderDetail, BY_OrderDetailPrint>(item, ref by_orderdetailprint);
                    Base_Steel base_steel = DataFactory.Database().FindEntity<Base_Steel>(by_orderdetailprint.SteelId);
                    by_orderdetailprint.base_steel = base_steel;
                    by_orderdetailprints.Add(by_orderdetailprint);
                }
                else
                {
                    BY_OrderDetailPrint by_orderdetailprint = new Entity.BY_OrderDetailPrint();
                    CommonHelper.CopySameFieldsObjectALL<BY_OrderDetail, BY_OrderDetailPrint>(item, ref by_orderdetailprint);
                    by_orderdetailprint.base_steel = new Base_Steel();
                    by_orderdetailprints.Add(by_orderdetailprint);
                }
            }


            var JsonData = new
            {
                OrderEntiy = by_order,
                OrderDetail = by_orderdetailprints,
                OrderDetailTotal = orderdetail_List.Sum(p => p.Qty),
                QuotationEntiy = by_quotation,
                Base_Customer = Base_Customer
            };
            return Content(JsonData.ToJson());
        }

        public ActionResult GetChooseType(string customercode,string LType)
        {
            IList<ChooseType> choosetypes = new List<ChooseType>();
            //ChooseType choosetype1 = new ChooseType()
            //{
            //    GUID = Guid.NewGuid().ToString(),
            //    Key = "BCPrintPreviewkl",
            //    Value = "开料单",
            //    Remark = "kl",
            //    SortOrder = 0
            //};
            ChooseType choosetype2 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "BCPrintPreviewxm",
                Value = "生产单",
                Remark = "xm",
                SortOrder = 10
            };
            ChooseType choosetype3 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "BCPrintPreviewLogo",
                Value = "打标单",
                Remark = "",
                SortOrder = 20
            };
            ChooseType choosetype4 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "BJPrintList",
                Value = "汇总表",
                Remark = "",
                SortOrder = 30
            };
            ChooseType choosetype5 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "YCPrint1",
                Value = "毅昌检测单",
                Remark = "",
                SortOrder = 40
            };

            ChooseType choosetype6 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "YCPrint2",
                Value = "毅昌材质单",
                Remark = "",
                SortOrder = 50
            };
            ChooseType choosetype7 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "QDPrint",
                Value = "群达检验",
                Remark = "",
                SortOrder = 60
            };
            ChooseType choosetype8 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "BYDPrint",
                Value = "普通检验",
                Remark = "",
                SortOrder = 70
            };
            ChooseType choosetype9 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "BJPrintPreview",
                Value = "材质证明",
                Remark = "",
                SortOrder = 80
            };
            ChooseType choosetype10 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "BJPrintJY",
                Value = "BYDiqc",
                Remark = "",
                SortOrder = 90
            };
            ChooseType choosetype11 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "BJPrintPreviewJY",
                Value = "A122材质证明",
                Remark = "",
                SortOrder = 100
            };
            ChooseType QPrint = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "QPrint",
                Value = "普通合同",
                Remark = "",
                SortOrder = 100
            };
            ChooseType QPrint1 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "QPrint1",
                Value = "迈格弗合同",
                Remark = "",
                SortOrder = 110
            };
            ChooseType QPrint2 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "QPrint2",
                Value = "英雷合同",
                Remark = "",
                SortOrder = 120
            };
            ChooseType QPrint3 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "QPrint3",
                Value = "振鑫合同",
                Remark = "",
                SortOrder = 120
            };

            ChooseType choosetype21 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "OrderPrintSigle2",
                Value = "加工单(分页)",
                Remark = "",
                SortOrder = 10
            };
            ChooseType BCPrintPreviewLJ = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "BCPrintPreviewLJ",
                Value = "工艺单",
                Remark = "",
                SortOrder = 10
            };
            
            //choosetypes.Add(choosetype1);
            choosetypes.Add(choosetype2);
            //choosetypes.Add(choosetype21);
            if (LType == "LJ")
            {
                choosetypes.Add(BCPrintPreviewLJ);
            }
            //choosetypes.Add(choosetype3);
            //choosetypes.Add(QPrint);
            //choosetypes.Add(QPrint1);
            //choosetypes.Add(QPrint2);
            //choosetypes.Add(QPrint3);
            //choosetypes.Add(choosetype4);
            //choosetypes.Add(choosetype5);
            //choosetypes.Add(choosetype6);
            //choosetypes.Add(choosetype7);
            //choosetypes.Add(choosetype8);
            //choosetypes.Add(choosetype9);
            //choosetypes.Add(choosetype10);
            //choosetypes.Add(choosetype11);
            Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>("Code", customercode);

            var JsonData = new
            {
                Data = choosetypes,
                Customer = base_customer
            };
            return Json(JsonData, JsonRequestBehavior.AllowGet);
        }
        public class ChooseType : BaseEntity
        {
            #region 获取/设置 字段值
            /// <summary>
            /// GUID
            /// </summary>
            /// <returns></returns>
            public string GUID { get; set; }
            /// <summary>
            /// JsonColumn
            /// </summary>
            /// <returns></returns>
            public string Key { get; set; }
            /// <summary>
            /// JsonData
            /// </summary>
            /// <returns></returns>
            public string Value { get; set; }
            /// <summary>
            /// ExportField
            /// </summary>
            /// <returns></returns>
            public string Remark { get; set; }
            /// <summary>
            /// ExportField
            /// </summary>
            /// <returns></returns>
            public int SortOrder { get; set; }
            #endregion

            #region 扩展操作
            /// <summary>
            /// 新增调用
            /// </summary>
            public override void Create()
            {
                this.GUID = CommonHelper.GetGuid;
            }
            /// <summary>
            /// 编辑调用
            /// </summary>
            /// <param name="KeyValue"></param>
            public override void Modify(string KeyValue)
            {
                this.GUID = KeyValue;
            }
            #endregion
        }


        BY_OrderBll by_orderbll = new BY_OrderBll();
        /// <summary>
        /// 订单列表（返回表格JSON）
        /// </summary>
        /// <param name="Condition">查询条件</param>
        /// <param name="jqgridparam">分页参数</param>
        /// <returns></returns>
        public ActionResult GridPageListJson(string Condition, JqGridParam jqgridparam)
        {
            try
            {
                Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
                Stopwatch watch = CommonHelper.TimerStart();
                List<BY_Order> ListData = by_orderbll.GetOrderPrintList(QueryParameter, ref jqgridparam);
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
        public ActionResult BCPrintPreviewLJ()
        {
            return View();
        }

        public ActionResult BCPrintDataJsonLJ(string OrderId, string KeyValueQ)
        {
            if (string.IsNullOrEmpty(OrderId) && string.IsNullOrEmpty(KeyValueQ))
            {
                return Content(null);
            }
            BY_Order by_order = new BY_Order();
            List<BY_OrderDetail> orderdetail_List = new List<BY_OrderDetail>();
            if (string.IsNullOrEmpty(KeyValueQ))
            {
                orderdetail_List = (from items in DataFactory.Database().FindList<BY_OrderDetail>("OrderId", OrderId) where items.SteelCode != "FY" orderby  items.LineCode  select items).ToList<BY_OrderDetail>();
                by_order = DataFactory.Database().FindEntity<BY_Order>(OrderId);
            }
            else
            {
                BY_Quotation by_quotation1 = new BY_Quotation();
                by_quotation1 = DataFactory.Database().FindEntity<BY_Quotation>(KeyValueQ);
                if (string.IsNullOrEmpty(by_quotation1.OrderCode))
                {
                    return Content(null);
                }
                by_order = DataFactory.Database().FindEntity<BY_Order>("OrderCode", by_quotation1.OrderCode);
                orderdetail_List = (from items in DataFactory.Database().FindList<BY_OrderDetail>("OrderCode", by_quotation1.OrderCode) where items.SteelCode != "FY" orderby  items.LineCode  select items).ToList<BY_OrderDetail>();
            }
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>("MnemonicCode", by_order.MnemonicCode);
            Base_Customer Base_Customer = DataFactory.Database().FindEntity<Base_Customer>(by_order.CustomerId);
            List<BY_OrderDetailProcess> BY_OrderDetailProcesss = new List<BY_OrderDetailProcess>();
            List<TEMPorderdetails> TEMPorderdetailss = new List<TEMPorderdetails>();

            Base_Company company = DataFactory.Database().FindEntity<Base_Company>("FullName", string.IsNullOrEmpty(by_quotation.CompanyName) ? "" : by_quotation.CompanyName);

            foreach (var item in orderdetail_List)
            {
                TEMPorderdetails temporderdetails = new TEMPorderdetails();
                temporderdetails.by_orderdetail = item;
                //BY_OrderDetailProcesss = DataFactory.Database().FindList<BY_OrderDetailProcess>("QuotationDetailId", item.QuotationDetailId);
              
                BY_OrderDetailProcesss = DataFactory.Database().FindList<BY_OrderDetailProcess>(string.Format("AND QuotationDetailId = {0}  and (TechnologyName not like '%账%' and TechnologyName not like '%帐%'  and TechnologyName not like '%优惠%'  and TechnologyName not like '%费%')", item.QuotationDetailId));
                temporderdetails.by_orderdetailprocesss = BY_OrderDetailProcesss;
                TEMPorderdetailss.Add(temporderdetails);
            }

            var JsonData = new
            {
                OrderEntiy = by_order,
                TEMPorderdetailss = TEMPorderdetailss,
                OrderDetailTotal = orderdetail_List.Sum(p => p.Qty),
                QuotationEntiy = by_quotation,
                Base_Customer = Base_Customer,
                company = company
            };
            return Content(JsonData.ToJson());
        }


        #region 粗料单打印

        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult QPrint()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult QPrint1()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult QPrint2()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult QPrint3()
        {
            return View();
        }

        public ActionResult OrderPrintSigle2()
        {
            return View();
        }
        
        /// <summary>
        /// 粗料单打印
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult BCPrint()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult BJPrintJY()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult ChoosePrint()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult BJPrintForLab()
        {
            return View();
        }


        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult BYDPrint()
        {
            return View();
        }

        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult QDPrint()
        {
            return View();
        }

        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult YCPrint1()
        {
            return View();
        }

        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult YCPrint2()
        {
            return View();
        }
        /// <summary>
        /// 粗料单打印预览
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult BCPrintPreview()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult BCPrintPreviewkl()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult BCPrintPreviewxm()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult BCPrintPreviewLogo()
        {
            return View();
        }

        public ActionResult BCPrintListJsonJY(string OrderId, string OrderDetailIds, string QuotationId, string LType, int RowNum = 23)
        {
            BY_Order by_order = new BY_Order();
            IEnumerable<BY_OrderDetail> orderdetail_List = new List<BY_OrderDetail>();

            List<BY_OrderDetail> orderdetail_List1 = new List<BY_OrderDetail>();
            List<BY_OrderDetail> orderdetail_List2 = new List<BY_OrderDetail>();
            List<BY_OrderDetail> orderdetail_List3 = new List<BY_OrderDetail>();


            if (string.IsNullOrEmpty(OrderDetailIds))
            {
                orderdetail_List = DataFactory.Database().FindList<BY_OrderDetail>("OrderId", OrderId);
                by_order = DataFactory.Database().FindEntity<BY_Order>(OrderId);
            }
            else
            {
                List<BY_OrderDetail> tempList = new List<BY_OrderDetail>();
                string[] OrderDetailids = OrderDetailIds.Trim(',').Split(',');
                for (int i = 0; i < OrderDetailids.Length; i++)
                {
                    BY_OrderDetail by_orderdetail = DataFactory.Database().FindEntity<BY_OrderDetail>(OrderDetailids[i]);
                    if (by_order == null || by_order.OrderId == null)
                    {
                        by_order = DataFactory.Database().FindEntity<BY_Order>(by_orderdetail.OrderId);
                    }
                    tempList.Add(by_orderdetail);
                }
                orderdetail_List = tempList;
            }
            if (!string.IsNullOrEmpty(QuotationId))
            {
                by_order = DataFactory.Database().FindEntity<BY_Order>("QuotationId", QuotationId);
                if (by_order == null || by_order.OrderId == null)
                {
                    return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：请先提交报价单。" }.ToString());
                }
                orderdetail_List = (from items in DataFactory.Database().FindList<BY_OrderDetail>("OrderId", by_order.OrderId) where items.Length > 0 select items).ToList<BY_OrderDetail>();
            }



            if (LType == "LJ")
            {
                orderdetail_List = (from items in orderdetail_List orderby items.LineCode ascending select items).ToList<BY_OrderDetail>();

            }
            else
            {
                orderdetail_List = (from items in orderdetail_List orderby items.SteelCode, items.LineCode ascending select items).ToList<BY_OrderDetail>();
                orderdetail_List1 = orderdetail_List.Where(p => p.OrderType == "毛料生产").ToList();
                orderdetail_List2 = orderdetail_List.Where(p => p.OrderType == "光板加工").ToList();
                orderdetail_List3 = orderdetail_List.Where(p => p.OrderType == "精板加工").ToList();

            }


            List<BY_OrderDetail> by_orderdetails = new List<BY_OrderDetail>();
            BY_OrderDetail obj = new BY_OrderDetail();
            Type t = typeof(BY_OrderDetail);
            PropertyInfo[] myPropertyInfo;
            myPropertyInfo = t.GetProperties();
            for (int i = 0; i < myPropertyInfo.Length; i++)
            {
                switch (myPropertyInfo[i].PropertyType.ToString())
                {
                    case "System.Nullable`1[System.Int32]":
                        myPropertyInfo[i].SetValue(obj, 0, null);
                        break;
                    case "System.Nullable`1[System.Decimal]":
                        myPropertyInfo[i].SetValue(obj, Convert.ToDecimal(0), null);
                        break;
                    case "System.Nullable`1[System.DateTime]":
                        //myPropertyInfo[i].SetValue(obj, "", null);       
                        break;
                    case "System.DateTime":
                        //myPropertyInfo[i].SetValue(obj, "", null);       
                        break;

                }
            }

            if (LType == "LJ")
            {
                foreach (BY_OrderDetail item in orderdetail_List)
                {
                    //    if (num3 == 1)
                    //    {
                    //        steelcode = item.SteelCode;
                    //    }
                    //    if (item.SteelCode != steelcode)
                    //    {
                    //        int j = (RowNum - by_orderdetails.Count % RowNum);
                    //        for (int i = 0; i < j; i++)
                    //        {
                    //            obj.SortCode = num3;
                    //by_orderdetails.Add(obj);
                    //        }
                    //    }
                    //    item.SortCode = num3;
                    by_orderdetails.Add(item);
                    //    steelcode = item.SteelCode;
                    //    if (num3 == orderdetail_List.Count() && by_orderdetails.Count() % RowNum != 0)
                    //    {
                    //        int j = (RowNum - by_orderdetails.Count % RowNum);
                    //        for (int i = 0; i < j; i++)
                    //        {
                    //            obj.SortCode = num3;
                    //            by_orderdetails.Add(obj);
                    //        }
                    //    }
                    //    num3 += 1;
                }
            }
            else
            {
                int num3 = 1;
                string steelcode = "";
                foreach (BY_OrderDetail item in orderdetail_List1)
                {
                    if (num3 == 1)
                    {
                        steelcode = item.SteelCode;
                    }
                    if (item.SteelCode != steelcode)
                    {
                        int j = (RowNum - by_orderdetails.Count % RowNum);
                        for (int i = 0; i < j; i++)
                        {
                            obj.SortCode = num3;
                            by_orderdetails.Add(obj);
                        }
                    }
                    item.SortCode = num3;
                    by_orderdetails.Add(item);
                    steelcode = item.SteelCode;
                    if (num3 == orderdetail_List1.Count() && by_orderdetails.Count() % RowNum != 0)
                    {
                        int j = (RowNum - by_orderdetails.Count % RowNum);
                        for (int i = 0; i < j; i++)
                        {
                            obj.SortCode = num3;
                            by_orderdetails.Add(obj);
                        }
                    }
                    num3 += 1;
                }

                num3 = 1;
                foreach (BY_OrderDetail item in orderdetail_List2)
                {
                    if (num3 == 1)
                    {
                        steelcode = item.SteelCode;
                    }
                    if (item.SteelCode != steelcode)
                    {
                        int j = (RowNum - by_orderdetails.Count % RowNum);
                        for (int i = 0; i < j; i++)
                        {
                            obj.SortCode = num3;
                            by_orderdetails.Add(obj);
                        }
                    }
                    item.SortCode = num3;
                    by_orderdetails.Add(item);
                    steelcode = item.SteelCode;
                    if (num3 == orderdetail_List2.Count() && by_orderdetails.Count() % RowNum != 0)
                    {
                        int j = (RowNum - by_orderdetails.Count % RowNum);
                        for (int i = 0; i < j; i++)
                        {
                            obj.SortCode = num3;
                            by_orderdetails.Add(obj);
                        }
                    }
                    num3 += 1;
                }

                num3 = 1;
                foreach (BY_OrderDetail item in orderdetail_List3)
                {
                    if (num3 == 1)
                    {
                        steelcode = item.SteelCode;
                    }
                    if (item.SteelCode != steelcode)
                    {
                        int j = (RowNum - by_orderdetails.Count % RowNum);
                        for (int i = 0; i < j; i++)
                        {
                            obj.SortCode = num3;
                            by_orderdetails.Add(obj);
                        }
                    }
                    item.SortCode = num3;
                    by_orderdetails.Add(item);
                    steelcode = item.SteelCode;
                    if (num3 == orderdetail_List3.Count() && by_orderdetails.Count() % RowNum != 0)
                    {
                        int j = (RowNum - by_orderdetails.Count % RowNum);
                        for (int i = 0; i < j; i++)
                        {
                            obj.SortCode = num3;
                            by_orderdetails.Add(obj);
                        }
                    }
                    num3 += 1;
                }



            }




            if (by_orderdetails.Count() % RowNum != 0)
            {
                int j = (RowNum - by_orderdetails.Count % RowNum);
                for (int i = 0; i < j; i++)
                {
                    by_orderdetails.Add(obj);
                }
            }



            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(by_order.QuotationId);
            Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>("Code", by_quotation.CustomerCode);
            Base_Company company = DataFactory.Database().FindEntity<Base_Company>("FullName", string.IsNullOrEmpty(by_quotation.CompanyName) ? "" : by_quotation.CompanyName);
          
            var JsonData = new
            {
                OrderEntiy = by_order,
                OrderDetail = by_orderdetails,
                OrderDetailTotal = orderdetail_List.Sum(p => p.Qty),
                OrderDetailWeight = orderdetail_List.Sum(p => p.Weight),
                Customer = base_customer,
                QuotationEntiy = by_quotation,
                company = company,
            };
            return Content(JsonData.ToJson());
        }

        public ActionResult BCPrintDataJsonForFlawe(string OrderDetailId)
        {

            BY_Order by_order = new BY_Order();
            BY_OrderDetail orderdetail_List = new BY_OrderDetail();
            if (!string.IsNullOrEmpty(OrderDetailId))
            {
                orderdetail_List = DataFactory.Database().FindEntity<BY_OrderDetail>(OrderDetailId);
                by_order = DataFactory.Database().FindEntity<BY_Order>(orderdetail_List.OrderId);
            }
            
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(by_order.QuotationId);
            Base_Customer Base_Customer = DataFactory.Database().FindEntity<Base_Customer>(by_order.CustomerId);

            List<BY_OrderDetail> by_orderdetails = new List<BY_OrderDetail>();
            List<BY_OrderDetail> by_orderdetailsa = new List<BY_OrderDetail>();
            by_orderdetailsa.Add(orderdetail_List);

            BY_OrderDetail obj = new BY_OrderDetail();
            Type t = typeof(BY_OrderDetail);
            PropertyInfo[] myPropertyInfo;
            myPropertyInfo = t.GetProperties();
            for (int i = 0; i < myPropertyInfo.Length; i++)
            {
                switch (myPropertyInfo[i].PropertyType.ToString())
                {
                    case "System.Nullable`1[System.Int32]":
                        myPropertyInfo[i].SetValue(obj, 0, null);
                        break;
                    case "System.Nullable`1[System.Decimal]":
                        myPropertyInfo[i].SetValue(obj, Convert.ToDecimal(0), null);
                        break;
                    case "System.Nullable`1[System.DateTime]":
                        //myPropertyInfo[i].SetValue(obj, "", null);       
                        break;
                    case "System.DateTime":
                        //myPropertyInfo[i].SetValue(obj, "", null);       
                        break;
                    default:
                        myPropertyInfo[i].SetValue(obj, "", null);
                        break;
                }
            }

            int num3 = 1;
            string steelcode = "";
            int pageNum = 1;

            //精
            foreach (BY_OrderDetail item in by_orderdetailsa)
            {
                item.RepalceMl = "A" + item.SteelId.Substring(item.SteelId.Length - 3, 3);

                if (num3 == 1)
                {
                    steelcode = item.SteelCode;
                }
                if (item.SteelCode != steelcode)
                {
                    pageNum += 1;
                    int j = (9 - by_orderdetails.Count % 9);
                    for (int i = 0; i < j; i++)
                    {
                        obj.SortCode = num3;
                        by_orderdetails.Add(obj);
                    }
                }
                item.SortCode = num3;
                by_orderdetails.Add(item);
                steelcode = item.SteelCode;
                if (num3 == by_orderdetailsa.Count() && by_orderdetails.Count() % 9 != 0)
                {
                    int j = (9 - by_orderdetails.Count % 9);
                    for (int i = 0; i < j; i++)
                    {
                        obj.SortCode = num3;
                        by_orderdetails.Add(obj);
                    }
                }
                num3 += 1;
            }

            if (by_orderdetails.Count() % 9 != 0)
            {
                int j = (9 - by_orderdetails.Count % 9);
                for (int i = 0; i < j; i++)
                {
                    obj.SortCode = num3;
                    by_orderdetails.Add(obj);
                }
            }

            var JsonData = new
            {
                OrderEntiy = by_order,
                OrderDetail = by_orderdetails,
                OrderDetailTotal = by_orderdetails.Sum(p => p.Qty),
                QuotationEntiy = by_quotation,
                Base_Customer = Base_Customer
            };
            return Content(JsonData.ToJson());
        }

        

        public ActionResult BCPrintDataJson(string OrderId, string OrderDetailIds, string QuotationId, string KL, int RowNum)
        {

            BY_Order by_order = new BY_Order();
            List<BY_OrderDetail> orderdetail_List = new List<BY_OrderDetail>();
            if (string.IsNullOrEmpty(OrderDetailIds))
            {
                orderdetail_List = (from items in DataFactory.Database().FindList<BY_OrderDetail>("OrderId", OrderId) where items.Length > 0 select items).ToList<BY_OrderDetail>();
                by_order = DataFactory.Database().FindEntity<BY_Order>(OrderId);

            }
            else
            {
                string[] OrderDetailids = OrderDetailIds.Trim(',').Split(',');
                for (int i = 0; i < OrderDetailids.Length; i++)
                {
                    BY_OrderDetail by_orderdetail = DataFactory.Database().FindEntity<BY_OrderDetail>(OrderDetailids[i]);
                    if (by_order == null || by_order.OrderId == null)
                    {
                        by_order = DataFactory.Database().FindEntity<BY_Order>(by_orderdetail.OrderId);
                    }
                    orderdetail_List.Add(by_orderdetail);
                }
            }
            if (!string.IsNullOrEmpty(QuotationId))
            {
                by_order = DataFactory.Database().FindEntity<BY_Order>("QuotationId", QuotationId);
                if (by_order == null || by_order.OrderId == null)
                {
                    return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：请先提交报价单。" }.ToString());
                }
                orderdetail_List = (from items in DataFactory.Database().FindList<BY_OrderDetail>("OrderId", by_order.OrderId) where items.Length > 0 select items).ToList<BY_OrderDetail>();
            }

            Base_Customer Base_Customer = DataFactory.Database().FindEntity<Base_Customer>(by_order.CustomerId);
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(by_order.QuotationId);

            Base_Company company = DataFactory.Database().FindEntity<Base_Company>("FullName", string.IsNullOrEmpty(by_quotation.CompanyName) ? "" : by_quotation.CompanyName);

            if (KL == "kl")
            {
                if (by_quotation.CustMaterial == 1)
                {
                    return Content(null);
                }
                foreach (BY_OrderDetail item in orderdetail_List)
                {
                    if (item.Weigh == 0)
                    {
                        item.SteelType = "圆料";
                    }
                }
                orderdetail_List = (from items in orderdetail_List orderby items.SteelCode, items.SteelType, items.Thick ascending select items).ToList<BY_OrderDetail>();

                List<BY_OrderDetail> by_orderdetails = new List<BY_OrderDetail>();

                BY_OrderDetail obj = new BY_OrderDetail();
                Type t = typeof(BY_OrderDetail);
                PropertyInfo[] myPropertyInfo;
                // Get the properties of 'Type' class object.             
                myPropertyInfo = t.GetProperties();
                for (int i = 0; i < myPropertyInfo.Length; i++)
                {
                    switch (myPropertyInfo[i].PropertyType.ToString())
                    {
                        case "System.Nullable`1[System.Int32]":
                            myPropertyInfo[i].SetValue(obj, 0, null);
                            break;
                        case "System.Nullable`1[System.Decimal]":
                            myPropertyInfo[i].SetValue(obj, Convert.ToDecimal(0), null);
                            break;
                        case "System.Nullable`1[System.DateTime]":
                            //myPropertyInfo[i].SetValue(obj, "", null);       
                            break;
                        case "System.DateTime":
                            //myPropertyInfo[i].SetValue(obj, "", null);       
                            break;
                        default:
                            myPropertyInfo[i].SetValue(obj, "", null);
                            break;
                    }
                }

                int num3 = 1;
                string SteelType = "";
                int pageNum = 1;

                //int dtn = by_order.CreateDate > Convert.ToDateTime(Convert.ToDateTime(by_order.CreateDate).ToShortDateString() + " 12:00:00") ? 0 : 1;
                //精
                foreach (BY_OrderDetail item in orderdetail_List)
                {

                    if (num3 == 1)
                    {
                        SteelType = item.SteelType;
                    }
                    if (item.SteelType != SteelType)
                    {
                        pageNum += 1;
                        int j = (RowNum - by_orderdetails.Count % RowNum);
                        if (j != RowNum)
                        {
                            for (int i = 0; i < j; i++)
                            {
                                obj.SortCode = num3;
                                by_orderdetails.Add(obj);
                            }
                        }

                    }
                    item.ScanCodeK = "A" + pageNum.ToString();
                    item.SortCode = num3;
                    by_orderdetails.Add(item);
                    SteelType = item.SteelType;
                    if (num3 == orderdetail_List.Count() && by_orderdetails.Count() % RowNum != 0)
                    {
                        int j = (RowNum - by_orderdetails.Count % RowNum);
                        if (j != RowNum)
                        {
                            for (int i = 0; i < j; i++)
                            {
                                obj.SortCode = num3;
                                by_orderdetails.Add(obj);
                            }
                        }
                    }
                    num3 += 1;
                }

                if (by_orderdetails.Count() % RowNum != 0)
                {
                    int j = (RowNum - by_orderdetails.Count % RowNum);
                    if (j != RowNum)
                    {
                        for (int i = 0; i < j; i++)
                        {
                            obj.SortCode = num3;
                            by_orderdetails.Add(obj);
                        }
                    }
                }
                foreach (BY_OrderDetail item in orderdetail_List)
                {
                    if (item.SteelType == "圆料")
                    {
                        item.SteelType = "毛料生产";
                    }
                    DataFactory.Database().Update(item);
                }

                var JsonData = new
                {
                    OrderEntiy = by_order,
                    OrderDetail = by_orderdetails,
                    OrderDetailTotal = orderdetail_List.Sum(p => p.Qty),
                    QuotationEntiy = by_quotation,
                    Base_Customer = Base_Customer,
                    company = company
                };
                return Content(JsonData.ToJson());

            }
            else
            {
                //orderdetail_List = (from items in orderdetail_List where items.OrderType != "毛料生产" orderby items.SteelType, items.Thick, items.LineCode ascending select items).ToList<BY_OrderDetail>();


                List<BY_OrderDetail> orderdetail_List2 = new List<BY_OrderDetail>();
                List<BY_OrderDetail> orderdetail_List21 = new List<BY_OrderDetail>();
                List<BY_OrderDetail> orderdetail_List3 = new List<BY_OrderDetail>();
                //orderdetail_List2 = orderdetail_List.Where(p => p.OrderType == "光板加工").OrderBy(p => p.SteelType).OrderBy(p => p.SteelCode).OrderBy(p => p.Thick).ToList();
                //orderdetail_List3 = orderdetail_List.Where(p => p.OrderType == "精板加工").OrderBy(p => p.SteelType).OrderBy(p => p.SteelCode).OrderBy(p => p.Thick).ToList();


                //orderdetail_List2 = (from items in orderdetail_List where items.OrderType == "光板加工" && items.SteelType == "特殊钢" orderby items.Thick ascending select items).ToList<BY_OrderDetail>();
                //orderdetail_List21 = (from items in orderdetail_List where items.OrderType == "光板加工" && items.SteelType == "普通钢" orderby items.Length ascending select items).ToList<BY_OrderDetail>();
                //orderdetail_List3 = (from items in orderdetail_List where items.OrderType == "精板加工" orderby items.SteelCode, items.SteelType, items.Thick ascending select items).ToList<BY_OrderDetail>();


                orderdetail_List2 = (from items in orderdetail_List  orderby items.ProductName, items.Thick ascending select items).ToList<BY_OrderDetail>();


                List<BY_OrderDetail> by_orderdetails = new List<BY_OrderDetail>();

                BY_OrderDetail obj = new BY_OrderDetail();
                Type t = typeof(BY_OrderDetail);
                PropertyInfo[] myPropertyInfo;
                // Get the properties of 'Type' class object.             
                myPropertyInfo = t.GetProperties();
                for (int i = 0; i < myPropertyInfo.Length; i++)
                {
                    switch (myPropertyInfo[i].PropertyType.ToString())
                    {
                        case "System.Nullable`1[System.Int32]":
                            myPropertyInfo[i].SetValue(obj, 0, null);
                            break;
                        case "System.Nullable`1[System.Decimal]":
                            myPropertyInfo[i].SetValue(obj, Convert.ToDecimal(0), null);
                            break;
                        case "System.Nullable`1[System.DateTime]":
                            //myPropertyInfo[i].SetValue(obj, "", null);       
                            break;
                        case "System.DateTime":
                            //myPropertyInfo[i].SetValue(obj, "", null);       
                            break;
                        default:
                            myPropertyInfo[i].SetValue(obj, "", null);
                            break;
                    }
                }

                int num3 = 1;
                string SteelType = "";
                int pageNum = 1;

                //int dtn = by_order.CreateDate > Convert.ToDateTime(Convert.ToDateTime(by_order.CreateDate).ToShortDateString() + " 12:00:00") ? 0 : 1;
                //精
                foreach (BY_OrderDetail item in orderdetail_List3)
                {

                    if (num3 == 1)
                    {
                        SteelType = item.SteelType;

                    }
                    if (item.SteelType != SteelType)
                    {
                        pageNum += 1;
                        int j = (RowNum - by_orderdetails.Count % RowNum);
                        if (j != RowNum)
                        {
                            for (int i = 0; i < j; i++)
                            {
                                obj.SortCode = num3;
                                by_orderdetails.Add(obj);
                            }
                        }
                    }
                    item.ScanCodeX = "B" + pageNum.ToString();
                    item.SortCode = num3;
                    by_orderdetails.Add(item);
                    SteelType = item.SteelType;
                    if (num3 == orderdetail_List3.Count() && by_orderdetails.Count() % RowNum != 0)
                    {
                        int j = (RowNum - by_orderdetails.Count % RowNum);
                        if (j != RowNum)
                        {
                            for (int i = 0; i < j; i++)
                            {
                                obj.SortCode = num3;
                                by_orderdetails.Add(obj);
                            }
                        }
                    }
                    num3 += 1;
                }

                if (by_orderdetails.Count() % RowNum != 0)
                {
                    int j = (RowNum - by_orderdetails.Count % RowNum);
                    if (j != RowNum)
                    {
                        for (int i = 0; i < j; i++)
                        {
                            obj.SortCode = num3;
                            by_orderdetails.Add(obj);
                        }
                    }
                }

                foreach (BY_OrderDetail item in orderdetail_List3)
                {
                    if (item.SteelType == "圆料")
                    {
                        item.SteelType = "毛料生产";
                    }
                    DataFactory.Database().Update(item);
                }


                //光
                int num2 = 1;
                SteelType = "";
                foreach (BY_OrderDetail item in orderdetail_List2)
                {

                    if (num2 == 1)
                    {
                        SteelType = item.SteelType;
                    }
                    if (item.SteelType != SteelType)
                    {
                        pageNum += 1;
                        int j = (RowNum - by_orderdetails.Count % RowNum);
                        if (j != RowNum)
                        {
                            for (int i = 0; i < j; i++)
                            {
                                obj.SortCode = num2;
                                by_orderdetails.Add(obj);
                            }
                        }
                    }
                    item.SortCode = num2;
                    item.ScanCodeX = "B" + pageNum.ToString();
                    by_orderdetails.Add(item);
                    SteelType = item.SteelType;
                    if (num2 == orderdetail_List2.Count() && by_orderdetails.Count() % RowNum != 0)
                    {
                        int j = (RowNum - by_orderdetails.Count % RowNum);
                        if (j != RowNum)
                        {
                            for (int i = 0; i < j; i++)
                            {
                                obj.SortCode = num2;
                                by_orderdetails.Add(obj);
                            }
                        }
                    }
                    num2 += 1;
                }

                if (by_orderdetails.Count() % RowNum != 0)
                {
                    int j = (RowNum - by_orderdetails.Count % RowNum);
                    if (j != RowNum)
                    {
                        for (int i = 0; i < j; i++)
                        {
                            obj.SortCode = num2;
                            by_orderdetails.Add(obj);
                        }
                    }
                }
                foreach (BY_OrderDetail item in orderdetail_List2)
                {
                    if (item.SteelType == "圆料")
                    {
                        item.SteelType = "毛料生产";
                    }
                    DataFactory.Database().Update(item);
                }




                //光板，普通钢
                //光
                List<BY_OrderDetail> templist = new List<BY_OrderDetail>();
                decimal? totalThick = 0;
                decimal? Length1 = 0;
                num2 = 1;
                foreach (BY_OrderDetail item in orderdetail_List21)
                {

                    if (num2 == 1)
                    {
                        Length1 = item.Length;
                    }
                    totalThick = totalThick + item.Thick * item.Qty;
                    if (totalThick > 300 || item.Length - Length1 > 300)
                    {
                        by_orderdetails.AddRange(templist.OrderBy(p => p.Thick).ThenBy(p => p.Weigh).ThenBy(p => p.Length));
                        templist.Clear();
                        pageNum += 1;
                        int j = (RowNum - by_orderdetails.Count % RowNum);
                        if (j != RowNum)
                        {
                            for (int i = 0; i < j; i++)
                            {
                                obj.SortCode = num2;
                                by_orderdetails.Add(obj);
                            }
                        }
                        totalThick = item.Thick * item.Qty;
                    }
                    Length1 = item.Length;
                    item.SortCode = num2;
                    item.ScanCodeX = "B" + pageNum.ToString();
                    //by_orderdetails.Add(item);
                    templist.Add(item);
                    if (num2 == orderdetail_List21.Count() && by_orderdetails.Count() % RowNum != 0)
                    {
                        by_orderdetails.AddRange(templist.OrderBy(p => p.Thick).ThenBy(p => p.Weigh).ThenBy(p => p.Length));
                        templist.Clear();
                        int j = (RowNum - by_orderdetails.Count % RowNum);
                        if (j != RowNum)
                        {
                            for (int i = 0; i < j; i++)
                            {
                                obj.SortCode = num2;
                                by_orderdetails.Add(obj);
                            }
                        }
                    }
                    num2 += 1;
                }

                if (by_orderdetails.Count() % RowNum != 0)
                {
                    int j = (RowNum - by_orderdetails.Count % RowNum);
                    if (j != RowNum)
                    {
                        for (int i = 0; i < j; i++)
                        {
                            obj.SortCode = num2;
                            by_orderdetails.Add(obj);
                        }
                    }
                }

                foreach (BY_OrderDetail item in orderdetail_List21)
                {
                    if (item.SteelType == "圆料")
                    {
                        item.SteelType = "毛料生产";
                    }
                    DataFactory.Database().Update(item);
                }



                var JsonData = new
                {
                    OrderEntiy = by_order,
                    OrderDetail = by_orderdetails,
                    OrderDetailTotal = orderdetail_List.Sum(p => p.Qty),
                    QuotationEntiy = by_quotation,
                    Base_Customer = Base_Customer,
                    company = company
                };
                return Content(JsonData.ToJson());
            }



        }

        public ActionResult BCPrintDataJsonLogo(string OrderId, string OrderDetailIds,string QuotationId, int RowNum)
        {

            BY_Order by_order = new BY_Order();
            List<BY_OrderDetail> orderdetail_List = new List<BY_OrderDetail>();
            if (string.IsNullOrEmpty(OrderDetailIds))
            {
                orderdetail_List = (from items in DataFactory.Database().FindListBySql<BY_OrderDetail>(@" select * from BY_OrderDetail where  OrderId = '" + OrderId + @"' and OrderType != '毛料生产' 
")
                                    where items.Length > 0
                                    select items).ToList<BY_OrderDetail>();
                by_order = DataFactory.Database().FindEntity<BY_Order>(OrderId);

            }
            else
            {
                string[] OrderDetailids = OrderDetailIds.Trim(',').Split(',');
                for (int i = 0; i < OrderDetailids.Length; i++)
                {
                    BY_OrderDetail by_orderdetail = DataFactory.Database().FindEntity<BY_OrderDetail>(OrderDetailids[i]);
                    if (by_order == null || by_order.OrderId == null)
                    {
                        by_order = DataFactory.Database().FindEntity<BY_Order>(by_orderdetail.OrderId);
                    }
                    orderdetail_List.Add(by_orderdetail);
                }
            }

            if (!string.IsNullOrEmpty(QuotationId))
            {
                by_order = DataFactory.Database().FindEntity<BY_Order>("QuotationId", QuotationId);
                if (by_order == null || by_order.OrderId == null)
                {
                    return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：请先提交报价单。" }.ToString());
                }
                orderdetail_List = (from items in DataFactory.Database().FindListBySql<BY_OrderDetail>(@" select * from BY_OrderDetail where  OrderId = '" + by_order.OrderId + @"' and OrderType != '毛料生产' 
")
                                    where items.Length > 0
                                    select items).ToList<BY_OrderDetail>();
            }

            Base_Customer Base_Customer = DataFactory.Database().FindEntity<Base_Customer>(by_order.CustomerId);
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(by_order.QuotationId);
            if (by_quotation.CustMaterial == 1)
            {
                return Content(null);
            }
            Base_Company company = DataFactory.Database().FindEntity<Base_Company>("FullName", string.IsNullOrEmpty(by_quotation.CompanyName) ? "" : by_quotation.CompanyName);

            orderdetail_List = (from items in orderdetail_List where items.OrderType != "毛料生产" orderby items.LineCode ascending select items).ToList<BY_OrderDetail>();

            BY_OrderDetail obj = new BY_OrderDetail();
            Type t = typeof(BY_OrderDetail);
            PropertyInfo[] myPropertyInfo;
            // Get the properties of 'Type' class object.             
            myPropertyInfo = t.GetProperties();
            for (int i = 0; i < myPropertyInfo.Length; i++)
            {
                switch (myPropertyInfo[i].PropertyType.ToString())
                {
                    case "System.Nullable`1[System.Int32]":
                        myPropertyInfo[i].SetValue(obj, 0, null);
                        break;
                    case "System.Nullable`1[System.Decimal]":
                        myPropertyInfo[i].SetValue(obj, Convert.ToDecimal(0), null);
                        break;
                    case "System.Nullable`1[System.DateTime]":
                        //myPropertyInfo[i].SetValue(obj, "", null);       
                        break;
                    case "System.DateTime":
                        //myPropertyInfo[i].SetValue(obj, "", null);       
                        break;
                    default:
                        myPropertyInfo[i].SetValue(obj, "", null);
                        break;
                }
            }


            if (orderdetail_List.Count() % RowNum != 0)
            {
                int j = (RowNum - orderdetail_List.Count % RowNum);
                for (int i = 0; i < j; i++)
                {
                    orderdetail_List.Add(obj);
                }
            }
            var JsonData = new
            {
                OrderEntiy = by_order,
                OrderDetail = orderdetail_List,
                OrderDetailTotal = orderdetail_List.Sum(p => p.Qty),
                QuotationEntiy = by_quotation,
                Base_Customer = Base_Customer,
                company = company
            };
            return Content(JsonData.ToJson());
        }

        public ActionResult BCPrintDataJsonTotal(string OrderId, string OrderDetails,string QuotationId, int RowNum)
        {

            BY_Order by_order = new BY_Order();
            List<BY_OrderDetail> orderdetail_List = new List<BY_OrderDetail>();
            if (string.IsNullOrEmpty(OrderDetails))
            {
                orderdetail_List = (from items in DataFactory.Database().FindList<BY_OrderDetail>("OrderId", OrderId) where items.Length > 0 select items).ToList<BY_OrderDetail>();
                by_order = DataFactory.Database().FindEntity<BY_Order>(OrderId);

            }
            else
            {
                string[] OrderDetailids = OrderDetails.Trim(',').Split(',');
                for (int i = 0; i < OrderDetailids.Length; i++)
                {
                    BY_OrderDetail by_orderdetail = DataFactory.Database().FindEntity<BY_OrderDetail>(OrderDetailids[i]);
                    if (by_order == null || by_order.OrderId == null)
                    {
                        by_order = DataFactory.Database().FindEntity<BY_Order>(by_orderdetail.OrderId);
                    }
                    orderdetail_List.Add(by_orderdetail);
                }
            }
            if (!string.IsNullOrEmpty(QuotationId))
            {
                by_order = DataFactory.Database().FindEntity<BY_Order>("QuotationId", QuotationId);
                if (by_order == null || by_order.OrderId == null)
                {
                    return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：请先提交报价单。" }.ToString());
                }
                orderdetail_List = (from items in DataFactory.Database().FindList<BY_OrderDetail>("OrderId", by_order.OrderId) where items.Length > 0 select items).ToList<BY_OrderDetail>();
            }
            Base_Customer Base_Customer = DataFactory.Database().FindEntity<Base_Customer>(by_order.CustomerId);
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>( by_order.QuotationId);
            if (by_quotation.CustMaterial == 1)
            {
                return Content(null);
            }

            orderdetail_List = (from items in orderdetail_List orderby items.SteelCode, items.Thick, items.Weigh, items.Length, items.LineCode ascending select items).ToList<BY_OrderDetail>();

            BY_OrderDetail obj = new BY_OrderDetail();
            Type t = typeof(BY_OrderDetail);
            PropertyInfo[] myPropertyInfo;
            // Get the properties of 'Type' class object.             
            myPropertyInfo = t.GetProperties();
            for (int i = 0; i < myPropertyInfo.Length; i++)
            {
                switch (myPropertyInfo[i].PropertyType.ToString())
                {
                    case "System.Nullable`1[System.Int32]":
                        myPropertyInfo[i].SetValue(obj, 0, null);
                        break;
                    case "System.Nullable`1[System.Decimal]":
                        myPropertyInfo[i].SetValue(obj, Convert.ToDecimal(0), null);
                        break;
                    case "System.Nullable`1[System.DateTime]":
                        //myPropertyInfo[i].SetValue(obj, "", null);       
                        break;
                    case "System.DateTime":
                        //myPropertyInfo[i].SetValue(obj, "", null);       
                        break;
                    default:
                        myPropertyInfo[i].SetValue(obj, "", null);
                        break;
                }
            }


            if (orderdetail_List.Count() % RowNum != 0)
            {
                int j = (RowNum - orderdetail_List.Count % RowNum);
                for (int i = 0; i < j; i++)
                {
                    orderdetail_List.Add(obj);
                }
            }
            var JsonData = new
            {
                OrderEntiy = by_order,
                OrderDetail = orderdetail_List,
                OrderDetailTotal = orderdetail_List.Sum(p => p.Qty),
                QuotationEntiy = by_quotation,
                Base_Customer = Base_Customer
            };
            return Content(JsonData.ToJson());
        }

        public ActionResult BYDPrintList(string OrderId, string OrderDetails,string QuotationId, int RowNum)
        {

            BY_Order by_order = new BY_Order();
            List<BY_OrderDetail> orderdetail_List = new List<BY_OrderDetail>();
            if (string.IsNullOrEmpty(OrderDetails))
            {
                orderdetail_List = (from items in DataFactory.Database().FindList<BY_OrderDetail>("OrderId", OrderId) where items.Length > 0 select items).ToList<BY_OrderDetail>();
                by_order = DataFactory.Database().FindEntity<BY_Order>(OrderId);

            }
            else
            {
                string[] OrderDetailids = OrderDetails.Trim(',').Split(',');
                for (int i = 0; i < OrderDetailids.Length; i++)
                {
                    BY_OrderDetail by_orderdetail = DataFactory.Database().FindEntity<BY_OrderDetail>(OrderDetailids[i]);
                    if (by_order == null || by_order.OrderId == null)
                    {
                        by_order = DataFactory.Database().FindEntity<BY_Order>(by_orderdetail.OrderId);
                    }
                    orderdetail_List.Add(by_orderdetail);
                }
            }
            if (!string.IsNullOrEmpty(QuotationId))
            {
                by_order = DataFactory.Database().FindEntity<BY_Order>("QuotationId", QuotationId);
                if (by_order == null || by_order.OrderId == null)
                {
                    return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：请先提交报价单。" }.ToString());
                }
                orderdetail_List = (from items in DataFactory.Database().FindList<BY_OrderDetail>("OrderId", by_order.OrderId) where items.Length > 0 select items).ToList<BY_OrderDetail>();
            }
            Base_Customer Base_Customer = DataFactory.Database().FindEntity<Base_Customer>(by_order.CustomerId);
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(by_order.QuotationId);
            if (by_quotation.CustMaterial == 1)
            {
                return Content(null);
            }

            orderdetail_List = (from items in orderdetail_List orderby items.LineCode ascending select items).ToList<BY_OrderDetail>();

            BY_OrderDetailPrint obj = new BY_OrderDetailPrint();
            Type t = typeof(BY_OrderDetailPrint);
            PropertyInfo[] myPropertyInfo;
            // Get the properties of 'Type' class object.             
            myPropertyInfo = t.GetProperties();
            for (int i = 0; i < myPropertyInfo.Length; i++)
            {
                switch (myPropertyInfo[i].PropertyType.ToString())
                {
                    case "System.Nullable`1[System.Int32]":
                        myPropertyInfo[i].SetValue(obj, 0, null);
                        break;
                    case "System.Nullable`1[System.Decimal]":
                        myPropertyInfo[i].SetValue(obj, Convert.ToDecimal(0), null);
                        break;
                    case "System.Nullable`1[System.DateTime]":
                        //myPropertyInfo[i].SetValue(obj, "", null);       
                        break;
                    case "System.DateTime":
                        //myPropertyInfo[i].SetValue(obj, "", null);       
                        break;
                    
                }
            }

            List<BY_OrderDetailPrint> by_orderdetailprints = new List<BY_OrderDetailPrint>();
            foreach (BY_OrderDetail item in orderdetail_List)
            {
                BY_OrderDetailPrint by_orderdetailprint = new Entity.BY_OrderDetailPrint();
                CommonHelper.CopySameFieldsObjectALL<BY_OrderDetail, BY_OrderDetailPrint>(item, ref by_orderdetailprint);
                Base_Steel base_steel = DataFactory.Database().FindEntity<Base_Steel>(by_orderdetailprint.SteelId);
                by_orderdetailprint.base_steel = base_steel;
                by_orderdetailprints.Add(by_orderdetailprint);
            }
            if (by_orderdetailprints.Count() % RowNum != 0)
            {
                int j = (RowNum - by_orderdetailprints.Count % RowNum);
                for (int i = 0; i < j; i++)
                {
                    by_orderdetailprints.Add(obj);
                }
            }
            var JsonData = new
            {
                OrderEntiy = by_order,
                OrderDetail = by_orderdetailprints,
                OrderDetailTotal = orderdetail_List.Sum(p => p.Qty),
                QuotationEntiy = by_quotation,
                Base_Customer = Base_Customer
            };
            return Content(JsonData.ToJson());
        }
        public ActionResult YCPrintList(string OrderId, string OrderDetails,string QuotationId,  int RowNum)
        {

            BY_Order by_order = new BY_Order();
            List<BY_OrderDetail> orderdetail_List = new List<BY_OrderDetail>();
            if (string.IsNullOrEmpty(OrderDetails))
            {
                orderdetail_List = (from items in DataFactory.Database().FindList<BY_OrderDetail>("OrderId", OrderId) where items.Length > 0 select items).ToList<BY_OrderDetail>();
                by_order = DataFactory.Database().FindEntity<BY_Order>(OrderId);

            }
            else
            {
                string[] OrderDetailids = OrderDetails.Trim(',').Split(',');
                for (int i = 0; i < OrderDetailids.Length; i++)
                {
                    BY_OrderDetail by_orderdetail = DataFactory.Database().FindEntity<BY_OrderDetail>(OrderDetailids[i]);
                    if (by_order == null || by_order.OrderId == null)
                    {
                        by_order = DataFactory.Database().FindEntity<BY_Order>(by_orderdetail.OrderId);
                    }
                    orderdetail_List.Add(by_orderdetail);
                }
            }
            if (!string.IsNullOrEmpty(QuotationId))
            {
                by_order = DataFactory.Database().FindEntity<BY_Order>("QuotationId", QuotationId);
                if (by_order == null || by_order.OrderId == null)
                {
                    return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：请先提交报价单。" }.ToString());
                }
                orderdetail_List = (from items in DataFactory.Database().FindList<BY_OrderDetail>("OrderId", by_order.OrderId) where items.Length > 0 select items).ToList<BY_OrderDetail>();
            }
            Base_Customer Base_Customer = DataFactory.Database().FindEntity<Base_Customer>(by_order.CustomerId);
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>( by_order.QuotationId);
            if (by_quotation.CustMaterial == 1)
            {
                return Content(null);
            }

            orderdetail_List = (from items in orderdetail_List orderby items.LineCode ascending select items).ToList<BY_OrderDetail>();

            BY_OrderDetailPrint obj = new BY_OrderDetailPrint();
            Type t = typeof(BY_OrderDetailPrint);
            PropertyInfo[] myPropertyInfo;
            // Get the properties of 'Type' class object.             
            myPropertyInfo = t.GetProperties();
            for (int i = 0; i < myPropertyInfo.Length; i++)
            {
                switch (myPropertyInfo[i].PropertyType.ToString())
                {
                    case "System.Nullable`1[System.Int32]":
                        myPropertyInfo[i].SetValue(obj, 0, null);
                        break;
                    case "System.Nullable`1[System.Decimal]":
                        myPropertyInfo[i].SetValue(obj, Convert.ToDecimal(0), null);
                        break;
                    case "System.Nullable`1[System.DateTime]":
                        //myPropertyInfo[i].SetValue(obj, "", null);       
                        break;
                    case "System.DateTime":
                        //myPropertyInfo[i].SetValue(obj, "", null);       
                        break;
                    
                }
            }

            List<BY_OrderDetailPrint> by_orderdetailprints = new List<BY_OrderDetailPrint>();
            foreach (BY_OrderDetail item in orderdetail_List)
            {
                BY_OrderDetailPrint by_orderdetailprint = new Entity.BY_OrderDetailPrint();
                CommonHelper.CopySameFieldsObjectALL<BY_OrderDetail, BY_OrderDetailPrint>(item, ref by_orderdetailprint);
                Base_Steel base_steel = DataFactory.Database().FindEntity<Base_Steel>(by_orderdetailprint.SteelId);
                by_orderdetailprint.base_steel = base_steel;
                by_orderdetailprints.Add(by_orderdetailprint);
            }


            if (by_orderdetailprints.Count() % RowNum != 0)
            {
                int j = (RowNum - by_orderdetailprints.Count % RowNum);
                for (int i = 0; i < j; i++)
                {
                    by_orderdetailprints.Add(obj);
                }
            }
            var JsonData = new
            {
                OrderEntiy = by_order,
                OrderDetail = by_orderdetailprints,
                OrderDetailTotal = orderdetail_List.Sum(p => p.Qty),
                QuotationEntiy = by_quotation,
                Base_Customer = Base_Customer
            };
            return Content(JsonData.ToJson());
        }

        public ActionResult YCPrintList2(string OrderId, string OrderDetails,string QuotationId, int RowNum)
        {

            BY_Order by_order = new BY_Order();
            List<BY_OrderDetail> orderdetail_List = new List<BY_OrderDetail>();
            if (string.IsNullOrEmpty(OrderDetails))
            {
                orderdetail_List = (from items in DataFactory.Database().FindList<BY_OrderDetail>("OrderId", OrderId) where items.Length > 0 select items).ToList<BY_OrderDetail>();
                by_order = DataFactory.Database().FindEntity<BY_Order>(OrderId);

            }
            else
            {
                string[] OrderDetailids = OrderDetails.Trim(',').Split(',');
                for (int i = 0; i < OrderDetailids.Length; i++)
                {
                    BY_OrderDetail by_orderdetail = DataFactory.Database().FindEntity<BY_OrderDetail>(OrderDetailids[i]);
                    if (by_order == null || by_order.OrderId == null)
                    {
                        by_order = DataFactory.Database().FindEntity<BY_Order>(by_orderdetail.OrderId);
                    }
                    orderdetail_List.Add(by_orderdetail);
                }
            }
            if (!string.IsNullOrEmpty(QuotationId))
            {
                by_order = DataFactory.Database().FindEntity<BY_Order>("QuotationId", QuotationId);
                if (by_order == null || by_order.OrderId == null)
                {
                    return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：请先提交报价单。" }.ToString());
                }
                orderdetail_List = (from items in DataFactory.Database().FindList<BY_OrderDetail>("OrderId", by_order.OrderId) where items.Length > 0 select items).ToList<BY_OrderDetail>();
            }
            Base_Customer Base_Customer = DataFactory.Database().FindEntity<Base_Customer>(by_order.CustomerId);
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(by_order.QuotationId);
            if (by_quotation.CustMaterial == 1)
            {
                return Content(null);
            }
            

            orderdetail_List = (from items in orderdetail_List orderby items.SteelCode, items.LineCode ascending select items).ToList<BY_OrderDetail>();
            List<BY_OrderDetail> by_orderdetails = new List<BY_OrderDetail>();
            List<BY_OrderDetail> by_orderdetailsa = new List<BY_OrderDetail>();
            by_orderdetailsa = orderdetail_List;

            BY_OrderDetail obj = new BY_OrderDetail();
            Type t = typeof(BY_OrderDetail);
            PropertyInfo[] myPropertyInfo;
            // Get the properties of 'Type' class object.             
            myPropertyInfo = t.GetProperties();
            for (int i = 0; i < myPropertyInfo.Length; i++)
            {
                switch (myPropertyInfo[i].PropertyType.ToString())
                {
                    case "System.Nullable`1[System.Int32]":
                        myPropertyInfo[i].SetValue(obj, 0, null);
                        break;
                    case "System.Nullable`1[System.Decimal]":
                        myPropertyInfo[i].SetValue(obj, Convert.ToDecimal(0), null);
                        break;
                    case "System.Nullable`1[System.DateTime]":
                        //myPropertyInfo[i].SetValue(obj, "", null);       
                        break;
                    case "System.DateTime":
                        //myPropertyInfo[i].SetValue(obj, "", null);       
                        break;

                }
            }

            int num3 = 1;
            string steelcode = "";
            int pageNum = 1;
            //精
            foreach (BY_OrderDetail item in by_orderdetailsa)
            {
                if (num3 == 1)
                {
                    steelcode = item.SteelCode;
                }
                if (item.SteelCode != steelcode)
                {
                    pageNum += 1;
                    int j = (RowNum - by_orderdetails.Count % RowNum);
                    for (int i = 0; i < j; i++)
                    {
                        obj.SortCode = num3;
                        by_orderdetails.Add(obj);
                    }
                }
                item.SortCode = num3;
                by_orderdetails.Add(item);
                steelcode = item.SteelCode;
                if (num3 == by_orderdetailsa.Count() && by_orderdetails.Count() % RowNum != 0)
                {
                    int j = (RowNum - by_orderdetails.Count % RowNum);
                    for (int i = 0; i < j; i++)
                    {
                        obj.SortCode = num3;
                        by_orderdetails.Add(obj);
                    }
                }
                num3 += 1;
            }

            if (by_orderdetails.Count() % RowNum != 0)
            {
                int j = (RowNum - by_orderdetails.Count % RowNum);
                for (int i = 0; i < j; i++)
                {
                    obj.SortCode = num3;
                    by_orderdetails.Add(obj);
                }
            }


            List<BY_OrderDetailPrint> by_orderdetailprints = new List<BY_OrderDetailPrint>();
            foreach (BY_OrderDetail item in by_orderdetails)
            {
                if (!string.IsNullOrEmpty(item.SteelCode))
                {
                    BY_OrderDetailPrint by_orderdetailprint = new Entity.BY_OrderDetailPrint();
                    CommonHelper.CopySameFieldsObjectALL<BY_OrderDetail, BY_OrderDetailPrint>(item, ref by_orderdetailprint);
                    Base_Steel base_steel = DataFactory.Database().FindEntity<Base_Steel>(by_orderdetailprint.SteelId);
                    by_orderdetailprint.base_steel = base_steel;
                    by_orderdetailprints.Add(by_orderdetailprint);
                }
                else
                {
                    BY_OrderDetailPrint by_orderdetailprint = new Entity.BY_OrderDetailPrint();
                    CommonHelper.CopySameFieldsObjectALL<BY_OrderDetail, BY_OrderDetailPrint>(item, ref by_orderdetailprint);
                    by_orderdetailprints.Add(by_orderdetailprint);
                }
            }


            var JsonData = new
            {
                OrderEntiy = by_order,
                OrderDetail = by_orderdetailprints,
                OrderDetailTotal = orderdetail_List.Sum(p => p.Qty),
                QuotationEntiy = by_quotation,
                Base_Customer = Base_Customer
            };
            return Content(JsonData.ToJson());
        }


        #endregion

        #region 精料单打印
        /// <summary>
        /// 精料单打印
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult BJPrint()
        {
            return View();
        }
        /// <summary>
        /// 精料单打印
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult BJPrintList()
        {
            return View();
        }
        /// <summary>
        /// 精料单打印预览
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult BJPrintPreview()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult BJPrintPreviewJY()
        {
            return View();
        }
        public ActionResult BCPrintListJsonJYForLab(string OrderId, string OrderDetails, string QuotationId,string type)
        {
            BY_Order by_order = new BY_Order();
            IEnumerable<BY_OrderDetail> orderdetail_List = new List<BY_OrderDetail>();
            List<BY_OrderDetail> orderdetail_ListForlab = new List<BY_OrderDetail>();
            List<BY_OrderDetail> orderdetail_ListForlab2 = new List<BY_OrderDetail>();

            if (!string.IsNullOrEmpty(OrderId))
            {
                orderdetail_List = DataFactory.Database().FindList<BY_OrderDetail>("OrderId", OrderId);
                by_order = DataFactory.Database().FindEntity<BY_Order>(OrderId);
            }
            else if (!string.IsNullOrEmpty(QuotationId))
            {
                by_order = DataFactory.Database().FindEntity<BY_Order>("QuotationId", QuotationId);
                if (by_order == null || by_order.OrderId == null)
                {
                    return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：请先提交报价单。" }.ToString());
                }
                orderdetail_List = (from items in DataFactory.Database().FindList<BY_OrderDetail>("OrderId", by_order.OrderId) where items.Length > 0 select items).ToList<BY_OrderDetail>();
            }
            else if(!string.IsNullOrEmpty(OrderDetails))
            {
                List<BY_OrderDetail> tempList = new List<BY_OrderDetail>();
                string[] OrderDetailids = OrderDetails.Trim(',').Split(',');
                for (int i = 0; i < OrderDetailids.Length; i++)
                {
                    BY_OrderDetail by_orderdetail = DataFactory.Database().FindEntity<BY_OrderDetail>(OrderDetailids[i]);
                    if (by_order == null || by_order.OrderId == null)
                    {
                        by_order = DataFactory.Database().FindEntity<BY_Order>(by_orderdetail.OrderId);
                    }
                    tempList.Add(by_orderdetail);
                }
                orderdetail_List = tempList;
            }
            else
            {
                return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：请先提交报价单。" }.ToString());
            }
            BY_OrderDetail BY_OrderDetaillab = new BY_OrderDetail(); ;
            foreach (var item in orderdetail_List)
            {
                for (int i = 0; i < item.Qty; i++)
                {
                    orderdetail_ListForlab.Add(item);
                }
            }
            //二维码
            if (type == "2")
            {
                foreach (var item in orderdetail_ListForlab)
                {

                    orderdetail_ListForlab2.Add(item);
                    BY_OrderDetail od2 = new BY_OrderDetail();
                    od2.LineCode = item.LineCode;
                    od2.Length = item.Length;
                    od2.Weigh = item.Weigh;
                    od2.Thick = item.Thick;
                    od2.OrderCode = item.OrderCode;
                    od2.DingCode = item.DingCode;
                    od2.Qty = item.Qty;
                    if (!string.IsNullOrWhiteSpace(od2.DingCode) && od2.DingCode != "&nbsp;")
                    {
                        orderdetail_ListForlab2.Add(od2);
                    }

                }
            }
            else
            {
                orderdetail_ListForlab2 = orderdetail_ListForlab;
            }

            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(by_order.QuotationId);
            Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>("Code", by_quotation.CustomerCode);

            var JsonData = new
            {
                OrderEntiy = by_order,
                OrderDetail = orderdetail_ListForlab2.OrderBy(p => p.LineCode),
                OrderDetailTotal = orderdetail_List.Sum(p => p.Qty),
                Customer = base_customer,
                QuotationEntiy = by_quotation
            };
            return Content(JsonData.ToJson());
        }
        /// <summary>
        /// 获取要打印数据（返回订单信息）
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public ActionResult BJPrintDataJson(string OrderId, string OrderDetails, string QuotationId)
        {
            BY_Order by_order = new BY_Order();
            List<BY_OrderDetail> orderdetail_List = new List<BY_OrderDetail>();
            List<BY_OrderDetail> orderdetail_ListD = new List<BY_OrderDetail>();
            if (string.IsNullOrEmpty(OrderDetails))
            {
                orderdetail_List = (from items in DataFactory.Database().FindList<BY_OrderDetail>("OrderId", OrderId) where items.Length > 0 select items).ToList<BY_OrderDetail>(); 
                by_order = DataFactory.Database().FindEntity<BY_Order>(OrderId);
            }
            else
            {
                string[] OrderDetailids = OrderDetails.Trim(',').Split(',');
                for (int i = 0; i < OrderDetailids.Length; i++)
                {
                    BY_OrderDetail by_orderdetail = DataFactory.Database().FindEntity<BY_OrderDetail>(OrderDetailids[i]);
                    if (by_order == null || by_order.OrderId == null)
                    {
                        by_order = DataFactory.Database().FindEntity<BY_Order>(by_orderdetail.OrderId);
                    }
                    orderdetail_List.Add(by_orderdetail);
                }
            }
            if (!string.IsNullOrEmpty(QuotationId))
            {
                by_order = DataFactory.Database().FindEntity<BY_Order>("QuotationId", QuotationId);
                if (by_order == null || by_order.OrderId == null)
                {
                    return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：请先提交报价单。" }.ToString());
                }
                orderdetail_List = (from items in DataFactory.Database().FindList<BY_OrderDetail>("OrderId", by_order.OrderId) where items.Length > 0 select items).ToList<BY_OrderDetail>();
            }
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(by_order.QuotationId);
            Base_Customer Base_Customer = DataFactory.Database().FindEntity<Base_Customer>(by_order.CustomerId);
            foreach (BY_OrderDetail item in orderdetail_List)
            {
                IEnumerable<BY_OrderDetailProcess> by_orderdetailprocesss = from items in DataFactory.Database().FindList<BY_OrderDetailProcess>("OrderDetailId", item.OrderDetailId) orderby items.SortCode select items;
                Base_Steel base_steel = DataFactory.Database().FindEntity<Base_Steel>("SteelCode", item.SteelCode);
                if (item.Thick != 0 && item.Length != 0 && item.Weigh == 0)
                {
                    item.Weight = (( (item.Thick/2)  *  (item.Thick/2) * CommonHelper.GetDecimal(3.14)   *  item.Length *item.Qty ) * CommonHelper.GetDecimal(base_steel.Density)) / 1000000;
                    //if (BoreSize != 0)
                    //{
                    //    _Weight = _Weight - Length * (BoreSize / 2) * (BoreSize / 2) * CommonHelper.GetDecimal(3.14) * CommonHelper.GetDecimal(base_steel.Density) / 1000000;
                    //}
                }
                else
                {
                    item.Weight = ((item.Thick * item.Weigh * item.Length *item.Qty) * CommonHelper.GetDecimal(base_steel.Density)) / 1000000;
                }
                
                
                string strProcess = string.Empty;
                if (item.OrderType != "毛料生产")
                {
                    foreach (BY_OrderDetailProcess entity in by_orderdetailprocesss)
                    {
                        //if (item.OrderType == "光板加工" && entity.TechnologyId == "J001")
                        //{
                        //    continue;
                        //}
                        //if (item.OrderType == "光板加工" && entity.TechnologyId == "CX01")
                        //{
                        //    strProcess = strProcess + entity.TechnologyName + "(精铣)-";
                        //    continue;
                        //}
                        //if (entity.TechnologyId == "JP01")
                        //{
                        //    continue;
                        //}
                        //if (entity.TechnologyId == "P001")
                        //{
                        //    strProcess = strProcess + entity.TechnologyName + "(精平铣)-";
                        //    continue;
                        //}
                        strProcess = strProcess + entity.TechnologyName + "-";
                    }
                    strProcess = strProcess.TrimEnd('-');
                    item.ScaleProcess = strProcess;
                }
                else if (item.OrderType == "毛料生产" && !string.IsNullOrEmpty(item.Technology.Replace("&nbsp;", "")))
                {
                    foreach (BY_OrderDetailProcess entity in by_orderdetailprocesss)
                    {
                        //if (entity.TechnologyId == "JP01")
                        //{
                        //    continue;
                        //}
                        //if (entity.TechnologyId == "P001")
                        //{
                        //    strProcess = strProcess + entity.TechnologyName + "(精平铣)-";
                        //    continue;
                        //}
                        strProcess = strProcess + entity.TechnologyName + "-";
                    }
                    strProcess = strProcess + item.Technology.Replace("&nbsp;", "");
                    strProcess = strProcess.TrimEnd('-');
                    item.ScaleProcess = strProcess;
                }
                else
                {
                    if (by_quotation.CustMaterial != 1)
                    {
                        if (by_orderdetailprocesss.Count() < 2)
                        {
                            orderdetail_ListD.Add(item);
                        }
                    }
                    if (by_quotation.CustMaterial == 1)
                    {
                        if (by_orderdetailprocesss.Count() < 1)
                        {
                            orderdetail_ListD.Add(item);
                        }
                    }
                    
                }
            }
            foreach (BY_OrderDetail item in orderdetail_ListD)
            {
                orderdetail_List.Remove(item);
            }

            var JsonData = new
            {
                OrderEntiy = by_order,
                OrderDetail = orderdetail_List.OrderBy(p=>p.LineCode),
                OrderDetailQty = orderdetail_List.Sum(p=>p.Qty),
                QuotationEntiy = by_quotation,
                Base_Customer = Base_Customer
            };
            return Content(JsonData.ToJson());
        }
        /// <summary>
        /// 获取要打印数据（返回订单信息）
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public ActionResult BCPrintListJson(string OrderId, string OrderDetails, string QuotationId)
        {
            BY_Order by_order = new BY_Order();
            IEnumerable<BY_OrderDetail> orderdetail_List = new List<BY_OrderDetail>();
            if (string.IsNullOrEmpty(OrderDetails))
            {
                orderdetail_List = DataFactory.Database().FindList<BY_OrderDetail>("OrderId", OrderId).Where(p => p.Length > 0);
                by_order = DataFactory.Database().FindEntity<BY_Order>(OrderId);
            }
            else
            {
                List<BY_OrderDetail> tempList = new List<BY_OrderDetail>();
                string[] OrderDetailids = OrderDetails.Trim(',').Split(',');
                for (int i = 0; i < OrderDetailids.Length; i++)
                {
                    BY_OrderDetail by_orderdetail = DataFactory.Database().FindEntity<BY_OrderDetail>(OrderDetailids[i]);
                    if (by_order == null || by_order.OrderId == null)
                    {
                        by_order = DataFactory.Database().FindEntity<BY_Order>(by_orderdetail.OrderId);
                    }
                    tempList.Add(by_orderdetail);
                }
                orderdetail_List = tempList;
            }
            if (!string.IsNullOrEmpty(QuotationId))
            {
                by_order = DataFactory.Database().FindEntity<BY_Order>("QuotationId", QuotationId);
                if (by_order == null || by_order.OrderId == null)
                {
                    return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：请先提交报价单。" }.ToString());
                }
                orderdetail_List = (from items in DataFactory.Database().FindList<BY_OrderDetail>("OrderId", by_order.OrderId) where items.Length > 0 select items).ToList<BY_OrderDetail>();
            }
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>( by_order.QuotationId);
            Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>("Code", by_quotation.CustomerCode);

            var JsonData = new
            {
                OrderEntiy = by_order,
                OrderDetail = orderdetail_List.OrderBy(p => p.LineCode),
                OrderDetailTotal = orderdetail_List.Sum(p => p.Qty),
                Customer = base_customer,
                QuotationEntiy = by_quotation
            };
            return Content(JsonData.ToJson());
        }
        #endregion

        #region 更改订单状态+明细状态
        /// <summary>
        /// 更改订单状态
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public ActionResult UpdateOrderStatus(string OrderId, string KeyValueQ)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                //if (string.IsNullOrEmpty(OrderId) && string.IsNullOrEmpty(KeyValueQ))
                //{
                //    database.Rollback();
                //    return Content(new JsonMessage { Success = true, Code = "1", Message = "打印成功。" }.ToString());
                //}
                if (string.IsNullOrEmpty(OrderId) && string.IsNullOrEmpty(KeyValueQ))
                {
                    return Content(new JsonMessage { Success = true, Code = "1", Message = "打印状态更改失败。" }.ToString());
                }
                BY_Order by_order = database.FindEntity<BY_Order>(OrderId);
                if (by_order == null || by_order.OrderId == null)
                {
                    by_order = database.FindEntity<BY_Order>("QuotationId",KeyValueQ);
                }
                if (by_order.PrintDate == null && by_order.PrintUserId == null)
                {
                    by_order.OrderStatus = OrderBillStatus.正常生产.ToString();
                    Hashtable by_orderdetail = new Hashtable();
                    by_orderdetail["OrderId"] = OrderId;
                    by_orderdetail["BillStatus"] = by_order.OrderStatus;
                    database.Update("by_orderdetail", by_orderdetail, "OrderId", isOpenTrans);
                }
                by_order.PrintDate = DateTime.Now;
                by_order.PrintUserId = ManageProvider.Provider.Current().UserName;
                database.Update(by_order, isOpenTrans);

                

                BY_OpearLog by_opearlog = new BY_OpearLog();
                by_opearlog.Create();
                by_opearlog.BillId = OrderId;
                by_opearlog.OpearType = by_order.OrderStatus;
                database.Insert(by_opearlog, isOpenTrans);

                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "打印状态更改成功。" }.ToString());
            }
            catch (Exception)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = true, Code = "-1", Message = "打印状态更改失败。" }.ToString());
            }
        }
        #endregion



    }
}
