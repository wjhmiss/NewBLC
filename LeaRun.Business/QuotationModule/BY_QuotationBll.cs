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
    /// 报价单主表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.17 18:00</date>
    /// </author>
    /// </summary>
    public class BY_QuotationBll : RepositoryFactory<BY_Quotation>
    {
        public DataTable QuotationDetailListJson(string sql, ref JqGridParam jqgridparam)
        {

            List<DbParameter> parameter = new List<DbParameter>();
            return Repository().FindTablePageBySql(sql, parameter.ToArray(), ref jqgridparam);
        }

        /// <summary>
        /// 报价单查询列表
        /// </summary>
        /// <param name="QuotationCode">报价单号</param>
        /// <param name="StartTime">入单开始时间</param>
        /// <param name="EndTime">入单结束时间</param>
        /// <param name="jqgridparam">分页条件</param>
        /// <returns></returns>
        public DataTable GetPageList(string QuotationCode, string StartTime, string EndTime,string CustomerName,string MnemonicCode,string CustomerCode,string CustomerPO,string ProcessingAlgorithm,string FreeBill, string LType, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  *
                            FROM      BY_Quotation Q
                            WHERE   1 = 1  AND Q.DeleteMark = 0");
            if (!string.IsNullOrEmpty(QuotationCode))
            {
                strSql.Append(" AND QuotationCode LIKE @QuotationCode");
                parameter.Add(DbFactory.CreateDbParameter("@QuotationCode", '%' + QuotationCode + '%'));
            }
            if (!string.IsNullOrEmpty(CustomerName))
            {
                strSql.Append(" AND (Q.CustomerCode LIKE '%'+@CustomerName+'%' or  Q.ShortName LIKE '%'+@CustomerName+'%' or Q.CustomerName LIKE '%'+@CustomerName+'%' )");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerName", '%' + CustomerName + '%'));
            }
            if (!string.IsNullOrEmpty(StartTime) && !string.IsNullOrEmpty(EndTime))
            {
                strSql.Append(" AND CreateDate Between @StartTime AND @EndTime ");
                parameter.Add(DbFactory.CreateDbParameter("@StartTime", CommonHelper.GetDateTime(StartTime + " 00:00")));
                parameter.Add(DbFactory.CreateDbParameter("@EndTime", CommonHelper.GetDateTime(EndTime + " 23:59")));
            }
           
            if (!string.IsNullOrEmpty(MnemonicCode))
            {
                strSql.Append(" AND ( MnemonicCode LIKE '%'+@MnemonicCode+'%' or QuotationCode LIKE '%'+@MnemonicCode+'%' ) ");
                parameter.Add(DbFactory.CreateDbParameter("@MnemonicCode", MnemonicCode));
            }
            if (!string.IsNullOrEmpty(CustomerCode))
            {
                strSql.Append(" AND (CustomerCode LIKE '%'+@CustomerCode+'%' or CustomerName LIKE '%'+@CustomerCode+'%' or ShortName LIKE '%'+@CustomerCode+'%' )");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerCode", CustomerCode));
            }

            if (!string.IsNullOrEmpty(CustomerPO))
            {
                strSql.Append(" AND (CustomerPO LIKE '%'+@CustomerPO+'%' )");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerPO", CustomerPO));
            }
            
            if (FreeBill == "1")
            {
                strSql.Append(" AND FreeBill = 1");
            }


            if (!string.IsNullOrEmpty(ProcessingAlgorithm))
            {
                string[] temps = ProcessingAlgorithm.Split(',');
                strSql.AppendFormat(@" AND q.CustomerCode IN (
        SELECT  c.code
        FROM    Base_Customer c
        WHERE   c.ProcessingAlgorithm in ('{0}','{1}','{2}')) ", temps[0], temps[1], temps[2]);
            }

            if (!string.IsNullOrEmpty(LType))
            {
                strSql.Append(" AND LType = @LType ");
                parameter.Add(DbFactory.CreateDbParameter("@LType", LType));
            }

            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        public DataTable GetPageList1(string Size, string CustomerName, string MnemonicCode, string SteelCode, string LType, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  de.*,q.MnemonicCode,q.customercode,s.Density,Q.customerNAME,q.ShortName,q.DepartName
FROM    BY_QuotationDetail de ,
        BY_Quotation q,dbo.Base_Steel s
WHERE   de.QuotationId = q.QuotationId   AND Q.DeleteMark = 0
AND de.SteelCode = s.SteelCode ");
            if (!HashtableHelper.IsNullOrDBNull(Size))
            {
                string[] Sizes = Size.Replace(" ", "").Split('*');
                if (Sizes.Length == 2)
                {
                    strSql.Append(" AND de.Length like @Length AND de.Weigh=0 AND de.Thick like @Thick  ");
                    parameter.Add(DbFactory.CreateDbParameter("@Length", Sizes[0].Replace("Φ", "") + "%"));
                    parameter.Add(DbFactory.CreateDbParameter("@Thick", Sizes[1].Replace("Φ", "") + "%"));
                }
                if (Sizes.Length == 3)
                {
                    strSql.Append(@"  AND ((de.Length like @Length AND de.Weigh like @Weigh AND de.Thick like @Thick) or  
(de.Length like @Length AND de.Weigh like @Thick AND de.Thick like @Weigh) or 

(de.Length like @Weigh AND de.Weigh like @Length AND de.Thick like @Thick) or 

(de.Length like @Thick AND de.Weigh like @Weigh AND de.Thick like @Length) or 

(de.Length like @Weigh AND de.Weigh like @Thick AND de.Thick like @Length) or 

(de.Length like @Thick AND de.Weigh like @Weigh AND de.Thick like @Length)  )  ");
                    parameter.Add(DbFactory.CreateDbParameter("@Thick", Sizes[2] + "%"));
                    parameter.Add(DbFactory.CreateDbParameter("@Weigh", Sizes[1] + "%"));
                    parameter.Add(DbFactory.CreateDbParameter("@Length", Sizes[0] + "%"));
                }

            }
            if (!HashtableHelper.IsNullOrDBNull(CustomerName))
            {
                strSql.Append(" AND (Q.CustomerCode LIKE '%'+@CustomerName+'%' or  Q.ShortName LIKE '%'+@CustomerName+'%' or Q.CustomerName LIKE '%'+@CustomerName+'%' ) ");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerName",CustomerName ));
            }
            if (!HashtableHelper.IsNullOrDBNull(MnemonicCode))
            {
                strSql.Append(" AND ( q.MnemonicCode like @MnemonicCode  or  q.QuotationCode like @MnemonicCode ) ");
                parameter.Add(DbFactory.CreateDbParameter("@MnemonicCode", "%"+MnemonicCode+"%"));
            }
            if (!HashtableHelper.IsNullOrDBNull(SteelCode))
            {
                strSql.Append(" AND ( de.SteelCode like @SteelCode  or  de.SteelDescription like @SteelCode ) ");
                parameter.Add(DbFactory.CreateDbParameter("@SteelCode", "%" + SteelCode + "%"));
            }
            if (!HashtableHelper.IsNullOrDBNull(LType))
            {
                strSql.Append(" AND q.LType = @LType  ");
                parameter.Add(DbFactory.CreateDbParameter("@MnemonicCode", LType));
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        public DataTable GetPageList11(string Size, string CustomerName, string MnemonicCode,string FreeBill, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  de.*,q.MnemonicCode,q.customercode,s.Density,Q.customerNAME,q.ShortName,q.DepartName
FROM    BY_QuotationDetail de ,
        BY_Quotation q,dbo.Base_Steel s
WHERE   de.QuotationId = q.QuotationId  AND Q.DeleteMark = 0
AND de.SteelCode = s.SteelCode ");
            if (!HashtableHelper.IsNullOrDBNull(Size))
            {
                string[] Sizes = Size.Replace(" ", "").Split('*');
                if (Sizes.Length == 2)
                {
                    strSql.Append(" AND de.Length like @Length AND de.Weigh=0 AND de.Thick like @Thick  ");
                    parameter.Add(DbFactory.CreateDbParameter("@Length", Sizes[0].Replace("Φ", "") + "%"));
                    parameter.Add(DbFactory.CreateDbParameter("@Thick", Sizes[1].Replace("Φ", "") + "%"));
                }
                if (Sizes.Length == 3)
                {
                    strSql.Append(@"  AND ((de.Length like @Length AND de.Weigh like @Weigh AND de.Thick like @Thick) or  
(de.Length like @Length AND de.Weigh like @Thick AND de.Thick like @Weigh) or 

(de.Length like @Weigh AND de.Weigh like @Length AND de.Thick like @Thick) or 

(de.Length like @Thick AND de.Weigh like @Weigh AND de.Thick like @Length) or 

(de.Length like @Weigh AND de.Weigh like @Thick AND de.Thick like @Length) or 

(de.Length like @Thick AND de.Weigh like @Weigh AND de.Thick like @Length)  )  ");
                    parameter.Add(DbFactory.CreateDbParameter("@Thick", Sizes[2] + "%"));
                    parameter.Add(DbFactory.CreateDbParameter("@Weigh", Sizes[1] + "%"));
                    parameter.Add(DbFactory.CreateDbParameter("@Length", Sizes[0] + "%"));
                }

            }
            if (!HashtableHelper.IsNullOrDBNull(CustomerName))
            {
                strSql.Append(" AND (Q.CustomerCode LIKE '%'+@CustomerName+'%' or  Q.ShortName LIKE '%'+@CustomerName+'%' or Q.CustomerName LIKE '%'+@CustomerName+'%' ) ");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerName", CustomerName));
            }
            if (!HashtableHelper.IsNullOrDBNull(MnemonicCode))
            {
                strSql.Append(" AND ( q.MnemonicCode like @MnemonicCode  or  q.QuotationCode like @MnemonicCode ) ");
                parameter.Add(DbFactory.CreateDbParameter("@MnemonicCode", "%" + MnemonicCode + "%"));
            }
            if (FreeBill == "1")
            {
                strSql.Append(" AND ( q.FreeBill =1 ) ");
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        /// <summary>
        /// 报价单打印列表
        /// </summary>
        /// <param name="Condition">查询条件</param>
        /// <param name="jqgridparam">分页参数</param>
        /// <returns></returns>
        public List<BY_Quotation> GetQuotationPrintList(Hashtable Condition, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT * FROM BY_Quotation WHERE 1=1  AND DeleteMark = 0");
            //订单单号
            if (!HashtableHelper.IsNullOrDBNull(Condition["QuotationCode"]))
            {
                strSql.Append(" AND QuotationCode LIKE @QuotationCode");
                parameter.Add(DbFactory.CreateDbParameter("@QuotationCode", '%' + Condition["QuotationCode"].ToString() + '%'));
            }
            //订单日期
            if (!HashtableHelper.IsNullOrDBNull(Condition["CheckDate_StartTime"]) && !HashtableHelper.IsNullOrDBNull(Condition["CheckDate_EndTime"]))
            {
                strSql.Append(" AND CheckDate Between @CheckDate_StartTime AND @CheckDate_EndTime ");
                parameter.Add(DbFactory.CreateDbParameter("@CheckDate_StartTime", CommonHelper.GetDateTime(Condition["CheckDate_StartTime"] + " :00")));
                parameter.Add(DbFactory.CreateDbParameter("@CheckDate_EndTime", CommonHelper.GetDateTime(Condition["CheckDate_EndTime"] + " :59")));
            }
            //订单类型
            if (!HashtableHelper.IsNullOrDBNull(Condition["OrderType"]))
            {
                strSql.Append(" AND OrderType = @OrderType");
                parameter.Add(DbFactory.CreateDbParameter("@OrderType", Condition["OrderType"].ToString()));
            }
            //审核状态
            if (!HashtableHelper.IsNullOrDBNull(Condition["CheckStatus"]))
            {
                strSql.Append(" AND CheckStatus = @CheckStatus");
                parameter.Add(DbFactory.CreateDbParameter("@CheckStatus", Condition["CheckStatus"].ToString()));
            }
            return Repository().FindListPageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
    }
}