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
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Areas.CommonModule.Controllers
{
    /// <summary>
    /// Base_Supplier控制器
    /// </summary>
    public class Base_SupplierController : PublicController<Base_Supplier>
    {
        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public ActionResult SubmitFormSupplier(Base_Supplier entity, string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                Base_Supplier entityold = DataFactory.Database().FindEntity<Base_Supplier>(KeyValue);
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    entity.Modify(KeyValue);
                    IsOk = repositoryfactory.Repository().Update(entity, isOpenTrans);
                    List<Base_MaterielCheck> Base_MaterielCheck = DataFactory.Database().FindList<Base_MaterielCheck>("CustomerCode", entityold.Code);
                    foreach (Base_MaterielCheck item in Base_MaterielCheck)
                    {
                        item.CustomerCode = entity.Code;
                        item.CustomerName = entity.FullName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<Base_MaterielGet> Base_MaterielGet = DataFactory.Database().FindList<Base_MaterielGet>("CustomerCode", entityold.Code);
                    foreach (Base_MaterielGet item in Base_MaterielGet)
                    {
                        item.CustomerCode = entity.Code;
                        item.CustomerName = entity.FullName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<Base_MaterielPurchase> Base_MaterielPurchase = DataFactory.Database().FindList<Base_MaterielPurchase>("CustomerCode", entityold.Code);
                    foreach (Base_MaterielPurchase item in Base_MaterielPurchase)
                    {
                        item.CustomerCode = entity.Code;
                        item.CustomerName = entity.FullName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<Base_MaterielReturn> Base_MaterielReturn = DataFactory.Database().FindList<Base_MaterielReturn>("CustomerCode", entityold.Code);
                    foreach (Base_MaterielReturn item in Base_MaterielReturn)
                    {
                        item.CustomerCode = entity.Code;
                        item.CustomerName = entity.FullName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<Base_MaterielStorage> Base_MaterielStorage = DataFactory.Database().FindList<Base_MaterielStorage>("CustomerCode", entityold.Code);
                    foreach (Base_MaterielStorage item in Base_MaterielStorage)
                    {
                        item.CustomerCode = entity.Code;
                        item.CustomerName = entity.FullName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<Base_MaterielType> Base_MaterielType = DataFactory.Database().FindList<Base_MaterielType>("CustomerCode", entityold.Code);
                    foreach (Base_MaterielType item in Base_MaterielType)
                    {
                        item.CustomerCode = entity.Code;
                        item.CustomerName = entity.FullName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<Base_PayCompany> Base_PayCompany = DataFactory.Database().FindList<Base_PayCompany>("CustomerCode", entityold.Code);
                    foreach (Base_PayCompany item in Base_PayCompany)
                    {
                        item.CustomerCode = entity.Code;
                        item.CustomerName = entity.FullName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<Base_SteelDDBeout> Base_SteelDDBeout = DataFactory.Database().FindList<Base_SteelDDBeout>("CustomerCode", entityold.Code);
                    foreach (Base_SteelDDBeout item in Base_SteelDDBeout)
                    {
                        item.CustomerCode = entity.Code;
                        item.CustomerName = entity.FullName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<Base_SteelDDPerchase> Base_SteelDDPerchase = DataFactory.Database().FindList<Base_SteelDDPerchase>("CustomerCode", entityold.Code);
                    foreach (Base_SteelDDPerchase item in Base_SteelDDPerchase)
                    {
                        item.CustomerCode = entity.Code;
                        item.CustomerName = entity.FullName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<Base_SteelPurchase> Base_SteelPurchase = DataFactory.Database().FindList<Base_SteelPurchase>("CustomerCode", entityold.Code);
                    foreach (Base_SteelPurchase item in Base_SteelPurchase)
                    {
                        item.CustomerCode = entity.Code;
                        item.CustomerName = entity.FullName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<BY_AdjustQuery> BY_AdjustQuery = DataFactory.Database().FindList<BY_AdjustQuery>("CustomerCode", entityold.Code);
                    foreach (BY_AdjustQuery item in BY_AdjustQuery)
                    {
                        item.CustomerCode = entity.Code;
                        item.CustomerName = entity.FullName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<BY_InStock> BY_InStock = DataFactory.Database().FindList<BY_InStock>("CustomerCode", entityold.Code);
                    foreach (BY_InStock item in BY_InStock)
                    {
                        item.CustomerCode = entity.Code;
                        item.CustomerName = entity.FullName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<BY_Inventory> BY_Inventory = DataFactory.Database().FindList<BY_Inventory>("CustomerCode", entityold.Code);
                    foreach (BY_Inventory item in BY_Inventory)
                    {
                        item.CustomerCode = entity.Code;
                        item.CustomerName = entity.FullName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<BY_PayCheckSheel> BY_PayCheckSheel = DataFactory.Database().FindList<BY_PayCheckSheel>("CustomerCode", entityold.Code);
                    foreach (BY_PayCheckSheel item in BY_PayCheckSheel)
                    {
                        item.CustomerCode = entity.Code;
                        item.CustomerName = entity.FullName;
                        item.PaymentMode = entity.PaymentMode;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<BY_PayReimburseSheet> BY_PayReimburseSheet = DataFactory.Database().FindList<BY_PayReimburseSheet>("CustomerCode", entityold.Code);
                    foreach (BY_PayReimburseSheet item in BY_PayReimburseSheet)
                    {
                        item.CustomerCode = entity.Code;
                        item.CustomerName = entity.FullName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }
                    List<BY_QuotationHeatBeout> BY_QuotationHeatBeout = DataFactory.Database().FindList<BY_QuotationHeatBeout>("CustomerCode", entityold.Code);
                    foreach (BY_QuotationHeatBeout item in BY_QuotationHeatBeout)
                    {
                        item.CustomerCode = entity.Code;
                        item.CustomerName = entity.FullName;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }


                }
                else
                {
                    entity.Create();
                    IsOk = repositoryfactory.Repository().Insert(entity, isOpenTrans);
                }

                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        Base_SupplierBll base_supplierbll = new Base_SupplierBll();
        public ActionResult GetAutoCompleJson(string keywords)
        {
            List<Base_Supplier> ListData = base_supplierbll.GetAutoComple(keywords);
            return Content(ListData.ToJson());
        }
        public ActionResult GetAutoForSupAndCus(string keywords)
        {
            List<Base_Supplier> ListData = base_supplierbll.GetAutoForSupAndCus(keywords);
            return Content(ListData.ToJson());
        }
    }
}