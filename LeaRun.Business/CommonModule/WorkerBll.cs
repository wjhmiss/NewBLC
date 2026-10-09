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
    /// 员工表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.27 19:55</date>
    /// </author>
    /// </summary>
    public class WorkerBll : RepositoryFactory<BY_Worker>
    {
        /// <summary>
        /// 获得页面列表
        /// </summary>
        /// <param name="MaterialType">0-条料，1-碎料</param>
        /// <param name="SteelCode">钢种</param>
        /// <param name="BatchCode">钢材编号</param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GetPageList(string DepartId, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  WorkerId ,
                                    WorkerName ,
                                    WorkerCode ,
                                    Degree ,mobile,
                                    d.FullName AS DepartName
                            FROM    BY_Worker w
                                    INNER JOIN dbo.Base_Department d ON w.DepartId = d.DepartmentId 
                                                        WHERE   1 = 1 ");
            if (!string.IsNullOrEmpty(DepartId))
            {
                strSql.Append(" AND  w.DepartId = @MaterialType");
                parameter.Add(DbFactory.CreateDbParameter("@MaterialType", DepartId));
            }

            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        public DataTable GetPageList(string DepartId)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  WorkerId ,
                                    WorkerName ,
                                    WorkerCode ,
                                    Degree ,
                                    d.FullName AS DepartName
                            FROM    BY_Worker w
                                    INNER JOIN dbo.Base_Department d ON w.DepartId = d.DepartmentId 
                                                        WHERE   1 = 1 ");
            if (!string.IsNullOrEmpty(DepartId))
            {
                strSql.Append(" AND  w.DepartId = @MaterialType");
                parameter.Add(DbFactory.CreateDbParameter("@MaterialType", DepartId));
            }

            return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
        }

        public List<BY_Worker> GetAutoComple(string keywords, string DepartmentName=null)
        {
            StringBuilder WhereSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            if (!string.IsNullOrEmpty(keywords))
            {
                WhereSql.Append(@" AND (WorkerName LIKE @keyword
                                    OR WorkerCode LIKE @keyword
                                    )");
                parameter.Add(DbFactory.CreateDbParameter("@keyword", keywords + '%'));
            }
  //          if (!string.IsNullOrEmpty(DepartmentName))
  //          {
  //              WhereSql.Append(@" AND DepartId = (SELECT DepartmentId FROM Base_Department
  //WHERE FullName=@DepartmentName) ");
  //              parameter.Add(DbFactory.CreateDbParameter("@DepartmentName", DepartmentName));
  //          }
            return Repository().FindListTop(20, WhereSql.ToString(), parameter.ToArray());
        }

    }
}