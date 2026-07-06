using DNS.Business;
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

namespace DNS.WebApp.Areas.QuotationModule.Controllers
{
    public class AssemblePartsController : PublicController<BY_Quotation>
    {
        public virtual JsonResult ProcessForIMG(string ParameterJson)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                List<BY_OrderDetailProcess> ListData = new List<BY_OrderDetailProcess>();
                List<temall> temalls = new List<temall>();
                BY_OrderDetail BY_OrderDetail = new BY_OrderDetail();
                if (!string.IsNullOrEmpty(ParameterJson))
                {
                    List<DbParameter> parameter = new List<DbParameter>();
                    List<Condition> conditions = ParameterJson.JonsToList<Condition>();
                    string WhereSql = ConditionBuilder.GetWhereSql(conditions, out parameter);
                    string sql = @"SELECT [OrderDetailProcessId]
                                  ,[OrderDetailId]
                                  ,[OrderCode]
                                  ,[OrderType]
                                  ,[LineCode]
                                  ,[Thick]
                                  ,[Weigh]
                                  ,[Length]
                                  ,[TechnologyId]
                                  ,[TechnologyName]
                                  ,[Qty]
                                  ,[QtyComplete]
                                  ,[SteelId]
                                  ,[SteelCode]
                                  ,[SteelDescription]
                                  ,(SELECT STUFF((SELECT '，'+bf.Status FROM base_file bf where bf.AttachId = t.OrderDetailProcessId for xml path('')),1,1,'') ) [SteelOtherDescription]
                                  ,[SteelState]
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
                                  ,[MnemonicCode]
                                  ,[WorkerPer]
                                  ,[WorkerTotal]
                                  ,[Area]
                                  ,[QuotationDetailId]
                              FROM [dbo].[BY_OrderDetailProcess] t where 1=1 ";
                    ListData = DataFactory.Database().FindListBySql<BY_OrderDetailProcess>(sql + WhereSql + " order by SortCode asc", parameter.ToArray());
                    if (conditions[0] != null)
                    {
                        BY_OrderDetail = DataFactory.Database().FindEntity<BY_OrderDetail>(CommonHelper.GetString(conditions[0].ParamValue).Trim());

                    }
                }
                List<base_file> base_files = new List<base_file>();
                List<BY_OrderDetailProcessDetail> by_orderdetailprocessdetails = new List<BY_OrderDetailProcessDetail>();
                foreach (var item in ListData)
                {
                    temall temall = new temall();

                    base_files = DataFactory.Database().FindList<base_file>("AttachId", item.OrderDetailProcessId);
                    by_orderdetailprocessdetails = DataFactory.Database().FindList<BY_OrderDetailProcessDetail>("OrderDetailProcessId", item.OrderDetailProcessId);
                    temall.by_orderdetailprocessdetails = by_orderdetailprocessdetails.OrderByDescending(p=>p.CreateDate).ToList();

                    if (item.Qty == by_orderdetailprocessdetails.Sum(p=>p.Qty))
                    {
                        item.Enabled = 0;
                    }
                    else
                    {
                        item.Enabled = 1;
                    }
                    temall.by_orderdetailprocess = item;
                    temall.base_files = base_files;

                    temalls.Add(temall);

                }
                var JsonData = new
                {
                    rows = temalls,
                    OrderDetail = BY_OrderDetail
                };
                return Json(JsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message + "\r\n条件：" + ParameterJson);
                return null;
            }
        }

        public class temall
        {
            public BY_OrderDetailProcess by_orderdetailprocess { get; set; }
            public List<base_file> base_files { get; set; }
            public List<BY_OrderDetailProcessDetail> by_orderdetailprocessdetails { get; set; }
            
        }

        public ActionResult QueryProcessH5(string OrderDetailId)
        {
            DataTable dt = DataFactory.Database().FindTableBySql(@"SELECT  O.* ,
                                    q.CustMaterial,q.ShipDescription
,(SELECT STUFF((SELECT '，'+bf.Status FROM base_file bf where bf.AttachId = Q.QuotationCode  and bf.Type='图片' for xml path('')),1,1,'') ) [fileattach]
                            FROM    BY_Order O
                                    LEFT JOIN BY_Quotation Q ON Q.QuotationId = O.QuotationId WHERE 1=1 AND O.OrderId = '" + OrderDetailId + "' ");
            DataTable dt2 = DataFactory.Database().FindTableBySql(@"SELECT  O.* ,
                                    q.CustMaterial,q.ShipDescription
,(SELECT STUFF((SELECT '，'+bf.Status FROM base_file bf where bf.AttachId = Q.QuotationCode  and bf.Type='图片' for xml path('')),1,1,'') ) [fileattach]
                            FROM    BY_Order O
                                    LEFT JOIN BY_Quotation Q ON Q.QuotationId = O.QuotationId WHERE 1=1 AND O.OrderId = '" + OrderDetailId + "' ");
            var JsonData = new
            {
                Entiy = dt,
                EntiyDetail = dt2,
            };
            return Content(JsonData.ToJson());
        }
        public ActionResult QueryForH5(string OrderId)
        {
            DataTable dt = DataFactory.Database().FindTableBySql(@"SELECT  O.* ,
                                    q.CustMaterial,q.ShipDescription
,(SELECT STUFF((SELECT '，'+bf.Status FROM base_file bf where bf.AttachId = Q.QuotationCode  and bf.Type='图片' for xml path('')),1,1,'') ) [fileattach]
                            FROM    BY_Order O
                                    LEFT JOIN BY_Quotation Q ON Q.QuotationId = O.QuotationId WHERE 1=1 AND O.OrderId = '" + OrderId + "' ");

            DataTable dt2 = DataFactory.Database().FindTableBySql(@" SELECT  * ,
        ( SELECT TOP 1
                     TechnologyName+'已接收' 
          FROM      BY_OrderDetailProcessDetail opd
          WHERE     opd.OrderDetailId = od.OrderDetailId
          ORDER BY  CreateDate DESC
        ) BillStatus1  --当前状态 
,
( SELECT TOP 1
                    CreateDate
          FROM      dbo.BY_DeliveryDetail opd
          WHERE     opd.OrderDetailId = od.OrderDetailId
          ORDER BY  CreateDate DESC
        ) CreateDate1 ,   
        ISNULL(( SELECT SUM(odd.Qty)
                 FROM   BY_OrderDetailProcessDetail odd
                 WHERE  odd.OrderDetailId = od.OrderDetailId
                        AND odd.LineCode = od.LineCode
                        AND (odd.TechnologyId = 'K001' or odd.TechnologyName = '锯床' or odd.TechnologyId = 'FG' or odd.TechnologyName = '风割' )
               ), 0) Oqty ,
        ISNULL(( SELECT SUM(odd.weight)
                 FROM   BY_OrderDetailProcessDetail odd
                 WHERE  odd.OrderDetailId = od.OrderDetailId
                        AND odd.LineCode = od.LineCode
                        AND (odd.TechnologyId = 'K001' or odd.TechnologyName = '锯床' or odd.TechnologyId = 'FG' or odd.TechnologyName = '风割' )
               ), 0) Oweight, (SELECT STUFF((SELECT '，'+bf.Status FROM base_file bf where bf.AttachId = od.QuotationDetailId  and bf.Type='图片' for xml path('')),1,1,'') ) [Fang1]
FROM    BY_OrderDetail od
WHERE   od.OrderId = '" + OrderId + "' AND OD.SteelCode!='FY' ");

            var JsonData = new
            {
                Entiy = dt,
                EntiyDetail = dt2,
            };
            return Content(JsonData.ToJson());
        }


        public ActionResult PrintWW(string Base_SteelDDBeoutId)
        {
            Base_SteelDDBeout base_steelddbeout = DataFactory.Database().FindEntity<Base_SteelDDBeout>(Base_SteelDDBeoutId);
            List<Base_SteelDDBeoutDetail> base_steelddbeoutdetails = DataFactory.Database().FindList<Base_SteelDDBeoutDetail>("Base_SteelDDBeoutId", base_steelddbeout.Base_SteelDDBeoutId);
 
            List<BY_QuotationDetail> by_quotationdetails = new List<BY_QuotationDetail>();
            List<BY_OrderDetailProcess> by_orderdetailprocesss = new List<BY_OrderDetailProcess>();
            foreach (var item in base_steelddbeoutdetails)
            {
                BY_QuotationDetail by_quotationdetail = DataFactory.Database().FindEntity<BY_QuotationDetail>(item.QuotationDetailId);
                if (by_quotationdetail != null && by_quotationdetail.QuotationDetailId != null)
                {
                    by_quotationdetail.Remark = item.Base_SteelDDBeoutDetailId;
                    by_quotationdetails.Add(by_quotationdetail);
                    
                }
            }
            List<tempsppp> tempsppps = new List<tempsppp>();
            foreach (var item in by_quotationdetails)
            {
                tempsppp tempsppp = new tempsppp();
                List<base_file> base_files = new List<base_file>();
                BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(item.QuotationId);
                
                List<BY_OrderDetailProcess> by_orderdetailprocesssTEMP = DataFactory.Database().FindListBySql<BY_OrderDetailProcess>(" SELECT * FROM BY_OrderDetailProcess WHERE QuotationDetailId ='" + item.QuotationDetailId + "' order by ModifyDate desc");
                for (int i = 0; i < by_orderdetailprocesssTEMP.Count; i++)
                {
                    List<base_file> base_filesTEMP2 = DataFactory.Database().FindListBySql<base_file>(" SELECT * FROM base_file WHERE Type = '图片' AND AttachId ='" + by_orderdetailprocesssTEMP[i].OrderDetailProcessId + "' ");
                    for (int j = 0; j < base_filesTEMP2.Count; j++)
                    {
                        base_filesTEMP2[j].Type = item.QuotationCode.Substring(9) + "-" + item.LineCode.ToString() + "，更新：" + by_orderdetailprocesssTEMP[i].TechnologyName;
                        base_files.Add(base_filesTEMP2[j]);
                    }
                }
                
                List<base_file> base_filesTEMP = DataFactory.Database().FindListBySql<base_file>(" SELECT * FROM base_file WHERE Type = '图片' AND AttachId ='" + item.QuotationDetailId + "' ");
                for (int i = 0; i < base_filesTEMP.Count; i++)
                {
                    base_filesTEMP[i].Type = item.QuotationCode.Substring(9) + "-" + item.LineCode.ToString();
                    base_files.Add(base_filesTEMP[i]);
                }
                List<base_file> base_filesTEMP3 = DataFactory.Database().FindListBySql<base_file>(" SELECT * FROM base_file WHERE Type = '图片' AND AttachId ='" + by_quotation.QuotationCode + "' ");
                for (int i = 0; i < base_filesTEMP3.Count; i++)
                {
                    base_filesTEMP3[i].Type = by_quotation.QuotationCode.Substring(9);
                    base_files.Add(base_filesTEMP3[i]);
                }
                tempsppp.titles = by_quotation.ShortName + "：" + item.QuotationCode + "-" + item.LineCode + "*" + item.Remark;
                tempsppp.base_files = base_files;
                tempsppps.Add(tempsppp);
            }
            
            var JsonData = new
            {
                tempsppps = tempsppps
            };
            return Content(JsonData.ToJson());
        }

        public class tempsppp {
            public string  titles { get; set; }
            public List<base_file> base_files { get; set; }
        }


        
       
        public ActionResult ChoosePrint()
        {
            return View();
        }
        public ActionResult DetailForm()
        {
            return View();
        }
        public ActionResult QueryIndex()
        {
            return View();
        }
        public ActionResult CompleteIndex()
        {
            return View();
        }
        public ActionResult OrderQueryIndex()
        {
            return View();
        }
        public ActionResult OrderDetailIndex()
        {
            return View();
        }
        public ActionResult DeliveryIndex()
        { 
            return View();
        }
        public ActionResult ComfirmIndex()
        {
            return View();
        }
    }
}
