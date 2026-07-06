//=====================================================================================
// All Rights Reserved , Copyright @ DNS 2016
// Software Developers @ DNS 2016
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
    /// BY_QuotationHeat
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.12.22 16:02</date>
    /// </author>
    /// </summary>
    public class BY_QuotationHeatBll : RepositoryFactory<BY_QuotationHeat>
    {
        public DataTable GetGridPageJson(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            List<DbParameter> parameter = new List<DbParameter>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"   SELECT    d.DeliveryId ,d.companyname ,
            d.DeliveryCode ,
            d.OrderCode ,
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
            d.ShipDescription ,
            d.Remark ,d.Driver,
            ( SELECT   TOP 1   bt.GetedAmount
              FROM      dbo.BY_CheckSheet bt
              WHERE     bt.CheckSheetCode = d.CheckSheetCode
                        AND CustomerCode != 'XJMS'
            ) AS CSGetedAmount ,
            d.QuotationCode ,d.CheckSheetCode,
            d.Enabled,d.createusername,d.IsInvoice,
	c.sellpeople,( SELECT STRING_AGG(ifd.InvoiceCode, ',')   FROM By_InvoiceForDetail ifd where ifd.CheckSheetCode = d.CheckSheetCode) as 'InvoiceCodes'
  FROM      BY_Delivery d
            
LEFT
	JOIN Base_Customer c
	on c.Code = d.CustomerCode
  WHERE     d.LType = 'RCL' and d.DeleteMark = 0  ");

            if (!HashtableHelper.IsNullOrDBNull(Condition["CompanyName"]))
            {
                strSql.Append(" AND d.CompanyName = @CompanyName ");
                parameter.Add(DbFactory.CreateDbParameter("@CompanyName", Condition["CompanyName"].ToString()));
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["CustomerPO"]))
            {
                strSql.Append(" AND d.CustomerPO = @CustomerPO ");
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
            
            if (!HashtableHelper.IsNullOrDBNull(Condition["MnemonicCode"]))
            {
                strSql.Append(" AND (d.MnemonicCode LIKE '%'+@MnemonicCode+'%'  )");
                parameter.Add(DbFactory.CreateDbParameter("@MnemonicCode", Condition["MnemonicCode"].ToString().Trim()));
            }

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
                        strSql.Append(" AND  (d.printuserid  is not null and d.printuserid  != '' )    ");

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


        public DataTable GetQueryList(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT qh.*,(SELECT STUFF((SELECT '，'+bf.Status FROM base_file bf where bf.AttachId = qh.quotationheatid for xml path('')),1,1,'') ) [fileattch],
(SELECT STUFF((SELECT ','+bf.Path FROM base_file bf where bf.AttachId = qh.quotationheatid for xml path('')),1,1,'') ) filepath FROM BY_QuotationHeat qh 
                            WHERE 1=1 ");
            if (!HashtableHelper.IsNullOrDBNull(Condition["QuotationHeatCode"]))
            {
                strSql.Append(" AND qh.QuotationHeatCode LIKE @QuotationHeatCode");
                parameter.Add(DbFactory.CreateDbParameter("@QuotationHeatCode", '%' + Condition["QuotationHeatCode"].ToString() + '%'));
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["ShipDescription"]))
            {
                strSql.Append(" AND qh.ShipDescription LIKE @ShipDescription");
                parameter.Add(DbFactory.CreateDbParameter("@ShipDescription", '%' + Condition["ShipDescription"].ToString() + '%'));
            }


            if (!HashtableHelper.IsNullOrDBNull(Condition["CustomerCode"]))
            {
                strSql.Append(" AND ( qh.CustomerName LIKE @CustomerCode or qh.CustomerCode LIKE @CustomerCode ) ");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerCode", '%' + Condition["CustomerCode"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["SteelDescription"]))
            {
                strSql.Append(" AND qh.SteelDescription LIKE @SteelDescription");
                parameter.Add(DbFactory.CreateDbParameter("@SteelDescription", '%' + Condition["SteelDescription"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["ProductName"]))
            {
                strSql.Append(" AND qh.ProductName LIKE @ProductName");
                parameter.Add(DbFactory.CreateDbParameter("@ProductName", '%' + Condition["ProductName"].ToString() + '%'));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["Remark"]))
            {
                strSql.Append(" AND qh.Remark LIKE @Remark");
                parameter.Add(DbFactory.CreateDbParameter("@Remark", '%' + Condition["Remark"].ToString() + '%'));
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["Technology"]))
            {
                strSql.Append(" AND qh.Technology = @Technology");
                parameter.Add(DbFactory.CreateDbParameter("@Technology", Condition["Technology"].ToString()));
            }

            if (!HashtableHelper.IsNullOrDBNull(Condition["CreateDate_StartTime"]))
            {
                strSql.Append(" AND qh.CreateDate >= @CreateDate_StartTime  ");
                parameter.Add(DbFactory.CreateDbParameter("@CreateDate_StartTime", CommonHelper.GetDateTime(Condition["CreateDate_StartTime"] + ":00")));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["CreateDate_EndTime"]))
            {
                strSql.Append(" AND qh.CreateDate <= @CreateDate_EndTime  ");
                parameter.Add(DbFactory.CreateDbParameter("@CreateDate_EndTime", CommonHelper.GetDateTime(Condition["CreateDate_EndTime"] + ":59")));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["BillStatus"]))
            {
                strSql.Append(" AND qh.BillStatus = @BillStatus");
                parameter.Add(DbFactory.CreateDbParameter("@BillStatus", Condition["BillStatus"].ToString()));
            }
            if (!HashtableHelper.IsNullOrDBNull(Condition["CreateUserId"]))
            {
                strSql.Append(" AND qh.CreateUserId = @CreateUserId");
                parameter.Add(DbFactory.CreateDbParameter("@CreateUserId", Condition["CreateUserId"].ToString()));
            }
            //if (HashtableHelper.IsNullOrDBNull(Condition["isall"]))
            //{
            //    strSql.Append(" and SteelCode != 'FY' ");
            //}
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
    }
}