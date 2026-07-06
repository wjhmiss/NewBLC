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

namespace DNS.Business
{
    /// <summary>
    /// 报价单明细
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.17 18:01</date>
    /// </author>
    /// </summary>
    public class BY_QuotationDetailBll : RepositoryFactory<BY_QuotationDetail>
    {
        /// <summary>
        /// 报价单明细列表
        /// </summary>
        /// <param name="QuotationId">报价单主键</param>
        /// <returns></returns>
        public List<BY_QuotationDetail> GetList(string QuotationId, JqGridParam jqgridparam)
        {
            if (!string.IsNullOrEmpty(QuotationId))
            {
                string sql = @"
SELECT [QuotationDetailId]
      ,[QuotationId]
      ,[LineCode]
      ,[BoardId]
      ,[BoardDescription]
      ,[Qty]
      ,[ProjectCode]
      ,[IsGrindAngle]
      ,[IsChamfer]
      ,[SteelId]
      ,[SteelCode]
      ,[SteelDescription]
      ,[SteelOtherDescription]
      ,[SteelState]
      ,[DieCode]
      ,[Thick]
      ,[Weigh]
      ,[Length]
      ,[AddThick]
      ,[AddWeigh]
      ,[AddLength]
      ,[Technology]
      ,[Weight]
      ,[MaterialPrice]
      ,[MaterialLess]
      ,[MaterialAmount]
      ,[WorkPrice]
      ,[WorkLess]
      ,[WorkAmount]
      ,[HeatPrice]
      ,[HeatLess]
      ,[HeatAmount]
      ,[SalesAmount]
      ,[CostAmount]
      ,[ProfitAmount]
      ,[ProfitRate]
      ,[ShipDescription]
      ,[Remark]
      ,[Enabled]
      ,[SortCode]
      ,[DeleteMark]
      ,[CreateDate]
      ,[CreateUserId]
      ,[CreateUserName]
      ,[ModifyDate]
      ,[ModifyUserId]
      ,[ModifyUserName]
      ,[PlaceOrderStatus]
      ,[OrderType]
      ,[CustomerOrder]
      ,[CustomerProduct]
      ,[QuotationCode]
      ,[LimitWeight]
      ,[LimitThick]
      ,[WorkCost]
      ,[MaterialCost]
      ,[ManagerCost]
      ,[InvoiceCost]
      ,[RepalceMl]
      ,[JJtype]
      ,[ProductName]
      ,[LiftingHole]
      ,[GasCuttingSteel]
      ,[BoreSize]
      ,[SteelType]
      ,[Area]
      ,[TheoryWeight]
      ,[MandXSteel]
      ,[MorXSteel]
      ,[BSteel]
      ,[MianCostArea]
      ,[MianCostPer]
      ,[MianCost]
      ,[BianCostLong]
      ,[BianCostPer]
      ,[BianCost]
      ,[JKCost]
      ,[SmallCost]
      ,[WorkCostForWork]
      ,[JBSteel]
      ,[WorkCostForWorkTemp]
      ,[MianType]
      ,[BianType]
      ,[MianCostUnit]
      ,[BianCostUnit]
      ,[WorkUnit]
      ,[XorMSteel]
      ,[CountShow]
      ,[MianCostWork]
      ,[BianCostWork]
      , (SELECT STUFF((SELECT '，'+bf.Status FROM base_file bf where bf.AttachId = QuotationDetailId for xml path('')),1,1,'') ) [Fang],[CNC]
      ,[YM]
      ,[JQ]
      ,[Kprocess]
      ,[JKCostPer]
      ,[JKCostQty]
      ,[SmallAmount]
      
      ,[DDCostPer]
      ,[DDCost]
      ,[DD]
      ,[BoreSizeAdd]
      ,[Brand]
      ,[BYDUnit]
      ,[BYDNum]
      ,[ProductName1]
      ,[Mheipi]
      ,[RemarkKL]
      ,[Dingcode]
      ,[WorkWeight]
      ,[Delivery]
      ,[LJtype1]
      ,[LJtype2]
      ,[LJsize]
  FROM [dbo].[BY_QuotationDetail] where 1= 1

";
                return Repository().FindListPageBySql(sql + " and QuotationId = '" + QuotationId + "' ", ref jqgridparam);
            }
            else
            {
                return null;
            }
        }
    }
}