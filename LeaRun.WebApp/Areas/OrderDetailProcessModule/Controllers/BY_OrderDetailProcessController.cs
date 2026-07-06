using DNS.Business;
using DNS.Business.Utility.Enum;
using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace DNS.WebApp.Areas.OrderDetailProcessModule.Controllers
{


    /// <summary>
    /// 工单工序表控制器
    /// </summary>
    public class BY_OrderDetailProcessController : PublicController<BY_OrderDetailProcess>
    {

        public JsonResult GetOrderInfor(string orderline)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                string[] strs = orderline.Split('-');
                if (strs.Length != 3)
                {
                    return null;
                }
                string order = strs[0]+"-"+ strs[1];
                string line = strs[2];
                string sql = @"SELECT  bp.*, ISNULL((select sum(bpd.Qty) from BY_OrderDetailProcessDetail bpd where bpd.OrderDetailProcessId = bp.OrderDetailProcessId  ),0) qtyed FROM [dbo].[BY_OrderDetailProcess] bp where 1=1 and OrderCode = '" + order + "' and LineCode = "+ line + "";
                DataTable dt = repositoryfactory.Repository().FindTableBySql(sql);


                string json = JsonConvert.SerializeObject(dt);

                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message );
                return null;
            }
        }




        public virtual JsonResult GridPageJsonForIMG(string ParameterJson, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                List<BY_OrderDetailProcess> ListData = new List<BY_OrderDetailProcess>();
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
                                  ,[WorkerTotal],SalePer,SaleMc,SaleTotal 
                                  ,(select sum(op.Qty) from BY_OrderDetailProcessDetail op where op.OrderDetailProcessId = t.OrderDetailProcessId) Area
                                  ,[QuotationDetailId]
                              FROM [dbo].[BY_OrderDetailProcess] t where 1=1 ";
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


        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult Update(string orderdetailprocessids)
        {
            try
            {
                BY_OrderDetailProcess by_orderdetailprocess = new BY_OrderDetailProcess();
                int IsOk = 0;
                if (!string.IsNullOrEmpty(orderdetailprocessids))
                {
                    string[] ids = orderdetailprocessids.Split(',');
                    for (int i = 0; i < ids.Length; i++)
                    {
                        by_orderdetailprocess.Enabled = 1;
                        by_orderdetailprocess.Modify(ids[i]);
                        IsOk = DataFactory.Database().Update(by_orderdetailprocess);
                    }

                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "操作成功！" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        [HttpPost]
        [ManagerPermission(PermissionMode.Enforce)]
        public virtual ActionResult Delete1(string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                BY_OrderDetailProcess by_orderdetailprocess = DataFactory.Database().FindEntity<BY_OrderDetailProcess>(KeyValue);
                if (by_orderdetailprocess.OrderDetailProcessId != null)
                {
                    DataFactory.Database().Delete(by_orderdetailprocess, isOpenTrans);
                    List<BY_OrderDetailProcessDetail> by_orderdetailprocessdetails = DataFactory.Database().FindList<BY_OrderDetailProcessDetail>("OrderDetailProcessId",by_orderdetailprocess.OrderDetailProcessId);
                    foreach (var item in by_orderdetailprocessdetails)
                    {
                        DataFactory.Database().Delete(item,isOpenTrans);
                    }
                }
                DataFactory.Database().Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "删除成功!" }.ToString());
            }
            catch (Exception ex)
            {
                DataFactory.Database().Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        [LoginAuthorize]
        public virtual ActionResult GridPageJson3(string ParameterJson, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = new DataTable();
                List<DbParameter> parameter = new List<DbParameter>();
                string sql = @"SELECT  P.* ,
        OD.CountShow ,
        O.CustomerName ,
        CASE WHEN OD.OrderType = '毛料生产'
                  AND ( OD.MandXSteel = 1
                        OR OD.MorXSteel = 1
                      )
                  AND ( OD.BSteel = 1
                        OR OD.JBSteel = 1
                      )
             THEN '飞边(' + CASE WHEN OD.BianType = 1 THEN '双短'
                               WHEN OD.BianType = 2 THEN '双长'
                               WHEN OD.BianType = 3 THEN '长短'
                               WHEN OD.BianType = 4 THEN '四'
                          END + '边)磨面(' + CAST(OD.MianType AS VARCHAR(50))
                  + '面)'
             WHEN OD.OrderType = '毛料生产'
                  AND ( OD.MandXSteel = 1
                        OR OD.MorXSteel = 1
                      )
                  AND ( OD.BSteel != 1
                        OR OD.JBSteel != 1
                      ) THEN '磨面(' + CAST(OD.MianType AS VARCHAR(50)) + '面)'
             WHEN OD.OrderType = '毛料生产'
                  AND ( OD.MandXSteel != 1
                        OR OD.MorXSteel != 1
                      )
                  AND ( OD.BSteel = 1
                        OR OD.JBSteel = 1
                      ) THEN '飞边(' + CASE WHEN OD.BianType = 1 THEN '双短'
                                          WHEN OD.BianType = 2 THEN '双长'
                                          WHEN OD.BianType = 3 THEN '长短'
                                          WHEN OD.BianType = 4 THEN '四'
                                     END + '边)'
             WHEN OD.OrderType = '毛料生产'
                  AND ( OD.MandXSteel != 1
                        OR OD.MorXSteel != 1
                      )
                  AND ( OD.BSteel != 1
                        OR OD.JBSteel != 1
                      ) THEN '毛料'
             WHEN OD.OrderType = '精板加工' or OD.OrderType = '精光板加工' THEN '精板'
             WHEN OD.OrderType = '光板加工' THEN '光板'
        END REMARK1 ,
        OD.ProductName,q.CustMaterial
FROM    BY_OrderDetailProcess P
        INNER JOIN dbo.BY_Order O ON P.OrderCode = O.OrderCode
        INNER JOIN dbo.BY_Quotation q ON q.OrderCode = O.OrderCode
        INNER JOIN dbo.BY_OrderDetail OD ON P.OrderDetailId = OD.OrderDetailId
WHERE   1 = 1 ";

                if (!string.IsNullOrEmpty(ParameterJson))
                {
                    IList conditions = ParameterJson.JonsToList<Condition>();
                    string WhereSql = ConditionBuilder.GetWhereSql(conditions, out parameter);
                    ListData = repositoryfactory.Repository().FindTablePageBySql(sql + WhereSql, parameter.ToArray(), ref jqgridparam);
                }
                else
                {
                    ListData = repositoryfactory.Repository().FindTablePageBySql(sql, parameter.ToArray(), ref jqgridparam);
                }
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
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message + "\r\n条件：" + ParameterJson);
                return null;
            }
        }


        [LoginAuthorize]
        public virtual JsonResult GridPageJson1(string ParameterJson, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                List<BY_OrderDetailProcess> ListData = new List<BY_OrderDetailProcess>();
                if (!string.IsNullOrEmpty(ParameterJson))
                {
                    List<DbParameter> parameter = new List<DbParameter>();
                    IList conditions = ParameterJson.JonsToList<Condition>();
                    string WhereSql = ConditionBuilder.GetWhereSql(conditions, out parameter);
                    //ListData = repositoryfactory.Repository().FindListBySql(WhereSql, parameter.ToArray(), ref jqgridparam);
                }
                else
                {
                    //ListData = repositoryfactory.Repository().FindListPage(ref jqgridparam);
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
        public virtual ActionResult UpdateForm(BY_OrderDetailProcess entity, string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    BY_OrderDetailProcess Oldentity = repositoryfactory.Repository().FindEntity(KeyValue);//获取没更新之前实体对象
                    //BY_OrderDetailProcess item = repositoryfactory.Repository().FindEntityByWhere(string.Format(" and OrderCode='{0}' AND LineCode={1} AND TechnologyId='{2}'",
                    //    Oldentity.OrderCode,Oldentity.LineCode,entity.TechnologyId));
                    //if (item!=null && item.OrderDetailProcessId != null)
                    //{
                    //    return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "操作失败：已存在相同工序！" }.ToString());
                    //}

                    List<BY_OrderDetailProcessDetail> by_orderdetailprocessdetails = DataFactory.Database().FindListBySql<BY_OrderDetailProcessDetail>(string.Format(@"  SELECT  *
FROM    BY_OrderDetailProcessDetail
WHERE   OrderCode='{0}' and LineCode={1} AND TechnologyId='{2}'  ", Oldentity.OrderCode, Oldentity.LineCode, Oldentity.TechnologyId));
                    foreach (var item1 in by_orderdetailprocessdetails)
                    {
                        Base_SetPerformance base_setperformance = DataFactory.Database().FindEntityByWhere<Base_SetPerformance>(string.Format(" and WeightEnd >= {0} AND WeightStart <= {0} AND TechnologyId='{1}' ", item1.WeightPer,entity.TechnologyId));
                        if (base_setperformance != null && base_setperformance.SetPerformanceId != null)
                        {
                            item1.WorkerPer = base_setperformance.Price;
                            item1.WorkerUnit = base_setperformance.Unit;
                            if (base_setperformance.Unit == "件")
                            {
                                item1.WorkerAmount = item1.WorkerPer * item1.Qty;
                                
                            }
                            else
                            {
                                item1.WorkerAmount = item1.WorkerPer * item1.Weight;
                                
                            }
                        }
                        item1.TechnologyId = entity.TechnologyId;
                        item1.TechnologyName = entity.TechnologyName;
                        DataFactory.Database().Update(item1, isOpenTrans);
                    }

                    entity.Modify(KeyValue);
                    IsOk = repositoryfactory.Repository().Update(entity, isOpenTrans);
                    this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }
                else
                {
                    if (!string.IsNullOrEmpty(entity.OrderCode) && entity.LineCode != null)
                    {
                        BY_OrderDetailProcess by_orderdetailprocess = repositoryfactory.Repository().FindEntityByWhere(string.Format(@" and OrderCode='{0}' AND LineCode={1} AND TechnologyId='{2}' ", entity.OrderCode, entity.LineCode, entity.TechnologyId));
                        if (by_orderdetailprocess != null && by_orderdetailprocess.OrderDetailProcessId != null)
                        {
                            database.Rollback();
                            return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：已有此工序！" }.ToString());
                        }
                        BY_OrderDetail by_orderdetail = DataFactory.Database().FindEntityByWhere<BY_OrderDetail>(string.Format(@" and OrderCode='{0}' AND LineCode={1}  ", entity.OrderCode, entity.LineCode));
                        BY_Order BY_Order = DataFactory.Database().FindEntity<BY_Order>("OrderCode", by_orderdetail.OrderCode);
                        by_orderdetailprocess.OrderDetailId = by_orderdetail.OrderDetailId;
                        by_orderdetailprocess.OrderCode = by_orderdetail.OrderCode;
                        by_orderdetailprocess.LineCode = by_orderdetail.LineCode;
                        by_orderdetailprocess.SortCode = 6;
                        by_orderdetailprocess.Qty = by_orderdetail.Qty;
                        by_orderdetailprocess.Length = by_orderdetail.Length;
                        by_orderdetailprocess.Weigh = by_orderdetail.Weigh;
                        by_orderdetailprocess.Thick = by_orderdetail.Thick;
                        by_orderdetailprocess.SteelId = by_orderdetail.SteelId;
                        by_orderdetailprocess.SteelCode = by_orderdetail.SteelCode;
                        by_orderdetailprocess.SteelDescription = by_orderdetail.SteelDescription;
                        by_orderdetailprocess.SteelOtherDescription = by_orderdetail.SteelOtherDescription;
                        by_orderdetailprocess.SteelState = by_orderdetail.SteelState;
                        by_orderdetailprocess.OrderType = by_orderdetail.OrderType;
                        by_orderdetailprocess.MnemonicCode = BY_Order.MnemonicCode;
                        by_orderdetailprocess.TechnologyId = entity.TechnologyId;
                        by_orderdetailprocess.TechnologyName = entity.TechnologyName;
                        by_orderdetailprocess.WorkerPer = entity.WorkerPer;
                        by_orderdetailprocess.WorkerTotal = entity.WorkerTotal;


                        by_orderdetailprocess.Create();
                        DataFactory.Database().Insert<BY_OrderDetailProcess>(by_orderdetailprocess, isOpenTrans);
                    }
                }
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1".ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                this.WriteLog(-1, entity, null, KeyValue, "操作失败：" + ex.Message);
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult SubmitScanForm(BY_OrderDetailProcess entity)
        {
            try
            {
                string[] strtemp = entity.OrderCode.Split('-');
                entity.OrderCode = "";
                if (strtemp == null || strtemp.Length != 3)
                {
                    this.WriteLog(-1, entity, null, "", "操作失败：请输入正确的工单号+行号！");
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：请输入正确的工单号+行号！" }.ToString());
                }
                if (strtemp[0] != null && strtemp[1] != null)
                {
                    entity.OrderCode = strtemp[0] + "-" + strtemp[1];
                }
                entity.LineCode = strtemp[2] == null ? -1 : Convert.ToInt32(strtemp[2]);
                if (string.IsNullOrEmpty(entity.OrderCode) || entity.LineCode == -1 || string.IsNullOrEmpty(entity.TechnologyId) || string.IsNullOrEmpty(entity.TechnologyName) || entity.Qty == null)
                {
                    this.WriteLog(-1, entity, null, "", "操作失败：参数为空请联系管理员！");
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：参数为空请联系管理员！" }.ToString());
                }
                
                List<DbParameter> parameter = new List<DbParameter>();
                parameter.Add(DbFactory.CreateDbParameter("@OrderCode", entity.OrderCode));
                parameter.Add(DbFactory.CreateDbParameter("@LineCode", entity.LineCode));
                parameter.Add(DbFactory.CreateDbParameter("@TechnologyId", entity.TechnologyId));
                parameter.Add(DbFactory.CreateDbParameter("@Qty", entity.Qty));
                

                //检查工单行
                BY_Order by_order = DataFactory.Database().FindEntityByWhere<BY_Order>(" AND OrderCode=@OrderCode ", parameter.ToArray());
                BY_OrderDetail by_orderdetail = new BY_OrderDetail();
                if (by_order != null && by_order.OrderId != null)
                {
                    if (by_order.OrderStatus != OrderBillStatus.正常生产.ToString() && by_order.OrderStatus != OrderBillStatus.部分发运.ToString() && by_order.OrderStatus != OrderBillStatus.部分完工.ToString())
                    {
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：此工单状态为" + by_order.OrderStatus + "，不允许扫描！" }.ToString());
                    }
                    parameter.Add(DbFactory.CreateDbParameter("@OrderId", by_order.OrderId));
                    by_orderdetail = DataFactory.Database().FindEntityByWhere<BY_OrderDetail>(" AND OrderId=@OrderId and LineCode=@LineCode", parameter.ToArray());
                    if (by_orderdetail != null && by_orderdetail.OrderDetailId != null)
                    {
                        entity.OrderDetailId = by_orderdetail.OrderDetailId;
                    }
                    else
                    {
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：此行号不存在！" }.ToString());
                    }
                }
                else
                {
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：此工单不存在！" }.ToString());
                }
                if (entity.Qty > by_orderdetail.Qty)
                {
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：数量已超过工单行数量！" }.ToString());
                }
                //检查数量
                parameter.Add(DbFactory.CreateDbParameter("@OrderDetailId", by_orderdetail.OrderDetailId));
                BY_OrderDetailProcess by_orderdetailprocess = DataFactory.Database().FindEntityByWhere<BY_OrderDetailProcess>(" and OrderDetailId =@OrderDetailId AND LineCode =@LineCode AND TechnologyId=@TechnologyId ", parameter.ToArray());
                if (by_orderdetailprocess != null && by_orderdetailprocess.OrderDetailProcessId != null)
                {
                    if (by_orderdetailprocess.Qty + entity.Qty > by_orderdetail.Qty)
                    {
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：数量已超过工单行数量！" }.ToString());
                    }
                    int IsOk = 0;
                    string Message = "更新数量成功。";
                    by_orderdetailprocess.Qty = by_orderdetailprocess.Qty + entity.Qty;
                    IsOk = repositoryfactory.Repository().Update(by_orderdetailprocess);
                    this.WriteLog(IsOk, entity, null, by_orderdetailprocess.OrderDetailProcessId, Message);
                    return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
                }


                //检查工序
                parameter.Add(DbFactory.CreateDbParameter("@DataDictionaryId", "21d16fdf-780d-43ac-9bab-2c5142ba6a7c"));
                Base_DataDictionaryDetail base_datadictionarydetail = DataFactory.Database().FindEntityByWhere<Base_DataDictionaryDetail>(" AND DataDictionaryId=@DataDictionaryId AND Code=@TechnologyId  ", parameter.ToArray());
                if (base_datadictionarydetail != null && base_datadictionarydetail.DataDictionaryDetailId != null)
                {
                    entity.TechnologyName = base_datadictionarydetail.FullName;
                }
                else
                {
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：此工单不存在此工序！" }.ToString());
                }

                entity.Length = by_orderdetail.Length;
                entity.Weigh = by_orderdetail.Weigh;
                entity.Thick = by_orderdetail.Thick;
                entity.SteelId = by_orderdetail.SteelId;
                entity.SteelCode = by_orderdetail.SteelCode;
                entity.SteelDescription = by_orderdetail.SteelDescription;
                entity.SteelOtherDescription = by_orderdetail.SteelOtherDescription;
                entity.SteelState = by_orderdetail.SteelState;
                entity.OrderType = by_order.OrderType;
                entity.Create();
                int IsOk1 = 0;
                string Message1 = "补录成功。";
                IsOk1 = repositoryfactory.Repository().Insert(entity);
                this.WriteLog(IsOk1, entity, null, entity.OrderDetailProcessId, Message1);
                return Content(new JsonMessage { Success = true, Code = IsOk1.ToString(), Message = Message1 }.ToString());

            }
            catch (Exception ex)
            {
                this.WriteLog(-1, entity, null, "", "操作失败：" + ex.Message);
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }

        }
    }
}