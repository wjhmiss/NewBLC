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
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.OrderModule.Controllers
{

    /// <summary>
    /// 订单完工扫描销数
    /// </summary>
    public class OrderCompleteController : PublicController<BY_OrderComplete>
    {
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult QueryOrder()
        {
            return View();
        }

        public ActionResult CompleteGridPageList(string Condition, JqGridParam jqgridparam)
        {
            Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
            Stopwatch watch = CommonHelper.TimerStart();
            DataTable ListData = this.GetPageList(QueryParameter, ref jqgridparam);
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
        /// <summary>
        /// 生产报表列表
        /// </summary>
        /// <param name="Condition"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GetPageList(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  o.OrderCode ,--订单号
        od.LineCode ,--行号
        o.ShortName ,--客户简称
        q.SalesArea ,  --销售区域                                 
        o.CustomerCode ,--客户编号
        od.SteelCode ,--钢种
        od.DieCode ,--模号
        CAST(od.Thick AS VARCHAR(10)) + 'X' + CAST(od.Weigh AS VARCHAR(10))
        + 'X' + CAST(od.Length AS VARCHAR(10)) AS Size ,--尺寸
        qd.Technology ,--热处理：有值勾上，空值不勾选
        od.Qty AS TOTALQTY ,
        ISNULL(OC.Qty,0) AS Qty ,--完成数量
        od.BillStatus ,--当前状态
        o.CreateDate ,--落单日期
        o.Delivery ,--要求完工期
        case when len(od.Delivery)=10 or len(od.Delivery)=9 or len(od.Delivery)=8 then
                                    - DATEDIFF(DAY,CONVERT(DATETIME,LEFT(isnull( od.Delivery,o.Delivery),10)), GETDATE())
									when len(od.Delivery)=7 then
									 - DATEDIFF(DAY,CONVERT(DATETIME,LEFT(isnull( od.Delivery,o.Delivery),10), 1), GETDATE()) end AS OverDay ,--过期天数
        oc.Weight ,--重量
        q.ShipRoute ,--路线
        q.Remark--备注
        ,
        OD.OrderType,o.MnemonicCode
FROM    dbo.BY_Order o
        INNER JOIN dbo.BY_OrderDetail od ON o.OrderId = od.OrderId
        INNER JOIN dbo.BY_QuotationDetail qd ON qd.QuotationDetailId = od.QuotationDetailId
        INNER JOIN dbo.BY_Quotation q ON q.QuotationId = qd.QuotationId
        LEFT JOIN ( SELECT oc.OrderDetailId ,
                            SUM(oc.Qty) QTY ,
                            SUM(oc.Weight) Weight
                     FROM   BY_OrderComplete oc
                     GROUP BY oc.OrderDetailId
                   ) oc ON od.OrderDetailId = oc.OrderDetailId
WHERE   1 = 1  AND Q.DeleteMark = 0 and o.DeleteMark=0 
 ");

            //订单单号
            if (!HashtableHelper.IsNullOrDBNull(Condition["OrderCode"]))
            {
                strSql.Append(" AND o.OrderCode LIKE @OrderCode");
                parameter.Add(DbFactory.CreateDbParameter("@OrderCode", '%' + Condition["OrderCode"].ToString() + '%'));
            }
            //订单日期
            if (!HashtableHelper.IsNullOrDBNull(Condition["CreateDate_StartTime"]) && !HashtableHelper.IsNullOrDBNull(Condition["CreateDate_EndTime"]))
            {
                strSql.Append(" AND o.CreateDate Between @CreateDate_StartTime AND @CreateDate_EndTime ");
                parameter.Add(DbFactory.CreateDbParameter("@CreateDate_StartTime", CommonHelper.GetDateTime(Condition["CreateDate_StartTime"] + " 00:00")));
                parameter.Add(DbFactory.CreateDbParameter("@CreateDate_EndTime", CommonHelper.GetDateTime(Condition["CreateDate_EndTime"] + " 23:59")));
            }
            //客户编号
            if (!HashtableHelper.IsNullOrDBNull(Condition["CustomerCode"]))
            {
                strSql.Append(" AND (o.CustomerCode LIKE '%'+@CustomerCode+'%' or  o.ShortName LIKE '%'+@CustomerCode+'%' or o.CustomerName LIKE '%'+@CustomerCode+'%' )");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerCode", Condition["CustomerCode"].ToString()));
            }
            //客户简称
            if (!HashtableHelper.IsNullOrDBNull(Condition["ShortName"]))
            {
                strSql.Append(" AND (o.CustomerCode LIKE '%'+@ShortName+'%' or  o.ShortName LIKE '%'+@ShortName+'%' or o.CustomerName LIKE '%'+@ShortName+'%' )");
                parameter.Add(DbFactory.CreateDbParameter("@ShortName",  Condition["ShortName"].ToString() ));
            }
            //销售区域
            if (!HashtableHelper.IsNullOrDBNull(Condition["SalesArea"]))
            {
                strSql.Append(" AND q.SalesArea LIKE @SalesArea");
                parameter.Add(DbFactory.CreateDbParameter("@SalesArea", '%' + Condition["SalesArea"].ToString() + '%'));
            }
            //
            if (!HashtableHelper.IsNullOrDBNull(Condition["OrderType"]))
            {
                strSql.Append(" AND OD.OrderType=@OrderType ");
                parameter.Add(DbFactory.CreateDbParameter("@OrderType", Condition["OrderType"].ToString()));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["MnemonicCode"]))
            {
                strSql.Append(" AND ( o.MnemonicCode LIKE '%'+@MnemonicCode+'%' or o.QuotationCode LIKE '%'+@MnemonicCode+'%' )");
                parameter.Add(DbFactory.CreateDbParameter("@MnemonicCode", Condition["MnemonicCode"].ToString().Trim()));
            }
            DataTable dt = repositoryfactory.Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
            return dt;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult ShowComplete()
        {
            return View();
        }
        BY_OrderCompleteBll by_ordercompletebll = new BY_OrderCompleteBll();
        /// <summary>
        /// 订单查询列表（返回表格JSON）
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
                DataTable ListData = by_ordercompletebll.GetOrderCompleteList(QueryParameter, ref jqgridparam);
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
        public ActionResult GridPageListJson1(string Condition, JqGridParam jqgridparam)
        {
            IDatabase database = DataFactory.Database();
                DbTransaction isOpenTrans = database.BeginTrans();
                try
                {
                    Hashtable QueryParameter = HashtableHelper.JsonToHashtable(Condition);
                    Stopwatch watch = CommonHelper.TimerStart();
                    DataTable ListData = by_ordercompletebll.GetOrderCompleteList1(QueryParameter, ref jqgridparam);

                    //if (ListData != null && ListData.Rows.Count > 0)
                    //{
                    //    for (int i = 0; i < ListData.Rows.Count; i++)
                    //    {
                    //        BY_OrderDetail by_orderdetail = database.FindEntity<BY_OrderDetail>(ListData.Rows[i]["OrderDetailId"].ToString());
                    //        by_orderdetail.ScaleUser = DNS.Utilities.ManageProvider.Provider.Current().UserId;
                    //        database.Update(by_orderdetail, isOpenTrans);
                    //    }
                    //    database.Commit();
                    //}
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

         


        /// <summary>
        /// 提交确认记录
        /// </summary>
        /// <param name="OrderCompleteJson">要确认数据</param>
        /// <returns></returns>
        public ActionResult SubmitOrderComplete(string OrderCompleteJson)
        {
            IDatabase database = DataFactory.Database();
            List<BY_Order> DataBY_OrderList = new List<BY_Order>();//从数据库中取出相关订单记录
            List<BY_OrderDetail> DataBY_OrderDetailList = new List<BY_OrderDetail>();//从数据库中取出相关订单明细记录
            List<BY_OrderComplete> DataBY_OrderCompleteList = new List<BY_OrderComplete>();//从数据库中取出相关完工记录
            List<BY_OrderComplete> DataBY_OrderCompleteList1 = new List<BY_OrderComplete>();//从数据库中取出相关完工记录
            List<BY_OrderComplete> OrderCompletelist = OrderCompleteJson.JonsToList<BY_OrderComplete>();
            List<BY_OrderComplete> InsertBY_OrderComplete = new List<BY_OrderComplete>();
            List<string> orderidList = new List<string>();
            List<string> orderdetaildidList = new List<string>();
            //遍历完工表取出所有相关的订单主键
            foreach (BY_OrderComplete entity in OrderCompletelist)
            {
                if (!orderidList.Contains(entity.OrderId))
                {
                    orderidList.Add(entity.OrderId);
                }
                orderdetaildidList.Add(entity.OrderDetailId);

            }
            string condition = " and orderid in(";
            //根据订单主键找出相关的订单明细
            foreach (string orderid in orderidList)
            {
                condition = condition + "'" + orderid + "',";
            }
            condition = condition.Substring(0, condition.Length - 1) + ")";//去除最后一个多余的逗号

            string condition1 = " and OrderDetailId in(";
            foreach (string orderdetaildi in orderdetaildidList)
            {
                condition1 = condition1 + "'" + orderdetaildi + "',";
            }
            condition1 = condition1.Substring(0, condition1.Length - 1) + ")";//去除最后一个多余的逗号


            DataBY_OrderList = database.FindListBySql<BY_Order>("select * from BY_Order where 1=1 " + condition);
            DataBY_OrderDetailList = database.FindListBySql<BY_OrderDetail>("select * from BY_OrderDetail where 1=1 " + condition);
            DataBY_OrderCompleteList = database.FindListBySql<BY_OrderComplete>("select * from BY_OrderComplete where 1=1 " + condition);
            DataBY_OrderCompleteList1 = database.FindListBySql<BY_OrderComplete>("select * from BY_OrderComplete where 1=1 " + condition);
            IList<BY_OrderDetailProcessDetail> by_orderdetailprocessdetails = database.FindListBySql<BY_OrderDetailProcessDetail>(@"SELECT  *
FROM    dbo.BY_OrderDetailProcessDetail odp
WHERE   (odp.TechnologyId = 'K001' or odp.TechnologyName = '锯床' or odp.TechnologyId = 'FG' or odp.TechnologyName = '风割' )
        AND odp.isclear = 0 " + condition1);


            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                foreach (BY_OrderDetailProcessDetail item in by_orderdetailprocessdetails)
                {
                    item.IsClear = 1;
                    database.Update<BY_OrderDetailProcessDetail>(item, isOpenTrans);
                }
                foreach (BY_OrderComplete entity in OrderCompletelist)
                {
                    //取出本条订明细已插入过订单完工表的记录
                    //BY_OrderComplete by_ordercomplete = DataBY_OrderCompleteList.Find(
                    //                                    delegate(BY_OrderComplete t)
                    //                                    {
                    //                                        return t.OrderDetailId == entity.OrderDetailId;
                    //                                    });
                    BY_OrderComplete by_ordercomplete = (from en in DataBY_OrderCompleteList
                                                        where en.OrderDetailId == entity.OrderDetailId && string.IsNullOrEmpty(en.DeliveryCode)
                                                         select en).FirstOrDefault<BY_OrderComplete>();

                    if (by_ordercomplete == null )
                    {
                        //当本订单明细未进过完工操作或者已发运时插入一条完工记录
                        DataBY_OrderCompleteList.Add(entity);
                        InsertBY_OrderComplete.Add(entity);
                    }
                    else if (by_ordercomplete != null && string.IsNullOrEmpty(by_ordercomplete.DeliveryCode))
                    {
                        //未发运时累计完工数据下次一起发运
                        entity.Modify(by_ordercomplete.OrderCompleteId);
                        entity.CompleteDate = DateTime.Now;
                        entity.CompleteUserId = ManageProvider.Provider.Current().UserName;
                        by_ordercomplete.Qty = by_ordercomplete.Qty + entity.Qty;
                        by_ordercomplete.Weight = by_ordercomplete.Weight + entity.Weight;
                        by_ordercomplete.TheoryWeight = by_ordercomplete.TheoryWeight + entity.TheoryWeight;
                        InsertBY_OrderComplete.Add(by_ordercomplete);
                    }
                    #region 处理订单明细状态
                    //取出当前完工操作对应的订单明细
                    BY_OrderDetail by_orderdetail = DataBY_OrderDetailList.Find(
                                                        delegate(BY_OrderDetail t)
                                                        {
                                                            return t.OrderDetailId == entity.OrderDetailId;
                                                        });
                    by_orderdetail.CompleteQty = CommonHelper.GetDecimal(by_orderdetail.CompleteQty) + entity.Qty;//累计已完工数
                    if (by_orderdetail.BillStatus == OrderBillStatus.部分完工.ToString() || by_orderdetail.BillStatus == OrderBillStatus.正常生产.ToString())
                    {
                        if (by_orderdetail.CompleteQty >= by_orderdetail.Qty)
                        {
                            by_orderdetail.BillStatus = OrderBillStatus.已完工.ToString();
                        }
                        else
                        {
                            by_orderdetail.BillStatus = OrderBillStatus.部分完工.ToString();
                        }
                    }

                    int? all = (from en in DataBY_OrderCompleteList1
                                                         where en.OrderDetailId == entity.OrderDetailId
                                                         select en.Qty).Sum();
                    if (all + entity.Qty > by_orderdetail.Qty)
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败:信息已更新，请重新提交！" }.ToString());
                    }
                    #endregion
                }

                #region 写入完工明细
                string strClassesShift;
                DateTime? strClassesDate;
                foreach (BY_OrderComplete entity in InsertBY_OrderComplete)
                {
                    
                    if (string.IsNullOrEmpty(entity.OrderCompleteId))
                    {
                        entity.Create();
                        entity.CompleteDate = DateTime.Now;
                        entity.CompleteUserId = ManageProvider.Provider.Current().UserName;
                        new BY_OrderDetailProcessDetailBll().GetClassesDateClassesShift(entity.CreateDate, out strClassesDate, out strClassesShift);
                        entity.ClassesDate = strClassesDate;
                        entity.ClassesShift = strClassesShift;
                        database.Insert<BY_OrderComplete>(entity, isOpenTrans);
                    }
                    else
                    {
                        entity.Modify(entity.OrderCompleteId);
                        new BY_OrderDetailProcessDetailBll().GetClassesDateClassesShift(entity.ModifyDate, out strClassesDate, out strClassesShift);
                        entity.ClassesDate = strClassesDate;
                        entity.ClassesShift = strClassesShift;
                        database.Update<BY_OrderComplete>(entity, isOpenTrans);
                    }
                }
                #endregion



                #region 向数据库更新订单行状态
                foreach (BY_OrderDetail entity in DataBY_OrderDetailList)
                {
                    database.Update<BY_OrderDetail>(entity, isOpenTrans);
                }
                #endregion

                #region 向数据库更新订单状态
                foreach (BY_Order entity in DataBY_OrderList)
                {
                    if (entity.OrderStatus != OrderBillStatus.部分发运.ToString() && entity.OrderStatus != OrderBillStatus.部分取消.ToString())
                    {
                        //找出部分完工的订单明细
                        BY_OrderDetail by_orderdetailpart = DataBY_OrderDetailList.Find(
                                    delegate(BY_OrderDetail t)
                                    {
                                        return t.OrderId == entity.OrderId && t.BillStatus == OrderBillStatus.部分完工.ToString();
                                    });
                        if (by_orderdetailpart != null)
                        {
                            //只要有一条部分完工的整个订单状态就是部分完工
                            entity.OrderStatus = OrderBillStatus.部分完工.ToString();
                        }
                        BY_OrderDetail by_orderdetailall = DataBY_OrderDetailList.Find(
                                   delegate(BY_OrderDetail t)
                                   {
                                       return t.OrderId == entity.OrderId && (t.BillStatus != OrderBillStatus.已完工.ToString());
                                   });
                        BY_OrderDetail by_orderdetailallZ = DataBY_OrderDetailList.Find(
                                   delegate(BY_OrderDetail t)
                                   {
                                       return t.OrderId == entity.OrderId && (t.BillStatus == OrderBillStatus.正常生产.ToString());
                                   });
                        BY_OrderDetail by_orderdetailallW = DataBY_OrderDetailList.Find(
                                   delegate(BY_OrderDetail t)
                                   {
                                       return t.OrderId == entity.OrderId && (t.BillStatus == OrderBillStatus.已完工.ToString());
                                   });
                        if (by_orderdetailall == null)
                        {
                            //没有一条部分完工和状态为空的整个订单状态就是完工
                            entity.OrderStatus = OrderBillStatus.已完工.ToString();
                        }
                        if (by_orderdetailallZ != null && by_orderdetailallW != null)
                        {
                            entity.OrderStatus = OrderBillStatus.部分完工.ToString();
                        }
                        database.Update<BY_Order>(entity, isOpenTrans);

                    }
                }

                

                #endregion

                database.Commit();
                foreach (BY_Order entity in DataBY_OrderList)
                {
                   new OrderQueryController().SetOrderStatus(entity.OrderCode);
                }
                return Content(new JsonMessage { Success = true, Code = "1", Message = "确认成功。" }.ToString());
            }
            catch (Exception)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败。" }.ToString());
            }
        }

    }
}
