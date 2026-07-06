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
    /// ¸Ö²Ä¹ºÂòÉêÇë
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.07.10 11:55</date>
    /// </author>
    /// </summary>
    public class Base_SteelPurchaseBll : RepositoryFactory<Base_SteelPurchase>
    {
        public DataTable GetPageList(int MaterialType, string SteelCode, string BatchCode, string Thick, string Weigh, string Length, string alarmck, string SteelPurchaseCode, string CustomerCode, string StartTime, string EndTime, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  i.InventoryId ,qty,
        SteelCode ,
        SteelDescription ,
        BatchCode ,
        oThick ,
        oWeigh ,
        oLength ,
        OWeight ,
        Weightcheng ,
        TheoryWeight ,
        InStockDate ,
        StoveCode ,
        i.Remark ,
        i.CreateUserName ,
        i.OriginalSize ,
        i.CustomerCode ,
        i.CustomerName ,
        i.BoreSize ,
        i.steelpurchasecode ,
        i.price,Enabled,TotalAmount,Weight,Length,i.CreateDate
FROM    dbo.BY_Inventory i
WHERE    Density > 0  AND MaterialType !=1 and i.steelpurchasecode is not null and i.steelpurchasecode != ''
    ");
            if (!string.IsNullOrEmpty(SteelPurchaseCode))
            {
                strSql.Append(" AND i.SteelPurchaseCode = @SteelPurchaseCode");
                parameter.Add(DbFactory.CreateDbParameter("@SteelPurchaseCode", SteelPurchaseCode));
            }

            if (!string.IsNullOrEmpty(StartTime))
            {
                strSql.Append(" AND i.instockdate >= @StartTime");
                parameter.Add(DbFactory.CreateDbParameter("@StartTime", StartTime));
            }
            if (!string.IsNullOrEmpty(EndTime))
            {
                strSql.Append(" AND i.instockdate <= @EndTime");
                parameter.Add(DbFactory.CreateDbParameter("@EndTime", EndTime));
            }

            if (MaterialType >= 0)
            {
                strSql.Append(" AND i.MaterialType = @MaterialType");
                parameter.Add(DbFactory.CreateDbParameter("@MaterialType", MaterialType));
            }
            if (!string.IsNullOrEmpty(SteelCode))
            {
                strSql.Append(" AND (i.SteelCode like @SteelCode or i.SteelDescription like @SteelCode)");
                parameter.Add(DbFactory.CreateDbParameter("@SteelCode", "%" + SteelCode + "%"));
            }
            if (!string.IsNullOrEmpty(CustomerCode))
            {
                strSql.Append(" AND (i.CustomerCode like @CustomerCode or i.CustomerName like @CustomerCode)");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerCode", "%"+CustomerCode + "%"));
            }
            if (!string.IsNullOrEmpty(BatchCode))
            {
                strSql.Append(" AND i.BatchCode like @BatchCode");
                parameter.Add(DbFactory.CreateDbParameter("@BatchCode", "%" + BatchCode + "%"));
            }
            if (!string.IsNullOrEmpty(Thick))
            {
                strSql.Append(" AND i.Thick >= @Thick1 AND i.Thick <=@Thick2");
                parameter.Add(DbFactory.CreateDbParameter("@Thick1", CommonHelper.GetDecimal(Thick) - 5));
                parameter.Add(DbFactory.CreateDbParameter("@Thick2", CommonHelper.GetDecimal(Thick) + 5));
            }
            if (!string.IsNullOrEmpty(Weigh))
            {
                strSql.Append(" AND i.Weigh>= @Weigh1 AND i.Weigh<=@Weigh2");
                parameter.Add(DbFactory.CreateDbParameter("@Weigh1", CommonHelper.GetDecimal(Weigh) - 5));
                parameter.Add(DbFactory.CreateDbParameter("@Weigh2", CommonHelper.GetDecimal(Weigh) + 5));
            }
            if (!string.IsNullOrEmpty(Length))
            {
                strSql.Append(" AND i.Length>= @Length1 AND i.Length<=@Length2");
                parameter.Add(DbFactory.CreateDbParameter("@Length1", CommonHelper.GetDecimal(Length) - 5));
                parameter.Add(DbFactory.CreateDbParameter("@Length2", CommonHelper.GetDecimal(Length) + 5));
            }
            if (!string.IsNullOrEmpty(alarmck) && alarmck == "checked")
            {
                strSql.Append(@" AND EXISTS ( SELECT 1
                     FROM   dbo.BY_Inventory i1
                     WHERE  i1.MaterialType = 0
                            AND i1.SteelCode = i.SteelCode
                            AND i1.Thick = i.Thick
                            AND i1.Weigh = i.Weigh
                            AND i1.Weigh <>0
                     GROUP BY i1.SteelCode ,
                            i1.Thick ,
                            i1.Weigh
                     HAVING MAX(i1.Length) < 1000 
                     union ALL
                     SELECT 1
                     FROM   dbo.BY_Inventory i1
                     WHERE  i1.MaterialType = 0
                            AND i1.SteelCode = i.SteelCode
                            AND i1.Thick = i.Thick
                            AND i1.Weigh = i.Weigh
                            AND i1.Weigh =0
                     GROUP BY i1.SteelCode ,
                            i1.Thick ,
                            i1.Weigh
                     HAVING MAX(i1.Length) < 3000 
                     )  ");
                //parameter.Add(DbFactory.CreateDbParameter("@alarmck", alarmck));
            }
            if (DNS.Utilities.ManageProvider.Provider.Current().IsMobileUser && DNS.Utilities.ManageProvider.Provider.Current().ObjectId.IndexOf("20178431-3604-46b6-a75b-f7ba13548aed") < 0)
            {
                strSql.Append(" AND 1=2 ");
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
    }
}