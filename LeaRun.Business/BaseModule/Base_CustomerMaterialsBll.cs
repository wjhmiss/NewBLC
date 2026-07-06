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
    /// 客户材料费折扣表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.15 10:54</date>
    /// </author>
    /// </summary>
    public class Base_CustomerMaterialsBll : RepositoryFactory<Base_CustomerMaterials>
    {
        /// <summary>
        /// 材料费折扣列表
        /// </summary>
        /// <param name="CustomerId">客户主键</param>
        /// <returns></returns>
//        public DataTable GetList(string CustomerId)
//        {
//            StringBuilder strSql = new StringBuilder();
//            List<DbParameter> parameter = new List<DbParameter>();
//            strSql.Append(@"SELECT  cm.CustomerMaterialsId ,
//                                    cm.SteelCode ,
//                                    cm.SteelName ,
//                                    cm.ProcessType,
//                                    cm.CommonCode ,
//                                    cm.Unit ,
//                                    cm.DiscountRate ,
//                                    cm.EnabledTimeStart ,
//                                    cm.EnabledTimeEnd ,
//                                    cm.ENABLED ,
//                                    cm.Remark,cm.Price,
//                                    SP.SizeStart,
//                                    SP.SizeEnd
//                            FROM    Base_CustomerMaterials cm LEFT JOIN 
//                            Base_SteelPrice SP ON SP.SteelPriceId = cm.SteelPriceId
//                            WHERE   1 = 1
//                                    AND cm.CustomerId =@CustomerId");
//            parameter.Add(DbFactory.CreateDbParameter("@CustomerId", CustomerId));
//            return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
//        }
        public List<Base_CustomerMaterials> GetList(string CustomerId,string SteelCode, ref JqGridParam jqgridparam)
        {
            if (string.IsNullOrEmpty(SteelCode))
            {
                return Repository().FindListPageBySql(string.Format(" select * from Base_CustomerMaterials where CustomerId='{0}' ", CustomerId), ref jqgridparam);
            }
            return Repository().FindListPageBySql(string.Format(" select * from Base_CustomerMaterials where CustomerId='{0}' and (SteelCode like '%{1}%' or SteelName like '%{1}%' )", CustomerId, SteelCode), ref jqgridparam);
        }
        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
//        public bool IsExists(Base_CustomerMaterials entity)
//        {
//            StringBuilder strSql = new StringBuilder();
//            List<DbParameter> parameter = new List<DbParameter>();
//            strSql.Append(@"SELECT COUNT(1) FROM Base_CustomerMaterials WHERE 1 = 1  
//                                    AND CustomerId = @CustomerId 
//									AND SteelPriceId = @SteelPriceId
//                                    AND ProcessType = @ProcessType
//								    AND (EnabledTimeStart  BETWEEN @EnabledTimeStart AND @EnabledTimeEnd
//								    or EnabledTimeEnd  BETWEEN @EnabledTimeStart AND @EnabledTimeEnd)");
//            if (!string.IsNullOrEmpty(entity.CustomerMaterialsId))
//            {
//                strSql.Append(" AND CustomerMaterialsId != @CustomerMaterialsId");
//                parameter.Add(DbFactory.CreateDbParameter("@CustomerMaterialsId", entity.CustomerMaterialsId));
//            }
//            parameter.Add(DbFactory.CreateDbParameter("@CustomerId", entity.CustomerId));
//            parameter.Add(DbFactory.CreateDbParameter("@SteelPriceId", entity.SteelPriceId));
//            parameter.Add(DbFactory.CreateDbParameter("@ProcessType", entity.ProcessType));
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