using DNS.Business;
using DNS.Entity;
using DNS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.CommonModule.Controllers
{
    /// <summary>
    /// 钢材价目表控制器
    /// </summary>
    public class SteelPriceController : PublicController<Base_SteelPrice>
    {
        Base_SteelPriceBll base_steelpricebll = new Base_SteelPriceBll();
        /// <summary>
        /// 【自动补全】返回钢材列表JSON
        /// </summary>
        /// <param name="keywords">查询关键字</param>
        /// <returns></returns>
        public ActionResult GetAutoCompleJson(string keywords)
        {
            List<Base_SteelPrice> ListData = base_steelpricebll.GetAutoComple(keywords);
            return Content(ListData.ToJson());
        }
        /// <summary>
        /// 提交表单
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public override ActionResult SubmitForm(Base_SteelPrice entity, string KeyValue)
        {
            try
            {
                entity.SteelPriceId = KeyValue;
                if (base_steelpricebll.IsExists(entity))
                {
                    return Content(new JsonMessage { Success = true, Code = "-1", Message = "已有相同厚度类型的钢材！" }.ToString());
                }
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    entity.Modify(KeyValue);
                    IsOk = repositoryfactory.Repository().Update(entity);
                }
                else
                {
                    entity.Create();
                    IsOk = repositoryfactory.Repository().Insert(entity);
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        #region 获得钢材单价以及折扣率
        /// <summary>
        /// 获得钢材单价以及折扣率
        /// </summary>
        /// <param name="CustomerId">客户ID</param>
        /// <param name="SteelCode">钢种</param>
        /// <param name="Thick">厚</param>
        /// <param name="Weith">宽</param>
        /// <param name="Length">长</param>
        /// <returns></returns>
        public ActionResult GetSteelPrice(string OrderType, string CustomerId, string SteelCode, decimal Thick, decimal Weigh, decimal Length, decimal AddThick, decimal AddWeigh, decimal AddLength, decimal BoreSize, decimal LimitWeight, string Technology, decimal Qty, int GasCuttingSteel, int LiftingHole, int CustMaterial, string BoreSizeAdd)
        {
            Hashtable HashtableData = base_steelpricebll.GetSteelPrice(OrderType, CustomerId, SteelCode, Thick, Weigh, Length, AddThick, AddWeigh, AddLength, BoreSize, LimitWeight, Technology, Qty, GasCuttingSteel, LiftingHole, CustMaterial, BoreSizeAdd);
            return Content(HashtableData.ToJson());
        }

        public ActionResult GetProductName(string OrderType, string CustomerId, string SteelCode, int DD, int CustMaterial)
        {
            Hashtable HashtableData = base_steelpricebll.GetProductName(OrderType, CustomerId, SteelCode, DD,  CustMaterial);
            return Content(HashtableData.ToJson());
        }

        public ActionResult GetProductNameForQ(string Technology, string CustomerId, string QuotationId)
        {
            Hashtable HashtableData = base_steelpricebll.GetProductNameForQ(Technology, CustomerId, QuotationId);
            return Content(HashtableData.ToJson());
        }

        public ActionResult GetAdd(string OrderType, string CustomerId, string SteelCode)
        {
            Hashtable HashtableData = base_steelpricebll.GetAdd( OrderType,  CustomerId,  SteelCode);
            return Content(HashtableData.ToJson());
        }
        #endregion
    }
}