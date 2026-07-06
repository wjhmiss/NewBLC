//=====================================================================================
// All Rights Reserved , Copyright @ DNS 2015
// Software Developers @ DNS 2015
//=====================================================================================

using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using DNS.DataAccess;
using System;
using System.Linq;

namespace DNS.Business
{
    /// <summary>
    /// 入仓表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.17 14:41</date>
    /// </author>
    /// </summary>
    public class BY_InStockBll : RepositoryFactory<BY_InStock>
    {
        /// <summary>
        /// 入仓材料查询
        /// </summary>
        /// <param name="InStockType">入库类型</param>
        /// <param name="jqgridparam">分页条件</param>
        /// <returns></returns>
        public DataTable GetPageList(string InStockType, DateTime StartTime, DateTime EndTime, string SteelCode, string CustomerCode, string BatchCode, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  *,weight*price as totmal
FROM    BY_InStock i
WHERE   1=1 
        ");
            //if (!string.IsNullOrEmpty(InStockType))
            //{
            //    if (InStockType == "-1")
            //    {
            //        //strSql.Append("  AND ( i.InStockType <=1 ) ");
            //    }
            //    else
            //    {
            //        strSql.Append(" AND i.InStockType = @InStockType");
            //        parameter.Add(DbFactory.CreateDbParameter("@InStockType", InStockType));
            //    }
            //}
            if (StartTime != null)
            {
                strSql.Append(" AND i.CreateDate>= @StartTime");
                parameter.Add(DbFactory.CreateDbParameter("@StartTime", StartTime));
            }
            if (EndTime != null)
            {
                strSql.Append(" AND i.CreateDate<= @EndTime");
                parameter.Add(DbFactory.CreateDbParameter("@EndTime", EndTime.AddDays(1)));
            }
            if (!string.IsNullOrEmpty(SteelCode))
            {
                strSql.Append(" AND i.SteelCode = @SteelCode");
                parameter.Add(DbFactory.CreateDbParameter("@SteelCode", SteelCode));
            }
            if (!string.IsNullOrEmpty(CustomerCode))
            {
                strSql.Append(" AND i.CustomerCode = @CustomerCode");
                parameter.Add(DbFactory.CreateDbParameter("@CustomerCode", CustomerCode));
            }
            if (!string.IsNullOrEmpty(BatchCode))
            {
                strSql.Append(" AND i.BillCode = @BatchCode");
                parameter.Add(DbFactory.CreateDbParameter("@BatchCode", BatchCode));
            }

            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        public DataTable GetPageListInventory(string InStockType, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  *
FROM    BY_Inventory i
WHERE   Enabled = 0 and InStockDate is null and SteelCode != 'FY' 
        ");
            //if (!string.IsNullOrEmpty(InStockType))
            //{
            //    if (InStockType == "-1")
            //    {
            //        //strSql.Append("  AND ( i.InStockType <=1 ) ");
            //    }
            //    else
            //    {
            //        strSql.Append(" AND i.InStockType = @InStockType");
            //        parameter.Add(DbFactory.CreateDbParameter("@InStockType", InStockType));
            //    }
            //}


            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        /// <summary>
        /// 获得最新入库单号
        /// </summary>
        /// <param name="by_instock">入库信息</param>
        /// <returns>入库单号</returns>
        public string GetInStockCode(BY_InStock by_instock)
        {

            string seriousCode = "001";

            string spec = "";
            if (by_instock.Weigh != 0)
            {
                spec = by_instock.SteelCode + "-" + by_instock.Thick + "X" + by_instock.Weigh + "-";
            }
            else
            {
                spec = by_instock.SteelCode + "-" + by_instock.Thick + "-";

            }
            BY_Inventory maxBY_InStock = DataFactory.Database().FindEntityBySql<BY_Inventory>(string.Format("SELECT TOP 1 * FROM BY_Inventory WHERE BatchCode LIKE '{0}___' ORDER BY BatchCode DESC      ", spec));
            //如果存在同种材质的钢种先取出流水号
            if (maxBY_InStock.BatchCode != null)
            {
                //取出流水号
                seriousCode = maxBY_InStock.BatchCode.Substring(maxBY_InStock.BatchCode.Length - 3, 3);
                //流水号+1
                seriousCode = (int.Parse(seriousCode) + 1).ToString().PadLeft(3, '0');
            }
            return spec + seriousCode;
        }
        /// <summary>
        /// 获得最新入库单号
        /// </summary>
        /// <param name="by_instock">入库信息</param>
        /// <returns>入库单号</returns>
        //public List<BY_InStock> GetInStockCode(List<BY_InStock> by_instocks)
        //{
        //    ArrayList tempArray = new ArrayList();
        //    foreach (BY_InStock by_instock in by_instocks)
        //    {
        //        string seriousCode = "00001";
        //        string spec = by_instock.SteelCode + "-" + by_instock.CustomerCode;
        //        if (!tempArray.Contains(spec))
        //        {
        //            BY_InStock maxBY_InStock = Repository().FindEntityBySql(string.Format("SELECT TOP 1 * FROM   BY_InStock WHERE BillCode LIKE '{0}_____' ORDER BY CreateDate DESC   ", spec));
        //            //如果存在同种材质的钢种先取出流水号
        //            if (maxBY_InStock.BillCode != null)
        //            {
        //                //取出流水号
        //                seriousCode = maxBY_InStock.BillCode.Substring(maxBY_InStock.BillCode.Length - 5, 5);
        //                //流水号+1
        //                seriousCode = (int.Parse(seriousCode) + 1).ToString().PadLeft(5 , '0');
        //            }
        //            tempArray.Add(spec);
        //        }
        //        else
        //        {
        //            string temp = ReturnArrB(tempArray, spec).ToString();
        //            seriousCode = temp.PadLeft(5,'0');
        //        }
        //        by_instock.BillCode = spec + seriousCode;
        //    }
        //    return by_instocks;
        //}
        private int ReturnArrB(List<Fcode> fcodes, string strTemp)
        {
            return Convert.ToInt32(fcodes.Where(p => p.spec == strTemp).Max(p => p.num)) + 1;
        }
        public class Fcode
        {
            public string spec { get; set; }
            public string num { get; set; }
        }
        public List<BY_Inventory> GetInventoryCode(List<BY_Inventory> by_instocks)
        {
            ArrayList tempArray = new ArrayList();
            List<Fcode> fcodes = new List<Fcode>();
            foreach (BY_Inventory by_instock in by_instocks)
            {
                Fcode fcode = new Fcode();
                string seriousCode = "001";
                string spec = "";
                if (by_instock.Weigh != 0)
                {
                    spec = by_instock.SteelCode + "-" + by_instock.Thick + "X" + by_instock.Weigh + "-";
                }
                else
                {
                    spec = by_instock.SteelCode + "-" + by_instock.Thick + "-";

                }
                if (!tempArray.Contains(spec))
                {
                    BY_Inventory maxBY_InStock = DataFactory.Database().FindEntityBySql<BY_Inventory>(string.Format("SELECT TOP 1 * FROM BY_Inventory WHERE BatchCode LIKE '{0}___' ORDER BY BatchCode DESC      ", spec));
                    //如果存在同种材质的钢种先取出流水号
                    if (maxBY_InStock.BatchCode != null)
                    {
                        //取出流水号
                        seriousCode = maxBY_InStock.BatchCode.Substring(maxBY_InStock.BatchCode.Length - 3, 3);
                        //流水号+1
                        seriousCode = (int.Parse(seriousCode) + 1).ToString().PadLeft(3, '0');
                    }
                    tempArray.Add(spec);
                }
                else
                {
                    string temp = ReturnArrB(fcodes, spec).ToString();
                    seriousCode = temp.PadLeft(3, '0');

                }
                fcode.spec = spec;
                fcode.num = seriousCode;
                fcodes.Add(fcode);
                by_instock.BatchCode = spec + seriousCode;
            }
            return by_instocks;
        }
        public string GetInventoryCode(BY_Inventory by_instock)
        {
            string seriousCode = "001";
            string spec = "";
            if (by_instock.Weigh != 0)
            {
                spec = by_instock.SteelCode + "-" + by_instock.Thick + "X" + by_instock.Weigh + "-";
            }
            else
            {
                spec = by_instock.SteelCode + "-" + by_instock.Thick + "-";

            }
            BY_Inventory maxBY_InStock = DataFactory.Database().FindEntityBySql<BY_Inventory>(string.Format("SELECT TOP 1 * FROM BY_Inventory WHERE BatchCode LIKE '{0}___' ORDER BY BatchCode DESC      ", spec));
            //如果存在同种材质的钢种先取出流水号
            if (maxBY_InStock.BatchCode != null)
            {
                //取出流水号
                seriousCode = maxBY_InStock.BatchCode.Substring(maxBY_InStock.BatchCode.Length - 3, 3);
                //流水号+1
                seriousCode = (int.Parse(seriousCode) + 1).ToString().PadLeft(3, '0');
            }
            return spec + seriousCode;
        }
        /// <summary>
        /// 获得入库列表
        /// </summary>
        /// <param name="BillCode"></param>
        /// <returns></returns>
        public List<BY_InStock> GetInStockList(string BillCode)
        {
            List<DbParameter> parameter = new List<DbParameter>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  top 10 * from BY_InStock  WHERE 1=1  ");
            if (!string.IsNullOrEmpty(BillCode))
            {
                strSql.Append(" and BillCode like @BillCode+'%' ");
                parameter.Add(DbFactory.CreateDbParameter("@BillCode", BillCode));
            }
            return Repository().FindListBySql(strSql.ToString(), parameter.ToArray());
        }
        /// <summary>
        /// 获得称重重量
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public Hashtable GetScale(string ip)
        {
            IDatabase database = DataFactory.Database();
            Hashtable ht = database.FindHashtableBySql(string.Format(" select top 1 scale from BY_Scale where ip='{0}'", ip));
            if (ht.Count == 0)
            {
                ht["scale"] = "0";
            }
            return ht;
        }
    }
}