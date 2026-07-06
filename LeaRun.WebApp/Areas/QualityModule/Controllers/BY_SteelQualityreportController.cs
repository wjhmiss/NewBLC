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
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.QualityModule.Controllers
{
    /// <summary>
    /// BY_SteelQualityreport控制器
    /// </summary>
    public class BY_SteelQualityreportController : PublicController<BY_SteelQualityreport>
    {

        public JsonResult GridPageJson1(string ParameterJson, string Size,string StoveCode,string OrderCodeLine, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                List<BY_SteelQualityreport> ListData = new List<BY_SteelQualityreport>();
                if (!string.IsNullOrEmpty(ParameterJson))
                {
                    List<DbParameter> parameter = new List<DbParameter>();
                    IList conditions = ParameterJson.JonsToList<Condition>();
                    string WhereSql = ConditionBuilder.GetWhereSql(conditions, out parameter);

                    if (!string.IsNullOrEmpty(Size))
                    {
                        string[] Sizes = Size.Replace(" ", "").Split('*');
                        if (Size.IndexOf('Φ') > -1)
                        {
                            WhereSql += @" and SteelQualityreportId IN ( SELECT SteelQualityreportId
                                        FROM BY_SteelQualityreportList de
                                        where  de.Length like @Length AND de.Weigh=0 AND de.Thick like @Thick ) ";
                            parameter.Add(DbFactory.CreateDbParameter("@Length", Sizes[1].Replace("Φ", "") + "%"));
                            parameter.Add(DbFactory.CreateDbParameter("@Thick", Sizes[0].Replace("Φ", "") + "%"));
                        }
                        else
                        {
                            WhereSql += @"  and SteelQualityreportId IN ( SELECT SteelQualityreportId
                                        FROM BY_SteelQualityreportList de
                                        where  ((de.Length like @Length AND de.Weigh like @Weigh AND de.Thick like @Thick) or  
(de.Length like @Length AND de.Weigh like @Thick AND de.Thick like @Weigh) or 

(de.Length like @Weigh AND de.Weigh like @Length AND de.Thick like @Thick) or 

(de.Length like @Thick AND de.Weigh like @Weigh AND de.Thick like @Length) or 

(de.Length like @Weigh AND de.Weigh like @Thick AND de.Thick like @Length) or 

(de.Length like @Thick AND de.Weigh like @Weigh AND de.Thick like @Length)  )  ) ";
                            parameter.Add(DbFactory.CreateDbParameter("@Thick", Sizes[2] + "%"));
                            parameter.Add(DbFactory.CreateDbParameter("@Weigh", Sizes[1] + "%"));
                            parameter.Add(DbFactory.CreateDbParameter("@Length", Sizes[0] + "%"));
                        }
                    }
                    if (!string.IsNullOrEmpty(StoveCode))
                    {
                        
                            WhereSql += @" and SteelQualityreportId IN ( SELECT SteelQualityreportId
                                        FROM BY_SteelQualityreportList de
                                        where  de.CustomerProduct like @StoveCode  ) ";
                            parameter.Add(DbFactory.CreateDbParameter("@StoveCode", "%"+ StoveCode + "%"));
                        
                    }
                    if (!string.IsNullOrEmpty(OrderCodeLine))
                    {

                        WhereSql += @" and SteelQualityreportId IN ( SELECT SteelQualityreportId
                                        FROM BY_SteelQualityreportList de
                                        where  de.OrderCodeLine like @OrderCodeLine  ) ";
                        parameter.Add(DbFactory.CreateDbParameter("@OrderCodeLine", "%" + OrderCodeLine + "%"));

                    }
                    
                    ListData = repositoryfactory.Repository().FindListPage(WhereSql, parameter.ToArray(), ref jqgridparam);
                }
                else
                {
                    ListData = repositoryfactory.Repository().FindListPage(ref jqgridparam);
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


        public ActionResult PrintList(string SteelQualityreportId, int RowNum)
        {
            BY_SteelQualityreport by_steelqualityreport = new BY_SteelQualityreport();
            List<BY_SteelQualityreportList> by_steelqualityreportlists = new List<BY_SteelQualityreportList>();
            if (!string.IsNullOrEmpty(SteelQualityreportId))
            {
                by_steelqualityreport = DataFactory.Database().FindEntity<BY_SteelQualityreport>(SteelQualityreportId);
                by_steelqualityreportlists = (from items in DataFactory.Database().FindList<BY_SteelQualityreportList>("SteelQualityreportId", by_steelqualityreport.SteelQualityreportId) select items).ToList<BY_SteelQualityreportList>();
            }
            else
            {
                return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：请先提交报价单。" }.ToString());
            }

            BY_SteelQualityreportList obj = new BY_SteelQualityreportList();
            Type t = typeof(BY_SteelQualityreportList);
            PropertyInfo[] myPropertyInfo;
            // Get the properties of 'Type' class object.             
            myPropertyInfo = t.GetProperties();
            for (int i = 0; i < myPropertyInfo.Length; i++)
            {
                switch (myPropertyInfo[i].PropertyType.ToString())
                {
                    case "System.Nullable`1[System.Int32]":
                        myPropertyInfo[i].SetValue(obj, 0, null);
                        break;
                    case "System.Nullable`1[System.Decimal]":
                        myPropertyInfo[i].SetValue(obj, Convert.ToDecimal(0), null);
                        break;
                    case "System.Nullable`1[System.DateTime]":
                        //myPropertyInfo[i].SetValue(obj, "", null);       
                        break;
                    case "System.DateTime":
                        //myPropertyInfo[i].SetValue(obj, "", null);       
                        break;

                }
            }

            if (by_steelqualityreportlists.Count() % RowNum != 0)
            {
                int j = (RowNum - by_steelqualityreportlists.Count % RowNum);
                for (int i = 0; i < j; i++)
                {
                    by_steelqualityreportlists.Add(obj);
                }
            }
            var JsonData = new
            {
                BY_SteelQualityreport = by_steelqualityreport,
                BY_SteelQualityreportLists = by_steelqualityreportlists,
            };
            return Content(JsonData.ToJson());
        }
        public virtual ActionResult Print()
        {
            return View();
        }


        public ActionResult SubmitForm1(string KeyValue, BY_SteelQualityreport entity, string SteelListJson)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                List<BY_SteelQualityreportList> by_steelqualityreportlists = (SteelListJson.JonsToList<BY_SteelQualityreportList>()).OrderBy(P => P.SortCode).ToList<BY_SteelQualityreportList>();
                List<BY_SteelQualityreportList> by_steelqualityreportlistadd = new List<BY_SteelQualityreportList>();

                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    entity.Modify(KeyValue);
                }
                else
                {
                    entity.Create();
                }

                BY_SteelQualityreportList by_steelqualityreportlist;
                Base_Supplier Base_Supplier = DataFactory.Database().FindEntity<Base_Supplier>(entity.CustomerId);
                int rownum = 0;
                BY_Order BY_Order = new BY_Order();
                foreach (BY_SteelQualityreportList item in by_steelqualityreportlists)
                {
                    by_steelqualityreportlist = new BY_SteelQualityreportList();
                    CommonHelper.CopySameFieldsObject<BY_SteelQualityreportList, BY_SteelQualityreportList>(item, ref by_steelqualityreportlist);
                    BY_QuotationDetail by_quotationdetail = DataFactory.Database().FindEntity<BY_QuotationDetail>(item.QuotationDetailId);
                    BY_OrderDetail by_orderdetail = DataFactory.Database().FindEntity<BY_OrderDetail>("QuotationDetailId", item.QuotationDetailId);
                    by_steelqualityreportlist.OrderDetailId = by_orderdetail.OrderDetailId;
                    by_steelqualityreportlist.Thick = by_quotationdetail.Thick;
                    by_steelqualityreportlist.Weigh = by_quotationdetail.Weigh;
                    by_steelqualityreportlist.Length = by_quotationdetail.Length;
                    by_steelqualityreportlist.SteelQualityreportCode = entity.SteelQualityreportCode;
                    by_steelqualityreportlist.SteelQualityreportId = entity.SteelQualityreportId;
                    by_steelqualityreportlist.Create();
                    by_steelqualityreportlistadd.Add(by_steelqualityreportlist);
                    rownum = rownum + 1;
                }
                
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    database.Update(entity, isOpenTrans);
                }
                else
                {
                    database.Insert(entity, isOpenTrans);
                }
                BY_SteelQualityreportT by_steelqualityreportt = DataFactory.Database().FindEntity<BY_SteelQualityreportT>("SteelQualityreportCode", entity.SteelQualityreportCode);
                if (by_steelqualityreportt != null && by_steelqualityreportt.SteelQualityreportId != null)
                {
                    CommonHelper.CopySameFieldsObjectALL<BY_SteelQualityreport, BY_SteelQualityreportT>(entity, ref by_steelqualityreportt);
                    by_steelqualityreportt.Modify(by_steelqualityreportt.SteelQualityreportId);
                    database.Update(by_steelqualityreportt, isOpenTrans);
                    
                }
                else
                {
                    CommonHelper.CopySameFieldsObjectALL<BY_SteelQualityreport, BY_SteelQualityreportT>(entity, ref by_steelqualityreportt);
                    by_steelqualityreportt.Create();
                    database.Insert(by_steelqualityreportt, isOpenTrans);

                }
                database.Delete<BY_SteelQualityreportList>("SteelQualityreportId", entity.SteelQualityreportId, isOpenTrans);
                database.Insert(by_steelqualityreportlistadd, isOpenTrans);

                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = entity.SteelQualityreportId }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：录入的数据重复，请检查" }.ToString());
            }
        }
    }
}