using DNS.Business;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.CommonModule.Controllers
{
    /// <summary>
    /// 客户价目控制器
    /// </summary>
    public class CustomerPriceController : Controller
    {
        Base_CustomerProcessBll base_customerprocessbll = new Base_CustomerProcessBll();
        Base_CustomerMaterialsBll base_customermaterialsbll = new Base_CustomerMaterialsBll();
        Base_CustomerManageBll base_customermanagebll = new Base_CustomerManageBll();

        /// <summary>
        /// 客户价目页面
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult Index()
        {
            return View();
        }

        #region 设置客户总折扣
        /// <summary>
        /// 设置客户折扣页面
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult SetDiscount()
        {
            return View();
        }
        #endregion

        #region 加工费折扣
        /// <summary>
        /// 加工费折扣列表（返回表格JSON）
        /// </summary>
        /// <param name="CustomerId">客户主键</param>
        /// <returns></returns>
        public ActionResult GridProcessList(string CustomerId,string rrradio,string SteelCode,  JqGridParam jqgridparam)
        {
            if (string.IsNullOrEmpty(CustomerId))
                return null;
            List<Base_CustomerProcess> ListData = base_customerprocessbll.GetList(CustomerId,rrradio, SteelCode,ref jqgridparam);
            var JsonData = new
            {
                rows = ListData,
            };
            return Content(JsonData.ToJson());
        }
        /// <summary>
        /// 删除加工费折扣
        /// </summary>
        /// <param name="KeyValue">主键</param>
        /// <returns></returns>
        public ActionResult DeleteProcess(string KeyValue)
        {
            try
            {
                int IsOk = base_customerprocessbll.Repository().Delete(KeyValue);
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "删除成功。" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        /// <summary>
        /// 加工费折扣表单页面
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult ProcessForm()
        {
            return View();
        }
        /// <summary>
        /// 获取加工费折扣对象
        /// </summary>
        /// <param name="KeyValue">主键</param>
        /// <returns></returns>
        public ActionResult SetFormProcess(string KeyValue)
        {
            Base_CustomerProcess entity = DataFactory.Database().FindEntity<Base_CustomerProcess>(KeyValue);
            return Content(entity.ToJson());
        }
        
        /// <summary>
        /// 保存加工费折扣表单
        /// </summary>
        /// <param name="KeyValue">主键</param>
        /// <param name="entity">对象实体</param>
        /// <returns></returns>
        public ActionResult SubmitFormProcess(string KeyValue, Base_CustomerProcess entity)
        {
            try
            {
                entity.CustomerProcessId = KeyValue;  
                //if (base_customerprocessbll.IsExists(entity))
                //{
                //    return Content(new JsonMessage { Success = true, Code = "-1", Message = "有效期内，此钢种规格已设置过折扣率" }.ToString());
                //}
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    entity.Modify(KeyValue);
                    IsOk = DataFactory.Database().Update(entity);
                }
                else
                {
                    entity.Create();
                    if (entity.CustomerId == "520f7953-c9c0-48a6-82b5-92a842c4fd37")
                    {
                        entity.SortCode = 1;
                    }
                    IsOk = DataFactory.Database().Insert(entity);
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        #endregion

        #region 材料费折扣
        /// <summary>
        /// 材料费折扣列表（返回表格JSON）
        /// </summary>
        /// <param name="CustomerId">客户主键</param>
        /// <returns></returns>
        public ActionResult GridMaterialsList(string CustomerId,string SteelCode, JqGridParam jqgridparam)
        {
            if (string.IsNullOrEmpty(CustomerId))
                return null;
            List<Base_CustomerMaterials> ListData = base_customermaterialsbll.GetList(CustomerId,SteelCode,ref  jqgridparam);
            var JsonData = new
            {
                rows = ListData,
            };
            return Content(JsonData.ToJson());

        }
        /// <summary>
        /// 删除材料费折扣
        /// </summary>
        /// <param name="KeyValue">主键</param>
        /// <returns></returns>
        public ActionResult DeleteMaterials(string KeyValue)
        {
            try
            {
                int IsOk = base_customermaterialsbll.Repository().Delete(KeyValue);
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "删除成功。" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        /// <summary>
        /// 材料费折扣表单页面
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult MaterialsForm()
        {
            return View();
        }
        /// <summary>
        /// 获取材料费折扣对象
        /// </summary>
        /// <param name="KeyValue">主键</param>
        /// <returns></returns>
        public ActionResult SetFormMaterials(string KeyValue)
        {
            Base_CustomerMaterials entity = DataFactory.Database().FindEntity<Base_CustomerMaterials>(KeyValue);
            return Content(entity.ToJson());
        }

        /// <summary>
        /// 保存材料费折扣表单
        /// </summary>
        /// <param name="KeyValue">主键</param>
        /// <param name="entity">对象实体</param>
        /// <returns></returns>
        public ActionResult SubmitFormMaterials(string KeyValue, Base_CustomerMaterials entity)
        {
            try
            {
                entity.CustomerMaterialsId = KeyValue;
                //if (base_customermaterialsbll.IsExists(entity))
                //{
                //    return Content(new JsonMessage { Success = true, Code = "-1", Message = "有效期内，此钢种规格已设置过折扣率" }.ToString());
                //}
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    entity.Modify(KeyValue);
                    IsOk = DataFactory.Database().Update(entity);
                }
                else
                {
                    entity.Create();
                    if (entity.CustomerId == "520f7953-c9c0-48a6-82b5-92a842c4fd37")
                    {
                        entity.SortCode = 1;
                    }
                    IsOk = DataFactory.Database().Insert(entity);
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        #endregion

        #region 热处理费折扣
        /// <summary>
        /// 热处理费折扣列表（返回表格JSON）
        /// </summary>
        /// <param name="CustomerId">客户主键</param>
        /// <returns></returns>
        public ActionResult GridManageList(string CustomerId)
        {
            if (string.IsNullOrEmpty(CustomerId))
                return null;
            List<Base_CustomerManage> ListData = base_customermanagebll.GetList(CustomerId);
            var JsonData = new
            {
                rows = ListData,
            };
            return Content(JsonData.ToJson());
        }
        /// <summary>
        /// 删除热处理费折扣
        /// </summary>
        /// <param name="KeyValue">主键</param>
        /// <returns></returns>
        public ActionResult DeleteManage(string KeyValue)
        {
            try
            {
                int IsOk = base_customermanagebll.Repository().Delete(KeyValue);
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "删除成功。" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        /// <summary>
        /// 热处理费折扣表单页面
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult ManageForm()
        {
            return View();
        }
        /// <summary>
        /// 获取热处理费折扣对象
        /// </summary>
        /// <param name="KeyValue">主键</param>
        /// <returns></returns>
        public ActionResult SetFormManage(string KeyValue)
        {
            Base_CustomerManage entity = DataFactory.Database().FindEntity<Base_CustomerManage>(KeyValue);
            return Content(entity.ToJson());
        }
        /// <summary>
        /// 保存热处理费折扣表单
        /// </summary>
        /// <param name="KeyValue">主键</param>
        /// <param name="entity">对象实体</param>
        /// <returns></returns>
        public ActionResult SubmitFormManage(string KeyValue, Base_CustomerManage entity)
        {
            try
            {
                entity.CustomerManageId = KeyValue;
                //if (base_customermanagebll.IsExists(entity))
                //{
                //    return Content(new JsonMessage { Success = true, Code = "-1", Message = "有效期内，此热处理已设置过折扣率" }.ToString());
                //}
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    entity.Modify(KeyValue);
                    IsOk = DataFactory.Database().Update(entity);
                }
                else
                {
                    entity.Create();
                    if (entity.CustomerId == "520f7953-c9c0-48a6-82b5-92a842c4fd37")
                    {
                        entity.SortCode = 1;
                    }
                    IsOk = DataFactory.Database().Insert(entity);
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        #endregion
    }
}
