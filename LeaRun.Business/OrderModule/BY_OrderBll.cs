//=====================================================================================
// All Rights Reserved , Copyright @ DNS 2015
// Software Developers @ DNS 2015
//=====================================================================================

using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Linq;
using System;

namespace DNS.Business
{
    /// <summary>
    /// 订单主表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.20 17:04</date>
    /// </author>
    /// </summary>
    public class BY_OrderBll : RepositoryFactory<BY_Order>
    {
        /// <summary>
        /// 订单查询列表
        /// </summary>
        /// <param name="Condition">查询条件</param>
        /// <param name="jqgridparam">分页参数</param>
        /// <returns></returns>
        public DataTable GetQuotDetailList(Hashtable Condition)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  CONVERT(VARCHAR(100), o.CreateDate, 23) AS '下单日期' ,
''''+q.CustomerCode '客户编码',
        o.MnemonicCode '助记码' ,qd.Delivery '交期' ,
        o.CustomerName '客户名称' ,
        o.shortname '客户简称' ,
        qd.ProductName '产品名称' ,
        qd.SteelDescription '钢种' ,
        CASE WHEN qd.Weigh > 0
             THEN  CAST(CONVERT(FLOAT, qd.Thick) AS VARCHAR(50)) + '*'
                  + CAST(CONVERT(FLOAT, qd.Weigh) AS VARCHAR(50)) + '*'
                  + CAST(CONVERT(FLOAT, qd.Length) AS VARCHAR(50))
             ELSE 'Φ' + CAST(CONVERT(FLOAT, qd.Thick) AS VARCHAR(50)) + '*'
                  + CAST(CONVERT(FLOAT, qd.Length) AS VARCHAR(50))
        END AS '规格' ,
        qd.Qty '数量' ,
qd.Thick '厚度',
        qd.Weight AS '重量' ,
        qd.OrderType '加工类型' ,
        CASE WHEN qd.IsChamfer = 1 THEN '倒角'
             ELSE ''
        END + CASE WHEN qd.CNC = 1 THEN 'CNC'
             ELSE ''
        END + CASE WHEN qd.YM = 1 THEN '退火后研磨'
             ELSE ''
        END + CASE WHEN qd.JQ = 1 THEN '锯切'
             ELSE ''
        END '特殊工序' ,
        q.CustomerCode ,
        qd.OrderCode ,
        qd.LineCode ,
    ''''+  qd.ExecRemark '公差备注' ,
        CASE WHEN q.CustMaterial = 1 THEN '是'
             ELSE '否'
        END '客来料',qd.CreateDate ,''''+ qd.customerproduct  '模号',q.ShipAddress '送货地址',q.SalesMan '业务员'
FROM    dbo.BY_OrderDetail qd
        LEFT JOIN dbo.BY_Order o ON qd.OrderCode = o.OrderCode
        LEFT JOIN dbo.BY_QuotationDetail qdd ON qd.QuotationDetailId = qdd.QuotationDetailId
        LEFT JOIN BY_Quotation Q ON q.QuotationId = o.QuotationId
        LEFT JOIN dbo.Base_Steel s ON qd.SteelCode = s.SteelCode
WHERE   1 = 1 and qd.steelcode != 'FY' AND Q.DeleteMark = 0 and o.DeleteMark=0 
         ");
            //订单单号
            if (!HashtableHelper.IsNullOrDBNull(Condition["LType"]))
            {
                strSql.Append(" AND O.LType = @LType ");
                parameter.Add(DbFactory.CreateDbParameter("@LType", Condition["LType"].ToString().Trim()));
            }
            //订单单号
            if (!HashtableHelper.IsNullOrDBNull(Condition["OrderCode"]))
            {
                strSql.Append(" AND O.OrderCode LIKE @OrderCode");
                parameter.Add(DbFactory.CreateDbParameter("@OrderCode", '%' + Condition["OrderCode"].ToString() + '%'));
            }
            //订单状态
            if (!HashtableHelper.IsNullOrDBNull(Condition["OrderStatus"]))
            {
                strSql.Append(" AND O.OrderStatus = @OrderStatus");
                parameter.Add(DbFactory.CreateDbParameter("@OrderStatus", Condition["OrderStatus"].ToString()));
            }
            //订单日期
            if (!HashtableHelper.IsNullOrDBNull(Condition["CreateDate_StartTime"]) && !HashtableHelper.IsNullOrDBNull(Condition["CreateDate_EndTime"]))
            {
                strSql.Append(" AND O.CreateDate Between @CreateDate_StartTime AND @CreateDate_EndTime ");
                parameter.Add(DbFactory.CreateDbParameter("@CreateDate_StartTime", CommonHelper.GetDateTime(Condition["CreateDate_StartTime"] + " :00")));
                parameter.Add(DbFactory.CreateDbParameter("@CreateDate_EndTime", CommonHelper.GetDateTime(Condition["CreateDate_EndTime"] + " :59")));
            }
            //报价单号
            if (!HashtableHelper.IsNullOrDBNull(Condition["QuotationCode"]))
            {
                strSql.Append(" AND Q.QuotationCode LIKE @QuotationCode");
                parameter.Add(DbFactory.CreateDbParameter("@QuotationCode", '%' + Condition["QuotationCode"].ToString() + '%'));
            }
            //客户名称
            if (!HashtableHelper.IsNullOrDBNull(Condition["ShortName"]))
            {
                strSql.Append(" AND ( o.CustomerName LIKE @ShortName or o.CustomerCode LIKE @ShortName or o.ShortName LIKE @ShortName ) ");
                parameter.Add(DbFactory.CreateDbParameter("@ShortName", '%' + Condition["ShortName"].ToString() + '%'));
            }
            //交货日期
            if (!HashtableHelper.IsNullOrDBNull(Condition["Delivery_StartTime"]) && !HashtableHelper.IsNullOrDBNull(Condition["Delivery_EndTime"]))
            {
                strSql.Append(" AND  CAST(LEFT(q.Delivery, 10) AS DATETIME) BETWEEN @Delivery_StartTime AND @Delivery_EndTime");
                parameter.Add(DbFactory.CreateDbParameter("@Delivery_StartTime", CommonHelper.GetDateTime(Condition["Delivery_StartTime"] + " :00")));
                parameter.Add(DbFactory.CreateDbParameter("@Delivery_EndTime", CommonHelper.GetDateTime(Condition["Delivery_EndTime"] + " :59")));
            }
            if (DNS.Utilities.ManageProvider.Provider.Current().IsMobileUser && DNS.Utilities.ManageProvider.Provider.Current().ObjectId.IndexOf("20178431-3604-46b6-a75b-f7ba13548aed") < 0)
            {
                strSql.Append(" AND O.CustomerCode=@CustomerCode ");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerCode", DNS.Utilities.ManageProvider.Provider.Current().UserId));
            }
            //报价单号
            if (!HashtableHelper.IsNullOrDBNull(Condition["MnemonicCode"]))
            {
                strSql.Append(" AND ( O.MnemonicCode LIKE '%'+@MnemonicCode+'%' or O.QuotationCode LIKE '%'+@MnemonicCode+'%') ");
                parameter.Add(DbFactory.CreateDbParameter("@MnemonicCode", Condition["MnemonicCode"].ToString().Trim()));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["CustomerCode"]))
            {
                strSql.Append(" AND (O.CustomerCode LIKE '%'+@CustomerCode+'%' or  O.ShortName LIKE '%'+@CustomerCode+'%' or O.CustomerName LIKE '%'+@CustomerCode+'%' )");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerCode", Condition["CustomerCode"].ToString().Trim()));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["CSstatus"]))
            {
                if (Condition["CSstatus"].ToString() == "1")
                {
                    strSql.Append(@" and OrderStatus != '已取消'  AND EXISTS (SELECT  1
                                                        FROM    dbo.BY_OrderDetail od
                                                        WHERE   (od.BillStatus = '正常生产'
                                                                OR od.BillStatus = '部分完工'
                                                                OR od.BillStatus = '部分发运')
                                                                AND DATEADD(day, 1, o.Delivery) < SYSDATETIME() AND od.OrderDetailId = qd.OrderDetailId   )
     
            
");
                }
            }

            strSql.Append("  order by o.CustomerName,qd.ProductName ,qd.Thick");

            return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
        }

        /// <summary>
        /// 订单查询列表
        /// </summary>
        /// <param name="Condition">查询条件</param>
        /// <param name="jqgridparam">分页参数</param>
        /// <returns></returns>
        public DataTable GetOrderQueryList(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  O.* ,
                                    q.CustMaterial,q.ShipDescription,(SELECT top 1 CreateDate FROM BY_Delivery d WHERE d.OrderId = o.OrderId  and d.DeleteMark = 0  ) ShipCreateDate
,(SELECT STUFF((SELECT '，'+bf.Status FROM base_file bf where bf.AttachId = Q.QuotationCode for xml path('')),1,1,'') ) [fileattach]
                            FROM    BY_Order O
                                    LEFT JOIN BY_Quotation Q ON Q.QuotationId = O.QuotationId WHERE 1=1  and o.DeleteMark=0  AND Q.DeleteMark = 0 ");
            //订单单号
            if (!HashtableHelper.IsNullOrDBNull(Condition["OrderCode"]))
            {
                strSql.Append(" AND O.OrderCode LIKE @OrderCode");
                parameter.Add(DbFactory.CreateDbParameter("@OrderCode", '%' + Condition["OrderCode"].ToString() + '%'));
            }
            //订单状态
            if (!HashtableHelper.IsNullOrDBNull(Condition["OrderStatus"]))
            {
                strSql.Append(" AND O.OrderStatus = @OrderStatus");
                parameter.Add(DbFactory.CreateDbParameter("@OrderStatus", Condition["OrderStatus"].ToString()));
            }
            //订单日期
            if (!HashtableHelper.IsNullOrDBNull(Condition["CreateDate_StartTime"]) && !HashtableHelper.IsNullOrDBNull(Condition["CreateDate_EndTime"]))
            {
                strSql.Append(" AND O.CreateDate Between @CreateDate_StartTime AND @CreateDate_EndTime ");
                parameter.Add(DbFactory.CreateDbParameter("@CreateDate_StartTime", CommonHelper.GetDateTime(Condition["CreateDate_StartTime"] + " :00")));
                parameter.Add(DbFactory.CreateDbParameter("@CreateDate_EndTime", CommonHelper.GetDateTime(Condition["CreateDate_EndTime"] + " :59")));
            }
            //报价单号
            if (!HashtableHelper.IsNullOrDBNull(Condition["QuotationCode"]))
            {
                strSql.Append(" AND Q.QuotationCode LIKE @QuotationCode");
                parameter.Add(DbFactory.CreateDbParameter("@QuotationCode", '%' + Condition["QuotationCode"].ToString() + '%'));
            }
            //客户名称
            if (!HashtableHelper.IsNullOrDBNull(Condition["ShortName"]))
            {
                strSql.Append(" AND ( o.CustomerName LIKE @ShortName or o.CustomerCode LIKE @ShortName or o.ShortName LIKE @ShortName ) ");
                parameter.Add(DbFactory.CreateDbParameter("@ShortName", '%' + Condition["ShortName"].ToString() + '%'));
            }
            //交货日期
            if (!HashtableHelper.IsNullOrDBNull(Condition["Delivery_StartTime"]) && !HashtableHelper.IsNullOrDBNull(Condition["Delivery_EndTime"]))
            {
                strSql.Append(" AND  CAST(LEFT(q.Delivery, 10) AS DATETIME) BETWEEN @Delivery_StartTime AND @Delivery_EndTime");
                parameter.Add(DbFactory.CreateDbParameter("@Delivery_StartTime", CommonHelper.GetDateTime(Condition["Delivery_StartTime"] + " :00")));
                parameter.Add(DbFactory.CreateDbParameter("@Delivery_EndTime", CommonHelper.GetDateTime(Condition["Delivery_EndTime"] + " :59")));
            }
            //报价单号
            if (!HashtableHelper.IsNullOrDBNull(Condition["MnemonicCode"]))
            {
                strSql.Append(" AND ( O.MnemonicCode LIKE '%'+@MnemonicCode+'%' or O.QuotationCode LIKE '%'+@MnemonicCode+'%') ");
                parameter.Add(DbFactory.CreateDbParameter("@MnemonicCode",Condition["MnemonicCode"].ToString().Trim()));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["CustomerCode"]))
            {
                strSql.Append(" AND (O.CustomerCode LIKE '%'+@CustomerCode+'%' or  O.ShortName LIKE '%'+@CustomerCode+'%' or O.CustomerName LIKE '%'+@CustomerCode+'%' )");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerCode", Condition["CustomerCode"].ToString().Trim()));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["CustomerPO"]))
            {
                strSql.Append(" AND (O.CustomerPO LIKE '%'+@CustomerPO+'%' )");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerPO", Condition["CustomerPO"].ToString().Trim()));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["DepartName"]))
            {
                strSql.Append(" AND (q.DepartName LIKE '%'+@DepartName+'%' )");
                parameter.Add(DbFactory.CreateDbParameter("@DepartName", Condition["DepartName"].ToString().Trim()));
            }
            
            if (!HashtableHelper.IsNullOrDBNull(Condition["CSstatus"]))
            {
                if (Condition["CSstatus"].ToString() == "1")
                {
                    strSql.Append(@" and OrderStatus != '已取消'  AND EXISTS (SELECT  1
                                                        FROM    dbo.BY_OrderDetail od
                                                        WHERE   (od.BillStatus = '正常生产'
                                                                OR od.BillStatus = '部分完工'
                                                                OR od.BillStatus = '部分发运')
                                                                AND DATEADD(day, 1, o.Delivery) < SYSDATETIME() AND od.OrderId = o.OrderId   )
     
            
");
                }
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["CustomerProduct"]))
            {
                strSql.Append(@" and o.orderid in ( SELECT orderid FROM dbo.BY_OrderDetail od WHERE od.CustomerProduct LIKE '%'+@CustomerProduct+'%' ) ");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerProduct", Condition["CustomerProduct"].ToString().Trim()));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["LType"]))
            {
                strSql.Append(" AND O.LType = @LType ");
                parameter.Add(DbFactory.CreateDbParameter("@LType", Condition["LType"].ToString().Trim()));
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        /// <summary>
        /// 订单打印列表
        /// </summary>
        /// <param name="Condition">查询条件</param>
        /// <param name="jqgridparam">分页参数</param>
        /// <returns></returns>
        public List<BY_Order> GetOrderPrintList(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT * FROM BY_Order o WHERE 1=1  and o.DeleteMark=0 ");
            //订单单号
            if (!HashtableHelper.IsNullOrDBNull(Condition["OrderCode"]))
            {
                strSql.Append(" AND OrderCode LIKE @OrderCode");
                parameter.Add(DbFactory.CreateDbParameter("@OrderCode", '%' + Condition["OrderCode"].ToString() + '%'));
            }
            //订单日期
            if (!HashtableHelper.IsNullOrDBNull(Condition["CreateDate_StartTime"]) && !HashtableHelper.IsNullOrDBNull(Condition["CreateDate_EndTime"]))
            {
                strSql.Append(" AND CreateDate Between @CreateDate_StartTime AND @CreateDate_EndTime ");
                parameter.Add(DbFactory.CreateDbParameter("@CreateDate_StartTime", CommonHelper.GetDateTime(Condition["CreateDate_StartTime"] + " :00")));
                parameter.Add(DbFactory.CreateDbParameter("@CreateDate_EndTime", CommonHelper.GetDateTime(Condition["CreateDate_EndTime"] + " :59")));
            }
            //订单类型
            if (!HashtableHelper.IsNullOrDBNull(Condition["OrderType"]))
            {
                strSql.Append(" AND OrderType = @OrderType");
                parameter.Add(DbFactory.CreateDbParameter("@OrderType", Condition["OrderType"].ToString()));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["MnemonicCode"]))
            {
                strSql.Append(" AND (MnemonicCode LIKE '%'+@MnemonicCode+'%' or QuotationCode LIKE '%'+@MnemonicCode+'%') ");
                parameter.Add(DbFactory.CreateDbParameter("@MnemonicCode", Condition["MnemonicCode"].ToString().Trim()));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["CustomerName"]))
            {
                strSql.Append(" AND (O.CustomerCode LIKE '%'+@CustomerName+'%' or  O.ShortName LIKE '%'+@CustomerName+'%' or O.CustomerName LIKE '%'+@CustomerName+'%' )");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerName", Condition["CustomerName"].ToString().Trim()));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["LType"]))
            {
                strSql.Append(" AND O.LType = @LType ");
                parameter.Add(DbFactory.CreateDbParameter("@LType", Condition["LType"].ToString().Trim()));
            }
            return Repository().FindListPageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        /// <summary>
        /// 订单状态变更列表
        /// </summary>
        /// <param name="Condition">查询条件</param>
        /// <param name="jqgridparam">分页参数</param>
        /// <returns></returns>
        public List<BY_Order> GetOrderStatusList(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT * FROM BY_Order WHERE 1=1  and DeleteMark=0 ");
            //订单单号
            if (!HashtableHelper.IsNullOrDBNull(Condition["OrderCode"]))
            {
                strSql.Append(" AND OrderCode LIKE @OrderCode");
                parameter.Add(DbFactory.CreateDbParameter("@OrderCode", '%' + Condition["OrderCode"].ToString() + '%'));
            }
            //订单日期
            if (!HashtableHelper.IsNullOrDBNull(Condition["CreateDate_StartTime"]) && !HashtableHelper.IsNullOrDBNull(Condition["CreateDate_EndTime"]))
            {
                strSql.Append(" AND CreateDate Between @CreateDate_StartTime AND @CreateDate_EndTime ");
                parameter.Add(DbFactory.CreateDbParameter("@CreateDate_StartTime", CommonHelper.GetDateTime(Condition["CreateDate_StartTime"] + " :00")));
                parameter.Add(DbFactory.CreateDbParameter("@CreateDate_EndTime", CommonHelper.GetDateTime(Condition["CreateDate_EndTime"] + " :59")));
            }
            return Repository().FindListPageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        /// <summary>
        /// 根据 订单号+行号 获取明细对象
        /// </summary>
        /// <param name="BarCode">条码</param>
        /// <returns></returns>
        public DataTable GetOrderDetailEntity(string BarCode)
        {
            string[] arr = BarCode.Split('-');
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  od.* ,
                            qd.SalesAmount ,
                            o.CustomerCode ,
                            o.CustomerName
                    FROM    BY_OrderDetail od
                            INNER JOIN BY_Order o ON od.OrderId = o.OrderId
                            LEFT JOIN dbo.BY_QuotationDetail qd ON o.QuotationId = qd.QuotationId
                                                                   AND qd.LineCode = od.LineCode
                    WHERE   (o.OrderCode = @OrderCode or qd.QuotationCode = @OrderCode) and o.DeleteMark=0 
                            AND od.LineCode = @LineCode");
            List<DbParameter> parameter = new List<DbParameter>();
            parameter.Add(DbFactory.CreateDbParameter("@OrderCode", arr[0] + "-" + arr[1]));
            parameter.Add(DbFactory.CreateDbParameter("@LineCode", arr[2]));
            return DataFactory.Database().FindTableBySql(strSql.ToString(), parameter.ToArray());
        }
        /// <summary>
        /// 根据 订单号+行号 获取明细对象
        /// </summary>
        /// <param name="BarCode">条码</param>
        /// <returns></returns>
        public DataTable GetOrderDetailEntityForGB(string BarCode)
        {
            string[] arr = BarCode.Split('-');
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  od.* ,
        ISNULL(( SELECT SUM(Qty)
                 FROM   BY_OrderDetailProcessDetail
                 WHERE  OrderCode = od.OrderCode
                        AND LineCode = od.LineCode
                        AND IsClear = 0
                        AND (TechnologyId = 'K001' or TechnologyName = '锯床' or TechnologyId = 'FG' or TechnologyName = '风割' )
               ), 0) Qty1 ,
        ISNULL(( SELECT SUM(Weight)
                 FROM   BY_OrderDetailProcessDetail
                 WHERE  OrderCode = od.OrderCode
                        AND LineCode = od.LineCode
                        AND IsClear = 0
                        AND (TechnologyId = 'K001' or TechnologyName = '锯床' or TechnologyId = 'FG' or TechnologyName = '风割' )
               ), 0) Weight1 ,
        o.CustomerCode ,
        o.CustomerName ,
        o.mnemoniccode
FROM    BY_OrderDetail od
        INNER JOIN BY_Order o ON od.OrderId = o.OrderId
                    WHERE   o.OrderCode = @OrderCode and o.DeleteMark=0 
                            AND od.LineCode = @LineCode");

            List<DbParameter> parameter = new List<DbParameter>();
            parameter.Add(DbFactory.CreateDbParameter("@OrderCode", arr[0] + "-" + arr[1]));
            parameter.Add(DbFactory.CreateDbParameter("@LineCode", arr[2]));
            return DataFactory.Database().FindTableBySql(strSql.ToString(), parameter.ToArray());
        }
        /// <summary>
        /// 根据 订单号+行号 获取明细对象
        /// </summary>
        /// <param name="BarCode">条码</param>
        /// <returns></returns>
        public DataTable GetOrderDetailScaleJson(string BarCode)
        {
            string[] arr = BarCode.Split('-');
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@" SELECT  od.SteelId ,od.OpenQty,
        od.OrderDetailId ,
        od.SteelCode ,
        od.SteelState ,
        od.Thick ,
        od.Length ,
        od.Weigh ,
        od.Qty ,
        od.LineCode ,
        od.completeqty ,od.Weight,
        qd.SalesAmount ,
        o.CustomerCode ,
        o.CustomerName ,
        o.Delivery ,
        
        o.OrderCode ,
        --i.BatchCode AS BrokenSteel ,
        --i.Weight StockWeight,
        od.SteelDescription,od.OrderType,
        (ISNULL((SELECT SUM(odp.Weight) FROM dbo.BY_OrderDetailProcessDetail odp
WHERE odp.OrderCode = od.OrderCode AND odp.LineCode = od.LineCode AND (odp.TechnologyId = 'K001' or odp.TechnologyName = '锯床' or odp.TechnologyId = 'FG' or odp.TechnologyName = '风割' ) AND odp.IsCKXS = 0),0)) as ClearWeight,
(ISNULL((SELECT SUM(odp.Qty) FROM dbo.BY_OrderDetailProcessDetail odp
WHERE odp.OrderCode = od.OrderCode AND odp.LineCode = od.LineCode AND (odp.TechnologyId = 'K001' or odp.TechnologyName = '锯床' or odp.TechnologyId = 'FG' or odp.TechnologyName = '风割' ) AND odp.IsCKXS = 0),0)) as ClearQty
FROM    BY_OrderDetail od
        INNER JOIN BY_Order o ON od.OrderId = o.OrderId
        LEFT JOIN dbo.BY_QuotationDetail qd ON o.QuotationId = qd.QuotationId
                                               AND qd.LineCode = od.LineCode 
                                               --LEFT JOIN dbo.BY_Inventory i ON od.SteelCode = i.BatchCode
                    WHERE o.OrderCode = @OrderCode and o.DeleteMark=0 
                            AND od.LineCode = @LineCode
                            --and od.BillStatus != '已发运'
");
            List<DbParameter> parameter = new List<DbParameter>();
            parameter.Add(DbFactory.CreateDbParameter("@OrderCode", arr[0] + "-" + arr[1]));
            parameter.Add(DbFactory.CreateDbParameter("@LineCode", arr[2]));
            DataTable dt = DataFactory.Database().FindTableBySql(strSql.ToString(), parameter.ToArray());
            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    Base_DataDictionaryDetail base_datadictionarydetail = DataFactory.Database().FindEntity<Base_DataDictionaryDetail>("FullName", dt.Rows[0]["SteelState"].ToString());
            //    BY_Inventory by_inventory = DataFactory.Database().FindEntityBySql<BY_Inventory>(string.Format(" select top 1 * from BY_Inventory where BatchCode like '{0}-%{1}'", dt.Rows[0]["SteelCode"].ToString(), base_datadictionarydetail.Code));
            //    dt.Rows[0]["BrokenSteel"] = CommonHelper.GetString(by_inventory.BatchCode);
            //    dt.Rows[0]["StockWeight"] = CommonHelper.GetDecimal(by_inventory.Weight);
            //}
            return dt;
        }

        /// <summary>
        /// 获取还没有完成发货的订单
        /// </summary>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GetNoCompleteOrders(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            List<DbParameter> parameter = new List<DbParameter>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@" SELECT  O.OrderId ,
        o.OrderCode ,
        
        o.ShortName ,
        o.OrderStatus ,
        o.CustomerName ,
        o.MnemonicCode
FROM    BY_Order o
LEFT JOIN dbo.BY_Quotation q ON o.OrderCode=q.OrderCode
WHERE   o.OrderId IN ( SELECT   OrderId
                           FROM     dbo.BY_OrderComplete
                           WHERE    DeliveryCode IS NULL or DeliveryCode = '') and o.CustomerName != '内部下单'  AND Q.DeleteMark = 0  and o.DeleteMark=0  ");

           
            if (!HashtableHelper.IsNullOrDBNull(Condition["OrderCode"]))
            {
                strSql.Append(" AND o.OrderCode LIKE @OrderCode ");
                parameter.Add(DbFactory.CreateDbParameter("@OrderCode", '%' + Condition["OrderCode"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["QuotationCode"]))
            {
                strSql.Append(" AND q.QuotationCode LIKE @QuotationCode ");
                parameter.Add(DbFactory.CreateDbParameter("@QuotationCode", '%' + Condition["QuotationCode"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["CheckBoxComplete"]) && Condition["CheckBoxComplete"].ToString() == "1")
            {
                strSql.Append(" AND o.OrderStatus='已完工' ");
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["MnemonicCode"]))
            {
                strSql.Append(" AND ( o.MnemonicCode LIKE '%' +@MnemonicCode+'%' or o.QuotationCode LIKE '%' +@MnemonicCode+'%' ) ");
                parameter.Add(DbFactory.CreateDbParameter("@MnemonicCode", Condition["MnemonicCode"].ToString().Trim()));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["LType"]))
            {
                strSql.Append(" AND o.LType  = @LType");
                parameter.Add(DbFactory.CreateDbParameter("@LType", Condition["LType"].ToString().Trim()));
            }
            //strSql.Append(" ORDER BY o.Delivery DESC ");

            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        /// <summary>
        /// 获取完成但没有发运的订单详情
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public DataTable GetNoCompleteOrdersDetails(string OrderId)
        {
            string strSql = @"         SELECT  d.BoardDescription ,
                d.SteelCode ,
                CASE WHEN D.Length > 0
                     THEN CASE WHEN d.Weigh = 0
                               THEN 'Φ'
                                    + CAST(CONVERT(FLOAT, d.Thick) AS VARCHAR(10))
                                    + '*'
                                    + CAST(CONVERT(FLOAT, d.Length) AS VARCHAR(10))
                               ELSE CAST(CONVERT(FLOAT, d.Thick) AS VARCHAR(10))
                                    + '*'
                                    + CAST(CONVERT(FLOAT, d.Weigh) AS VARCHAR(10))
                                    + '*'
                                    + CAST(CONVERT(FLOAT, d.Length) AS VARCHAR(10))
                          END
                     ELSE ''
                END AS SIZE ,--尺寸 AS  ,
                d.Qty ,
                c.Qty AS CompleteQty ,
                c.TheoryWeight ,
                c.Weight ,
                d.OrderType ,
                d.LineCode,OrderCompleteId,d.ProductName,d.customerproduct
        FROM    BY_OrderDetail d ,
                BY_OrderComplete c
WHERE   d.OrderDetailId = c.OrderDetailId
AND (c.DeliveryCode IS NULL or c.DeliveryCode='')
AND d.OrderId = @OrderId  order by d.LineCode asc ";
            List<DbParameter> parameter = new List<DbParameter>();
            parameter.Add(DbFactory.CreateDbParameter("@OrderId", OrderId));
            return DataFactory.Database().FindTableBySql(strSql, parameter.ToArray());
        }
        /// <summary>
        /// 获取没有完成的订单详情
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public DataTable GetCompleteOrdersDetails(string OrderId)
        {
            string strSql = @"      SELECT  d.BoardDescription ,
                d.SteelCode ,
                 CASE WHEN D.Length > 0
                     THEN CASE WHEN d.Weigh = 0
                               THEN 'Φ'
                                    + CAST(CONVERT(FLOAT, d.Thick) AS VARCHAR(10))
                                    + '*'
                                    + CAST(CONVERT(FLOAT, d.Length) AS VARCHAR(10))
                               ELSE CAST(CONVERT(FLOAT, d.Thick) AS VARCHAR(10))
                                    + '*'
                                    + CAST(CONVERT(FLOAT, d.Weigh) AS VARCHAR(10))
                                    + '*'
                                    + CAST(CONVERT(FLOAT, d.Length) AS VARCHAR(10))
                          END
                     ELSE ''
                END AS SIZE ,--尺寸 AS  ,
                d.Qty ,
                d.Qty - d.CompleteQty AS NoCompleteQty ,
                d.OrderType,d.ProductName
        FROM    BY_OrderDetail d
                LEFT JOIN BY_OrderComplete c ON d.OrderDetailId = c.OrderDetailId 
WHERE   (c.DeliveryCode IS NULL or c.DeliveryCode='')  and d.Qty -d.CompleteQty != 0 
        AND d.OrderId = @OrderId  order by d.LineCode asc  ";
            List<DbParameter> parameter = new List<DbParameter>();
            parameter.Add(DbFactory.CreateDbParameter("@OrderId", OrderId));
            return DataFactory.Database().FindTableBySql(strSql, parameter.ToArray());
        }
        /// <summary>
        /// 返回订单整单状态
        /// </summary>
        /// <param name="OrderId">订单ID</param>
        /// <param name="OrderDetailID">订单详细ID</param>
        /// <param name="OrderDetailIDStatus">订单详细状态ID</param>
        /// <returns></returns>
        public string ReturnOrderStatus(string OrderId, string OrderDetailID, string OrderDetailIDStatus)
        {
            IDatabase database = DataFactory.Database();
            List<BY_OrderDetail> by_orderdetails = database.FindList<BY_OrderDetail>("OrderId", OrderId);
            int count = by_orderdetails.Count();
            //是否有取消
            int havacancel = (from entity in by_orderdetails
                              where entity.OrderDetailId != OrderDetailID && entity.BillStatus == "已取消"
                              select entity).Count();
            havacancel += OrderDetailIDStatus == "已取消" ? 1 : 0;
            if (havacancel == count)
            {
                return "已取消";
            }
            if (havacancel > 0)
            {
                return "部分取消";
            }

            //是否有暂停
            int havaPause = (from entity in by_orderdetails
                             where entity.OrderDetailId != OrderDetailID && entity.BillStatus == "暂停"
                             select entity).Count();
            havaPause += OrderDetailIDStatus == "暂停" ? 1 : 0;
            if (havaPause == count)
            {
                return "暂停";
            }
            if (havaPause > 0)
            {
                return "部分暂停";
            }
            //是否有正常生产
            int havanormal = (from entity in by_orderdetails
                              where entity.OrderDetailId != OrderDetailID && entity.BillStatus == "正常生产"
                              select entity).Count();
            havanormal += OrderDetailIDStatus == "正常生产" ? 1 : 0;
            if (havanormal == count)
            {
                return "正常生产";
            }

            //是否有已完工
            int havacompleted = (from entity in by_orderdetails
                                 where entity.OrderDetailId != OrderDetailID && entity.BillStatus == "已完工"
                                 select entity).Count();
            havacompleted += OrderDetailIDStatus == "已完工" ? 1 : 0;
            if (havacompleted == count)
            {
                return "已完工";
            }
            if (havacompleted > 0)
            {
                return "部分完工";
            }
            //是否有部分完工
            int havaNOAllcompleted = (from entity in by_orderdetails
                                      where entity.OrderDetailId != OrderDetailID && entity.BillStatus == "部分完工"
                                      select entity).Count();
            havaNOAllcompleted += OrderDetailIDStatus == "部分完工" ? 1 : 0;
            if (havaNOAllcompleted > 0)
            {
                return "部分完工";
            }

            //是否有已发运
            int havaShipment = (from entity in by_orderdetails
                                where entity.OrderDetailId != OrderDetailID && entity.BillStatus == "已发运"
                                select entity).Count();
            havaShipment += OrderDetailIDStatus == "已发运" ? 1 : 0;
            if (havaShipment == count)
            {
                return "已发运";
            }
            if (havaShipment > 0)
            {
                return "部分发运";
            }
            //是否有部分发运
            int havaNOAllShipment = (from entity in by_orderdetails
                                     where entity.OrderDetailId != OrderDetailID && entity.BillStatus == "部分发运"
                                     select entity).Count();
            havaNOAllShipment += OrderDetailIDStatus == "部分发运" ? 1 : 0;

            if (havaNOAllShipment > 0)
            {
                return "部分发运";
            }
            return "";
        }
    }
}