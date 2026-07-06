using DNS.Business;
using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.PaymentModule.Controllers
{
    /// <summary>
    /// 付款管理控制器
    /// </summary>
    public class BY_PayReimburseSheetController : PublicController<BY_PayReimburseSheet>
    {
        public class BankSummaryEntity
        {
            public string ID { get; set; }
            public string PID { get; set; }
            public int LEV { get; set; }
            public string Type1 { get; set; }
            public string Type2 { get; set; }
            public string CustomerName { get; set; }
            public string WorkerName { get; set; }
            public string GETMoney { get; set; }
            public string ZRMoney { get; set; }
            public string ZRRemark { get; set; }
            public string BankNum { get; set; }
            public string namelie { get; set; }
            public string Remark { get; set; }
            public string Remark1 { get; set; }
            public DateTime RequiredDate { get; set; }
        }

        public ActionResult GridPageListJson(string StartTime,string EndTime)
        {
            try
            {
                if (string.IsNullOrEmpty(StartTime) || string.IsNullOrEmpty(EndTime))
                {
                    return null;

                }
                StringBuilder sb = new StringBuilder();
                List<BankSummaryEntity> ListData = DataFactory.Database().FindListBySql<BankSummaryEntity>(@"
SELECT  Type1 ID ,
        '0' PID ,
        1 LEV,
        Type1 ,
        '' Type2 ,
        CAST(SUM(GETMoney) AS VARCHAR(50))  CustomerName ,
        '' WorkerName ,
        '0' GETMoney,
        '' BankNum ,
        '1999-01-01' RequiredDate,Type1 namelie,'' Remark,'' Remark1
FROM    dbo.BY_PayReimburseSheet where RequiredDate >= '" + StartTime + @"' and RequiredDate <= '" + EndTime + @"'  AND Type1 IS NOT NULL
GROUP BY Type1
UNION ALL
SELECT  Type1+Type2 ID ,
        Type1 PID ,
        2 LEV,
        '' Type1 ,
        Type2 ,
        '' CustomerName ,
       CAST(SUM(GETMoney) AS VARCHAR(50)) WorkerName ,
        '0' GETMoney,
        '' BankNum ,
        '1999-01-01' RequiredDate,Type2 namelie,'' Remark,'' Remark1
FROM    dbo.BY_PayReimburseSheet where RequiredDate >= '" + StartTime + @"' and RequiredDate <= '" + EndTime + @"'  AND Type1 IS NOT NULL
GROUP BY Type2,Type1
UNION ALL
SELECT  CAST(NEWID() AS VARCHAR(50))  ID ,
        Type1+Type2 PID ,
        3 LEV,
       '' Type1 ,
       '' Type2 ,
        CustomerName ,
        WorkerName ,
        CAST(GETMoney AS VARCHAR(50)) GETMoney ,
        BankNum ,
        RequiredDate, CASE WHEN  CustomerName IS NULL THEN '' ELSE CustomerName END   + CASE WHEN  WorkerName IS NULL THEN '' ELSE + ',经手人：'+WorkerName END  namelie ,Remark, Remark1
FROM    dbo.BY_PayReimburseSheet where RequiredDate >= '" + StartTime + @"' and RequiredDate <= '" + EndTime + @"'  AND Type1 IS NOT NULL
UNION ALL
SELECT  '合计' ID ,
        '0' PID ,
        1 LEV,
        '' Type1 ,
        '' Type2 ,
        CAST(SUM(GETMoney) AS VARCHAR(50)) CustomerName ,
         '' WorkerName ,
        '0' GETMoney,
        '' BankNum ,
        '1999-01-01' RequiredDate,'' namelie,'' Remark,'' Remark1
FROM    dbo.BY_PayReimburseSheet where RequiredDate >= '" + StartTime + @"' and RequiredDate <= '" + EndTime + @"' AND Type1 IS NOT NULL"); ;
                sb.Append("{ \"rows\": [");
                sb.Append(TreeGridJson(ListData, -1,"0",1,1));
                sb.Append("]}");
                return Content(sb.ToString());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }
        public string TreeGridJson(List<BankSummaryEntity> ListData, int index, string PID, int lftNum, int LEV)
        {
            StringBuilder sb = new StringBuilder();
            List<BankSummaryEntity> ChildNodeList = ListData.FindAll(t => t.PID == PID && t.LEV == LEV);
            if (ChildNodeList.Count > 0) { index++; }
            foreach (BankSummaryEntity entity in ChildNodeList)
            {
                string strJson = entity.ToJson();
                strJson = strJson.Insert(1, "\"level\":\"" + index + "\",");
                strJson = strJson.Insert(1, "\"isLeaf\":\"" + (ListData.Count<BankSummaryEntity>(t => t.PID == entity.ID && t.LEV == LEV+1) == 0 ? true : false).ToString().ToLower() + "\",");
                strJson = strJson.Insert(1, "\"expanded\":\"false\",");
                strJson = strJson.Insert(1, "\"loaded\":\"true\",");
                strJson = strJson.Insert(1, "\"lft\":\"" + lftNum + "\",");
                if (ListData.Count<BankSummaryEntity>(t => t.PID == entity.ID && t.LEV == entity.LEV+1) == 0)
                {
                    lftNum = (lftNum + 1);
                    strJson = strJson.Insert(1, "\"rgt\":\"" + lftNum + "\",");
                    sb.Append(strJson);
                    sb.Append(TreeGridJson(ListData, index, entity.ID, lftNum + 1, entity.LEV + 1));
                }
                else
                {
                    int rgt = lftNum + GetChildNum(ListData, entity.ID, entity.LEV + 1) * 2 + 1;
                    strJson = strJson.Insert(1, "\"rgt\":\"" + rgt + "\",");
                    sb.Append(strJson);
                    sb.Append(TreeGridJson(ListData, index, entity.ID, lftNum + 1, entity.LEV + 1));
                    lftNum = lftNum + GetChildNum(ListData, entity.ID, entity.LEV + 1) * 2 + 2;
                }
               
            }
            return sb.ToString().Replace("}{", "},{");
        }

        public int GetChildNum(List<BankSummaryEntity> ListData, string PID,int LEV)
        {
            int total = 0;
            List<BankSummaryEntity> ChildNodeList = ListData.FindAll(t => t.PID == PID && t.LEV == LEV);
            total = ChildNodeList.Count;
            foreach (BankSummaryEntity entity in ChildNodeList)
            {
                total += GetChildNum(ListData, entity.ID, entity.LEV+1);
            }
            return total;
        }

        public ActionResult GridPageJsonYS(string CustomerName, string CreateDate_StartTime, string CreateDate_EndTime, JqGridParam jqgridparam)
        {
            try
            {
                List<DbParameter> parameter = new List<DbParameter>();
                string sql = string.Format(@"
SELECT  *
FROM    ( 
          SELECT    RequiredDate AS BuildDate ,
                    InvoiceNum AS Code ,
                    0 TotalAmount ,
                    -GetMoney GetMoney,
                    GetType AS GetType ,
                    0 AS NoGetMoney ,
                    Remark ,
                    0 ZRMoney,  BankNum
          FROM      BY_PayReimburseSheet
          WHERE     1 = 1
                    AND ( PayReimburseSheetCode IS NULL
                          OR PayReimburseSheetCode = ''
                          OR Opions = '退款'
                        )
                                AND CustomerCode = @CustomerName
                    AND CreateDate > @StartTime
                    AND CreateDate < @EndTime
                    and GetMoney  < 0
          UNION ALL
          SELECT    CreateDate AS BuildDate ,
                    Base_SteelDDBeoutCode AS Code ,
                    -TotalAmount TotalAmount ,
                    0 AS GetMoney ,
                    '' AS GetType ,
                    0 AS NoGetMoney ,
                    Remark ,
                    ( SELECT    SUM(ISNULL(s.ZRMoney, 0))
                      FROM      BY_PayReimburseSheet s
                      WHERE     s.Code = Base_SteelDDBeout.Base_SteelDDBeoutCode
                    ) ZRMoney,'' BankNum
          FROM      dbo.Base_SteelDDBeout
          WHERE     ( PayCheckSheetCode IS  NULL
                      OR PayCheckSheetCode = ''
                    )
                                AND CustomerCode = @CustomerName
                    AND CreateDate > @StartTime
                    AND CreateDate < @EndTime
and TotalAmount <0
          UNION ALL
          SELECT    CreateDate AS BuildDate ,
                    Base_SteelDDPerchaseCode AS Code ,
                    -TotalAmount TotalAmount ,
                    0 AS GetMoney ,
                    '' AS GetType ,
                    0 AS NoGetMoney ,
                    Remark ,
                    ( SELECT    SUM(ISNULL(s.ZRMoney, 0))
                      FROM      BY_PayReimburseSheet s
                      WHERE     s.Code = Base_SteelDDPerchase.Base_SteelDDPerchaseCode
                    ) ZRMoney,'' BankNum
          FROM      dbo.Base_SteelDDPerchase
          WHERE     ( PayCheckSheetCode IS  NULL
                      OR PayCheckSheetCode = ''
                    )
                                AND CustomerCode = @CustomerName
                    AND CreateDate > @StartTime
                    AND CreateDate < @EndTime
and TotalAmount <0

          UNION ALL
          SELECT    CreateDate AS BuildDate ,
                    SteelPurchaseCode AS Code ,
                    -TotalAmount TotalAmount ,
                    0 AS GetMoney ,
                    '' AS GetType ,
                    0 AS NoGetMoney ,
                    Remark ,
                    ( SELECT    SUM(ISNULL(s.ZRMoney, 0))
                      FROM      BY_PayReimburseSheet s
                      WHERE     s.Code = Base_SteelPurchase.SteelPurchaseCode
                    ) ZRMoney,'' BankNum
          FROM      dbo.Base_SteelPurchase
          WHERE     ( PayCheckSheetCode IS  NULL
                      OR PayCheckSheetCode = ''
                    )
                    AND CustomerCode = @CustomerName
                    AND CreateDate > @StartTime
                    AND CreateDate < @EndTime
and TotalAmount <0
          UNION ALL
          SELECT    CreateDate AS BuildDate ,
                    MaterielPurchaseCode AS Code ,
                    -TotalAmount TotalAmount ,
                    0 AS GetMoney ,
                    '' AS GetType ,
                    0 AS NoGetMoney ,
                    Remark ,
                    ( SELECT    SUM(ISNULL(s.ZRMoney, 0))
                      FROM      BY_PayReimburseSheet s
                      WHERE     s.Code = Base_MaterielPurchase.MaterielPurchaseCode
                    ) ZRMoney,'' BankNum
          FROM      dbo.Base_MaterielPurchase
          WHERE     ( PayCheckSheetCode IS  NULL
                      OR PayCheckSheetCode = ''
                    )
                    AND CustomerCode = @CustomerName
                    AND CreateDate > @StartTime
                    AND CreateDate < @EndTime
and TotalAmount <0
          UNION ALL
          SELECT    CreateDate AS BuildDate ,
                    PayCheckSheetCode AS Code ,
                    -TotalAmount TotalAmount ,
                    0 AS GetMoney ,
                    '' AS GetType ,
                    0 AS NoGetMoney ,
                    Remark ,
                    ( SELECT    SUM(ISNULL(s.ZRMoney, 0))
                      FROM      BY_PayReimburseSheet s
                      WHERE     s.Code = PayCheckSheetCode
                    ) ZRMoney,'' BankNum
          FROM      dbo.BY_PayCheckSheel
          WHERE     1 = 1
                    AND CustomerCode = @CustomerName
                    AND CreateDate > @StartTime
                    AND CreateDate < @EndTime
and TotalAmount <0
          UNION ALL
          SELECT    CreateDate AS BuildDate ,
                    PayCompanyCode AS Code ,
                    -TotalAmount TotalAmount ,
                    0 AS GetMoney ,
                    '' AS GetType ,
                    0 AS NoGetMoney ,
                    Remark ,
                    ( SELECT    SUM(ISNULL(s.ZRMoney, 0))
                      FROM      BY_PayReimburseSheet s
                      WHERE     s.Code = PayCompanyCode
                    ) ZRMoney,'' BankNum
          FROM      Base_PayCompany
          WHERE     CustomerCode = @CustomerName
                    AND CreateDate > @StartTime
                    AND CreateDate < @EndTime
and TotalAmount <0
        ) B
ORDER BY BuildDate ASC
         ");
                if (string.IsNullOrEmpty(CreateDate_StartTime))
                {
                    CreateDate_StartTime = "2000-1-1";
                }
                if (string.IsNullOrEmpty(CreateDate_EndTime))
                {
                    CreateDate_EndTime = "2999-1-1";
                }

                sql = sql.Replace("@CustomerName", string.IsNullOrEmpty(CustomerName) ? "" : "'" + CustomerName + "'");
                sql = sql.Replace("@StartTime", string.IsNullOrEmpty(CreateDate_StartTime) ? "" : "'" + CreateDate_StartTime + "'");
                sql = sql.Replace("@EndTime", string.IsNullOrEmpty(CreateDate_EndTime) ? "" : "'" + CreateDate_EndTime + " 23:59:59'");

                List<LWtemp> lwtemps = DataFactory.Database().FindListBySql<LWtemp>(sql);
                decimal temp = 0;
                foreach (var item in lwtemps)
                {
                    temp = temp + item.TotalAmount - item.GetMoney - item.ZRMoney;
                    item.NoGetMoney = temp;
                }
                LWtemp lwtemp = new LWtemp();
                lwtemp.Remark = "合计";
                lwtemp.TotalAmount = lwtemps.Sum(p => p.TotalAmount);
                lwtemp.GetMoney = lwtemps.Sum(p => p.GetMoney);
                lwtemp.NoGetMoney = temp;
                lwtemp.ZRMoney = lwtemps.Sum(p => p.ZRMoney);
                lwtemps.Add(lwtemp);
                Stopwatch watch = CommonHelper.TimerStart();
                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = lwtemps,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception)
            {
                return null;
            }
        }
        public class LWtemp
        {
            public string PayReimburseSheetID { get; set; }
            public DateTime BuildDate { get; set; }
            public string GetType { get; set; }
            public string Opions { get; set; }
            public string Code { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal GetMoney { get; set; }
            public decimal NoGetMoney { get; set; }
            public string Remark { get; set; }
            public decimal ZRMoney { get; set; }
            public string BankNum { get; set; }
        }

        public ActionResult GridPageJsonLW(string CustomerName, string CreateDate_StartTime, string CreateDate_EndTime, string PayReimburseSheetCode, string audit, JqGridParam jqgridparam)
        {
            try
            {
                List<DbParameter> parameter = new List<DbParameter>();
                string sql = string.Format(@"
SELECT  *
FROM    ( SELECT    '' AS BuildDate ,
                    '期初' Code ,
                    SUM(TotalAmount) TotalAmount ,
                    SUM(GetMoney) GetMoney ,
                    '' GetType ,
                    0 NoGetMoney ,
                    '' Remark ,
                    SUM(ZRMoney) ZRMoney ,
                    '' BankNum
          FROM      ( SELECT    RequiredDate AS BuildDate ,
                                InvoiceNum AS Code ,
                                0 TotalAmount ,
                                GetMoney ,
                                GetType AS GetType ,
                                0 AS NoGetMoney ,
                                Remark ,
                                0 ZRMoney
                      FROM      BY_PayReimburseSheet
                      WHERE     1 = 1
                                AND ( PayReimburseSheetCode IS NULL
                                      OR PayReimburseSheetCode = ''
                                      OR Opions = '退款'
                                    )
                                AND CustomerCode = @CustomerName
                                AND RequiredDate < @StartTime
                      UNION ALL
                      SELECT    CreateDate AS BuildDate ,
                                Base_SteelDDBeoutCode AS Code ,
                                TotalAmount TotalAmount ,
                                0 AS GetMoney ,
                                '' AS GetType ,
                                0 AS NoGetMoney ,
                                Remark ,
                                ( SELECT    SUM(ISNULL(s.ZRMoney, 0))
                                  FROM      BY_PayReimburseSheet s
                                  WHERE     s.Code = Base_SteelDDBeout.Base_SteelDDBeoutCode
                                ) ZRMoney
                      FROM      dbo.Base_SteelDDBeout
                      WHERE     ( PayCheckSheetCode IS  NULL
                                  OR PayCheckSheetCode = ''
                                )
                                AND CustomerCode = @CustomerName
                                AND CreateDate < @StartTime
                      UNION ALL
                      SELECT    CreateDate AS BuildDate ,
                                Base_SteelDDPerchaseCode AS Code ,
                                TotalAmount TotalAmount ,
                                0 AS GetMoney ,
                                '' AS GetType ,
                                0 AS NoGetMoney ,
                                Remark ,
                                ( SELECT    SUM(ISNULL(s.ZRMoney, 0))
                                  FROM      BY_PayReimburseSheet s
                                  WHERE     s.Code = Base_SteelDDPerchase.Base_SteelDDPerchaseCode
                                ) ZRMoney
                      FROM      dbo.Base_SteelDDPerchase
                      WHERE     ( PayCheckSheetCode IS  NULL
                                  OR PayCheckSheetCode = ''
                                )
                                AND CustomerCode = @CustomerName
                                AND CreateDate < @StartTime
                      UNION ALL
                      SELECT    CreateDate AS BuildDate ,
                                SteelPurchaseCode AS Code ,
                                TotalAmount TotalAmount ,
                                0 AS GetMoney ,
                                '' AS GetType ,
                                0 AS NoGetMoney ,
                                Remark ,
                                ( SELECT    SUM(ISNULL(s.ZRMoney, 0))
                                  FROM      BY_PayReimburseSheet s
                                  WHERE     s.Code = Base_SteelPurchase.SteelPurchaseCode
                                ) ZRMoney
                      FROM      dbo.Base_SteelPurchase
                      WHERE     ( PayCheckSheetCode IS  NULL
                                  OR PayCheckSheetCode = ''
                                )
                                AND CustomerCode = @CustomerName
                                AND CreateDate < @StartTime
                      UNION ALL
                      SELECT    CreateDate AS BuildDate ,
                                MaterielPurchaseCode AS Code ,
                                TotalAmount TotalAmount ,
                                0 AS GetMoney ,
                                '' AS GetType ,
                                0 AS NoGetMoney ,
                                Remark ,
                                ( SELECT    SUM(ISNULL(s.ZRMoney, 0))
                                  FROM      BY_PayReimburseSheet s
                                  WHERE     s.Code = Base_MaterielPurchase.MaterielPurchaseCode
                                ) ZRMoney
                      FROM      dbo.Base_MaterielPurchase
                      WHERE     ( PayCheckSheetCode IS  NULL
                                  OR PayCheckSheetCode = ''
                                )
                                AND CustomerCode = @CustomerName
                                AND CreateDate < @StartTime
                      UNION ALL
                      SELECT    CreateDate AS BuildDate ,
                                PayCheckSheetCode AS Code ,
                                TotalAmount TotalAmount ,
                                0 AS GetMoney ,
                                '' AS GetType ,
                                0 AS NoGetMoney ,
                                Remark ,
                                ( SELECT    SUM(ISNULL(s.ZRMoney, 0))
                                  FROM      BY_PayReimburseSheet s
                                  WHERE     s.Code = PayCheckSheetCode
                                ) ZRMoney
                      FROM      dbo.BY_PayCheckSheel
                      WHERE     1 = 1
                                AND CustomerCode = @CustomerName
                                AND CreateDate < @StartTime
                      UNION ALL
                      SELECT    CreateDate AS BuildDate ,
                                QuotationHeatBeoutCode AS Code ,
                                TotalAmount TotalAmount ,
                                0 AS GetMoney ,
                                '' AS GetType ,
                                0 AS NoGetMoney ,
                                Remark ,
                                ( SELECT    SUM(ISNULL(s.ZRMoney, 0))
                                  FROM      BY_PayReimburseSheet s
                                  WHERE     s.Code = QuotationHeatBeoutCode
                                ) ZRMoney
                      FROM      dbo.BY_QuotationHeatBeout
                      WHERE     ( PayCheckSheetCode IS  NULL
                                  OR PayCheckSheetCode = ''
                                )
                                AND CustomerCode = @CustomerName
                                AND CreateDate < @StartTime

                      UNION ALL
                      SELECT    CreateDate AS BuildDate ,
                                PayCompanyCode AS Code ,
                                TotalAmount TotalAmount ,
                                0 AS GetMoney ,
                                '' AS GetType ,
                                0 AS NoGetMoney ,
                                Remark ,
                                ( SELECT    SUM(ISNULL(s.ZRMoney, 0))
                                  FROM      BY_PayReimburseSheet s
                                  WHERE     s.Code = PayCompanyCode
                                ) ZRMoney
                      FROM      dbo.Base_PayCompany
                      WHERE     ( PayCheckSheetCode IS  NULL
                                  OR PayCheckSheetCode = ''
                                )
                                AND CustomerCode = @CustomerName
                                AND CreateDate < @StartTime


                    ) A
          UNION ALL
          SELECT    RequiredDate AS BuildDate ,
                    InvoiceNum AS Code ,
                    0 TotalAmount ,
                    GetMoney ,
                    GetType AS GetType ,
                    0 AS NoGetMoney ,
                    Remark ,
                    0 ZRMoney,  BankNum
          FROM      BY_PayReimburseSheet
          WHERE     1 = 1
                    AND ( PayReimburseSheetCode IS NULL
                          OR PayReimburseSheetCode = ''
                          OR Opions = '退款'
                        )
                                AND CustomerCode = @CustomerName
                    AND CreateDate > @StartTime
                    AND CreateDate < @EndTime
          UNION ALL
          SELECT    CreateDate AS BuildDate ,
                    Base_SteelDDBeoutCode AS Code ,
                    TotalAmount TotalAmount ,
                    0 AS GetMoney ,
                    '' AS GetType ,
                    0 AS NoGetMoney ,
                    Remark ,
                    ( SELECT    SUM(ISNULL(s.ZRMoney, 0))
                      FROM      BY_PayReimburseSheet s
                      WHERE     s.Code = Base_SteelDDBeout.Base_SteelDDBeoutCode
                    ) ZRMoney,'' BankNum
          FROM      dbo.Base_SteelDDBeout
          WHERE     ( PayCheckSheetCode IS  NULL
                      OR PayCheckSheetCode = ''
                    )
                                AND CustomerCode = @CustomerName
                    AND CreateDate > @StartTime
                    AND CreateDate < @EndTime
          UNION ALL
          SELECT    CreateDate AS BuildDate ,
                    Base_SteelDDPerchaseCode AS Code ,
                    TotalAmount TotalAmount ,
                    0 AS GetMoney ,
                    '' AS GetType ,
                    0 AS NoGetMoney ,
                    Remark ,
                    ( SELECT    SUM(ISNULL(s.ZRMoney, 0))
                      FROM      BY_PayReimburseSheet s
                      WHERE     s.Code = Base_SteelDDPerchase.Base_SteelDDPerchaseCode
                    ) ZRMoney,'' BankNum
          FROM      dbo.Base_SteelDDPerchase
          WHERE     ( PayCheckSheetCode IS  NULL
                      OR PayCheckSheetCode = ''
                    )
                                AND CustomerCode = @CustomerName
                    AND CreateDate > @StartTime
                    AND CreateDate < @EndTime
          UNION ALL
          SELECT    CreateDate AS BuildDate ,
                    SteelPurchaseCode AS Code ,
                    TotalAmount TotalAmount ,
                    0 AS GetMoney ,
                    '' AS GetType ,
                    0 AS NoGetMoney ,
                    Remark ,
                    ( SELECT    SUM(ISNULL(s.ZRMoney, 0))
                      FROM      BY_PayReimburseSheet s
                      WHERE     s.Code = Base_SteelPurchase.SteelPurchaseCode
                    ) ZRMoney,'' BankNum
          FROM      dbo.Base_SteelPurchase
          WHERE     ( PayCheckSheetCode IS  NULL
                      OR PayCheckSheetCode = ''
                    )
                    AND CustomerCode = @CustomerName
                    AND CreateDate > @StartTime
                    AND CreateDate < @EndTime
          UNION ALL
          SELECT    CreateDate AS BuildDate ,
                    MaterielPurchaseCode AS Code ,
                    TotalAmount TotalAmount ,
                    0 AS GetMoney ,
                    '' AS GetType ,
                    0 AS NoGetMoney ,
                    Remark ,
                    ( SELECT    SUM(ISNULL(s.ZRMoney, 0))
                      FROM      BY_PayReimburseSheet s
                      WHERE     s.Code = Base_MaterielPurchase.MaterielPurchaseCode
                    ) ZRMoney,'' BankNum
          FROM      dbo.Base_MaterielPurchase
          WHERE     ( PayCheckSheetCode IS  NULL
                      OR PayCheckSheetCode = ''
                    )
                    AND CustomerCode = @CustomerName
                    AND CreateDate > @StartTime
                    AND CreateDate < @EndTime
          UNION ALL
          SELECT    CreateDate AS BuildDate ,
                    PayCheckSheetCode AS Code ,
                    TotalAmount TotalAmount ,
                    0 AS GetMoney ,
                    '' AS GetType ,
                    0 AS NoGetMoney ,
                    Remark ,
                    ( SELECT    SUM(ISNULL(s.ZRMoney, 0))
                      FROM      BY_PayReimburseSheet s
                      WHERE     s.Code = PayCheckSheetCode
                    ) ZRMoney,'' BankNum
          FROM      dbo.BY_PayCheckSheel
          WHERE     1 = 1
                    AND CustomerCode = @CustomerName
                    AND CreateDate > @StartTime
                    AND CreateDate < @EndTime

UNION ALL
          SELECT    CreateDate AS BuildDate ,
                    PayCompanyCode AS Code ,
                    TotalAmount TotalAmount ,
                    0 AS GetMoney ,
                    '' AS GetType ,
                    0 AS NoGetMoney ,
                    Remark ,
                    ( SELECT    SUM(ISNULL(s.ZRMoney, 0))
                      FROM      BY_PayReimburseSheet s
                      WHERE     s.Code = PayCompanyCode
                    ) ZRMoney,'' BankNum
          FROM      dbo.Base_PayCompany
          WHERE     1 = 1
                    AND CustomerCode = @CustomerName
                    AND CreateDate > @StartTime
                    AND CreateDate < @EndTime


           UNION ALL
          SELECT CreateDate AS BuildDate ,
        QuotationHeatBeoutCode AS Code ,
        TotalAmount TotalAmount ,
        0 AS GetMoney ,
        '' AS GetType ,
        0 AS NoGetMoney ,
        Remark ,
        ( SELECT    SUM(ISNULL(s.ZRMoney, 0))
          FROM      BY_PayReimburseSheet s
          WHERE     s.Code = QuotationHeatBeoutCode
        ) ZRMoney ,
        '' BankNum
 FROM   dbo.BY_QuotationHeatBeout
 WHERE  ( PayCheckSheetCode IS  NULL
                      OR PayCheckSheetCode = ''
                    )
                    AND CustomerCode = @CustomerName
                    AND CreateDate > @StartTime
                    AND CreateDate < @EndTime
        ) B
ORDER BY BuildDate ASC
         ");
                if (string.IsNullOrEmpty(CreateDate_StartTime))
                {
                    CreateDate_StartTime = "2000-1-1";
                }
                if (string.IsNullOrEmpty(CreateDate_EndTime))
                {
                    CreateDate_EndTime = "2999-1-1";
                }

                if (!string.IsNullOrEmpty(PayReimburseSheetCode))
                {
                    sql = string.Format(@"SELECT  RequiredDate AS BuildDate ,
        InvoiceNum AS Code ,
        0 TotalAmount ,
        RequiredMoney  GetMoney ,
        GetType AS GetType ,
        0 AS NoGetMoney ,
        Remark 
FROM    BY_PayReimburseSheet
WHERE  PayReimburseSheetCode = '{0}'", PayReimburseSheetCode);
                }

                if (audit == "1")
                {
                    sql = sql.Replace("@audit", " and AuditUserId != '' and AuditUserId is not null ");
                }
                else
                {
                    sql = sql.Replace("@audit", "");

                }

                sql = sql.Replace("@CustomerName", string.IsNullOrEmpty(CustomerName) ? "" : "'" + CustomerName + "'");
                sql = sql.Replace("@StartTime", string.IsNullOrEmpty(CreateDate_StartTime) ? "" : "'" + CreateDate_StartTime + "'");
                sql = sql.Replace("@EndTime", string.IsNullOrEmpty(CreateDate_EndTime) ? "" : "'" + CreateDate_EndTime + "'");

                List<LWtemp> lwtemps = DataFactory.Database().FindListBySql<LWtemp>(sql);
                decimal temp = 0;
                foreach (var item in lwtemps)
                {
                    temp = temp + item.TotalAmount - item.GetMoney - item.ZRMoney;
                    item.NoGetMoney = temp;
                }
                LWtemp lwtemp = new LWtemp();
                lwtemp.Remark = "合计";
                lwtemp.TotalAmount = lwtemps.Sum(p => p.TotalAmount);
                lwtemp.GetMoney = lwtemps.Sum(p => p.GetMoney);
                lwtemp.NoGetMoney = temp;
                lwtemp.ZRMoney = lwtemps.Sum(p => p.ZRMoney);
                lwtemps.Add(lwtemp);
                Stopwatch watch = CommonHelper.TimerStart();
                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = lwtemps,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception)
            {
                return null;
            }
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult BankSummary()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult BankPrint()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult LWDetail()
        {
            return View();
        }
        
        [LoginAuthorize]
        public  JsonResult GridPageJson1(string ParameterJson, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                List<BY_PayReimburseSheet> ListData = new List<BY_PayReimburseSheet>();
                if (!string.IsNullOrEmpty(ParameterJson))
                {
                    List<DbParameter> parameter = new List<DbParameter>();
                    IList conditions = ParameterJson.JonsToList<Condition>();
                    string WhereSql = ConditionBuilder.GetWhereSql(conditions, out parameter);
                    if (ManageProvider.Provider.Current().ObjectId.IndexOf("9d04b362-c9fe-4c67-95a5-d24d6ad9177d") < 0 && ManageProvider.Provider.Current().UserName != "超级管理员")
                    {
                        WhereSql += @"  AND CreateUserName NOT IN (SELECT  b.RealName
FROM    Base_ObjectUserRelation o ,
        Base_User b
WHERE   ObjectId = '9d04b362-c9fe-4c67-95a5-d24d6ad9177d'
        AND o.UserId = b.UserId) ";
                    }
                    ListData = repositoryfactory.Repository().FindListPage(WhereSql, parameter.ToArray(), ref jqgridparam);
                }
                else
                {
                    string WhereSql = "";
                    if (ManageProvider.Provider.Current().ObjectId.IndexOf("9d04b362-c9fe-4c67-95a5-d24d6ad9177d") < 0 && ManageProvider.Provider.Current().UserName != "超级管理员")
                    {
                        WhereSql += @"  AND CreateUserName NOT IN (SELECT  b.RealName
FROM    Base_ObjectUserRelation o ,
        Base_User b
WHERE   ObjectId = '9d04b362-c9fe-4c67-95a5-d24d6ad9177d'
        AND o.UserId = b.UserId) ";
                    }
                    ListData = repositoryfactory.Repository().FindListPage(WhereSql, ref jqgridparam);
                }
                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = ListData,
                };
                return Json(JsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message + "\r\n条件：" + ParameterJson);
                return null;
            }
        }



        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult ReturnMoney(string KeyValue, string Remark, decimal? RequiredMoney, string RequiredDate,string GetType,string BankNum)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                BY_PayReimburseSheet by_reimbursesheetR = DataFactory.Database().FindEntity<BY_PayReimburseSheet>(KeyValue);
                if (RequiredMoney - by_reimbursesheetR.CanUseMoney > 0)
                {
                    database.Rollback();
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：此预收款余额不足" }.ToString());
                }
                by_reimbursesheetR.CanUseMoney = by_reimbursesheetR.CanUseMoney - RequiredMoney;

                BY_PayReimburseSheet by_reimbursesheet = new BY_PayReimburseSheet();
                Base_CodeRuleBll base_coderulebll = new Base_CodeRuleBll();
                CommonHelper.CopySameFieldsObjectALL<BY_PayReimburseSheet, BY_PayReimburseSheet>(by_reimbursesheetR, ref by_reimbursesheet);
                by_reimbursesheet.Create();

                by_reimbursesheet.CanUseMoney = 0;
                by_reimbursesheet.RequiredMoney = -RequiredMoney;
                by_reimbursesheet.RequiredMan = ManageProvider.Provider.Current().UserName;
                by_reimbursesheet.Remark = Remark;
                by_reimbursesheet.GETMoney = by_reimbursesheet.RequiredMoney;
                by_reimbursesheet.Opions = "退款";
                by_reimbursesheet.RequiredDate = Convert.ToDateTime(RequiredDate);
                by_reimbursesheet.PayReimburseSheetCode = by_reimbursesheetR.PayReimburseSheetID;
                by_reimbursesheet.GetType = GetType;
                by_reimbursesheet.BankNum = BankNum;
                by_reimbursesheet.Type1 = by_reimbursesheetR.Type1;
                by_reimbursesheet.Type2 = by_reimbursesheetR.Type2;
                by_reimbursesheet.Type3 = by_reimbursesheetR.Type3;


                database.Insert(by_reimbursesheet, isOpenTrans);
                database.Update(by_reimbursesheetR, isOpenTrans);
                base_coderulebll.OccupyBillCode(ManageProvider.Provider.Current().UserId, "SC", isOpenTrans);//--获取后必须占用
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "退款成功" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult PayMoney(string KeyValue, string ids, string type, string totalmoneys, string CanUseMoney, string Remark, string zrmoney, string RequiredDate)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                BY_PayReimburseSheet by_reimbursesheetYF = DataFactory.Database().FindEntity<BY_PayReimburseSheet>(KeyValue);
                List<BY_PayCheckSheel> by_paychecksheeladd = new List<BY_PayCheckSheel>();//

                string[] hids = (ids.TrimEnd(',')).Split(',');
                string sqltem = " in (";
                foreach (var id in hids)
                {
                    sqltem = sqltem + "'" + id + "',";
                }
                sqltem = sqltem.TrimEnd(',') + ")";
                List<Base_SteelDDBeout> base_steelddbeouts = DataFactory.Database().FindListBySql<Base_SteelDDBeout>(" select * from Base_SteelDDBeout where Base_SteelDDBeoutCode " + sqltem);
                List<Base_SteelDDPerchase> base_steelddperchases = DataFactory.Database().FindListBySql<Base_SteelDDPerchase>(" select * from Base_SteelDDPerchase where Base_SteelDDPerchaseCode" + sqltem);
                List<BY_PayCheckSheel> by_paychecksheels = DataFactory.Database().FindListBySql<BY_PayCheckSheel>(" select * from BY_PayCheckSheel where PayCheckSheetCode" + sqltem);
                List<Base_MaterielPurchase> base_materielpurchases = DataFactory.Database().FindListBySql<Base_MaterielPurchase>(" select * from Base_MaterielPurchase where MaterielPurchaseCode" + sqltem);
                List<Base_SteelPurchase> base_steelpurchases = DataFactory.Database().FindListBySql<Base_SteelPurchase>(" select * from Base_SteelPurchase where SteelPurchaseCode" + sqltem);
                List<Base_PayCompany> base_paycompanys = DataFactory.Database().FindListBySql<Base_PayCompany>(" select * from Base_PayCompany where PayCompanyCode" + sqltem);
                List<BY_QuotationHeatBeout> by_quotationheatbeouts = DataFactory.Database().FindListBySql<BY_QuotationHeatBeout>(" select * from BY_QuotationHeatBeout where QuotationHeatBeoutCode" + sqltem);



                if (Convert.ToDecimal(totalmoneys) > Convert.ToDecimal(CanUseMoney) + Convert.ToDecimal(zrmoney))
                {
                    by_reimbursesheetYF.CanUseMoney = 0;
                    by_reimbursesheetYF.Remark = Remark;
                    by_reimbursesheetYF.Modify(by_reimbursesheetYF.PayReimburseSheetID);
                    DataFactory.Database().Update<BY_PayReimburseSheet>(by_reimbursesheetYF, isOpenTrans);

                    decimal? CanUse = Convert.ToDecimal(CanUseMoney) + Convert.ToDecimal(zrmoney);

                    for (int i = 0; i < hids.Length; i++)
                    {

                        BY_PayReimburseSheet by_payreimbursesheet = new BY_PayReimburseSheet();
                        if (i != 0)
                        {
                            by_payreimbursesheet.ZRMoney = 0;
                        }
                        else
                        {
                            by_payreimbursesheet.ZRMoney = Convert.ToDecimal(zrmoney);
                        }
                        by_payreimbursesheet.Create();
                        by_payreimbursesheet.AuditMan = DNS.Utilities.ManageProvider.Provider.Current().UserName;
                        by_payreimbursesheet.Code = hids[i];
                        by_payreimbursesheet.Status = 1;
                        by_payreimbursesheet.RequiredMan = ManageProvider.Provider.Current().UserName;
                        by_payreimbursesheet.RequiredDate = Convert.ToDateTime(RequiredDate); ;
                        by_payreimbursesheet.CustomerCode = by_reimbursesheetYF.CustomerCode;
                        by_payreimbursesheet.CustomerName = by_reimbursesheetYF.CustomerName;
                        by_payreimbursesheet.CustomerId = by_reimbursesheetYF.CustomerId;
                        by_payreimbursesheet.Opions = "应付款";
                        by_payreimbursesheet.GetType = by_reimbursesheetYF.GetType;
                        by_payreimbursesheet.BankNum = by_reimbursesheetYF.BankNum;
                        if (type == "Base_SteelDDBeoutCode")
                        {
                            Base_SteelDDBeout base_steelddbeout = base_steelddbeouts.Where(p => p.Base_SteelDDBeoutCode == hids[i]).FirstOrDefault();//("Base_SteelDDBeoutCode", hids[i]);
                            by_payreimbursesheet.CodeType = "委外加工";
                            if (CanUse > base_steelddbeout.TotalAmount - base_steelddbeout.GetedAmount)
                            {
                                by_payreimbursesheet.RequiredMoney = base_steelddbeout.TotalAmount - base_steelddbeout.GetedAmount;
                                CanUse = CanUse - (base_steelddbeout.TotalAmount - base_steelddbeout.GetedAmount);
                            }
                            else
                            {
                                by_payreimbursesheet.RequiredMoney = CanUse;
                                CanUse = 0;
                            }
                            by_payreimbursesheet.GETMoney = by_payreimbursesheet.RequiredMoney - by_payreimbursesheet.ZRMoney;
                            by_payreimbursesheet.CanUseMoney = 0;
                            by_payreimbursesheet.TotalAmount = base_steelddbeout.TotalAmount;
                            by_payreimbursesheet.Remark = Remark;


                            base_steelddbeout.GetedAmount += by_payreimbursesheet.RequiredMoney;
                            by_payreimbursesheet.NotGetMoney = base_steelddbeout.TotalAmount - base_steelddbeout.GetedAmount;
                            by_payreimbursesheet.PayReimburseSheetCode = by_reimbursesheetYF.PayReimburseSheetID;
                            base_steelddbeout.Modify(base_steelddbeout.Base_SteelDDBeoutId);
                            DataFactory.Database().Update<Base_SteelDDBeout>(base_steelddbeout, isOpenTrans);
                            database.Insert(by_payreimbursesheet, isOpenTrans);

                        }
                        if (type == "Base_SteelDDPerchaseCode")
                        {
                            Base_SteelDDPerchase base_steelddperchase = base_steelddperchases.Where(p => p.Base_SteelDDPerchaseCode == hids[i]).FirstOrDefault();//("Base_SteelDDPerchaseCode", hids[i]);
                            by_payreimbursesheet.CodeType = "委外采购";

                            if (CanUse > base_steelddperchase.TotalAmount - base_steelddperchase.GetedAmount)
                            {
                                by_payreimbursesheet.RequiredMoney = base_steelddperchase.TotalAmount - base_steelddperchase.GetedAmount;
                                CanUse = CanUse - (base_steelddperchase.TotalAmount - base_steelddperchase.GetedAmount);
                            }
                            else
                            {
                                by_payreimbursesheet.RequiredMoney = CanUse;
                                CanUse = 0;
                            }

                            by_payreimbursesheet.GETMoney = by_payreimbursesheet.RequiredMoney - by_payreimbursesheet.ZRMoney;
                            by_payreimbursesheet.CanUseMoney = 0;
                            by_payreimbursesheet.TotalAmount = base_steelddperchase.TotalAmount;
                            by_payreimbursesheet.Remark = Remark;

                            base_steelddperchase.GetedAmount += by_payreimbursesheet.RequiredMoney;
                            by_payreimbursesheet.NotGetMoney = base_steelddperchase.TotalAmount - base_steelddperchase.GetedAmount;
                            by_payreimbursesheet.PayReimburseSheetCode = by_reimbursesheetYF.PayReimburseSheetID;
                            base_steelddperchase.Modify(base_steelddperchase.Base_SteelDDPerchaseId);
                            DataFactory.Database().Update<Base_SteelDDPerchase>(base_steelddperchase, isOpenTrans);
                            database.Insert(by_payreimbursesheet, isOpenTrans);

                        }
                        if (type == "PayCheckSheetCode")
                        {
                            BY_PayCheckSheel by_paychecksheel = by_paychecksheels.Where(p => p.PayCheckSheetCode == hids[i]).FirstOrDefault();//("PayCheckSheetCode", hids[i]);
                            by_payreimbursesheet.CodeType = "付款对账单";

                            if (CanUse > by_paychecksheel.TotalAmount - by_paychecksheel.GetedAmount)
                            {
                                by_payreimbursesheet.RequiredMoney = by_paychecksheel.TotalAmount - by_paychecksheel.GetedAmount;
                                CanUse = CanUse - (by_paychecksheel.TotalAmount - by_paychecksheel.GetedAmount);
                            }
                            else
                            {
                                by_payreimbursesheet.RequiredMoney = CanUse;
                                CanUse = 0;
                            }

                            by_payreimbursesheet.GETMoney = by_payreimbursesheet.RequiredMoney - by_payreimbursesheet.ZRMoney;
                            by_payreimbursesheet.CanUseMoney = 0;
                            by_payreimbursesheet.TotalAmount = by_paychecksheel.TotalAmount;
                            by_payreimbursesheet.Remark = Remark;

                            by_paychecksheel.GetedAmount += by_payreimbursesheet.RequiredMoney;
                            by_payreimbursesheet.NotGetMoney = by_paychecksheel.TotalAmount - by_paychecksheel.GetedAmount;
                            by_payreimbursesheet.PayReimburseSheetCode = by_reimbursesheetYF.PayReimburseSheetID;
                            by_paychecksheel.Modify(by_paychecksheel.PayCheckSheetId);
                            DataFactory.Database().Update<BY_PayCheckSheel>(by_paychecksheel, isOpenTrans);
                            database.Insert(by_payreimbursesheet, isOpenTrans);
                            if (by_paychecksheel.TotalAmount == by_paychecksheel.GetedAmount)
                            {
                                by_paychecksheeladd.Add(by_paychecksheel);
                            }
                        }
                        if (type == "MaterielPurchaseCode")
                        {
                            Base_MaterielPurchase base_materielpurchase = base_materielpurchases.Where(p => p.MaterielPurchaseCode == hids[i]).FirstOrDefault();//("MaterielPurchaseCode", hids[i]);
                            by_payreimbursesheet.CodeType = "物料申请单";

                            if (CanUse > base_materielpurchase.TotalAmount - base_materielpurchase.GetedAmount)
                            {
                                by_payreimbursesheet.RequiredMoney = base_materielpurchase.TotalAmount - base_materielpurchase.GetedAmount;
                                CanUse = CanUse - (base_materielpurchase.TotalAmount - base_materielpurchase.GetedAmount);
                            }
                            else
                            {
                                by_payreimbursesheet.RequiredMoney = CanUse;
                                CanUse = 0;
                            }

                            by_payreimbursesheet.GETMoney = by_payreimbursesheet.RequiredMoney - by_payreimbursesheet.ZRMoney;
                            by_payreimbursesheet.CanUseMoney = 0;
                            by_payreimbursesheet.TotalAmount = base_materielpurchase.TotalAmount;
                            by_payreimbursesheet.Remark = Remark;

                            base_materielpurchase.GetedAmount += by_payreimbursesheet.RequiredMoney;
                            by_payreimbursesheet.NotGetMoney = base_materielpurchase.TotalAmount - base_materielpurchase.GetedAmount;
                            by_payreimbursesheet.PayReimburseSheetCode = by_reimbursesheetYF.PayReimburseSheetID;
                            base_materielpurchase.Modify(base_materielpurchase.MaterielPurchaseId);
                            DataFactory.Database().Update<Base_MaterielPurchase>(base_materielpurchase, isOpenTrans);
                            database.Insert(by_payreimbursesheet, isOpenTrans);

                        }
                        if (type == "SteelPurchaseCode")
                        {
                            Base_SteelPurchase base_steelpurchase = base_steelpurchases.Where(p => p.SteelPurchaseCode == hids[i]).FirstOrDefault();//("SteelPurchaseCode", hids[i]);
                            by_payreimbursesheet.CodeType = "钢材申请单";

                            if (CanUse > base_steelpurchase.TotalAmount - base_steelpurchase.GetedAmount)
                            {
                                by_payreimbursesheet.RequiredMoney = base_steelpurchase.TotalAmount - base_steelpurchase.GetedAmount;
                                CanUse = CanUse - (base_steelpurchase.TotalAmount - base_steelpurchase.GetedAmount);
                            }
                            else
                            {
                                by_payreimbursesheet.RequiredMoney = CanUse;
                                CanUse = 0;
                            }

                            by_payreimbursesheet.GETMoney = by_payreimbursesheet.RequiredMoney - by_payreimbursesheet.ZRMoney;
                            by_payreimbursesheet.CanUseMoney = 0;
                            by_payreimbursesheet.TotalAmount = base_steelpurchase.TotalAmount;
                            by_payreimbursesheet.Remark = Remark;

                            base_steelpurchase.GetedAmount += by_payreimbursesheet.RequiredMoney;
                            by_payreimbursesheet.NotGetMoney = base_steelpurchase.TotalAmount - base_steelpurchase.GetedAmount;
                            by_payreimbursesheet.PayReimburseSheetCode = by_reimbursesheetYF.PayReimburseSheetID;
                            base_steelpurchase.Modify(base_steelpurchase.SteelPurchaseId);
                            DataFactory.Database().Update<Base_SteelPurchase>(base_steelpurchase, isOpenTrans);
                            database.Insert(by_payreimbursesheet, isOpenTrans);

                        }

                        if (type == "PayCompanyCode")
                        {
                            Base_PayCompany base_paycompany = base_paycompanys.Where(p => p.PayCompanyCode == hids[i]).FirstOrDefault();//("PayCompanyCode", hids[i]);
                            by_payreimbursesheet.CodeType = "供应商付款";

                            if (CanUse > base_paycompany.TotalAmount - base_paycompany.GetedAmount)
                            {
                                by_payreimbursesheet.RequiredMoney = base_paycompany.TotalAmount - base_paycompany.GetedAmount;
                                CanUse = CanUse - (base_paycompany.TotalAmount - base_paycompany.GetedAmount);
                            }
                            else
                            {
                                by_payreimbursesheet.RequiredMoney = CanUse;
                                CanUse = 0;
                            }

                            by_payreimbursesheet.GETMoney = by_payreimbursesheet.RequiredMoney - by_payreimbursesheet.ZRMoney;
                            by_payreimbursesheet.CanUseMoney = 0;
                            by_payreimbursesheet.TotalAmount = base_paycompany.TotalAmount;
                            by_payreimbursesheet.Remark = Remark;

                            base_paycompany.GetedAmount += by_payreimbursesheet.RequiredMoney;
                            by_payreimbursesheet.NotGetMoney = base_paycompany.TotalAmount - base_paycompany.GetedAmount;
                            by_payreimbursesheet.PayReimburseSheetCode = by_reimbursesheetYF.PayReimburseSheetID;
                            base_paycompany.Modify(base_paycompany.PayCompanyId);
                            DataFactory.Database().Update<Base_PayCompany>(base_paycompany, isOpenTrans);
                            database.Insert(by_payreimbursesheet, isOpenTrans);

                        }
                        if (type == "QuotationHeatBeoutCode")
                        {
                            BY_QuotationHeatBeout by_quotationheatbeout = by_quotationheatbeouts.Where(p => p.QuotationHeatBeoutCode == hids[i]).FirstOrDefault();//("QuotationHeatBeoutCode", hids[i]);
                            by_payreimbursesheet.CodeType = "热处理委外";

                            if (CanUse > by_quotationheatbeout.TotalAmount - by_quotationheatbeout.GetedAmount)
                            {
                                by_payreimbursesheet.RequiredMoney = by_quotationheatbeout.TotalAmount - by_quotationheatbeout.GetedAmount;
                                CanUse = CanUse - (by_quotationheatbeout.TotalAmount - by_quotationheatbeout.GetedAmount);
                            }
                            else
                            {
                                by_payreimbursesheet.RequiredMoney = CanUse;
                                CanUse = 0;
                            }

                            by_payreimbursesheet.GETMoney = by_payreimbursesheet.RequiredMoney - by_payreimbursesheet.ZRMoney;
                            by_payreimbursesheet.CanUseMoney = 0;
                            by_payreimbursesheet.TotalAmount = by_quotationheatbeout.TotalAmount;
                            by_payreimbursesheet.Remark = Remark;

                            by_quotationheatbeout.GetedAmount += by_payreimbursesheet.RequiredMoney;
                            by_payreimbursesheet.NotGetMoney = by_quotationheatbeout.TotalAmount - by_quotationheatbeout.GetedAmount;
                            by_payreimbursesheet.PayReimburseSheetCode = by_reimbursesheetYF.PayReimburseSheetID;
                            by_quotationheatbeout.Modify(by_quotationheatbeout.QuotationHeatBeoutId);
                            DataFactory.Database().Update<BY_QuotationHeatBeout>(by_quotationheatbeout, isOpenTrans);
                            database.Insert(by_payreimbursesheet, isOpenTrans);

                        }
                        if (CanUse == 0)
                        {
                            break;
                        }
                    }
                }
                if (Convert.ToDecimal(totalmoneys) <= Convert.ToDecimal(CanUseMoney) + Convert.ToDecimal(zrmoney))
                {
                    if (Convert.ToDecimal(totalmoneys) < Convert.ToDecimal(CanUseMoney))
                    {
                        by_reimbursesheetYF.CanUseMoney = by_reimbursesheetYF.CanUseMoney - Convert.ToDecimal(totalmoneys) + Convert.ToDecimal(zrmoney);
                        by_reimbursesheetYF.Remark = Remark;
                        by_reimbursesheetYF.Modify(by_reimbursesheetYF.PayReimburseSheetID);
                        DataFactory.Database().Update<BY_PayReimburseSheet>(by_reimbursesheetYF, isOpenTrans);
                    }
                    else
                    {
                        by_reimbursesheetYF.CanUseMoney = 0;
                        by_reimbursesheetYF.Remark = Remark;
                        by_reimbursesheetYF.Modify(by_reimbursesheetYF.PayReimburseSheetID);
                        DataFactory.Database().Update<BY_PayReimburseSheet>(by_reimbursesheetYF, isOpenTrans);
                    }

                    for (int i = 0; i < hids.Length; i++)
                    {
                        BY_PayReimburseSheet by_payreimbursesheet = new BY_PayReimburseSheet();
                        if (i != 0)
                        {
                            by_payreimbursesheet.ZRMoney = 0;
                        }
                        else
                        {
                            by_payreimbursesheet.ZRMoney = Convert.ToDecimal(zrmoney);
                        }
                        by_payreimbursesheet.Create();
                        by_payreimbursesheet.AuditMan = DNS.Utilities.ManageProvider.Provider.Current().UserName;
                        by_payreimbursesheet.Code = hids[i];
                        by_payreimbursesheet.Status = 1;
                        by_payreimbursesheet.RequiredMan = ManageProvider.Provider.Current().UserName;
                        by_payreimbursesheet.RequiredDate = Convert.ToDateTime(RequiredDate);
                        by_payreimbursesheet.CustomerCode = by_reimbursesheetYF.CustomerCode;
                        by_payreimbursesheet.CustomerName = by_reimbursesheetYF.CustomerName;
                        by_payreimbursesheet.CustomerId = by_reimbursesheetYF.CustomerId;
                        by_payreimbursesheet.Opions = "应付款";
                        by_payreimbursesheet.GetType = by_reimbursesheetYF.GetType;
                        by_payreimbursesheet.BankNum = by_reimbursesheetYF.BankNum;

                        if (type == "Base_SteelDDBeoutCode")
                        {
                            Base_SteelDDBeout base_steelddbeout = base_steelddbeouts.Where(p => p.Base_SteelDDBeoutCode == hids[i]).FirstOrDefault();//("Base_SteelDDBeoutCode", hids[i]);
                            by_payreimbursesheet.CodeType = "委外加工";
                            by_payreimbursesheet.RequiredMoney = base_steelddbeout.TotalAmount - base_steelddbeout.GetedAmount;
                            by_payreimbursesheet.GETMoney = by_payreimbursesheet.RequiredMoney - by_payreimbursesheet.ZRMoney;
                            by_payreimbursesheet.CanUseMoney = 0;
                            by_payreimbursesheet.TotalAmount = base_steelddbeout.TotalAmount;
                            by_payreimbursesheet.Remark = Remark;



                            base_steelddbeout.GetedAmount += by_payreimbursesheet.RequiredMoney;
                            by_payreimbursesheet.NotGetMoney = base_steelddbeout.TotalAmount - base_steelddbeout.GetedAmount;
                            by_payreimbursesheet.PayReimburseSheetCode = by_reimbursesheetYF.PayReimburseSheetID;
                            base_steelddbeout.Modify(base_steelddbeout.Base_SteelDDBeoutId);
                            DataFactory.Database().Update<Base_SteelDDBeout>(base_steelddbeout, isOpenTrans);
                            database.Insert(by_payreimbursesheet, isOpenTrans);


                        }
                        if (type == "Base_SteelDDPerchaseCode")
                        {
                            Base_SteelDDPerchase base_steelddperchase = base_steelddperchases.Where(p => p.Base_SteelDDPerchaseCode == hids[i]).FirstOrDefault();//("Base_SteelDDPerchaseCode", hids[i]);
                            by_payreimbursesheet.CodeType = "委外采购";
                            by_payreimbursesheet.RequiredMoney = base_steelddperchase.TotalAmount - base_steelddperchase.GetedAmount;
                            by_payreimbursesheet.GETMoney = by_payreimbursesheet.RequiredMoney - by_payreimbursesheet.ZRMoney;
                            by_payreimbursesheet.CanUseMoney = 0;
                            by_payreimbursesheet.TotalAmount = base_steelddperchase.TotalAmount;
                            by_payreimbursesheet.Remark = Remark;


                            base_steelddperchase.GetedAmount += by_payreimbursesheet.RequiredMoney;
                            by_payreimbursesheet.NotGetMoney = base_steelddperchase.TotalAmount - base_steelddperchase.GetedAmount;
                            by_payreimbursesheet.PayReimburseSheetCode = by_reimbursesheetYF.PayReimburseSheetID;
                            base_steelddperchase.Modify(base_steelddperchase.Base_SteelDDPerchaseId);
                            DataFactory.Database().Update<Base_SteelDDPerchase>(base_steelddperchase, isOpenTrans);
                            database.Insert(by_payreimbursesheet, isOpenTrans);
                        }


                        if (type == "MaterielPurchaseCode")
                        {
                            Base_MaterielPurchase base_materielpurchase = base_materielpurchases.Where(p => p.MaterielPurchaseCode == hids[i]).FirstOrDefault();//("MaterielPurchaseCode", hids[i]);
                            by_payreimbursesheet.CodeType = "物料申请单";
                            by_payreimbursesheet.RequiredMoney = base_materielpurchase.TotalAmount - base_materielpurchase.GetedAmount;
                            by_payreimbursesheet.GETMoney = by_payreimbursesheet.RequiredMoney - by_payreimbursesheet.ZRMoney;
                            by_payreimbursesheet.CanUseMoney = 0;
                            by_payreimbursesheet.TotalAmount = base_materielpurchase.TotalAmount;
                            by_payreimbursesheet.Remark = Remark;


                            base_materielpurchase.GetedAmount += by_payreimbursesheet.RequiredMoney;
                            by_payreimbursesheet.NotGetMoney = base_materielpurchase.TotalAmount - base_materielpurchase.GetedAmount;
                            by_payreimbursesheet.PayReimburseSheetCode = by_reimbursesheetYF.PayReimburseSheetID;
                            base_materielpurchase.Modify(base_materielpurchase.MaterielPurchaseId);
                            DataFactory.Database().Update<Base_MaterielPurchase>(base_materielpurchase, isOpenTrans);
                            database.Insert(by_payreimbursesheet, isOpenTrans);
                        }


                        if (type == "PayCheckSheetCode")
                        {
                            BY_PayCheckSheel by_paychecksheel = by_paychecksheels.Where(p => p.PayCheckSheetCode == hids[i]).FirstOrDefault();//("PayCheckSheetCode", hids[i]);
                            by_payreimbursesheet.CodeType = "付款对账单";
                            by_payreimbursesheet.RequiredMoney = by_paychecksheel.TotalAmount - by_paychecksheel.GetedAmount;
                            by_payreimbursesheet.GETMoney = by_payreimbursesheet.RequiredMoney - by_payreimbursesheet.ZRMoney;
                            by_payreimbursesheet.CanUseMoney = 0;
                            by_payreimbursesheet.TotalAmount = by_paychecksheel.TotalAmount;
                            by_payreimbursesheet.Remark = Remark;


                            by_paychecksheel.GetedAmount += by_payreimbursesheet.RequiredMoney;
                            by_payreimbursesheet.NotGetMoney = by_paychecksheel.TotalAmount - by_paychecksheel.GetedAmount;
                            by_payreimbursesheet.PayReimburseSheetCode = by_reimbursesheetYF.PayReimburseSheetID;
                            by_paychecksheel.Modify(by_paychecksheel.PayCheckSheetId);
                            DataFactory.Database().Update<BY_PayCheckSheel>(by_paychecksheel, isOpenTrans);
                            database.Insert(by_payreimbursesheet, isOpenTrans);
                            if (by_paychecksheel.TotalAmount == by_paychecksheel.GetedAmount)
                            {
                                by_paychecksheeladd.Add(by_paychecksheel);
                            }
                        }

                        if (type == "SteelPurchaseCode")
                        {
                            Base_SteelPurchase base_steelpurchase = base_steelpurchases.Where(p => p.SteelPurchaseCode == hids[i]).FirstOrDefault();//("SteelPurchaseCode", hids[i]);
                            by_payreimbursesheet.CodeType = "钢材申请单";
                            by_payreimbursesheet.RequiredMoney = base_steelpurchase.TotalAmount - base_steelpurchase.GetedAmount;
                            by_payreimbursesheet.GETMoney = by_payreimbursesheet.RequiredMoney - by_payreimbursesheet.ZRMoney;
                            by_payreimbursesheet.CanUseMoney = 0;
                            by_payreimbursesheet.TotalAmount = base_steelpurchase.TotalAmount;
                            by_payreimbursesheet.Remark = Remark;


                            base_steelpurchase.GetedAmount += by_payreimbursesheet.RequiredMoney;
                            by_payreimbursesheet.NotGetMoney = base_steelpurchase.TotalAmount - base_steelpurchase.GetedAmount;
                            by_payreimbursesheet.PayReimburseSheetCode = by_reimbursesheetYF.PayReimburseSheetID;
                            base_steelpurchase.Modify(base_steelpurchase.SteelPurchaseId);
                            DataFactory.Database().Update<Base_SteelPurchase>(base_steelpurchase, isOpenTrans);
                            database.Insert(by_payreimbursesheet, isOpenTrans);

                        }

                        if (type == "PayCompanyCode")
                        {
                            Base_PayCompany base_paycompany = base_paycompanys.Where(p => p.PayCompanyCode == hids[i]).FirstOrDefault();//("PayCompanyCode", hids[i]);
                            by_payreimbursesheet.CodeType = "供应商付款";
                            by_payreimbursesheet.RequiredMoney = base_paycompany.TotalAmount - base_paycompany.GetedAmount;
                            by_payreimbursesheet.GETMoney = by_payreimbursesheet.RequiredMoney - by_payreimbursesheet.ZRMoney;
                            by_payreimbursesheet.CanUseMoney = 0;
                            by_payreimbursesheet.TotalAmount = base_paycompany.TotalAmount;
                            by_payreimbursesheet.Remark = Remark;

                            base_paycompany.GetedAmount += by_payreimbursesheet.RequiredMoney;
                            by_payreimbursesheet.NotGetMoney = base_paycompany.TotalAmount - base_paycompany.GetedAmount;
                            by_payreimbursesheet.PayReimburseSheetCode = by_reimbursesheetYF.PayReimburseSheetID;
                            base_paycompany.Modify(base_paycompany.PayCompanyId);
                            DataFactory.Database().Update<Base_PayCompany>(base_paycompany, isOpenTrans);
                            database.Insert(by_payreimbursesheet, isOpenTrans);

                        }
                        if (type == "QuotationHeatBeoutCode")
                        {
                            BY_QuotationHeatBeout by_quotationheatbeout = by_quotationheatbeouts.Where(p => p.QuotationHeatBeoutCode == hids[i]).FirstOrDefault();//("QuotationHeatBeoutCode", hids[i]);
                            by_payreimbursesheet.CodeType = "热处理委外";
                            by_payreimbursesheet.RequiredMoney = by_quotationheatbeout.TotalAmount - by_quotationheatbeout.GetedAmount;
                            by_payreimbursesheet.GETMoney = by_payreimbursesheet.RequiredMoney - by_payreimbursesheet.ZRMoney;
                            by_payreimbursesheet.CanUseMoney = 0;
                            by_payreimbursesheet.TotalAmount = by_quotationheatbeout.TotalAmount;
                            by_payreimbursesheet.Remark = Remark;

                            by_quotationheatbeout.GetedAmount += by_payreimbursesheet.RequiredMoney;
                            by_payreimbursesheet.NotGetMoney = by_quotationheatbeout.TotalAmount - by_quotationheatbeout.GetedAmount;
                            by_payreimbursesheet.PayReimburseSheetCode = by_reimbursesheetYF.PayReimburseSheetID;
                            by_quotationheatbeout.Modify(by_quotationheatbeout.QuotationHeatBeoutId);
                            DataFactory.Database().Update<BY_QuotationHeatBeout>(by_quotationheatbeout, isOpenTrans);
                            database.Insert(by_payreimbursesheet, isOpenTrans);

                        }

                    }
                }



                List<Base_SteelDDBeout> Base_SteelDDBeouts = new List<Base_SteelDDBeout>();
                List<Base_SteelDDPerchase> Base_SteelDDPerchases = new List<Base_SteelDDPerchase>();
                List<Base_SteelPurchase> Base_SteelPurchases = new List<Base_SteelPurchase>();
                List<Base_MaterielPurchase> Base_MaterielPurchases = new List<Base_MaterielPurchase>();
                List<BY_QuotationHeatBeout> BY_QuotationHeatBeouts = new List<BY_QuotationHeatBeout>();
                List<Base_PayCompany> Base_PayCompanys = new List<Base_PayCompany>();


                foreach (BY_PayCheckSheel itmes in by_paychecksheeladd)
                {
                    Base_SteelDDBeouts.AddRange(DataFactory.Database().FindList<Base_SteelDDBeout>("PayCheckSheetCode", itmes.PayCheckSheetCode));
                    Base_SteelDDPerchases.AddRange(DataFactory.Database().FindList<Base_SteelDDPerchase>("PayCheckSheetCode", itmes.PayCheckSheetCode));
                    Base_SteelPurchases.AddRange(DataFactory.Database().FindList<Base_SteelPurchase>("PayCheckSheetCode", itmes.PayCheckSheetCode));
                    Base_MaterielPurchases.AddRange(DataFactory.Database().FindList<Base_MaterielPurchase>("PayCheckSheetCode", itmes.PayCheckSheetCode));
                    BY_QuotationHeatBeouts.AddRange(DataFactory.Database().FindList<BY_QuotationHeatBeout>("PayCheckSheetCode", itmes.PayCheckSheetCode));
                    Base_PayCompanys.AddRange(DataFactory.Database().FindList<Base_PayCompany>("PayCheckSheetCode", itmes.PayCheckSheetCode));
                }
                foreach (Base_SteelDDBeout item in Base_SteelDDBeouts)
                {
                    item.GetedAmount = item.TotalAmount;
                    item.Modify(item.Base_SteelDDBeoutId);
                    DataFactory.Database().Update<Base_SteelDDBeout>(item, isOpenTrans);
                }
                foreach (Base_SteelDDPerchase item in Base_SteelDDPerchases)
                {
                    item.GetedAmount = item.TotalAmount;
                    item.Modify(item.Base_SteelDDPerchaseId);
                    DataFactory.Database().Update<Base_SteelDDPerchase>(item, isOpenTrans);
                }
                foreach (Base_SteelPurchase item in Base_SteelPurchases)
                {
                    item.GetedAmount = item.TotalAmount;
                    item.Modify(item.SteelPurchaseId);
                    DataFactory.Database().Update<Base_SteelPurchase>(item, isOpenTrans);
                }
                foreach (Base_MaterielPurchase item in Base_MaterielPurchases)
                {
                    item.GetedAmount = item.TotalAmount;
                    item.Modify(item.MaterielPurchaseId);
                    DataFactory.Database().Update<Base_MaterielPurchase>(item, isOpenTrans);
                }
                foreach (BY_QuotationHeatBeout item in BY_QuotationHeatBeouts)
                {
                    item.GetedAmount = item.TotalAmount;
                    item.Modify(item.QuotationHeatBeoutId);
                    DataFactory.Database().Update<BY_QuotationHeatBeout>(item, isOpenTrans);
                }
                foreach (Base_PayCompany item in Base_PayCompanys)
                {
                    item.GetedAmount = item.TotalAmount;
                    item.Modify(item.PayCompanyId);
                    DataFactory.Database().Update<Base_PayCompany>(item, isOpenTrans);
                }


                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }



        [ManagerPermission(PermissionMode.Enforce)]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult ReturnMoneyView()
        {
            return View();
        }
        [ManagerPermission(PermissionMode.Enforce)]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult PayMoneyView()
        {
            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult SubmitFormCost(BY_PayReimburseSheet by_payreimbursesheet1, string Code1)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                List<BY_PayReimburseSheet> by_payreimbursesheets = new List<BY_PayReimburseSheet>();
                string[] codes = (Code1.TrimEnd(',')).Split(',');
                decimal? CanUse = Convert.ToDecimal(by_payreimbursesheet1.GETMoney) + Convert.ToDecimal(by_payreimbursesheet1.ZRMoney);

                string ids = "";
                for (int i = 0; i < codes.Length; i++)
                {
                    ids = ids + "'" + codes[i] + "',";
                }
                ids = ids.TrimEnd(',');
                List<BY_PayCheckSheel> by_paychecksheels = new List<BY_PayCheckSheel>();
                List<Base_PaySheet> base_paysheets = new List<Base_PaySheet>();
                List<Base_MaterielPurchase> base_materielpurchases = new List<Base_MaterielPurchase>();
                List<Base_SteelPurchase> base_steelpurchases = new List<Base_SteelPurchase>();
                List<Base_PayCompany> base_paycompanys = new List<Base_PayCompany>();

                if (string.IsNullOrEmpty(Code1) && by_payreimbursesheet1.Opions == "预付款")
                {
                    by_payreimbursesheet1.Create();
                    //by_reimbursesheet1.AuditMan = DNS.Utilities.ManageProvider.Provider.Current().UserName;
                    by_payreimbursesheet1.RequiredMan = ManageProvider.Provider.Current().UserName;
                    by_payreimbursesheet1.CanUseMoney = by_payreimbursesheet1.RequiredMoney;
                    DataFactory.Database().Insert<BY_PayReimburseSheet>(by_payreimbursesheet1, isOpenTrans);
                    database.Commit();
                    return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功" }.ToString());

                }

                List<Base_SteelDDBeout> base_steelddbeouts = new List<Base_SteelDDBeout>();
                List<Base_SteelDDPerchase> base_steelddperchases = new List<Base_SteelDDPerchase>();

                List<BY_PayCheckSheel> by_paychecksheeladd = new List<BY_PayCheckSheel>();
                List<BY_QuotationHeatBeout> by_quotationheatbeouts = new List<BY_QuotationHeatBeout>();

                if (Code1.Substring(0, 2) == "PC")
                {
                    by_paychecksheels = database.FindList<BY_PayCheckSheel>(string.Format("  and PayCheckSheetCode in ({0})", ids));
                }
                else if (Code1.Substring(0, 2) == "CG")
                {
                    base_steelddperchases = database.FindList<Base_SteelDDPerchase>(string.Format("  and Base_SteelDDPerchaseCode in ({0})", ids));
                }
                else if (Code1.Substring(0, 2) == "WF")
                {
                    base_steelddbeouts = database.FindList<Base_SteelDDBeout>(string.Format("  and Base_SteelDDBeoutCode in ({0})", ids));
                }
                else if (Code1.Substring(0, 1) == "H")
                {
                    by_quotationheatbeouts = database.FindList<BY_QuotationHeatBeout>(string.Format("  and QuotationHeatBeoutCode in ({0})", ids));
                }
                else if (Code1.Substring(0, 1) == "C")
                {
                    base_paysheets = database.FindList<Base_PaySheet>(string.Format("  and PaySheetCode in ({0})", ids));
                }
                else if (Code1.Substring(0, 1) == "P")
                {
                    base_materielpurchases = database.FindList<Base_MaterielPurchase>(string.Format("  and MaterielPurchaseCode in ({0})", ids));
                }
                else if (Code1.Substring(0, 1) == "S")
                {
                    base_steelpurchases = database.FindList<Base_SteelPurchase>(string.Format("  and SteelPurchaseCode in ({0})", ids));
                }
                else if (Code1.Substring(0, 1) == "F")
                {
                    base_paycompanys = database.FindList<Base_PayCompany>(string.Format("  and PayCompanyCode in ({0})", ids));
                }
                else
                {
                    database.Rollback();
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：无此单号" }.ToString());
                }



                for (int i = 0; i < codes.Length; i++)
                {
                    BY_PayReimburseSheet by_payreimbursesheet = new BY_PayReimburseSheet();
                    CommonHelper.CopySameFieldsObjectALL<BY_PayReimburseSheet, BY_PayReimburseSheet>(by_payreimbursesheet1, ref by_payreimbursesheet);
                    if (i != 0)
                    {
                        by_payreimbursesheet.ZRMoney = 0;
                    }
                    string Code = codes[i];
                    BY_PayCheckSheel by_paychecksheel = new BY_PayCheckSheel();
                    Base_PaySheet base_paysheet = new Base_PaySheet();
                    Base_MaterielPurchase base_materielpurchase = new Base_MaterielPurchase();
                    Base_SteelPurchase base_steelpurchase = new Base_SteelPurchase();
                    Base_PayCompany base_paycompany = new Base_PayCompany();
                    Base_SteelDDPerchase base_steelddperchase = new Base_SteelDDPerchase();
                    Base_SteelDDBeout base_steelddbeout = new Base_SteelDDBeout();
                    BY_QuotationHeatBeout by_quotationheatbeout = new BY_QuotationHeatBeout();

                    if (by_payreimbursesheet.Code.Substring(0, 2) == "PC")
                    {
                        by_payreimbursesheet.CodeType = "付款对账单";
                        by_paychecksheel = by_paychecksheels.Find(p => p.PayCheckSheetCode == Code);

                        if (CanUse > by_paychecksheel.TotalAmount - by_paychecksheel.GetedAmount)
                        {
                            by_payreimbursesheet.RequiredMoney = by_paychecksheel.TotalAmount - by_paychecksheel.GetedAmount;
                            CanUse = CanUse - (by_paychecksheel.TotalAmount - by_paychecksheel.GetedAmount);
                        }
                        else
                        {
                            by_payreimbursesheet.RequiredMoney = CanUse;
                            CanUse = 0;
                        }
                        by_payreimbursesheet.GETMoney = by_payreimbursesheet.RequiredMoney - by_payreimbursesheet.ZRMoney;
                        by_payreimbursesheet.CanUseMoney = 0;
                        by_payreimbursesheet.TotalAmount = by_paychecksheel.TotalAmount;
                        by_payreimbursesheet.InvoiceNum = by_paychecksheel.InvoiceNum;
                    }
                    else if (by_payreimbursesheet.Code.Substring(0, 2) == "CG")
                    {
                        by_payreimbursesheet.CodeType = "委外采购";
                        base_steelddperchase = base_steelddperchases.Find(p => p.Base_SteelDDPerchaseCode == Code);

                        if (CanUse > base_steelddperchase.TotalAmount - base_steelddperchase.GetedAmount)
                        {
                            by_payreimbursesheet.RequiredMoney = base_steelddperchase.TotalAmount - base_steelddperchase.GetedAmount;
                            CanUse = CanUse - (base_steelddperchase.TotalAmount - base_steelddperchase.GetedAmount);
                        }
                        else
                        {
                            by_payreimbursesheet.RequiredMoney = CanUse;
                            CanUse = 0;
                        }
                        by_payreimbursesheet.GETMoney = by_payreimbursesheet.RequiredMoney - by_payreimbursesheet.ZRMoney;
                        by_payreimbursesheet.CanUseMoney = 0;
                        by_payreimbursesheet.TotalAmount = base_steelddperchase.TotalAmount;
                    }
                    else if (by_payreimbursesheet.Code.Substring(0, 2) == "WF")
                    {
                        by_payreimbursesheet.CodeType = "委外加工";
                        base_steelddbeout = base_steelddbeouts.Find(p => p.Base_SteelDDBeoutCode == Code);

                        if (CanUse > base_steelddbeout.TotalAmount - base_steelddbeout.GetedAmount)
                        {
                            by_payreimbursesheet.RequiredMoney = base_steelddbeout.TotalAmount - base_steelddbeout.GetedAmount;
                            CanUse = CanUse - (base_steelddbeout.TotalAmount - base_steelddbeout.GetedAmount);
                        }
                        else
                        {
                            by_payreimbursesheet.RequiredMoney = CanUse;
                            CanUse = 0;
                        }
                        by_payreimbursesheet.GETMoney = by_payreimbursesheet.RequiredMoney - by_payreimbursesheet.ZRMoney;
                        by_payreimbursesheet.CanUseMoney = 0;
                        by_payreimbursesheet.TotalAmount = base_steelddbeout.TotalAmount;
                    }
                    else if (by_payreimbursesheet.Code.Substring(0, 1) == "H")
                    {
                        by_payreimbursesheet.CodeType = "热处理委外";
                        by_quotationheatbeout = by_quotationheatbeouts.Find(p => p.QuotationHeatBeoutCode == Code);

                        if (CanUse > by_quotationheatbeout.TotalAmount - by_quotationheatbeout.GetedAmount)
                        {
                            by_payreimbursesheet.RequiredMoney = by_quotationheatbeout.TotalAmount - by_quotationheatbeout.GetedAmount;
                            CanUse = CanUse - (by_quotationheatbeout.TotalAmount - by_quotationheatbeout.GetedAmount);
                        }
                        else
                        {
                            by_payreimbursesheet.RequiredMoney = CanUse;
                            CanUse = 0;
                        }
                        by_payreimbursesheet.GETMoney = by_payreimbursesheet.RequiredMoney - by_payreimbursesheet.ZRMoney;
                        by_payreimbursesheet.CanUseMoney = 0;
                        by_payreimbursesheet.TotalAmount = by_quotationheatbeout.TotalAmount;
                    }
                    else if (by_payreimbursesheet.Code.Substring(0, 1) == "C")
                    {
                        by_payreimbursesheet.CodeType = "内部费用";
                        base_paysheet = base_paysheets.Find(p => p.PaySheetCode == Code);

                        if (CanUse > base_paysheet.TotalAmount - base_paysheet.GetedAmount)
                        {
                            by_payreimbursesheet.RequiredMoney = base_paysheet.TotalAmount - base_paysheet.GetedAmount;
                            CanUse = CanUse - (base_paysheet.TotalAmount - base_paysheet.GetedAmount);
                        }
                        else
                        {
                            by_payreimbursesheet.RequiredMoney = CanUse;
                            CanUse = 0;
                        }
                        by_payreimbursesheet.GETMoney = by_payreimbursesheet.RequiredMoney - by_payreimbursesheet.ZRMoney;
                        by_payreimbursesheet.CanUseMoney = 0;
                        by_payreimbursesheet.TotalAmount = base_paysheet.TotalAmount;

                        by_payreimbursesheet.WorkerId = base_paysheet.WorkerId;
                        by_payreimbursesheet.WorkerName = base_paysheet.WorkerName;
                        by_payreimbursesheet.WorkerCode = base_paysheet.WorkerCode;
                        by_payreimbursesheet.DepartmentName = base_paysheet.DepartmentName;


                    }
                    else if (by_payreimbursesheet.Code.Substring(0, 1) == "P")
                    {
                        by_payreimbursesheet.CodeType = "物料申请单";
                        base_materielpurchase = base_materielpurchases.Find(p => p.MaterielPurchaseCode == Code);

                        if (CanUse > base_materielpurchase.TotalAmount - base_materielpurchase.GetedAmount)
                        {
                            by_payreimbursesheet.RequiredMoney = base_materielpurchase.TotalAmount - base_materielpurchase.GetedAmount;
                            CanUse = CanUse - (base_materielpurchase.TotalAmount - base_materielpurchase.GetedAmount);
                        }
                        else
                        {
                            by_payreimbursesheet.RequiredMoney = CanUse;
                            CanUse = 0;
                        }
                        by_payreimbursesheet.GETMoney = by_payreimbursesheet.RequiredMoney - by_payreimbursesheet.ZRMoney;
                        by_payreimbursesheet.CanUseMoney = 0;
                        by_payreimbursesheet.TotalAmount = base_materielpurchase.TotalAmount;
                    }
                    else if (by_payreimbursesheet.Code.Substring(0, 1) == "S")
                    {
                        by_payreimbursesheet.CodeType = "钢材申请单";
                        base_steelpurchase = base_steelpurchases.Find(p => p.SteelPurchaseCode == Code);

                        if (CanUse > base_steelpurchase.TotalAmount - base_steelpurchase.GetedAmount)
                        {
                            by_payreimbursesheet.RequiredMoney = base_steelpurchase.TotalAmount - base_steelpurchase.GetedAmount;
                            CanUse = CanUse - (base_steelpurchase.TotalAmount - base_steelpurchase.GetedAmount);
                        }
                        else
                        {
                            by_payreimbursesheet.RequiredMoney = CanUse;
                            CanUse = 0;
                        }
                        by_payreimbursesheet.GETMoney = by_payreimbursesheet.RequiredMoney - by_payreimbursesheet.ZRMoney;
                        by_payreimbursesheet.CanUseMoney = 0;
                        by_payreimbursesheet.TotalAmount = base_steelpurchase.TotalAmount;
                    }
                    else if (by_payreimbursesheet.Code.Substring(0, 1) == "F")
                    {
                        by_payreimbursesheet.CodeType = "供应商付款";
                        base_paycompany = base_paycompanys.Find(p => p.PayCompanyCode == Code);

                        if (CanUse > base_paycompany.TotalAmount - base_paycompany.GetedAmount)
                        {
                            by_payreimbursesheet.RequiredMoney = base_paycompany.TotalAmount - base_paycompany.GetedAmount;
                            CanUse = CanUse - (base_paycompany.TotalAmount - base_paycompany.GetedAmount);
                        }
                        else
                        {
                            by_payreimbursesheet.RequiredMoney = CanUse;
                            CanUse = 0;
                        }
                        by_payreimbursesheet.GETMoney = by_payreimbursesheet.RequiredMoney - by_payreimbursesheet.ZRMoney;
                        by_payreimbursesheet.CanUseMoney = 0;
                        by_payreimbursesheet.TotalAmount = base_paycompany.TotalAmount;
                    }

                    by_payreimbursesheet.Create();
                    by_payreimbursesheet.AuditMan = DNS.Utilities.ManageProvider.Provider.Current().UserName;
                    by_payreimbursesheet.Code = Code;
                    by_payreimbursesheet.Status = 1;
                    by_payreimbursesheet.RequiredMan = ManageProvider.Provider.Current().UserName;

                    //by_reimbursesheets.Add(by_payreimbursesheet);

                    //Base_CodeRuleBll base_coderulebll = new Base_CodeRuleBll();
                    //by_payreimbursesheet.PayReimburseSheetCode = base_coderulebll.GetBillCode(ManageProvider.Provider.Current().UserId, "SC");
                    //base_coderulebll.OccupyBillCode(ManageProvider.Provider.Current().UserId, "SC", isOpenTrans);//--获取后必须占用

                    //BY_PayCheckSheel by_paychecksheel = new BY_PayCheckSheel();
                    //Base_PaySheet base_paysheet = new Base_PaySheet();
                    //Base_MaterielPurchase base_materielpurchase = new Base_MaterielPurchase();
                    //Base_SteelPurchase base_steelpurchase = new Base_SteelPurchase();
                    //Base_PayCompany base_paycompany = new Base_PayCompany();
                    //付款单
                    if (by_payreimbursesheet.CodeType == "付款对账单")
                    {
                        if (by_paychecksheel.TotalAmount - by_paychecksheel.GetedAmount < by_payreimbursesheet.RequiredMoney)
                        {
                            database.Rollback();
                            return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：超过此付款单可付金额！可付金额为：" + (by_paychecksheel.TotalAmount - by_paychecksheel.GetedAmount).ToString() }.ToString());
                        }
                        by_paychecksheel.GetedAmount += by_payreimbursesheet.RequiredMoney;
                        by_paychecksheel.Modify(by_paychecksheel.PayCheckSheetId);


                        if (by_paychecksheel.TotalAmount == by_paychecksheel.GetedAmount)
                        {
                            by_paychecksheeladd.Add(by_paychecksheel);
                        }


                        DataFactory.Database().Update<BY_PayCheckSheel>(by_paychecksheel, isOpenTrans);
                        by_payreimbursesheet.NotGetMoney = by_paychecksheel.TotalAmount - by_paychecksheel.GetedAmount;


                        by_payreimbursesheet.CustomerName = by_paychecksheel.CustomerName;
                        by_payreimbursesheet.CustomerCode = by_paychecksheel.CustomerCode;
                        by_payreimbursesheet.CustomerId = by_paychecksheel.CustomerId;
                        by_payreimbursesheet.Remark1 = by_paychecksheel.Remark;

                    }
                    if (by_payreimbursesheet.CodeType == "委外采购")
                    {
                        if (base_steelddperchase.TotalAmount - base_steelddperchase.GetedAmount < by_payreimbursesheet.RequiredMoney)
                        {
                            database.Rollback();
                            return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：超过此付款单可付金额！可付金额为：" + (base_steelddperchase.TotalAmount - base_steelddperchase.GetedAmount).ToString() }.ToString());
                        }
                        base_steelddperchase.GetedAmount += by_payreimbursesheet.RequiredMoney;
                        base_steelddperchase.Modify(base_steelddperchase.Base_SteelDDPerchaseId);
                        DataFactory.Database().Update<Base_SteelDDPerchase>(base_steelddperchase, isOpenTrans);
                        by_payreimbursesheet.NotGetMoney = base_steelddperchase.TotalAmount - base_steelddperchase.GetedAmount;

                        by_payreimbursesheet.CustomerName = base_steelddperchase.CustomerName;
                        by_payreimbursesheet.CustomerCode = base_steelddperchase.CustomerCode;
                        by_payreimbursesheet.CustomerId = base_steelddperchase.CustomerId;
                        by_payreimbursesheet.WorkerCode = base_steelddperchase.WorkerCode;
                        by_payreimbursesheet.WorkerName = base_steelddperchase.WorkerName;
                        by_payreimbursesheet.WorkerId = base_steelddperchase.WorkerId;
                        by_payreimbursesheet.DepartmentName = base_steelddperchase.DepartmentName;
                        by_payreimbursesheet.Remark1 = base_steelddperchase.Remark;
                    }
                    if (by_payreimbursesheet.CodeType == "委外加工")
                    {
                        if (base_steelddbeout.TotalAmount - base_steelddbeout.GetedAmount < by_payreimbursesheet.RequiredMoney)
                        {
                            database.Rollback();
                            return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：超过此付款单可付金额！可付金额为：" + (base_steelddbeout.TotalAmount - base_steelddbeout.GetedAmount).ToString() }.ToString());
                        }
                        base_steelddbeout.GetedAmount += by_payreimbursesheet.RequiredMoney;
                        base_steelddbeout.Modify(base_steelddbeout.Base_SteelDDBeoutId);
                        DataFactory.Database().Update<Base_SteelDDBeout>(base_steelddbeout, isOpenTrans);
                        by_payreimbursesheet.NotGetMoney = base_steelddbeout.TotalAmount - base_steelddbeout.GetedAmount;

                        by_payreimbursesheet.CustomerName = base_steelddbeout.CustomerName;
                        by_payreimbursesheet.CustomerCode = base_steelddbeout.CustomerCode;
                        by_payreimbursesheet.CustomerId = base_steelddbeout.CustomerId;
                        by_payreimbursesheet.WorkerCode = base_steelddbeout.WorkerCode;
                        by_payreimbursesheet.WorkerName = base_steelddbeout.WorkerName;
                        by_payreimbursesheet.WorkerId = base_steelddbeout.WorkerId;
                        by_payreimbursesheet.DepartmentName = base_steelddbeout.DepartmentName;
                        by_payreimbursesheet.Remark1 = base_steelddbeout.Remark;
                    }
                    else if (by_payreimbursesheet.CodeType == "内部费用")
                    {
                        if (base_paysheet.TotalAmount - base_paysheet.GetedAmount < by_payreimbursesheet.RequiredMoney)
                        {
                            database.Rollback();
                            return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：超过此付款单可付金额！可付金额为：" + (base_paysheet.TotalAmount - base_paysheet.GetedAmount).ToString() }.ToString());
                        }
                        base_paysheet.GetedAmount += by_payreimbursesheet.RequiredMoney;
                        base_paysheet.Modify(base_paysheet.PaySheetId);
                        DataFactory.Database().Update<Base_PaySheet>(base_paysheet, isOpenTrans);
                        by_payreimbursesheet.NotGetMoney = base_paysheet.TotalAmount - base_paysheet.GetedAmount;

                        by_payreimbursesheet.WorkerCode = base_paysheet.WorkerCode;
                        by_payreimbursesheet.WorkerName = base_paysheet.WorkerName;
                        by_payreimbursesheet.WorkerId = base_paysheet.WorkerId;
                        by_payreimbursesheet.DepartmentName = base_paysheet.DepartmentName;
                        by_payreimbursesheet.Remark1 = base_paysheet.Remark;
                    }
                    else if (by_payreimbursesheet.CodeType == "物料申请单")
                    {
                        if (base_materielpurchase.TotalAmount - base_materielpurchase.GetedAmount < by_payreimbursesheet.RequiredMoney)
                        {
                            database.Rollback();
                            return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：超过此付款单可付金额！可付金额为：" + (base_materielpurchase.TotalAmount - base_materielpurchase.GetedAmount).ToString() }.ToString());
                        }
                        base_materielpurchase.GetedAmount += by_payreimbursesheet.RequiredMoney;
                        base_materielpurchase.Modify(base_materielpurchase.MaterielPurchaseId);
                        DataFactory.Database().Update<Base_MaterielPurchase>(base_materielpurchase, isOpenTrans);
                        by_payreimbursesheet.NotGetMoney = base_materielpurchase.TotalAmount - base_materielpurchase.GetedAmount;

                        by_payreimbursesheet.CustomerName = base_materielpurchase.CustomerName;
                        by_payreimbursesheet.CustomerCode = base_materielpurchase.CustomerCode;
                        by_payreimbursesheet.CustomerId = base_materielpurchase.CustomerId;
                        by_payreimbursesheet.WorkerCode = base_materielpurchase.WorkerCode;
                        by_payreimbursesheet.WorkerName = base_materielpurchase.WorkerName;
                        by_payreimbursesheet.WorkerId = base_materielpurchase.WorkerId;
                        by_payreimbursesheet.DepartmentName = base_materielpurchase.DepartmentName;
                        by_payreimbursesheet.Remark1 = base_materielpurchase.Remark;
                    }
                    else if (by_payreimbursesheet.CodeType == "钢材申请单")
                    {

                        if (base_steelpurchase.TotalAmount - base_steelpurchase.GetedAmount < by_payreimbursesheet.RequiredMoney)
                        {
                            database.Rollback();
                            return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：超过此付款单可付金额！可付金额为：" + (base_steelpurchase.TotalAmount - base_steelpurchase.GetedAmount).ToString() }.ToString());
                        }
                        base_steelpurchase.GetedAmount += by_payreimbursesheet.RequiredMoney;
                        base_steelpurchase.Modify(base_steelpurchase.SteelPurchaseId);
                        DataFactory.Database().Update<Base_SteelPurchase>(base_steelpurchase, isOpenTrans);
                        by_payreimbursesheet.NotGetMoney = base_steelpurchase.TotalAmount - base_steelpurchase.GetedAmount;

                        by_payreimbursesheet.CustomerName = base_steelpurchase.CustomerName;
                        by_payreimbursesheet.CustomerCode = base_steelpurchase.CustomerCode;
                        by_payreimbursesheet.CustomerId = base_steelpurchase.CustomerId;
                        by_payreimbursesheet.WorkerCode = base_steelpurchase.WorkerCode;
                        by_payreimbursesheet.WorkerName = base_steelpurchase.WorkerName;
                        by_payreimbursesheet.WorkerId = base_steelpurchase.WorkerId;
                        by_payreimbursesheet.DepartmentName = base_steelpurchase.DepartmentName;
                        by_payreimbursesheet.Remark1 = base_steelpurchase.Remark;
                    }
                    else if (by_payreimbursesheet.CodeType == "供应商付款")
                    {
                        if (base_paycompany.TotalAmount - base_paycompany.GetedAmount < by_payreimbursesheet.RequiredMoney)
                        {
                            database.Rollback();
                            return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：超过此付款单可付金额！可付金额为：" + (base_paycompany.TotalAmount - base_paycompany.GetedAmount).ToString() }.ToString());
                        }
                        base_paycompany.GetedAmount += by_payreimbursesheet.RequiredMoney;
                        base_paycompany.Modify(base_paycompany.PayCompanyId);
                        DataFactory.Database().Update<Base_PayCompany>(base_paycompany, isOpenTrans);
                        by_payreimbursesheet.NotGetMoney = base_paycompany.TotalAmount - base_paycompany.GetedAmount;
                        by_payreimbursesheet.CustomerName = base_paycompany.CustomerName;
                        by_payreimbursesheet.CustomerCode = base_paycompany.CustomerCode;
                        by_payreimbursesheet.CustomerId = base_paycompany.CustomerId;
                        by_payreimbursesheet.WorkerCode = base_paycompany.WorkerCode;
                        by_payreimbursesheet.WorkerName = base_paycompany.WorkerName;
                        by_payreimbursesheet.WorkerId = base_paycompany.WorkerId;
                        by_payreimbursesheet.DepartmentName = base_paycompany.DepartmentName;
                        by_payreimbursesheet.Remark1 = base_paycompany.Remark + base_paycompany.PayName;
                    }
                    else if (by_payreimbursesheet.CodeType == "热处理委外")
                    {
                        if (by_quotationheatbeout.TotalAmount - by_quotationheatbeout.GetedAmount < by_payreimbursesheet.RequiredMoney)
                        {
                            database.Rollback();
                            return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：超过此付款单可付金额！可付金额为：" + (by_quotationheatbeout.TotalAmount - by_quotationheatbeout.GetedAmount).ToString() }.ToString());
                        }
                        by_quotationheatbeout.GetedAmount += by_payreimbursesheet.RequiredMoney;
                        by_quotationheatbeout.Modify(by_quotationheatbeout.QuotationHeatBeoutId);
                        DataFactory.Database().Update<BY_QuotationHeatBeout>(by_quotationheatbeout, isOpenTrans);
                        by_payreimbursesheet.NotGetMoney = by_quotationheatbeout.TotalAmount - by_quotationheatbeout.GetedAmount;
                        by_payreimbursesheet.CustomerName = by_quotationheatbeout.CustomerName;
                        by_payreimbursesheet.CustomerCode = by_quotationheatbeout.CustomerCode;
                        by_payreimbursesheet.CustomerId = by_quotationheatbeout.CustomerId;
                        by_payreimbursesheet.WorkerCode = by_quotationheatbeout.WorkerCode;
                        by_payreimbursesheet.WorkerName = by_quotationheatbeout.WorkerName;
                        by_payreimbursesheet.WorkerId = by_quotationheatbeout.WorkerId;
                        by_payreimbursesheet.DepartmentName = by_quotationheatbeout.DepartmentName;
                        by_payreimbursesheet.Remark1 = by_quotationheatbeout.Remark;
                    }
                    database.Insert(by_payreimbursesheet, isOpenTrans);
                    if (CanUse == 0)
                    {
                        break;
                    }
                }


                List<Base_SteelDDBeout> Base_SteelDDBeouts = new List<Base_SteelDDBeout>();
                List<Base_SteelDDPerchase> Base_SteelDDPerchases = new List<Base_SteelDDPerchase>();
                List<Base_SteelPurchase> Base_SteelPurchases = new List<Base_SteelPurchase>();
                List<Base_MaterielPurchase> Base_MaterielPurchases = new List<Base_MaterielPurchase>();
                List<BY_QuotationHeatBeout> BY_QuotationHeatBeouts = new List<BY_QuotationHeatBeout>();
                List<Base_PayCompany> Base_PayCompanys = new List<Base_PayCompany>();


                foreach (BY_PayCheckSheel itmes in by_paychecksheeladd)
                {
                    Base_SteelDDBeouts.AddRange(DataFactory.Database().FindList<Base_SteelDDBeout>("PayCheckSheetCode", itmes.PayCheckSheetCode));
                    Base_SteelDDPerchases.AddRange(DataFactory.Database().FindList<Base_SteelDDPerchase>("PayCheckSheetCode", itmes.PayCheckSheetCode));
                    Base_SteelPurchases.AddRange(DataFactory.Database().FindList<Base_SteelPurchase>("PayCheckSheetCode", itmes.PayCheckSheetCode));
                    Base_MaterielPurchases.AddRange(DataFactory.Database().FindList<Base_MaterielPurchase>("PayCheckSheetCode", itmes.PayCheckSheetCode));
                    BY_QuotationHeatBeouts.AddRange(DataFactory.Database().FindList<BY_QuotationHeatBeout>("PayCheckSheetCode", itmes.PayCheckSheetCode));
                    Base_PayCompanys.AddRange(DataFactory.Database().FindList<Base_PayCompany>("PayCheckSheetCode", itmes.PayCheckSheetCode));
                }
                foreach (Base_SteelDDBeout item in Base_SteelDDBeouts)
                {
                    item.GetedAmount = item.TotalAmount;
                    item.Modify(item.Base_SteelDDBeoutId);
                    DataFactory.Database().Update<Base_SteelDDBeout>(item, isOpenTrans);
                }
                foreach (Base_SteelDDPerchase item in Base_SteelDDPerchases)
                {
                    item.GetedAmount = item.TotalAmount;
                    item.Modify(item.Base_SteelDDPerchaseId);
                    DataFactory.Database().Update<Base_SteelDDPerchase>(item, isOpenTrans);
                }
                foreach (Base_SteelPurchase item in Base_SteelPurchases)
                {
                    item.GetedAmount = item.TotalAmount;
                    item.Modify(item.SteelPurchaseId);
                    DataFactory.Database().Update<Base_SteelPurchase>(item, isOpenTrans);
                }
                foreach (Base_MaterielPurchase item in Base_MaterielPurchases)
                {
                    item.GetedAmount = item.TotalAmount;
                    item.Modify(item.MaterielPurchaseId);
                    DataFactory.Database().Update<Base_MaterielPurchase>(item, isOpenTrans);
                }
                foreach (BY_QuotationHeatBeout item in BY_QuotationHeatBeouts)
                {
                    item.GetedAmount = item.TotalAmount;
                    item.Modify(item.QuotationHeatBeoutId);
                    DataFactory.Database().Update<BY_QuotationHeatBeout>(item, isOpenTrans);
                }
                foreach (Base_PayCompany item in Base_PayCompanys)
                {
                    item.GetedAmount = item.TotalAmount;
                    item.Modify(item.PayCompanyId);
                    DataFactory.Database().Update<Base_PayCompany>(item, isOpenTrans);
                }
                //by_reimbursesheets = new BY_ReimburseSheetBll().GetInventoryCode(by_inventorysaddN);

                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        [HttpPost]
        [ManagerPermission(PermissionMode.Enforce)]
        public override ActionResult Delete(string KeyValue, string ParentId)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                BY_PayReimburseSheet by_payreimbursesheet = database.FindEntity<BY_PayReimburseSheet>(KeyValue);
                if (by_payreimbursesheet.Opions == "应付款")
                {
                    if (!string.IsNullOrEmpty(by_payreimbursesheet.PayReimburseSheetCode))
                    {
                        BY_PayReimburseSheet YFentity = repositoryfactory.Repository().FindEntity(by_payreimbursesheet.PayReimburseSheetCode);//获取没更新之前实体对象
                        if (YFentity.PayReimburseSheetID != null)
                        {
                            YFentity.CanUseMoney = YFentity.CanUseMoney + by_payreimbursesheet.RequiredMoney - by_payreimbursesheet.ZRMoney;
                            if (YFentity.CanUseMoney < 0)
                            {
                                database.Rollback();
                                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：关联的预付款不够！" }.ToString());
                            }
                            YFentity.Modify(YFentity.PayReimburseSheetID);
                            database.Update(YFentity, isOpenTrans);
                        }
                    }


                    if (by_payreimbursesheet.CodeType == "付款对账单")
                    {
                        BY_PayCheckSheel by_paychecksheel = database.FindEntity<BY_PayCheckSheel>("PayCheckSheetCode", by_payreimbursesheet.Code);
                        if (by_paychecksheel.PayCheckSheetId != null)
                        {
                            by_paychecksheel.GetedAmount = by_paychecksheel.GetedAmount - by_payreimbursesheet.RequiredMoney;
                            by_paychecksheel.Modify(by_paychecksheel.PayCheckSheetId);
                            database.Update(by_paychecksheel, isOpenTrans);

                            List<Base_SteelDDBeout> Base_SteelDDBeouts = DataFactory.Database().FindList<Base_SteelDDBeout>("PayCheckSheetCode", by_paychecksheel.PayCheckSheetCode);
                            foreach (Base_SteelDDBeout item in Base_SteelDDBeouts)
                            {
                                item.GetedAmount = 0;
                                item.Modify(item.Base_SteelDDBeoutId);
                                DataFactory.Database().Update<Base_SteelDDBeout>(item, isOpenTrans);
                            }
                            List<Base_SteelDDPerchase> Base_SteelDDPerchases = DataFactory.Database().FindList<Base_SteelDDPerchase>("PayCheckSheetCode", by_paychecksheel.PayCheckSheetCode);
                            foreach (Base_SteelDDPerchase item in Base_SteelDDPerchases)
                            {
                                item.GetedAmount = 0;
                                item.Modify(item.Base_SteelDDPerchaseId);
                                DataFactory.Database().Update<Base_SteelDDPerchase>(item, isOpenTrans);
                            }

                            List<Base_SteelPurchase> Base_SteelPurchases = DataFactory.Database().FindList<Base_SteelPurchase>("PayCheckSheetCode", by_paychecksheel.PayCheckSheetCode);
                            foreach (Base_SteelPurchase item in Base_SteelPurchases)
                            {
                                item.GetedAmount = 0;
                                item.Modify(item.SteelPurchaseId);
                                DataFactory.Database().Update<Base_SteelPurchase>(item, isOpenTrans);
                            }

                            List<Base_MaterielPurchase> Base_MaterielPurchases = DataFactory.Database().FindList<Base_MaterielPurchase>("PayCheckSheetCode", by_paychecksheel.PayCheckSheetCode);
                            foreach (Base_MaterielPurchase item in Base_MaterielPurchases)
                            {
                                item.GetedAmount = 0;
                                item.Modify(item.MaterielPurchaseId);
                                DataFactory.Database().Update<Base_MaterielPurchase>(item, isOpenTrans);
                            }

                            List<BY_QuotationHeatBeout> BY_QuotationHeatBeout = DataFactory.Database().FindList<BY_QuotationHeatBeout>("PayCheckSheetCode", by_paychecksheel.PayCheckSheetCode);
                            foreach (BY_QuotationHeatBeout item in BY_QuotationHeatBeout)
                            {
                                item.GetedAmount = 0;
                                item.Modify(item.QuotationHeatBeoutId);
                                DataFactory.Database().Update<BY_QuotationHeatBeout>(item, isOpenTrans);
                            }

                            List<Base_PayCompany> Base_PayCompany = DataFactory.Database().FindList<Base_PayCompany>("PayCheckSheetCode", by_paychecksheel.PayCheckSheetCode);
                            foreach (Base_PayCompany item in Base_PayCompany)
                            {
                                item.GetedAmount = 0;
                                item.Modify(item.PayCompanyId);
                                DataFactory.Database().Update<Base_PayCompany>(item, isOpenTrans);
                            }
                        }
                    }

                    if (by_payreimbursesheet.CodeType == "委外采购")
                    {
                        Base_SteelDDPerchase base_steelddperchase = database.FindEntity<Base_SteelDDPerchase>("Base_SteelDDPerchaseCode", by_payreimbursesheet.Code);
                        if (base_steelddperchase.Base_SteelDDPerchaseId != null)
                        {
                            base_steelddperchase.GetedAmount = base_steelddperchase.GetedAmount - by_payreimbursesheet.RequiredMoney;
                            base_steelddperchase.Modify(base_steelddperchase.Base_SteelDDPerchaseId);
                            database.Update(base_steelddperchase, isOpenTrans);
                        }
                    }
                    if (by_payreimbursesheet.CodeType == "委外加工")
                    {
                        Base_SteelDDBeout base_steelddbeout = database.FindEntity<Base_SteelDDBeout>("Base_SteelDDBeoutCode", by_payreimbursesheet.Code);
                        if (base_steelddbeout.Base_SteelDDBeoutId != null)
                        {
                            base_steelddbeout.GetedAmount = base_steelddbeout.GetedAmount - by_payreimbursesheet.RequiredMoney;
                            base_steelddbeout.Modify(base_steelddbeout.Base_SteelDDBeoutId);
                            database.Update(base_steelddbeout, isOpenTrans);
                        }
                    }


                    if (by_payreimbursesheet.CodeType == "内部费用")
                    {
                        Base_PaySheet base_paysheet = database.FindEntity<Base_PaySheet>("PaySheetCode", by_payreimbursesheet.Code);
                        if (base_paysheet.PaySheetId != null)
                        {
                            base_paysheet.GetedAmount = base_paysheet.GetedAmount - by_payreimbursesheet.RequiredMoney;
                            base_paysheet.Modify(base_paysheet.PaySheetId);
                            database.Update(base_paysheet, isOpenTrans);
                        }
                    }
                    if (by_payreimbursesheet.CodeType == "物料申请单")
                    {
                        Base_MaterielPurchase base_materielpurchase = database.FindEntity<Base_MaterielPurchase>("MaterielPurchaseCode", by_payreimbursesheet.Code);
                        if (base_materielpurchase.MaterielPurchaseId != null)
                        {
                            base_materielpurchase.GetedAmount = base_materielpurchase.GetedAmount - by_payreimbursesheet.RequiredMoney;
                            base_materielpurchase.Modify(base_materielpurchase.MaterielPurchaseId);
                            database.Update(base_materielpurchase, isOpenTrans);
                        }
                    }
                    if (by_payreimbursesheet.CodeType == "钢材申请单")
                    {
                        Base_SteelPurchase base_steelpurchase = database.FindEntity<Base_SteelPurchase>("SteelPurchaseCode", by_payreimbursesheet.Code);
                        if (base_steelpurchase.SteelPurchaseId != null)
                        {
                            base_steelpurchase.GetedAmount = base_steelpurchase.GetedAmount - by_payreimbursesheet.RequiredMoney;
                            base_steelpurchase.Modify(base_steelpurchase.SteelPurchaseId);
                            database.Update(base_steelpurchase, isOpenTrans);
                        }
                    }
                    if (by_payreimbursesheet.CodeType == "供应商付款")
                    {
                        Base_PayCompany base_paycompany = database.FindEntity<Base_PayCompany>("PayCompanyCode", by_payreimbursesheet.Code);
                        if (base_paycompany.PayCompanyId != null)
                        {
                            base_paycompany.GetedAmount = base_paycompany.GetedAmount - by_payreimbursesheet.RequiredMoney;
                            base_paycompany.Modify(base_paycompany.PayCompanyId);
                            database.Update(base_paycompany, isOpenTrans);
                        }
                    }
                    if (by_payreimbursesheet.CodeType == "热处理委外")
                    {
                        BY_QuotationHeatBeout by_quotationheatbeout = database.FindEntity<BY_QuotationHeatBeout>("QuotationHeatBeoutCode", by_payreimbursesheet.Code);
                        if (by_quotationheatbeout.QuotationHeatBeoutId != null)
                        {
                            by_quotationheatbeout.GetedAmount = by_quotationheatbeout.GetedAmount - by_payreimbursesheet.RequiredMoney;
                            by_quotationheatbeout.Modify(by_quotationheatbeout.QuotationHeatBeoutId);
                            database.Update(by_quotationheatbeout, isOpenTrans);
                        }
                    }
                }
                if (by_payreimbursesheet.Opions == "预付款")
                {
                    if (by_payreimbursesheet.RequiredMoney != by_payreimbursesheet.CanUseMoney)
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：预付款已经使用或部分使用，请联系管理员！" }.ToString());
                    }
                }
                if (by_payreimbursesheet.Opions == "退款")
                {
                    if (!string.IsNullOrEmpty(by_payreimbursesheet.PayReimburseSheetCode))
                    {
                        BY_PayReimburseSheet YFentity = repositoryfactory.Repository().FindEntity(by_payreimbursesheet.PayReimburseSheetCode);//获取没更新之前实体对象
                        if (YFentity.PayReimburseSheetID != null)
                        {
                            YFentity.CanUseMoney = YFentity.CanUseMoney - by_payreimbursesheet.RequiredMoney;
                            if (YFentity.CanUseMoney < 0)
                            {
                                database.Rollback();
                                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：关联的预付款不够！" }.ToString());
                            }
                            YFentity.Modify(YFentity.PayReimburseSheetID);
                            database.Update(YFentity, isOpenTrans);
                        }
                    }
                }



                repositoryfactory.Repository().Delete(by_payreimbursesheet, isOpenTrans);
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功！" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult Audit(string KeyValue)
        {
            try
            {
                int IsOk = 0;
                BY_PayReimburseSheet BY_PayReimburseSheet = DataFactory.Database().FindEntity<BY_PayReimburseSheet>(KeyValue);
                BY_PayReimburseSheet.Status = BY_PayReimburseSheet.Status == 1 ? 0 : 1;
                BY_PayReimburseSheet.Modify(KeyValue);
                IsOk = DataFactory.Database().Update(BY_PayReimburseSheet);

                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "操作成功！" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
    }
}