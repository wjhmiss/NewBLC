//=====================================================================================
// All Rights Reserved , Copyright @ DNS 2015
// Software Developers @ DNS 2015
//=====================================================================================

using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace DNS.Business
{
    /// <summary>
    /// 发运单明细
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.24 16:30</date>
    /// </author>
    /// </summary>
    public class BY_DeliveryDetailBll : RepositoryFactory<BY_DeliveryDetail>
    {
        /// <summary>
        /// 获得发运单明细
        /// </summary>
        /// <param name="DeliveryCode">发运单号</param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GetPageList(string LType,string DeliveryCode, string MnemonicCode, string CustomerName, string ProductName, string Size, string StartTime, string EndTime, string SteelCode, string Technology, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"  SELECT dd.DeliveryDetailId ,
        d.DeliveryCode ,
        dd.LineCode ,
        dd.Qty ,
        dd.Weight ,
        dd.MaterialPrice ,
        dd.MaterialAmount ,
        dd.WorkPrice ,
        dd.WorkAmount ,
        dd.SmallCost ,
        dd.SmallAmount ,
        dd.SalesAmount ,
        d.CreateDate ,
        d.MnemonicCode ,
        d.ComeBackUserId ,
        d.AuditUserId ,
        d.GetedAmount ,
        dd.DDCostPer ,
        dd.DDCost ,
        ( SELECT    bt.GetedAmount
          FROM      dbo.BY_CheckSheet bt
          WHERE     bt.CheckSheetCode = d.CheckSheetCode
        ) AS CSGetedAmount ,
        dd.TheoryWeight ,
        dd.Thick ,
        dd.Weigh ,
        dd.Length ,
        dd.HeatPrice ,
        dd.HeatAmount ,
        dd.DeliveryId ,
        d.CustomerName ,        d.shortname ,
dd.CustomerProduct,  dd.LJtype1 + '-' + dd.LJtype2 +'('+dd.LJsize+')'  'LJsize',
        dd.JKCostPer ,
        dd.JKCost ,
        dd.Remark ,
        dd.ProductName ,
        dd.ProjectCode ,
        qd.brand,dd.orderdetailid,dd.steelcode,dd.steeldescription,dd.technology,(SELECT top 1 CompleteUserId FROM BY_OrderComplete o WHERE o.OrderDetailId= dd.OrderDetailId and o.DeliveryCode = d.DeliveryCode) CompleteUserId,
(SELECT top 1 CompleteDate FROM BY_OrderComplete o WHERE o.OrderDetailId= dd.OrderDetailId and o.DeliveryCode = d.DeliveryCode) CompleteDate,d.customercode,dd.shipdescription,d.SalesMan
 FROM   BY_Delivery d
        INNER JOIN BY_DeliveryDetail dd ON d.DeliveryId = dd.DeliveryId
        LEFT JOIN dbo.BY_QuotationDetail qd ON dd.QuotationDetailId = qd.QuotationDetailId
 WHERE  1 = 1  and d.DeleteMark = 0 and d.Enabled >0  ");
            if (!string.IsNullOrEmpty(LType))
            {
                strSql.Append(" AND d.LType  = @LType ");
                parameter.Add(DbFactory.CreateDbParameter("@LType", LType));
            }
            if (!string.IsNullOrEmpty(StartTime))
            {
                strSql.Append(" AND d.CreateDate  > @StartTime ");
                parameter.Add(DbFactory.CreateDbParameter("@StartTime", Convert.ToDateTime(StartTime + " 00:00:00")));
            }
            if (!string.IsNullOrEmpty(EndTime))
            {
                strSql.Append(" AND d.CreateDate  < @EndTime ");
                parameter.Add(DbFactory.CreateDbParameter("@EndTime", Convert.ToDateTime(EndTime + " 23:59:59")));
            }
            if (!string.IsNullOrEmpty(DeliveryCode))
            {
                strSql.Append(" AND d.DeliveryCode like @DeliveryCode ");
                parameter.Add(DbFactory.CreateDbParameter("@DeliveryCode", "%" + DeliveryCode + "%"));
            }
            if (!string.IsNullOrEmpty(MnemonicCode))
            {
                strSql.Append(" AND ( d.MnemonicCode like @MnemonicCode OR   d.QuotationCode like @MnemonicCode  )");
                parameter.Add(DbFactory.CreateDbParameter("@MnemonicCode", "%" + MnemonicCode + "%"));
            }
            if (!string.IsNullOrEmpty(CustomerName))
            {
                strSql.Append(" AND ( d.CustomerName LIKE @CustomerName or d.CustomerCode LIKE @CustomerName or d.ShortName LIKE @CustomerName ) ");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerName", "%" + CustomerName + "%"));
            }
            if (!string.IsNullOrEmpty(ProductName))
            {
                strSql.Append(" AND dd.ProductName like @ProductName ");
                parameter.Add(DbFactory.CreateDbParameter("@ProductName", "%" + ProductName + "%"));
            }

            if (!string.IsNullOrEmpty(SteelCode))
            {
                strSql.Append(" AND dd.SteelCode like @SteelCode ");
                parameter.Add(DbFactory.CreateDbParameter("@SteelCode", "%" + SteelCode + "%"));
            }
            if (!string.IsNullOrEmpty(Technology))
            {
                strSql.Append(" AND dd.Technology like @Technology ");
                parameter.Add(DbFactory.CreateDbParameter("@Technology", "%" + Technology + "%"));
            }


            if (!string.IsNullOrEmpty(Size))
            {
                string[] Sizes = Size.Replace(" ", "").Split('*');
                if (Size.IndexOf('Φ') > -1)
                {
                    strSql.Append(@" AND dd.Length like @Length AND dd.Weigh=0 AND dd.Thick like @Thick  ");
                    parameter.Add(DbFactory.CreateDbParameter("@Length", Sizes[0].Replace("Φ", "") + "%"));
                    parameter.Add(DbFactory.CreateDbParameter("@Thick", Sizes[1].Replace("Φ", "") + "%"));
                }
                else
                {
                    strSql.Append(@"  AND ((dd.Length like @Length AND dd.Weigh like @Weigh AND dd.Thick like @Thick) or  
(dd.Length like @Length AND dd.Weigh like @Thick AND dd.Thick like @Weigh) or 

(dd.Length like @Weigh AND dd.Weigh like @Length AND dd.Thick like @Thick) or 

(dd.Length like @Thick AND dd.Weigh like @Weigh AND dd.Thick like @Length) or 

(dd.Length like @Weigh AND dd.Weigh like @Thick AND dd.Thick like @Length) or 

(dd.Length like @Thick AND dd.Weigh like @Weigh AND dd.Thick like @Length)  ) ");
                    parameter.Add(DbFactory.CreateDbParameter("@Thick", Sizes[2] + "%"));
                    parameter.Add(DbFactory.CreateDbParameter("@Weigh", Sizes[1] + "%"));
                    parameter.Add(DbFactory.CreateDbParameter("@Length", Sizes[0] + "%"));
                }
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        /// <summary>
        /// 获得发运单明细
        /// </summary>
        /// <param name="DeliveryCode">发运单号</param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GetPageList2(string DeliveryCode, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"
SELECT
	dd.SteelCode + '圆' SteelCode,
	dd.MaterialPrice,
	SUM(dd.Weight) Weight
FROM BY_Delivery d
INNER JOIN BY_DeliveryDetail dd
	ON d.DeliveryId = dd.DeliveryId
WHERE 1 = 1 AND dd.Weigh = 0  and d.DeleteMark = 0 ");
            strSql.Append(@" AND d.DeliveryCode = '" + DeliveryCode + "' ");
            strSql.Append(@" GROUP BY	dd.SteelCode,
			dd.MaterialPrice UNION ALL SELECT
	dd.SteelCode + '板' SteelCode,
	dd.MaterialPrice,
	SUM(dd.Weight) Weight
FROM BY_Delivery d
INNER JOIN BY_DeliveryDetail dd
	ON d.DeliveryId = dd.DeliveryId
WHERE 1 = 1 AND dd.Weigh > 0  and d.DeleteMark = 0  ");
            strSql.Append(@" AND d.DeliveryCode = '" + DeliveryCode + "' ");
            strSql.Append(@"GROUP BY	dd.SteelCode,
			dd.MaterialPrice ");


            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }

        /// <summary>
        /// 获得发运单明细
        /// </summary>
        /// <param name="DeliveryCode">发运单号</param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public List<BY_DeliveryDetail> GetPageList1(string CheckSheetId, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@" SELECT  dd.ProductName ,dd.SteelCode ,
        dd.MaterialPrice ,
        SUM(dd.Qty)  Qty,
        SUM(dd.Weight)  Weight,
        SUM(dd.MaterialAmount) MaterialAmount
        --SUM(dd.WorkAmount) WorkAmount,
        --SUM(dd.HeatAmount) HeatAmount,
        --SUM(dd.SmallAmount) SmallAmount,
        --SUM(dd.DDCost) DDCost,
        --SUM(dd.JKCost) JKCost
FROM    BY_DeliveryDetail dd
        INNER JOIN BY_Delivery d ON dd.DeliveryId = d.DeliveryId
        INNER JOIN BY_CheckSheet cs ON d.CheckSheetCode = cs.CheckSheetCode
WHERE   cs.CheckSheetId = '{0}'   and d.Enabled > 0 and d.DeleteMark = 0 and dd.SteelCode != 'FY' 
GROUP BY dd.ProductName ,dd.SteelCode ,
        dd.MaterialPrice

         ", CheckSheetId);
            return Repository().FindListPageBySql(strSql.ToString(), ref jqgridparam);
            //<BY_DeliveryDetail>(strSql.ToString(), ref jqgridparam);
        }
        public List<BY_DeliveryDetail> GetPageList11(string CheckSheetId, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@" SELECT dd.*
FROM    BY_DeliveryDetail dd
        INNER JOIN BY_Delivery d ON dd.DeliveryId = d.DeliveryId
        INNER JOIN BY_CheckSheet cs ON d.CheckSheetCode = cs.CheckSheetCode
WHERE   cs.CheckSheetId = '{0}'  and  d.Enabled > 0  and d.DeleteMark = 0 

         ", CheckSheetId);
            return Repository().FindListPageBySql(strSql.ToString(), ref jqgridparam);
            //<BY_DeliveryDetail>(strSql.ToString(), ref jqgridparam);
        }
    }

}
