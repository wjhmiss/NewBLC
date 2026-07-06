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

namespace DNS.WebApp.Areas.OrderModule.Controllers
{
    /// <summary>
    /// BY_OrderDetailBeOut控制器
    /// </summary>
    public class BY_OrderDetailBeOutController : PublicController<BY_OrderDetailBeOut>
    {
        public ActionResult BeOutListJson(string OrderDetailOutIds, string CustomerCode)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                string[] orderdetailoutids = OrderDetailOutIds.Split(',');
                string temp = "";
                for (int i = 0; i < orderdetailoutids.Length; i++)
                {
                    temp = temp + "'" + orderdetailoutids[i] + "',";
                }
                temp = temp.Trim(',');
                IEnumerable<BY_OrderDetailBeOut> by_orderdetailbeouts = DataFactory.Database().FindListBySql<BY_OrderDetailBeOut>(string.Format(" select * from BY_OrderDetailBeOut where OrderDetailOutId in ({0})", temp));
                //Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>("Code", by_orderdetailbeouts.FirstOrDefault<BY_OrderDetailBeOut>().CustomerCode);
                foreach (BY_OrderDetailBeOut item in by_orderdetailbeouts)
                {
                    item.BillStatus = "委外生产中";
                    item.Modify(item.OrderDetailOutId);
                    repositoryfactory.Repository().Update(item, isOpenTrans);
                }
                var JsonData = new
                {
                    OrderDetail = by_orderdetailbeouts.OrderBy(p => p.MnemonicCode).ThenBy(p=>p.LineCode),
                    OrderDetailTotal = by_orderdetailbeouts.Sum(p => p.Qty),
                    OrderDetailCost = by_orderdetailbeouts.Sum(p => p.CostAmount),
                    //Customer = base_customer,
                };
                
                database.Commit();
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
            
        }
        /// <summary>
        /// 精料单打印
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult BeOutPrintList()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult AllFrom()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult Update(BY_OrderDetailBeOut entity, string OrderCode = null, string OrderDetailOutIds = null)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                if (!string.IsNullOrEmpty(OrderCode))
                {
                    List<BY_OrderDetailBeOut> by_orderdetailbeouts = DataFactory.Database().FindList<BY_OrderDetailBeOut>("OrderCode",OrderCode);
                    foreach (BY_OrderDetailBeOut item in by_orderdetailbeouts)
                    {
                        item.BeOutCompleteDate = DateTime.Now;
                        item.BeOutCompletePerson = DNS.Utilities.ManageProvider.Provider.Current().UserId;
                        item.BillStatus = "验收通过";
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    database.Commit();
                    return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功！" }.ToString());
                }
                int IsOk = 0;
                if (!string.IsNullOrEmpty(entity.OrderDetailOutId))
                {
                    entity.Modify(entity.OrderDetailOutId);
                    IsOk = DataFactory.Database().Update(entity);
                }
                if (!string.IsNullOrEmpty(OrderDetailOutIds))
                {
                    string[] ids = OrderDetailOutIds.Split(',');
                    BY_OrderDetailBeOut by_orderdetailbeout;
                    for (int i = 0; i < ids.Length; i++)
                    {
                        by_orderdetailbeout = new BY_OrderDetailBeOut();
                        CommonHelper.CopySameFieldsObject<BY_OrderDetailBeOut, BY_OrderDetailBeOut>(entity, ref by_orderdetailbeout);
                        by_orderdetailbeout.BeOutCompleteDate = DateTime.Now;
                        by_orderdetailbeout.BeOutCompletePerson = DNS.Utilities.ManageProvider.Provider.Current().UserId;
                        by_orderdetailbeout.Modify(ids[i]);
                        DataFactory.Database().Update(by_orderdetailbeout, isOpenTrans);
                    }
                    database.Commit();
                    return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功！" }.ToString());
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "操作成功！" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        /// <summary>
        /// 粗料单打印
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult Print()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult ChangePrice()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult SubmitForm1(BY_OrderDetailBeOut entity, string KeyValue, string audit, string s)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (string.IsNullOrEmpty(s) && KeyValue.IndexOf(',')>0)
                {
                    string[] ids = KeyValue.Split(',');
                    for (int i = 0; i < ids.Length; i++)
                    {
                        BY_OrderDetailBeOut by_orderdetailbeout = DataFactory.Database().FindEntity<BY_OrderDetailBeOut>(ids[i]);
                        if (by_orderdetailbeout != null && by_orderdetailbeout.OrderDetailOutId != null)
                        {
                            by_orderdetailbeout.AreaPrice = entity.AreaPrice;
                            by_orderdetailbeout.JJtype = entity.JJtype;

                            by_orderdetailbeout = ReturnCost(by_orderdetailbeout);
                            if (by_orderdetailbeout.AmerceAmount > 0)
                            {
                                by_orderdetailbeout.CostAmount = -by_orderdetailbeout.AmerceAmount;
                            }
                            by_orderdetailbeout.Modify(by_orderdetailbeout.OrderDetailOutId);
                            repositoryfactory.Repository().Update(by_orderdetailbeout, isOpenTrans);
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(KeyValue))
                {
                    if (!string.IsNullOrEmpty(audit))
                    {
                        Message = "验收通过";
                        entity.BeOutCompleteDate = DateTime.Now;
                        entity.BeOutCompletePerson = DNS.Utilities.ManageProvider.Provider.Current().UserName;
                        entity.BillStatus = "验收通过";
                    }
                    if (entity.AmerceAmount > 0)
                    {
                        entity.BillStatus = "不合格";
                    }
                    entity.Modify(KeyValue);
                    repositoryfactory.Repository().Update(entity, isOpenTrans);
                }
                else if (!string.IsNullOrEmpty(s))
                {
                    string[] qos = s.Trim(',').Split(',');
                    string temps = "";
                    for (int i = 0; i < qos.Length; i++)
                    {
                        temps += "'" + qos[i] + "',";
                    }
                    temps = temps.Trim(',');
                    IEnumerable<BY_OrderDetailProcess> by_orderdetailprocesssY = database.FindList<BY_OrderDetailProcess>(string.Format(" and OrderDetailId in ({0})", temps)).Where<BY_OrderDetailProcess>(p => p.TechnologyId != "K001");

                    for (int i = 0; i < qos.Length; i++)
                    {
                        BY_OrderDetailBeOut by_orderdetailbeout = new BY_OrderDetailBeOut();
                        string OrderDetailId = qos[i].Split(',')[0];
                        by_orderdetailbeout.Create();
                        BY_OrderDetail by_orderdetail = database.FindEntity<BY_OrderDetail>(OrderDetailId);
                        BY_QuotationDetail by_quotationdetail = database.FindEntity<BY_QuotationDetail>(by_orderdetail.QuotationDetailId);
                        if (by_orderdetail.OrderDetailId == null || by_quotationdetail.QuotationDetailId == null)
                        {
                            database.Rollback();
                            return Content(new JsonMessage { Success = true, Code = "-1", Message = "委外失败，请联系管理员" }.ToString());
                        }
                        by_orderdetail.IsBeOut = 1;
                        CommonHelper.CopySameFieldsObject<BY_OrderDetail, BY_OrderDetailBeOut>(by_orderdetail, ref by_orderdetailbeout);
                        CommonHelper.CopySameFieldsObject<BY_QuotationDetail, BY_OrderDetailBeOut>(by_quotationdetail, ref by_orderdetailbeout);
                        CommonHelper.CopySameFieldsObject<BY_OrderDetailBeOut, BY_OrderDetailBeOut>(entity, ref by_orderdetailbeout);
                        database.Update<BY_OrderDetail>(by_orderdetail, isOpenTrans);
                        by_orderdetailbeout.BillStatus = "打印中";
                        by_orderdetailbeout.AreaPrice = entity.AreaPrice;
                        by_orderdetailbeout.CostAmount = by_orderdetailbeout.AreaPrice * by_orderdetailbeout.Area;
                        //by_orderdetailbeout.Remark = entity.Remark;
                        by_orderdetailbeout.JJtype = entity.JJtype;
                        by_orderdetailbeout = ReturnCost(by_orderdetailbeout);

                        Hashtable ht = new Hashtable();
                        ht.Add("OrderCode", by_orderdetailbeout.OrderCode);
                        ht.Add("LineCode", by_orderdetailbeout.LineCode);
                        repositoryfactory.Repository().Delete("BY_OrderDetailBeOut", ht, isOpenTrans);
                        repositoryfactory.Repository().Insert(by_orderdetailbeout, isOpenTrans);
                        //返修
                        IEnumerable<BY_OrderDetailProcess> by_orderdetailprocesss = by_orderdetailprocesssY.Where<BY_OrderDetailProcess>(p=>p.OrderDetailId== by_orderdetail.OrderDetailId);
                        foreach (var item in by_orderdetailprocesss)
                        {
                            DataFactory.Database().Delete(item, isOpenTrans);
                        }
                        BY_OrderDetailProcess by_orderdetailprocess5 = new BY_OrderDetailProcess();
                        by_orderdetailprocess5.OrderDetailId = by_orderdetail.OrderDetailId;
                        by_orderdetailprocess5.OrderCode = by_orderdetail.OrderCode;
                        by_orderdetailprocess5.LineCode = by_orderdetail.LineCode;
                        by_orderdetailprocess5.TechnologyId = "FX01";
                        by_orderdetailprocess5.TechnologyName = "返修";
                        by_orderdetailprocess5.SortCode = 30;
                        by_orderdetailprocess5.Qty = by_orderdetail.Qty;
                        by_orderdetailprocess5.Length = by_orderdetail.Length;
                        by_orderdetailprocess5.Weigh = by_orderdetail.Weigh;
                        by_orderdetailprocess5.Thick = by_orderdetail.Thick;
                        by_orderdetailprocess5.SteelId = by_orderdetail.SteelId;
                        by_orderdetailprocess5.SteelCode = by_orderdetail.SteelCode;
                        by_orderdetailprocess5.SteelDescription = by_orderdetail.SteelDescription;
                        by_orderdetailprocess5.SteelOtherDescription = by_orderdetail.SteelOtherDescription;
                        by_orderdetailprocess5.SteelState = by_orderdetail.SteelState;
                        by_orderdetailprocess5.OrderType = by_orderdetail.OrderType;
                        by_orderdetailprocess5.Create();
                        DataFactory.Database().Insert<BY_OrderDetailProcess>(by_orderdetailprocess5, isOpenTrans);
                    }
                    Message = "委外成功！";
                }
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1".ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        public BY_OrderDetailBeOut ReturnCost(BY_OrderDetailBeOut by_orderdetailbeout)
        {
            //理论重量=（厚计价余量厚 * 宽+计价余量宽 * 高+计价余量高）*密度*数量

            decimal? Area = 0;

            if (by_orderdetailbeout.OrderType == "光板加工")
            {
                if (by_orderdetailbeout.Thick < 60)
                {
                    Area = (by_orderdetailbeout.Weigh * by_orderdetailbeout.Length * 2) / 1000000;
                }
                else if (by_orderdetailbeout.Thick >= 60 && by_orderdetailbeout.Thick < 80)
                {
                    Area = (by_orderdetailbeout.Weigh * by_orderdetailbeout.Length + by_orderdetailbeout.Length * by_orderdetailbeout.Thick) * 2  / 1000000;
                }
                else
                {
                    Area = (by_orderdetailbeout.Thick * by_orderdetailbeout.Weigh + by_orderdetailbeout.Thick * by_orderdetailbeout.Length + by_orderdetailbeout.Weigh * by_orderdetailbeout.Length) * 2  / 1000000;
                }
            }
            if (by_orderdetailbeout.OrderType == "精板加工")
            {
                Area = (by_orderdetailbeout.Thick * by_orderdetailbeout.Weigh + by_orderdetailbeout.Thick * by_orderdetailbeout.Length + by_orderdetailbeout.Weigh * by_orderdetailbeout.Length) * 2  / 1000000;
            }
            by_orderdetailbeout.Area = Area;
            decimal? CostAmount = 0;
            if (by_orderdetailbeout.JJtype == "按公斤")
            {
                if (by_orderdetailbeout.SteelType == "普通钢")
                {
                    if (by_orderdetailbeout.Weight * by_orderdetailbeout.AreaPrice / by_orderdetailbeout.Qty < 6)
                    {
                        CostAmount = 6 * by_orderdetailbeout.Qty;
                    }
                    else
                    {
                        CostAmount = by_orderdetailbeout.Weight * by_orderdetailbeout.AreaPrice;
                    }
                }
                if (by_orderdetailbeout.SteelType == "特殊钢")
                {
                    if (by_orderdetailbeout.Weight * by_orderdetailbeout.AreaPrice / by_orderdetailbeout.Qty < 7)
                    {
                        CostAmount = 7 * by_orderdetailbeout.Qty;
                    }
                    else
                    {
                        CostAmount = by_orderdetailbeout.Weight * by_orderdetailbeout.AreaPrice;
                    }
                }
            }
            else if (by_orderdetailbeout.JJtype == "按件数")
            {
                if (by_orderdetailbeout.SteelType == "普通钢")
                {
                    if (  by_orderdetailbeout.AreaPrice < 6)
                    {
                        CostAmount = 6 * by_orderdetailbeout.Qty;
                    }
                    else
                    {
                        CostAmount = by_orderdetailbeout.Qty * by_orderdetailbeout.AreaPrice;
                    }
                }
                if (by_orderdetailbeout.SteelType == "特殊钢")
                {

                    if (  by_orderdetailbeout.AreaPrice < 7)
                    {
                        CostAmount = 7 * by_orderdetailbeout.Qty;
                    }
                    else
                    {
                        CostAmount = by_orderdetailbeout.Qty * by_orderdetailbeout.AreaPrice;
                    }
                }
            }
            else
            {
                if (by_orderdetailbeout.SteelType == "普通钢")
                {

                    if (by_orderdetailbeout.Area * by_orderdetailbeout.AreaPrice < 6)
                    {
                        CostAmount = 6 * by_orderdetailbeout.Qty;
                    }
                    else
                    {
                        CostAmount = by_orderdetailbeout.Area * by_orderdetailbeout.AreaPrice * by_orderdetailbeout.Qty;
                    }
                }
                if (by_orderdetailbeout.SteelType == "特殊钢")
                {
                    if (by_orderdetailbeout.Area * by_orderdetailbeout.AreaPrice < 7)
                    {
                        CostAmount = 7 * by_orderdetailbeout.Qty;
                    }
                    else
                    {
                        CostAmount = by_orderdetailbeout.Area * by_orderdetailbeout.AreaPrice * by_orderdetailbeout.Qty;
                    }
                }
            }
            if (by_orderdetailbeout.AmerceAmount > 0)
            {
                CostAmount = -by_orderdetailbeout.AmerceAmount;
            }

            by_orderdetailbeout.CostAmount = CostAmount ;
            
            return by_orderdetailbeout;
        }
        

    }
}