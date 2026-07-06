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
using System.Data.Common;
using System.Text;

namespace DNS.Business
{
    /// <summary>
    /// 客户加工费折扣表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.15 10:55</date>
    /// </author>
    /// </summary>
    public class Base_CustomerProcessBll : RepositoryFactory<Base_CustomerProcess>
    {
        /// <summary>
        /// 加工费折扣列表
        /// </summary>
        /// <param name="CustomerId">客户主键</param>
        /// <returns></returns>
        public List<Base_CustomerProcess> GetList(string CustomerId,string rrradio,string SteelCode,ref JqGridParam jqgridparam)
        {
            if (!string.IsNullOrEmpty(rrradio))
            {
                rrradio = " and " + rrradio + "=1 ";
            }
            return Repository().FindListPageBySql(string.Format(" select * from Base_CustomerProcess where CustomerId = '{0}' {1} {2} "
                , CustomerId, string.IsNullOrEmpty(SteelCode) ? "" : " and SteelCode  like '" + SteelCode + "%'"
                , rrradio), ref jqgridparam);
        }
        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
//        public bool IsExists(Base_CustomerProcess entity)
//        {
//            StringBuilder strSql = new StringBuilder();
//            List<DbParameter> parameter = new List<DbParameter>();
//            strSql.Append(@"SELECT COUNT(1) FROM Base_CustomerProcess WHERE 1 = 1  
//                                    AND CustomerId = @CustomerId
//									AND SteelCode = @SteelCode
//
//                                    AND ((( @PachyStart BETWEEN PachyStart and PachyEnd ) 
//									or (@PachyEnd  BETWEEN PachyStart and PachyEnd ))
//								   
//								    AND ((@WidthEnd BETWEEN WidthStart and WidthEnd  )
//								    or (@WidthEnd  BETWEEN WidthStart and WidthEnd ))
//								    
//								    AND ((@LengthStart BETWEEN LengthStart and LengthEnd )
//								    or (@LengthEnd BETWEEN LengthStart and LengthEnd  )))--判断规格是否重复
//
//								    AND(EnabledTimeStart  BETWEEN @EnabledTimeStart AND @EnabledTimeEnd
//								    OR EnabledTimeEnd  BETWEEN @EnabledTimeStart AND @EnabledTimeEnd)");
//            if (!string.IsNullOrEmpty(entity.CustomerProcessId))
//            {
//                strSql.Append(" AND CustomerProcessId != @CustomerProcessId");
//                parameter.Add(DbFactory.CreateDbParameter("@CustomerProcessId", entity.CustomerProcessId));
//            }
//            parameter.Add(DbFactory.CreateDbParameter("@CustomerId", entity.CustomerId));
//            parameter.Add(DbFactory.CreateDbParameter("@SteelCode", entity.SteelCode));
//            parameter.Add(DbFactory.CreateDbParameter("@PachyStart", entity.PachyStart));
//            parameter.Add(DbFactory.CreateDbParameter("@PachyEnd", entity.PachyEnd));
//            parameter.Add(DbFactory.CreateDbParameter("@WidthStart", entity.WidthStart));
//            parameter.Add(DbFactory.CreateDbParameter("@WidthEnd", entity.WidthEnd));
//            parameter.Add(DbFactory.CreateDbParameter("@LengthStart", entity.LengthStart));
//            parameter.Add(DbFactory.CreateDbParameter("@LengthEnd", entity.LengthEnd));
//            parameter.Add(DbFactory.CreateDbParameter("@EnabledTimeStart", entity.EnabledTimeStart));
//            parameter.Add(DbFactory.CreateDbParameter("@EnabledTimeEnd", entity.EnabledTimeEnd));
//            int Count = Repository().FindCountBySql(strSql.ToString(), parameter.ToArray());
//            if (Count > 0)
//            {
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }
    }
}