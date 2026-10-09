using DNS.Business;
using DNS.Business.Utility.Enum;
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
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.OrderDetailProcessModule.Controllers
{
    /// <summary>
    /// 工单工序详情表控制器
    /// </summary>
    public class BY_OrderDetailProcessDetailController : PublicController<BY_OrderDetailProcessDetail>
    {
        public  ActionResult Delete1(string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                BY_OrderDetailProcessDetail by_orderdetailprocessdetail = DataFactory.Database().FindEntity<BY_OrderDetailProcessDetail>(KeyValue);
                var Message = "删除成功。";
                DataFactory.Database().Delete(by_orderdetailprocessdetail, isOpenTrans);
                
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1".ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        [LoginAuthorize]
        public virtual ActionResult ClassGridsummary(DateTime? StartTime, DateTime? EndTime, JqGridParam jqgridparam)
        {
            try
            {
                BY_OrderBll by_orderbll = new Business.BY_OrderBll();
                DataTable by_orderdetail = new BY_OrderDetailProcessDetailBll().ClassGridsummary(StartTime, EndTime, ref jqgridparam);
                Stopwatch watch = CommonHelper.TimerStart();
                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = by_orderdetail,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public override ActionResult SubmitForm(BY_OrderDetailProcessDetail entity, string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    BY_OrderDetailProcessDetail Oldentity = repositoryfactory.Repository().FindEntity(KeyValue);//获取没更新之前实体对象
                    List<BY_OrderDetailProcessDetail> by_orderdetailprocessdetails = DataFactory.Database().FindList<BY_OrderDetailProcessDetail>("OrderDetailProcessId", Oldentity.OrderDetailProcessId);
                    BY_OrderDetailProcess by_orderdetailprocess = DataFactory.Database().FindEntity<BY_OrderDetailProcess>(Oldentity.OrderDetailProcessId);

                    if (by_orderdetailprocess.Qty < by_orderdetailprocessdetails.Sum(p => p.Qty) - Oldentity.Qty + entity.Qty)
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：超过此工序最大数量！" }.ToString());
                    }

                    BY_OrderDetail by_orderdetail = DataFactory.Database().FindEntity<BY_OrderDetail>(Oldentity.OrderDetailId);
                    if (Oldentity.TechnologyId == "K001" && Oldentity.Qty != entity.Qty)
                    {
                        if (by_orderdetail.BillStatus != "正常生产")
                        {
                            database.Rollback();
                            return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：请联系管理员修改" }.ToString());
                        }
                        by_orderdetail.OpenQty = by_orderdetailprocessdetails.Sum(p => p.Qty) - Oldentity.Qty + entity.Qty;
                        by_orderdetail.Modify(by_orderdetail.OrderDetailId);
                        DataFactory.Database().Update(by_orderdetail, isOpenTrans);
                    }
                    entity.Modify(KeyValue);
                    IsOk = repositoryfactory.Repository().Update(entity, isOpenTrans);
                    this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
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
        public virtual ActionResult Update(string orderdetailprocessdetailids)
        {
            try
            {
                BY_OrderDetailProcessDetail by_orderdetailprocessdetail = new BY_OrderDetailProcessDetail();
                int IsOk = 0;
                if (!string.IsNullOrEmpty(orderdetailprocessdetailids))
                {
                    string[] ids = orderdetailprocessdetailids.Split(',');
                    for (int i = 0; i < ids.Length; i++)
                    {
                        by_orderdetailprocessdetail.Enabled = 1;
                        by_orderdetailprocessdetail.Modify(ids[i]);
                        IsOk = DataFactory.Database().Update(by_orderdetailprocessdetail);
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
        [ValidateInput(false)]
        public virtual ActionResult SubmitScanForm(BY_OrderDetailProcessDetail entity)
        {
            try
            {
                string[] strtemp = entity.OrderCode.Split('-');
                if (strtemp.Length < 3)
                {
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：订单号+行号！" }.ToString());
                }
                //if ((entity.TechnologyId == "KT" || entity.TechnologyId == "JC" || entity.TechnologyId == "QG" || entity.TechnologyId == "LP"))
                //{
                List<DbParameter> parameter = new List<DbParameter>();

                parameter.Add(DbFactory.CreateDbParameter("@OrderCodeLine", strtemp[0] + "-" + strtemp[1] + "-" + strtemp[2]));
                parameter.Add(DbFactory.CreateDbParameter("@Qty", entity.Qty));
                parameter.Add(DbFactory.CreateDbParameter("@Weight", entity.Weight));
                parameter.Add(DbFactory.CreateDbParameter("@ProcessCode", entity.TechnologyId));
                parameter.Add(DbFactory.CreateDbParameter("@MachineCode", entity.MachineId));
                parameter.Add(DbFactory.CreateDbParameter("@UserId", entity.OperatorId));
                DbParameter ReturnMessage = DbFactory.CreateDbParameter();
                ReturnMessage.Direction = ParameterDirection.Output;
                ReturnMessage.ParameterName = "@ReturnMessage";
                ReturnMessage.Size = 50;
                ReturnMessage.DbType = DbType.String;
                parameter.Add(ReturnMessage);

                //DbParameter ReturnID = DbFactory.CreateDbParameter();
                //ReturnID.Direction = ParameterDirection.Output;
                //ReturnID.ParameterName = "@processdetailid";
                //ReturnID.Size = 50;
                //ReturnID.DbType = DbType.String;

                //parameter.Add(ReturnID);
                DataFactory.Database().ExecuteByProc("StoredPBYERP", parameter.ToArray());
                if (ReturnMessage.Value.ToString() == "1")
                {

                    return Content(new JsonMessage { Success = false, Code = "1", Message = "操作成功！" }.ToString());

                }
                else
                {
                    WriteLoginFileLog("扫描", "StoredPBYERP", "失败", ReturnMessage.Value.ToString());
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ReturnMessage.Value }.ToString());
                }
                //}
                //else
                //{
                //    List<DbParameter> parameter = new List<DbParameter>();
                //    parameter.Add(DbFactory.CreateDbParameter("@Type", "A02"));
                //    parameter.Add(DbFactory.CreateDbParameter("@OrderCodeLine", strtemp[0] + "-" + strtemp[1] + "-" + strtemp[2]));
                //    parameter.Add(DbFactory.CreateDbParameter("@ProcessCode", entity.TechnologyId));
                //    parameter.Add(DbFactory.CreateDbParameter("@MachineCode", entity.MachineId));
                //    parameter.Add(DbFactory.CreateDbParameter("@UserId", entity.OperatorId));
                //    parameter.Add(DbFactory.CreateDbParameter("@Qty", entity.Qty));
                //    DbParameter ReturnMessage = DbFactory.CreateDbParameter();
                //    ReturnMessage.Direction = ParameterDirection.Output;
                //    ReturnMessage.ParameterName = "@ReturnMessage";
                //    ReturnMessage.Size = 50;
                //    ReturnMessage.DbType = DbType.String;
                //    parameter.Add(ReturnMessage);
                //    DataFactory.Database().ExecuteByProc("StoredPBYERP", parameter.ToArray());
                //    if (ReturnMessage.Value.ToString() == "1")
                //    {
                //        return Content(new JsonMessage { Success = false, Code = "1", Message = "操作成功！" }.ToString());
                //    }
                //    else
                //    {
                //        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ReturnMessage.Value }.ToString());
                //    }
                //}







                //string[] strtemp = entity.OrderCode.Split('-');
                //entity.OrderCode = "";
                //if (strtemp==null || strtemp.Length !=3)
                //{
                //    this.WriteLog(-1, entity, null, "", "操作失败：请输入正确的工单号+行号！");
                //    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：请输入正确的工单号+行号！" }.ToString());
                //}
                //if (strtemp[0]!=null&&strtemp[1]!=null)
                //{
                //    entity.OrderCode = strtemp[0] + "-" + strtemp[1];
                //}
                //entity.LineCode = strtemp[2] == null ? -1 : Convert.ToInt32(strtemp[2]);
                //if (string.IsNullOrEmpty(entity.OrderCode) || entity.LineCode == -1 || string.IsNullOrEmpty(entity.TechnologyId) || string.IsNullOrEmpty(entity.MachineId) || string.IsNullOrEmpty(entity.OperatorId) || entity.OperatorDate == null || entity.Qty == null)
                //{
                //    this.WriteLog(-1, entity, null, "", "操作失败：参数为空请联系管理员！");
                //    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：参数为空请联系管理员！" }.ToString());
                //}
                //List<DbParameter> parameter = new List<DbParameter>();
                //parameter.Add(DbFactory.CreateDbParameter("@OrderCode", entity.OrderCode));
                //parameter.Add(DbFactory.CreateDbParameter("@LineCode", entity.LineCode));
                //parameter.Add(DbFactory.CreateDbParameter("@TechnologyId", entity.TechnologyId));
                //parameter.Add(DbFactory.CreateDbParameter("@MachineId", entity.MachineId));
                //parameter.Add(DbFactory.CreateDbParameter("@OperatorId", entity.OperatorId));
                //parameter.Add(DbFactory.CreateDbParameter("@OperatorDate", entity.OperatorDate));
                //parameter.Add(DbFactory.CreateDbParameter("@Qty", entity.Qty));

                ////string OrderDetailProcessId, OrderDetailId, TechnologyName,MachineName,OperatorName;
                ////检查工单行
                //BY_Order by_order = DataFactory.Database().FindEntityByWhere<BY_Order>(" AND OrderCode=@OrderCode ", parameter.ToArray());
                //BY_OrderDetail by_orderdetail = new BY_OrderDetail();
                //if (by_order != null && by_order.OrderId != null)
                //{
                //    //if (by_order.OrderStatus != OrderBillStatus.正常生产.ToString() && by_order.OrderStatus != OrderBillStatus.部分发运.ToString() && by_order.OrderStatus != OrderBillStatus.部分完工.ToString() )
                //    //{
                //    //    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：此工单状态为" + by_order.OrderStatus + "，不允许扫描！" }.ToString());
                //    //}
                //    parameter.Add(DbFactory.CreateDbParameter("@OrderId", by_order.OrderId));
                //    by_orderdetail = DataFactory.Database().FindEntityByWhere<BY_OrderDetail>(" AND OrderId=@OrderId and LineCode=@LineCode", parameter.ToArray());
                //    if (by_orderdetail != null && by_orderdetail.OrderDetailId != null)
                //    {
                //        if (by_orderdetail.BillStatus != OrderBillStatus.正常生产.ToString() && by_orderdetail.BillStatus != OrderBillStatus.部分发运.ToString() && by_orderdetail.BillStatus != OrderBillStatus.部分完工.ToString())
                //        {
                //            return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：此工单状态为" + by_orderdetail.BillStatus + "，不允许扫描！" }.ToString());
                //        }
                //        entity.OrderDetailId = by_orderdetail.OrderDetailId;
                //    }
                //    else
                //    {
                //        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：此行号不存在！" }.ToString());
                //    }
                //}
                //else
                //{
                //    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：此工单不存在！" }.ToString());
                //}
                ////检查工序
                //parameter.Add(DbFactory.CreateDbParameter("@DataDictionaryId", "21d16fdf-780d-43ac-9bab-2c5142ba6a7c"));
                //Base_DataDictionaryDetail base_datadictionarydetail = DataFactory.Database().FindEntityByWhere<Base_DataDictionaryDetail>(" AND DataDictionaryId=@DataDictionaryId AND Code=@TechnologyId  ", parameter.ToArray());
                //if (base_datadictionarydetail != null && base_datadictionarydetail.DataDictionaryDetailId != null)
                //{
                //    entity.TechnologyName = base_datadictionarydetail.FullName;
                //}
                //else
                //{
                //    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：此工单不存在此工序！" }.ToString());
                //}
                ////检查机床
                //parameter.Add(DbFactory.CreateDbParameter("@DataDictionaryId2", "7fe64f67-f3c7-4f11-905d-8ad508f01f4d"));
                //Base_DataDictionaryDetail base_datadictionarydetail2 = DataFactory.Database().FindEntityByWhere<Base_DataDictionaryDetail>(" AND DataDictionaryId=@DataDictionaryId2 AND Code=@MachineId  ", parameter.ToArray());
                //if (base_datadictionarydetail2 != null && base_datadictionarydetail2.DataDictionaryDetailId != null)
                //{
                //    entity.MachineName = base_datadictionarydetail2.FullName;
                //}
                //else
                //{
                //    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：此机床不存在！" }.ToString());
                //}
                ////检查工单工序
                //BY_OrderDetailProcess by_orderdetailprocess = DataFactory.Database().FindEntityByWhere<BY_OrderDetailProcess>(" AND OrderCode=@OrderCode AND LineCode=@LineCode AND TechnologyId=@TechnologyId ", parameter.ToArray());
                //if (by_orderdetailprocess != null && by_orderdetailprocess.OrderDetailProcessId != null)
                //{
                //    entity.OrderDetailProcessId = by_orderdetailprocess.OrderDetailProcessId;
                //}
                //else
                //{
                //    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：此工单号没有此操作工序！" }.ToString());
                //}
                ////检查人员        
                //BY_Worker by_worker = DataFactory.Database().FindEntityByWhere<BY_Worker>(" AND WorkerCode=@OperatorId  ", parameter.ToArray());
                //if (by_worker != null && by_worker.WorkerId != null)
                //{
                //    entity.OperatorName = by_worker.WorkerName;
                //}
                //else
                //{
                //    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：此员工不存在！" }.ToString());
                //}
                ////钢材信息
                //parameter.Add(DbFactory.CreateDbParameter("@SteelId", by_orderdetail.SteelId));
                //Base_Steel base_steel = DataFactory.Database().FindEntityByWhere<Base_Steel>(" AND SteelId=@SteelId  ", parameter.ToArray());
                ////数量判断                                                                                                    
                //decimal strQty = CommonHelper.GetDecimal( DataFactory.Database().FindSum<BY_OrderDetailProcessDetail>("Qty", " and OrderCode = @OrderCode AND LineCode =@LineCode AND TechnologyId=@TechnologyId ", parameter.ToArray()));
                //if (strQty + entity.Qty > by_orderdetailprocess.Qty )
                //{
                //    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：数量大于此工序的可扫描数！" }.ToString());
                //}
                //entity.Length = by_orderdetail.Length;
                //entity.Weigh = by_orderdetail.Weigh;
                //entity.Thick = by_orderdetail.Thick;
                //entity.SteelId = by_orderdetail.SteelId;
                //entity.SteelCode = by_orderdetail.SteelCode;
                //entity.SteelDescription = by_orderdetail.SteelDescription;
                //entity.SteelOtherDescription = by_orderdetail.SteelOtherDescription;
                //entity.SteelState = by_orderdetail.SteelState;
                //entity.OrderType = by_orderdetail.OrderType;
                //entity.SteelType = base_steel.SteelType;
                //int IsOk = 0;
                //string Message = "新增成功。";
                //entity.Create();
                //entity = new BY_OrderDetailProcessDetailBll().GetClassesDateClassesShift(entity);
                ////获取计算面积
                //entity = new BY_OrderDetailProcessDetailBll().GetWeightAreaCalcLength(entity, by_order, by_orderdetail, base_steel);
                //if (entity.Weight == 0 && entity.Area == 0 && entity.CalcLength == 0)
                //{
                //    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：未设置工序的计算，请联系管理员！" }.ToString());
                //}
                //IsOk = repositoryfactory.Repository().Insert(entity);
                //this.WriteLog(IsOk, entity, null, entity.OrderDetailProcessDetailId, Message);
                //return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                //this.WriteLog(-1, entity, null, "", "操作失败：" + ex.Message);
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        
        /// <summary>
        /// 转序视图
        /// </summary>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Enforce)]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult ChangeReport()
        {
            return View();
        }
        [ManagerPermission(PermissionMode.Enforce)]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult Summary()
        {
            return View();
        }
        
        /// <summary>
        /// 转序视图
        /// </summary>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Enforce)]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult WorkingHours()
        {
            return View();
        }
        /// <summary>
        /// 开料视图
        /// </summary>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Enforce)]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult OpenReport()
        {
            return View();
        }
        /// <summary>
        /// 开料视图
        /// </summary>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Enforce)]
        [OutputCache(CacheProfile = "Cache1Hour")]
        public virtual ActionResult ClassesReport()
        {
            return View();
        }

        public ActionResult GetQty(string ordercodeline,string processid)
        {
            //if ((entity.TechnologyId == "KT" || entity.TechnologyId == "JC" || entity.TechnologyId == "QG" || entity.TechnologyId == "LP"))
            //{
            List<DbParameter> parameter = new List<DbParameter>();
            parameter.Add(DbFactory.CreateDbParameter("@OrderCodeLine", ordercodeline));

            DbParameter ReturnMessageQty = DbFactory.CreateDbParameter();
            ReturnMessageQty.Direction = ParameterDirection.Output;
            ReturnMessageQty.ParameterName = "@Qty";
            ReturnMessageQty.DbType = DbType.Decimal;
            parameter.Add(ReturnMessageQty);

            DbParameter ReturnMessageWeight = DbFactory.CreateDbParameter();
            ReturnMessageWeight.Direction = ParameterDirection.Output;
            ReturnMessageWeight.ParameterName = "@Weight";
            ReturnMessageWeight.DbType = DbType.Decimal;
            parameter.Add(ReturnMessageWeight);

            DbParameter ReturnMessage = DbFactory.CreateDbParameter();
            ReturnMessage.Direction = ParameterDirection.Output;
            ReturnMessage.ParameterName = "@ReturnMessage";
            ReturnMessage.Size = 50;
            ReturnMessage.DbType = DbType.String;
            parameter.Add(ReturnMessage);
            DataFactory.Database().ExecuteByProc("GetOrderInfo", parameter.ToArray());
            if (ReturnMessage.Value.ToString() == "1")
            {
                return Content(new JsonMessage { Success = false, Code = "1", Message = ReturnMessageQty.Value.ToString() }.ToString());
            }
            else
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ReturnMessage.Value }.ToString());
            }
            
        }



        /// <summary>
        /// 班次完成订单量表格
        /// </summary>
        /// <param name="ParameterJson">查询条件</param>
        /// <param name="Gridpage">分页条件</param>
        /// <returns></returns>
        [LoginAuthorize]
        public virtual ActionResult ClassGridPageJson(DateTime? datetime,DateTime? datetime2, JqGridParam jqgridparam)
        {
            try
            {
                BY_OrderBll by_orderbll = new Business.BY_OrderBll();
                DataTable by_orderdetail = new BY_OrderDetailProcessDetailBll().ClassGridPageJson(datetime,datetime2, ref jqgridparam);
                Stopwatch watch = CommonHelper.TimerStart();
                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = by_orderdetail,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 绑定表格
        /// </summary>
        /// <param name="ParameterJson">查询条件</param>
        /// <param name="Gridpage">分页条件</param>
        /// <returns></returns>
        public  ActionResult GridPageJson1(string ParameterJson,string Size, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                //List<BY_OrderDetailProcessDetail> ListData = new List<BY_OrderDetailProcessDetail>();
                DataTable ListData = new DataTable();
                if (!string.IsNullOrEmpty(ParameterJson))
                {
                    List<DbParameter> parameter = new List<DbParameter>();
                    IList conditions = ParameterJson.JonsToList<Condition>();
                    string WhereSql = ConditionBuilder.GetWhereSql(conditions, out parameter,"","","");

                    string sql = @"SELECT  odd.* , bp.SalePer,bp.SaleMc,bp.SaleTotal ,
        o.CustomerName ,q.ShortName,
        od.CountShow ,
        q.CustMaterial ,
        CASE WHEN odd.Weigh > 0
             THEN CAST(CONVERT(FLOAT, odd.Length) AS VARCHAR(50)) + '*'
                  + CAST(CONVERT(FLOAT, odd.Weigh) AS VARCHAR(50)) + '*'
                  + CAST(CONVERT(FLOAT, odd.Thick) AS VARCHAR(50))
             ELSE 'Φ' + CAST(CONVERT(FLOAT, odd.Thick) AS VARCHAR(50)) + '*'
                  + CAST(CONVERT(FLOAT, odd.Length) AS VARCHAR(50))
        END AS size,
(SELECT STUFF((SELECT '，'+bf.Status FROM base_file bf where bf.AttachId = odd.OrderDetailProcessDetailId for xml path('')),1,1,'') ) fileattach,
(SELECT STUFF((SELECT ','+bf.Path FROM base_file bf where bf.AttachId = odd.OrderDetailProcessDetailId for xml path('')),1,1,'') ) filepath
FROM    BY_OrderDetailProcessDetail odd
        INNER JOIN dbo.BY_Order o ON odd.OrderCode = o.OrderCode
        INNER JOIN dbo.BY_Quotation q ON q.OrderCode = O.OrderCode
        INNER JOIN BY_orderDetail od ON odd.OrderDetailId = od.OrderDetailId
        INNER JOIN dbo.BY_OrderDetailProcess bp ON bp.OrderDetailProcessId = odd.OrderDetailProcessId
WHERE   1 = 1  AND Q.DeleteMark = 0  and o.DeleteMark=0 " + WhereSql;
                    if (!string.IsNullOrEmpty(Size))
                    {
                        string[] Sizes = Size.Replace(" ", "").Split('*');
                        if (Size.IndexOf('Φ') > -1)
                        {
                            sql += @"    AND odd.Length like @Length AND odd.Weigh=0 AND odd.Thick like @Thick   ";
                            parameter.Add(DbFactory.CreateDbParameter("@Length", Sizes[1].Replace("Φ", "") + "%"));
                            parameter.Add(DbFactory.CreateDbParameter("@Thick", Sizes[0].Replace("Φ", "") + "%"));
                        }
                        else
                        {
                            sql += @"  AND ((odd.Length like @Length AND odd.Weigh like @Weigh AND odd.Thick like @Thick) or  
(odd.Length like @Length AND odd.Weigh like @Thick AND odd.Thick like @Weigh) or 

(odd.Length like @Weigh AND odd.Weigh like @Length AND odd.Thick like @Thick) or 

(odd.Length like @Thick AND odd.Weigh like @Weigh AND odd.Thick like @Length) or 

(odd.Length like @Weigh AND odd.Weigh like @Thick AND odd.Thick like @Length) or 

(odd.Length like @Thick AND odd.Weigh like @Weigh AND odd.Thick like @Length)  )  ";
                            parameter.Add(DbFactory.CreateDbParameter("@Thick", Sizes[2] + "%"));
                            parameter.Add(DbFactory.CreateDbParameter("@Weigh", Sizes[1] + "%"));
                            parameter.Add(DbFactory.CreateDbParameter("@Length", Sizes[0] + "%"));
                        }
                    }
                    ListData = repositoryfactory.Repository().FindTablePageBySql(sql, parameter.ToArray(), ref jqgridparam);
                }
                else
                {
                    ListData = repositoryfactory.Repository().FindTablePageBySql(@"SELECT  odd.* ,
        o.CustomerName ,
        od.CountShow ,
        q.CustMaterial ,
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
        END REMARK1
FROM    BY_OrderDetailProcessDetail odd
        INNER JOIN dbo.BY_Order o ON odd.OrderCode = o.OrderCode
        INNER JOIN dbo.BY_Quotation q ON q.OrderCode = O.OrderCode
        INNER JOIN BY_orderDetail od ON odd.OrderDetailId = od.OrderDetailId
WHERE   1 = 1  AND Q.DeleteMark = 0  and o.DeleteMark=0 ", ref jqgridparam);
                }
                //List<Base_SetPerformance> CostDatas = DataFactory.Database().FindList<Base_SetPerformance>();
                //foreach (BY_OrderDetailProcessDetail item in ListData)
                //{
                //    Base_SetPerformance base_setperformance = (from entity in CostDatas
                //                      where entity.TechnologyName==item.TechnologyName && entity.OrderType==item.OrderType && entity.SteelType==item.SteelType
                //                   select entity).FirstOrDefault<Base_SetPerformance>(); ;
                //    if (base_setperformance != null && base_setperformance.PriceArea != null)
                //    {
                //        if (item.Area < base_setperformance.LimitArea)
                //        {
                //            item.WordCost = base_setperformance.PriceArea * base_setperformance.LimitArea;
                //        }
                //        else
                //        {
                //            item.WordCost = base_setperformance.PriceArea * item.Area;
                //        }
                //    }
                //}
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
                return Content(null);
            }
        }



        public ActionResult GridPageJson2(string ParameterJson, string Size, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                //List<BY_OrderDetailProcessDetail> ListData = new List<BY_OrderDetailProcessDetail>();
                DataTable ListData = new DataTable();
                if (!string.IsNullOrEmpty(ParameterJson))
                {
                    List<DbParameter> parameter = new List<DbParameter>();
                    IList conditions = ParameterJson.JonsToList<Condition>();
                    string WhereSql = ConditionBuilder.GetWhereSql(conditions, out parameter, "", "", "");

                    string sql = @"select bp.*,
(select isnull(sum(bd.Qty),0) from BY_OrderDetailProcessDetail bd where bd.OrderDetailProcessId = bp.OrderDetailProcessId) compeletqty,
(select top 1 bd.OperatorDate from BY_OrderDetailProcessDetail bd where bd.OrderDetailProcessId = bp.OrderDetailProcessId) operatordate,
(select top 1 bd.operatorname from BY_OrderDetailProcessDetail bd where bd.OrderDetailProcessId = bp.OrderDetailProcessId) operatorname,
(select top 1 bd.operatoroverdate from BY_OrderDetailProcessDetail bd where bd.OrderDetailProcessId = bp.OrderDetailProcessId) operatoroverdate,
(SELECT STUFF((SELECT '，'+bf.Status FROM base_file bf,BY_OrderDetailProcessDetail bd where bf.AttachId = bd.OrderDetailProcessDetailId and bd.OrderDetailProcessId = bp.OrderDetailProcessId for xml path('')),1,1,'') ) fileattach
from BY_OrderDetailProcess bp where 1= 1 " + WhereSql;
                    ListData = repositoryfactory.Repository().FindTablePageBySql(sql, parameter.ToArray(), ref jqgridparam);
                }
                
                //List<Base_SetPerformance> CostDatas = DataFactory.Database().FindList<Base_SetPerformance>();
                //foreach (BY_OrderDetailProcessDetail item in ListData)
                //{
                //    Base_SetPerformance base_setperformance = (from entity in CostDatas
                //                      where entity.TechnologyName==item.TechnologyName && entity.OrderType==item.OrderType && entity.SteelType==item.SteelType
                //                   select entity).FirstOrDefault<Base_SetPerformance>(); ;
                //    if (base_setperformance != null && base_setperformance.PriceArea != null)
                //    {
                //        if (item.Area < base_setperformance.LimitArea)
                //        {
                //            item.WordCost = base_setperformance.PriceArea * base_setperformance.LimitArea;
                //        }
                //        else
                //        {
                //            item.WordCost = base_setperformance.PriceArea * item.Area;
                //        }
                //    }
                //}
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
                return Content(null);
            }
        }
    }
}