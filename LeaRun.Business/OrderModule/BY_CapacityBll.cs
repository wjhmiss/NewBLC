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
    /// 产能表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.20 14:10</date>
    /// </author>
    /// </summary>
    public class BY_CapacityBll : RepositoryFactory<BY_Capacity>
    {
        /// <summary>
        /// 获取产能列表
        /// </summary>
        /// <returns></returns>
        public List<BY_Capacity> GetList()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  CapacityType ,
                                    DayPart ,
                                    CapacityValue
                            FROM    BY_Capacity
                            WHERE   CONVERT(VARCHAR(10), CapacityDate, 120) = CONVERT(VARCHAR(10), GETDATE(), 120)
                            ORDER BY CapacityType,DayPart");
            return Repository().FindListBySql(strSql.ToString());
        }
        /// <summary>
        /// 设置产能
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int SetCapacity(BY_Capacity entity)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"UPDATE BY_Capacity
                            SET    CapacityValue = @CapacityValue
                            WHERE  CapacityDate >= CAST(CONVERT(VARCHAR(10), GETDATE(), 120) AS DATETIME)
                                AND DayPart = @DayPart
                                AND CapacityType = @CapacityType");
            DbParameter[] parameters = 
            {
                DbFactory.CreateDbParameter("@CapacityValue", entity.CapacityValue),
                DbFactory.CreateDbParameter("@DayPart", entity.DayPart),
                DbFactory.CreateDbParameter("@CapacityType",  entity.CapacityType)
            };
            return Repository().ExecuteBySql(strSql, parameters);
        }
        /// <summary>
        /// 产能看板列表
        /// </summary>
        /// <param name="StartTime">日期开始</param>
        /// <param name="EndTime">日期介绍</param>
        /// <returns></returns>
        public DataTable GetCapacityReportList(string StartTime, string EndTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  CapacityDate ,
                                    CapacityType ,
                                    SUM(CASE WHEN DayPart = 'A' THEN CapacityValue
                                             ELSE 0
                                        END) AS CapacityValue_A ,
                                    SUM(CASE WHEN DayPart = 'A' THEN BookedQty
                                             ELSE 0
                                        END) AS BookedQty_A ,
                                    SUM(CASE WHEN DayPart = 'B' THEN CapacityValue
                                             ELSE 0
                                        END) AS CapacityValue_B ,
                                    SUM(CASE WHEN DayPart = 'B' THEN BookedQty
                                             ELSE 0
                                        END) AS BookedQty_B ,
                                    SUM(CASE WHEN DayPart = 'C' THEN CapacityValue
                                             ELSE 0
                                        END) AS CapacityValue_C ,
                                    SUM(CASE WHEN DayPart = 'C' THEN BookedQty
                                             ELSE 0
                                        END) AS BookedQty_C ,
                                    SUM(CASE WHEN DayPart = 'D' THEN CapacityValue
                                             ELSE 0
                                        END) AS CapacityValue_D ,
                                    SUM(CASE WHEN DayPart = 'D' THEN BookedQty
                                             ELSE 0
                                        END) AS BookedQty_D
                            FROM    dbo.BY_Capacity
                            WHERE   CapacityDate BETWEEN @StartTime
                                                 AND     @EndTime
                            GROUP BY CapacityDate ,
                                    CapacityType
                            ORDER BY CapacityDate ,
                                    CapacityType");
            DbParameter[] parameters = 
            {
                DbFactory.CreateDbParameter("@StartTime", StartTime),
                DbFactory.CreateDbParameter("@EndTime", EndTime)
            };
            return Repository().FindTableBySql(strSql.ToString(), parameters);
        }
    }
}