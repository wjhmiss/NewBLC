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
    /// 库存表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.18 16:01</date>
    /// </author>
    /// </summary>
    public class BY_InventoryBll : RepositoryFactory<BY_Inventory>
    {
        /// <summary>
        /// 根据批号获取列表
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public List<BY_Inventory> GetList(string BatchCode, string InventoryType)
        {
            if (string.IsNullOrEmpty(BatchCode))
            {
                if (string.IsNullOrEmpty(InventoryType))
                {
                    return Repository().FindList(" and Enabled > 0");
                }
                return Repository().FindList(string.Format("  and Enabled > 0 and MaterialType='{0}'", InventoryType));
            }
            else
            {
                if (string.IsNullOrEmpty(InventoryType))
                {
                    return Repository().FindListBySql(string.Format("Select * from BY_Inventory Where Enabled > 0 and BatchCode like '{0}%'", BatchCode));
                }
                return Repository().FindListBySql(string.Format("Select * from BY_Inventory Where Enabled > 0 and BatchCode like '{0}%' and MaterialType='" + InventoryType + "'", BatchCode));
            }
        }
        /// <summary>
        /// 替代料选择
        /// </summary>
        /// <param name="BatchCode"></param>
        /// <param name="InventoryType"></param>
        /// <param name="batchcodereplace"></param>
        /// <returns></returns>
        public List<BY_Inventory> GetListforReplace(string BatchCode, string InventoryType, string batchcodereplace)
        {
            return Repository().FindListBySql(@" SELECT top 10 i.InventoryId ,
        SteelCode ,
        SteelDescription ,
        BatchCode ,
        Thick ,
        Weigh ,
        Length ,
        Weight ,
        InStockDate ,
        StoveCode ,
        i.Remark ,
        i.CreateUserName ,
        i.OriginalSize ,
        i.CustomerCode ,
        i.CustomerName ,
        i.BoreSize ,
        i.steelpurchasecode ,
        i.price ,
        i.TypeName ,
        RemarkSteelperchase ,
        Weight * Price AS totalmoney,i.Density,i.position
FROM    dbo.BY_Inventory i
WHERE   InStockDate IS NOT NULL
        AND Density > 0
        AND BatchCode != '&nbsp;'
        AND BatchCode != ''
        AND BatchCode IS NOT NULL
        AND i.Enabled > 0
        AND ( i.Length > 0
              OR MaterialType = 1
            ) and InStockDate IS NOT NULL AND Enabled > 0 AND Weight > 0 AND BatchCode LIKE '%" + BatchCode + "%' ");
            //Base_Steel base_steel = DataFactory.Database().FindEntity<Base_Steel>("SteelCode", batchcodereplace);
            //string str = "'" + batchcodereplace + "'";
            //if (base_steel != null && base_steel.RepalceMl != null)
            //{
            //    string[] strreplaces = base_steel.RepalceMl.Split(',');
            //    for (int i = 0; i < strreplaces.Length; i++)
            //    {
            //        str = str + ",'" + strreplaces[i] + "'";
            //    }
            //}

            //if (string.IsNullOrEmpty(BatchCode))
            //{
            //    if (string.IsNullOrEmpty(InventoryType))
            //    {
            //        return Repository().FindList(" and  SteelCode IN (" + str + ") and InStockDate IS NOT NULL AND Enabled > 0  ");
            //    }
            //    return Repository().FindList(" and InStockDate IS NOT NULL AND Enabled > 0  and  SteelCode IN (" + str + ")  and MaterialType='" + InventoryType + "'");
            //}
            //else
            //{
            //    if (string.IsNullOrEmpty(InventoryType))
            //    {
            //        return Repository().FindListBySql(string.Format("Select * from BY_Inventory Where  InStockDate IS NOT NULL AND Enabled > 0  and BatchCode like '%{0}%'  and  SteelCode IN (" + str + ") ", BatchCode));
            //    }
            //    return Repository().FindListBySql(string.Format("Select * from BY_Inventory Where  InStockDate IS NOT NULL AND Enabled > 0  and BatchCode like '%{0}%' and MaterialType='" + InventoryType + "' and  SteelCode IN (" + str + ") ", BatchCode));
            //}
        }
        public DataTable GetDistinctBatchCode(string BatchCode, string InventoryType,string ParentInventoryId ,string Enabled)
        {
            string sql = "Select DISTINCT * from BY_Inventory Where  InStockDate IS NOT NULL ";
            if (!string.IsNullOrEmpty(BatchCode))
            {
                sql += "and BatchCode like '"+ BatchCode + "%'";
            }
            if (!string.IsNullOrEmpty(InventoryType))
            {
                sql += " and MaterialType='" + InventoryType + "'";
            }
            if (!string.IsNullOrEmpty(ParentInventoryId))
            {
                sql += " and ParentInventoryId='" + ParentInventoryId + "'";
            }
            if (!string.IsNullOrEmpty(Enabled))
            {
                sql += " and Enabled=" + Enabled + "";
            }
            return Repository().FindTableBySql(sql);
        }

        public DataTable GetSmoney()
        {
            return Repository().FindTableBySql(string.Format(@" SELECT
	SUM(Ttotalmoney) Ttotalmoney
FROM (SELECT
	CASE
		WHEN Price > 0 THEN Weight * Price ELSE Weight * (SELECT
		s.Price
	FROM Base_Steel s
	WHERE s.SteelCode = BY_Inventory.SteelCode)
	END
	AS Ttotalmoney
FROM BY_Inventory
WHERE InStockDate IS NOT NULL
AND Enabled > 0
AND Weight > 0)
a


"));
        }
        /// <summary>
        /// 获得页面列表
        /// </summary>
        /// <param name="MaterialType">0-条料，1-碎料</param>
        /// <param name="SteelCode">钢种</param>
        /// <param name="BatchCode">钢材编号</param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GetPageList(int MaterialType, string SteelCode, string BatchCode, string Thick, string Weigh, string Length, string alarmck, string SteelPurchaseCode, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"
SELECT  i.InventoryId ,
        SteelCode ,
        SteelDescription ,
        BatchCode ,
        Thick ,
        Weigh ,
        Length ,
        Weight ,
        InStockDate ,
        StoveCode ,
        i.Remark ,
        i.CreateUserName ,
        i.OriginalSize ,
        i.CustomerCode ,
        i.CustomerName ,
        i.BoreSize ,
        i.steelpurchasecode ,
        i.price ,
        i.TypeName ,
        RemarkSteelperchase ,
        Weight * Price AS totalmoney,i.position
FROM    dbo.BY_Inventory i
WHERE   InStockDate IS NOT NULL
        AND i.Enabled > 0
        AND BatchCode != '&nbsp;'
        AND BatchCode != ''
        AND BatchCode IS NOT NULL
@SteelCode
@BatchCode
@SteelPurchaseCode
@Thick
@Weigh
@Length
@alarmck

        
");
            if (!string.IsNullOrEmpty(SteelCode))
            {
                strSql.Replace("@SteelCode", " AND (i.SteelCode like @SteelCode or i.SteelDescription like @SteelCode)");
                parameter.Add(DbFactory.CreateDbParameter("@SteelCode", SteelCode + "%"));
            }
            else
            {
                strSql.Replace("@SteelCode", " ");

            }

            if (!string.IsNullOrEmpty(BatchCode))
            {
                strSql.Replace("@BatchCode", "  AND i.BatchCode like @BatchCode ");
                parameter.Add(DbFactory.CreateDbParameter("@BatchCode", "%" + BatchCode + "%"));
            }
            else
            {
                strSql.Replace("@BatchCode", " ");

            } 
            
            if (!string.IsNullOrEmpty(SteelPurchaseCode))
            {
                strSql.Replace("@SteelPurchaseCode", "  AND i.SteelPurchaseCode like @SteelPurchaseCode ");
                parameter.Add(DbFactory.CreateDbParameter("@SteelPurchaseCode", "%" + BatchCode + "%"));
            }
            else
            {
                strSql.Replace("@SteelPurchaseCode", " ");

            }
            if (!string.IsNullOrEmpty(Thick))
            {
                strSql.Replace("@Thick", "  AND i.Thick = @Thick ");
                parameter.Add(DbFactory.CreateDbParameter("@Thick", CommonHelper.GetDecimal(Thick)));
            }
            else
            {
                strSql.Replace("@Thick", " ");

            }
            if (!string.IsNullOrEmpty(Weigh))
            {
                strSql.Replace("@Weigh", "  AND i.Weigh = @Weigh ");
                parameter.Add(DbFactory.CreateDbParameter("@Weigh", CommonHelper.GetDecimal(Weigh)));
            }
            else
            {
                strSql.Replace("@Weigh", " ");

            }
            if (!string.IsNullOrEmpty(Length))
            {
                strSql.Replace("@Length", "  AND i.Length = @Length ");
                parameter.Add(DbFactory.CreateDbParameter("@Length", CommonHelper.GetDecimal(Length)));
            }
            else
            {
                strSql.Replace("@Length", " ");

            }

            if (!string.IsNullOrEmpty(alarmck) && alarmck == "checked")
            {
//                strSql.Append(@" AND EXISTS ( SELECT 1
//                     FROM   dbo.BY_Inventory i1
//                     WHERE  i1.MaterialType = 0
//                            AND i1.SteelCode = i.SteelCode
//                            AND i1.Thick = i.Thick
//                            AND i1.Weigh = i.Weigh
//                            AND i1.Weigh <>0
//                     GROUP BY i1.SteelCode ,
//                            i1.Thick ,
//                            i1.Weigh
//                     HAVING MAX(i1.Length) < 1000 
//                     union ALL
//                     SELECT 1
//                     FROM   dbo.BY_Inventory i1
//                     WHERE  i1.MaterialType = 0
//                            AND i1.SteelCode = i.SteelCode
//                            AND i1.Thick = i.Thick
//                            AND i1.Weigh = i.Weigh
//                            AND i1.Weigh =0
//                     GROUP BY i1.SteelCode ,
//                            i1.Thick ,
//                            i1.Weigh
//                     HAVING MAX(i1.Length) < 3000 
//                     )  ");
                strSql.Replace("@alarmck", @"  AND ISNULL(( SELECT SUM(ISNULL(s.Weight, 0))
                 FROM   BY_InStock s
                 WHERE  s.steelcode = i.SteelCode
                        AND s.Thick = i.Thick
                        AND ( s.BillCode = '&nbsp;'
                              OR s.BillCode = ''
                              OR s.BillCode IS NULL
                            )
               ), 0) - ISNULL(( SELECT  SUM(ISNULL(s.Weight, 0))
                                FROM    BY_OutStock s
                                WHERE   s.steelcode = i.SteelCode
                                        AND s.OpenThick = i.Thick
                                        AND ( s.BillCode = '&nbsp;'
                                              OR s.BillCode = ''
                                              OR s.BillCode IS NULL
                                            )
                              ), 0) < 1000 ");

            }
            else
            {
                strSql.Replace("@alarmck", " ");

            }


            if (DNS.Utilities.ManageProvider.Provider.Current().IsMobileUser && DNS.Utilities.ManageProvider.Provider.Current().ObjectId.IndexOf("20178431-3604-46b6-a75b-f7ba13548aed") < 0)
            {
                strSql.Append(" AND 1=2 ");
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }

        /// <summary>
        /// 根据批号和子批号获取库存实体
        /// </summary>
        /// <param name="BatchCode"></param>
        /// <param name="ChildBatch"></param>
        /// <returns></returns>
        public BY_Inventory GetEntity(string BatchCode, string ChildBatch)
        {
            List<DbParameter> parameter = new List<DbParameter>();
            string slq = "";
            if (!string.IsNullOrEmpty(BatchCode))
            {
                slq += " and BatchCode=@BatchCode ";
                parameter.Add(DbFactory.CreateDbParameter("@BatchCode", BatchCode));
            }
            if (!string.IsNullOrEmpty(ChildBatch))
            {
                slq += " and ChildBatch=@ChildBatch ";
                parameter.Add(DbFactory.CreateDbParameter("@ChildBatch", ChildBatch));
            }
            return Repository().FindEntityByWhere(slq, parameter.ToArray());
        }
    }
}