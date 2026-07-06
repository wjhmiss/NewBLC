using DNS.Business;
using DNS.Business.Utility.Enum;
using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using DNS.WebApp.Areas.OrderModule.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.DeliveryModule.Controllers
{
    /// <summary>
    /// 发运单控制器
    /// </summary>
    public class BY_DeliveryController : PublicController<BY_Delivery>
    {
        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult SubmitForm2(BY_Delivery entity, string KeyValue)
        {
            try
            {
                
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    BY_Delivery Oldentity = repositoryfactory.Repository().FindEntity(KeyValue);//获取没更新之前实体对象
                    if ((!string.IsNullOrEmpty(Oldentity.CheckSheetCode) && Oldentity.CheckSheetCode != "&nbsp;") || Oldentity.GetedAmount >0)
                    {
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：已对账或已付款！" }.ToString());
                    }
                    entity.Modify(KeyValue);
                    IsOk = repositoryfactory.Repository().Update(entity);
                    this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }
                else
                {
                    entity.Create();
                    IsOk = repositoryfactory.Repository().Insert(entity);
                    this.WriteLog(IsOk, entity, null, KeyValue, Message);
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                this.WriteLog(-1, entity, null, KeyValue, "操作失败：" + ex.Message);
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }


        public void DownLoadExcel1(string Condition)
        {
            BY_DeliveryBll by_deliverybll = new Business.BY_DeliveryBll();
            Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
            QueryParameter.Add("WorkCostForWork", 3);
            JqGridParam jqgridparam = new JqGridParam() { page = 1, records = 99999, rows = 99999, sidx = "CustomerCode,DeliveryCode,LineCode" };
            DataTable ListData = by_deliverybll.DaoGetGridPageJson(QueryParameter, ref jqgridparam);
            if (QueryParameter["LType"].ToString() == "RCL")
            {
                string[] DataColumn = { "日期", "助记码", "送货单号", "客户名称", "简称", "产品名称", "数量", "重量", "热处理单价", "合计费用", "备注", "类型", "业务员", "放" };
                string fileName = "送货单明细汇总";
                DeriveExcel.DataTableToExcel(ListData, DataColumn, fileName);
            }
            else
            {
                string[] DataColumn = { "日期", "助记码", "送货单号", "客户名称", "简称", "产品名称", "规格", "数量", "理重", "重量", "材料单价", "材料合计", "加工单价(ks)", "加工合计", "小件单价(件)", "小件合计", "合计费用", "模号", "发运备注", "配件号", "业务员", "放" };
                string fileName = "送货单明细汇总";
                DeriveExcel.DataTableToExcel(ListData, DataColumn, fileName);
            }
            
        }
        public void DownLoadExcel2(string Condition)
        {
            BY_DeliveryBll by_deliverybll = new Business.BY_DeliveryBll();
            Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
            QueryParameter.Add("WorkCostForWork", 3);
            JqGridParam jqgridparam = new JqGridParam() { page = 1, records = 99999, rows = 99999, sidx = "CustomerCode,DeliveryCode,LineCode" };
            DataTable ListData = by_deliverybll.DaoGetGridPageJson(QueryParameter, ref jqgridparam);
            if (QueryParameter["LType"].ToString() == "RCL")
            {
                string[] DataColumn = { "日期", "助记码", "送货单号", "客户名称", "产品名称", "数量", "重量", "热处理单价", "合计费用", "备注", "类型", "业务员", "放" };
                string fileName = "送货单明细汇总";
                DeriveExcel.DataTableToExcel(ListData, DataColumn, fileName);
            }
            else
            {
                string[] DataColumn = {"送货单号", "LineCode", "SteelCode", "Technology", "Length", "cccc", "Weigh", "cccc1", "Thick", "CustomerProduct-ShipDescription", "ProjectCode", "Qty", "Weight" };
                string fileName = "送货单明细汇总";
                DeriveExcel.DataTableToExcel(ListData, DataColumn, fileName);
            }

        }
        //[OutputCache(CacheProfile = "Cache1Hour")]
        //public ActionResult DNSJG()
        //{
        //    return View();
        //}
        //[OutputCache(CacheProfile = "Cache1Hour")]
        //public ActionResult DNSNJG()
        //{
        //    return View();
        //}
        //[OutputCache(CacheProfile = "Cache1Hour")]
        //public ActionResult MDL()
        //{
        //    return View();
        //}
        //[OutputCache(CacheProfile = "Cache1Hour")]
        //public ActionResult SMT()
        //{
        //    return View();
        //}
        //[OutputCache(CacheProfile = "Cache1Hour")]
        //public ActionResult DHW()
        //{
        //    return View();
        //}
        //[OutputCache(CacheProfile = "Cache1Hour")]
        //public ActionResult HX()
        //{
        //    return View();
        //}
        //[OutputCache(CacheProfile = "Cache1Hour")]
        //public ActionResult DF()
        //{
        //    return View();
        //}
        //[OutputCache(CacheProfile = "Cache1Hour")]
        //public ActionResult JMJ()
        //{
        //    return View();
        //}
        //[OutputCache(CacheProfile = "Cache1Hour")]
        //public ActionResult JMJRCL()
        //{
        //    return View();
        //}
        //[OutputCache(CacheProfile = "Cache1Hour")]
        //public ActionResult RPF()
        //{
        //    return View();
        //}
        //[OutputCache(CacheProfile = "Cache1Hour")]
        //public ActionResult SL()
        //{
        //    return View();
        //}
        //[OutputCache(CacheProfile = "Cache1Hour")]
        //public ActionResult MS()
        //{
        //    return View();
        //}
        //[OutputCache(CacheProfile = "Cache1Hour")]
        //public ActionResult QX()
        //{
        //    return View();
        //}
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult LFMB()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult IndexList()
        {
            return View();
        }
        
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult QueryDelivery()
        {
            return View();
        }

        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult LF11()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult LF21()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult LF31()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult LF41()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult LF5()
        {
            return View();
        }
        public ActionResult LF1ForLJ6()
        {
            return View();
        }
        public ActionResult LF1ForLJ()
        {
            return View();
        }
        public ActionResult LF1ForLJ2()
        {
            return View();
        }
        public ActionResult LF1ForLJ3()
        {
            return View();
        }
        public ActionResult LF1ForLJ4()
        {
            return View();
        }
        public ActionResult LF1ForLJ5()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult LF51()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult LF1()
        {
            return View();
        }

        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult LF2()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult LF22()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult LF23()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult LF3()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult LF4()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult YH()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult YHN()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult DHT()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult QueryWeight()
        {
            return View();
        }
        public ActionResult GetChooseType(string customercode, string LType = "")
        {
            IList<ChooseType> choosetypes = new List<ChooseType>();
            //ChooseType choosetype1 = new ChooseType()
            //{
            //    GUID = Guid.NewGuid().ToString(),
            //    Key = "DNSJG",
            //    Value = "代价格",
            //    Remark = "代价格",
            //    SortOrder = 0
            //};
            //ChooseType choosetype2 = new ChooseType()
            //{
            //    GUID = Guid.NewGuid().ToString(),
            //    Key = "DNSNJG",
            //    Value = "不代价格",
            //    Remark = "不代价格",
            //    SortOrder = 1
            //};
            //ChooseType choosetype3 = new ChooseType()
            //{
            //    GUID = Guid.NewGuid().ToString(),
            //    Key = "MDL",
            //    Value = "美德乐格式",
            //    Remark = "美德乐格式",
            //    SortOrder = 10
            //};
            //ChooseType choosetype4 = new ChooseType()
            //{
            //    GUID = Guid.NewGuid().ToString(),
            //    Key = "SMT",
            //    Value = "斯茂特格式",
            //    Remark = "斯茂特格式",
            //    SortOrder = 20
            //};
            //ChooseType choosetype5 = new ChooseType()
            //{
            //    GUID = Guid.NewGuid().ToString(),
            //    Key = "DHW",
            //    Value = "德海威格式",
            //    Remark = "德海威格式",
            //    SortOrder = 30
            //};

            //ChooseType choosetype6 = new ChooseType()
            //{
            //    GUID = Guid.NewGuid().ToString(),
            //    Key = "HX",
            //    Value = "泓旭格式",
            //    Remark = "泓旭格式",
            //    SortOrder = 40
            //};
            //ChooseType choosetype7 = new ChooseType()
            //{
            //    GUID = Guid.NewGuid().ToString(),
            //    Key = "DF",
            //    Value = "大富格式",
            //    Remark = "大富格式",
            //    SortOrder = 50
            //};
            //ChooseType choosetype8 = new ChooseType()
            //{
            //    GUID = Guid.NewGuid().ToString(),
            //    Key = "JMJ",
            //    Value = "金岷江格式",
            //    Remark = "金岷江格式",
            //    SortOrder = 60
            //};
            //ChooseType choosetype10 = new ChooseType()
            //{
            //    GUID = Guid.NewGuid().ToString(),
            //    Key = "JMJRCL",
            //    Value = "金岷江带热处理",
            //    Remark = "金岷江带热处理",
            //    SortOrder = 60
            //};
            //ChooseType choosetype9 = new ChooseType()
            //{
            //    GUID = Guid.NewGuid().ToString(),
            //    Key = "RPF",
            //    Value = "瑞鹏飞格式",
            //    Remark = "瑞鹏飞格式",
            //    SortOrder = 70
            //};
            //ChooseType choosetype11 = new ChooseType()
            //{
            //    GUID = Guid.NewGuid().ToString(),
            //    Key = "SL",
            //    Value = "索菱格式",
            //    Remark = "索菱格式",
            //    SortOrder = 100
            //};
            //ChooseType choosetype12 = new ChooseType()
            //{
            //    GUID = Guid.NewGuid().ToString(),
            //    Key = "MS",
            //    Value = "门市专用",
            //    Remark = "门市专用",
            //    SortOrder = 120
            //};
            //ChooseType choosetype13 = new ChooseType()
            //{
            //    GUID = Guid.NewGuid().ToString(),
            //    Key = "QX",
            //    Value = "群协格式",
            //    Remark = "群协格式",
            //    SortOrder = 130
            //};
            ChooseType choosetype14 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "LF1",
                Value = "材料",
                Remark = "标准",
                SortOrder = 1
            };
            ChooseType choosetype15 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "LF2",
                Value = "无单价有总额",
                Remark = "无单价有总额",
                SortOrder = 2
            };
            ChooseType choosetype16 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "LF3",
                Value = "无单价无总额",
                Remark = "无单价无总额",
                SortOrder = 3
            };
            ChooseType choosetype17 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "LF4",
                Value = "北仑格式4",
                Remark = "北仑格式4",
                SortOrder = 125
            };

            ChooseType choosetype141 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "LF11",
                Value = "超歌格式1",
                Remark = "超歌格式1",
                SortOrder = 120
            };
            ChooseType choosetype151 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "LF21",
                Value = "超歌格式2",
                Remark = "超歌格式2",
                SortOrder = 130
            };
            ChooseType choosetype161 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "LF31",
                Value = "超歌格式3",
                Remark = "超歌格式3",
                SortOrder = 125
            };
            ChooseType choosetype171 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "LF41",
                Value = "超歌格式4",
                Remark = "超歌格式4",
                SortOrder = 125
            };

            ChooseType choosetype165 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "LF5",
                Value = "北仑格式5",
                Remark = "北仑格式5",
                SortOrder = 125
            };
            ChooseType choosetype176 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "LF51",
                Value = "超歌格式5",
                Remark = "超歌格式5",
                SortOrder = 125
            };


            ChooseType choosetype19 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "LFMB",
                Value = "面边格式",
                Remark = "面边格式",
                SortOrder = 126
            };


            ChooseType choosetype20 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "LF22",
                Value = "北仑汇总",
                Remark = "北仑汇总",
                SortOrder = 130
            };
            ChooseType choosetype22 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "LF23",
                Value = "超歌汇总",
                Remark = "超歌汇总",
                SortOrder = 130
            };
            ChooseType choosetype24 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "LF4",
                Value = "热处理",
                Remark = "热处理",
                SortOrder = 130
            };
            ChooseType choosetype25 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "LF1ForLJ",
                Value = "整件",
                Remark = "整件",
                SortOrder = 130
            };
            ChooseType choosetype251 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "LF1ForLJ6",
                Value = "整件2",
                Remark = "整件2",
                SortOrder = 130
            };
            ChooseType choosetype26 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "LF1ForLJ2",
                Value = "含材质",
                Remark = "含材质",
                SortOrder = 130
            };
            ChooseType choosetype27 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "LF1ForLJ3",
                Value = "不含金额",
                Remark = "不含金额",
                SortOrder = 130
            };

            ChooseType choosetype28 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "LF1ForLJ4",
                Value = "不含材质",
                Remark = "不含材质",
                SortOrder = 130
            };
            ChooseType choosetype29 = new ChooseType()
            {
                GUID = Guid.NewGuid().ToString(),
                Key = "LF1ForLJ5",
                Value = "不含材质金额",
                Remark = "不含材质金额",
                SortOrder = 130
            };


            //ChooseType choosetype18 = new ChooseType()
            //{
            //    GUID = Guid.NewGuid().ToString(),
            //    Key = "DHT",
            //    Value = "沿汉带编号",
            //    Remark = "沿汉带编号",
            //    SortOrder = 125
            //};
            //ChooseType choosetype19 = new ChooseType()
            //{
            //    GUID = Guid.NewGuid().ToString(),
            //    Key = "LF3",
            //    Value = "BYD格式",
            //    Remark = "BYD格式",
            //    SortOrder = 150
            //};
            //choosetypes.Add(choosetype1);
            //choosetypes.Add(choosetype2);
            //choosetypes.Add(choosetype3);
            //choosetypes.Add(choosetype4);
            //choosetypes.Add(choosetype5);
            //choosetypes.Add(choosetype6);
            //choosetypes.Add(choosetype7);
            //choosetypes.Add(choosetype8);
            //choosetypes.Add(choosetype9);
            //choosetypes.Add(choosetype10);
            //choosetypes.Add(choosetype11);
            //choosetypes.Add(choosetype12);
            //choosetypes.Add(choosetype13);
            if (LType == "LJ")
            {
                choosetypes.Add(choosetype25);
                choosetypes.Add(choosetype251);
                
                choosetypes.Add(choosetype26);
                choosetypes.Add(choosetype27);
                choosetypes.Add(choosetype28);
                choosetypes.Add(choosetype29);

            }
            else if (LType == "RCL")
            {
                choosetypes.Add(choosetype24);
            }
            else
            {
                choosetypes.Add(choosetype14);
                choosetypes.Add(choosetype15);

            }

            //choosetypes.Add(choosetype16);
            //choosetypes.Add(choosetype17);
            //choosetypes.Add(choosetype165);
            //choosetypes.Add(choosetype141);
            //choosetypes.Add(choosetype151);
            //choosetypes.Add(choosetype161);
            //choosetypes.Add(choosetype171);
            //choosetypes.Add(choosetype176);
            //choosetypes.Add(choosetype18);
            //choosetypes.Add(choosetype19);

            //choosetypes.Add(choosetype20);
            //choosetypes.Add(choosetype22);

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
        /// <summary>
        /// 发货单打印
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult Print()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult ChoosePrint()
        {
            return View();
        }
        public ActionResult PrintDataJsonLJ6(string DeliveryIds, string QuotationId, int num)
        {

            string[] ids = DeliveryIds.Split(',');
            List<BY_DeliveryDetail> deliverydetailList = new List<BY_DeliveryDetail>();
            for (int i = 0; i < ids.Length; i++)
            {
                BY_Delivery temp = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryCode", ids[i]);
                deliverydetailList.AddRange(DataFactory.Database().FindList<BY_DeliveryDetail>("DeliveryId", temp.DeliveryId).OrderBy(p => p.LineCode));
            }

            var grouped = deliverydetailList
               .GroupBy(p => new { p.LJtype1, p.LJtype2, p.LJsize })
               .Select(g => new
               {
                   LJtype1 = g.Key.LJtype1,
                   LJtype2 = g.Key.LJtype2,
                   LJsize = g.Key.LJsize,
                   count = g.ToList().Count,
                   QtySum = g.Sum(x => x.Qty),
                   WeightSum = g.Sum(x => x.Weight),
                   SalesAmountSum = g.Sum(x => x.SalesAmount)
               })
               .ToList();
            deliverydetailList = deliverydetailList.OrderBy(p => p.LJtype1).ThenBy(p => p.LJtype2).ThenBy(p => p.LJsize).ToList();
            List<BY_DeliveryDetail> quotationdetailsaddok = new List<BY_DeliveryDetail>();
            string ljtype12 = "";
            foreach (var item in deliverydetailList)
            {
                if (ljtype12 != item.LJtype1 + item.LJtype2 + item.LJsize)
                {
                    ljtype12 = item.LJtype1 + item.LJtype2 + item.LJsize;
                    BY_DeliveryDetail obj = new BY_DeliveryDetail();
                    obj.LJsize = item.LJsize;
                    obj.LJtype1 = item.LJtype1;
                    obj.LJtype2 = item.LJtype2;
                    obj.listcount = grouped.Where(p => p.LJtype1 == item.LJtype1 && p.LJtype2 == item.LJtype2 && p.LJsize == item.LJsize).FirstOrDefault().count;
                    obj.CustomerProduct = item.CustomerProduct;
                    obj.Qty = grouped.Where(p => p.LJtype1 == item.LJtype1 && p.LJtype2 == item.LJtype2 && p.LJsize == item.LJsize).FirstOrDefault().QtySum;

                    obj.Weight = grouped.Where(p => p.LJtype1 == item.LJtype1 && p.LJtype2 == item.LJtype2 && p.LJsize == item.LJsize).FirstOrDefault().WeightSum;
                    obj.SalesAmount = grouped.Where(p => p.LJtype1 == item.LJtype1 && p.LJtype2 == item.LJtype2 && p.LJsize == item.LJsize).FirstOrDefault().SalesAmountSum;
                    quotationdetailsaddok.Add(obj);
                }
            }

            BY_Delivery by_delivery = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryCode", ids[0]);
            Base_CustomerProcess base_customerprocess = DataFactory.Database().FindEntityBySql<Base_CustomerProcess>(string.Format(@"SELECT *
  FROM Base_CustomerProcess WHERE CustomerId='{0}'
  AND WhereQty > 0", by_delivery.CustomerId));

            Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>("Code", by_delivery.CustomerCode);

            Base_CustomerAddress base_customeraddress = DataFactory.Database().FindEntity<Base_CustomerAddress>("CustomerId", by_delivery.CustomerId);
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>("QuotationCode", string.IsNullOrEmpty(by_delivery.QuotationCode) ? "" : by_delivery.QuotationCode);


            List<BY_DeliveryDetail> deliverydetailListAdd = new List<BY_DeliveryDetail>();

            bool temo = false;
            BY_DeliveryDetail by_deliverydetail;

            decimal? xiaojiqty = 0;
            decimal? xiaojiwiehgt = 0;
            decimal? xiaojisalesamount = 0;
            decimal? workamount = 0;



            for (int i = 0; i < quotationdetailsaddok.Count; i++)
            {
                deliverydetailListAdd.Add(quotationdetailsaddok[i]);
                xiaojiqty += quotationdetailsaddok[i].Qty;
                xiaojiwiehgt += quotationdetailsaddok[i].Weight;
                xiaojisalesamount += quotationdetailsaddok[i].SalesAmount;
                workamount += quotationdetailsaddok[i].WorkAmount;
                if ((i + 1) % (num - 1) == 0)
                {
                    by_deliverydetail = new BY_DeliveryDetail();
                    deliverydetailListAdd.Add(by_deliverydetail);
                    temo = true;
                }
                if (temo)
                {
                    deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());
                    by_deliverydetail = new BY_DeliveryDetail();
                    by_deliverydetail.LineCode = 0;
                    by_deliverydetail.ProductName = "小计:";
                    by_deliverydetail.SteelCode = "小计:";
                    by_deliverydetail.WorkAmount = workamount;
                    by_deliverydetail.Qty = xiaojiqty;
                    by_deliverydetail.Weight = xiaojiwiehgt;
                    by_deliverydetail.SalesAmount = xiaojisalesamount;
                    by_deliverydetail.Length = 0;
                    by_deliverydetail.Weigh = 0;
                    by_deliverydetail.Thick = 0;
                    by_deliverydetail.JJtype = "按公斤";
                    deliverydetailListAdd.Add(by_deliverydetail);
                    xiaojiqty = 0;
                    xiaojiwiehgt = 0;
                    xiaojisalesamount = 0;
                    workamount = 0;
                    temo = false;
                }
            }

            bool temo2 = false;

            //if (deliverydetailListAdd.Count % num > 0)
            //{
            //    int j = (num - deliverydetailListAdd.Count % num);
            //    for (int i = 0; i < j; i++)
            //    {
            //        by_deliverydetail = new BY_DeliveryDetail();
            //        deliverydetailListAdd.Add(by_deliverydetail);
            //        temo2 = true;
            //    }
            //}

            if (temo2)
            {
                deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());
            }

            if (deliverydetailListAdd.Count % num == 0 && deliverydetailListAdd.LastOrDefault().LineCode == null)
            {
                deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());

            }
            if (deliverydetailListAdd.LastOrDefault().ProductName == null || (deliverydetailListAdd.LastOrDefault().ProductName != null && deliverydetailListAdd.LastOrDefault().ProductName.IndexOf("小计") < 0))
            {
                by_deliverydetail = new BY_DeliveryDetail();
                by_deliverydetail.LineCode = 0;
                by_deliverydetail.ProductName = "合计:";
                by_deliverydetail.SteelCode = "合计:";
                by_deliverydetail.Qty = quotationdetailsaddok.Sum<BY_DeliveryDetail>(m => m.Qty);
                by_deliverydetail.Weight = quotationdetailsaddok.Sum<BY_DeliveryDetail>(m => (base_customer.Calc == 0 || m.OrderType == "毛料生产") ? m.Weight : m.TheoryWeight);
                //by_deliverydetail.Weight = deliverydetailList.Where(p => p.SteelCode != "FY" && p.Thick > 0).Sum<BY_DeliveryDetail>(m => m.Weight);
                by_deliverydetail.SalesAmount = quotationdetailsaddok.Sum<BY_DeliveryDetail>(m => m.SalesAmount);
                by_deliverydetail.WorkAmount = quotationdetailsaddok.Sum<BY_DeliveryDetail>(m => m.WorkAmount + m.SmallAmount);
                by_deliverydetail.Length = 0;
                by_deliverydetail.Weigh = 0;
                by_deliverydetail.Thick = 0;
                by_deliverydetail.JJtype = "按件";
                //deliverydetailListAdd.Add(by_deliverydetail);
            }

            for (int i = 0; i < deliverydetailListAdd.Count; i++)
            {
                if (deliverydetailListAdd[i].OrderDetailId != null && by_delivery.LType == "RCL")
                {
                    deliverydetailListAdd[i].ProductName = DataFactory.Database().FindEntity<BY_QuotationHeat>("QuotationHeatCode", deliverydetailListAdd[i].OrderDetailId)?.TechnologQuery;
                }
            }
            Base_Company company = DataFactory.Database().FindEntity<Base_Company>("FullName", string.IsNullOrEmpty(by_delivery.CompanyName) ? "" : by_delivery.CompanyName);

            var JsonData = new
            {
                Delivery = by_delivery,
                BY_Quotation = by_quotation,
                DeliveryDetail = deliverydetailListAdd,
                QtySum = grouped.Sum(m => m.QtySum),//总重量
                WeightSum = grouped.Sum(m => m.WeightSum),//总重量
                MaterialAmountSum = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount),//总金额
                UpperMaterialAmountSum = RMBHelper.CmycurD(CommonHelper.GetDecimal(deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount))),//大写总金额
                DeliveryCodes = DeliveryIds,
                base_customer = base_customer,
                base_customeraddress = base_customeraddress,
                hang = deliverydetailList.Count,
                company = company
            };
            return Content(JsonData.ToJson());

        }
        /// <summary>
        /// 获取发货单打印数据（返回订单信息）
        /// </summary>
        /// <param name="DeliveryId"></param>
        /// <returns></returns>
        public ActionResult PrintDataJsonLJ(string DeliveryIds, string QuotationId, int num)
        {

            string[] ids = DeliveryIds.Split(',');
            List<BY_DeliveryDetail> deliverydetailList = new List<BY_DeliveryDetail>();
            for (int i = 0; i < ids.Length; i++)
            {
                BY_Delivery temp = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryCode", ids[i]);
                deliverydetailList.AddRange(DataFactory.Database().FindList<BY_DeliveryDetail>("DeliveryId", temp.DeliveryId).OrderBy(p => p.LineCode));
            }

            var grouped = deliverydetailList
               .GroupBy(p => new { p.LJtype1, p.LJtype2, p.LJsize })
               .Select(g => new
               {
                   LJtype1 = g.Key.LJtype1,
                   LJtype2 = g.Key.LJtype2,
                   LJsize = g.Key.LJsize,
                   count = g.ToList().Count,
                   QtySum = g.Sum(x => x.Qty),
                   WeightSum = g.Sum(x => x.Weight),
                   SalesAmountSum = g.Sum(x => x.SalesAmount)
               })
               .ToList();
            deliverydetailList = deliverydetailList.OrderBy(p => p.LJtype1).ThenBy(p => p.LJtype2).ThenBy(p => p.LJsize).ToList();
            List<BY_DeliveryDetail> quotationdetailsaddok = new List<BY_DeliveryDetail>();
            string ljtype12 = "";
            foreach (var item in deliverydetailList)
            {
                if (ljtype12 != item.LJtype1 + item.LJtype2 + item.LJsize)
                {
                    ljtype12 = item.LJtype1 + item.LJtype2 + item.LJsize;
                    BY_DeliveryDetail obj = new BY_DeliveryDetail();
                    obj.LJsize = item.LJsize;
                    obj.LJtype1 = item.LJtype1;
                    obj.LJtype2 = item.LJtype2;
                    obj.listcount = grouped.Where(p => p.LJtype1 == item.LJtype1 && p.LJtype2 == item.LJtype2 && p.LJsize == item.LJsize).FirstOrDefault().count;
                    obj.CustomerProduct = item.CustomerProduct;
                    obj.Qty = 1;

                    obj.Weight = grouped.Where(p => p.LJtype1 == item.LJtype1 && p.LJtype2 == item.LJtype2 && p.LJsize == item.LJsize).FirstOrDefault().WeightSum;
                    obj.SalesAmount = grouped.Where(p => p.LJtype1 == item.LJtype1 && p.LJtype2 == item.LJtype2 && p.LJsize == item.LJsize).FirstOrDefault().SalesAmountSum;
                    quotationdetailsaddok.Add(obj);
                }
            }

            BY_Delivery by_delivery = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryCode", ids[0]);
            Base_CustomerProcess base_customerprocess = DataFactory.Database().FindEntityBySql<Base_CustomerProcess>(string.Format(@"SELECT *
  FROM Base_CustomerProcess WHERE CustomerId='{0}'
  AND WhereQty > 0", by_delivery.CustomerId));

            Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>("Code", by_delivery.CustomerCode);

            Base_CustomerAddress base_customeraddress = DataFactory.Database().FindEntity<Base_CustomerAddress>("CustomerId", by_delivery.CustomerId);
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>("QuotationCode", string.IsNullOrEmpty(by_delivery.QuotationCode) ? "" : by_delivery.QuotationCode);


            List<BY_DeliveryDetail> deliverydetailListAdd = new List<BY_DeliveryDetail>();

            bool temo = false;
            BY_DeliveryDetail by_deliverydetail;

            decimal? xiaojiqty = 0;
            decimal? xiaojiwiehgt = 0;
            decimal? xiaojisalesamount = 0;
            decimal? workamount = 0;

            

            for (int i = 0; i < quotationdetailsaddok.Count; i++)
            {
                deliverydetailListAdd.Add(quotationdetailsaddok[i]);
                xiaojiqty += quotationdetailsaddok[i].Qty;
                xiaojiwiehgt += quotationdetailsaddok[i].Weight;
                xiaojisalesamount += quotationdetailsaddok[i].SalesAmount;
                workamount += quotationdetailsaddok[i].WorkAmount;
                if ((i + 1) % (num - 1) == 0)
                {
                    by_deliverydetail = new BY_DeliveryDetail();
                    deliverydetailListAdd.Add(by_deliverydetail);
                    temo = true;
                }
                if (temo)
                {
                    deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());
                    by_deliverydetail = new BY_DeliveryDetail();
                    by_deliverydetail.LineCode = 0;
                    by_deliverydetail.ProductName = "小计:";
                    by_deliverydetail.SteelCode = "小计:";
                    by_deliverydetail.WorkAmount = workamount;
                    by_deliverydetail.Qty = xiaojiqty;
                    by_deliverydetail.Weight = xiaojiwiehgt;
                    by_deliverydetail.SalesAmount = xiaojisalesamount;
                    by_deliverydetail.Length = 0;
                    by_deliverydetail.Weigh = 0;
                    by_deliverydetail.Thick = 0;
                    by_deliverydetail.JJtype = "按公斤";
                    deliverydetailListAdd.Add(by_deliverydetail);
                    xiaojiqty = 0;
                    xiaojiwiehgt = 0;
                    xiaojisalesamount = 0;
                    workamount = 0;
                    temo = false;
                }
            }

            bool temo2 = false;

            //if (deliverydetailListAdd.Count % num > 0)
            //{
            //    int j = (num - deliverydetailListAdd.Count % num);
            //    for (int i = 0; i < j; i++)
            //    {
            //        by_deliverydetail = new BY_DeliveryDetail();
            //        deliverydetailListAdd.Add(by_deliverydetail);
            //        temo2 = true;
            //    }
            //}

            if (temo2)
            {
                deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());
            }

            if (deliverydetailListAdd.Count % num == 0 && deliverydetailListAdd.LastOrDefault().LineCode == null)
            {
                deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());

            }
            if (deliverydetailListAdd.LastOrDefault().ProductName == null || (deliverydetailListAdd.LastOrDefault().ProductName != null && deliverydetailListAdd.LastOrDefault().ProductName.IndexOf("小计") < 0))
            {
                by_deliverydetail = new BY_DeliveryDetail();
                by_deliverydetail.LineCode = 0;
                by_deliverydetail.ProductName = "合计:";
                by_deliverydetail.SteelCode = "合计:";
                by_deliverydetail.Qty = quotationdetailsaddok.Sum<BY_DeliveryDetail>(m => m.Qty);
                by_deliverydetail.Weight = quotationdetailsaddok.Sum<BY_DeliveryDetail>(m => (base_customer.Calc == 0 || m.OrderType == "毛料生产") ? m.Weight : m.TheoryWeight);
                //by_deliverydetail.Weight = deliverydetailList.Where(p => p.SteelCode != "FY" && p.Thick > 0).Sum<BY_DeliveryDetail>(m => m.Weight);
                by_deliverydetail.SalesAmount = quotationdetailsaddok.Sum<BY_DeliveryDetail>(m => m.SalesAmount);
                by_deliverydetail.WorkAmount = quotationdetailsaddok.Sum<BY_DeliveryDetail>(m => m.WorkAmount + m.SmallAmount);
                by_deliverydetail.Length = 0;
                by_deliverydetail.Weigh = 0;
                by_deliverydetail.Thick = 0;
                by_deliverydetail.JJtype = "按件";
                //deliverydetailListAdd.Add(by_deliverydetail);
            }

            for (int i = 0; i < deliverydetailListAdd.Count; i++)
            {
                if (deliverydetailListAdd[i].OrderDetailId != null && by_delivery.LType == "RCL")
                {
                    deliverydetailListAdd[i].ProductName = DataFactory.Database().FindEntity<BY_QuotationHeat>("QuotationHeatCode", deliverydetailListAdd[i].OrderDetailId)?.TechnologQuery;
                }
            }
            Base_Company company = DataFactory.Database().FindEntity<Base_Company>("FullName", string.IsNullOrEmpty(by_delivery.CompanyName) ? "" : by_delivery.CompanyName);

            var JsonData = new
            {
                Delivery = by_delivery,
                BY_Quotation = by_quotation,
                DeliveryDetail = deliverydetailListAdd,
                QtySum = grouped.Count,//总数量
                WeightSum = grouped.Sum(m => m.WeightSum),//总重量
                MaterialAmountSum = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount),//总金额
                UpperMaterialAmountSum = RMBHelper.CmycurD(CommonHelper.GetDecimal(deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount))),//大写总金额
                DeliveryCodes = DeliveryIds,
                base_customer = base_customer,
                base_customeraddress = base_customeraddress,
                hang = deliverydetailList.Count,
                company = company
            };
            return Content(JsonData.ToJson());

        }

        public ActionResult PrintDataJsonLJ2(string DeliveryIds, string QuotationId, int num)
        {

            string[] ids = DeliveryIds.Split(',');
            List<BY_DeliveryDetail> deliverydetailList = new List<BY_DeliveryDetail>();
            for (int i = 0; i < ids.Length; i++)
            {
                BY_Delivery temp = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryCode", ids[i]);
                deliverydetailList.AddRange(DataFactory.Database().FindList<BY_DeliveryDetail>("DeliveryId", temp.DeliveryId).OrderBy(p => p.LineCode));
            }

            var grouped = deliverydetailList
               .GroupBy(p => new { p.LJtype1, p.LJtype2, p.LJsize })
               .Select(g => new
               {
                   LJtype1 = g.Key.LJtype1,
                   LJtype2 = g.Key.LJtype2,
                   LJsize = g.Key.LJsize,
                   count = g.ToList().Count,
                   QtySum = g.Sum(x => x.Qty),
                   WeightSum = g.Sum(x => x.Weight),
                   SalesAmountSum = g.Sum(x => x.SalesAmount)
               })
               .ToList();
            deliverydetailList = deliverydetailList.OrderBy(p => p.LJtype1).ThenBy(p => p.LJtype2).ThenBy(p => p.LJsize).ToList();
            List<BY_DeliveryDetail> quotationdetailsaddok = new List<BY_DeliveryDetail>();
            string ljtype12 = "";
            foreach (var item in deliverydetailList)
            {
                if (ljtype12 != item.LJtype1 + item.LJtype2 + item.LJsize)
                {
                    ljtype12 = item.LJtype1 + item.LJtype2 + item.LJsize;
                    BY_DeliveryDetail obj = new BY_DeliveryDetail();
                    obj.LJsize = item.LJsize;
                    obj.LJtype1 = item.LJtype1;
                    obj.LJtype2 = item.LJtype2;
                    obj.listcount = grouped.Where(p => p.LJtype1 == item.LJtype1 && p.LJtype2 == item.LJtype2 && p.LJsize == item.LJsize).FirstOrDefault().count;
                    obj.CustomerProduct = item.CustomerProduct;
                    obj.Qty = 1;
                    obj.Weight = grouped.Where(p => p.LJtype1 == item.LJtype1 && p.LJtype2 == item.LJtype2 && p.LJsize == item.LJsize).FirstOrDefault().WeightSum;
                    obj.SalesAmount = grouped.Where(p => p.LJtype1 == item.LJtype1 && p.LJtype2 == item.LJtype2 && p.LJsize == item.LJsize).FirstOrDefault().SalesAmountSum;
                    quotationdetailsaddok.Add(obj);
                }
                quotationdetailsaddok.Add(item);
            }


            BY_Delivery by_delivery = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryCode", ids[0]);
            Base_CustomerProcess base_customerprocess = DataFactory.Database().FindEntityBySql<Base_CustomerProcess>(string.Format(@"SELECT *
  FROM Base_CustomerProcess WHERE CustomerId='{0}'
  AND WhereQty > 0", by_delivery.CustomerId));

            Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>("Code", by_delivery.CustomerCode);

            Base_CustomerAddress base_customeraddress = DataFactory.Database().FindEntity<Base_CustomerAddress>("CustomerId", by_delivery.CustomerId);
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>("QuotationCode", string.IsNullOrEmpty(by_delivery.QuotationCode) ? "" : by_delivery.QuotationCode);


            List<BY_DeliveryDetail> deliverydetailListAdd = new List<BY_DeliveryDetail>();

            bool temo = false;
            BY_DeliveryDetail by_deliverydetail;

            decimal? xiaojiqty = 0;
            decimal? xiaojiwiehgt = 0;
            decimal? xiaojisalesamount = 0;
            decimal? workamount = 0;



            for (int i = 0; i < quotationdetailsaddok.Count; i++)
            {
                deliverydetailListAdd.Add(quotationdetailsaddok[i]);
                xiaojiqty += quotationdetailsaddok[i].LineCode > 0 ? quotationdetailsaddok[i].Qty : 0;
                xiaojiwiehgt += quotationdetailsaddok[i].LineCode > 0 ? quotationdetailsaddok[i].Weight : 0;
                xiaojisalesamount += quotationdetailsaddok[i].LineCode > 0 ? quotationdetailsaddok[i].SalesAmount : 0;
                workamount += quotationdetailsaddok[i].LineCode > 0 ? quotationdetailsaddok[i].WorkAmount : 0;
                if ((i + 1) % (num - 1) == 0)
                {
                    by_deliverydetail = new BY_DeliveryDetail();
                    deliverydetailListAdd.Add(by_deliverydetail);
                    temo = true;
                }
                if (temo)
                {
                    deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());
                    by_deliverydetail = new BY_DeliveryDetail();
                    by_deliverydetail.LineCode = 0;
                    by_deliverydetail.ProductName = "小计:";
                    by_deliverydetail.SteelCode = "小计:";
                    by_deliverydetail.WorkAmount = workamount;
                    by_deliverydetail.Qty = xiaojiqty;
                    by_deliverydetail.Weight = xiaojiwiehgt;
                    by_deliverydetail.SalesAmount = xiaojisalesamount;
                    by_deliverydetail.Length = 0;
                    by_deliverydetail.Weigh = 0;
                    by_deliverydetail.Thick = 0;
                    by_deliverydetail.JJtype = "按公斤";
                    //deliverydetailListAdd.Add(by_deliverydetail);//取消小计
                    xiaojiqty = 0;
                    xiaojiwiehgt = 0;
                    xiaojisalesamount = 0;
                    workamount = 0;
                    temo = false;
                }
            }

            bool temo2 = false;

            //if (deliverydetailListAdd.Count % num > 0)
            //{
            //    int j = (num - deliverydetailListAdd.Count % num);
            //    for (int i = 0; i < j; i++)
            //    {
            //        by_deliverydetail = new BY_DeliveryDetail();
            //        deliverydetailListAdd.Add(by_deliverydetail);
            //        temo2 = true;
            //    }
            //}

            if (temo2)
            {
                deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());
            }

            if (deliverydetailListAdd.Count % num == 0 && deliverydetailListAdd.LastOrDefault().LineCode == null)
            {
                deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());

            }
            if (deliverydetailListAdd.LastOrDefault().ProductName == null || (deliverydetailListAdd.LastOrDefault().ProductName != null && deliverydetailListAdd.LastOrDefault().ProductName.IndexOf("小计") < 0))
            {
                by_deliverydetail = new BY_DeliveryDetail();
                by_deliverydetail.LineCode = 0;
                by_deliverydetail.ProductName = "合计:";
                by_deliverydetail.SteelCode = "合计:";
                by_deliverydetail.Qty = grouped.Count;//deliverydetailList.Sum<BY_DeliveryDetail>(m => m.Qty);
                by_deliverydetail.Weight = deliverydetailList.Sum<BY_DeliveryDetail>(m => (base_customer.Calc == 0 || m.OrderType == "毛料生产") ? m.Weight : m.TheoryWeight);
                //by_deliverydetail.Weight = deliverydetailList.Where(p => p.SteelCode != "FY" && p.Thick > 0).Sum<BY_DeliveryDetail>(m => m.Weight);
                by_deliverydetail.SalesAmount = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount);
                by_deliverydetail.WorkAmount = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.WorkAmount + m.SmallAmount);
                by_deliverydetail.Length = 0;
                by_deliverydetail.Weigh = 0;
                by_deliverydetail.Thick = 0;
                by_deliverydetail.JJtype = "按件";
                //deliverydetailListAdd.Add(by_deliverydetail); //取消合计

            }

            for (int i = 0; i < deliverydetailListAdd.Count; i++)
            {
                if (deliverydetailListAdd[i].OrderDetailId != null && by_delivery.LType == "RCL")
                {
                    deliverydetailListAdd[i].ProductName = DataFactory.Database().FindEntity<BY_QuotationHeat>("QuotationHeatCode", deliverydetailListAdd[i].OrderDetailId)?.TechnologQuery;
                }
            }
            Base_Company company = DataFactory.Database().FindEntity<Base_Company>("FullName", string.IsNullOrEmpty(by_delivery.CompanyName) ? "" : by_delivery.CompanyName);

            var JsonData = new
            {
                Delivery = by_delivery,
                BY_Quotation = by_quotation,
                DeliveryDetail = deliverydetailListAdd,
                QtySum = grouped.Count, //deliverydetailList.Where(p => p.SteelCode != "FY").Sum<BY_DeliveryDetail>(m => m.Qty),//总数量
                WeightSum = deliverydetailList.Where(p => p.SteelCode != "FY").Sum<BY_DeliveryDetail>(m => m.Weight),//总重量
                MaterialAmountSum = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount),//总金额
                UpperMaterialAmountSum = RMBHelper.CmycurD(CommonHelper.GetDecimal(deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount))),//大写总金额
                DeliveryCodes = DeliveryIds,
                base_customer = base_customer,
                base_customeraddress = base_customeraddress,
                hang = deliverydetailList.Count,
                company = company
            };
            return Content(JsonData.ToJson());

        }

        /// <summary>
        /// 获取发货单打印数据（返回订单信息）
        /// </summary>
        /// <param name="DeliveryId"></param>
        /// <returns></returns>
        public ActionResult PrintDataJson(string DeliveryIds, string QuotationId, int num)
        {

            string[] ids = DeliveryIds.Split(',');
            List<BY_DeliveryDetail> deliverydetailList = new List<BY_DeliveryDetail>();
            for (int i = 0; i < ids.Length; i++)
            {
                BY_Delivery temp = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryCode", ids[i]);
                deliverydetailList.AddRange(DataFactory.Database().FindList<BY_DeliveryDetail>("DeliveryId", temp.DeliveryId).OrderBy(p => p.LineCode));
            }
            BY_Delivery by_delivery = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryCode", ids[0]);
            Base_CustomerProcess base_customerprocess = DataFactory.Database().FindEntityBySql<Base_CustomerProcess>(string.Format(@"SELECT *
  FROM Base_CustomerProcess WHERE CustomerId='{0}'
  AND WhereQty > 0", by_delivery.CustomerId));
            
            Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>("Code", by_delivery.CustomerCode);

            Base_CustomerAddress base_customeraddress = DataFactory.Database().FindEntity<Base_CustomerAddress>("CustomerId", by_delivery.CustomerId);
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>("QuotationCode", string.IsNullOrEmpty(by_delivery.QuotationCode) ? "" : by_delivery.QuotationCode);


            List<BY_DeliveryDetail> deliverydetailListAdd = new List<BY_DeliveryDetail>();

            bool temo = false;
            BY_DeliveryDetail by_deliverydetail;

            decimal? xiaojiqty = 0;
            decimal? xiaojiwiehgt = 0;
            decimal? xiaojisalesamount = 0;
            decimal? workamount = 0;

            if (by_delivery.LType == "RCL")
            {
                deliverydetailList = deliverydetailList.OrderBy(P => P.Technology).ToList();
            }

            for (int i = 0; i < deliverydetailList.Count; i++)
            {
                deliverydetailListAdd.Add(deliverydetailList[i]);
                xiaojiqty += deliverydetailList[i].Qty;
                xiaojiwiehgt += (base_customer.Calc == 0 || deliverydetailList[i].OrderType == "毛料生产")? deliverydetailList[i].Weight : deliverydetailList[i].TheoryWeight;
                xiaojisalesamount += deliverydetailList[i].SalesAmount;
                workamount += deliverydetailList[i].WorkAmount;
                if ((i + 1) % (num-1) == 0)
                {
                    by_deliverydetail = new BY_DeliveryDetail();
                    deliverydetailListAdd.Add(by_deliverydetail);
                    temo = true;
                }
                if (temo)
                {
                    deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());
                    by_deliverydetail = new BY_DeliveryDetail();
                    by_deliverydetail.LineCode = 0;
                    by_deliverydetail.ProductName = "小计:";
                    by_deliverydetail.SteelCode = "小计:";
                    by_deliverydetail.WorkAmount = workamount;
                    by_deliverydetail.Qty = xiaojiqty;
                    by_deliverydetail.Weight = xiaojiwiehgt;
                    by_deliverydetail.SalesAmount = xiaojisalesamount;
                    by_deliverydetail.Length = 0;
                    by_deliverydetail.Weigh = 0;
                    by_deliverydetail.Thick = 0;
                    by_deliverydetail.JJtype = "按公斤";
                    deliverydetailListAdd.Add(by_deliverydetail);
                    xiaojiqty = 0;
                    xiaojiwiehgt = 0;
                    xiaojisalesamount = 0;
                    workamount = 0;
                    temo = false;
                }
            }

            bool temo2 = false;

            //if (deliverydetailListAdd.Count % num > 0)
            //{
            //    int j = (num - deliverydetailListAdd.Count % num);
            //    for (int i = 0; i < j; i++)
            //    {
            //        by_deliverydetail = new BY_DeliveryDetail();
            //        deliverydetailListAdd.Add(by_deliverydetail);
            //        temo2 = true;
            //    }
            //}

            if (temo2)
            {
                deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());
            }

            if (deliverydetailListAdd.Count % num == 0 && deliverydetailListAdd.LastOrDefault().LineCode == null)
            {
                deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());

            }
            if (deliverydetailListAdd.LastOrDefault().ProductName == null || (deliverydetailListAdd.LastOrDefault().ProductName != null && deliverydetailListAdd.LastOrDefault().ProductName.IndexOf("小计") < 0))
            {
                by_deliverydetail = new BY_DeliveryDetail();
                by_deliverydetail.LineCode = 0;
                by_deliverydetail.ProductName = "合计:";
                by_deliverydetail.SteelCode = "合计:";
                by_deliverydetail.Qty = deliverydetailList.Where(p => p.SteelCode != "FY").Sum<BY_DeliveryDetail>(m => m.Qty);
                by_deliverydetail.Weight = deliverydetailList.Where(p => p.SteelCode != "FY").Sum<BY_DeliveryDetail>(m => (base_customer.Calc == 0 || m.OrderType == "毛料生产") ? m.Weight : m.TheoryWeight);
                //by_deliverydetail.Weight = deliverydetailList.Where(p => p.SteelCode != "FY" && p.Thick > 0).Sum<BY_DeliveryDetail>(m => m.Weight);
                by_deliverydetail.SalesAmount = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount);
                by_deliverydetail.WorkAmount = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.WorkAmount + m.SmallAmount);
                by_deliverydetail.Length = 0;
                by_deliverydetail.Weigh = 0;
                by_deliverydetail.Thick = 0;
                by_deliverydetail.JJtype = "按公斤";
                //deliverydetailListAdd.Add(by_deliverydetail);
            }

            for (int i = 0; i < deliverydetailListAdd.Count; i++)
            {
                if (deliverydetailListAdd[i].OrderDetailId != null && by_delivery.LType == "RCL")
                {
                    deliverydetailListAdd[i].ProductName = DataFactory.Database().FindEntity<BY_QuotationHeat>("QuotationHeatCode", deliverydetailListAdd[i].OrderDetailId)?.TechnologQuery;
                }
            }
            Base_Company company = DataFactory.Database().FindEntity<Base_Company>("FullName", string.IsNullOrEmpty(by_delivery.CompanyName) ? "" : by_delivery.CompanyName);

            var JsonData = new
            {
                Delivery = by_delivery,
                BY_Quotation = by_quotation,
                DeliveryDetail = deliverydetailListAdd,
                QtySum = deliverydetailList.Where(p=>p.SteelCode != "FY").Sum<BY_DeliveryDetail>(m => m.Qty),//总数量
                WeightSum = deliverydetailList.Where(p => p.SteelCode != "FY").Sum<BY_DeliveryDetail>(m => m.Weight),//总重量
                MaterialAmountSum = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount),//总金额
                UpperMaterialAmountSum = RMBHelper.CmycurD(CommonHelper.GetDecimal(deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount))),//大写总金额
                DeliveryCodes = DeliveryIds,
                base_customer = base_customer,
                base_customeraddress = base_customeraddress,
                hang = deliverydetailList.Count,
                company = company
            };
            return Content(JsonData.ToJson());

        }

        public ActionResult PrintDataJson2(string DeliveryIds, string QuotationId, int num)
        {

            string[] ids = DeliveryIds.Split(',');
            List<BY_DeliveryDetail> deliverydetailList = new List<BY_DeliveryDetail>();
            for (int i = 0; i < ids.Length; i++)
            {
                BY_Delivery temp = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryCode", ids[i]);
                List<BY_DeliveryDetail> dds = DataFactory.Database().FindList<BY_DeliveryDetail>("DeliveryId", temp.DeliveryId).OrderBy(p => p.LineCode).ToList();
                deliverydetailList.Add(new BY_DeliveryDetail() { 
                LineCode = i+1,
                Qty = dds.Where(p => p.Length > 0).Sum<BY_DeliveryDetail>(m => m.Qty),
                    SalesAmount = dds.Sum<BY_DeliveryDetail>(m => m.SalesAmount),
                    Weight = dds.Where(p => p.Length > 0).Sum<BY_DeliveryDetail>(m => m.Weight),
                    SteelCode = dds.First().SteelCode,
                    ProductName = dds.First().ProductName,
                });
            }






            BY_Delivery by_delivery = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryCode", ids[0]);
            Base_CustomerProcess base_customerprocess = DataFactory.Database().FindEntityBySql<Base_CustomerProcess>(string.Format(@"SELECT *
  FROM Base_CustomerProcess WHERE CustomerId='{0}'
  AND WhereQty > 0", by_delivery.CustomerId));

            Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>("Code", by_delivery.CustomerCode);

            Base_CustomerAddress base_customeraddress = DataFactory.Database().FindEntity<Base_CustomerAddress>("CustomerId", by_delivery.CustomerId);
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>("QuotationCode", string.IsNullOrEmpty(by_delivery.QuotationCode) ? "" : by_delivery.QuotationCode);


            List<BY_DeliveryDetail> deliverydetailListAdd = new List<BY_DeliveryDetail>();

            bool temo = false;
            BY_DeliveryDetail by_deliverydetail;

            decimal? xiaojiqty = 0;
            decimal? xiaojiwiehgt = 0;
            decimal? xiaojisalesamount = 0;
            decimal? workamount = 0;

            if (by_delivery.LType == "RCL")
            {
                deliverydetailList = deliverydetailList.OrderBy(P => P.Technology).ToList();
            }

            for (int i = 0; i < deliverydetailList.Count; i++)
            {
                deliverydetailListAdd.Add(deliverydetailList[i]);
                xiaojiqty += deliverydetailList[i].Qty;
                xiaojiwiehgt += (base_customer.Calc == 0 || deliverydetailList[i].OrderType == "毛料生产") ? deliverydetailList[i].Weight : deliverydetailList[i].TheoryWeight;
                xiaojisalesamount += deliverydetailList[i].SalesAmount;
                workamount += deliverydetailList[i].WorkAmount;
                if ((i + 1) % (num - 1) == 0)
                {
                    by_deliverydetail = new BY_DeliveryDetail();
                    deliverydetailListAdd.Add(by_deliverydetail);
                    temo = true;
                }
                if (temo)
                {
                    deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());
                    by_deliverydetail = new BY_DeliveryDetail();
                    by_deliverydetail.LineCode = 0;
                    by_deliverydetail.ProductName = "小计:";
                    by_deliverydetail.SteelCode = "小计:";
                    by_deliverydetail.WorkAmount = workamount;
                    by_deliverydetail.Qty = xiaojiqty;
                    by_deliverydetail.Weight = xiaojiwiehgt;
                    by_deliverydetail.SalesAmount = xiaojisalesamount;
                    by_deliverydetail.Length = 0;
                    by_deliverydetail.Weigh = 0;
                    by_deliverydetail.Thick = 0;
                    by_deliverydetail.JJtype = "按公斤";
                    deliverydetailListAdd.Add(by_deliverydetail);
                    xiaojiqty = 0;
                    xiaojiwiehgt = 0;
                    xiaojisalesamount = 0;
                    workamount = 0;
                    temo = false;
                }
            }

            bool temo2 = false;

            if (deliverydetailListAdd.Count % num > 0)
            {
                int j = (num - deliverydetailListAdd.Count % num);
                for (int i = 0; i < j; i++)
                {
                    by_deliverydetail = new BY_DeliveryDetail();
                    deliverydetailListAdd.Add(by_deliverydetail);
                    temo2 = true;
                }

            }

            if (temo2)
            {
                deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());
            }

            if (deliverydetailListAdd.Count % num == 0 && deliverydetailListAdd.LastOrDefault().LineCode == null)
            {
                deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());

            }
            if (deliverydetailListAdd.LastOrDefault().ProductName == null || (deliverydetailListAdd.LastOrDefault().ProductName != null && deliverydetailListAdd.LastOrDefault().ProductName.IndexOf("小计") < 0))
            {
                //if (deliverydetailList.Count >= num)
                //{
                //    by_deliverydetail = new BY_DeliveryDetail();
                //    by_deliverydetail.LineCode = 0;
                //    by_deliverydetail.ProductName = "小计:";
                //    by_deliverydetail.SteelCode = "小计:";
                //    by_deliverydetail.Qty = deliverydetailList.Where(p => p.SteelCode != "FY" ).Sum<BY_DeliveryDetail>(m => m.Qty) - deliverydetailListAdd.Where(p => p.ProductName == "小计:").Sum<BY_DeliveryDetail>(m => m.Qty);
                //    by_deliverydetail.Weight = deliverydetailList.Where(p => p.SteelCode != "FY" ).Sum<BY_DeliveryDetail>(m => (base_customer.Calc == 0 || m.OrderType == "毛料生产") ? m.Weight : m.TheoryWeight) - deliverydetailListAdd.Where(p => p.ProductName == "小计:").Sum<BY_DeliveryDetail>(m => m.Weight);
                //    by_deliverydetail.SalesAmount = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount) - deliverydetailListAdd.Where(p => p.ProductName == "小计:").Sum<BY_DeliveryDetail>(m => m.SalesAmount);
                //    by_deliverydetail.WorkAmount = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.WorkAmount) - deliverydetailListAdd.Where(p => p.ProductName == "小计:").Sum<BY_DeliveryDetail>(m => m.WorkAmount);
                //    by_deliverydetail.Length = 0;
                //    by_deliverydetail.Weigh = 0;
                //    by_deliverydetail.Thick = 0;
                //    by_deliverydetail.JJtype = "按公斤";
                //    deliverydetailListAdd.Add(by_deliverydetail);
                //}
                //else
                //{
                by_deliverydetail = new BY_DeliveryDetail();
                by_deliverydetail.LineCode = 0;
                by_deliverydetail.ProductName = "合计:";
                by_deliverydetail.SteelCode = "合计:";
                by_deliverydetail.Qty = deliverydetailList.Where(p => p.SteelCode != "FY").Sum<BY_DeliveryDetail>(m => m.Qty);
                by_deliverydetail.Weight = deliverydetailList.Where(p => p.SteelCode != "FY").Sum<BY_DeliveryDetail>(m => (base_customer.Calc == 0 || m.OrderType == "毛料生产") ? m.Weight : m.TheoryWeight);
                //by_deliverydetail.Weight = deliverydetailList.Where(p => p.SteelCode != "FY" && p.Thick > 0).Sum<BY_DeliveryDetail>(m => m.Weight);
                by_deliverydetail.SalesAmount = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount);
                by_deliverydetail.WorkAmount = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.WorkAmount + m.SmallAmount);
                by_deliverydetail.Length = 0;
                by_deliverydetail.Weigh = 0;
                by_deliverydetail.Thick = 0;
                by_deliverydetail.JJtype = "按公斤";
                deliverydetailListAdd.Add(by_deliverydetail);
                //}
            }

            for (int i = 0; i < deliverydetailListAdd.Count; i++)
            {
                if (deliverydetailListAdd[i].OrderDetailId != null && by_delivery.LType == "RCL")
                {
                    deliverydetailListAdd[i].ProductName = DataFactory.Database().FindEntity<BY_QuotationHeat>("QuotationHeatCode", deliverydetailListAdd[i].OrderDetailId)?.TechnologQuery;
                }
            }
            Base_Company company = DataFactory.Database().FindEntity<Base_Company>("FullName", string.IsNullOrEmpty(by_delivery.CompanyName) ? "" : by_delivery.CompanyName);

            var JsonData = new
            {
                Delivery = by_delivery,
                BY_Quotation = by_quotation,
                DeliveryDetail = deliverydetailListAdd,
                QtySum = deliverydetailList.Where(p => p.SteelCode != "FY").Sum<BY_DeliveryDetail>(m => m.Qty),//总数量
                WeightSum = deliverydetailList.Where(p => p.SteelCode != "FY").Sum<BY_DeliveryDetail>(m => m.Weight),//总重量
                MaterialAmountSum = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount),//总金额
                UpperMaterialAmountSum = RMBHelper.CmycurD(CommonHelper.GetDecimal(deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount))),//大写总金额
                DeliveryCodes = DeliveryIds,
                base_customer = base_customer,
                base_customeraddress = base_customeraddress,
                hang = deliverydetailList.Count,
                company = company,
            };
            return Content(JsonData.ToJson());

        }



        public ActionResult PrintDataJsonSum(string DeliveryIds, string QuotationId, int num)
        {

            string[] ids = DeliveryIds.Split(',');
            List<BY_DeliveryDetail> deliverydetailList = new List<BY_DeliveryDetail>();
            for (int i = 0; i < ids.Length; i++)
            {
                BY_Delivery temp = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryCode", ids[i]);
                deliverydetailList.AddRange(DataFactory.Database().FindList<BY_DeliveryDetail>("DeliveryId", temp.DeliveryId).OrderBy(p => p.SortCode));
            }
            BY_Delivery by_delivery = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryCode", ids[0]);
            Base_CustomerProcess base_customerprocess = DataFactory.Database().FindEntityBySql<Base_CustomerProcess>(string.Format(@"SELECT *
  FROM Base_CustomerProcess WHERE CustomerId='{0}'
  AND WhereQty > 0", by_delivery.CustomerId));

            Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>("Code", by_delivery.CustomerCode);

            Base_CustomerAddress base_customeraddress = DataFactory.Database().FindEntity<Base_CustomerAddress>("CustomerId", by_delivery.CustomerId);
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>("QuotationCode", string.IsNullOrEmpty(by_delivery.QuotationCode) ? "" : by_delivery.QuotationCode);


            //BY_DeliveryDetail BY_DeliveryDetail1Small = new BY_DeliveryDetail();
            BY_DeliveryDetail BY_DeliveryDetail1Work = new BY_DeliveryDetail();
            BY_DeliveryDetail BY_DeliveryDetail1Work1 = new BY_DeliveryDetail();
            BY_DeliveryDetail BY_DeliveryDetail1JK = new BY_DeliveryDetail();
            BY_DeliveryDetail BY_DeliveryDetail1DD = new BY_DeliveryDetail();
            BY_DeliveryDetail BY_DeliveryDetail1Mian = new BY_DeliveryDetail();
            BY_DeliveryDetail BY_DeliveryDetail1Bian = new BY_DeliveryDetail();
            BY_DeliveryDetail BY_DeliveryDetail1Heat = new BY_DeliveryDetail();


            decimal? WorkPrice1 = 0;
            foreach (var item in deliverydetailList)
            {
                //if (item.SmallAmount > 0)
                //{
                //    BY_DeliveryDetail1Small.ProductName = "加工费";
                //    BY_DeliveryDetail1Small.SmallAmount = CommonHelper.GetDecimal(BY_DeliveryDetail1Small.SmallAmount) + item.SmallAmount;
                //    BY_DeliveryDetail1Small.SmallCost = item.SmallCost;
                //    BY_DeliveryDetail1Small.Qty = BY_DeliveryDetail1Small.Qty + item.Qty;
                //    item.SmallAmount = 0;
                //    item.SmallCost = 0;
                //}
                
                if (item.WorkAmount > 0)
                {
                    if (WorkPrice1 == 0)
                    {
                        WorkPrice1 = item.WorkPrice;
                    }
                    if (WorkPrice1 == item.WorkPrice)
                    {
                        BY_DeliveryDetail1Work.ProductName = "小件费";
                        BY_DeliveryDetail1Work.WorkAmount = CommonHelper.GetDecimal(BY_DeliveryDetail1Work.WorkAmount) + item.WorkAmount;
                        BY_DeliveryDetail1Work.WorkPrice = item.WorkPrice;
                        BY_DeliveryDetail1Work.Qty = CommonHelper.GetDecimal(BY_DeliveryDetail1Work.Qty) + item.Qty;
                        BY_DeliveryDetail1Work.SalesAmount = CommonHelper.GetDecimal(BY_DeliveryDetail1Work.SalesAmount) + item.WorkAmount;
                        item.SalesAmount = CommonHelper.GetDecimal(item.SalesAmount) - item.WorkAmount;
                        item.WorkAmount = 0;
                        item.WorkPrice = 0;
                    }
                    else 
                    {
                        BY_DeliveryDetail1Work1.ProductName = "小件费";
                        BY_DeliveryDetail1Work1.WorkAmount = CommonHelper.GetDecimal(BY_DeliveryDetail1Work1.WorkAmount) + item.WorkAmount;
                        BY_DeliveryDetail1Work1.WorkPrice = item.WorkPrice;
                        BY_DeliveryDetail1Work1.Qty = CommonHelper.GetDecimal(BY_DeliveryDetail1Work1.Qty) + item.Qty;
                        BY_DeliveryDetail1Work1.SalesAmount = CommonHelper.GetDecimal(BY_DeliveryDetail1Work1.SalesAmount) + item.WorkAmount;
                        item.SalesAmount = CommonHelper.GetDecimal(item.SalesAmount) - item.WorkAmount;
                        item.WorkAmount = 0;
                        item.WorkPrice = 0;
                    }
                    
                }
                if (item.JKCost > 0)
                {
                    BY_DeliveryDetail1JK.ProductName = "锯弓费";
                    BY_DeliveryDetail1JK.JKCost = CommonHelper.GetDecimal(BY_DeliveryDetail1JK.JKCost) + item.JKCost;
                    BY_DeliveryDetail1JK.JKCostPer = item.JKCostPer;
                    BY_DeliveryDetail1JK.Qty = CommonHelper.GetDecimal(BY_DeliveryDetail1JK.Qty) + item.Qty;
                    BY_DeliveryDetail1JK.SalesAmount = CommonHelper.GetDecimal(BY_DeliveryDetail1JK.SalesAmount) + item.JKCost;
                    item.SalesAmount = CommonHelper.GetDecimal(item.SalesAmount) - item.JKCost;
                    item.JKCost = 0;
                    item.JKCostPer = 0;
                }
                if (item.DDCost > 0)
                {
                    BY_DeliveryDetail1DD.ProductName = "切角费";
                    BY_DeliveryDetail1DD.DDCost = CommonHelper.GetDecimal(BY_DeliveryDetail1DD.DDCost) + item.DDCost;
                    BY_DeliveryDetail1DD.DDCostPer = item.DDCostPer;
                    BY_DeliveryDetail1DD.Qty = CommonHelper.GetDecimal(BY_DeliveryDetail1DD.Qty) + item.Qty;
                    BY_DeliveryDetail1DD.SalesAmount = CommonHelper.GetDecimal(BY_DeliveryDetail1DD.SalesAmount) + item.DDCost;
                    item.SalesAmount = CommonHelper.GetDecimal(item.SalesAmount) - item.DDCost;
                    item.DDCost = 0;
                    item.DDCostPer = 0;

                }
                if (item.MianCost > 0)
                {
                    BY_DeliveryDetail1Mian.ProductName = "面加工";
                    BY_DeliveryDetail1Mian.MianCost = CommonHelper.GetDecimal(BY_DeliveryDetail1Mian.MianCost) + item.MianCost;
                    BY_DeliveryDetail1Mian.MianCostPer = item.MianCostPer;
                    BY_DeliveryDetail1Mian.Qty = CommonHelper.GetDecimal(BY_DeliveryDetail1Mian.Qty) + item.Qty;
                    BY_DeliveryDetail1Mian.SalesAmount = CommonHelper.GetDecimal(BY_DeliveryDetail1Mian.SalesAmount) + item.MianCost;
                    item.SalesAmount = CommonHelper.GetDecimal(item.SalesAmount) - item.MianCost;
                    item.MianCost = 0;
                    item.MianCostPer = 0;
                }
                if (item.BianCost > 0)
                {
                    BY_DeliveryDetail1Bian.ProductName = "边加工";
                    BY_DeliveryDetail1Bian.BianCost = CommonHelper.GetDecimal(BY_DeliveryDetail1Bian.BianCost) + item.BianCost;
                    BY_DeliveryDetail1Bian.BianCostPer = item.BianCostPer;
                    BY_DeliveryDetail1Bian.Qty = CommonHelper.GetDecimal(BY_DeliveryDetail1Bian.Qty) + item.Qty;
                    BY_DeliveryDetail1Bian.SalesAmount = CommonHelper.GetDecimal(BY_DeliveryDetail1Bian.SalesAmount) + item.BianCost;
                    item.SalesAmount = CommonHelper.GetDecimal(item.SalesAmount) - item.BianCost;
                    item.BianCost = 0;
                    item.BianCostPer = 0;
                }
                if (item.HeatAmount > 0)
                {
                    BY_DeliveryDetail1Heat.ProductName = "热处理";
                    BY_DeliveryDetail1Heat.HeatAmount = CommonHelper.GetDecimal(BY_DeliveryDetail1Heat.HeatAmount)+ item.HeatAmount;
                    BY_DeliveryDetail1Heat.HeatPrice = item.HeatPrice;
                    BY_DeliveryDetail1Heat.Qty = CommonHelper.GetDecimal(BY_DeliveryDetail1Heat.Qty) + item.Qty;
                    BY_DeliveryDetail1Heat.SalesAmount = CommonHelper.GetDecimal(BY_DeliveryDetail1Heat.SalesAmount) + item.HeatAmount;
                    item.SalesAmount = CommonHelper.GetDecimal(item.SalesAmount) - item.HeatAmount;
                    item.HeatAmount = 0;
                    item.HeatPrice = 0;
                }
                
            }
            //if (BY_DeliveryDetail1Small.SmallAmount>0)
            //{
            //    deliverydetailList.Add(BY_DeliveryDetail1Small);
                
            //}
            if (BY_DeliveryDetail1Work1.WorkAmount > 0)
            {
                deliverydetailList.Add(BY_DeliveryDetail1Work1);

            }
            if (BY_DeliveryDetail1Work.WorkAmount > 0)
            {
                deliverydetailList.Add(BY_DeliveryDetail1Work);

            }
            if (BY_DeliveryDetail1JK.JKCost > 0)
            {
                deliverydetailList.Add(BY_DeliveryDetail1JK);

            }
            if (BY_DeliveryDetail1DD.DDCost > 0)
            {
                deliverydetailList.Add(BY_DeliveryDetail1DD);

            }
            if (BY_DeliveryDetail1Mian.MianCost > 0)
            {
                deliverydetailList.Add(BY_DeliveryDetail1Mian);

            }
            if (BY_DeliveryDetail1Bian.BianCost > 0)
            {
                deliverydetailList.Add(BY_DeliveryDetail1Bian);

            }
            if (BY_DeliveryDetail1Heat.HeatAmount > 0)
            {
                deliverydetailList.Add(BY_DeliveryDetail1Heat);

            }

            List<BY_DeliveryDetail> deliverydetailListAdd = new List<BY_DeliveryDetail>();

            bool temo = false;
            BY_DeliveryDetail by_deliverydetail;

            decimal? xiaojiqty = 0;
            decimal? xiaojiwiehgt = 0;
            decimal? xiaojisalesamount = 0;
            decimal? workamount = 0;

            for (int i = 0; i < deliverydetailList.Count; i++)
            {
                deliverydetailListAdd.Add(deliverydetailList[i]);
                if (CommonHelper.GetDecimal(deliverydetailList[i].Length) > 0)
                {
                    xiaojiqty += CommonHelper.GetDecimal(deliverydetailList[i].Qty);
                    xiaojiwiehgt += CommonHelper.GetDecimal(deliverydetailList[i].Weight);
                }
                xiaojisalesamount += CommonHelper.GetDecimal(deliverydetailList[i].SalesAmount);
                workamount += CommonHelper.GetDecimal(deliverydetailList[i].WorkAmount);
                if ((i + 1) % (num - 1) == 0)
                {
                    by_deliverydetail = new BY_DeliveryDetail();
                    deliverydetailListAdd.Add(by_deliverydetail);
                    temo = true;
                }
                if (temo)
                {
                    deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());
                    by_deliverydetail = new BY_DeliveryDetail();
                    by_deliverydetail.LineCode = 0;
                    by_deliverydetail.ProductName = "小计:";
                    by_deliverydetail.WorkAmount = workamount;
                    by_deliverydetail.Qty = xiaojiqty;
                    by_deliverydetail.Weight = xiaojiwiehgt;
                    by_deliverydetail.SalesAmount = xiaojisalesamount;
                    by_deliverydetail.Length = 0;
                    by_deliverydetail.Weigh = 0;
                    by_deliverydetail.Thick = 0;
                    by_deliverydetail.JJtype = "按公斤";
                    deliverydetailListAdd.Add(by_deliverydetail);
                    xiaojiqty = 0;
                    xiaojiwiehgt = 0;
                    xiaojisalesamount = 0;
                    workamount = 0;
                    temo = false;
                }
            }

            if (deliverydetailListAdd.Count % num > 0)
            {
                int j = (num - deliverydetailListAdd.Count % num);
                for (int i = 0; i < j; i++)
                {
                    by_deliverydetail = new BY_DeliveryDetail();
                    deliverydetailListAdd.Add(by_deliverydetail);
                    temo = true;
                }

            }

            if (temo)
            {
                deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());
            }





            if (deliverydetailListAdd.Count % num == 0 && deliverydetailListAdd.LastOrDefault().LineCode == null)
            {
                deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());

            }

            if (deliverydetailList.Count >= num)
            {
                by_deliverydetail = new BY_DeliveryDetail();
                by_deliverydetail.LineCode = 0;
                by_deliverydetail.ProductName = "小计:";
                by_deliverydetail.Qty = deliverydetailList.Where(p => p.SteelCode != "FY"  && CommonHelper.GetDecimal(p.Thick) > 0).Sum<BY_DeliveryDetail>(m => m.Qty) - deliverydetailListAdd.Where(p => p.ProductName == "小计:").Sum<BY_DeliveryDetail>(m => m.Qty);
                by_deliverydetail.Weight = deliverydetailList.Where(p => p.SteelCode != "FY" && CommonHelper.GetDecimal(p.Thick) > 0).Sum<BY_DeliveryDetail>(m => m.Weight) - deliverydetailListAdd.Where(p => p.ProductName == "小计:").Sum<BY_DeliveryDetail>(m => m.Weight);
                by_deliverydetail.SalesAmount = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount) - deliverydetailListAdd.Where(p => p.ProductName == "小计:").Sum<BY_DeliveryDetail>(m => m.SalesAmount);
                by_deliverydetail.WorkAmount = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.WorkAmount) - deliverydetailListAdd.Where(p => p.ProductName == "小计:").Sum<BY_DeliveryDetail>(m => m.WorkAmount);
                by_deliverydetail.Length = 0;
                by_deliverydetail.Weigh = 0;
                by_deliverydetail.Thick = 0;
                    by_deliverydetail.JJtype = "按公斤";
                deliverydetailListAdd.Add(by_deliverydetail);
            }
            else
            {
                by_deliverydetail = new BY_DeliveryDetail();
                by_deliverydetail.LineCode = 0;
                by_deliverydetail.ProductName = "小计:";
                by_deliverydetail.Qty = deliverydetailList.Where(p => p.SteelCode != "FY" && CommonHelper.GetDecimal(p.Thick) > 0).Sum<BY_DeliveryDetail>(m => m.Qty);
                by_deliverydetail.Weight = deliverydetailList.Where(p => p.SteelCode != "FY" && CommonHelper.GetDecimal(p.Thick) > 0).Sum<BY_DeliveryDetail>(m => m.Weight);
                by_deliverydetail.SalesAmount = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount);
                by_deliverydetail.WorkAmount = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.WorkAmount);
                by_deliverydetail.Length = 0;
                by_deliverydetail.Weigh = 0;
                by_deliverydetail.Thick = 0;
                    by_deliverydetail.JJtype = "按公斤";
                deliverydetailListAdd.Add(by_deliverydetail);
            }


            var JsonData = new
            {
                Delivery = by_delivery,
                BY_Quotation = by_quotation,
                DeliveryDetail = deliverydetailListAdd,
                QtySum = deliverydetailList.Where(p => p.SteelCode != "FY" && CommonHelper.GetDecimal(p.Thick) != 0).Sum<BY_DeliveryDetail>(m => m.Qty),//总数量
                WeightSum = deliverydetailList.Where(p => p.SteelCode != "FY" && CommonHelper.GetDecimal(p.Thick) != 0).Sum<BY_DeliveryDetail>(m => m.Weight),//总重量
                MaterialAmountSum = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount),//总金额
                UpperMaterialAmountSum = RMBHelper.CmycurD(CommonHelper.GetDecimal(deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount))),//大写总金额
                DeliveryCodes = DeliveryIds,
                base_customer = base_customer,
                base_customeraddress = base_customeraddress,
            };
            return Content(JsonData.ToJson());

        }


        public ActionResult PrintDataJsonsummary(string DeliveryIds, int num)
        {
            string[] ids = DeliveryIds.Split(',');
            List<BY_DeliveryDetail> deliverydetailList = new List<BY_DeliveryDetail>();
            for (int i = 0; i < ids.Length; i++)
            {
                BY_Delivery temp = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryCode", ids[i]);
                deliverydetailList.AddRange(DataFactory.Database().FindListBySql<BY_DeliveryDetail>(@"select ProductName,MaterialPrice,sum(Qty) Qty,sum(MaterialAmount) MaterialAmount,sum(Weight) Weight,sum(WorkAmount+SmallAmount+DDCost) WorkAmount,sum(SalesAmount) SalesAmount from BY_DeliveryDetail where DeliveryId='" + temp.DeliveryId + @"'
group by ProductName,MaterialPrice"));
            }
            BY_Delivery by_delivery = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryCode", ids[0]);
            Base_CustomerProcess base_customerprocess = DataFactory.Database().FindEntityBySql<Base_CustomerProcess>(string.Format(@"SELECT *
  FROM Base_CustomerProcess WHERE CustomerId='{0}'
  AND WhereQty > 0", by_delivery.CustomerId));

            Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>("Code", by_delivery.CustomerCode);

            Base_CustomerAddress base_customeraddress = DataFactory.Database().FindEntity<Base_CustomerAddress>("CustomerId", by_delivery.CustomerId);
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>("QuotationCode", string.IsNullOrEmpty(by_delivery.QuotationCode) ? "" : by_delivery.QuotationCode);


            List<BY_DeliveryDetail> deliverydetailListAdd = new List<BY_DeliveryDetail>();

            bool temo = false;
            BY_DeliveryDetail by_deliverydetail;

            decimal? xiaojiqty = 0;
            decimal? xiaojiwiehgt = 0;
            decimal? xiaojisalesamount = 0;
            decimal? workamount = 0;

            for (int i = 0; i < deliverydetailList.Count; i++)
            {
                deliverydetailListAdd.Add(deliverydetailList[i]);
                xiaojiqty += deliverydetailList[i].Qty;
                xiaojiwiehgt += deliverydetailList[i].Weight;
                xiaojisalesamount += deliverydetailList[i].SalesAmount;
                workamount += deliverydetailList[i].WorkAmount;
                if ((i + 1) % (num - 1) == 0)
                {
                    by_deliverydetail = new BY_DeliveryDetail();
                    deliverydetailListAdd.Add(by_deliverydetail);
                    temo = true;
                }
                if (temo)
                {
                    deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());
                    by_deliverydetail = new BY_DeliveryDetail();
                    by_deliverydetail.LineCode = 0;
                    by_deliverydetail.ProductName = "小计:";
                    by_deliverydetail.WorkAmount = workamount;
                    by_deliverydetail.Qty = xiaojiqty;
                    by_deliverydetail.Weight = xiaojiwiehgt;
                    by_deliverydetail.SalesAmount = xiaojisalesamount;
                    by_deliverydetail.Length = 0;
                    by_deliverydetail.Weigh = 0;
                    by_deliverydetail.Thick = 0;
                    by_deliverydetail.JJtype = "按公斤";
                    deliverydetailListAdd.Add(by_deliverydetail);
                    xiaojiqty = 0;
                    xiaojiwiehgt = 0;
                    xiaojisalesamount = 0;
                    workamount = 0;
                    temo = false;
                }
            }

            if (deliverydetailListAdd.Count % num > 0)
            {
                int j = (num - deliverydetailListAdd.Count % num);
                for (int i = 0; i < j; i++)
                {
                    by_deliverydetail = new BY_DeliveryDetail();
                    deliverydetailListAdd.Add(by_deliverydetail);
                    temo = true;
                }

            }

            if (temo)
            {
                deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());
            }





            if (deliverydetailListAdd.Count % num == 0 && deliverydetailListAdd.LastOrDefault().LineCode == null)
            {
                deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());

            }

            if (deliverydetailList.Count >= num)
            {
                by_deliverydetail = new BY_DeliveryDetail();
                by_deliverydetail.LineCode = 0;
                by_deliverydetail.ProductName = "小计:";
                by_deliverydetail.Qty = deliverydetailList.Where(p => p.SteelCode != "FY" && p.Thick > 0).Sum<BY_DeliveryDetail>(m => m.Qty) - deliverydetailListAdd.Where(p => p.ProductName == "小计:").Sum<BY_DeliveryDetail>(m => m.Qty);
                by_deliverydetail.Weight = deliverydetailList.Where(p => p.SteelCode != "FY" && p.Thick > 0).Sum<BY_DeliveryDetail>(m => m.Weight) - deliverydetailListAdd.Where(p => p.ProductName == "小计:").Sum<BY_DeliveryDetail>(m => m.Weight);
                by_deliverydetail.SalesAmount = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount) - deliverydetailListAdd.Where(p => p.ProductName == "小计:").Sum<BY_DeliveryDetail>(m => m.SalesAmount);
                by_deliverydetail.WorkAmount = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.WorkAmount) - deliverydetailListAdd.Where(p => p.ProductName == "小计:").Sum<BY_DeliveryDetail>(m => m.WorkAmount);
                by_deliverydetail.Length = 0;
                by_deliverydetail.Weigh = 0;
                by_deliverydetail.Thick = 0;
                    by_deliverydetail.JJtype = "按公斤";
                deliverydetailListAdd.Add(by_deliverydetail);
            }
            else
            {
                by_deliverydetail = new BY_DeliveryDetail();
                by_deliverydetail.LineCode = 0;
                by_deliverydetail.ProductName = "小计:";
                by_deliverydetail.Qty = deliverydetailList.Where(p => p.SteelCode != "FY").Sum<BY_DeliveryDetail>(m => m.Qty);
                by_deliverydetail.Weight = deliverydetailList.Where(p => p.SteelCode != "FY" ).Sum<BY_DeliveryDetail>(m => m.Weight);
                by_deliverydetail.SalesAmount = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount);
                by_deliverydetail.WorkAmount = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.WorkAmount);
                by_deliverydetail.Length = 0;
                by_deliverydetail.Weigh = 0;
                by_deliverydetail.Thick = 0;
                    by_deliverydetail.JJtype = "按公斤";
                deliverydetailListAdd.Add(by_deliverydetail);
            }


            var JsonData = new
            {
                Delivery = by_delivery,
                BY_Quotation = by_quotation,
                DeliveryDetail = deliverydetailListAdd,
                QtySum = deliverydetailList.Where(p => p.SteelCode != "FY" && p.Thick != 0).Sum<BY_DeliveryDetail>(m => m.Qty),//总数量
                WeightSum = deliverydetailList.Where(p => p.SteelCode != "FY" && p.Thick != 0).Sum<BY_DeliveryDetail>(m => m.Weight),//总重量
                MaterialAmountSum = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount),//总金额
                UpperMaterialAmountSum = RMBHelper.CmycurD(CommonHelper.GetDecimal(deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount))),//大写总金额
                DeliveryCodes = DeliveryIds,
                base_customer = base_customer,
                base_customeraddress = base_customeraddress,
            };
            return Content(JsonData.ToJson());

        }


        public ActionResult PrintDataJsonRCL(string DeliveryIds, string QuotationId, int num)
        {
            string[] ids = DeliveryIds.Split(',');
            List<BY_DeliveryDetail> deliverydetailList = new List<BY_DeliveryDetail>();
            for (int i = 0; i < ids.Length; i++)
            {
                BY_Delivery temp = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryCode", ids[i]);
                deliverydetailList.AddRange(DataFactory.Database().FindList<BY_DeliveryDetail>("DeliveryId", temp.DeliveryId).OrderBy(p => p.SortCode));
            }


            BY_Delivery by_delivery = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryCode", ids[0]);
            Base_CustomerProcess base_customerprocess = DataFactory.Database().FindEntityBySql<Base_CustomerProcess>(string.Format(@"SELECT *
  FROM Base_CustomerProcess WHERE CustomerId='{0}'
  AND WhereQty > 0", by_delivery.CustomerId));

            Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>("Code", by_delivery.CustomerCode);

            Base_CustomerAddress base_customeraddress = DataFactory.Database().FindEntity<Base_CustomerAddress>("CustomerId", by_delivery.CustomerId);

            List<BY_DeliveryDetail> deliverydetailListAdd = new List<BY_DeliveryDetail>();

            bool temo = false;
            BY_DeliveryDetail by_deliverydetail;

            decimal? xiaojiqty = 0;
            decimal? xiaojiwiehgt = 0;
            decimal? xiaojisalesamount = 0;


            int sale = 0;
            for (int i = 0; i < deliverydetailList.Count; i++)
            {
                if (deliverydetailList[i].SalesAmount == 0)
                {
                    sale = 1;
                }
                deliverydetailListAdd.Add(deliverydetailList[i]);
                xiaojiqty += deliverydetailList[i].Qty;
                xiaojiwiehgt += deliverydetailList[i].Weight;
                xiaojisalesamount += deliverydetailList[i].SalesAmount;
                if ((i + 1) % (num - 1) == 0)
                {
                    by_deliverydetail = new BY_DeliveryDetail();
                    deliverydetailListAdd.Add(by_deliverydetail);
                    temo = true;
                }
                if (temo)
                {
                    deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());
                    by_deliverydetail = new BY_DeliveryDetail();
                    by_deliverydetail.LineCode = 0;
                    by_deliverydetail.ProductName = "小计:";
                    by_deliverydetail.Qty = xiaojiqty;
                    by_deliverydetail.Weight = xiaojiwiehgt;
                    by_deliverydetail.SalesAmount = xiaojisalesamount;
                    by_deliverydetail.Length = 0;
                    by_deliverydetail.Weigh = 0;
                    by_deliverydetail.Thick = 0;
                    by_deliverydetail.JJtype = "按公斤";
                    deliverydetailListAdd.Add(by_deliverydetail);
                    xiaojiqty = 0;
                    xiaojiwiehgt = 0;
                    xiaojisalesamount = 0;
                    temo = false;
                }
            }

            if (deliverydetailListAdd.Count % num > 0)
            {
                int j = (num - deliverydetailListAdd.Count % num);
                for (int i = 0; i < j; i++)
                {
                    by_deliverydetail = new BY_DeliveryDetail();
                    deliverydetailListAdd.Add(by_deliverydetail);
                    temo = true;
                }

            }

            if (temo)
            {
                deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());
            }





            if (deliverydetailListAdd.Count % num == 0 && deliverydetailListAdd.LastOrDefault().LineCode == null)
            {
                deliverydetailListAdd.Remove(deliverydetailListAdd.LastOrDefault());

            }

            if (deliverydetailList.Count >= num)
            {
                by_deliverydetail = new BY_DeliveryDetail();
                by_deliverydetail.LineCode = 0;
                by_deliverydetail.ProductName = "小计:";
                by_deliverydetail.Qty = deliverydetailList.Where(p => p.SteelCode != "FY" && p.Thick > 0).Sum<BY_DeliveryDetail>(m => m.Qty) - deliverydetailListAdd.Where(p => p.ProductName == "小计:").Sum<BY_DeliveryDetail>(m => m.Qty);
                by_deliverydetail.Weight = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.Weight) - deliverydetailListAdd.Where(p => p.ProductName == "小计:").Sum<BY_DeliveryDetail>(m => m.Weight);
                by_deliverydetail.SalesAmount = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount) - deliverydetailListAdd.Where(p => p.ProductName == "小计:").Sum<BY_DeliveryDetail>(m => m.SalesAmount);
                by_deliverydetail.Length = 0;
                by_deliverydetail.Weigh = 0;
                by_deliverydetail.Thick = 0;
                    by_deliverydetail.JJtype = "按公斤";
                deliverydetailListAdd.Add(by_deliverydetail);
            }
            else
            {
                by_deliverydetail = new BY_DeliveryDetail();
                by_deliverydetail.LineCode = 0;
                by_deliverydetail.ProductName = "小计:";
                by_deliverydetail.Qty = deliverydetailList.Where(p => p.SteelCode != "FY" && p.Thick > 0).Sum<BY_DeliveryDetail>(m => m.Qty);
                by_deliverydetail.Weight = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.Weight);
                by_deliverydetail.SalesAmount = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount);
                by_deliverydetail.Length = 0;
                by_deliverydetail.Weigh = 0;
                by_deliverydetail.Thick = 0;
                    by_deliverydetail.JJtype = "按公斤";
                deliverydetailListAdd.Add(by_deliverydetail);
            }


            var JsonData = new
            {
                Delivery = by_delivery,
                DeliveryDetail = deliverydetailListAdd,
                QtySum = deliverydetailList.Where(p => p.SteelCode != "FY" && p.Thick > 0).Sum<BY_DeliveryDetail>(m => m.Qty),//总数量
                WeightSum = deliverydetailList.Where(p => p.SteelCode != "FY" && p.Thick > 0).Sum<BY_DeliveryDetail>(m => m.Weight),//总重量
                MaterialAmountSum = deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount),//总金额
                UpperMaterialAmountSum = RMBHelper.CmycurD(CommonHelper.GetDecimal(deliverydetailList.Sum<BY_DeliveryDetail>(m => m.SalesAmount))),//大写总金额
                DeliveryCodes = DeliveryIds,
                base_customer = base_customer,
                base_customeraddress = base_customeraddress,
                sale = sale,
            };
            return Content(JsonData.ToJson());

        }

        
        /// <summary>
        /// 绑定表格
        /// </summary>
        /// <param name="ParameterJson">查询条件</param>
        /// <param name="Gridpage">分页条件</param>
        /// <returns></returns>
        [LoginAuthorize]
        public ActionResult GetGridPageJson(string Condition, JqGridParam jqgridparam)
        {

            BY_DeliveryBll by_deliverybll = new Business.BY_DeliveryBll();
            Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
            DataTable by_orderdetail = by_deliverybll.GetGridPageJson(QueryParameter, ref jqgridparam);
            Stopwatch watch = CommonHelper.TimerStart();
            var JsonData = new
            {
                total = jqgridparam.total,
                page = jqgridparam.page,
                records = jqgridparam.records,
                costtime = CommonHelper.TimerEnd(watch),
                rows = by_orderdetail,
            };
            return Content(JsonData.ToJson());

        }
        [LoginAuthorize]
        public ActionResult GetGridPageJson1(string Condition, JqGridParam jqgridparam)
        {

            BY_DeliveryBll by_deliverybll = new Business.BY_DeliveryBll();
            Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
            DataTable by_orderdetail = by_deliverybll.GetGridPageJson1(QueryParameter, ref jqgridparam);
            Stopwatch watch = CommonHelper.TimerStart();
            var JsonData = new
            {
                total = jqgridparam.total,
                page = jqgridparam.page,
                records = jqgridparam.records,
                costtime = CommonHelper.TimerEnd(watch),
                rows = by_orderdetail,
            };
            return Content(JsonData.ToJson());

        }
        public virtual ActionResult UpdatePrint(string DeliveryIds, string PrintUserId)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                string[] ids = DeliveryIds.Split(',');
                string idss = "";
                for (int i = 0; i < ids.Length; i++)
                {
                    idss = idss + "'"+ids[i] + "'"+",";
                }
                idss = idss.Trim(',');
                List<BY_Delivery> by_deliverys = DataFactory.Database().FindListBySql<BY_Delivery>(string.Format(@" select * from BY_Delivery where  DeliveryCode in ({0})   and DeleteMark = 0 ", idss));
                foreach (var item in by_deliverys)
	            {
                    if (item != null && item.DeliveryId != null)
                    {
                        item.PrintUserId = ManageProvider.Provider.Current().UserName;
                        if (item.PrintDate == null)
                        {
                            item.PrintDate = DateTime.Now;
                        }
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                }
                DataFactory.Database().Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功！" }.ToString());
            }

            catch (Exception ex)
            {
                DataFactory.Database().Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult Update(string KeyValue, string PrintUserId, string ComeBackUserId, string AuditUserId, int? Enabled)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    BY_Delivery by_delivery = DataFactory.Database().FindEntity<BY_Delivery>(KeyValue);
                    BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(KeyValue);

                    


                    BY_Delivery by_delivery1 = new BY_Delivery();
                    if (by_quotation.QuotationCode!= null)
                    {
                        by_delivery1 = DataFactory.Database().FindEntity<BY_Delivery>("QuotationCode", by_quotation.QuotationCode);
                    }

                    if (by_delivery != null && by_delivery.DeliveryId != null)
                    {
                        if (!string.IsNullOrEmpty(PrintUserId))
                        {
                            by_delivery.PrintUserId = string.IsNullOrEmpty(by_delivery.PrintUserId) ? ManageProvider.Provider.Current().UserId : "";
                            if (by_delivery.PrintDate == null)
                            {
                                by_delivery.PrintDate = DateTime.Now;
                            }
                        }
                        if (!string.IsNullOrEmpty(ComeBackUserId))
                        {
                            by_delivery.ComeBackUserId = string.IsNullOrEmpty(by_delivery.ComeBackUserId) ? ManageProvider.Provider.Current().UserId : "";
                            by_delivery.ComeBackDate = DateTime.Now;
                        }
                        if (!string.IsNullOrEmpty(AuditUserId))
                        {
                            by_delivery.AuditUserId = string.IsNullOrEmpty(by_delivery.AuditUserId) ? ManageProvider.Provider.Current().UserId : "";
                            by_delivery.AuditDate = DateTime.Now;
                        }
                        BY_Quotation by_quotation1 = DataFactory.Database().FindEntity<BY_Quotation>("QuotationCode", string.IsNullOrEmpty(by_delivery.QuotationCode) ? "" : by_delivery.QuotationCode);
                        if (by_quotation1 != null && by_quotation1.QuotationId != null)
                        {
                            if (!string.IsNullOrEmpty(PrintUserId))
                            {
                                by_quotation1.PrintUserId = ManageProvider.Provider.Current().UserId;
                                if (by_quotation1.PrintDate == null)
                                {
                                    by_quotation1.PrintDate = DateTime.Now;
                                }
                            }
                            DataFactory.Database().Update(by_quotation1, isOpenTrans);
                        }
                        if (Enabled == -1)
                        {
                            if (by_delivery.GetedAmount > 0)
                            {
                                DataFactory.Database().Rollback();
                                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：已收费，不能作废!"  }.ToString());
                            }
                            if (!string.IsNullOrEmpty(by_delivery.CheckSheetCode ))
                            {
                                DataFactory.Database().Rollback();
                                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：已对账，不能作废!" }.ToString());
                            }
                            
                            by_delivery.Enabled = -by_delivery.Enabled;
                            by_delivery.TotalAmount = 0;

                        }
                        DataFactory.Database().Update(by_delivery, isOpenTrans);
                    }


                    
                    if (by_quotation != null && by_quotation.QuotationId != null)
                    {
                        if (!string.IsNullOrEmpty(PrintUserId))
                        {
                            by_quotation.PrintUserId = ManageProvider.Provider.Current().UserId;
                            if (by_quotation.PrintDate == null)
                            {
                                by_quotation.PrintDate = DateTime.Now;
                            }
                        }
                        
                        if (by_delivery1 != null && by_delivery1.DeliveryId != null)
                        {
                            if (!string.IsNullOrEmpty(PrintUserId))
                            {
                                by_delivery1.PrintUserId = ManageProvider.Provider.Current().UserId;
                                if (by_delivery1.PrintDate == null)
                                {
                                    by_delivery1.PrintDate = DateTime.Now;
                                }
                            }
                            if (!string.IsNullOrEmpty(ComeBackUserId))
                            {
                                by_delivery1.ComeBackUserId = ManageProvider.Provider.Current().UserId;
                                by_delivery1.ComeBackDate = DateTime.Now;
                            }
                            if (!string.IsNullOrEmpty(AuditUserId))
                            {
                                by_delivery1.AuditUserId = ManageProvider.Provider.Current().UserId;
                                by_delivery1.AuditDate = DateTime.Now;
                            }
                            DataFactory.Database().Update(by_delivery1, isOpenTrans);
                        }
                        DataFactory.Database().Update(by_quotation, isOpenTrans);
                    }
                }
                DataFactory.Database().Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功！" }.ToString());
            }
            catch (Exception ex)
            {
                DataFactory.Database().Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult ReturnDetail(string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                BY_DeliveryDetail by_deliverydetail = DataFactory.Database().FindEntity<BY_DeliveryDetail>(KeyValue);
                BY_Delivery by_delivery = DataFactory.Database().FindEntity<BY_Delivery>(by_deliverydetail.DeliveryId);
                if (!string.IsNullOrEmpty(by_delivery.CheckSheetCode))
                {
                    database.Rollback();
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：已生成对账单，无法退回！" }.ToString());
                }

                if (!string.IsNullOrEmpty(KeyValue))
                {
                    BY_OrderDetail by_orderdetail = DataFactory.Database().FindEntity<BY_OrderDetail>("QuotationDetailId",by_deliverydetail.QuotationDetailId);

                    List<BY_OrderComplete> by_ordercompletes = DataFactory.Database().FindList<BY_OrderComplete>("OrderDetailId", by_orderdetail.OrderDetailId);
                    if (by_ordercompletes.Count == 0)
                    {
                        //database.Rollback();
                        //return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：直接发运单无法回退！" }.ToString());
                        BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>("QuotationCode", by_delivery.QuotationCode);
                        by_quotation.IsMobileInput = 0;
                        by_quotation.BillStatus = QuotationBillStatus.已驳回.ToString();
                        by_quotation.CheckStatus = -1;
                        by_quotation.CheckDate = null;
                        by_quotation.CheckUserId = "";
                        by_quotation.SureDate = null;
                        by_quotation.SureUserId = "";
                        by_quotation.PlaceOrderDate = null;
                        by_quotation.PlaceOrderUserId = "";

                        DataFactory.Database().Update(by_quotation, isOpenTrans);

                    }

                    BY_OrderComplete by_ordercomplete1 = by_ordercompletes.Where(p => string.IsNullOrEmpty(p.DeliveryCode)).FirstOrDefault();
                    if (by_ordercomplete1!= null && by_ordercomplete1.OrderCompleteId != null)
                    {
                        by_ordercomplete1.Qty = by_ordercomplete1.Qty + Convert.ToInt32(by_deliverydetail.Qty);
                        by_ordercomplete1.Weight = by_ordercomplete1.Weight + by_deliverydetail.Weight;
                        by_ordercomplete1.TheoryWeight = by_ordercomplete1.TheoryWeight + by_deliverydetail.TheoryWeight;
                        by_ordercomplete1.Modify(by_ordercomplete1.OrderCompleteId);
                        DataFactory.Database().Update(by_ordercomplete1, isOpenTrans);
                    }
                    else
                    {
                        BY_OrderComplete by_ordercomplete2 = by_ordercompletes.Where(p => p.DeliveryCode == by_delivery.DeliveryCode).FirstOrDefault();
                        if (by_ordercomplete2!=null && by_ordercomplete2.OrderCompleteId != null)
                        {
                            by_ordercomplete2.DeliveryCode = "";
                            by_ordercomplete2.Modify(by_ordercomplete2.OrderCompleteId);
                            DataFactory.Database().Update(by_ordercomplete2, isOpenTrans);
                        }

                    }
                    DataFactory.Database().Delete(by_deliverydetail, isOpenTrans);

                }
                DataFactory.Database().Commit();

                List<BY_DeliveryDetail> by_deliverydetails = DataFactory.Database().FindList<BY_DeliveryDetail>("DeliveryId", by_delivery.DeliveryId);
                by_delivery.Qty = by_deliverydetails.Where(p => p.Length > 0).Sum(p => p.Qty);
                by_delivery.Weight = by_deliverydetails.Where(p => p.Length > 0).Sum(p => p.Weight);
                by_delivery.TotalAmount = by_deliverydetails.Sum(p => p.SalesAmount);
                DataFactory.Database().Update(by_delivery);
                new OrderQueryController().SetOrderStatus(by_delivery.OrderCode);

                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功！" }.ToString());
            }
            catch (Exception ex)
            {
                DataFactory.Database().Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult Return(string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            BY_Delivery by_delivery = DataFactory.Database().FindEntity<BY_Delivery>(KeyValue);

            try
            {
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    List<BY_DeliveryDetail> by_deliverydetails = DataFactory.Database().FindList<BY_DeliveryDetail>("DeliveryId", by_delivery.DeliveryId);
                    if (!string.IsNullOrEmpty(by_delivery.CheckSheetCode))
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：已生成对账单，无法退回！" }.ToString());
                    }
                    if (by_delivery.GetedAmount>0)
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：已收款，无法退回！" }.ToString());
                    }
                    
                    List<BY_OrderComplete> by_ordercompletes = DataFactory.Database().FindList<BY_OrderComplete>("DeliveryCode", by_delivery.DeliveryCode);
                    if (by_ordercompletes.Count == 0)
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：直接发运单无法回退！" }.ToString());
                    }
                    List<BY_OrderComplete> by_ordercompletesorder = DataFactory.Database().FindList<BY_OrderComplete>("OrderId", by_delivery.OrderId);
                    foreach (BY_OrderComplete item in by_ordercompletes)
                    {
                        //BY_OrderComplete by_ordercomplete = DataFactory.Database().FindEntityByWhere<BY_OrderComplete>(" and (DeliveryCode IS  null or DeliveryCode='')  and OrderDetailId = '" + item.OrderDetailId + "' and OrderCompleteId != '" + item.OrderCompleteId + "'");
                        BY_OrderComplete by_ordercomplete = by_ordercompletesorder.Where(p => string.IsNullOrEmpty(p.DeliveryCode) && p.OrderDetailId == item.OrderDetailId && p.OrderCompleteId != item.OrderCompleteId).FirstOrDefault();
                        if (by_ordercomplete == null || by_ordercomplete.OrderCompleteId == null)
                        {
                            item.DeliveryCode = "";
                            item.Modify(item.OrderCompleteId);
                            DataFactory.Database().Update(item, isOpenTrans);
                        }
                        else
                        {
                            by_ordercomplete.Qty = by_ordercomplete.Qty + item.Qty;
                            by_ordercomplete.Weight = by_ordercomplete.Weight + item.Weight;
                            by_ordercomplete.TheoryWeight = by_ordercomplete.TheoryWeight + item.TheoryWeight;
                            by_ordercomplete.Modify(by_ordercomplete.OrderCompleteId);
                            DataFactory.Database().Update(by_ordercomplete, isOpenTrans);
                            DataFactory.Database().Delete(item, isOpenTrans);
                        }
                    }
                    DataFactory.Database().Delete(by_delivery, isOpenTrans);
                    foreach (var item in by_deliverydetails)
                    {
                        DataFactory.Database().Delete(item, isOpenTrans);
                    }
                }
                DataFactory.Database().Commit();
                new OrderQueryController().SetOrderStatus(by_delivery.OrderCode);

                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功！" }.ToString());
            }
            catch (Exception ex)
            {
                DataFactory.Database().Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
    }
}