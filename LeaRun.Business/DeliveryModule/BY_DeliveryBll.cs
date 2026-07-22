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

namespace DNS.Business
{
    /// <summary>
    /// 发运单
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.24 16:29</date>
    /// </author>
    /// </summary>
    public class BY_DeliveryBll : RepositoryFactory<BY_Delivery>
    {
        public DataTable DaoGetGridPageJson(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            List<DbParameter> parameter = new List<DbParameter>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"  SELECT CONVERT(VARCHAR(100), d.CreateDate, 23) AS '日期' ,
        '''' + d.MnemonicCode '助记码' ,
        d.DeliveryCode '送货单号' ,dd.SteelCode,SUBSTRING(dd.OrderType,1,2) OrderType,dd.Length,'*' cccc,'*'cccc1,dd.Weigh,dd.Thick,'''' + dd.CustomerProduct CustomerProduct,'''' + dd.ProjectCode ProjectCode,dd.Qty,dd.Weight,'''' + dd.ShipDescription ShipDescription,
        isnull(dd.CustomerProduct,'') + '-' + isnull(dd.ShipDescription,'') as 'CustomerProduct-ShipDescription',
        d.CustomerName '客户名称' ,        d.shortname '简称' ,

        dd.ProductName '产品名称' ,
               CASE WHEN dd.Weigh > 0 
             THEN  CAST(CONVERT(FLOAT, dd.Thick) AS VARCHAR(50))+ '*'
                  + CAST(CONVERT(FLOAT, dd.Weigh) AS VARCHAR(50)) + '*'
                  + CAST(CONVERT(FLOAT, dd.Length) AS VARCHAR(50))
			 WHEN dd.BoreSize = 0
             THEN 'Φ' + CAST(CONVERT(FLOAT, dd.Thick) AS VARCHAR(50)) + '*'
                  + CAST(CONVERT(FLOAT, dd.Length) AS VARCHAR(50))
				  WHEN dd.BoreSize > 0
				  THEN 'Φ' + CAST(CONVERT(FLOAT, dd.Thick) AS VARCHAR(50)) +'Φ' + CAST(CONVERT(FLOAT, dd.BoreSize) AS VARCHAR(50)) + '*'
                  + CAST(CONVERT(FLOAT, dd.Length) AS VARCHAR(50))
        END AS '规格',
        dd.Qty '数量' ,
        dd.TheoryWeight '理重' ,
        dd.Weight AS '重量' ,
        dd.MaterialPrice AS '材料单价' ,
        dd.MaterialAmount '材料合计' ,
        dd.WorkPrice '小件单价(件)' ,
        dd.WorkAmount '小件合计' ,
        dd.SmallCost '加工单价(ks)' ,
        dd.SmallAmount '加工合计' ,
        dd.HeatPrice '热处理单价' ,
        dd.HeatAmount '热处理合计' ,
dd.JKCost '锯工费' ,
        dd.SalesAmount '合计费用' ,
        '''' + CustomerCode CustomerCode,
        dd.LineCode,dd.DDCost'切角',dd.DeliveryCode,'''' + dd.CustomerProduct '模号','''' + dd.ShipDescription '发运备注','''' + dd.ProjectCode '配件号','''' + DD.REMARK '备注',dd.Technology '类型',case when dd.Technology is null or dd.Technology = '' or dd.Technology='&nbsp;' then dd.OrderType else dd.Technology end as Technology,d.SalesMan '业务员',od.Fang '放'
 FROM   dbo.BY_DeliveryDetail dd
        LEFT JOIN dbo.BY_Delivery d ON dd.DeliveryId = d.DeliveryId
left join BY_OrderDetail od on od.OrderDetailId = dd.OrderDetailId
 WHERE  1 = 1   and d.DeleteMark = 0 ");

            if (!HashtableHelper.IsNullOrDBNull(Condition["LType"]))
            {
                strSql.Append(" AND d.LType = @LType   ");
                parameter.Add(DbFactory.CreateDbParameter("@LType", Condition["LType"].ToString()));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["ComeBackDate_StartTime"]))
            {
                strSql.Append(" AND d.ComeBackDate >= @ComeBackDate_StartTime ");
                parameter.Add(DbFactory.CreateDbParameter("@ComeBackDate_StartTime", CommonHelper.GetDateTime(Condition["ComeBackDate_StartTime"] + " :00")));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["ComeBackDate_EndTime"]))
            {
                strSql.Append(" AND d.ComeBackDate <= @ComeBackDate_EndTime ");
                parameter.Add(DbFactory.CreateDbParameter("@ComeBackDate_EndTime", CommonHelper.GetDateTime(Condition["ComeBackDate_EndTime"] + " :59")));
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["Delivery_StartTime"]))
            {
                strSql.Append(" AND d.CreateDate >= @Delivery_StartTime ");
                parameter.Add(DbFactory.CreateDbParameter("@Delivery_StartTime", CommonHelper.GetDateTime(Condition["Delivery_StartTime"] + " :00")));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["Delivery_EndTime"]))
            {
                strSql.Append(" AND d.CreateDate <= @Delivery_EndTime ");
                parameter.Add(DbFactory.CreateDbParameter("@Delivery_EndTime", CommonHelper.GetDateTime(Condition["Delivery_EndTime"] + " :59")));
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["OrderCode"]))
            {
                strSql.Append(" AND d.OrderCode LIKE @OrderCode ");
                parameter.Add(DbFactory.CreateDbParameter("@OrderCode", '%' + Condition["OrderCode"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["DepartName"]))
            {
                strSql.Append(" AND d.DepartName LIKE @DepartName ");
                parameter.Add(DbFactory.CreateDbParameter("@DepartName", '%' + Condition["DepartName"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["ShortName"]))
            {
                strSql.Append(" AND ( d.CustomerName LIKE @ShortName or d.CustomerCode LIKE @ShortName or d.ShortName LIKE @ShortName ) ");
                parameter.Add(DbFactory.CreateDbParameter("@ShortName", '%' + Condition["ShortName"].ToString() + '%'));
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["CreateUserName"]))
            {
                strSql.Append(" AND d.CreateUserName LIKE @CreateUserName ");
                parameter.Add(DbFactory.CreateDbParameter("@CreateUserName", '%' + Condition["CreateUserName"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["Sellpeople"]))
            {
                strSql.Append(" AND d.SalesMan LIKE @Sellpeople ");
                parameter.Add(DbFactory.CreateDbParameter("@Sellpeople", '%' + Condition["Sellpeople"].ToString() + '%'));
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["DeliveryCode"]))
            {
                strSql.Append(" AND d.DeliveryCode LIKE @DeliveryCode ");
                parameter.Add(DbFactory.CreateDbParameter("@DeliveryCode", '%' + Condition["DeliveryCode"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["QuotationCode"]))
            {
                strSql.Append(" AND d.QuotationCode LIKE @QuotationCode ");
                parameter.Add(DbFactory.CreateDbParameter("@QuotationCode", '%' + Condition["QuotationCode"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["OrderType"]))
            {
                strSql.Append(" AND d.OrderType = @OrderType ");
                parameter.Add(DbFactory.CreateDbParameter("@OrderType", Condition["OrderType"].ToString()));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["noprint"]) && Condition["noprint"].ToString() == "1" && !HashtableHelper.IsNullOrDBNull(Condition["printed"]) && Condition["printed"].ToString() != "1")
            {
                strSql.Append(" AND d.PrintDate is null ");
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["noprint"]) && Condition["noprint"].ToString() != "1" && !HashtableHelper.IsNullOrDBNull(Condition["printed"]) && Condition["printed"].ToString() == "1")
            {
                strSql.Append(" AND d.PrintDate is not null ");
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["CustomerCode"]))
            {
                strSql.Append(" AND d.CustomerCode = @CustomerCode ");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerCode", Condition["CustomerCode"].ToString()));
            }
            //if (DNS.Utilities.ManageProvider.Provider.Current().IsMobileUser && DNS.Utilities.ManageProvider.Provider.Current().ObjectId.IndexOf("20178431-3604-46b6-a75b-f7ba13548aed") < 0)
            //{
            //    strSql.Append(" AND d.CustomerCode = @CustomerCodef ");
            //    parameter.Add(DbFactory.CreateDbParameter("@CustomerCodef", DNS.Utilities.ManageProvider.Provider.Current().UserId));
            //}
            if (!HashtableHelper.IsNullOrDBNull(Condition["MnemonicCode"]))
            {
                strSql.Append(" AND (d.MnemonicCode LIKE '%'+@MnemonicCode+'%'  )");
                parameter.Add(DbFactory.CreateDbParameter("@MnemonicCode", Condition["MnemonicCode"].ToString().Trim()));
            }
            //            if (!HashtableHelper.IsNullOrDBNull(Condition["s1"]))
            //            {
            //                string s1 = Condition["s1"].ToString() == "1" ? "门市" : "";
            //                string s2 = Condition["s2"].ToString() == "1" ? "同行" : "";
            //                string s3 = Condition["s3"].ToString() == "1" ? "客户" : "";
            //                string s = s1 + s2 + s3;
            //                if (s != "" && s != "门市同行客户")
            //                {
            //                    strSql.AppendFormat(@" AND d.CustomerCode IN (
            //            SELECT  c.Code
            //            FROM    Base_Customer c
            //            WHERE   c.ProcessingAlgorithm IN  ('{0}','{1}','{2}')) ", s1, s2, s3);
            //                }
            //            }


            if (!HashtableHelper.IsNullOrDBNull(Condition["s41"]))
            {
                if (Condition["s41"].ToString() == "1")
                {
                    strSql.Append(" AND  (d.comebackuserid  is not null or d.comebackuserid  != '' )    ");
                }
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["s42"]))
            {
                if (Condition["s42"].ToString() == "1")
                {
                    strSql.Append(" AND  (d.CheckSheetCode  is  null or d.CheckSheetCode  = '' )    ");

                }
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["s1"]))
            {
                if ((Condition["s1"].ToString() != "1" && Condition["s11"].ToString() == "1") || (Condition["s1"].ToString() == "1" && Condition["s11"].ToString() != "1"))
                {
                    if (Condition["s1"].ToString() == "1")
                    {
                        strSql.Append(" AND d.Enabled  = 1");
                    }
                    if (Condition["s11"].ToString() == "1")
                    {
                        strSql.Append(" AND d.Enabled  = -1");
                    }
                }

                if ((Condition["s2"].ToString() != "1" && Condition["s21"].ToString() == "1") || (Condition["s2"].ToString() == "1" && Condition["s21"].ToString() != "1"))
                {
                    if (Condition["s2"].ToString() == "1")
                    {
                        strSql.Append(" AND (d.CheckSheetCode is not null and d.CheckSheetCode != '') ");

                    }
                    if (Condition["s21"].ToString() == "1")
                    {
                        strSql.Append(" AND (d.CheckSheetCode is  null or d.CheckSheetCode = '') ");

                    }
                }

                if ((Condition["s3"].ToString() != "1" && Condition["s31"].ToString() == "1") || (Condition["s3"].ToString() == "1" && Condition["s31"].ToString() != "1"))
                {
                    if (Condition["s3"].ToString() == "1")
                    {
                        strSql.Append(" AND (d.printuserid  is null or d.printuserid  = '' )  ");
                    }
                    if (Condition["s31"].ToString() == "1")
                    {
                        strSql.Append(" AND  (d.audituserid  is not null and d.audituserid  != '' )    ");

                    }
                }
                if ((Condition["c5"].ToString() != "1" && Condition["c51"].ToString() == "1") || (Condition["c5"].ToString() == "1" && Condition["c51"].ToString() != "1"))
                {
                    if (Condition["c5"].ToString() == "1")
                    {
                        strSql.Append(" AND (d.paymentmode not  like '月结%' )  ");
                    }
                    if (Condition["c51"].ToString() == "1")
                    {
                        strSql.Append(" AND  (d.paymentmode  like '月结%' )    ");
                    }
                }


                if ((Condition["s4"].ToString() != "1" && Condition["s41"].ToString() == "1") || (Condition["s4"].ToString() == "1" && Condition["s41"].ToString() != "1"))
                {
                    if (Condition["s4"].ToString() == "1")
                    {
                        strSql.Append(" AND (d.comebackuserid  is null or d.comebackuserid  = '' )  ");
                    }

                    if (Condition["s41"].ToString() == "1")
                    {
                        strSql.Append(" AND  (d.comebackuserid  is not null and d.comebackuserid  != '' )    ");

                    }
                }
                if ((Condition["H1"].ToString() != "1" && Condition["H11"].ToString() == "1") || (Condition["H1"].ToString() == "1" && Condition["H11"].ToString() != "1"))
                {
                    if (Condition["H1"].ToString() == "1")
                    {
                        strSql.Append(" AND d.IsInvoice =1 ");
                    }

                    if (Condition["H11"].ToString() == "1")
                    {
                        strSql.Append(" AND  d.IsInvoice!=1   ");

                    }
                }



            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["s5"]))
            {
                if ((Condition["s5"].ToString() != "1" && Condition["s51"].ToString() == "1") || (Condition["s5"].ToString() == "1" && Condition["s51"].ToString() != "1"))
                {
                    if (Condition["s5"].ToString() == "1")
                    {
                        strSql.Append(" AND (d.CheckSheetCode  is not null and d.CheckSheetCode  != '' )  ");
                    }

                    if (Condition["s51"].ToString() == "1")
                    {
                        strSql.Append(" AND  (d.CheckSheetCode  is  null or d.CheckSheetCode  = '' )   ");

                    }
                }
                if ((Condition["s6"].ToString() != "1" && Condition["s61"].ToString() == "1") || (Condition["s6"].ToString() == "1" && Condition["s61"].ToString() != "1"))
                {
                    if (Condition["s6"].ToString() == "1")
                    {
                        strSql.Append("  AND d.TotalAmount - d.GetedAmount = 0 ");
                    }

                    if (Condition["s61"].ToString() == "1")
                    {
                        strSql.Append("  AND d.TotalAmount - d.GetedAmount  != 0 ");

                    }
                }
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["CheckSheetCode"]))
            {
                strSql.Append(" AND d.CheckSheetCode like '%'+ @CheckSheetCode +'%' ");
                parameter.Add(DbFactory.CreateDbParameter("@CheckSheetCode", Condition["CheckSheetCode"].ToString()));
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["isover"]))
            {
                strSql.Append(@" AND d.totalamount- d.getedamount != 0    AND ( d.CheckSheetCode IS NULL
                        OR d.CheckSheetCode = ''
                      )  and  d.Enabled=1");
            }

            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }

        /// <summary>
        /// 获取还没有完成发货的订单
        /// </summary>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GetGridPageJson(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            List<DbParameter> parameter = new List<DbParameter>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"   SELECT    d.DeliveryId ,d.companyname ,
            d.DeliveryCode ,
            d.OrderCode ,
            d.CustomerId ,
            d.CustomerCode ,
            d.CustomerName ,
            d.ContactTel ,
            d.ContactName ,
            d.ShipAddress ,
            d.SalesMan ,
            d.ShipMode ,
            d.PaymentMode ,
            d.Qty ,
            d.TotalAmount ,
            d.CreateDate ,
            d.printuserid ,
            d.ShortName ,
            d.Weight ,
'' DieCode,
            d.GetedAmount ,
            d.MnemonicCode ,
            d.ComeBackUserId ,
            d.AuditUserId ,
            d.PrintDate ,
            d.ComeBackDate ,
            d.AuditDate ,
            d.CustomerPO ,
            d.ShipClause ,
            d.ShipDescription ,d.Driver,
            d.Remark ,
            ( SELECT  TOP 1    bt.GetedAmount
              FROM      dbo.BY_CheckSheet bt
              WHERE     bt.CheckSheetCode = d.CheckSheetCode
                        AND CustomerCode != 'XJMS'
            ) AS CSGetedAmount ,
            d.QuotationCode ,d.CheckSheetCode,
            d.Enabled,d.CustMaterial,d.createusername,d.IsInvoice,
	c.sellpeople,D.DepartName,d.ltype,( SELECT STRING_AGG(ifd.InvoiceCode, ',')   FROM By_InvoiceForDetail ifd where ifd.CheckSheetCode = d.CheckSheetCode) as 'InvoiceCodes'
  FROM      BY_Delivery d
LEFT
	JOIN Base_Customer c
	on c.Code = d.CustomerCode
  WHERE      d.CustomerName != '内部下单' and d.DeleteMark = 0  ");
            if (!HashtableHelper.IsNullOrDBNull(Condition["CompanyName"]))
            {
                strSql.Append(" AND d.CompanyName = @CompanyName ");
                parameter.Add(DbFactory.CreateDbParameter("@CompanyName", Condition["CompanyName"].ToString()));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["LType"]))
            {
                strSql.Append(" AND d.LType = @LType   ");
                parameter.Add(DbFactory.CreateDbParameter("@LType", Condition["LType"].ToString()));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["CustomerPO"]))
            {
                strSql.Append(" AND d.CustomerPO = @CustomerPO   ");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerPO", Condition["CustomerPO"].ToString()));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["ComeBackDate_StartTime"]))
            {
                strSql.Append(" AND d.ComeBackDate >= @ComeBackDate_StartTime ");
                parameter.Add(DbFactory.CreateDbParameter("@ComeBackDate_StartTime", CommonHelper.GetDateTime(Condition["ComeBackDate_StartTime"] + " :00")));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["ComeBackDate_EndTime"]))
            {
                strSql.Append(" AND d.ComeBackDate <= @ComeBackDate_EndTime ");
                parameter.Add(DbFactory.CreateDbParameter("@ComeBackDate_EndTime", CommonHelper.GetDateTime(Condition["ComeBackDate_EndTime"] + " :59")));
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["Delivery_StartTime"]))
            {
                strSql.Append(" AND d.CreateDate >= @Delivery_StartTime ");
                parameter.Add(DbFactory.CreateDbParameter("@Delivery_StartTime", CommonHelper.GetDateTime(Condition["Delivery_StartTime"] + " :00")));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["Delivery_EndTime"]))
            {
                strSql.Append(" AND d.CreateDate <= @Delivery_EndTime ");
                parameter.Add(DbFactory.CreateDbParameter("@Delivery_EndTime", CommonHelper.GetDateTime(Condition["Delivery_EndTime"] + " :59")));
            }
             if (!HashtableHelper.IsNullOrDBNull(Condition["BeginDate"]))
            {
                strSql.Append(" AND d.CreateDate >= @BeginDate ");
                parameter.Add(DbFactory.CreateDbParameter("@BeginDate", CommonHelper.GetDateTime(Condition["BeginDate"] + " :00")));
            }
             if (!HashtableHelper.IsNullOrDBNull(Condition["EndDate"]))
            {
                strSql.Append(" AND d.CreateDate <= @EndDate ");
                parameter.Add(DbFactory.CreateDbParameter("@EndDate", CommonHelper.GetDateTime(Condition["EndDate"] + " :59")));
            }
             
                
            if (!HashtableHelper.IsNullOrDBNull(Condition["OrderCode"]))
            {
                strSql.Append(" AND d.OrderCode LIKE @OrderCode ");
                parameter.Add(DbFactory.CreateDbParameter("@OrderCode", '%' + Condition["OrderCode"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["DepartName"]))
            {
                strSql.Append(" AND d.DepartName LIKE @DepartName ");
                parameter.Add(DbFactory.CreateDbParameter("@DepartName", '%' + Condition["DepartName"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["ShortName"]))
            {
                strSql.Append(" AND ( d.CustomerName LIKE @ShortName or d.CustomerCode LIKE @ShortName or d.ShortName LIKE @ShortName ) ");
                parameter.Add(DbFactory.CreateDbParameter("@ShortName", '%' + Condition["ShortName"].ToString() + '%'));
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["CreateUserName"]))
            {
                strSql.Append(" AND d.CreateUserName LIKE @CreateUserName ");
                parameter.Add(DbFactory.CreateDbParameter("@CreateUserName", '%' + Condition["CreateUserName"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["Sellpeople"]))
            {
                strSql.Append(" AND c.Sellpeople LIKE @Sellpeople ");
                parameter.Add(DbFactory.CreateDbParameter("@Sellpeople", '%' + Condition["Sellpeople"].ToString() + '%'));
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["DeliveryCode"]))
            {
                strSql.Append(" AND d.DeliveryCode LIKE @DeliveryCode ");
                parameter.Add(DbFactory.CreateDbParameter("@DeliveryCode", '%' + Condition["DeliveryCode"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["QuotationCode"]))
            {
                strSql.Append(" AND d.QuotationCode LIKE @QuotationCode ");
                parameter.Add(DbFactory.CreateDbParameter("@QuotationCode", '%' + Condition["QuotationCode"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["OrderType"]))
            {
                strSql.Append(" AND d.OrderType = @OrderType ");
                parameter.Add(DbFactory.CreateDbParameter("@OrderType", Condition["OrderType"].ToString()));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["noprint"]) && Condition["noprint"].ToString() == "1" && !HashtableHelper.IsNullOrDBNull(Condition["printed"]) && Condition["printed"].ToString() != "1")
            {
                strSql.Append(" AND d.PrintDate is null ");
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["noprint"]) && Condition["noprint"].ToString() != "1" && !HashtableHelper.IsNullOrDBNull(Condition["printed"]) && Condition["printed"].ToString() == "1")
            {
                strSql.Append(" AND d.PrintDate is not null ");
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["CustomerCode"]) || !HashtableHelper.IsNullOrDBNull(Condition["CustomerId"]))
            {
                strSql.Append(" AND (d.CustomerCode = @CustomerCode or d.CustomerId = @CustomerId )");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerCode", Condition["CustomerCode"] == null ? "": Condition["CustomerCode"].ToString()));
                parameter.Add(DbFactory.CreateDbParameter("@CustomerId", Condition["CustomerId"] == null ? "" : Condition["CustomerId"].ToString()));
            }
            //if (DNS.Utilities.ManageProvider.Provider.Current().IsMobileUser && DNS.Utilities.ManageProvider.Provider.Current().ObjectId.IndexOf("20178431-3604-46b6-a75b-f7ba13548aed") < 0)
            //{
            //    strSql.Append(" AND d.CustomerCode = @CustomerCodef ");
            //    parameter.Add(DbFactory.CreateDbParameter("@CustomerCodef", DNS.Utilities.ManageProvider.Provider.Current().UserId));
            //}
            if (!HashtableHelper.IsNullOrDBNull(Condition["MnemonicCode"]))
            {
                strSql.Append(" AND (d.MnemonicCode LIKE '%'+@MnemonicCode+'%'  )");
                parameter.Add(DbFactory.CreateDbParameter("@MnemonicCode", Condition["MnemonicCode"].ToString().Trim()));
            }
            //            if (!HashtableHelper.IsNullOrDBNull(Condition["s1"]))
            //            {
            //                string s1 = Condition["s1"].ToString() == "1" ? "门市" : "";
            //                string s2 = Condition["s2"].ToString() == "1" ? "同行" : "";
            //                string s3 = Condition["s3"].ToString() == "1" ? "客户" : "";
            //                string s = s1 + s2 + s3;
            //                if (s != "" && s != "门市同行客户")
            //                {
            //                    strSql.AppendFormat(@" AND d.CustomerCode IN (
            //            SELECT  c.Code
            //            FROM    Base_Customer c
            //            WHERE   c.ProcessingAlgorithm IN  ('{0}','{1}','{2}')) ", s1, s2, s3);
            //                }
            //            }


            if (!HashtableHelper.IsNullOrDBNull(Condition["s41"]))
            {
                if (Condition["s41"].ToString() == "1")
                {
                    strSql.Append(" AND  (d.comebackuserid  is not null or d.comebackuserid  != '' )    ");
                }
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["s42"]))
            {
                if (Condition["s42"].ToString() == "1")
                {
                    strSql.Append(" AND  (d.CheckSheetCode  is  null or d.CheckSheetCode  = '' )    ");

                }
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["s1"]))
            {
                if ((Condition["s1"].ToString() != "1" && Condition["s11"].ToString() == "1") || (Condition["s1"].ToString() == "1" && Condition["s11"].ToString() != "1"))
                {
                    if (Condition["s1"].ToString() == "1")
                    {
                        strSql.Append(" AND d.Enabled  = 1");
                    }
                    if (Condition["s11"].ToString() == "1")
                    {
                        strSql.Append(" AND d.Enabled  = -1");
                    }
                }

                if ((Condition["s2"].ToString() != "1" && Condition["s21"].ToString() == "1") || (Condition["s2"].ToString() == "1" && Condition["s21"].ToString() != "1"))
                {
                    if (Condition["s2"].ToString() == "1")
                    {
                        strSql.Append(" AND (d.CheckSheetCode is not null and d.CheckSheetCode != '') ");

                    }
                    if (Condition["s21"].ToString() == "1")
                    {
                        strSql.Append(" AND (d.CheckSheetCode is  null or d.CheckSheetCode = '') ");

                    }
                }

                if ((Condition["s3"].ToString() != "1" && Condition["s31"].ToString() == "1") || (Condition["s3"].ToString() == "1" && Condition["s31"].ToString() != "1"))
                {
                    if (Condition["s3"].ToString() == "1")
                    {
                        strSql.Append(" AND (d.printuserid  is null or d.printuserid  = '' )  ");
                    }
                    if (Condition["s31"].ToString() == "1")
                    {
                        strSql.Append(" AND  (d.audituserid  is not null and d.audituserid  != '' )    ");

                    }
                }
                if ((Condition["c5"].ToString() != "1" && Condition["c51"].ToString() == "1") || (Condition["c5"].ToString() == "1" && Condition["c51"].ToString() != "1"))
                {
                    if (Condition["c5"].ToString() == "1")
                    {
                        strSql.Append(" AND (d.paymentmode not  like '月结%' )  ");
                    }
                    if (Condition["c51"].ToString() == "1")
                    {
                        strSql.Append(" AND  (d.paymentmode  like '月结%' )    ");
                    }
                }


                if ((Condition["s4"].ToString() != "1" && Condition["s41"].ToString() == "1") || (Condition["s4"].ToString() == "1" && Condition["s41"].ToString() != "1"))
                {
                    if (Condition["s4"].ToString() == "1")
                    {
                        strSql.Append(" AND (d.comebackuserid  is null or d.comebackuserid  = '' )  ");
                    }

                    if (Condition["s41"].ToString() == "1")
                    {
                        strSql.Append(" AND  (d.comebackuserid  is not null and d.comebackuserid  != '' )    ");

                    }
                }
                if ((Condition["H1"].ToString() != "1" && Condition["H11"].ToString() == "1") || (Condition["H1"].ToString() == "1" && Condition["H11"].ToString() != "1"))
                {
                    if (Condition["H1"].ToString() == "1")
                    {
                        strSql.Append(" AND d.IsInvoice =1 ");
                    }

                    if (Condition["H11"].ToString() == "1")
                    {
                        strSql.Append(" AND  d.IsInvoice!=1   ");

                    }
                }



            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["s5"]))
            {
                if ((Condition["s5"].ToString() != "1" && Condition["s51"].ToString() == "1") || (Condition["s5"].ToString() == "1" && Condition["s51"].ToString() != "1"))
                {
                    if (Condition["s5"].ToString() == "1")
                    {
                        strSql.Append(" AND (d.CheckSheetCode  is not null and d.CheckSheetCode  != '' )  ");
                    }

                    if (Condition["s51"].ToString() == "1")
                    {
                        strSql.Append(" AND  (d.CheckSheetCode  is  null or d.CheckSheetCode  = '' )   ");

                    }
                }
                if ((Condition["s6"].ToString() != "1" && Condition["s61"].ToString() == "1") || (Condition["s6"].ToString() == "1" && Condition["s61"].ToString() != "1"))
                {
                    if (Condition["s6"].ToString() == "1")
                    {
                        strSql.Append("  AND d.TotalAmount - d.GetedAmount = 0 ");
                    }

                    if (Condition["s61"].ToString() == "1")
                    {
                        strSql.Append("  AND d.TotalAmount - d.GetedAmount  != 0 ");

                    }
                }
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["CheckSheetCode"]))
            {
                strSql.Append(" AND d.CheckSheetCode like '%'+ @CheckSheetCode +'%' ");
                parameter.Add(DbFactory.CreateDbParameter("@CheckSheetCode", Condition["CheckSheetCode"].ToString()));
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["isover"]))
            {
                strSql.Append(@" AND d.totalamount- d.getedamount != 0    AND ( d.CheckSheetCode IS NULL
                        OR d.CheckSheetCode = ''
                      )  and  d.Enabled=1");
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }


        /// <summary>
        /// 获取还没有完成发货的订单
        /// </summary>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GetGridPageJsonCust(string starttime, string endtime, string CustomerId, ref JqGridParam jqgridparam)
        {
            List<DbParameter> parameter = new List<DbParameter>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"  SELECT *
 FROM   ( SELECT    d.createdate ,   --落单日期
                    q.CustomerName ,     --客户名称
                    d.DeliveryCode ,    --报价单号
                    de.LineCode ,        --行号
                    de.SteelCode ,       --钢种 
                    '销售:' + qd.ProductName ProductName ,
                    de.Thick ,           --厚
                    de.Weigh ,           --宽
                    de.Length ,          --长
                    de.Qty ,             --数量
                    de.Weight ,          --重量
                    qd.MaterialPrice ,   --材料单价
                    de.MaterialAmount ,  --材料费
                    de.WorkAmount ,      --加工费
                    de.HeatAmount ,      --热处理费
                    de.SalesAmount ,     --送货单金额
                    de.JKCost ,
                    de.JKCostQty * 2 * de.Qty AS jkcc ,
                    de.WorkCost ,
                    de.MaterialCost ,
                    de.ManagerCost ,
                    de.InvoiceCost ,
                    de.HeatCost ,
                    de.WorkCost + de.MaterialCost + de.ManagerCost
                    + de.InvoiceCost + de.HeatCost + de.JKCostQty * 2 AS TotalCost ,
                    ( de.SalesAmount - ( de.WorkCost + de.MaterialCost
                                         + de.ManagerCost + de.InvoiceCost
                                         + de.HeatCost + de.JKCostQty * 2
                                         * de.Qty ) ) AS yingli
          FROM      dbo.BY_DeliveryDetail de
                    INNER JOIN dbo.BY_Delivery d ON de.DeliveryId = d.DeliveryId
                    INNER JOIN dbo.BY_Quotation q ON d.QuotationCode = q.QuotationCode
                    INNER JOIN dbo.BY_QuotationDetail qd ON q.QuotationId = qd.QuotationId
                                                            AND de.LineCode = qd.LineCode
          WHERE     1 = 1  AND Q.DeleteMark = 0
                    AND d.CustomerId = @CustomerId and d.DeleteMark = 0 
          UNION ALL
          SELECT    CreateDate ,   --落单日期
                    CustomerName ,     --客户名称
                    PayCompanyCode DeliveryCode ,    --报价单号
                    0 LineCode ,        --行号
                    '' SteelCode ,       --钢种 
                    '其他:' + PayName ProductName ,
                    0 Thick ,           --厚
                    0 Weigh ,           --宽
                    0 Length ,          --长
                    0 Qty ,             --数量
                    0 Weight ,          --重量
                    0 MaterialPrice ,   --材料单价
                    0 MaterialAmount ,  --材料费
                    0 WorkAmount ,      --加工费
                    0 HeatAmount ,      --热处理费
                    0 SalesAmount ,     --送货单金额
                    0 JKCost ,
                    0 AS jkcc ,
                    0 WorkCost ,
                    TotalAmount MaterialCost ,
                    0 ManagerCost ,
                    0 InvoiceCost ,
                    0 HeatCost ,
                    TotalAmount AS TotalCost ,
                    ( -TotalAmount ) yingli
          FROM      Base_PayCompany
          WHERE     1 = 1
                    AND CustomerId = @CustomerId
          UNION ALL
          SELECT    RequiredDate CreateDate,   --落单日期
                    CustomerName ,     --客户名称
                    Code DeliveryCode ,    --报价单号
                    0 LineCode ,        --行号
                    '' SteelCode ,       --钢种 
                    '折让:' + ZRRemark ProductName ,
                    0 Thick ,           --厚
                    0 Weigh ,           --宽
                    0 Length ,          --长
                    0 Qty ,             --数量
                    0 Weight ,          --重量
                    0 MaterialPrice ,   --材料单价
                    0 MaterialAmount ,  --材料费
                    0 WorkAmount ,      --加工费
                    0 HeatAmount ,      --热处理费
                    0 SalesAmount ,     --送货单金额
                    0 JKCost ,
                    0 AS jkcc ,
                    0 WorkCost ,
                    0 MaterialCost ,
                    0 ManagerCost ,
                    0 InvoiceCost ,
                    0 HeatCost ,
                    ZRMoney AS TotalCost ,
                    ( -ZRMoney ) yingli
          FROM      BY_ReimburseSheet
          WHERE     1 = 1 AND ZRMoney != 0
                    AND CustomerId = @CustomerId
        ) a
 WHERE  1 = 1 
 
  ");
            if (!string.IsNullOrEmpty(starttime))
            {
                strSql.Append(" and createdate > @starttime ");
                parameter.Add(DbFactory.CreateDbParameter("@starttime", starttime));
            }
             if (!string.IsNullOrEmpty(endtime))
            {
                strSql.Append(" and createdate < @endtime ");
                parameter.Add(DbFactory.CreateDbParameter("@endtime", endtime));
            }
          

            parameter.Add(DbFactory.CreateDbParameter("@CustomerId", CustomerId));

            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        public DataTable GetGridPageJsonforPay(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            List<DbParameter> parameter = new List<DbParameter>();
            StringBuilder strSql = new StringBuilder();
            
            if (!HashtableHelper.IsNullOrDBNull(Condition["PayCheckSheetCode"]))
            {
                strSql.Append(@" select * from ( SELECT MaterielPurchaseCode AS CODE,CustomerName,TotalAmount,CreateDate,Status,PayCheckSheetCode FROM Base_MaterielPurchase where  PayCheckSheetCode = @PayCheckSheetCode  UNION ALL ");
                strSql.Append(@" SELECT
	MnemonicCode AS CODE,
	CustomerName,
	CostAmount TotalAmount,
	CreateDate,
 CASE when BillStatus = '验收通过' THEN 1 ELSE 0 END
 	Status,
	PayCheckSheetCode
FROM BY_OrderDetailBeOut
WHERE PayCheckSheetCode = @PayCheckSheetCode  UNION ALL ");
                strSql.Append(@" SELECT SteelPurchaseCode AS CODE,CustomerName,TotalAmount,CreateDate,Status,PayCheckSheetCode  FROM Base_SteelPurchase where  PayCheckSheetCode = @PayCheckSheetCode ) a ");
                parameter.Add(DbFactory.CreateDbParameter("@PayCheckSheetCode", Condition["PayCheckSheetCode"].ToString()));
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }

        public DataTable GetGridPageJson1(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            List<DbParameter> parameter = new List<DbParameter>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@" 
 
 

  
  SELECT    a.CustomerCode ,
            a.CustomerName ,a.PaymentMode,
            ISNULL(SUM(Amount30), 0) Amount30 ,
            ISNULL(SUM(Amount60), 0) Amount60 ,
            ISNULL(SUM(Amount90), 0) Amount90 ,
            ISNULL(SUM(Amount180), 0) Amount180 ,
            ISNULL(SUM(Amount999), 0) Amount999 ,
            ISNULL(SUM(Amount), 0) Amount ,
            (SELECT sum(Amount) FROM BY_ORDER O WHERE O.CustomerCode =a.CustomerCode AND O.OrderStatus = '正常生产' and o.DeleteMark=0 ) ondoing
  FROM      ( SELECT    CustomerCode ,
                        CustomerName ,PaymentMode,
                        Amount30 ,
                        0 Amount60 ,
                        0 Amount90 ,
                        0 Amount180 ,
                        0 Amount999 ,
                        0 Amount
                        
                        
              FROM      ( SELECT    CustomerCode ,
                                    CustomerName ,PaymentMode,
                                    SUM(Amount) Amount30 
                                    
                          FROM      ( SELECT    d.CustomerCode ,
                                                d.CustomerName ,d.PaymentMode,
                                                ISNULL(SUM(d.TotalAmount - d.GetedAmount), 0) Amount 
                                                
                                      FROM      BY_Delivery d
                                      WHERE     d.enabled = 1 and d.DeleteMark = 0  and (d.CheckSheetCode is null or d.CheckSheetCode ='')
                                                AND ISNULL(DATEDIFF(DAY,
                                                              d.CreateDate,
                                                              @CreateDate_EndTime), 0) > 0
                                                AND ISNULL(DATEDIFF(DAY,
                                                              d.CreateDate,
                                                              @CreateDate_EndTime), 0) <= 30
                                      GROUP BY  d.CustomerCode ,
                                                d.CustomerName ,d.PaymentMode
                                       
									   union all 
									   select c.CustomerCode ,
                                                c.CustomerName ,c.PaymentMode,
                                                ISNULL(SUM(c.TotalAmount - c.GetedAmount), 0) Amount 
												from BY_CheckSheet c where ISNULL(DATEDIFF(DAY,
                                                              c.YearMonth + '-01',
                                                              @CreateDate_EndTime), 0) > 0
                                                AND ISNULL(DATEDIFF(DAY,
                                                              c.YearMonth + '-01',
                                                              @CreateDate_EndTime), 0) <= 30
                                      GROUP BY  c.CustomerCode ,
                                                c.CustomerName ,c.PaymentMode

                                    ) a
                          GROUP BY  CustomerCode ,
                                    CustomerName ,PaymentMode
                                    
                        ) a30
              UNION ALL
              SELECT    CustomerCode ,
                        CustomerName ,PaymentMode,
                        0 Amount30 ,
                        Amount60 ,
                        0 Amount90 ,
                        0 Amount180 ,
                        0 Amount999 ,
                        0 Amount 
                        
                        
                        
                        
                        
                        
              FROM      ( SELECT    CustomerCode ,
                                    CustomerName ,PaymentMode,
                                    SUM(Amount) Amount60 
                                    
                          FROM      ( SELECT    d.CustomerCode ,
                                                d.CustomerName ,d.PaymentMode,
                                                ISNULL(SUM(d.TotalAmount - d.GetedAmount), 0) Amount 
                                                
                                      FROM      BY_Delivery d
                                      WHERE     d.enabled = 1 and d.DeleteMark = 0 and (d.CheckSheetCode is null or d.CheckSheetCode ='')
                                                AND ISNULL(DATEDIFF(DAY,
                                                              d.CreateDate,
                                                              @CreateDate_EndTime), 0) > 30
                                                AND ISNULL(DATEDIFF(DAY,
                                                              d.CreateDate,
                                                              @CreateDate_EndTime), 0) <= 60
                                      GROUP BY  d.CustomerCode ,
                                                d.CustomerName ,d.PaymentMode
                                                union all 
									   select c.CustomerCode ,
                                                c.CustomerName ,c.PaymentMode,
                                                ISNULL(SUM(c.TotalAmount - c.GetedAmount), 0) Amount 
												from BY_CheckSheet c where ISNULL(DATEDIFF(DAY,
                                                              c.YearMonth + '-01',
                                                              @CreateDate_EndTime), 0) > 30
                                                AND ISNULL(DATEDIFF(DAY,
                                                              c.YearMonth + '-01',
                                                              @CreateDate_EndTime), 0) <= 60
                                      GROUP BY  c.CustomerCode ,
                                                c.CustomerName ,c.PaymentMode
                                    ) a
                          GROUP BY  CustomerCode ,
                                    CustomerName ,PaymentMode
                                    
                        ) a30
              UNION ALL
              SELECT    CustomerCode ,
                        CustomerName ,PaymentMode,
                        0 Amount30 ,
                        0 Amount60 ,
                        Amount90 ,
                        0 Amount180 ,
                        0 Amount999 ,
                        0 Amount 
                        
                        
                        
                        
                        
                        
              FROM      ( SELECT    CustomerCode ,
                                    CustomerName ,PaymentMode,
                                    SUM(Amount) Amount90 
                                    
                          FROM      ( SELECT    d.CustomerCode ,
                                                d.CustomerName ,d.PaymentMode,
                                                ISNULL(SUM(d.TotalAmount - d.GetedAmount), 0) Amount 
                                                
                                      FROM      BY_Delivery d
                                      WHERE     d.enabled = 1 and d.DeleteMark = 0 and (d.CheckSheetCode is null or d.CheckSheetCode ='')
                                                AND ISNULL(DATEDIFF(DAY,
                                                              d.CreateDate,
                                                              @CreateDate_EndTime), 0) > 60
                                                AND ISNULL(DATEDIFF(DAY,
                                                              d.CreateDate,
                                                              @CreateDate_EndTime), 0) <= 90
                                      GROUP BY  d.CustomerCode ,
                                                d.CustomerName ,d.PaymentMode
                                                union all 
									   select c.CustomerCode ,
                                                c.CustomerName ,c.PaymentMode,
                                                ISNULL(SUM(c.TotalAmount - c.GetedAmount), 0) Amount 
												from BY_CheckSheet c where ISNULL(DATEDIFF(DAY,
                                                              c.YearMonth + '-01',
                                                              @CreateDate_EndTime), 0) > 60
                                                AND ISNULL(DATEDIFF(DAY,
                                                              c.YearMonth + '-01',
                                                              @CreateDate_EndTime), 0) <= 90
                                      GROUP BY  c.CustomerCode ,
                                                c.CustomerName ,c.PaymentMode
                                    ) a
                          GROUP BY  CustomerCode ,
                                    CustomerName ,PaymentMode
                                    
                        ) a30
              UNION ALL
              SELECT    CustomerCode ,
                        CustomerName ,PaymentMode,
                        0 Amount30 ,
                        0 Amount60 ,
                        0 Amount90 ,
                        Amount180 ,
                        0 Amount999 ,
                        0 Amount 
                        
                        
                        
                        
                        
                        
              FROM      ( SELECT    CustomerCode ,
                                    CustomerName ,PaymentMode,
                                    SUM(Amount) Amount180 
                                    
                          FROM      ( SELECT    d.CustomerCode ,
                                                d.CustomerName ,d.PaymentMode,
                                                ISNULL(SUM(d.TotalAmount - d.GetedAmount), 0) Amount 
                                                
                                      FROM      BY_Delivery d
                                      WHERE     d.enabled = 1  and d.DeleteMark = 0 and (d.CheckSheetCode is null or d.CheckSheetCode ='')
                                                AND ISNULL(DATEDIFF(DAY,
                                                              d.CreateDate,
                                                              @CreateDate_EndTime), 0) > 90
                                                AND ISNULL(DATEDIFF(DAY,
                                                              d.CreateDate,
                                                              @CreateDate_EndTime), 0) <= 180
                                      GROUP BY  d.CustomerCode ,
                                                d.CustomerName ,d.PaymentMode
                                                union all 
									   select c.CustomerCode ,
                                                c.CustomerName ,c.PaymentMode,
                                                ISNULL(SUM(c.TotalAmount - c.GetedAmount), 0) Amount 
												from BY_CheckSheet c where ISNULL(DATEDIFF(DAY,
                                                              c.YearMonth + '-01',
                                                              @CreateDate_EndTime), 0) > 90
                                                AND ISNULL(DATEDIFF(DAY,
                                                              c.YearMonth + '-01',
                                                              @CreateDate_EndTime), 0) <= 180
                                      GROUP BY  c.CustomerCode ,
                                                c.CustomerName ,c.PaymentMode
                                    ) a
                          GROUP BY  CustomerCode ,
                                    CustomerName ,PaymentMode
                                    
                        ) a30
              UNION ALL
              SELECT    CustomerCode ,
                        CustomerName ,PaymentMode,
                        0 Amount30 ,
                        0 Amount60 ,
                        0 Amount90 ,
                        0 Amount180 ,
                        Amount999 ,
                        0 Amount 
                        
                        
                        
                        
                        
                        
              FROM      ( SELECT    CustomerCode ,
                                    CustomerName ,PaymentMode,
                                    SUM(Amount) Amount999 
                                    
                          FROM      ( SELECT    d.CustomerCode ,
                                                d.CustomerName ,d.PaymentMode,
                                                ISNULL(SUM(d.TotalAmount - d.GetedAmount), 0) Amount 
                                                
                                      FROM      BY_Delivery d
                                      WHERE     d.enabled = 1 and d.DeleteMark = 0 and (d.CheckSheetCode is null or d.CheckSheetCode ='')
                                                AND ISNULL(DATEDIFF(DAY,
                                                              d.CreateDate,
                                                              @CreateDate_EndTime), 0) > 180
                                      GROUP BY  d.CustomerCode ,
                                                d.CustomerName ,d.PaymentMode
                         union all 
									   select c.CustomerCode ,
                                                c.CustomerName ,c.PaymentMode,
                                                ISNULL(SUM(c.TotalAmount - c.GetedAmount), 0) Amount 
												from BY_CheckSheet c where ISNULL(DATEDIFF(DAY,
                                                              c.YearMonth + '-01',
                                                              @CreateDate_EndTime), 0) > 180
                                                
                                      GROUP BY  c.CustomerCode ,
                                                c.CustomerName ,c.PaymentMode
                                    ) a
                          GROUP BY  CustomerCode ,
                                    CustomerName ,PaymentMode
                                    
                        ) a30
              UNION ALL
              SELECT    CustomerCode ,
                        CustomerName ,PaymentMode,
                        0 Amount30 ,
                        0 Amount60 ,
                        0 Amount90 ,
                        0 Amount180 ,
                        0 Amount999 ,
                        Amount 
                        
              FROM      ( SELECT    CustomerCode ,
                                    CustomerName ,PaymentMode,
                                    SUM(Amount) Amount 
                                    
                          FROM      ( SELECT    d.CustomerCode ,
                                                d.CustomerName ,d.PaymentMode,
                                                ISNULL(SUM(d.TotalAmount - d.GetedAmount), 0) Amount 
                                                
                                      FROM      BY_Delivery d
                                      WHERE     d.enabled = 1 and d.DeleteMark = 0 and (d.CheckSheetCode is null or d.CheckSheetCode ='')
                                                
                                      GROUP BY  d.CustomerCode ,
                                                d.CustomerName ,d.PaymentMode
                             union all 
									   select c.CustomerCode ,
                                                c.CustomerName ,c.PaymentMode,
                                                ISNULL(SUM(c.TotalAmount - c.GetedAmount), 0) Amount 
												from BY_CheckSheet c 
                                      GROUP BY  c.CustomerCode ,
                                                c.CustomerName ,c.PaymentMode
                                     
                                                
                                    ) a
                          GROUP BY  CustomerCode ,
                                    CustomerName ,PaymentMode
                                    
                        ) a30
              
            ) a
            LEFT JOIN Base_Customer c ON a.CustomerCode = c.Code
  WHERE     a.CustomerCode = c.code
 

       
             
 

");


            parameter.Add(DbFactory.CreateDbParameter("@CreateDate_EndTime", CommonHelper.GetDateTime(Condition["CreateDate_EndTime"]).AddDays(1)));



            if (!HashtableHelper.IsNullOrDBNull(Condition["CustomerCode"]))
            {
                strSql.Append(" AND (a.CustomerName LIKE @CustomerCode or a.CustomerCode LIKE @CustomerCode) ");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerCode", '%' + Condition["CustomerCode"].ToString() + '%'));
            }
            //if (!HashtableHelper.IsNullOrDBNull(Condition["LType"]))
            //{
            //    strSql.Append(" AND a.LType = @LType ");
            //    parameter.Add(DbFactory.CreateDbParameter("@LType", Condition["LType"].ToString()));
            //}


            //if (!HashtableHelper.IsNullOrDBNull(Condition["Sellpeople"]))
            //{
            //    strSql.Append(" AND c.Sellpeople LIKE @Sellpeople ");
            //    parameter.Add(DbFactory.CreateDbParameter("@Sellpeople", '%' + Condition["Sellpeople"].ToString() + '%'));
            //}



            //if (DNS.Utilities.ManageProvider.Provider.Current().DepartmentId == "15175d9a-d80e-4e03-b0f0-63b1cff0f44d")
            //{
            //    strSql.Append(" AND sellpeople  = @sellpeople ");
            //    parameter.Add(DbFactory.CreateDbParameter("@sellpeople", DNS.Utilities.ManageProvider.Provider.Current().UserName));
            //}

            strSql.Append(@"   
  GROUP BY  a.CustomerCode ,
            a.CustomerName,a.PaymentMode  ");
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
    }
}