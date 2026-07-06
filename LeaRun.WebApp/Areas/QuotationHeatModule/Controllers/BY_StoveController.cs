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

namespace DNS.WebApp.Areas.QuotationHeatModule.Controllers
{
    /// <summary>
    /// BY_Stove控制器
    /// </summary>
    public class BY_StoveController : PublicController<BY_Stove>
    {
        [LoginAuthorize]
        public  JsonResult GridPageJson1(string QuotationHeatCode, string ParameterJson, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                List<BY_Stove> ListData = new List<BY_Stove>();
                if (!string.IsNullOrEmpty(ParameterJson))
                {
                    List<DbParameter> parameter = new List<DbParameter>();
                    IList conditions = ParameterJson.JonsToList<Condition>();
                    string sql = "select * from BY_Stove s where 1=1 ";
                    string WhereSql = ConditionBuilder.GetWhereSql(conditions, out parameter);
                    if (!string.IsNullOrEmpty(QuotationHeatCode))
                    {
                        WhereSql += @" and EXISTS ( SELECT 1
                 FROM   BY_StoveHeatList sh
                 WHERE  sh.StoveId = s.StoveId 
                 AND sh.QuotationHeatCode = '" + QuotationHeatCode + "')";
                    }
                    ListData = repositoryfactory.Repository().FindListPageBySql(sql + WhereSql, parameter.ToArray(), ref jqgridparam);
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

        public class TempChart
        {
            public string H1Time { get; set; }
            public string H1Code { get; set; }
            public decimal H1Temprature { get; set; }
            public string H1Time1 { get; set; }
            public string H1Code1 { get; set; }
            public decimal H1Temprature1 { get; set; }
            public string H1Time2 { get; set; }
            public string H1Code2 { get; set; }
            public decimal H1Temprature2 { get; set; }
            public string H1Time3 { get; set; }
            public string H1Code3 { get; set; }
            public decimal H1Temprature3 { get; set; }
            public string H1Time4 { get; set; }
            public string H1Code4 { get; set; }
            public decimal H1Temprature4 { get; set; }
            public string H1Time5 { get; set; }
            public string H1Code5 { get; set; }
            public decimal H1Temprature5 { get; set; }
            public string H1Time6 { get; set; }
            public string H1Code6 { get; set; }
            public decimal H1Temprature6 { get; set; }
            public string H1Time7 { get; set; }
            public string H1Code7 { get; set; }
            public decimal H1Temprature7 { get; set; }
            public string H1Time8 { get; set; }
            public string H1Code8 { get; set; }
            public decimal H1Temprature8 { get; set; }



            public string H1Time9 { get; set; }
            public string H1Code9 { get; set; }
            public decimal H1Temprature9 { get; set; }


            public string H1Time10 { get; set; }
            public string H1Code10 { get; set; }
            public decimal H1Temprature10 { get; set; }

            public string H1Time11 { get; set; }
            public string H1Code11 { get; set; }
            public decimal H1Temprature11 { get; set; }
        }
        public ActionResult GetChart(string KeyValue)
        {
            try
            {
                BY_Stove by_stove = DataFactory.Database().FindEntity<BY_Stove>(KeyValue);
                TempChart tempchart = new BY_StoveController.TempChart();
                tempchart.H1Time = "0";
                tempchart.H1Code = "";
                tempchart.H1Temprature = 0;

                tempchart.H1Time1 = "H1(" + by_stove.H1Time1.Replace("&nbsp;", "") + "min)";
                tempchart.H1Code1 = by_stove.H1Code1;
                if (CommonHelper.GetDecimal(by_stove.H1Temprature1) == 0)
                {
                    tempchart.H1Temprature1 = CommonHelper.GetDecimal(by_stove.H1Temprature2);
                }
                else
                {
                    tempchart.H1Temprature1 = CommonHelper.GetDecimal(by_stove.H1Temprature1);
                }

                tempchart.H1Time2 = "H2(" + by_stove.H1Time2.Replace("&nbsp;", "") + "min)";
                tempchart.H1Code2 = by_stove.H1Code2;
                if (CommonHelper.GetDecimal(by_stove.H1Temprature2) == 0)
                {
                    tempchart.H1Temprature2 = CommonHelper.GetDecimal(by_stove.H1Temprature3);
                }
                else
                {
                    tempchart.H1Temprature2 = CommonHelper.GetDecimal(by_stove.H1Temprature2);
                }

                tempchart.H1Time3 = "H3(" + by_stove.H1Time3.Replace("&nbsp;", "") + "min)";
                tempchart.H1Code3 = by_stove.H1Code3;
                if (CommonHelper.GetDecimal(by_stove.H1Temprature3) == 0)
                {
                    tempchart.H1Temprature3 = CommonHelper.GetDecimal(by_stove.H1Temprature4);
                }
                else
                {
                    tempchart.H1Temprature3 = CommonHelper.GetDecimal(by_stove.H1Temprature3);
                }

                tempchart.H1Time4 = "H4(" + by_stove.H1Time4.Replace("&nbsp;", "") + "min)";
                tempchart.H1Code4 = by_stove.H1Code4;
                if (CommonHelper.GetDecimal(by_stove.H1Temprature4) == 0)
                {
                    tempchart.H1Temprature4 = CommonHelper.GetDecimal(by_stove.H1Temprature5);
                }
                else
                {
                    tempchart.H1Temprature4 = CommonHelper.GetDecimal(by_stove.H1Temprature4);
                }

                tempchart.H1Time5 = "H5(" + by_stove.H1Time5.Replace("&nbsp;", "") + "min)";
                tempchart.H1Code5 = by_stove.H1Code5;
                if (CommonHelper.GetDecimal(by_stove.H1Temprature5) == 0)
                {
                    tempchart.H1Temprature5 = CommonHelper.GetDecimal(by_stove.H1Temprature6);
                }
                else
                {
                    tempchart.H1Temprature5 = CommonHelper.GetDecimal(by_stove.H1Temprature5);
                }

                tempchart.H1Time6 = "H6(" + by_stove.H1Time6.Replace("&nbsp;", "") + "min)";
                tempchart.H1Code6 = by_stove.H1Code6;
                if (CommonHelper.GetDecimal(by_stove.H1Temprature6) == 0)
                {
                    tempchart.H1Temprature6 = CommonHelper.GetDecimal(by_stove.H1Temprature7);
                }
                else
                {
                    tempchart.H1Temprature6 = CommonHelper.GetDecimal(by_stove.H1Temprature6);
                }

                tempchart.H1Time7 = "H7(" + by_stove.H1Time7.Replace("&nbsp;", "") + "min)";
                tempchart.H1Code7 = by_stove.H1Code7;
                if (CommonHelper.GetDecimal(by_stove.H1Temprature7) == 0)
                {
                    tempchart.H1Temprature7 = CommonHelper.GetDecimal(by_stove.H1Temprature8);
                }
                else
                {
                    tempchart.H1Temprature7 = CommonHelper.GetDecimal(by_stove.H1Temprature7);
                }


                tempchart.H1Time8 = "H8(" + by_stove.H1Time8.Replace("&nbsp;", "") + "min)";
                tempchart.H1Code8 = by_stove.H1Code8;
                if (CommonHelper.GetDecimal(by_stove.H1Temprature8) == 0)
                {
                    tempchart.H1Temprature8 = CommonHelper.GetDecimal(by_stove.H1Temprature9);
                }
                else
                {
                    tempchart.H1Temprature8 = CommonHelper.GetDecimal(by_stove.H1Temprature8);
                }

                tempchart.H1Time9 = "H9(" + by_stove.H1Time9.Replace("&nbsp;", "") + "min)";
                tempchart.H1Code9 = by_stove.H1Code9;
                if (CommonHelper.GetDecimal(by_stove.H1Temprature9) == 0)
                {
                    tempchart.H1Temprature9 = CommonHelper.GetDecimal(by_stove.H1Temprature10);
                }
                else
                {
                    tempchart.H1Temprature9 = CommonHelper.GetDecimal(by_stove.H1Temprature9);
                }

                tempchart.H1Time10 = "H10(" + by_stove.H1Time10.Replace("&nbsp;", "") + "min)";
                tempchart.H1Code10 = by_stove.H1Code10;
                if (CommonHelper.GetDecimal(by_stove.H1Temprature10) == 0)
                {
                    tempchart.H1Temprature10 = CommonHelper.GetDecimal(tempchart.H1Temprature11);
                }
                else
                {
                    tempchart.H1Temprature10 = CommonHelper.GetDecimal(by_stove.H1Temprature10);
                }

                tempchart.H1Time11 = "0";
                tempchart.H1Code11 = "";
                tempchart.H1Temprature11 = 0;


                var JsonData = new
                {
                    BY_Stove = tempchart,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception)
            {
                return Content(null);
                throw;
            }

        }


        public ActionResult PrintList(string StoveId, int RowNum)
        {
            BY_Stove by_stove = new BY_Stove();
            List<BY_StoveHeatList> by_stoveheatlists = new List<BY_StoveHeatList>();
            if (!string.IsNullOrEmpty(StoveId))
            {
                by_stove = DataFactory.Database().FindEntity<BY_Stove>(StoveId);
                by_stoveheatlists = (from items in DataFactory.Database().FindList<BY_StoveHeatList>("StoveId", by_stove.StoveId) select items).OrderBy(p=>p.QuotationHeatCode).ToList<BY_StoveHeatList>();
            }
            else
            {
                return Content(new JsonMessage { Success = true, Code = "-1", Message = "操作失败：请先选择。" }.ToString());
            }

            BY_StoveHeatList obj = new BY_StoveHeatList();
            Type t = typeof(BY_StoveHeatList);
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
            bool temo = false;

            if (by_stoveheatlists.Count() % RowNum != 0)
            {
                int j = (RowNum - by_stoveheatlists.Count % RowNum);
                for (int i = 0; i < j; i++)
                {
                    by_stoveheatlists.Add(obj);
                    temo = true;
                }
            }

            if (temo)
            {
                by_stoveheatlists.Remove(by_stoveheatlists.LastOrDefault());
            }
            
            BY_StoveHeatList by_stoveheatlist = new BY_StoveHeatList();
            by_stoveheatlist.CustomerName = "小计:";
            by_stoveheatlist.Qty = by_stoveheatlists.Sum<BY_StoveHeatList>(m => m.Qty);
            by_stoveheatlist.Weight = by_stoveheatlists.Sum<BY_StoveHeatList>(m => m.Weight);
            by_stoveheatlists.Add(by_stoveheatlist);

            var JsonData = new
            {
                BY_Stove = by_stove,
                BY_StoveHeatLists = by_stoveheatlists,
            };
            return Content(JsonData.ToJson());
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult PrintZK()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult PrintCG()
        {
            return View();
        }
        private string BillCode(string ModuleId)
        {
            string UserId = ManageProvider.Provider.Current().UserId;
            return base_coderulebll.GetBillCode(UserId, ModuleId);
        }
        Base_CodeRuleBll base_coderulebll = new Base_CodeRuleBll();
        public ActionResult SubmitForm1(string KeyValue, BY_Stove entity, string SteelListJson, string SDDateTime, string DWDateTime, string CLDateTime, string postData2)
        {
            string ModuleId = "L";
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                List<BY_StoveHeatList> by_stoveheatlists = (SteelListJson.JonsToList<BY_StoveHeatList>()).OrderBy(P => P.SortCode).ToList<BY_StoveHeatList>();
                List<BY_StoveHeatList> by_stoveheatlistadd = new List<BY_StoveHeatList>();
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    entity.Modify(KeyValue);
                }
                else
                {
                    entity.StoveCode =     this.BillCode(ModuleId);
                    entity.Create();
                }

                BY_StoveHeatList by_stoveheatlist;
                BY_QuotationHeat by_quotationheat;
                BY_Order BY_Order = new BY_Order();
                foreach (BY_StoveHeatList item in by_stoveheatlists)
                {

                    by_stoveheatlist = new BY_StoveHeatList();
                    by_quotationheat = DataFactory.Database().FindEntity<BY_QuotationHeat>(item.QuotationHeatId);
                    CommonHelper.CopySameFieldsObject<BY_QuotationHeat, BY_StoveHeatList>(by_quotationheat, ref by_stoveheatlist);
                    by_stoveheatlist.TechnologQueryWrite = item.TechnologQueryWrite;
                    by_stoveheatlist.StoveId = entity.StoveId;
                    by_stoveheatlist.StartDateTime = entity.StartDateTime;
                    by_stoveheatlist.StoveNum = entity.StoveNum;
                    by_stoveheatlist.StoveCode = entity.StoveCode;
                    by_stoveheatlist.MethodBuilder = entity.MethodBuilder;
                    by_stoveheatlist.StoveType = entity.StoveType;
                    by_stoveheatlist.BuildMethod = entity.BuildMethod;
                    by_stoveheatlist.CreateDate = by_quotationheat.CreateDate;
                    by_stoveheatlist.Create();
                    by_stoveheatlist.SDDateTime = entity.SDDateTime;
                    by_stoveheatlist.DWDateTime = entity.DWDateTime;
                    by_stoveheatlist.CLDateTime = entity.CLDateTime;
                    by_stoveheatlistadd.Add(by_stoveheatlist);
                }

                if (!string.IsNullOrEmpty(SDDateTime.Replace("&nbsp;", "")))
                {
                    entity.SDDateTime = CommonHelper.GetDateTime(SDDateTime.Replace("：", ":"));
                }
                else
                {
                    entity.SDDateTime = null;
                }
                if (!string.IsNullOrEmpty(DWDateTime.Replace("&nbsp;", "")))
                {
                    entity.DWDateTime = CommonHelper.GetDateTime(DWDateTime.Replace("：", ":"));
                }
                else
                {
                    entity.DWDateTime = null;
                }
                if (!string.IsNullOrEmpty(CLDateTime.Replace("&nbsp;", "")))
                {
                    entity.CLDateTime = CommonHelper.GetDateTime(CLDateTime.Replace("：", ":"));
                }
                else
                {
                    entity.CLDateTime = null;
                }

                entity.Qty = by_stoveheatlistadd.Sum(p => p.Qty);
                entity.Weight = by_stoveheatlistadd.Sum(p => p.Weight);
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    database.Update(entity, isOpenTrans);
                }
                else
                {
                    database.Insert(entity, isOpenTrans);
                }

                database.Delete<BY_StoveHeatList>("StoveId", entity.StoveId, isOpenTrans);
                database.Insert(by_stoveheatlistadd, isOpenTrans);
                base_coderulebll.OccupyBillCode(ManageProvider.Provider.Current().UserId, ModuleId, isOpenTrans);

                database.Commit();
                foreach (var item in by_stoveheatlistadd)
                {
                    UpdateHeatStatus(item);
                }
                return Content(new JsonMessage { Success = true, Code = "1", Message = entity.StoveId, Attribute = entity.StoveCode }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：录入的数据重复，请检查" }.ToString());
            }
        }

        public void UpdateHeatStatus(BY_StoveHeatList by_stoveheatlist)
        {
            List<BY_StoveHeatList> by_stoveheatlistallzk = DataFactory.Database().FindListBySql<BY_StoveHeatList>(" select * from BY_StoveHeatList where StoveType = '真空炉' and   QuotationHeatId = '" + by_stoveheatlist.QuotationHeatId + "'");
            List<BY_StoveHeatList> by_stoveheatlistallpt = DataFactory.Database().FindListBySql<BY_StoveHeatList>(" select * from BY_StoveHeatList where StoveType = '常规炉' and   QuotationHeatId = '" + by_stoveheatlist.QuotationHeatId + "'");
            BY_QuotationHeat by_quotationheat = DataFactory.Database().FindEntity<BY_QuotationHeat>(by_stoveheatlist.QuotationHeatId);
            int zk = 0;
            int pt = 0;
            int tiii = 1;
            string HeatStatus = "";

            foreach (var item in by_stoveheatlistallzk)
            {
                if (item.SDDateTime == null)
                {
                    item.SDDateTime = item.StartDateTime;
                }
            }
            foreach (var item in by_stoveheatlistallpt)
            {
                if (item.SDDateTime == null)
                {
                    item.SDDateTime = item.StartDateTime;
                }
            }
            by_stoveheatlistallzk = by_stoveheatlistallzk.OrderBy(p => p.SDDateTime).ToList();
            by_stoveheatlistallpt = by_stoveheatlistallpt.OrderBy(p => p.SDDateTime).ToList(); 

            if (by_stoveheatlist.Technology == "真空淬火")
            {
                foreach (var item in by_stoveheatlistallzk)
                {
                    item.HeatStatus = (++zk).ToString() + "真空淬火";
                    HeatStatus = item.HeatStatus;
                    DataFactory.Database().Update(item);
                }
                foreach (var item in by_stoveheatlistallpt)
                {
                    item.HeatStatus = "第" + (++pt).ToString() + "次回火";
                    HeatStatus = item.HeatStatus;
                    DataFactory.Database().Update(item);
                }
            }
            else if (by_stoveheatlist.Technology == "调质" || by_stoveheatlist.Technology == "常规淬火")
            {
                foreach (var item in by_stoveheatlistallzk)
                {
                    if (tiii == 1)
                    {
                        item.HeatStatus = (++zk).ToString() + "次淬火";
                        HeatStatus = item.HeatStatus;
                        DataFactory.Database().Update(item);
                    }
                    else
                    {
                        item.HeatStatus = (++zk).ToString() + "次回火";
                        HeatStatus = item.HeatStatus;
                        DataFactory.Database().Update(item);
                    }
                    tiii = tiii + 1;
                }
                foreach (var item in by_stoveheatlistallpt)
                {
                    if (tiii == 1)
                    {
                        item.HeatStatus = (++zk).ToString() + "次淬火";
                        HeatStatus = item.HeatStatus;
                        DataFactory.Database().Update(item);
                    }
                    else
                    {
                        item.HeatStatus = (++zk).ToString() + "次回火";
                        HeatStatus = item.HeatStatus;
                        DataFactory.Database().Update(item);
                    }
                    tiii = tiii + 1;
                }
            }
            else if (by_stoveheatlist.Technology == "去应力" || by_stoveheatlist.Technology == "退火")
            {
                foreach (var item in by_stoveheatlistallzk)
                {
                    item.HeatStatus = (++zk).ToString() + "次回火";
                    HeatStatus = item.HeatStatus;
                    DataFactory.Database().Update(item);
                }
                foreach (var item in by_stoveheatlistallpt)
                {
                    item.HeatStatus = (++zk).ToString() + "次回火";
                    HeatStatus = item.HeatStatus;
                    DataFactory.Database().Update(item);
                }
            }
            else
            {
                foreach (var item in by_stoveheatlistallzk)
                {
                    item.HeatStatus = (++zk).ToString() + "次工序";
                    HeatStatus = item.HeatStatus;
                    DataFactory.Database().Update(item);
                }
                foreach (var item in by_stoveheatlistallpt)
                {
                    item.HeatStatus = (++zk).ToString() + "次工序";
                    HeatStatus = item.HeatStatus;
                    DataFactory.Database().Update(item);
                }
            }
            by_quotationheat.HeatStatus = HeatStatus;
            DataFactory.Database().Update(by_quotationheat);
        }

        [HttpPost]
        [ManagerPermission(PermissionMode.Enforce)]
        public virtual ActionResult Delete1(string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                BY_Stove BY_Stove = DataFactory.Database().FindEntity<BY_Stove>(KeyValue);
                List<BY_StoveHeatList> by_stoveheatlists = DataFactory.Database().FindList<BY_StoveHeatList>("StoveCode", BY_Stove.StoveCode);
                DataFactory.Database().Delete(BY_Stove, isOpenTrans);
                foreach (var item in by_stoveheatlists)
                {
                    DataFactory.Database().Delete(item, isOpenTrans);
                }

                database.Commit();
                foreach (var item in by_stoveheatlists)
                {
                    UpdateHeatStatus(item);
                }
                return Content(new JsonMessage { Success = true, Code = "1", Message = "删除成功！" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
    }
}