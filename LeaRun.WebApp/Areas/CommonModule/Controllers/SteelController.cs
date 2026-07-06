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
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.CommonModule.Controllers
{
    /// <summary>
    /// 钢材信息表控制器
    /// </summary>
    public class SteelController : PublicController<Base_Steel>
    {
        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public ActionResult SubmitForm1(Base_Steel entity, string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    Base_Steel Oldentity = repositoryfactory.Repository().FindEntity(KeyValue);//获取没更新之前实体对象
                    entity.Modify(KeyValue);
                    IsOk = repositoryfactory.Repository().Update(entity);
                    this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);



                    List<Base_CustomerProcess> Base_CustomerProcess = DataFactory.Database().FindList<Base_CustomerProcess>("SteelId", Oldentity.SteelId);
                    foreach (Base_CustomerProcess item in Base_CustomerProcess)
                    {
                        item.SteelCode = entity.SteelCode;
                        item.SteelId = entity.SteelId;
                        item.SteelName = entity.SteelName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<Base_CustomerMaterials> Base_CustomerMaterials = DataFactory.Database().FindList<Base_CustomerMaterials>("SteelId", Oldentity.SteelId);
                    foreach (Base_CustomerMaterials item in Base_CustomerMaterials)
                    {
                        item.SteelCode = entity.SteelCode;
                        item.SteelId = entity.SteelId;
                        item.SteelName = entity.SteelName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<Base_SteelStatus> Base_SteelStatus = DataFactory.Database().FindList<Base_SteelStatus>("SteelId", Oldentity.SteelId);
                    foreach (Base_SteelStatus item in Base_SteelStatus)
                    {
                        item.SteelCode = entity.SteelCode;
                        item.SteelId = entity.SteelId;
                        item.SteelName = entity.SteelName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<Base_SteelPrice> Base_SteelPrice = DataFactory.Database().FindList<Base_SteelPrice>("SteelId", Oldentity.SteelId);
                    foreach (Base_SteelPrice item in Base_SteelPrice)
                    {
                        item.SteelCode = entity.SteelCode;
                        item.SteelId = entity.SteelId;
                        item.SteelName = entity.SteelName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                   
                    List<BY_QuotationDetail> BY_QuotationDetail = DataFactory.Database().FindList<BY_QuotationDetail>("SteelId", Oldentity.SteelId);
                    foreach (BY_QuotationDetail item in BY_QuotationDetail)
                    {
                        item.SteelCode = entity.SteelCode;
                        item.SteelId = entity.SteelId;
                        item.SteelDescription = entity.SteelName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<BY_OutStock> BY_OutStock = DataFactory.Database().FindList<BY_OutStock>("SteelId", Oldentity.SteelId);
                    foreach (BY_OutStock item in BY_OutStock)
                    {
                        item.SteelCode = entity.SteelCode;
                        item.SteelId = entity.SteelId;
                        item.SteelDescription = entity.SteelName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<BY_OrderDetailProcessDetail> BY_OrderDetailProcessDetail = DataFactory.Database().FindList<BY_OrderDetailProcessDetail>("SteelId", Oldentity.SteelId);
                    foreach (BY_OrderDetailProcessDetail item in BY_OrderDetailProcessDetail)
                    {
                        item.SteelCode = entity.SteelCode;
                        item.SteelId = entity.SteelId;
                        item.SteelDescription = entity.SteelName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<BY_OrderDetailProcess> BY_OrderDetailProcess = DataFactory.Database().FindList<BY_OrderDetailProcess>("SteelId", Oldentity.SteelId);
                    foreach (BY_OrderDetailProcess item in BY_OrderDetailProcess)
                    {
                        item.SteelCode = entity.SteelCode;
                        item.SteelId = entity.SteelId;
                        item.SteelDescription = entity.SteelName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<BY_OrderDetailBeOut> BY_OrderDetailBeOut = DataFactory.Database().FindList<BY_OrderDetailBeOut>("SteelId", Oldentity.SteelId);
                    foreach (BY_OrderDetailBeOut item in BY_OrderDetailBeOut)
                    {
                        item.SteelCode = entity.SteelCode;
                        item.SteelId = entity.SteelId;
                        item.SteelDescription = entity.SteelName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<BY_OrderDetail> BY_OrderDetail = DataFactory.Database().FindList<BY_OrderDetail>("SteelId", Oldentity.SteelId);
                    foreach (BY_OrderDetail item in BY_OrderDetail)
                    {
                        item.SteelCode = entity.SteelCode;
                        item.SteelId = entity.SteelId;
                        item.SteelDescription = entity.SteelName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<BY_Inventory> BY_Inventory = DataFactory.Database().FindList<BY_Inventory>("SteelId", Oldentity.SteelId);
                    foreach (BY_Inventory item in BY_Inventory)
                    {
                        item.SteelCode = entity.SteelCode;
                        item.SteelId = entity.SteelId;
                        item.SteelDescription = entity.SteelName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<BY_InStock> BY_InStock = DataFactory.Database().FindList<BY_InStock>("SteelId", Oldentity.SteelId);
                    foreach (BY_InStock item in BY_InStock)
                    {
                        item.SteelCode = entity.SteelCode;
                        item.SteelId = entity.SteelId;
                        item.SteelDescription = entity.SteelName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<BY_DeliveryDetail> BY_DeliveryDetail = DataFactory.Database().FindList<BY_DeliveryDetail>("SteelId", Oldentity.SteelId);
                    foreach (BY_DeliveryDetail item in BY_DeliveryDetail)
                    {
                        item.SteelCode = entity.SteelCode;
                        item.SteelId = entity.SteelId;
                        item.SteelDescription = entity.SteelName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<BY_AdjustQuery> BY_AdjustQuery = DataFactory.Database().FindList<BY_AdjustQuery>("SteelId", Oldentity.SteelId);
                    foreach (BY_AdjustQuery item in BY_AdjustQuery)
                    {
                        item.SteelCode = entity.SteelCode;
                        item.SteelId = entity.SteelId;
                        item.SteelDescription = entity.SteelName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                }
                else
                {
                    entity.Create();

                    string batchCode = entity.SteelCode;
                    BY_Inventory nt = DataFactory.Database().FindList<BY_Inventory>("MaterialType", "1").Where<BY_Inventory>(p => p.BatchCode == batchCode).FirstOrDefault<BY_Inventory>();
                    if (nt == null || nt.InventoryId == null)
                    {
                        nt = new BY_Inventory();
                        nt.BatchCode = batchCode;
                        nt.ChildBatch = "0×0×0";
                        nt.DeleteMark = 0;
                        nt.Enabled = 1;
                        nt.MaterialType = 1;
                        nt.Remark = entity.Remark;
                        nt.SteelId = entity.SteelId;
                        nt.SteelType = entity.SteelType;
                        nt.SteelCode = entity.SteelCode;
                        nt.SteelDescription = entity.SteelName;
                        nt.Weight = 0;
                        nt.TheoryWeight = 0;
                        nt.Price = entity.Price;
                        nt.InStockDate = DateTime.Now;
                        nt.Density = entity.Density;
                        nt.Create();
                        DataFactory.Database().Insert<BY_Inventory>(nt, isOpenTrans);
                    }


                    repositoryfactory.Repository().Insert(entity, isOpenTrans);
                    
                }
                DataFactory.Database().Commit();
                return Content(new JsonMessage { Success = true, Code = "1".ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                DataFactory.Database().Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：编码或助记码重复！"  }.ToString());
            }
        }


        Base_SteelBll base_steelbll = new Base_SteelBll();
        /// <summary>
        /// 【钢材信息】返回列表JSON
        /// </summary>
        /// <param name="keywords">查询关键字</param>
        /// <param name="jqgridparam">表格参数</param>
        /// <returns></returns>
        public ActionResult GridPageListJson(string keywords, JqGridParam jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                List<DbParameter> parameter = new List<DbParameter>();
                if (keywords!=null)
                {
                    IList conditions = keywords.JonsToList<Condition>();
                    keywords = ConditionBuilder.GetWhereSql(conditions, out parameter);
                }
                List<Base_Steel> ListData = base_steelbll.GetPageList(keywords,parameter, ref jqgridparam);
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
        /// <summary>
        /// 【自动补全】返回钢材列表JSON
        /// </summary>
        /// <param name="keywords">查询关键字</param>
        /// <returns></returns>
        public ActionResult GetAutoCompleJson(string keywords,string isBanOrYuan="", string type = null)
        {
            List<Base_Steel> ListData = base_steelbll.GetAutoComple(keywords, isBanOrYuan);
            return Content(ListData.ToJson());
        }
        /// <summary>
        /// 根据钢种ID获得钢种实体
        /// </summary>
        /// <param name="SteelId">钢种ID</param>
        /// <returns></returns>
        public ActionResult GetSteelEntity(string SteelId)
        {
            Base_Steel Entity =  DataFactory.Database().FindEntity<Base_Steel>(SteelId);
            return Content(Entity.ToJson());
        }

        /// <summary>
        /// 根据钢种code获得钢种实体
        /// </summary>
        /// <param name="SteelId">钢种ID</param>
        /// <returns></returns>
        public ActionResult GetSteelEntityByCode(string SteelCode,string isBan = "")
        {
            
            Base_Steel Entity = DataFactory.Database().FindEntityByWhere<Base_Steel>(string.Format(" and (SteelCode = '{0}' or RemenberCode='{0}') ", SteelCode));
            return Content(Entity.ToJson());
        }

        public ActionResult BindDropSteelCode()
        {
            List<Base_Steel> ListData = DataFactory.Database().FindList<Base_Steel>();
            return Content(ListData.ToJson());
        }
    }
}