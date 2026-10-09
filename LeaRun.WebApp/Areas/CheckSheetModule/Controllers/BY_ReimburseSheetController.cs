using DNS.Business;
using DNS.Cache;
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
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.CheckSheetModule.Controllers
{
    /// <summary>
    /// 费用申请单控制器
    /// </summary>
    public class BY_ReimburseSheetController : PublicController<BY_ReimburseSheet>
    {
        [LoginAuthorize]
        public JsonResult GridPageJson11(string ParameterJson, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                List<BY_ReimburseSheet> ListData = new List<BY_ReimburseSheet>();
                if (!string.IsNullOrEmpty(ParameterJson))
                {
                    List<DbParameter> parameter = new List<DbParameter>();
                    IList conditions = ParameterJson.JonsToList<Condition>();
                    string sql = @"
SELECT  [ReimburseSheetID]
      ,[ReimburseSheetCode]
      --,[Code]
      ,[CodeType]
      ,[RequiredMan]
      ,[RequiredDate]
      ,[RequiredMoney]
      ,[AuditMoney]
      ,[AuditMan]
      ,[AuditDateTime]
      ,[AuditRemark]
      ,[Status]
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
      ,[CustomerId]
      ,[CustomerCode]
      ,[CustomerName]
      ,[MnemonicCode]
      ,[QuotationCode]
      ,[ZRMoney]
      ,[GETMoney]
      ,[NotGetMoney]
      ,[GetType]
      ,[Opions]
      ,[CanUseMoney]
      ,[TotalAmount]
      --,[InvoiceNum]
      ,[BankNum]
      ,[Type3]
      ,[Type2]
      ,[Type1]
      ,[ZRRemark]
      ,[WorkerId]
      ,[WorkerName]
      ,[WorkerCode]
      ,[DepartmentName]
      ,[Remark1]
      ,[Type0]
      ,[LType]
    ,ISNULL(STUFF(( SELECT ',' + c.Code FROM BY_ReimburseSheet c WHERE c.ReimburseSheetCode = p.ReimburseSheetID ORDER BY c.ReimburseSheetID FOR XML PATH( '' ), TYPE
    ). value ( '.' , 'nvarchar(max)' ), 1 , 1 , '' ), Code) as  'Code'
    FROM BY_ReimburseSheet p 
 WHERE  1=1 ";
                    string WhereSql = ConditionBuilder.GetWhereSql(conditions, out parameter);
//                    if (ManageProvider.Provider.Current().ObjectId.IndexOf("9d04b362-c9fe-4c67-95a5-d24d6ad9177d") < 0 && ManageProvider.Provider.Current().UserName != "超级管理员")
//                    {
//                        WhereSql += @"  AND CreateUserName NOT IN (SELECT  b.RealName
//FROM    Base_ObjectUserRelation o ,
//        Base_User b
//WHERE   ObjectId = '9d04b362-c9fe-4c67-95a5-d24d6ad9177d'
//        AND o.UserId = b.UserId) ";
//                    }
                    ListData = repositoryfactory.Repository().FindListPageBySql(sql + WhereSql, parameter.ToArray(), ref jqgridparam);
                }
                else
                {
                    string WhereSql = "";
//                    if (ManageProvider.Provider.Current().ObjectId.IndexOf("9d04b362-c9fe-4c67-95a5-d24d6ad9177d") < 0 && ManageProvider.Provider.Current().UserName != "超级管理员")
//                    {
//                        WhereSql += @"  AND CreateUserName NOT IN (SELECT  b.RealName
//FROM    Base_ObjectUserRelation o ,
//        Base_User b
//WHERE   ObjectId = '9d04b362-c9fe-4c67-95a5-d24d6ad9177d'
//        AND o.UserId = b.UserId) ";
//                    }
                    ListData = repositoryfactory.Repository().FindListPage(WhereSql, ref jqgridparam);
                }


                foreach (var item in ListData)
                {
                    if (item.Code != null)
                    {
                        foreach (var code in item.Code.Split(','))
                        {
                            By_InvoiceForDetail by_invoicefordetail = DataFactory.Database().FindEntity<By_InvoiceForDetail>("CheckSheetCode", code);
                            item.InvoiceNum += by_invoicefordetail.InvoiceNum + ",";
                        }
                        item.InvoiceNum = item.InvoiceNum.Trim(',');
                    }
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



        public ActionResult GetSZ()
        {
            List<Base_DataDictionaryDetail> base_datadictionarydetailsS1 = DataFactory.Database().FindListBySql<Base_DataDictionaryDetail>(" SELECT * FROM dbo.Base_DataDictionaryDetail WHERE DataDictionaryId='3e3af419-d017-43fd-8cfc-3145685f9838' AND ParentId='0'");
            List<Base_DataDictionaryDetail> base_datadictionarydetailsJ2 = DataFactory.Database().FindListBySql<Base_DataDictionaryDetail>(" SELECT * FROM dbo.Base_DataDictionaryDetail WHERE DataDictionaryId='aaae1665-c9c7-425c-9ffe-4ab4cf715d5c' AND ParentId='0'");

            var JsonData = new
            {
                base_datadictionarydetailsS1 = base_datadictionarydetailsS1.Where(p=> p.FullName != "营业收入" && p.FullName != "投资款"),
                base_datadictionarydetailsJ2 = base_datadictionarydetailsJ2.Where(p => p.FullName != "原料采购" )
            };
            return Json(JsonData, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PrintDataJsonBank(string Starttime, string Endtime)
        {
            try
            {
                List<BY_ReimburseSheet> Datas = DataFactory.Database().FindListBySql<BY_ReimburseSheet>(string.Format(@"   
SELECT  A.Type1 ,
        A.Type2 ,
        A.Remark ,
        A.RequiredDate ,
        A.CustomerName ,
        A.BankNum ,
        B.GETMoney1 ,
        B.TypeCOUNT1 ,
        C.GETMoney2 ,
        C.TypeCOUNT2
FROM    ( SELECT    *
          FROM      BY_ReimburseSheet R
          WHERE     ( ReimburseSheetCode IS NULL
                      OR ReimburseSheetCode = ''
                      OR Opions = '退款'
                    )
                    AND RequiredDate > '{0}'
                    AND RequiredDate < '{1}'
        ) A
        LEFT JOIN ( SELECT  Type1 ,
                            SUM(GETMoney) GETMoney1 ,
                            COUNT(1) TypeCOUNT1
                    FROM    BY_ReimburseSheet T
                    WHERE   ( ReimburseSheetCode IS NULL
                              OR ReimburseSheetCode = ''
                              OR Opions = '退款'
                            )
                            AND RequiredDate > '{0}'
                            AND RequiredDate < '{1}'
                    GROUP BY T.Type1
                  ) B ON A.Type1 = B.Type1
        LEFT JOIN ( SELECT  Type2 ,
                            SUM(GETMoney) GETMoney2 ,
                            COUNT(1) TypeCOUNT2
                    FROM    BY_ReimburseSheet T
                    WHERE   ( ReimburseSheetCode IS NULL
                              OR ReimburseSheetCode = ''
                              OR Opions = '退款'
                            )
                            AND RequiredDate > '{0}'
                            AND RequiredDate < '{1}'
                    GROUP BY T.Type2
                  ) C ON A.Type2 = C.Type2
ORDER BY A.Type1 ,
        A.Type2", Starttime, Endtime));

                var JsonData = new
                {
                    Datas = Datas,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception)
            {
                return null;
            }
        }


        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult ChangeBankNum(string BankNum1,string Remark,decimal RequiredMoney,string RequiredDate,string GetType,string BankNum)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                BY_ReimburseSheet by_reimbursesheet = new BY_ReimburseSheet();
                by_reimbursesheet.Create();
                BY_PayReimburseSheet by_payreimbursesheet = new BY_PayReimburseSheet();
                by_payreimbursesheet.Create();

                by_reimbursesheet.AuditMan = DNS.Utilities.ManageProvider.Provider.Current().UserName;
                by_reimbursesheet.Code = BankNum;
                by_reimbursesheet.Status = "1";
                by_reimbursesheet.RequiredMan = ManageProvider.Provider.Current().UserName;
                by_reimbursesheet.RequiredDate = Convert.ToDateTime(RequiredDate);
                by_reimbursesheet.Opions = "内部转账";
                by_reimbursesheet.GetType = GetType;
                by_reimbursesheet.RequiredMoney = RequiredMoney;
                by_reimbursesheet.GETMoney = RequiredMoney;
                by_reimbursesheet.CanUseMoney = 0;
                by_reimbursesheet.TotalAmount = 0;
                by_reimbursesheet.Remark = Remark;
                by_reimbursesheet.NotGetMoney = 0;
                by_reimbursesheet.BankNum = BankNum;
                database.Insert(by_reimbursesheet, isOpenTrans);

                by_payreimbursesheet.AuditMan = DNS.Utilities.ManageProvider.Provider.Current().UserName;
                by_payreimbursesheet.Code = BankNum1;
                by_payreimbursesheet.Status = 1;
                by_payreimbursesheet.RequiredMan = ManageProvider.Provider.Current().UserName;
                by_payreimbursesheet.RequiredDate = Convert.ToDateTime(RequiredDate);
                by_payreimbursesheet.Opions = "内部转账";
                by_payreimbursesheet.GetType = GetType;
                by_payreimbursesheet.RequiredMoney = RequiredMoney;
                by_payreimbursesheet.GETMoney = RequiredMoney;
                by_payreimbursesheet.CanUseMoney = 0;
                by_payreimbursesheet.TotalAmount = 0;
                by_payreimbursesheet.Remark = Remark;
                by_payreimbursesheet.NotGetMoney = 0;
                by_payreimbursesheet.BankNum = BankNum1;
                database.Insert(by_payreimbursesheet, isOpenTrans);

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
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult Audit(string KeyValue)
        {
            try
            {
                int IsOk = 0;
                BY_ReimburseSheet BY_ReimburseSheet = DataFactory.Database().FindEntity<BY_ReimburseSheet>(KeyValue);
                BY_ReimburseSheet.Status = BY_ReimburseSheet.Status == "1" ? "0" : "1";
                BY_ReimburseSheet.Modify(KeyValue);
                IsOk = DataFactory.Database().Update(BY_ReimburseSheet);

                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "操作成功！" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }


        public void DownLoadExcel(string CustomerName, string CreateDate_StartTime, string CreateDate_EndTime)
        {
            string sql = string.Format(@"
        SELECT  RequiredDate AS '日期' ,
        InvoiceNum AS '发票号/送货单号' ,
        0 '发票金额/送货单金额' ,
        RequiredMoney AS '收款金额',
        GetType AS '类型',
        0 AS '余额'
FROM    BY_ReimburseSheet WHERE 1=1 ", CustomerName, CreateDate_StartTime, CreateDate_EndTime);

            if (!string.IsNullOrEmpty(CustomerName))
            {
                sql += " AND CustomerName='" + CustomerName + "' ";
            }
            if (!string.IsNullOrEmpty(CreateDate_StartTime))
            {
                sql += "AND RequiredDate > '" + CreateDate_StartTime + "' ";
            }
            if (!string.IsNullOrEmpty(CreateDate_EndTime))
            {
                sql += " AND RequiredDate < '" + CreateDate_EndTime + "' ";
            }
            DataTable ListData = DataFactory.Database().FindTableBySql(sql);
            string[] DataColumn = { "日期", "发票号/送货单号", "发票金额/送货单金额", "收款金额", "类型", "余额" };
            string fileName = CustomerName + "往来明细" + CreateDate_StartTime + "-" + CreateDate_EndTime;
            DeriveExcel.DataTableToExcel(ListData, DataColumn, fileName);
        }

        public class LWtemp
        {
            public string ReimburseSheetID { get; set; }
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
            public decimal PayMoney { get; set; }
            public decimal QTPayMoney { get; set; }
            
        }

        public ActionResult GridPageJsonLW(string CustomerCode, string CreateDate_StartTime, string CreateDate_EndTime, string ReimburseSheetCode, string audit,string LType, JqGridParam jqgridparam)
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
                    SUM(PayMoney) PayMoney ,
                    SUM(QTPayMoney) QTPayMoney ,
                    '' GetType ,
                    0 NoGetMoney ,
                    '' Remark ,
                   SUM(ZRMoney) ZRMoney,'' BankNum
          FROM      ( SELECT    RequiredDate AS BuildDate ,
                                InvoiceNum AS Code ,
                                0 TotalAmount ,
                                GetMoney ,
                                0 PayMoney,
                                0 QTPayMoney ,
                                GetType AS GetType ,
                                0 AS NoGetMoney ,
                               ISNULL(Remark1,'') + ',' + ISNULL(Remark,'') Remark ,
                                0 ZRMoney
                      FROM      BY_ReimburseSheet
                      WHERE     1 = 1
                                AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') 
                                AND CustomerCode = @CustomerCode
                                AND RequiredDate < @StartTime
                                and GetMoney > 0
                                @LType
                      UNION ALL
                      SELECT    RequiredDate AS BuildDate ,
                                InvoiceNum AS Code ,
                                0 TotalAmount ,
                                0 GetMoney ,
                                -GetMoney PayMoney,
                                0 QTPayMoney ,
                                GetType AS GetType ,
                                0 AS NoGetMoney ,
                                ISNULL(Remark1,'') + ',' + ISNULL(Remark,'') Remark ,
                                0 ZRMoney
                      FROM      BY_ReimburseSheet
                      WHERE     1 = 1
                                AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') 
                                AND CustomerCode = @CustomerCode
                                AND RequiredDate < @StartTime
                                and GetMoney <= 0
                                @LType
                      
                      UNION ALL
                      SELECT    CreateDate AS BuildDate ,
                                DeliveryCode AS Code ,
                                TotalAmount TotalAmount ,
                                0 AS GetMoney ,
                                0 PayMoney,
                                0 QTPayMoney ,
                                '' AS GetType ,
                                0 AS NoGetMoney ,
                                Remark ,
                                (SELECT  SUM(ISNULL(s.ZRMoney, 0))
                                FROM    BY_ReimburseSheet s
                                WHERE   s.Code = BY_Delivery.DeliveryCode) ZRMoney
                      FROM      dbo.BY_Delivery
                      WHERE     ( CheckSheetCode IS  NULL
                                  OR CheckSheetCode = ''
                                )
                                AND 
                                Enabled > 0 @audit  and DeleteMark = 0 
                                AND CustomerCode = @CustomerCode
                                AND CreateDate < @StartTime
                                @LType
                      UNION ALL
                      SELECT    CreateDate AS BuildDate ,
                                CheckSheetCode AS Code ,
                                TotalAmount,-- TotalAmount TotalAmount ,
                                0 AS GetMoney ,
                                0 PayMoney,
                                0 QTPayMoney ,
                                '' AS GetType ,
                                0 AS NoGetMoney ,
                                Remark ,
                                (SELECT  SUM(ISNULL(s.ZRMoney, 0))
                                FROM    BY_ReimburseSheet s
                                WHERE   s.Code = CheckSheetCode) ZRMoney
                      FROM      dbo.BY_CheckSheet
                      WHERE     1 = 1
                                AND CustomerCode = @CustomerCode
                                AND CAST (YearMonth + '-1' AS DATETIME) < @StartTime
                                @LType
                     
                    ) A
          UNION ALL
          SELECT    RequiredDate AS BuildDate ,
                    InvoiceNum AS Code ,
                    0 TotalAmount ,
                    GetMoney ,
                    0 PayMoney,
                    0 QTPayMoney ,
                    GetType AS GetType ,
                    0 AS NoGetMoney ,
                    ISNULL(Remark1,'') + ',' + ISNULL(Remark,'') Remark ,
                    0 ZRMoney, BankNum
          FROM      BY_ReimburseSheet
          WHERE     1 = 1
                    AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') 
                    AND CustomerCode = @CustomerCode
                    AND RequiredDate > @StartTime
                    AND RequiredDate < @EndTime
                    and GetMoney > 0
                    @LType
          UNION ALL
          SELECT    RequiredDate AS BuildDate ,
                    InvoiceNum AS Code ,
                    0 TotalAmount ,
                    0 GetMoney ,
                    -GetMoney PayMoney,
                    0 QTPayMoney ,
                    GetType AS GetType ,
                    0 AS NoGetMoney ,
                    ISNULL(Remark1,'') + ',' + ISNULL(Remark,'') Remark ,
                    0 ZRMoney, BankNum
          FROM      BY_ReimburseSheet
          WHERE     1 = 1
                    AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') 
                    AND CustomerCode = @CustomerCode
                    AND RequiredDate > @StartTime
                    AND RequiredDate < @EndTime
                    and GetMoney <= 0
                    @LType
          UNION ALL

          SELECT    CreateDate AS BuildDate ,
                    DeliveryCode AS Code ,
                    TotalAmount TotalAmount ,
                    0 AS GetMoney ,
                    0 PayMoney,
                    0 QTPayMoney ,
                    '' AS GetType ,
                    0 AS NoGetMoney ,
                    Remark ,
                    (SELECT  SUM(ISNULL(s.ZRMoney, 0))
                    FROM    BY_ReimburseSheet s
                    WHERE   s.Code =BY_Delivery.DeliveryCode) ZRMoney,'' BankNum
          FROM      dbo.BY_Delivery
          WHERE     ( CheckSheetCode IS  NULL
                      OR CheckSheetCode = ''
                    )
                    AND 
                    Enabled > 0 @audit   and DeleteMark = 0 
                    AND CustomerCode = @CustomerCode
                    AND CreateDate > @StartTime
                    AND CreateDate < @EndTime
                    @LType
          UNION ALL
          SELECT    CAST (YearMonth + '-1' AS DATETIME) AS  BuildDate ,
                    CheckSheetCode AS Code ,
                    TotalAmount ,--TotalAmount TotalAmount ,
                    0 AS GetMoney ,
                    0 PayMoney,
                    0 QTPayMoney ,
                    '' AS GetType ,
                    0 AS NoGetMoney ,
                    '对账金额：'+CAST(TotalAmount AS VARCHAR(50))+','+Remark Remark ,
                    (SELECT  SUM(ISNULL(s.ZRMoney, 0))
FROM    BY_ReimburseSheet s
WHERE   s.Code = CheckSheetCode) ZRMoney,'' BankNum
          FROM      dbo.BY_CheckSheet
          WHERE     1 = 1
                    AND CustomerCode = @CustomerCode
                    AND CAST (YearMonth + '-1' AS DATETIME) >= @StartTime
                    AND CAST (YearMonth + '-1' AS DATETIME) <= @EndTime   
                    @LType
  
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

                if (!string.IsNullOrEmpty(ReimburseSheetCode))
                {
                    sql = string.Format(@"SELECT  RequiredDate AS BuildDate ,
        InvoiceNum AS Code ,
        0 TotalAmount ,
        RequiredMoney  GetMoney ,
        GetType AS GetType ,
        0 AS NoGetMoney ,
        Remark 
FROM    BY_ReimburseSheet
WHERE  ReimburseSheetCode = '{0}'", ReimburseSheetCode);
                }

                if (audit == "1")
                {
                    sql = sql.Replace("@audit", " and AuditUserId != '' and AuditUserId is not null ");
                }
                else
                {
                    sql = sql.Replace("@audit", "");

                }

                sql = sql.Replace("@CustomerCode", string.IsNullOrEmpty(CustomerCode) ? "" : "'" + CustomerCode + "'");
                sql = sql.Replace("@StartTime", string.IsNullOrEmpty(CreateDate_StartTime) ? "" : "'" + CreateDate_StartTime + "'");
                sql = sql.Replace("@EndTime", string.IsNullOrEmpty(CreateDate_EndTime) ? "" : "'" + CreateDate_EndTime + "'");
                sql = sql.Replace("@LType", string.IsNullOrEmpty(LType) ? "" : " and LType ='" + LType + "'");


                    

                List<LWtemp> lwtemps = DataFactory.Database().FindListBySql<LWtemp>(sql);
                decimal temp = 0;
                foreach (var item in lwtemps)
                {
                    temp = temp + item.TotalAmount - item.GetMoney - item.ZRMoney + item.PayMoney;
                    item.NoGetMoney = temp;
                }
                LWtemp lwtemp = new LWtemp();
                lwtemp.Remark = "合计";
                lwtemp.TotalAmount = lwtemps.Sum(p => p.TotalAmount);
                lwtemp.GetMoney = lwtemps.Sum(p => p.GetMoney);
                lwtemp.PayMoney = lwtemps.Sum(p => p.PayMoney);
                lwtemp.QTPayMoney = lwtemps.Sum(p => p.QTPayMoney);
                lwtemp.NoGetMoney = temp;
                lwtemp.ZRMoney = lwtemps.Sum(p => p.ZRMoney);
                lwtemps.Add(lwtemp);
                Stopwatch watch = CommonHelper.TimerStart();
                Base_Customer Base_Customer = DataFactory.Database().FindEntity<Base_Customer>("Code", CustomerCode);


                DateTime starttime = Convert.ToDateTime(CreateDate_StartTime);
                DateTime endtime = Convert.ToDateTime(CreateDate_EndTime).AddDays(1);
                jqgridparam.page = 1;
                jqgridparam.rows = 999;
                DataTable dt1 = new BY_CheckSheetBll().SalesummaryList(starttime.ToShortDateString(), endtime.ToShortDateString(), CustomerCode, "", "", ref jqgridparam);

                //Base_Company company = DataFactory.Database().FindEntity<Base_Company>("FullName", string.IsNullOrEmpty(by_quotation.CompanyName) ? "" : by_quotation.CompanyName);

                var JsonData = new
                {
                    base_customer = Base_Customer,
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = lwtemps,
                    dt1 = dt1,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception)
            {
                return null;
            }
        }

        public class Banktemp
        {
            public string Banktype { get; set; }
            public DateTime BuildDate { get; set; }
            public string perantID { get; set; }
            public string Remark { get; set; }
            public string Code { get; set; }
            public string InvoiceNum { get; set; }
            public string GetType { get; set; }
            public string Opions { get; set; }
            public decimal GetMoney { get; set; }
            public decimal paymoney { get; set; }
            public string BankNum { get; set; }
            public decimal NowMoney { get; set; }
            public string CustomerName { get; set; }
            public string Type1 { get; set; }
            public string Type2 { get; set; }
            public string WorkerName { get; set; }

            public decimal totalamount { get; set; }

        }


        public ActionResult GridBankList(string BankNum, string CreateDate_EndTime, JqGridParam jqgridparam)
        {
            try
            {
                List<DbParameter> parameter = new List<DbParameter>();
                string sql = string.Format(@"
     SELECT d.FullName ,
        ISNULL(( SELECT    SUM(ISNULL(GetMoney, 0))
          FROM      BY_ReimburseSheet r
          WHERE     r.BankNum = d.FullName  @EndTime
                    AND ( r.ReimburseSheetCode IS NULL
                          OR r.ReimburseSheetCode = ''
                          OR r.Opions = '退款'
                        )
        ),0)+d.SortCode GETMONEY ,
        ( SELECT    SUM(ISNULL(GetMoney, 0))
          FROM      BY_PayReimburseSheet r
          WHERE     r.BankNum = d.FullName @EndTime
                    AND ( r.PayReimburseSheetCode IS NULL
                          OR r.PayReimburseSheetCode = ''
                          OR r.Opions = '退款'
                        )
        ) USEMONEY ,
        ISNULL(( SELECT SUM(ISNULL(GetMoney, 0))
                 FROM   BY_ReimburseSheet r
                 WHERE  r.BankNum = d.FullName @EndTime
                        AND ( r.ReimburseSheetCode IS NULL
                              OR r.ReimburseSheetCode = ''
                              OR r.Opions = '退款'
                            )
               ), 0)+d.SortCode - ISNULL(( SELECT  SUM(ISNULL(GetMoney, 0))
                                FROM    BY_PayReimburseSheet r
                                WHERE   r.BankNum = d.FullName @EndTime
                                        AND ( r.PayReimburseSheetCode IS NULL
                                              OR r.PayReimburseSheetCode = ''
                                              OR r.Opions = '退款'
                                            )
                              ), 0) nowmoney
 FROM   Base_DataDictionaryDetail d
 WHERE  1 = 1
        AND DataDictionaryId = '6885bc06-ee98-4136-bf83-6e6ce9e2ee48' 

         ");
                if (!string.IsNullOrEmpty(BankNum))
                {
                    sql += " and d.FullName = @BankNum";
                    parameter.Add(DbFactory.CreateDbParameter("@BankNum", BankNum));
                }
                if (!string.IsNullOrEmpty(CreateDate_EndTime))
                {
                    sql = sql.Replace("@EndTime", " and  RequiredDate <= @CreateDate_EndTime");
                    parameter.Add(DbFactory.CreateDbParameter("@CreateDate_EndTime", CreateDate_EndTime));
                }
                else
                {
                    sql = sql.Replace("@EndTime", " ");
                }

                DataTable dt = DataFactory.Database().FindTableBySql(sql, parameter.ToArray());
                Stopwatch watch = CommonHelper.TimerStart();
                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = dt,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult GridPageJsonBank(string BankNum, string CustomerName, string CreateDate_StartTime, string CreateDate_EndTime, string InvoiceNum, JqGridParam jqgridparam)
        {
            try
            {
                List<DbParameter> parameter = new List<DbParameter>();
                string sql = string.Format(@"
       SELECT
	*
FROM (SELECT
	'起初' Banktype,
	'1999-01-01' AS BuildDate,
	NULL perantID,
	'起初金额' Remark,
	'' Code,
	NULL InvoiceNum,
	'现金' GetType,
	'' Opions,
	SortCode+ISNULL(( SELECT SUM(ISNULL(GetMoney, 0))
                 FROM   BY_ReimburseSheet r
                 WHERE  r.BankNum = d.FullName
                        AND ( r.ReimburseSheetCode IS NULL
                              OR r.ReimburseSheetCode = ''
                              OR r.Opions = '退款'
                            ) and r.RequiredDate < @StartTime
               ), 0) - ISNULL(( SELECT  SUM(ISNULL(GetMoney, 0))
                                FROM    BY_PayReimburseSheet r
                                WHERE   r.BankNum = d.FullName
                                        AND ( r.PayReimburseSheetCode IS NULL
                                              OR r.PayReimburseSheetCode = ''
                                              OR r.Opions = '退款'
                                            ) and r.RequiredDate < @StartTime
                              ), 0) GetMoney,0 paymoney,'' BankNum,'' Type1, '' Type2,'' CustomerName,'' WorkerName
FROM Base_DataDictionaryDetail d
WHERE 1=1 and DataDictionaryId ='6885bc06-ee98-4136-bf83-6e6ce9e2ee48' @BankNum1 UNION ALL SELECT
	*
FROM (SELECT
	Opions Banktype,
	RequiredDate AS BuildDate,
	ReimburseSheetCode perantID,
	CASE WHEN Remark IS NULL THEN ''
             ELSE Remark
        END + CASE WHEN Remark1 IS NULL THEN ''
                   ELSE Remark1
              END AS Remark ,
	Code,
	InvoiceNum,
	GetType,
	Opions,
	GetMoney,0 paymoney,BankNum ,Type1,Type2,CustomerName,WorkerName
FROM BY_ReimburseSheet where 1=1 @CustomerName UNION ALL SELECT
	Opions Banktype,
	RequiredDate AS BuildDate,
	PayReimburseSheetCode perantID,
	CASE WHEN Remark IS NULL THEN ''
             ELSE Remark
        END + CASE WHEN Remark1 IS NULL THEN ''
                   ELSE Remark1
              END AS Remark ,
	Code,
	InvoiceNum,
	GetType,
	Opions,
    0 GetMoney,GetMoney paymoney,BankNum ,Type1,Type2,CustomerName,WorkerName
FROM BY_PayReimburseSheet where 1=1 @CustomerName)
a
WHERE BuildDate > @StartTime AND BuildDate < @EndTime AND (perantID IS NULL OR perantID = '' or Opions='退款')  @BankNum2)
b
ORDER BY b.BuildDate ASC

         ");
                if (string.IsNullOrEmpty(CreateDate_StartTime))
                {
                    CreateDate_StartTime = "2000-1-1";
                }
                if (string.IsNullOrEmpty(CreateDate_EndTime))
                {
                    CreateDate_EndTime = "2999-1-1";
                }




                sql = sql.Replace("@BankNum1", string.IsNullOrEmpty(BankNum) ? "" : " and FullName = '" + BankNum + "'");
                sql = sql.Replace("@BankNum2", string.IsNullOrEmpty(BankNum) ? "" : " and BankNum = '" + BankNum + "'");
                sql = sql.Replace("@CustomerName", string.IsNullOrEmpty(CustomerName) ? "" : " and CustomerName = '" + CustomerName + "'");


                parameter.Add(DbFactory.CreateDbParameter("@StartTime", string.IsNullOrEmpty(CreateDate_StartTime) ? "" : CreateDate_StartTime));
                parameter.Add(DbFactory.CreateDbParameter("@EndTime", string.IsNullOrEmpty(CreateDate_EndTime) ? "" : CreateDate_EndTime));
                List<Banktemp> banktemps = DataFactory.Database().FindListBySql<Banktemp>(sql, parameter.ToArray());
                decimal temp = 0;
                foreach (var item in banktemps)
                {
                    temp = temp + item.GetMoney - item.paymoney;
                    item.NowMoney = temp;
                }
                Banktemp banktemp = new Banktemp();
                banktemp.Remark = "合计";
                banktemp.GetMoney = banktemps.Sum(p => p.GetMoney);
                banktemp.paymoney = banktemps.Sum(p => p.paymoney);
                banktemp.NowMoney = temp;
                banktemps.Add(banktemp);
                Stopwatch watch = CommonHelper.TimerStart();
                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = banktemps,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult GridPageJsonBank1(string BankNum, string CustomerName, string CreateDate_StartTime, string CreateDate_EndTime, string InvoiceNum,string Type1, JqGridParam jqgridparam)
        {
            try
            {
                if (Type1 == "固定资产投入")
                {
                    string sql = @"SELECT
	'' Banktype,
	CreateDate AS BuildDate,
	'' perantID,
	CASE WHEN Remark IS NULL THEN ''
             ELSE Remark
        END  Remark ,
	Code,
	'' InvoiceNum,
	'' GetType,
	'' Opions,
	0 GetMoney,GetedAmount paymoney,'' BankNum ,Type1,Type2,CustomerName,WorkerName,totalamount
FROM Base_PayCompany 
  where Type1 = '固定资产投入'";
                    List<Banktemp> banktemps = DataFactory.Database().FindListBySql<Banktemp>(sql);
                    Stopwatch watch = CommonHelper.TimerStart();
                    var JsonData = new
                    {
                        total = jqgridparam.total,
                        page = jqgridparam.page,
                        records = jqgridparam.records,
                        costtime = CommonHelper.TimerEnd(watch),
                        rows = banktemps,
                    };
                    return Content(JsonData.ToJson());

                }
                else
                {
                    List<DbParameter> parameter = new List<DbParameter>();
                    string sql = string.Format(@"
   SELECT
	*
FROM (SELECT
	Opions Banktype,
	RequiredDate AS BuildDate,
	ReimburseSheetCode perantID,
	CASE WHEN Remark IS NULL THEN ''
             ELSE Remark
        END + CASE WHEN Remark1 IS NULL THEN ''
                   ELSE Remark1
              END AS Remark ,
	Code,
	InvoiceNum,
	GetType,
	Opions,
	GetMoney,0 paymoney,BankNum ,Type1,Type2,CustomerName,WorkerName,totalamount
FROM BY_ReimburseSheet where 1=1 @CustomerName @Type1 UNION ALL SELECT
	Opions Banktype,
	RequiredDate AS BuildDate,
	PayReimburseSheetCode perantID,
	CASE WHEN Remark IS NULL THEN ''
             ELSE Remark
        END + CASE WHEN Remark1 IS NULL THEN ''
                   ELSE Remark1
              END AS Remark ,
	Code,
	InvoiceNum,
	GetType,
	Opions,
    0 GetMoney,GetMoney paymoney,BankNum ,Type1,Type2,CustomerName,WorkerName,totalamount
FROM BY_PayReimburseSheet where 1=1 @CustomerName @Type1)
a
WHERE BuildDate > @StartTime AND BuildDate < @EndTime AND (perantID IS NULL OR perantID = '' or Opions='退款')  @BankNum2 ORDER BY A.BuildDate ASC


         ");
                    if (string.IsNullOrEmpty(CreateDate_StartTime))
                    {
                        CreateDate_StartTime = "2000-1-1";
                    }
                    if (string.IsNullOrEmpty(CreateDate_EndTime))
                    {
                        CreateDate_EndTime = "2999-1-1";
                    }




                    sql = sql.Replace("@BankNum1", string.IsNullOrEmpty(BankNum) ? "" : " and FullName = '" + BankNum + "'");
                    sql = sql.Replace("@BankNum2", string.IsNullOrEmpty(BankNum) ? "" : " and BankNum = '" + BankNum + "'");
                    sql = sql.Replace("@CustomerName", string.IsNullOrEmpty(CustomerName) ? "" : " and CustomerName = '" + CustomerName + "'");

                    sql = sql.Replace("@Type1", string.IsNullOrEmpty(Type1) ? "" : " and Type1 = '" + Type1 + "'");



                    parameter.Add(DbFactory.CreateDbParameter("@StartTime", string.IsNullOrEmpty(CreateDate_StartTime) ? "" : CreateDate_StartTime));
                    parameter.Add(DbFactory.CreateDbParameter("@EndTime", string.IsNullOrEmpty(CreateDate_EndTime) ? "" : CreateDate_EndTime));

                    List<Banktemp> banktemps = DataFactory.Database().FindListBySql<Banktemp>(sql, parameter.ToArray());
                    decimal temp = 0;
                    foreach (var item in banktemps)
                    {
                        temp = temp + item.GetMoney - item.paymoney;
                        item.NowMoney = temp;
                    }
                    Banktemp banktemp = new Banktemp();
                    banktemp.Remark = "合计";
                    banktemp.GetMoney = banktemps.Sum(p => p.GetMoney);
                    banktemp.paymoney = banktemps.Sum(p => p.paymoney);
                    banktemp.NowMoney = temp;
                    banktemps.Add(banktemp);
                    Stopwatch watch = CommonHelper.TimerStart();
                    var JsonData = new
                    {
                        total = jqgridparam.total,
                        page = jqgridparam.page,
                        records = jqgridparam.records,
                        costtime = CommonHelper.TimerEnd(watch),
                        rows = banktemps,
                    };
                    return Content(JsonData.ToJson());
                }


            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult GridPageJsonBankforto(string StartTime, string EndTime, string type, string type1, JqGridParam jqgridparam)
        {
            try
            {
                //if (string.IsNullOrEmpty(type))
                //{
                //    return null;
                //}
                List<DbParameter> parameter = new List<DbParameter>();
                string ss = "";
                string[] sss = type.Split(',');
                for (int i = 0; i < sss.Length; i++)
                {
                    ss += "'" + sss[i] + "',";
                }
                ss = ss.Trim(',');

                string zz = "";
                string[] zzz = type1.Split(',');
                for (int i = 0; i < zzz.Length; i++)
                {
                    zz += "'" + zzz[i] + "',";
                }
                zz = zz.Trim(',');

                string sql = @"
   SELECT
	*
FROM (";
                //               if (type.IndexOf("材料销售")>=0)
                //{

                sql += @" SELECT    '材料销售利润' Banktype ,
                    '1999-01-01' AS BuildDate ,
                    NULL perantID ,
                    '销售利润' Remark ,
                    '' Code ,
                    NULL InvoiceNum ,
                    '' GetType ,
                    '' Opions ,
                    SUM(de.SalesAmount - ( de.WorkCost + de.MaterialCost
                                           + de.HeatCost )) GetMoney ,
                    0 paymoney ,
                    '' BankNum ,
                    '' Type1 ,
                    '' Type2 ,
                    '' CustomerName ,
                    '' WorkerName
           FROM      dbo.BY_DeliveryDetail de,BY_Delivery d
          WHERE     de.DeliveryId=d.DeliveryId and    d.CreateDate > @StartTime AND d.CreateDate < @EndTime and d.LType ='GC'   and d.DeleteMark = 0 and  d.Enabled = 1
UNION ALL ";
                //}
                //if (type.IndexOf("热处理销售") >= 0)
                //{

                sql += @" SELECT    '热处理销售利润' Banktype ,
                    '1999-01-01' AS BuildDate ,
                    NULL perantID ,
                    '销售利润' Remark ,
                    '' Code ,
                    NULL InvoiceNum ,
                    '' GetType ,
                    '' Opions ,
                    SUM(de.SalesAmount - ( de.WorkCost + de.MaterialCost
                                           + de.HeatCost )) GetMoney ,
                    0 paymoney ,
                    '' BankNum ,
                    '' Type1 ,
                    '' Type2 ,
                    '' CustomerName ,
                    '' WorkerName
           FROM      dbo.BY_DeliveryDetail de,BY_Delivery d
          WHERE     de.DeliveryId=d.DeliveryId and    d.CreateDate > @StartTime AND d.CreateDate < @EndTime and d.LType ='RCL'  and d.DeleteMark = 0  and  d.Enabled = 1
UNION ALL ";
                //}
                //if (type.IndexOf("固定资产损益") >= 0)
                //{
                sql += @"

SELECT    '固定资产损益' Banktype ,
                    '1999-01-01' AS BuildDate ,
                    NULL perantID ,
                    '固定资产损益' Remark ,
                    '' Code ,
                    NULL InvoiceNum ,
                    '' GetType ,
                    '' Opions ,
                    0 GetMoney ,
 (GetMoney)/60 paymoney ,
                    '' BankNum ,
                    '固定资产投入' Type1 ,
                    '' Type2 ,
                    '' CustomerName ,
                    '' WorkerName
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款') and RequiredDate >  @StartTime AND RequiredDate < @EndTime      and Type1 = '固定资产投入' and DATEADD(month, 60, RequiredDate) > SYSDATETIME()

UNION ALL ";
                //}

                sql += @" SELECT  '' Banktype ,
        '' BuildDate ,
        '' perantID ,
        '' Remark ,
        '' Code ,
        '' InvoiceNum ,
        '' GetType ,
        '' Opions ,
        SUM(GetMoney) GetMoney,
        0 paymoney ,
        '' BankNum ,
        Type1 ,
        '' Type2 ,
        '' CustomerName ,
        '' WorkerName
FROM    BY_ReimburseSheet
WHERE   1 = 1 AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') and RequiredDate > @StartTime AND RequiredDate < @EndTime and Type1 != '营业收入'  @ss 
GROUP BY Type1 
UNION ALL 
SELECT  '' Banktype ,
        '' BuildDate ,
        '' perantID ,
        '' Remark ,
        '' Code ,
        '' InvoiceNum ,
        '' GetType ,
        '' Opions ,
        0 GetMoney ,
        SUM(GetMoney) paymoney ,
        '' BankNum ,
        Type1 ,
        '' Type2 ,
        '' CustomerName ,
        '' WorkerName
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款') and RequiredDate > @StartTime AND RequiredDate < @EndTime   and Type1 != '固定资产投入'  and Type1 != '原料采购' @zz 
GROUP BY Type1   )

b
ORDER BY b.BuildDate desc


         "; ;
                if (string.IsNullOrEmpty(StartTime))
                {
                    StartTime = "2000-1-1";
                }
                if (string.IsNullOrEmpty(EndTime))
                {
                    EndTime = "2999-1-1";
                }
                ss = ""; zz = "";
                sql = sql.Replace("@ss", string.IsNullOrEmpty(ss) ? "" : " and type1 in ( " + ss + ") ");

                sql = sql.Replace("@zz", string.IsNullOrEmpty(zz) ? "" : " and type1 in (" + zz + ") ");


                parameter.Add(DbFactory.CreateDbParameter("@StartTime", string.IsNullOrEmpty(StartTime) ? "" : StartTime));
                parameter.Add(DbFactory.CreateDbParameter("@EndTime", string.IsNullOrEmpty(EndTime) ? "" : EndTime));
                List<Banktemp> banktemps = DataFactory.Database().FindListBySql<Banktemp>(sql, parameter.ToArray());
                decimal temp = 0;
                foreach (var item in banktemps)
                {
                    temp = temp + item.GetMoney - item.paymoney;
                    item.NowMoney = temp;
                }
                Banktemp banktemp = new Banktemp();
                banktemp.Remark = "合计";
                banktemp.GetMoney = banktemps.Sum(p => p.GetMoney);
                banktemp.paymoney = banktemps.Sum(p => p.paymoney);
                banktemp.NowMoney = temp;
                banktemps.Add(banktemp);
                Stopwatch watch = CommonHelper.TimerStart();
                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = banktemps,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ActionResult GridPageJsonBankforto2(string year, string type, string type1, JqGridParam jqgridparam)
        {
            type = "";
            type1 = "";
            try
            {
                if (string.IsNullOrEmpty(year))
                {
                    return null;
                }
                string keyss = "GridPageJsonBankforto2" + DateTime.Now.Date.ToShortTimeString();
                DataTable dt = new DataTable();
                object dtobject = CacheHelper.Get(keyss);
                //if (dtobject == null)
                if (true)
                {
                    #region
                    string sql = @"
   
select sum(GetMoney) GetMoney,sum(paymoney) paymoney,yearmonth 
FROM (SELECT    
                    SUM(de.SalesAmount - ( de.WorkCost + de.MaterialCost
                                           + de.HeatCost )) GetMoney , 0 paymoney,
                    '材料销售利润' Type1 ,
                   '@year-1' yearmonth
           FROM      dbo.BY_DeliveryDetail de,BY_Delivery d
          WHERE     de.DeliveryId=d.DeliveryId and    d.CreateDate > '@year-1-1' AND d.CreateDate < '@year-2-1' and d.LType ='GC'   and d.DeleteMark = 0 and  d.Enabled = 1

UNION ALL  SELECT  
        SUM(GetMoney) GetMoney,0 paymoney,
        Type1 ,
       '@year-1' yearmonth
FROM    BY_ReimburseSheet
WHERE   1 = 1 AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-1-1' AND RequiredDate < '@year-2-1'    and  Type1 != '营业收入'
GROUP BY Type1 
UNION ALL 
SELECT  
       0 GetMoney, -SUM(GetMoney) paymoney ,
       Type1,
       '@year-1' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-1-1' AND RequiredDate < '@year-2-1'   and Type1 != '原料采购'  and Type1 != '固定资产投入'
GROUP BY Type1  
UNION ALL 
SELECT  
      0 GetMoney,  -SUM(GetMoney)/60 paymoney ,
       Type1,
       '@year-1' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-1-1' AND RequiredDate < '@year-2-1'      and Type1 = '固定资产投入' and DATEADD(month, 60, RequiredDate) > SYSDATETIME()
GROUP BY Type1
) b
group by yearmonth

UNION ALL

select sum(GetMoney) GetMoney,sum(paymoney) paymoney,yearmonth 
FROM (SELECT    
                    SUM(de.SalesAmount - ( de.WorkCost + de.MaterialCost
                                           + de.HeatCost )) GetMoney , 0 paymoney,
                     '材料销售利润' Type1 ,
                   '@year-2' yearmonth
           FROM      dbo.BY_DeliveryDetail de,BY_Delivery d
          WHERE     de.DeliveryId=d.DeliveryId and    d.CreateDate > '@year-2-1' AND d.CreateDate < '@year-3-1' and d.LType ='GC'   and d.DeleteMark = 0 and  d.Enabled = 1

UNION ALL  SELECT  
        SUM(GetMoney) GetMoney,0 paymoney,
        Type1 ,
       '@year-2' yearmonth
FROM    BY_ReimburseSheet
WHERE   1 = 1 AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-2-1' AND RequiredDate < '@year-3-1'    and  Type1 != '营业收入'
GROUP BY Type1 
UNION ALL 
SELECT  
      0 GetMoney,  -SUM(GetMoney) paymoney ,
       Type1,
       '@year-2' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-2-1' AND RequiredDate < '@year-3-1'      and Type1 != '原料采购'   and Type1 != '固定资产投入'
GROUP BY Type1  
UNION ALL 
SELECT  
      0 GetMoney,  -SUM(GetMoney)/60 paymoney ,
       Type1,
       '@year-2' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-2-1' AND RequiredDate < '@year-3-1'     and Type1 = '固定资产投入' and DATEADD(month, 60, RequiredDate) > SYSDATETIME()
GROUP BY Type1
) b
group by yearmonth

UNION ALL

select sum(GetMoney) GetMoney,sum(paymoney) paymoney,yearmonth 
FROM (SELECT    
                    SUM(de.SalesAmount - ( de.WorkCost + de.MaterialCost
                                           + de.HeatCost )) GetMoney , 0 paymoney,
                     '材料销售利润' Type1 ,
                   '@year-3' yearmonth
           FROM      dbo.BY_DeliveryDetail de,BY_Delivery d
          WHERE     de.DeliveryId=d.DeliveryId and    d.CreateDate > '@year-3-1' AND d.CreateDate < '@year-4-1' and d.LType ='GC'   and d.DeleteMark = 0 and  d.Enabled = 1

UNION ALL  SELECT  
        SUM(GetMoney) GetMoney,0 paymoney,
        Type1 ,
       '@year-3' yearmonth
FROM    BY_ReimburseSheet
WHERE   1 = 1 AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-3-1' AND RequiredDate < '@year-4-1'    and  Type1 != '营业收入'
GROUP BY Type1 
UNION ALL 
SELECT  
      0 GetMoney,  -SUM(GetMoney) paymoney ,
       Type1,
       '@year-3' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-3-1' AND RequiredDate < '@year-4-1'      and Type1 != '原料采购'   and Type1 != '固定资产投入'
GROUP BY Type1  
UNION ALL 
SELECT  
      0 GetMoney,  -SUM(GetMoney)/60 paymoney ,
       Type1,
       '@year-3' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款')  and RequiredDate > '@year-3-1' AND RequiredDate < '@year-4-1'      and Type1 = '固定资产投入' and DATEADD(month, 60, RequiredDate) > SYSDATETIME()
GROUP BY Type1
) b
group by yearmonth

UNION ALL
select sum(GetMoney) GetMoney,sum(paymoney) paymoney,yearmonth 
FROM (SELECT    
                    SUM(de.SalesAmount - ( de.WorkCost + de.MaterialCost
                                           + de.HeatCost )) GetMoney , 0 paymoney,
                    '材料销售利润' Type1 ,
                   '@year-4' yearmonth
           FROM      dbo.BY_DeliveryDetail de,BY_Delivery d
          WHERE     de.DeliveryId=d.DeliveryId and    d.CreateDate > '@year-4-1' AND d.CreateDate < '@year-5-1' and d.LType ='GC'   and d.DeleteMark = 0 and  d.Enabled = 1

UNION ALL  SELECT  
        SUM(GetMoney) GetMoney,0 paymoney,
        Type1 ,
       '@year-4' yearmonth
FROM    BY_ReimburseSheet
WHERE   1 = 1 AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-4-1' AND RequiredDate < '@year-5-1'    and  Type1 != '营业收入'
GROUP BY Type1 
UNION ALL 
SELECT  
     0 GetMoney,   -SUM(GetMoney) paymoney ,
       Type1,
       '@year-4' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-4-1' AND RequiredDate < '@year-5-1'      and Type1 != '原料采购'   and Type1 != '固定资产投入'
GROUP BY Type1  
UNION ALL 
SELECT  
      0 GetMoney,  -SUM(GetMoney)/60 paymoney ,
       Type1,
       '@year-4' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款')  and RequiredDate > '@year-4-1' AND RequiredDate < '@year-5-1'     and Type1 = '固定资产投入' and DATEADD(month, 60, RequiredDate) > SYSDATETIME()
GROUP BY Type1
) b
group by yearmonth

UNION ALL

select sum(GetMoney) GetMoney,sum(paymoney) paymoney,yearmonth 
FROM (SELECT    
                    SUM(de.SalesAmount - ( de.WorkCost + de.MaterialCost
                                           + de.HeatCost )) GetMoney , 0 paymoney,
                    '材料销售利润' Type1 ,
                   '@year-5' yearmonth
           FROM      dbo.BY_DeliveryDetail de,BY_Delivery d
          WHERE     de.DeliveryId=d.DeliveryId and    d.CreateDate > '@year-5-1' AND d.CreateDate < '@year-6-1' and d.LType ='GC'   and d.DeleteMark = 0 and  d.Enabled = 1

UNION ALL  SELECT  
        SUM(GetMoney) GetMoney,0 paymoney,
        Type1 ,
       '@year-5' yearmonth
FROM    BY_ReimburseSheet
WHERE   1 = 1 AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-5-1' AND RequiredDate < '@year-6-1'    and  Type1 != '营业收入'
GROUP BY Type1 
UNION ALL 
SELECT  
      0 GetMoney,  -SUM(GetMoney) paymoney ,
       Type1,
       '@year-5' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-5-1' AND RequiredDate < '@year-6-1'      and Type1 != '原料采购'   and Type1 != '固定资产投入'
GROUP BY Type1  
UNION ALL 
SELECT  
      0 GetMoney,  -SUM(GetMoney)/60 paymoney ,
       Type1,
       '@year-5' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款')  and RequiredDate > '@year-5-1' AND RequiredDate < '@year-6-1'     and Type1 = '固定资产投入' and DATEADD(month, 60, RequiredDate) > SYSDATETIME()
GROUP BY Type1
) b
group by yearmonth

UNION ALL

select sum(GetMoney) GetMoney,sum(paymoney) paymoney,yearmonth 
FROM (SELECT    
                    SUM(de.SalesAmount - ( de.WorkCost + de.MaterialCost
                                           + de.HeatCost )) GetMoney , 0 paymoney,
                     '材料销售利润' Type1 ,
                   '@year-6' yearmonth
           FROM      dbo.BY_DeliveryDetail de,BY_Delivery d
          WHERE     de.DeliveryId=d.DeliveryId and    d.CreateDate > '@year-6-1' AND d.CreateDate < '@year-7-1' and d.LType ='GC'   and d.DeleteMark = 0 and  d.Enabled = 1

UNION ALL  SELECT  
        SUM(GetMoney) GetMoney,0 paymoney,
        Type1 ,
       '@year-6' yearmonth
FROM    BY_ReimburseSheet
WHERE   1 = 1 AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-6-1' AND RequiredDate < '@year-7-1'    and  Type1 != '营业收入'
GROUP BY Type1 
UNION ALL 
SELECT  
    0 GetMoney,    -SUM(GetMoney) paymoney ,
       Type1,
       '@year-6' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-6-1' AND RequiredDate < '@year-7-1'      and Type1 != '原料采购'   and Type1 != '固定资产投入'
GROUP BY Type1  
UNION ALL 
SELECT  
      0 GetMoney,  -SUM(GetMoney)/60 paymoney ,
       Type1,
       '@year-6' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款')  and RequiredDate > '@year-6-1' AND RequiredDate < '@year-7-1'        and Type1 = '固定资产投入' and DATEADD(month, 60, RequiredDate) > SYSDATETIME()
GROUP BY Type1
) b
group by yearmonth

UNION ALL
select sum(GetMoney) GetMoney,sum(paymoney) paymoney,yearmonth 
FROM (SELECT    
                    SUM(de.SalesAmount - ( de.WorkCost + de.MaterialCost
                                           + de.HeatCost )) GetMoney , 0 paymoney,
                '材料销售利润' Type1 ,
                   '@year-7' yearmonth
           FROM      dbo.BY_DeliveryDetail de,BY_Delivery d
          WHERE     de.DeliveryId=d.DeliveryId and    d.CreateDate > '@year-7-1' AND d.CreateDate < '@year-8-1' and d.LType ='GC'   and d.DeleteMark = 0 and  d.Enabled = 1

UNION ALL  SELECT  
        SUM(GetMoney) GetMoney,0 paymoney,
        Type1 ,
       '@year-7' yearmonth
FROM    BY_ReimburseSheet
WHERE   1 = 1 AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-7-1' AND RequiredDate < '@year-8-1'    and  Type1 != '营业收入'
GROUP BY Type1 
UNION ALL 
SELECT  
      0 GetMoney,  -SUM(GetMoney) paymoney ,
        Type1,
       '@year-7' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-7-1' AND RequiredDate < '@year-8-1'      and Type1 != '原料采购'   and Type1 != '固定资产投入'
GROUP BY Type1  
UNION ALL 
SELECT  
      0 GetMoney,  -SUM(GetMoney)/60 paymoney ,
       Type1,
       '@year-7' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-7-1' AND RequiredDate < '@year-8-1'     and Type1 = '固定资产投入' and DATEADD(month, 60, RequiredDate) > SYSDATETIME()
GROUP BY Type1
) b
group by yearmonth

UNION ALL
select sum(GetMoney) GetMoney,sum(paymoney) paymoney,yearmonth 
FROM (SELECT    
                    SUM(de.SalesAmount - ( de.WorkCost + de.MaterialCost
                                           + de.HeatCost )) GetMoney , 0 paymoney,
                    '材料销售利润' Type1 ,
                   '@year-8' yearmonth
           FROM      dbo.BY_DeliveryDetail de,BY_Delivery d
          WHERE     de.DeliveryId=d.DeliveryId and    d.CreateDate > '@year-8-1' AND d.CreateDate < '@year-9-1' and d.LType ='GC'   and d.DeleteMark = 0 and  d.Enabled = 1

UNION ALL  SELECT  
        SUM(GetMoney) GetMoney,0 paymoney,
        Type1 ,
       '@year-8' yearmonth
FROM    BY_ReimburseSheet
WHERE   1 = 1 AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-8-1' AND RequiredDate < '@year-9-1'    and  Type1 != '营业收入'
GROUP BY Type1 
UNION ALL 
SELECT  
      0 GetMoney,  -SUM(GetMoney) paymoney ,
      Type1,
       '@year-8' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-8-1' AND RequiredDate < '@year-9-1'      and Type1 != '原料采购'   and Type1 != '固定资产投入'
GROUP BY Type1  
UNION ALL 
SELECT  
      0 GetMoney,  -SUM(GetMoney)/60 paymoney ,
       Type1,
       '@year-8' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款')  and RequiredDate > '@year-8-1' AND RequiredDate < '@year-9-1'    and Type1 = '固定资产投入' and DATEADD(month, 60, RequiredDate) > SYSDATETIME()
GROUP BY Type1
) b
group by yearmonth

UNION ALL
select sum(GetMoney) GetMoney,sum(paymoney) paymoney,yearmonth 
FROM (SELECT    
                    SUM(de.SalesAmount - ( de.WorkCost + de.MaterialCost
                                           + de.HeatCost )) GetMoney , 0 paymoney,
                    '材料销售利润' Type1 ,
                   '@year-9' yearmonth
           FROM      dbo.BY_DeliveryDetail de,BY_Delivery d
          WHERE     de.DeliveryId=d.DeliveryId and    d.CreateDate > '@year-9-1' AND d.CreateDate < '@year-10-1' and d.LType ='GC'   and d.DeleteMark = 0 and  d.Enabled = 1

UNION ALL  SELECT  
        SUM(GetMoney) GetMoney,0 paymoney,
        Type1 ,
       '@year-9' yearmonth
FROM    BY_ReimburseSheet
WHERE   1 = 1 AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-9-1' AND RequiredDate < '@year-10-1'    and  Type1 != '营业收入'
GROUP BY Type1 
UNION ALL 
SELECT  
      0 GetMoney,  -SUM(GetMoney) paymoney ,
        Type1,
       '@year-9' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-9-1' AND RequiredDate < '@year-10-1'      and Type1 != '原料采购'   and Type1 != '固定资产投入'
GROUP BY Type1  
UNION ALL 
SELECT  
      0 GetMoney,  -SUM(GetMoney)/60 paymoney ,
       Type1,
       '@year-9' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款')  and RequiredDate > '@year-9-1' AND RequiredDate < '@year-10-1'       and Type1 = '固定资产投入' and DATEADD(month, 60, RequiredDate) > SYSDATETIME()
GROUP BY Type1
) b
group by yearmonth

UNION ALL
select sum(GetMoney) GetMoney,sum(paymoney) paymoney,yearmonth 
FROM (SELECT    
                    SUM(de.SalesAmount - ( de.WorkCost + de.MaterialCost
                                           + de.HeatCost )) GetMoney , 0 paymoney,
                  '材料销售利润' Type1 ,
                   '@year-10' yearmonth
           FROM      dbo.BY_DeliveryDetail de,BY_Delivery d
          WHERE     de.DeliveryId=d.DeliveryId and    d.CreateDate > '@year-10-1' AND d.CreateDate < '@year-11-1' and d.LType ='GC'   and d.DeleteMark = 0 and  d.Enabled = 1

UNION ALL  SELECT  
        SUM(GetMoney) GetMoney,0 paymoney,
        Type1 ,
       '@year-10' yearmonth
FROM    BY_ReimburseSheet
WHERE   1 = 1 AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-10-1' AND RequiredDate < '@year-11-1'    and  Type1 != '营业收入'
GROUP BY Type1 
UNION ALL 
SELECT  
    0 GetMoney,    -SUM(GetMoney) paymoney ,
        Type1,
       '@year-10' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-10-1' AND RequiredDate < '@year-11-1'      and Type1 != '原料采购'   and Type1 != '固定资产投入'
GROUP BY Type1  
UNION ALL 
SELECT  
      0 GetMoney,  -SUM(GetMoney)/60 paymoney ,
       Type1,
       '@year-10' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款')  and RequiredDate > '@year-10-1' AND RequiredDate < '@year-11-1'      and Type1 = '固定资产投入' and DATEADD(month, 60, RequiredDate) > SYSDATETIME()
GROUP BY Type1
) b
group by yearmonth

UNION ALL
select sum(GetMoney) GetMoney,sum(paymoney) paymoney,yearmonth 
FROM (SELECT    
                    SUM(de.SalesAmount - ( de.WorkCost + de.MaterialCost
                                           + de.HeatCost )) GetMoney , 0 paymoney,
                    '材料销售利润' Type1 ,
                   '@year-11' yearmonth
           FROM      dbo.BY_DeliveryDetail de,BY_Delivery d
          WHERE     de.DeliveryId=d.DeliveryId and    d.CreateDate > '@year-11-1' AND d.CreateDate < '@year-12-1' and d.LType ='GC'   and d.DeleteMark = 0 and  d.Enabled = 1

UNION ALL  SELECT  
        SUM(GetMoney) GetMoney,0 paymoney,
        Type1 ,
       '@year-11' yearmonth
FROM    BY_ReimburseSheet
WHERE   1 = 1 AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-11-1' AND RequiredDate < '@year-12-1'    and  Type1 != '营业收入'
GROUP BY Type1 
UNION ALL 
SELECT  
      0 GetMoney,  -SUM(GetMoney) paymoney ,
       Type1,
       '@year-11' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-11-1' AND RequiredDate < '@year-12-1'      and Type1 != '原料采购'   and Type1 != '固定资产投入'
GROUP BY Type1  
UNION ALL 
SELECT  
      0 GetMoney,  -SUM(GetMoney)/60 paymoney ,
       Type1,
       '@year-11' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款')  and RequiredDate > '@year-11-1' AND RequiredDate < '@year-12-1'     and Type1 = '固定资产投入' and DATEADD(month, 60, RequiredDate) > SYSDATETIME()
GROUP BY Type1
) b
group by yearmonth

UNION ALL
select sum(GetMoney) GetMoney,sum(paymoney) paymoney,yearmonth 

FROM (SELECT    
                    SUM(de.SalesAmount - ( de.WorkCost + de.MaterialCost
                                           + de.HeatCost )) GetMoney , 0 paymoney,
                    '材料销售利润' Type1 ,
                   '@year-12' yearmonth
           FROM      dbo.BY_DeliveryDetail de,BY_Delivery d
          WHERE     de.DeliveryId=d.DeliveryId and    d.CreateDate > '@year-12-1' AND d.CreateDate < '@year-12-30' and d.LType ='GC'   and d.DeleteMark = 0 and  d.Enabled = 1

UNION ALL  SELECT  
        SUM(GetMoney) GetMoney,0 paymoney,
        Type1 ,
       '@year-12' yearmonth
FROM    BY_ReimburseSheet
WHERE   1 = 1 AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-12-1' AND RequiredDate < '@year-12-30'    and  Type1 != '营业收入'
GROUP BY Type1 
UNION ALL 
SELECT  
     0 GetMoney,   -SUM(GetMoney) paymoney ,
        Type1,
       '@year-12' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款') and RequiredDate > '@year-12-1' AND RequiredDate < '@year-12-30'      and Type1 != '原料采购'   and Type1 != '固定资产投入'
GROUP BY Type1  
UNION ALL 
SELECT  
      0 GetMoney,  -SUM(GetMoney)/60 paymoney ,
       Type1,
       '@year-12' yearmonth
FROM    BY_PayReimburseSheet
WHERE   1 = 1 AND (PayReimburseSheetCode IS NULL OR PayReimburseSheetCode ='' or Opions='退款')  and RequiredDate > '@year-12-1' AND RequiredDate < '@year-12-30'      and Type1 = '固定资产投入' and DATEADD(month, 60, RequiredDate) > SYSDATETIME()
GROUP BY Type1
) b
group by yearmonth



         "; ;
                    #endregion
                    sql = sql.Replace("@year", year);
                    dt = DataFactory.Database().FindTableBySql(sql);
                    CacheHelper.Insert(keyss, dt);
                }
                else
                {
                    dt = (DataTable)dtobject;
                }


                Stopwatch watch = CommonHelper.TimerStart();
                var JsonData = new
                {
                    rows = dt,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult GridPageJsonYS(string CustomerCode, string CreateDate_StartTime, string CreateDate_EndTime, JqGridParam jqgridparam)
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
                    0 ZRMoney, BankNum
          FROM      BY_ReimburseSheet
          WHERE     1 = 1
                    AND (ReimburseSheetCode IS NULL OR ReimburseSheetCode ='' or Opions='退款') 
                    AND CustomerCode = @CustomerCode
                    AND RequiredDate > @StartTime
                    AND RequiredDate < @EndTime
                    and GetMoney < 0
          UNION ALL
          SELECT    CreateDate AS BuildDate ,
                    DeliveryCode AS Code ,
                    -TotalAmount TotalAmount ,
                    0 AS GetMoney ,
                    '' AS GetType ,
                    0 AS NoGetMoney ,
                    Remark ,
                    (SELECT  SUM(ISNULL(s.ZRMoney, 0))
FROM    BY_ReimburseSheet s
WHERE   s.Code =BY_Delivery.DeliveryCode) ZRMoney,'' BankNum
          FROM      dbo.BY_Delivery
          WHERE     ( CheckSheetCode IS  NULL
                      OR CheckSheetCode = ''
                    )
                    AND Enabled > 0   and DeleteMark = 0 
                    AND CustomerCode = @CustomerCode
                    AND CreateDate > @StartTime
                    AND CreateDate < @EndTime
                    AND TotalAmount < 0
          UNION ALL
          SELECT    CAST (YearMonth + '-1' AS DATETIME) AS  BuildDate ,
                    CheckSheetCode AS Code ,
                    -TotalAmount TotalAmount ,
                    0 AS GetMoney ,
                    '' AS GetType ,
                    0 AS NoGetMoney ,
                    Remark ,
                    (SELECT  SUM(ISNULL(s.ZRMoney, 0))
FROM    BY_ReimburseSheet s
WHERE   s.Code = CheckSheetCode) ZRMoney,'' BankNum
          FROM      dbo.BY_CheckSheet
          WHERE     1 = 1
                    AND CustomerCode = @CustomerCode
                    AND CreateDate > @StartTime
                    AND CreateDate < @EndTime
                    AND TotalAmount < 0
                    
          UNION ALL
          SELECT    CreateDate AS BuildDate ,
                    CheckCompanyCode AS Code ,
                    -TotalAmount TotalAmount ,
                    0 AS GetMoney ,
                    '' AS GetType ,
                    0 AS NoGetMoney ,
                    Remark ,
                    (SELECT  SUM(ISNULL(s.ZRMoney, 0))
FROM    BY_ReimburseSheet s
WHERE   s.Code = CheckCompanyCode) ZRMoney,'' BankNum
          FROM      BY_CheckCompany
          WHERE     CustomerCode = @CustomerCode
                    AND CreateDate > @StartTime
                    AND CreateDate < @EndTime
                    and TotalAmount < 0
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

                sql = sql.Replace("@CustomerCode", string.IsNullOrEmpty(CustomerCode) ? "" : "'" + CustomerCode + "'");
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

        [HttpPost]
        [ManagerPermission(PermissionMode.Enforce)]
        public override ActionResult Delete(string KeyValue, string ParentId)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                List<BY_ReimburseSheet> dreimbursesheetsforcheck = new List<BY_ReimburseSheet>();
                BY_ReimburseSheet by_reimbursesheet = database.FindEntity<BY_ReimburseSheet>(KeyValue);
                if (by_reimbursesheet.Opions == "应收款")
                {
                    if (by_reimbursesheet.Code.IndexOf("CS") >= 0)
                    {
                        BY_CheckSheet by_checksheet = database.FindEntity<BY_CheckSheet>("CheckSheetCode", by_reimbursesheet.Code);
                        if (by_checksheet.CheckSheetId != null)
                        {
                            if (by_checksheet.TotalAmount > by_checksheet.GetedAmount)
                            {
                                List<BY_Delivery> by_deliverys = DataFactory.Database().FindList<BY_Delivery>("CheckSheetCode", by_checksheet.CheckSheetCode);
                                foreach (BY_Delivery item in by_deliverys)
                                {
                                    List<BY_ReimburseSheet> dreimbursesheets = DataFactory.Database().FindList<BY_ReimburseSheet>("Code", item.DeliveryCode);
                                    dreimbursesheetsforcheck.AddRange(dreimbursesheets);
                                }
                            }
                        }

                    }

                    if (!string.IsNullOrEmpty(by_reimbursesheet.ReimburseSheetCode))
                    {
                        BY_ReimburseSheet YFentity = repositoryfactory.Repository().FindEntity(by_reimbursesheet.ReimburseSheetCode);//获取没更新之前实体对象
                        if (YFentity.ReimburseSheetID != null)
                        {
                            YFentity.CanUseMoney = YFentity.CanUseMoney + by_reimbursesheet.RequiredMoney - by_reimbursesheet.ZRMoney;
                            if (YFentity.CanUseMoney < 0)
                            {
                                database.Rollback();
                                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：关联的预付款不够！" }.ToString());
                            }
                            YFentity.Modify(YFentity.ReimburseSheetID);
                            database.Update(YFentity, isOpenTrans);
                        }
                    }

                    if (by_reimbursesheet.Code.IndexOf("CS") >= 0)
                    {
                        BY_CheckSheet by_checksheet = database.FindEntity<BY_CheckSheet>("CheckSheetCode", by_reimbursesheet.Code);
                        if (by_checksheet.CheckSheetId != null)
                        {
                            by_checksheet.GetedAmount = by_checksheet.GetedAmount - by_reimbursesheet.RequiredMoney;
                            by_checksheet.Modify(by_checksheet.CheckSheetId);
                            database.Update(by_checksheet, isOpenTrans);
                            if (by_checksheet.TotalAmount > by_checksheet.GetedAmount)
                            {
                                List<BY_Delivery> by_deliverys = DataFactory.Database().FindList<BY_Delivery>("CheckSheetCode", by_checksheet.CheckSheetCode);
                                foreach (BY_Delivery item in by_deliverys)
                                {
                                    List<BY_ReimburseSheet> dreimbursesheets = dreimbursesheetsforcheck.Where(p=>p.Code==item.DeliveryCode).ToList(); 
                                    item.GetedAmount = dreimbursesheets.Sum(p => p.RequiredMoney);
                                    item.Modify(item.DeliveryId);
                                    DataFactory.Database().Update<BY_Delivery>(item, isOpenTrans);
                                }
                            }
                        }

                    }
                    else if (by_reimbursesheet.Code.IndexOf("GC") >= 0 || by_reimbursesheet.Code.IndexOf("RC") >= 0 || by_reimbursesheet.Code.IndexOf("LJ") >= 0)
                    {
                        BY_Delivery by_delivery = database.FindEntity<BY_Delivery>("DeliveryCode", by_reimbursesheet.Code);
                        if (by_delivery.DeliveryId != null)
                        {
                            if (!string.IsNullOrEmpty(by_delivery.CheckSheetCode))
                            {
                                database.Rollback();
                                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：已对账单，请先删除对账单！" }.ToString());
                            }
                            by_delivery.GetedAmount = by_delivery.GetedAmount - by_reimbursesheet.RequiredMoney;
                            by_delivery.Modify(by_delivery.DeliveryId);
                            database.Update(by_delivery, isOpenTrans);
                        }
                    }
                    else if (by_reimbursesheet.Code.IndexOf("Y") >= 0)
                    {
                        BY_CheckCompany by_checkcompany = database.FindEntity<BY_CheckCompany>("CheckCompanyCode", by_reimbursesheet.Code);
                        if (by_checkcompany.CheckCompanyId != null)
                        {
                            by_checkcompany.GetedAmount = by_checkcompany.GetedAmount - by_reimbursesheet.RequiredMoney;
                            by_checkcompany.Modify(by_checkcompany.CheckCompanyId);
                            database.Update(by_checkcompany, isOpenTrans);
                        }
                    }
                    else if (by_reimbursesheet.Code.IndexOf("G") >= 0)
                    {
                        Base_Sheet base_sheet = database.FindEntity<Base_Sheet>("PaySheetCode", by_reimbursesheet.Code);
                        if (base_sheet.PaySheetId != null)
                        {
                            base_sheet.GetedAmount = base_sheet.GetedAmount - by_reimbursesheet.RequiredMoney;
                            base_sheet.Modify(base_sheet.PaySheetId);
                            database.Update(base_sheet, isOpenTrans);
                        }
                    }
                }
                if (by_reimbursesheet.Opions == "退款")
                {
                    if (!string.IsNullOrEmpty(by_reimbursesheet.ReimburseSheetCode))
                    {
                        BY_ReimburseSheet YFentity = repositoryfactory.Repository().FindEntity(by_reimbursesheet.ReimburseSheetCode);//获取没更新之前实体对象
                        if (YFentity.ReimburseSheetID != null)
                        {
                            YFentity.CanUseMoney = YFentity.CanUseMoney - by_reimbursesheet.RequiredMoney;
                            if (YFentity.CanUseMoney < 0)
                            {
                                database.Rollback();
                                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：关联的预付款不够！" }.ToString());
                            }
                            YFentity.Modify(YFentity.ReimburseSheetID);
                            database.Update(YFentity, isOpenTrans);
                        }
                    }
                }
                if (by_reimbursesheet.Opions == "预收款")
                {
                    if (by_reimbursesheet.RequiredMoney != by_reimbursesheet.CanUseMoney)
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：预收款已经使用或部分使用，请联系管理员！" }.ToString());
                    }
                }
                repositoryfactory.Repository().Delete(by_reimbursesheet, isOpenTrans);
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
        public override ActionResult SubmitForm(BY_ReimburseSheet entity, string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                BY_ReimburseSheet Oldentity = repositoryfactory.Repository().FindEntity(KeyValue);//获取没更新之前实体对象
                entity.RequiredMoney = entity.GETMoney + entity.ZRMoney;

                if (!string.IsNullOrEmpty(KeyValue) && entity.Opions == "应收款" && Oldentity.RequiredMoney != entity.RequiredMoney)
                {
                    if (!string.IsNullOrEmpty(Oldentity.ReimburseSheetCode))
                    {
                        BY_ReimburseSheet YFentity = repositoryfactory.Repository().FindEntity(Oldentity.ReimburseSheetCode);//获取没更新之前实体对象
                        if (YFentity.ReimburseSheetID != null)
                        {
                            YFentity.CanUseMoney = YFentity.CanUseMoney + Oldentity.RequiredMoney - entity.RequiredMoney;
                            if (YFentity.CanUseMoney < 0)
                            {
                                database.Rollback();
                                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：关联的预付款不够！" }.ToString());
                            }
                            YFentity.Modify(YFentity.ReimburseSheetID);
                            database.Update(YFentity, isOpenTrans);
                        }
                    }
                    if (Oldentity.TotalAmount < entity.RequiredMoney)
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：金额超过此单可收金额！" }.ToString());
                    }
                    if (Oldentity.Code.IndexOf("CS") >= 0)
                    {
                        BY_CheckSheet by_checksheet = database.FindEntity<BY_CheckSheet>("CheckSheetCode", Oldentity.Code);
                        if (by_checksheet.CheckSheetId != null)
                        {
                            by_checksheet.GetedAmount = by_checksheet.GetedAmount - Oldentity.RequiredMoney + entity.RequiredMoney;
                            entity.NotGetMoney = by_checksheet.TotalAmount - by_checksheet.GetedAmount;
                            if (entity.NotGetMoney < 0)
                            {
                                database.Rollback();
                                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：金额超过此单可收金额！" }.ToString());
                            }
                            by_checksheet.Modify(by_checksheet.CheckSheetId);
                            database.Update(by_checksheet, isOpenTrans);

                            if (by_checksheet.TotalAmount > by_checksheet.GetedAmount)
                            {
                                List<BY_Delivery> by_deliverys = DataFactory.Database().FindList<BY_Delivery>("CheckSheetCode", by_checksheet.CheckSheetCode);
                                foreach (BY_Delivery item in by_deliverys)
                                {
                                    List<BY_ReimburseSheet> dreimbursesheets = DataFactory.Database().FindList<BY_ReimburseSheet>("Code", item.DeliveryCode);
                                    item.GetedAmount = dreimbursesheets.Sum(p => p.RequiredMoney);
                                    item.Modify(item.DeliveryId);
                                    DataFactory.Database().Update<BY_Delivery>(item, isOpenTrans);
                                }
                            }
                            if (by_checksheet.TotalAmount == by_checksheet.GetedAmount)
                            {
                                List<BY_Delivery> by_deliverys = DataFactory.Database().FindList<BY_Delivery>("CheckSheetCode", by_checksheet.CheckSheetCode);
                                foreach (BY_Delivery item in by_deliverys)
                                {
                                    item.GetedAmount = item.TotalAmount;
                                    item.Modify(item.DeliveryId);
                                    DataFactory.Database().Update<BY_Delivery>(item, isOpenTrans);
                                }
                            }
                        }
                    }
                    else if (Oldentity.Code.IndexOf("GC") >= 0 || Oldentity.Code.IndexOf("RC") >= 0 || Oldentity.Code.IndexOf("LJ") >= 0)
                    {
                        BY_Delivery by_delivery = database.FindEntity<BY_Delivery>("DeliveryCode", Oldentity.Code);
                        if (by_delivery.DeliveryId != null)
                        {
                            by_delivery.GetedAmount = by_delivery.GetedAmount - Oldentity.RequiredMoney + entity.RequiredMoney;
                            entity.NotGetMoney = by_delivery.TotalAmount - by_delivery.GetedAmount;
                            if (entity.NotGetMoney < 0)
                            {
                                database.Rollback();
                                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：金额超过此单可收金额！" }.ToString());
                            }
                            by_delivery.Modify(by_delivery.DeliveryId);
                            database.Update(by_delivery, isOpenTrans);
                        }
                    }
                    else if (Oldentity.Code.IndexOf("Y") >= 0)
                    {
                        BY_CheckCompany by_checkcompany = database.FindEntity<BY_CheckCompany>("CheckCompanyCode", Oldentity.Code);
                        if (by_checkcompany.CheckCompanyId != null)
                        {
                            by_checkcompany.GetedAmount = by_checkcompany.GetedAmount - Oldentity.RequiredMoney + entity.RequiredMoney;
                            entity.NotGetMoney = by_checkcompany.TotalAmount - by_checkcompany.GetedAmount;
                            if (entity.NotGetMoney < 0)
                            {
                                database.Rollback();
                                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：金额超过此单可收金额！" }.ToString());
                            }
                            by_checkcompany.Modify(by_checkcompany.CheckCompanyId);
                            database.Update(by_checkcompany, isOpenTrans);
                        }
                    }
                    else if (Oldentity.Code.IndexOf("G") >= 0)
                    {
                        Base_Sheet base_sheet = database.FindEntity<Base_Sheet>("PaySheetCode", Oldentity.Code);
                        if (base_sheet.PaySheetId != null)
                        {
                            base_sheet.GetedAmount = base_sheet.GetedAmount - Oldentity.RequiredMoney + entity.RequiredMoney;
                            entity.NotGetMoney = base_sheet.TotalAmount - base_sheet.GetedAmount;
                            if (entity.NotGetMoney < 0)
                            {
                                database.Rollback();
                                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：金额超过此单可收金额！" }.ToString());
                            }
                            base_sheet.Modify(base_sheet.PaySheetId);
                            database.Update(base_sheet, isOpenTrans);
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(KeyValue) && entity.Opions == "退款")
                {
                    if (entity.RequiredMoney > 0)
                    {
                        entity.RequiredMoney = -entity.RequiredMoney;
                    }
                    if (!string.IsNullOrEmpty(Oldentity.ReimburseSheetCode))
                    {
                        BY_ReimburseSheet YSentity = repositoryfactory.Repository().FindEntity(Oldentity.ReimburseSheetCode);//获取预收
                        if (Oldentity.RequiredMoney != entity.RequiredMoney)
                        {
                            if (YSentity.CanUseMoney - Oldentity.RequiredMoney < -entity.RequiredMoney)
                            {
                                database.Rollback();
                                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：超过预收款可退金额！" }.ToString());
                            }
                            YSentity.CanUseMoney = YSentity.CanUseMoney - Oldentity.RequiredMoney + entity.RequiredMoney;
                            YSentity.Modify(YSentity.ReimburseSheetID);
                            database.Update(YSentity, isOpenTrans);
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(KeyValue) && entity.Opions == "预收款" && Oldentity.RequiredMoney != entity.RequiredMoney)
                {
                    if (Oldentity.RequiredMoney != entity.RequiredMoney && Oldentity.RequiredMoney != Oldentity.CanUseMoney)
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：预收款已经使用或部分使用，请联系管理员！" }.ToString());
                    }
                    entity.CanUseMoney = entity.RequiredMoney;
                }


                entity.Modify(KeyValue);
                IsOk = repositoryfactory.Repository().Update(entity, isOpenTrans);
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

       
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult Print()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult BankPrint()
        {
            return View();
        }
        
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult Print1()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult Form1()
        {
            return View();
        }
        [ManagerPermission(PermissionMode.Enforce)]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult AuditForm()
        {
            return View();
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
        [ManagerPermission(PermissionMode.Enforce)]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult DayReport()
        {
            return View();
        }
        [ManagerPermission(PermissionMode.Enforce)]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult LWDetail()
        {
            return View();
        }
        [ManagerPermission(PermissionMode.Enforce)]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult BankSFList()
        {
            return View();
        }
        [ManagerPermission(PermissionMode.Enforce)]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult BankDetail()
        {
            return View();
        }
        [ManagerPermission(PermissionMode.Enforce)]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult BankChange()
        {
            return View();
        }
        [ManagerPermission(PermissionMode.Enforce)]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult BankDetailLst()
        {
            return View();
        }
        
        
        public ActionResult PrintDataJson(DateTime StartTime, DateTime EndTime)
        {
            try
            {
                JqGridParam jqgridparam = new JqGridParam() { page = 1, records = 99999, rows = 99999, sidx = "CreateDate", sord = "desc" };
                DataTable dt1 = new BY_ReimburseSheetBll().GetPageList1(StartTime,EndTime, ref jqgridparam);
                DataTable dt2 = new BY_ReimburseSheetBll().GetPageList2(StartTime, EndTime, ref jqgridparam);
                DataTable dt3 = new BY_ReimburseSheetBll().GetPageList3(StartTime, EndTime, ref jqgridparam);
                var JsonData = new
                {
                    ListData1 = dt1,
                    ListData2 = dt2,
                    ListData3 = dt3,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }

        }

        public ActionResult GridPageJson1(DateTime StartTime, DateTime EndTime, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = new BY_ReimburseSheetBll().GetPageList1(StartTime,EndTime, ref jqgridparam);
                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = ListData,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ActionResult GridPageJson2(DateTime StartTime, DateTime EndTime, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = new BY_ReimburseSheetBll().GetPageList2(StartTime, EndTime, ref jqgridparam);
                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = ListData,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ActionResult GridPageJson3(DateTime StartTime, DateTime EndTime, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = new BY_ReimburseSheetBll().GetPageList3(StartTime, EndTime, ref jqgridparam);
                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = ListData,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                return null;
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
                if (string.IsNullOrEmpty(type))
                {
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：不支持此类型！"  }.ToString());
                }
                BY_ReimburseSheet by_reimbursesheetYF = DataFactory.Database().FindEntity<BY_ReimburseSheet>(KeyValue);

                string[] hids = (ids.TrimEnd(',')).Split(',');
                string codes = "";
                for (int i = 0; i < hids.Length; i++)
                {
                    codes =codes+ "'" + hids[i] + "',";
                }
                codes = codes.Trim(',');
                List<BY_Delivery> BY_Deliverys = new List<BY_Delivery>();
                if (type == "1")
                {
                    BY_Deliverys = DataFactory.Database().FindListBySql<BY_Delivery>("select * from BY_Delivery where  DeliveryCode in (" + codes + ")");
                }
                List<BY_CheckSheet> BY_CheckSheets = new List<BY_CheckSheet>();
                if (type == "2")
                {
                    BY_CheckSheets = DataFactory.Database().FindListBySql<BY_CheckSheet>("select * from BY_CheckSheet where  CheckSheetCode in (" + codes + ")");
                    BY_Deliverys = DataFactory.Database().FindListBySql<BY_Delivery>("select * from BY_Delivery where  CheckSheetCode in (" + codes + ")");
                }
                List<BY_CheckCompany> BY_CheckCompanys = new List<BY_CheckCompany>();
                if (type == "3")
                {
                    BY_CheckCompanys = DataFactory.Database().FindListBySql<BY_CheckCompany>("select * from BY_CheckCompany where  CheckCompanyCode in (" + codes + ")");

                }


                if (Convert.ToDecimal(totalmoneys) > Convert.ToDecimal(CanUseMoney) + Convert.ToDecimal(zrmoney))
                {
                    by_reimbursesheetYF.CanUseMoney = 0;
                    by_reimbursesheetYF.Remark = Remark;
                    by_reimbursesheetYF.Modify(by_reimbursesheetYF.ReimburseSheetID);
                    DataFactory.Database().Update<BY_ReimburseSheet>(by_reimbursesheetYF, isOpenTrans);

                    decimal? CanUse = Convert.ToDecimal(CanUseMoney) + Convert.ToDecimal(zrmoney);
                    for (int i = 0; i < hids.Length; i++)
                    {

                        BY_ReimburseSheet by_reimbursesheet = new BY_ReimburseSheet();
                        if (i != 0)
                        {
                            by_reimbursesheet.ZRMoney = 0;
                        }
                        else
                        {
                            by_reimbursesheet.ZRMoney = Convert.ToDecimal(zrmoney);
                        }
                        by_reimbursesheet.Create();
                        by_reimbursesheet.AuditMan = DNS.Utilities.ManageProvider.Provider.Current().UserName;
                        by_reimbursesheet.Code = hids[i];
                        by_reimbursesheet.Status = "1";
                        by_reimbursesheet.RequiredMan = ManageProvider.Provider.Current().UserName;
                        by_reimbursesheet.RequiredDate = Convert.ToDateTime(RequiredDate); ;
                        by_reimbursesheet.CustomerCode = by_reimbursesheetYF.CustomerCode;
                        by_reimbursesheet.CustomerName = by_reimbursesheetYF.CustomerName;
                        by_reimbursesheet.CustomerId = by_reimbursesheetYF.CustomerId;
                        by_reimbursesheet.GetType = by_reimbursesheetYF.GetType;
                        by_reimbursesheet.Opions = "应收款";
                        by_reimbursesheet.Type1 = by_reimbursesheetYF.Type1;
                        by_reimbursesheet.Type2 = by_reimbursesheetYF.Type2;
                        by_reimbursesheet.Type3 = by_reimbursesheetYF.Type3;
                        if (type == "1")
                        {
                            BY_Delivery entity = BY_Deliverys.Where(p => p.DeliveryCode == hids[i]).FirstOrDefault();
                            by_reimbursesheet.CodeType = "发运单";
                            if (CanUse > entity.TotalAmount - entity.GetedAmount)
                            {
                                by_reimbursesheet.RequiredMoney = entity.TotalAmount - entity.GetedAmount;
                                CanUse = CanUse - (entity.TotalAmount - entity.GetedAmount);
                            }
                            else
                            {
                                by_reimbursesheet.RequiredMoney = CanUse;
                                CanUse = 0;
                            }
                            by_reimbursesheet.GETMoney = by_reimbursesheet.RequiredMoney - by_reimbursesheet.ZRMoney;
                            by_reimbursesheet.CanUseMoney = 0;
                            by_reimbursesheet.TotalAmount = entity.TotalAmount;
                            by_reimbursesheet.Remark = Remark;
                            by_reimbursesheet.LType = entity.LType;


                            entity.GetedAmount += by_reimbursesheet.RequiredMoney;
                            by_reimbursesheet.NotGetMoney = entity.TotalAmount - entity.GetedAmount;
                            by_reimbursesheet.ReimburseSheetCode = by_reimbursesheetYF.ReimburseSheetID;
                            entity.Modify(entity.DeliveryId);
                            DataFactory.Database().Update<BY_Delivery>(entity, isOpenTrans);
                            database.Insert(by_reimbursesheet, isOpenTrans);

                        }
                        if (type == "2")
                        {
                            BY_CheckSheet entity = BY_CheckSheets.Where(p => p.CheckSheetCode == hids[i]).FirstOrDefault(); 
                            by_reimbursesheet.CodeType = "对账单";

                            if (CanUse > entity.TotalAmount - entity.GetedAmount)
                            {
                                by_reimbursesheet.RequiredMoney = entity.TotalAmount - entity.GetedAmount;
                                CanUse = CanUse - (entity.TotalAmount - entity.GetedAmount);
                            }
                            else
                            {
                                by_reimbursesheet.RequiredMoney = CanUse;
                                CanUse = 0;
                            }

                            by_reimbursesheet.GETMoney = by_reimbursesheet.RequiredMoney - by_reimbursesheet.ZRMoney;
                            by_reimbursesheet.CanUseMoney = 0;
                            by_reimbursesheet.TotalAmount = entity.TotalAmount;
                            by_reimbursesheet.Remark = Remark;
                            by_reimbursesheet.InvoiceNum = entity.InvoiceNum;
                            by_reimbursesheet.LType = entity.LType;


                            entity.GetedAmount += by_reimbursesheet.RequiredMoney;
                            by_reimbursesheet.NotGetMoney = entity.TotalAmount - entity.GetedAmount;
                            by_reimbursesheet.ReimburseSheetCode = by_reimbursesheetYF.ReimburseSheetID;
                            entity.Modify(entity.CheckSheetId);
                            DataFactory.Database().Update<BY_CheckSheet>(entity, isOpenTrans);
                            database.Insert(by_reimbursesheet, isOpenTrans);
                            if (entity.TotalAmount == entity.GetedAmount)
                            {
                                List<BY_Delivery> by_deliverys = BY_Deliverys.Where(p => p.CheckSheetCode == entity.CheckSheetCode).ToList();
                                foreach (BY_Delivery item in by_deliverys)
                                {
                                    item.GetedAmount = item.TotalAmount;
                                    item.Modify(item.DeliveryId);
                                    DataFactory.Database().Update<BY_Delivery>(item, isOpenTrans);
                                }
                            }
                        }
                        if (type == "3")
                        {
                            BY_CheckCompany entity = BY_CheckCompanys.Where(p => p.CheckCompanyCode == hids[i]).FirstOrDefault(); 
                            by_reimbursesheet.CodeType = "收款单";

                            if (CanUse > entity.TotalAmount - entity.GetedAmount)
                            {
                                by_reimbursesheet.RequiredMoney = entity.TotalAmount - entity.GetedAmount;
                                CanUse = CanUse - (entity.TotalAmount - entity.GetedAmount);
                            }
                            else
                            {
                                by_reimbursesheet.RequiredMoney = CanUse;
                                CanUse = 0;
                            }

                            by_reimbursesheet.GETMoney = by_reimbursesheet.RequiredMoney - by_reimbursesheet.ZRMoney;
                            by_reimbursesheet.CanUseMoney = 0;
                            by_reimbursesheet.TotalAmount = entity.TotalAmount;
                            by_reimbursesheet.Remark = Remark;



                            entity.GetedAmount += by_reimbursesheet.RequiredMoney;
                            by_reimbursesheet.NotGetMoney = entity.TotalAmount - entity.GetedAmount;
                            by_reimbursesheet.ReimburseSheetCode = by_reimbursesheetYF.ReimburseSheetID;
                            entity.Modify(entity.CheckCompanyId);
                            DataFactory.Database().Update<BY_CheckCompany>(entity, isOpenTrans);
                            database.Insert(by_reimbursesheet, isOpenTrans);

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
                        by_reimbursesheetYF.Modify(by_reimbursesheetYF.ReimburseSheetID);
                        DataFactory.Database().Update<BY_ReimburseSheet>(by_reimbursesheetYF, isOpenTrans);
                    }
                    else
                    {
                        by_reimbursesheetYF.CanUseMoney = 0;
                        by_reimbursesheetYF.Remark = Remark;
                        by_reimbursesheetYF.Modify(by_reimbursesheetYF.ReimburseSheetID);
                        DataFactory.Database().Update<BY_ReimburseSheet>(by_reimbursesheetYF, isOpenTrans);
                    }

                    for (int i = 0; i < hids.Length; i++)
                    {
                        BY_ReimburseSheet by_reimbursesheet = new BY_ReimburseSheet();
                        if (i != 0)
                        {
                            by_reimbursesheet.ZRMoney = 0;
                        }
                        else
                        {
                            by_reimbursesheet.ZRMoney = Convert.ToDecimal(zrmoney);
                        }

                        by_reimbursesheet.Create();
                        by_reimbursesheet.AuditMan = DNS.Utilities.ManageProvider.Provider.Current().UserName;
                        by_reimbursesheet.Code = hids[i];
                        by_reimbursesheet.Status = "1";
                        by_reimbursesheet.RequiredMan = ManageProvider.Provider.Current().UserName;
                        by_reimbursesheet.RequiredDate = Convert.ToDateTime(RequiredDate); ;
                        by_reimbursesheet.CustomerCode = by_reimbursesheetYF.CustomerCode;
                        by_reimbursesheet.CustomerName = by_reimbursesheetYF.CustomerName;
                        by_reimbursesheet.CustomerId = by_reimbursesheetYF.CustomerId;
                        by_reimbursesheet.GetType = by_reimbursesheetYF.GetType;
                        by_reimbursesheet.Opions = "应收款";
                        by_reimbursesheet.Type1 = by_reimbursesheetYF.Type1;
                        by_reimbursesheet.Type2 = by_reimbursesheetYF.Type2;
                        by_reimbursesheet.Type3 = by_reimbursesheetYF.Type3;


                        if (type == "1")
                        {
                            BY_Delivery entity = BY_Deliverys.Where(p => p.DeliveryCode == hids[i]).FirstOrDefault();
                            by_reimbursesheet.CodeType = "发运单";
                            by_reimbursesheet.RequiredMoney = entity.TotalAmount - entity.GetedAmount;
                            by_reimbursesheet.GETMoney = by_reimbursesheet.RequiredMoney - by_reimbursesheet.ZRMoney;
                            by_reimbursesheet.CanUseMoney = 0;
                            by_reimbursesheet.TotalAmount = entity.TotalAmount;
                            by_reimbursesheet.Remark = Remark;
                            by_reimbursesheet.LType = entity.LType;



                            entity.GetedAmount += by_reimbursesheet.RequiredMoney;
                            by_reimbursesheet.NotGetMoney = entity.TotalAmount - entity.GetedAmount;
                            by_reimbursesheet.ReimburseSheetCode = by_reimbursesheetYF.ReimburseSheetID;
                            entity.Modify(entity.DeliveryId);
                            DataFactory.Database().Update<BY_Delivery>(entity, isOpenTrans);
                            database.Insert(by_reimbursesheet, isOpenTrans);


                        }
                        if (type == "2")
                        {
                            BY_CheckSheet entity = BY_CheckSheets.Where(p => p.CheckSheetCode == hids[i]).FirstOrDefault();
                            by_reimbursesheet.CodeType = "对账单";
                            by_reimbursesheet.RequiredMoney = entity.TotalAmount - entity.GetedAmount;
                            by_reimbursesheet.GETMoney = by_reimbursesheet.RequiredMoney - by_reimbursesheet.ZRMoney;
                            by_reimbursesheet.CanUseMoney = 0;
                            by_reimbursesheet.TotalAmount = entity.TotalAmount;
                            by_reimbursesheet.Remark = Remark;
                            by_reimbursesheet.InvoiceNum = entity.InvoiceNum;
                            by_reimbursesheet.LType = entity.LType;



                            entity.GetedAmount += by_reimbursesheet.RequiredMoney;
                            by_reimbursesheet.NotGetMoney = entity.TotalAmount - entity.GetedAmount;
                            by_reimbursesheet.ReimburseSheetCode = by_reimbursesheetYF.ReimburseSheetID;
                            entity.Modify(entity.CheckSheetId);
                            DataFactory.Database().Update<BY_CheckSheet>(entity, isOpenTrans);
                            database.Insert(by_reimbursesheet, isOpenTrans);
                            if (entity.TotalAmount == entity.GetedAmount)
                            {
                                List<BY_Delivery> by_deliverys = BY_Deliverys.Where(p => p.CheckSheetCode == entity.CheckSheetCode).ToList();
                                foreach (BY_Delivery item in by_deliverys)
                                {
                                    item.GetedAmount = item.TotalAmount;
                                    item.Modify(item.DeliveryId);
                                    DataFactory.Database().Update<BY_Delivery>(item, isOpenTrans);
                                }
                            }

                        }
                        if (type == "3")
                        {
                            BY_CheckCompany entity = BY_CheckCompanys.Where(p => p.CheckCompanyCode == hids[i]).FirstOrDefault();
                            by_reimbursesheet.CodeType = "收款单";
                            by_reimbursesheet.RequiredMoney = entity.TotalAmount - entity.GetedAmount;
                            by_reimbursesheet.GETMoney = by_reimbursesheet.RequiredMoney - by_reimbursesheet.ZRMoney;
                            by_reimbursesheet.CanUseMoney = 0;
                            by_reimbursesheet.TotalAmount = entity.TotalAmount;
                            by_reimbursesheet.Remark = Remark;


                            entity.GetedAmount += by_reimbursesheet.RequiredMoney;
                            by_reimbursesheet.NotGetMoney = entity.TotalAmount - entity.GetedAmount;
                            by_reimbursesheet.ReimburseSheetCode = by_reimbursesheetYF.ReimburseSheetID;
                            entity.Modify(entity.CheckCompanyId);
                            DataFactory.Database().Update<BY_CheckCompany>(entity, isOpenTrans);
                            database.Insert(by_reimbursesheet, isOpenTrans);

                        }
                    }
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

        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult ReturnMoney(string KeyValue, string Remark, decimal? RequiredMoney, string RequiredDate, string GetType, string BankNum)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                BY_ReimburseSheet by_reimbursesheetR = DataFactory.Database().FindEntity<BY_ReimburseSheet>(KeyValue);
                if (RequiredMoney - by_reimbursesheetR.CanUseMoney > 0)
                {
                    database.Rollback();
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：此预收款余额不足" }.ToString());
                }
                by_reimbursesheetR.CanUseMoney = by_reimbursesheetR.CanUseMoney - RequiredMoney;

                BY_ReimburseSheet by_reimbursesheet = new BY_ReimburseSheet();
                Base_CodeRuleBll base_coderulebll = new Base_CodeRuleBll();
                CommonHelper.CopySameFieldsObjectALL<BY_ReimburseSheet, BY_ReimburseSheet>(by_reimbursesheetR, ref by_reimbursesheet);
                by_reimbursesheet.Create();

                by_reimbursesheet.CanUseMoney = 0;
                by_reimbursesheet.RequiredMoney = -RequiredMoney;
                by_reimbursesheet.RequiredMan = ManageProvider.Provider.Current().UserName;
                by_reimbursesheet.Remark = Remark;
                by_reimbursesheet.GETMoney = by_reimbursesheet.RequiredMoney;
                by_reimbursesheet.Opions = "退款";
                by_reimbursesheet.RequiredDate = Convert.ToDateTime(RequiredDate);
                by_reimbursesheet.ReimburseSheetCode = by_reimbursesheetR.ReimburseSheetID;
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
        public virtual ActionResult SubmitFormCost(BY_ReimburseSheet by_reimbursesheet1, string Code1, string CustomerCode)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();

            string[] codes = (Code1.TrimEnd(',')).Split(',');
            decimal? CanUse = Convert.ToDecimal(by_reimbursesheet1.GETMoney) + Convert.ToDecimal(by_reimbursesheet1.ZRMoney);
            string temp = "";
            for (int i = 0; i < codes.Length; i++)
            {
                temp = temp + "'" + codes[i] + "',";
            }
            temp = temp.Trim(',');

            try
            {
                List<BY_ReimburseSheet> by_reimbursesheets = new List<BY_ReimburseSheet>();
                ArrayList checkcodes = new ArrayList();

                Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>("Code", CustomerCode);
                by_reimbursesheet1.CustomerCode = base_customer.Code;
                by_reimbursesheet1.CustomerId = base_customer.CustomerId;
                by_reimbursesheet1.CustomerName = base_customer.FullName;
                List<BY_CheckSheet> by_checksheetall = DataFactory.Database().FindListBySql<BY_CheckSheet>(string.Format("select * from  BY_CheckSheet where CheckSheetCode in ({0})", temp));
                List<BY_Delivery> BY_Deliveryforchecksheet = DataFactory.Database().FindListBySql<BY_Delivery>(string.Format("select * from  BY_Delivery where CheckSheetCode in ({0})   and DeleteMark = 0 ", temp));
                List<BY_Delivery> by_deliveryall = DataFactory.Database().FindListBySql<BY_Delivery>(string.Format("select * from  BY_Delivery where DeliveryCode in ({0})   and DeleteMark = 0 ", temp));
                List<BY_CheckCompany> by_checkcompanyall = DataFactory.Database().FindListBySql<BY_CheckCompany>(string.Format("select * from  BY_CheckCompany where CheckCompanyCode in ({0})", temp));
                List<Base_Sheet> base_sheetall = DataFactory.Database().FindListBySql<Base_Sheet>(string.Format("select * from  Base_Sheet where PaySheetCode in ({0})", temp));

                if (string.IsNullOrEmpty(Code1) && by_reimbursesheet1.Opions == "预收款")
                {
                    by_reimbursesheet1.Create();
                    //by_reimbursesheet1.AuditMan = DNS.Utilities.ManageProvider.Provider.Current().UserName;
                    by_reimbursesheet1.RequiredMan = ManageProvider.Provider.Current().UserName;
                    by_reimbursesheet1.CanUseMoney = by_reimbursesheet1.RequiredMoney;
                    DataFactory.Database().Insert<BY_ReimburseSheet>(by_reimbursesheet1, isOpenTrans);
                    database.Commit();

                    return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功" }.ToString());

                }
                for (int i = 0; i < codes.Length; i++)
                {
                    BY_ReimburseSheet by_reimbursesheet = new BY_ReimburseSheet();
                    CommonHelper.CopySameFieldsObjectALL<BY_ReimburseSheet, BY_ReimburseSheet>(by_reimbursesheet1, ref by_reimbursesheet);
                    if (i != 0)
                    {
                        by_reimbursesheet.ZRMoney = 0;
                    }
                    string Code = codes[i];
                    BY_CheckSheet by_checksheet = by_checksheetall.Where(p => p.CheckSheetCode == Code).FirstOrDefault();
                    BY_Delivery by_delivery = by_deliveryall.Where(p => p.DeliveryCode == Code).FirstOrDefault();
                    BY_CheckCompany by_checkcompany = by_checkcompanyall.Where(p => p.CheckCompanyCode == Code).FirstOrDefault();
                    Base_Sheet base_sheet = base_sheetall.Where(p => p.PaySheetCode == Code).FirstOrDefault();
                    if (by_reimbursesheet.Code.IndexOf("CS") >= 0)
                    {
                        by_reimbursesheet.CodeType = "对账单";
                        //BY_CheckSheet by_checksheet = DataFactory.Database().FindEntity<BY_CheckSheet>("CheckSheetCode", Code);
                        if (CanUse > by_checksheet.TotalAmount - by_checksheet.GetedAmount)
                        {
                            by_reimbursesheet.RequiredMoney = by_checksheet.TotalAmount - by_checksheet.GetedAmount;
                            CanUse = CanUse - (by_checksheet.TotalAmount - by_checksheet.GetedAmount);
                        }
                        else
                        {
                            by_reimbursesheet.RequiredMoney = CanUse;
                            CanUse = 0;
                        }
                        by_reimbursesheet.GETMoney = by_reimbursesheet.RequiredMoney - by_reimbursesheet.ZRMoney;
                        by_reimbursesheet.CanUseMoney = 0;
                        by_reimbursesheet.TotalAmount = by_checksheet.TotalAmount;
                        by_reimbursesheet.InvoiceNum = by_checksheet.InvoiceNum;
                        by_reimbursesheet.LType = by_checksheet.LType;

                    }
                    else if (by_reimbursesheet.Code.IndexOf("GC") >= 0 || by_reimbursesheet.Code.IndexOf("RC") >= 0 || by_reimbursesheet.Code.IndexOf("LJ") >= 0)
                    {
                        by_reimbursesheet.CodeType = "发运单";
                        //BY_Delivery by_delivery = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryCode", Code);
                        if (CanUse > by_delivery.TotalAmount - by_delivery.GetedAmount)
                        {
                            by_reimbursesheet.RequiredMoney = by_delivery.TotalAmount - by_delivery.GetedAmount;
                            CanUse = CanUse - (by_delivery.TotalAmount - by_delivery.GetedAmount);
                        }
                        else
                        {
                            by_reimbursesheet.RequiredMoney = CanUse;
                            CanUse = 0;
                        }
                        by_reimbursesheet.GETMoney = by_reimbursesheet.RequiredMoney - by_reimbursesheet.ZRMoney;
                        by_reimbursesheet.CanUseMoney = 0;
                        by_reimbursesheet.TotalAmount = by_delivery.TotalAmount;
                        by_reimbursesheet.LType = by_delivery.LType;

                    }
                    else if (by_reimbursesheet.Code.IndexOf("Y") >= 0)
                    {
                        by_reimbursesheet.CodeType = "收款单";
                        //BY_CheckCompany by_checkcompany = DataFactory.Database().FindEntity<BY_CheckCompany>("CheckCompanyCode", Code);
                        if (CanUse > by_checkcompany.TotalAmount - by_checkcompany.GetedAmount)
                        {
                            by_reimbursesheet.RequiredMoney = by_checkcompany.TotalAmount - by_checkcompany.GetedAmount;
                            CanUse = CanUse - (by_checkcompany.TotalAmount - by_checkcompany.GetedAmount);
                        }
                        else
                        {
                            by_reimbursesheet.RequiredMoney = CanUse;
                            CanUse = 0;
                        }
                        by_reimbursesheet.GETMoney = by_reimbursesheet.RequiredMoney - by_reimbursesheet.ZRMoney;
                        by_reimbursesheet.CanUseMoney = 0;
                        by_reimbursesheet.TotalAmount = by_checkcompany.TotalAmount;
                    }
                    else if (by_reimbursesheet.Code.IndexOf("G") >= 0)
                    {
                        by_reimbursesheet.CodeType = "收费单";
                        if (CanUse > base_sheet.TotalAmount - base_sheet.GetedAmount)
                        {
                            by_reimbursesheet.RequiredMoney = base_sheet.TotalAmount - base_sheet.GetedAmount;
                            CanUse = CanUse - (base_sheet.TotalAmount - base_sheet.GetedAmount);
                        }
                        else
                        {
                            by_reimbursesheet.RequiredMoney = CanUse;
                            CanUse = 0;
                        }
                        by_reimbursesheet.GETMoney = by_reimbursesheet.RequiredMoney - by_reimbursesheet.ZRMoney;
                        by_reimbursesheet.CanUseMoney = 0;
                        by_reimbursesheet.TotalAmount = base_sheet.TotalAmount;

                        by_reimbursesheet.WorkerId = base_sheet.WorkerId;
                        by_reimbursesheet.WorkerName = base_sheet.WorkerName;
                        by_reimbursesheet.WorkerCode = base_sheet.WorkerCode;
                        by_reimbursesheet.DepartmentName = base_sheet.DepartmentName;

                    }

                    by_reimbursesheet.Create();
                    //by_reimbursesheet.AuditMan = DNS.Utilities.ManageProvider.Provider.Current().UserName;
                    by_reimbursesheet.Code = Code;
                    by_reimbursesheet.RequiredMan = ManageProvider.Provider.Current().UserName;

                    //by_reimbursesheets.Add(by_reimbursesheet);

                    //Base_CodeRuleBll base_coderulebll = new Base_CodeRuleBll();
                    //by_reimbursesheet.ReimburseSheetCode = base_coderulebll.GetBillCode(ManageProvider.Provider.Current().UserId, "SC");
                    //base_coderulebll.OccupyBillCode(ManageProvider.Provider.Current().UserId, "SC", isOpenTrans);//--获取后必须占用


                    //收款单
                    if (by_reimbursesheet.CodeType == "收款单")
                    {
                        //BY_CheckCompany entity = DataFactory.Database().FindEntity<BY_CheckCompany>("CheckCompanyCode", by_reimbursesheet.Code.Trim());
                        if (by_checkcompany.TotalAmount - by_checkcompany.GetedAmount < by_reimbursesheet.RequiredMoney)
                        {
                            database.Rollback();
                            return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：超过此收款单可收金额！可收金额为：" + (by_checkcompany.TotalAmount - by_checkcompany.GetedAmount).ToString() }.ToString());
                        }
                        by_checkcompany.GetedAmount += by_reimbursesheet.RequiredMoney;
                        by_checkcompany.Modify(by_checkcompany.CheckCompanyId);
                        DataFactory.Database().Update<BY_CheckCompany>(by_checkcompany, isOpenTrans);
                        by_reimbursesheet.NotGetMoney = by_checkcompany.TotalAmount - by_checkcompany.GetedAmount;
                        by_reimbursesheet.Remark1 = by_checkcompany.Remark;
                        by_reimbursesheet.CustomerCode = by_checkcompany.CustomerCode;
                        by_reimbursesheet.CustomerId = by_checkcompany.CustomerId;
                        by_reimbursesheet.CustomerName = by_checkcompany.CustomerName;
                    }
                    //收款单
                    if (by_reimbursesheet.CodeType == "收费单")
                    {
                        if (base_sheet.TotalAmount - base_sheet.GetedAmount < by_reimbursesheet.RequiredMoney)
                        {
                            database.Rollback();
                            return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：超过此收款单可收金额！可收金额为：" + (base_sheet.TotalAmount - base_sheet.GetedAmount).ToString() }.ToString());
                        }
                        base_sheet.GetedAmount += by_reimbursesheet.RequiredMoney;
                        base_sheet.Modify(base_sheet.PaySheetId);
                        DataFactory.Database().Update<Base_Sheet>(base_sheet, isOpenTrans);
                        by_reimbursesheet.NotGetMoney = base_sheet.TotalAmount - base_sheet.GetedAmount;
                        by_reimbursesheet.Remark1 = base_sheet.Remark;
                       
                    }
                    //对账单
                    if (by_reimbursesheet.CodeType == "对账单")
                    {
                        //BY_CheckSheet entity = DataFactory.Database().FindEntity<BY_CheckSheet>("CheckSheetCode", by_reimbursesheet.Code.Trim());
                        if (by_checksheet.TotalAmount - by_checksheet.GetedAmount < by_reimbursesheet.RequiredMoney)
                        {
                            database.Rollback();
                            return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：超过此对账单可收金额！可收金额为：" + (by_checksheet.TotalAmount - by_checksheet.GetedAmount).ToString() }.ToString());
                        }
                        by_checksheet.GetedAmount += by_reimbursesheet.RequiredMoney;
                        by_checksheet.Modify(by_checksheet.CheckSheetId);

                        if (by_checksheet.TotalAmount == by_checksheet.GetedAmount)
                        {
                            //List<BY_Delivery> by_deliverys = DataFactory.Database().FindList<BY_Delivery>("CheckSheetCode", by_checksheet.CheckSheetCode);
                            List<BY_Delivery> by_deliverys = BY_Deliveryforchecksheet.Where(p => p.CheckSheetCode == by_checksheet.CheckSheetCode).ToList();
                            foreach (BY_Delivery item in by_deliverys)
                            {
                                item.GetedAmount = item.TotalAmount;
                                item.Modify(item.DeliveryId);
                                DataFactory.Database().Update<BY_Delivery>(item, isOpenTrans);
                            }
                        }

                        DataFactory.Database().Update<BY_CheckSheet>(by_checksheet, isOpenTrans);
                        by_reimbursesheet.NotGetMoney = by_checksheet.TotalAmount - by_checksheet.GetedAmount;
                        by_reimbursesheet.InvoiceNum = by_checksheet.InvoiceNum;
                        by_reimbursesheet.Remark1 = by_checksheet.Remark;
                        by_reimbursesheet.CustomerCode = by_checksheet.CustomerCode;
                        by_reimbursesheet.CustomerId = by_checksheet.CustomerId;
                        by_reimbursesheet.CustomerName = by_checksheet.CustomerName;
                    }
                    //发运单
                    if (by_reimbursesheet.CodeType == "发运单")
                    {
                        //BY_Delivery entity = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryCode", by_reimbursesheet.Code.Trim());
                        if (by_delivery.TotalAmount - by_delivery.GetedAmount < by_reimbursesheet.RequiredMoney)
                        {
                            database.Rollback();
                            return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：超过此发运单可收金额！可收金额为：" + (by_delivery.TotalAmount - by_delivery.GetedAmount).ToString() }.ToString());
                        }

                        by_delivery.GetedAmount += by_reimbursesheet.RequiredMoney;
                        by_delivery.Modify(by_delivery.DeliveryId);
                        DataFactory.Database().Update<BY_Delivery>(by_delivery, isOpenTrans);
                        by_reimbursesheet.NotGetMoney = by_delivery.TotalAmount - by_delivery.GetedAmount;
                        by_reimbursesheet.Remark1 = by_delivery.Remark;
                        by_reimbursesheet.CustomerCode = by_delivery.CustomerCode;
                        by_reimbursesheet.CustomerId = by_delivery.CustomerId;
                        by_reimbursesheet.CustomerName = by_delivery.CustomerName;
                    }
                    database.Insert(by_reimbursesheet, isOpenTrans);
                    if (CanUse == 0)
                    {
                        break;
                    }
                }

                database.Commit();
                if (by_deliveryall.Count > 0)
                {
                    foreach (BY_Delivery item in by_deliveryall)
                    {
                        if (!string.IsNullOrEmpty(item.CheckSheetCode))
                        {
                            BY_CheckSheet updateby_checksheet = DataFactory.Database().FindEntity<BY_CheckSheet>("CheckSheetCode", item.CheckSheetCode);
                            List<BY_Delivery> updatebby_deliverys = DataFactory.Database().FindList<BY_Delivery>("CheckSheetCode", updateby_checksheet.CheckSheetCode);
                            updateby_checksheet.DTotalAmount = updatebby_deliverys.Sum(p => p.TotalAmount);
                            updateby_checksheet.GetedAmount = updatebby_deliverys.Sum(p => p.GetedAmount);
                            updateby_checksheet.DTotalAmountOri = updatebby_deliverys.Sum(p => p.TotalAmount);
                            updateby_checksheet.TotalAmount = updatebby_deliverys.Sum(p => p.TotalAmount);
                            DataFactory.Database().Update(updateby_checksheet);
                        }
                    }
                }



                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功" }.ToString());
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
        public virtual ActionResult AuditCost(BY_ReimburseSheet by_reimbursesheet, string KeyValue, string ReimburseSheetIDs = null)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                string[] ids = ReimburseSheetIDs.Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    by_reimbursesheet = DataFactory.Database().FindEntity<BY_ReimburseSheet>(ids[i]);
                    by_reimbursesheet.Status = "1";
                    by_reimbursesheet.AuditMan = DNS.Utilities.ManageProvider.Provider.Current().UserName;
                    int IsOk = 0;
                    string Message = "操作成功！";
                    if (by_reimbursesheet.Status == "1")
                    {
                        //收款单
                        if (by_reimbursesheet.CodeType == "收款单")
                        {
                            BY_CheckCompany entity = DataFactory.Database().FindEntity<BY_CheckCompany>("CheckCompanyCode", by_reimbursesheet.Code.Trim());
                            if (entity.TotalAmount - entity.GetedAmount < by_reimbursesheet.RequiredMoney)
                            {
                                database.Rollback();
                                return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：超过此收款单可收金额！可收金额为：" + (entity.TotalAmount - entity.GetedAmount).ToString() }.ToString());
                            }
                            entity.GetedAmount += by_reimbursesheet.RequiredMoney;
                            entity.Modify(entity.CheckCompanyId);
                            DataFactory.Database().Update<BY_CheckCompany>(entity, isOpenTrans);
                        }
                        //对账单
                        if (by_reimbursesheet.CodeType == "对账单")
                        {
                            BY_CheckSheet entity = DataFactory.Database().FindEntity<BY_CheckSheet>("CheckSheetCode", by_reimbursesheet.Code.Trim());
                            if (entity.TotalAmount - entity.GetedAmount < by_reimbursesheet.RequiredMoney)
                            {
                                database.Rollback();
                                return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：超过此对账单可收金额！可收金额为：" + (entity.TotalAmount - entity.GetedAmount).ToString() }.ToString());
                            }
                            entity.GetedAmount += by_reimbursesheet.RequiredMoney;
                            entity.Modify(entity.CheckSheetId);

                            DataFactory.Database().Update<BY_CheckSheet>(entity, isOpenTrans);
                        }
                        //发运单
                        if (by_reimbursesheet.CodeType == "发运单")
                        {
                            BY_Delivery entity = DataFactory.Database().FindEntity<BY_Delivery>("DeliveryCode", by_reimbursesheet.Code.Trim());
                            if (entity.TotalAmount - entity.GetedAmount < by_reimbursesheet.RequiredMoney)
                            {
                                database.Rollback();
                                return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：超过此发运单可收金额！可收金额为：" + (entity.TotalAmount - entity.GetedAmount).ToString() }.ToString());
                            }
                            if (!string.IsNullOrEmpty(entity.CheckSheetCode))
                            {
                                BY_CheckSheet by_checksheet = DataFactory.Database().FindEntity<BY_CheckSheet>("CheckSheetCode", entity.CheckSheetCode);
                                if (by_checksheet.TotalAmount - by_checksheet.GetedAmount < by_reimbursesheet.RequiredMoney)
                                {
                                    database.Rollback();
                                    return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：超过发运单对应的对账单可收金额！可收金额为：" + (by_checksheet.TotalAmount - by_checksheet.GetedAmount).ToString() }.ToString());
                                }
                                by_checksheet.GetedAmount += by_reimbursesheet.RequiredMoney;
                                by_checksheet.Modify(by_checksheet.CheckSheetId);
                                DataFactory.Database().Update<BY_CheckSheet>(by_checksheet, isOpenTrans);
                            }
                            entity.GetedAmount += by_reimbursesheet.RequiredMoney;
                            entity.Modify(entity.DeliveryId);
                            DataFactory.Database().Update<BY_Delivery>(entity, isOpenTrans);
                        }

                    }

                    by_reimbursesheet.Modify(by_reimbursesheet.ReimburseSheetID);
                    IsOk = repositoryfactory.Repository().Update(by_reimbursesheet, isOpenTrans);
                }
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功！" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                this.WriteLog(-1, by_reimbursesheet, null, KeyValue, "操作失败：" + ex.Message);
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
    }
}