using DNS.Business;
using DNS.Business.Utility.Enum;
using DNS.DataAccess;
using DNS.Entity;
using DNS.Repository;
using DNS.Utilities;
using DNS.WebApp.Areas.DeliveryModule.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using static DNS.Business.BY_InStockBll;

namespace DNS.WebApp.Areas.QuotationModule.Controllers
{
    /// <summary>
    /// 报价单录入
    /// </summary>
    public class QuotationInputController : PublicController<BY_Quotation>
    {
        public virtual ActionResult SetFormForImg(string KeyValue)
        {
            BY_Quotation entity = repositoryfactory.Repository().FindEntity(KeyValue);
            DataTable dt = DataFactory.Database().FindTableBySql("SELECT STUFF((SELECT '，'+bf.Status FROM base_file bf where bf.AttachId = '" + entity.QuotationCode + "' for xml path('')),1,1,'')");
            if (dt != null && dt.Rows.Count > 0)
            {
                entity.ShipDescription = dt.Rows[0][0].ToString();
            }
            return Content(entity.ToJson());
        }

        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult ExcelImportDialog()
        {
            return View();
        }
        Base_ExcelImportBll base_exceliportbll = new Base_ExcelImportBll();
        public ActionResult ImportExel(string QuotationId, int LineCode, int CustMaterial = 0, string FreeBill = "0",string BYD = "0")
        {
            int IsOk = 1;//导入状态
            DataTable Result = new DataTable();//导入错误记录表
            try
            {
                //string moduleId = Request["moduleId"];//表名
                StringBuilder sb_table = new StringBuilder();
                HttpFileCollectionBase files = Request.Files;
                HttpPostedFileBase file = files["filePath"];//获取上传的文件
                string fullname = file.FileName;
                string IsXls = System.IO.Path.GetExtension(fullname).ToString().ToLower();//System.IO.Path.GetExtension获得文件的扩展名
                if (IsXls != ".xls" && IsXls != ".xlsx")
                {
                    IsOk = 0;
                }
                else
                {

                    string filename = Guid.NewGuid().ToString() + ".xls";
                    if (fullname.IndexOf(".xlsx") > 0)
                    {
                        filename = Guid.NewGuid().ToString() + ".xlsx";
                    }
                    if (file != null && file.FileName != "")
                    {
                        string msg = UploadHelper.FileUpload(file, Server.MapPath("~/Resource/UploadFile/ImportExcel/"), filename);
                    }
                    DataTable dt = ImportExcel.ExcelToDataTableNPOI(Server.MapPath("~/Resource/UploadFile/ImportExcel/") + filename);
                    if (BYD == "1")
                    {
                        //IsOk = importexcel(QuotationId, CustMaterial, FreeBill, dt, out Result, LineCode);
                        
                    }
                    else
                    {
                        IsOk = importexcel2(QuotationId, CustMaterial, FreeBill, dt, out Result, LineCode);

                    }
                }

            }
            catch (Exception ex)
            {
               // Base_SysLogBll.Instance.WriteLog("", OperationType.Add, "-1", "异常错误：" + ex.Message);
                IsOk = 0;
            }
            if (Result.Rows.Count > 0)
            {
                IsOk = 0;
            }
            var JsonData = new
            {
                Status = IsOk > 0 ? "true" : "false",
                ResultData = Result
            };
            return Content(JsonData.ToJson());
        }

        public int importexcel2(string QuotationId, int CustMaterial, string FreeBill, DataTable dt, out DataTable Result, int LineCode = 1)
        {
            LineCode = 1;
            //构造导入返回结果表
            DataTable Newdt = new DataTable("Result");
            Newdt.Columns.Add("rowid", typeof(System.String));                 //行号
            Newdt.Columns.Add("locate", typeof(System.String));                 //位置
            Newdt.Columns.Add("reason", typeof(System.String));                 //原因
            int IsOk = 0;
            //获得导入模板
            if (dt != null && dt.Rows.Count > 0)
            {
                IDatabase database = DataFactory.Database();
                DbTransaction isOpenTrans = database.BeginTrans();
                List<BY_QuotationDetail> by_quotationdetails = new List<BY_QuotationDetail>();
                BY_Quotation by_quotation = database.FindEntity<BY_Quotation>(QuotationId);
                database.Delete("BY_QuotationDetail", "QuotationId", by_quotation.QuotationId);
                Base_Customer base_customer = database.FindEntity<Base_Customer>(by_quotation.CustomerId);
                int rowNum = 1;
                int errorNum = 1;
                try
                {
                    #region 遍历Excel数据行
                    BY_QuotationDetail by_quotationdetail;
                    foreach (DataRow item in dt.Rows)
                    {
                        if (item["A"].ToString() == "序号" || item["A"] == DBNull.Value || item["A"] == null || item["A"].ToString() == "")
                        {
                            continue;
                        }

                        //  A序号	B钢种	C光板加工	D厚	E宽	F长	G厚余量	H宽余量	I长余量	J数量	K零件号	L模具编号	M开料备注	N加工备注	O送货备注	P切角(0不，1单边，2双边)	Q钉边编码 /二维码	R热处理	S热处理单价	T热处理重量	U单位	V单价	W几放	X交期	Y加工要求


                        by_quotationdetail = new BY_QuotationDetail();
                        by_quotationdetail.Create();
                        by_quotationdetail.QuotationId = by_quotation.QuotationId;
                        by_quotationdetail.QuotationCode = by_quotation.QuotationCode;
                        by_quotationdetail.LineCode = LineCode;
                        by_quotationdetail.Qty = Convert.ToInt32(item["J"]);




                        if (item["C"].ToString().IndexOf("圆") >= 0 || item["C"].ToString().IndexOf("毛") >= 0 || item["C"].ToString().IndexOf("棒") >= 0)
                        {
                            by_quotationdetail.OrderType = "毛料生产";

                        }
                        else if (item["C"].ToString().IndexOf("精") >= 0)
                        {
                            by_quotationdetail.OrderType = "精板加工";

                        }
                        else if (item["C"].ToString().IndexOf("光") >= 0)
                        {
                            by_quotationdetail.OrderType = "光板加工";
                        }
                        else
                        {
                            by_quotationdetail.OrderType = "毛料生产";
                        }
                        if (by_quotationdetail.OrderType != "毛料生产" && by_quotationdetail.OrderType != "光板加工" && by_quotationdetail.OrderType != "精板加工")
                        {
                            DataRow dr = Newdt.NewRow();
                            dr = Newdt.NewRow();
                            dr[0] = errorNum;
                            dr[1] = "第[" + rowNum.ToString() + "]行[" + "C" + "]";
                            dr[2] = "加工类型不明确！" + item["C"].ToString();
                            Newdt.Rows.Add(dr);
                            database.Rollback();
                            IsOk = -1;
                            break;
                        }

                        Base_Steel base_steel = new Base_Steel();
                        if (!string.IsNullOrEmpty(item["B"].ToString().Trim()))
                        {
                             base_steel = database.FindEntity<Base_Steel>("SteelCode", item["B"].ToString().Trim());
                            if (base_steel.SteelId == null)
                            {
                                DataRow dr = Newdt.NewRow();
                                dr = Newdt.NewRow();
                                dr[0] = errorNum;
                                dr[1] = "第[" + rowNum.ToString() + "]行[" + "B" + "]";
                                dr[2] = "系统找不到该钢种，情新增此钢种！" + item["B"].ToString().Trim();
                                Newdt.Rows.Add(dr);
                                database.Rollback();
                                IsOk = -1;
                                break;
                            }
                            by_quotationdetail.SteelId = base_steel.SteelId;
                            by_quotationdetail.SteelType = base_steel.SteelType;
                            by_quotationdetail.SteelCode = base_steel.SteelCode;
                            by_quotationdetail.SteelDescription = base_steel.SteelName;
                        }


                        by_quotationdetail.Thick = CommonHelper.GetDecimal((item["D"].ToString().Replace("_", "").Trim()));
                        by_quotationdetail.Weigh = CommonHelper.GetDecimal((item["E"].ToString().Replace("_", "").Trim()));
                        by_quotationdetail.Length = CommonHelper.GetDecimal((item["F"].ToString().Replace("_", "").Trim()));

                        string productend = by_quotationdetail.SteelCode.Replace("棒", "圆").Replace("管", "圆");
                        productend = by_quotationdetail.SteelCode.IndexOf("圆") > 0 ? "" : by_quotationdetail.OrderType.Substring(0, 2);

                        //if (string.IsNullOrEmpty(by_quotationdetail.ProductName))
                        //{
                        //    by_quotationdetail.ProductName = by_quotationdetail.SteelCode + productend;
                        //}

                        //if (by_quotationdetail.Weigh == 0)
                        //{
                        //    by_quotationdetail.ProductName = by_quotationdetail.ProductName.Replace("毛料","");
                        //}
                        //by_quotationdetail.ProductName = by_quotationdetail.SteelCode + by_quotationdetail.OrderType.Substring(0, 2);
                        //by_quotationdetail.ProductName = by_quotationdetail.ProductName.Replace("光光","光").Replace("毛毛", "毛").Replace("精精", "精");
                        by_quotationdetail.ProductName = by_quotationdetail.SteelCode;
                        by_quotationdetail.AddThick = CommonHelper.GetDecimal((item["G"].ToString().Replace("_", "").Trim()));
                        by_quotationdetail.AddWeigh = CommonHelper.GetDecimal((item["H"].ToString().Replace("_", "").Trim()));
                        by_quotationdetail.AddLength = CommonHelper.GetDecimal((item["I"].ToString().Replace("_", "").Trim()));

                        if (by_quotationdetail.OrderType != "毛料生产")
                        {
                            by_quotationdetail.LimitWeight = base_customer.LimitWeight;
                            
                        }
                        else
                        {
                            by_quotationdetail.LimitWeight = 0;
                        }
                        by_quotationdetail.Technology = item["R"].ToString().Trim();
                        by_quotationdetail.HeatPrice = CommonHelper.GetDecimal((item["S"].ToString().Replace("_", "").Trim()));
                        by_quotationdetail.Weight = CommonHelper.GetDecimal((item["T"].ToString().Replace("_", "").Trim()));
                        //by_quotationdetail.BoreSize = CommonHelper.GetDecimal((item["W"].ToString().Replace("_", "").Trim()));
                        //W几放	X交期	Y加工要求
                        by_quotationdetail.Fang = item["W"].ToString();
                        by_quotationdetail.Delivery = item["X"].ToString();
                        by_quotationdetail.Kprocess = item["Y"].ToString();
                        Base_DataDictionaryBll base_datadictionarybll = new Base_DataDictionaryBll();
                        List<Base_DataDictionaryDetail> list = base_datadictionarybll.GetDataDictionaryDetailListByCode("加工流程");
                        if (string.IsNullOrWhiteSpace(by_quotationdetail.Kprocess) || list.Where(p=>p.Code == by_quotationdetail.Kprocess).Count() == 0)
                        {
                            throw new InvalidOperationException("加工要求系统里没有维护！");
                        }

                        new QuotationPlaceOrderController().GentQuotationDetailProcess(by_quotation, by_quotationdetail, new List<BY_OrderDetailProcess>(), "", isOpenTrans);

                        //by_quotationdetail.HeatAmount = by_quotationdetail.HeatPrice * by_quotationdetail.Weight;
                        Base_SteelPriceBll base_steelpricebll = new Base_SteelPriceBll();
                        Hashtable HashtableData =
            base_steelpricebll.GetSteelPrice(by_quotationdetail.OrderType, by_quotation.CustomerId
                            , base_steel.SteelCode, CommonHelper.GetDecimal(by_quotationdetail.Thick), CommonHelper.GetDecimal(by_quotationdetail.Weigh), CommonHelper.GetDecimal(by_quotationdetail.Length)
                            , CommonHelper.GetDecimal(by_quotationdetail.AddThick), CommonHelper.GetDecimal(by_quotationdetail.AddWeigh), CommonHelper.GetDecimal(by_quotationdetail.AddLength), CommonHelper.GetDecimal(by_quotationdetail.BoreSize), CommonHelper.GetDecimal(by_quotationdetail.LimitWeight), "", CommonHelper.GetDecimal(by_quotationdetail.Qty), 0, 0, CustMaterial, "0");

                        if (item["T"].ToString().Replace("_", "").Trim() == "" )
                        {
                            by_quotationdetail.Weight = CommonHelper.GetDecimal(HashtableData["Weight"]) * by_quotationdetail.Qty;
                        }
                        
                        by_quotationdetail.TheoryWeight = CommonHelper.GetDecimal(HashtableData["TheoryWeight"]) * by_quotationdetail.Qty;
                        by_quotationdetail.WorkWeight = CommonHelper.GetDecimal(HashtableData["WorkWeight"]) * by_quotationdetail.Qty;
                        by_quotationdetail.JJtype = item["U"].ToString().Trim();//"按公斤";
                        if (CommonHelper.GetDecimal((item["V"].ToString().Replace("_", "").Trim())) > 0)
                        {
                            by_quotationdetail.MaterialPrice = CommonHelper.GetDecimal((item["V"].ToString().Replace("_", "").Trim()));
                        }
                        else
                        {
                            by_quotationdetail.MaterialPrice = CommonHelper.GetDecimal(HashtableData["MaterialPrice"]);
                        }

                        if (by_quotationdetail.JJtype == "按件数")
                        {
                            by_quotationdetail.JJtype = "按件数";
                            by_quotationdetail.MaterialAmount = by_quotationdetail.MaterialPrice * by_quotationdetail.Qty;
                        }
                        else
                        {
                            by_quotationdetail.JJtype = "按公斤";
                            if (base_customer.Calc == 0 || by_quotationdetail.OrderType == "毛料生产")
                            {
                                by_quotationdetail.MaterialAmount = by_quotationdetail.MaterialPrice * by_quotationdetail.Weight;
                            }
                            else
                            {
                                by_quotationdetail.MaterialAmount = by_quotationdetail.MaterialPrice * by_quotationdetail.TheoryWeight;

                            }
                        }
                        /// A序号	B钢种	C加工类型	D厚	E宽	F长	G厚余量	H宽余量	I长余量	 J数量	K客户品名	L模具编号	M开料备注	N加工备注	O配件号	 P切角(0不，1单边，2双边)

                        if (by_quotationdetail.HeatPrice > 0)
                        {
                            by_quotationdetail.MaterialPrice = 0;
                            by_quotationdetail.MaterialAmount = 0;
                            if (by_quotationdetail.JJtype != "按公斤")
                            {
                                by_quotationdetail.HeatAmount = by_quotationdetail.HeatPrice * by_quotationdetail.Qty;
                            }
                            else
                            {
                                by_quotationdetail.HeatAmount = by_quotationdetail.HeatPrice * by_quotationdetail.Weight;
                            }
                        }
                        else
                        {
                            by_quotationdetail.Technology = "";
                            by_quotationdetail.HeatAmount = 0;
                        }

                        by_quotationdetail.DingCode = item["Q"].ToString().Trim();
                        by_quotationdetail.Remark = item["N"].ToString().Trim();
                        by_quotationdetail.RemarkKL = item["M"].ToString().Trim();
                        by_quotationdetail.CustomerProduct = item["L"].ToString().Trim();
                        by_quotationdetail.ProjectCode = item["K"].ToString().Trim();
                        by_quotationdetail.ShipDescription = item["O"].ToString().Trim();

                        //  SmallCost kg  与   WorkPrice ps 写反了
                        by_quotationdetail.SmallCost = CommonHelper.GetDecimal(HashtableData["SmallCost"]);

                        if (by_quotationdetail.WorkWeight > by_quotationdetail.TheoryWeight)
                        {
                            by_quotationdetail.SmallAmount = CommonHelper.GetDecimal(HashtableData["SmallCost"]) * by_quotationdetail.WorkWeight;

                        }
                        else
                        {
                            by_quotationdetail.SmallAmount = CommonHelper.GetDecimal(HashtableData["SmallCost"]) * by_quotationdetail.TheoryWeight;

                        }

                        by_quotationdetail.WorkPrice = CommonHelper.GetDecimal(HashtableData["WorkPrice"]);
                        by_quotationdetail.WorkAmount = by_quotationdetail.WorkPrice * by_quotationdetail.Qty;

                        if (item["P"].ToString() == "0")
                        {
                            by_quotationdetail.DDCostPer = 0;
                            
                        }
                        if (item["P"].ToString() == "1")
                        {
                            by_quotationdetail.DDCostPer = CommonHelper.GetDecimal(HashtableData["DDCostPer"]) / 2;

                        }
                        by_quotationdetail.JKCostPer = CommonHelper.GetDecimal(HashtableData["JKCostPer"]);

                        by_quotationdetail.JKCost = by_quotationdetail.JKCostPer * by_quotationdetail.Qty;

                        by_quotationdetail.DDCostPer = 0;
                        if (item["P"].ToString() == "2")
                        {
                            by_quotationdetail.DDCostPer = CommonHelper.GetDecimal(HashtableData["DDCostPer"]);

                        }
                        by_quotationdetail.DDCost = by_quotationdetail.DDCostPer * by_quotationdetail.Qty;

                        if (by_quotationdetail.OrderType == "毛料生产")
                        {
                            by_quotationdetail.WorkAmount = 0;
                            by_quotationdetail.SmallAmount = 0;
                        }
                        by_quotationdetail.SalesAmount = Math.Round(Convert.ToDecimal(by_quotationdetail.WorkAmount), 2) + Math.Round(Convert.ToDecimal( by_quotationdetail.MaterialAmount),2)
                            + Math.Round(Convert.ToDecimal(by_quotationdetail.SmallAmount), 2) + Math.Round(Convert.ToDecimal(by_quotationdetail.HeatAmount), 2)+
                            Math.Round(Convert.ToDecimal(by_quotationdetail.DDCost), 2) + Math.Round(Convert.ToDecimal(by_quotationdetail.JKCost), 2);
                        by_quotationdetails.Add(by_quotationdetail);
                        LineCode = LineCode + 1;
                        rowNum += 1;
                    }
                    #endregion 遍历Excel数据行结束
                    by_quotation.TotalAmount = by_quotationdetails.Sum(p => p.SalesAmount);
                    by_quotation.Qty = by_quotationdetails.Sum(p => p.Qty);
                    database.Update(by_quotation, isOpenTrans);
                    database.Insert(by_quotationdetails, isOpenTrans);
                    if (IsOk == -1)
                    {
                        database.Rollback();
                    }
                    else
                    {
                        database.Commit();
                    }
                    IsOk = 1;
                }
                catch (System.Exception ex)
                {
                    DataRow dr = Newdt.NewRow();
                    dr = Newdt.NewRow();
                    dr[0] = errorNum;
                    dr[1] = "第[" + rowNum.ToString() + "]行[" + "" + "]";
                    dr[2] = ex.Message;
                    Newdt.Rows.Add(dr);
                    database.Rollback();
                    IsOk = -1;
                }

            }

            Result = Newdt;
            return IsOk;
        }


        [HttpPost]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult Delete1(string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                int IsOk = 1;
                BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(KeyValue);
                List<BY_QuotationDetail> by_quotationdetails = DataFactory.Database().FindList<BY_QuotationDetail>("QuotationId", by_quotation.QuotationId);

                if (!string.IsNullOrEmpty(by_quotation.OrderCode))
                {
                    BY_Order by_order = DataFactory.Database().FindEntity<BY_Order>("OrderCode", by_quotation.OrderCode);
                    List<BY_OrderDetail> by_orderdetails = DataFactory.Database().FindList<BY_OrderDetail>("OrderId", by_order.OrderId);
                    //DataFactory.Database().Delete(by_order, isOpenTrans);
                    by_order.DeleteMark = 1;
                    by_order.Modify(by_order.OrderId);
                    DataFactory.Database().Update(by_order, isOpenTrans);
                    foreach (var item in by_orderdetails)
                    {
                        //DataFactory.Database().Delete(item, isOpenTrans);
                    }
                }

                BY_Delivery by_delivery = DataFactory.Database().FindEntity<BY_Delivery>("QuotationCode", by_quotation.QuotationCode);
                if (!string.IsNullOrEmpty(by_delivery.DeliveryId))
                {
                    List<BY_DeliveryDetail> by_deliverydetails = DataFactory.Database().FindList<BY_DeliveryDetail>("DeliveryId", by_delivery.DeliveryId);
                    //DataFactory.Database().Delete(by_delivery, isOpenTrans);
                    by_delivery.DeleteMark = 1;
                    by_delivery.Modify(by_delivery.DeliveryId);
                    DataFactory.Database().Update(by_delivery, isOpenTrans);
                    if (!string.IsNullOrEmpty(by_delivery.CheckSheetCode))
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：已生成对账单，无法退回！" }.ToString());
                    }
                    if (by_delivery.GetedAmount > 0)
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：已收款，无法退回！" }.ToString());
                    }
                    foreach (var item in by_deliverydetails)
                    {
                        //DataFactory.Database().Delete(item, isOpenTrans);
                    }
                }

                //DataFactory.Database().Delete(by_quotation, isOpenTrans);
                by_quotation.Modify(by_quotation.QuotationId);
                by_quotation.DeleteMark = 1;
                DataFactory.Database().Update(by_quotation, isOpenTrans);
                foreach (var item in by_quotationdetails)
                {
                    //DataFactory.Database().Delete(item, isOpenTrans);
                }

                DataFactory.Database().Commit();
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "删除成功！" }.ToString());
            }
            catch (Exception ex)
            {
                DataFactory.Database().Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }


        //string ModuleId = "";

        BY_QuotationDetailBll by_quotationdetailbll = new BY_QuotationDetailBll();
        Base_CodeRuleBll base_coderulebll = new Base_CodeRuleBll();

        [HttpPost]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult DeleteQ(string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    List<BY_QuotationDetail> by_quotationdetails = DataFactory.Database().FindList<BY_QuotationDetail>("QuotationId", KeyValue);
                    if (by_quotationdetails.Count != 0)
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：此单有明细行，不能删除！" }.ToString());
                    }
                    BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(KeyValue);
                    DataFactory.Database().Delete(by_quotation, isOpenTrans);
                    if (!string.IsNullOrEmpty(by_quotation.OrderCode))
                    {
                        BY_Order by_order = DataFactory.Database().FindEntity<BY_Order>("OrderCode", by_quotation.OrderCode);
                        DataFactory.Database().Delete(by_order, isOpenTrans);
                    }
                    database.Commit();
                    return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功" }.ToString());
                }
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "删除失败!" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "删除失败。" + ex.Message }.ToString());
            }
        }
       

        [HttpPost]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult ToDelivery(string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    List<BY_QuotationDetail> by_quotationdetails = DataFactory.Database().FindList<BY_QuotationDetail>("QuotationId", KeyValue);
                    BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(KeyValue);

                    DataTable ListData = GetAutoCompleForXY(by_quotation.CustomerCode);

                    if (ListData.Rows.Count > 0 && by_quotation.TotalAmount > Convert.ToDecimal(ListData.Rows[0]["XYSy"]))
                    {
                        return Content(new JsonMessage { Success = true, Code = "1", Message = "此客信用额度还剩" + ListData.Rows[0]["XYSy"] + "，需主管审核！" }.ToString());
                    }

                    BY_Delivery by_delivery = new Entity.BY_Delivery();
                    CommonHelper.CopySameFieldsObjectALL<BY_Quotation, BY_Delivery>(by_quotation, ref by_delivery);
                    by_delivery.DeliveryCode = by_quotation.QuotationCode ;
                    by_delivery.Remark = by_quotation.Remark;
                    by_delivery.Enabled = 1;
                    by_delivery.Weight = by_quotationdetails.Where(p => p.Length > 0).Sum<BY_QuotationDetail>(m => m.Weight); ;
                    by_delivery.Create();

                    List<BY_DeliveryDetail> by_deliverydetails = new List<BY_DeliveryDetail>();
                    BY_DeliveryDetail by_deliverydetail;
                    foreach (var item in by_quotationdetails)
                    {
                        by_deliverydetail = new BY_DeliveryDetail();
                        CommonHelper.CopySameFieldsObjectALL<BY_QuotationDetail, BY_DeliveryDetail>(item, ref by_deliverydetail);
                        by_deliverydetail.DeliveryId = by_delivery.DeliveryId;
                        by_deliverydetail.TheoryWeight = item.TheoryWeight;
                        by_deliverydetail.Enabled = 1;
                        by_deliverydetail.SortCode = item.LineCode;
                        by_deliverydetail.BYDUnit = item.BYDUnit;
                        by_deliverydetail.BYDNum = item.BYDNum;
                        by_deliverydetail.DeliveryCode = by_delivery.DeliveryCode;
                        by_deliverydetail = new InventoryComfirmController().ReturnCost(by_deliverydetail, by_quotation.CustomerId, "",by_quotation.CustMaterial, by_quotation.LType, database);
                        by_deliverydetail.Create();
                        DataFactory.Database().Insert(by_deliverydetail, isOpenTrans);
                    }
                    if (by_quotation.BillStatus == "已落单")
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "提交失败：已经提交，不能多次提交！" }.ToString());
                    }
                    by_quotation.IsMobileInput = 1;
                    by_quotation.BillStatus = QuotationBillStatus.已落单.ToString();
                    by_quotation.CheckStatus = 1;
                    by_quotation.CheckDate = DateTime.Now;
                    by_quotation.CheckUserId = ManageProvider.Provider.Current().UserName;
                    by_quotation.SureDate = DateTime.Now;
                    by_quotation.SureUserId = ManageProvider.Provider.Current().UserName;
                    by_quotation.PlaceOrderDate = DateTime.Now;
                    by_quotation.PlaceOrderUserId = ManageProvider.Provider.Current().UserName;

                    DataFactory.Database().Update(by_quotation, isOpenTrans);
                    DataFactory.Database().Insert(by_delivery, isOpenTrans);
                    database.Commit();
                    return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功" }.ToString());
                }
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "删除失败!" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "删除失败。" + ex.Message }.ToString());
            }
        }

        #region 主表
        /// <summary>
        /// 获取自动单据编码
        /// </summary>
        /// <returns></returns>
        private string BillCode(string ModuleId)
        {
            string UserId = ManageProvider.Provider.Current().UserId;
            return base_coderulebll.GetBillCode(UserId, ModuleId);
        }
        /// <summary>
        /// 报价单录入视图页面
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public override ActionResult Index()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult ChangeRemark()
        {
            return View();
        }

        /// <summary>
        /// 保存报价单主表
        /// </summary>
        /// <param name="KeyValue">主键</param>
        /// <param name="entity">对象实体</param>
        /// <returns></returns>
        public ActionResult SubmitQuotationInputForm(string KeyValue, BY_Quotation entity)
        {
            string ModuleId = "GC";
            if (entity.LType == "LJ")
            {
                ModuleId = "LJ";
            }

            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                Base_Customer c = database.FindEntity<Base_Customer>("CustomerId", entity.CustomerId);
                entity.CustomerName = c.FullName;
                entity.ShortName = c.ShortName;
                entity.CustomerCode = c.Code;

                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    entity.Modify(KeyValue);
                    database.Update(entity, isOpenTrans);

                    BY_Order by_order = DataFactory.Database().FindEntity<BY_Order>("QuotationId", entity.QuotationId);
                    if (by_order != null && by_order.OrderId != null)
                    {
                        by_order.CustomerPO = entity.CustomerPO;
                        by_order.MnemonicCode = entity.MnemonicCode;
                        by_order.QuotationId = entity.QuotationId;
                        by_order.CustomerId = entity.CustomerId;
                        by_order.CustomerCode = entity.CustomerCode;
                        by_order.CustomerName = entity.CustomerName;
                        by_order.ShortName = entity.ShortName;
                        by_order.ContactName = entity.ContactName;
                        by_order.ContactTel = entity.ContactTel;
                        by_order.ContactFax = entity.ContactFax;
                        by_order.ContactMail = entity.ContactMail;
                        by_order.MobileCode = entity.MobileCode;
                        by_order.CustomerAddress = entity.CustomerAddress;
                        //by_order.OrderType = by_quotation.OrderType;
                        by_order.CustomerPO = entity.CustomerPO;
                        by_order.CheckStatus = entity.CheckStatus;
                        by_order.CheckUserId = entity.CheckUserId;
                        by_order.CheckDate = entity.CheckDate;
                        by_order.SalesMan = entity.SalesMan;
                        by_order.IsInvoice = entity.IsInvoice;
                        
                        by_order.DepartName = entity.DepartName;
                        by_order.KLRequery = entity.KLRequery;
                        by_order.XMRequery = entity.XMRequery;
                        by_order.LType = entity.LType;
                        by_order.CompanyName = entity.CompanyName;

                        by_order.Delivery = entity.Delivery;
                        by_order.QuotationCode = entity.QuotationCode;
                        by_order.LJPlan = entity.LJPlan;

                        by_order.IsReturn = entity.IsReturn;

                        by_order.Remark = entity.Remark;
                        //by_order.BuildNum = entity.BuildNum;
                        //by_order.ProductCode = entity.ProductCode;
                        //by_order.BuildName = entity.BuildName;
                        //by_order.DieName = entity.DieName;
                        //by_order.PicCode = entity.PicCode;
                        //by_order.DieCode = entity.DieCode;



                        DataFactory.Database().Update(by_order, isOpenTrans);
                    }
                    List<BY_Delivery> by_deliverys = DataFactory.Database().FindList<BY_Delivery>("QuotationCode", entity.QuotationCode);
                    foreach (BY_Delivery by_delivery in by_deliverys)
                    {
                        by_delivery.ShipClause = entity.ShipClause;
                        by_delivery.ShipDescription = entity.ShipDescription;
                        by_delivery.Remark = entity.Remark;
                        by_delivery.MnemonicCode = entity.MnemonicCode;

                        by_delivery.CustomerPO = entity.CustomerPO;
                        by_delivery.MnemonicCode = entity.MnemonicCode;
                        by_delivery.CustomerId = entity.CustomerId;
                        by_delivery.CustomerCode = entity.CustomerCode;
                        by_delivery.CustomerName = entity.CustomerName;
                        by_delivery.ShortName = entity.ShortName;
                        by_delivery.ContactName = entity.ContactName;
                        by_delivery.ContactTel = entity.ContactTel;
                        by_delivery.ContactFax = entity.ContactFax;
                        by_delivery.ContactMail = entity.ContactMail;
                        by_delivery.MobileCode = entity.MobileCode;
                        by_delivery.CustomerAddress = entity.CustomerAddress;
                        //by_order.OrderType = by_quotation.OrderType;
                        by_delivery.CustomerPO = entity.CustomerPO;
                        by_delivery.SalesMan = entity.SalesMan;
                        by_delivery.IsInvoice = entity.IsInvoice;

                        by_delivery.CustomerPO = entity.CustomerPO;
                        by_delivery.ShipClause = entity.ShipClause;
                        by_delivery.ShipDescription = entity.ShipDescription;
                        by_delivery.Remark = entity.Remark;
                        by_delivery.MnemonicCode = entity.MnemonicCode;

                        by_delivery.DepartName = entity.DepartName;
                        by_delivery.KLRequery = entity.KLRequery;
                        by_delivery.XMRequery = entity.XMRequery;
                        by_delivery.LType = entity.LType;
                        by_delivery.CompanyName = entity.CompanyName;
                        by_delivery.LJPlan = entity.LJPlan;

                        by_delivery.Remark = entity.Remark;
                        by_delivery.BuildNum = entity.BuildNum;
                        by_delivery.ProductCode = entity.ProductCode;
                        by_delivery.BuildName = entity.BuildName;
                        by_delivery.DieName = entity.DieName;
                        by_delivery.PicCode = entity.PicCode;
                        by_delivery.DieCode = entity.DieCode;
                        by_delivery.IsReturn = entity.IsReturn;

                        
                        DataFactory.Database().Update(by_delivery, isOpenTrans);
                    }
                }
                else
                {
                    entity.Create();
                    if (String.IsNullOrWhiteSpace(entity.LType))
                    {
                        entity.LType = "GC";
                    }
                    entity.QuotationCode = ModuleId + this.BillCode("LType");
                    if (entity.MnemonicCode == "" || entity.MnemonicCode == "默认为报价单号" || entity.MnemonicCode == null)
                    {
                        entity.MnemonicCode = entity.QuotationCode;
                    }
                    database.Insert(entity, isOpenTrans);
                    base_coderulebll.OccupyBillCode(ManageProvider.Provider.Current().UserId, ModuleId, isOpenTrans);
                }
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Attribute = entity.QuotationId, Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        /// <summary>
        /// 提交到审核状态
        /// </summary>
        /// <param name="KeyValue">报价单主键</param>
        /// <param name="CheckStatus">审核标识</param>
        /// <returns></returns>
        public ActionResult SubmitAuditRun(string KeyValue, int CheckStatus)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                #region 落单生成订单
                //报价单主表
                BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(KeyValue);
                //entity.QuotationId = KeyValue;
                //entity.BillStatus = QuotationBillStatus.待审核.ToString();
                //entity.CheckStatus = CheckStatus;


                Base_Customer Base_Customer = DataFactory.Database().FindEntity<Base_Customer>(by_quotation.CustomerId);
                if (Base_Customer.Category.IndexOf("冻结") >= 0)
                {
                    database.Rollback();
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "提交失败：此账户已冻结！" }.ToString());
                }
                if (by_quotation.BillStatus == "已落单")
                {
                    database.Rollback();
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "提交失败：已经提交，不能多次提交！" }.ToString());
                }
                DataTable ListData = GetAutoCompleForXY(by_quotation.CustomerCode);

                if (ListData.Rows.Count > 0 && by_quotation.TotalAmount > Convert.ToDecimal(ListData.Rows[0]["XYSy"]))
                {
                    //BY_Quotation entity = new BY_Quotation();
                    //entity.QuotationId = KeyValue;
                    //entity.BillStatus = QuotationBillStatus.待审核.ToString();
                    //entity.CheckStatus = CheckStatus;
                    //entity.CheckDate = DateTime.Now;
                    //entity.CheckUserId = ManageProvider.Provider.Current().UserName;
                    //database.Update(entity, isOpenTrans);
                    //database.Commit();

                    return Content(new JsonMessage { Success = true, Code = "1", Message = "此客信用额度还剩" + ListData.Rows[0]["XYSy"] + "，需主管审核！" }.ToString());

                }
                if (true)
                {
                    by_quotation.BillStatus = QuotationBillStatus.已落单.ToString();
                    by_quotation.CheckStatus = 1;
                    by_quotation.CheckDate = DateTime.Now;
                    by_quotation.CheckUserId = ManageProvider.Provider.Current().UserName;
                    by_quotation.SureDate = DateTime.Now;
                    by_quotation.SureUserId = ManageProvider.Provider.Current().UserName;

                    //报价单明细列表
                    List<BY_QuotationDetail> QuotationDetailList = DataFactory.Database().FindList<BY_QuotationDetail>("QuotationId", KeyValue);
                    //订单单号
                    //string OrderCode = BillCode("M");
                    List<BY_OrderDetail> by_orderdetails = new List<BY_OrderDetail>();

                    #region 改变报价单落单 状态
                    by_quotation.BillStatus = QuotationBillStatus.已落单.ToString();
                    by_quotation.PlaceOrderDate = DateTime.Now;
                    by_quotation.PlaceOrderUserId = ManageProvider.Provider.Current().UserName;
                    by_quotation.OrderCode = by_quotation.QuotationCode.Substring(2, 4) + "-" + by_quotation.QuotationCode.Substring(9, 4);

                    List<BY_OrderDetailProcess> by_orderdetailprocesss = DataFactory.Database().FindList<BY_OrderDetailProcess>("OrderCode", by_quotation.OrderCode);

                    database.Update(by_quotation, isOpenTrans);
                    #endregion

                    #region 生成订单主表
                    BY_Order by_order = new BY_Order();
                    by_order.Create();
                    by_order.OrderStatus = OrderBillStatus.正常生产.ToString();
                    by_order.QuotationId = by_quotation.QuotationId;
                    by_order.CustomerId = by_quotation.CustomerId;
                    by_order.CustomerCode = by_quotation.CustomerCode;
                    by_order.CustomerName = by_quotation.CustomerName;
                    by_order.ShortName = by_quotation.ShortName;
                    by_order.ContactName = by_quotation.ContactName;
                    by_order.ContactTel = by_quotation.ContactTel;
                    by_order.ContactFax = by_quotation.ContactFax;
                    by_order.ContactMail = by_quotation.ContactMail;
                    by_order.MobileCode = by_quotation.MobileCode;
                    by_order.CustomerAddress = by_quotation.CustomerAddress;
                    //by_order.OrderType = by_quotation.OrderType;
                    by_order.OrderCode = by_quotation.OrderCode;
                    by_order.Qty = by_quotation.Qty;
                    by_order.Amount = by_quotation.TotalAmount;
                    by_order.CustomerPO = by_quotation.CustomerPO;
                    by_order.CheckStatus = by_quotation.CheckStatus;
                    by_order.CheckUserId = by_quotation.CheckUserId;
                    by_order.CheckDate = by_quotation.CheckDate;
                    by_order.PlaceOrderDate = DateTime.Now;
                    by_order.PlaceOrderUserId = ManageProvider.Provider.Current().UserName;
                    by_order.SalesMan = by_quotation.SalesMan;
                    by_order.IssuedUserId = ManageProvider.Provider.Current().UserName;
                    by_order.IssuedDate = DateTime.Now;
                    by_order.MnemonicCode = by_quotation.MnemonicCode;
                    by_order.Delivery = by_quotation.Delivery;

                    by_order.DepartName = by_quotation.DepartName;
                    by_order.KLRequery = by_quotation.KLRequery;
                    by_order.XMRequery = by_quotation.XMRequery;
                    by_order.LType = by_quotation.LType;
                    by_order.CompanyName = by_quotation.CompanyName;

                    by_order.QuotationCode = by_quotation.QuotationCode;
                    by_order.Remark = by_quotation.Remark;
                    by_order.IsReturn = by_quotation.IsReturn;
                    by_order.DeleteMark = 0;
                    database.Insert(by_order, isOpenTrans);
                    //base_coderulebll.OccupyBillCode(ManageProvider.Provider.Current().UserId, "M", isOpenTrans);

                    foreach (BY_QuotationDetail QuotationDetail in QuotationDetailList)
                    {
                        BY_OrderDetail by_orderdetail = new BY_OrderDetail();
                        //气割，吊孔不生成订单
                        if (QuotationDetail.Length == 0)
                        {
                            by_orderdetail.CompleteQty = QuotationDetail.Qty;
                            by_orderdetail.BillStatus = "已完工";
                        }
                        else
                        {
                            by_orderdetail.BillStatus = "正常生产";
                        }
                        #region 订单明细表

                        by_orderdetail.Create();
                        by_orderdetail.OrderId = by_order.OrderId;
                        by_orderdetail.OrderCode = by_order.OrderCode;
                        //by_orderdetail.IsChamfer = QuotationDetail.IsChamfer;
                        by_orderdetail.IsGrindAngle = QuotationDetail.IsGrindAngle;
                        by_orderdetail.LineCode = QuotationDetail.LineCode;
                        by_orderdetail.BoardId = QuotationDetail.BoardId;
                        by_orderdetail.BoardDescription = QuotationDetail.BoardDescription;
                        by_orderdetail.Qty = QuotationDetail.Qty;
                        by_orderdetail.SteelId = QuotationDetail.SteelId;
                        by_orderdetail.SteelCode = QuotationDetail.SteelCode;
                        by_orderdetail.SteelDescription = QuotationDetail.SteelDescription;
                        by_orderdetail.SteelOtherDescription = QuotationDetail.SteelOtherDescription;
                        by_orderdetail.SteelState = QuotationDetail.SteelState;
                        by_orderdetail.DieCode = QuotationDetail.DieCode;
                        by_orderdetail.Thick = QuotationDetail.Thick;
                        by_orderdetail.Weigh = QuotationDetail.Weigh;
                        by_orderdetail.Length = QuotationDetail.Length;
                        by_orderdetail.ExecRemark = QuotationDetail.Remark;
                        by_orderdetail.Weight = QuotationDetail.Weight;
                        by_orderdetail.QuotationDetailId = QuotationDetail.QuotationDetailId;
                        by_orderdetail.OrderType = QuotationDetail.OrderType;
                        by_orderdetail.RemarkKL = QuotationDetail.RemarkKL;
                        by_orderdetail.DingCode = QuotationDetail.DingCode;
                        by_orderdetail.TheoryWeight = QuotationDetail.TheoryWeight;

                        if (by_quotation.CustMaterial == 1 || QuotationDetail.Length == 0)
                        {
                            by_orderdetail.OpenQty = QuotationDetail.Qty;
                            by_orderdetail.WeightFact = QuotationDetail.Weight;
                        }

                        by_orderdetail.RepalceMl = QuotationDetail.RepalceMl;
                        by_orderdetail.ShipDescription = QuotationDetail.ShipDescription;
                        by_orderdetail.CustomerProduct = QuotationDetail.CustomerProduct;
                        by_orderdetail.ProjectCode = QuotationDetail.ProjectCode;

                        
                        by_orderdetail.ProductName = QuotationDetail.ProductName;
                        by_orderdetail.LiftingHole = QuotationDetail.LiftingHole;
                        by_orderdetail.BoreSize = QuotationDetail.BoreSize;
                        by_orderdetail.SteelType = QuotationDetail.SteelType;

                        by_orderdetail.MandXSteel = QuotationDetail.MandXSteel;
                        by_orderdetail.MorXSteel = QuotationDetail.MorXSteel;
                        by_orderdetail.BSteel = QuotationDetail.BSteel;
                        by_orderdetail.MianCostArea = QuotationDetail.MianCostArea;
                        by_orderdetail.MianCostPer = QuotationDetail.MianCostPer;
                        by_orderdetail.MianCost = QuotationDetail.MianCost;
                        by_orderdetail.BianCostLong = QuotationDetail.BianCostLong;
                        by_orderdetail.BianCostPer = QuotationDetail.BianCostPer;
                        by_orderdetail.BianCost = QuotationDetail.BianCost;
                        by_orderdetail.JKCost = QuotationDetail.JKCost;
                        by_orderdetail.SmallCost = QuotationDetail.SmallCost;
                        by_orderdetail.WorkCostForWork = QuotationDetail.WorkCostForWork;
                        by_orderdetail.AddLength = QuotationDetail.AddLength;
                        by_orderdetail.AddThick = QuotationDetail.AddThick;
                        by_orderdetail.AddWeigh = QuotationDetail.AddWeigh;
                        by_orderdetail.JBSteel = QuotationDetail.JBSteel;
                        by_orderdetail.MianCostUnit = QuotationDetail.MianCostUnit;
                        by_orderdetail.BianCostUnit = QuotationDetail.BianCostUnit;
                        by_orderdetail.WorkUnit = QuotationDetail.WorkUnit;
                        by_orderdetail.XorMSteel = QuotationDetail.XorMSteel;

                        by_orderdetail.MianType = QuotationDetail.MianType;
                        by_orderdetail.BianType = QuotationDetail.BianType;
                        by_orderdetail.CountShow = QuotationDetail.CountShow;

                        by_orderdetail.MianCostWork = QuotationDetail.MianCostWork;
                        by_orderdetail.BianCostWork = QuotationDetail.BianCostWork;

                        by_orderdetail.CNC = QuotationDetail.CNC;
                        by_orderdetail.YM = QuotationDetail.YM;
                        by_orderdetail.JQ = QuotationDetail.JQ;
                        by_orderdetail.Kprocess = QuotationDetail.Kprocess;

                        by_orderdetail.JKCostPer = QuotationDetail.JKCostPer;
                        by_orderdetail.JKCostQty = QuotationDetail.JKCostQty;

                        by_orderdetail.Technology = QuotationDetail.Technology;

                        by_orderdetail.SmallAmount = QuotationDetail.SmallAmount;
                        by_orderdetail.WorkWeight = QuotationDetail.WorkWeight;
                        by_orderdetail.Fang = QuotationDetail.Fang;
                        by_orderdetail.Brand = QuotationDetail.Brand;

                        by_orderdetail.BYDUnit = QuotationDetail.BYDUnit;
                        by_orderdetail.BYDNum = QuotationDetail.BYDNum;
                        by_orderdetail.ProductName1 = QuotationDetail.ProductName1;
                        by_orderdetail.Delivery = QuotationDetail.Delivery;

                        by_orderdetail.KSize = QuotationDetail.KSize;
                        by_orderdetail.ZSize = QuotationDetail.ZSize;
                        by_orderdetail.STSize = QuotationDetail.STSize;
                        by_orderdetail.Hardness = QuotationDetail.Hardness;
                        by_orderdetail.NowStatus = QuotationDetail.NowStatus;

                        by_orderdetail.LJtype1 = QuotationDetail.LJtype1;
                        by_orderdetail.LJtype2 = QuotationDetail.LJtype2;
                        by_orderdetail.LJsize = QuotationDetail.LJsize;
                        by_orderdetail.DJ = QuotationDetail.DJ;

                        by_orderdetails.Add(by_orderdetail);
                        database.Insert(by_orderdetail, isOpenTrans);


                        #region 费用写到完工表
                        if (by_orderdetail.Length == 0)
                        {
                            BY_OrderComplete entity = DataFactory.Database().FindEntity<BY_OrderComplete>("OrderDetailId", by_orderdetail.OrderDetailId);
                            if (entity != null && entity.OrderCompleteId != null)
                            {
                                DataFactory.Database().Delete(entity, isOpenTrans);
                            }
                            entity.Create();
                            entity.CompleteDate = DateTime.Now;
                            entity.CompleteUserId = ManageProvider.Provider.Current().UserName;
                            entity.OrderDetailId = by_orderdetail.OrderDetailId;
                            entity.OrderId = by_orderdetail.OrderId;
                            entity.Qty = Convert.ToInt32(by_orderdetail.Qty);
                            entity.TheoryWeight = by_orderdetail.TheoryWeight;
                            entity.Weight = by_orderdetail.Weight;
                            database.Insert<BY_OrderComplete>(entity, isOpenTrans);
                        }
                        #endregion

                        var sj = by_orderdetailprocesss.Where(s => s.QuotationDetailId == QuotationDetail.QuotationDetailId);
                        foreach (var sj2 in sj)
                        {
                            sj2.OrderDetailId = by_orderdetail.OrderDetailId;
                        }
                        
                        
                        //new QuotationPlaceOrderController().GentOrderDetailProcess(by_orderdetail, by_orderdetail.OrderCode, by_quotation, QuotationDetail.WorkAmount);
                        #endregion
                    }

                    #endregion
                    database.Update(by_orderdetailprocesss, isOpenTrans);

                }


                #endregion
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功：请打印订单" }.ToString());
            }
            catch (Exception e)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "提交失败。" + e.Message }.ToString());
            }
        }
        #endregion


        public DataTable GetAutoCompleForXY(string keywords)
        {
            StringBuilder Sql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            Sql.Append(@"    SELECT    c.XYmoney-
            ( SELECT    ISNULL(SUM(TotalAmount), 0) - ISNULL(SUM(GetedAmount),
                                                             0)
              FROM      dbo.BY_Delivery
              WHERE     CustomerCode = c.Code AND ( CheckSheetCode IS NULL OR CheckSheetCode ='')   and DeleteMark = 0 
            ) -
            ( SELECT    ISNULL(SUM(q.TotalAmount), 0)
              FROM      dbo.BY_Quotation q
              WHERE     q.QuotationCode NOT IN ( SELECT QuotationCode
                                                 FROM   dbo.BY_Delivery )
                        AND q.CustomerCode = c.Code
                        AND q.BillStatus = '已落单'  AND Q.DeleteMark = 0
            ) -
( SELECT    ISNULL(SUM(TotalAmount), 0) - ISNULL(SUM(GetedAmount),
                                                             0)
              FROM      dbo.BY_CheckSheet
              WHERE     CustomerCode = c.Code 
            ) 
XYSy
 FROM   Base_Customer c
 WHERE  ( c.Code = @keyword
        ) ");
            parameter.Add(DbFactory.CreateDbParameter("@keyword", keywords));
            return DataFactory.Database().FindTableBySql(Sql.ToString(), parameter.ToArray());
        }

        #region 明细
        /// <summary>
        /// 报价单明细录入视图页面
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult DetailForm()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult DetailFormForY()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult DetailFormForQ()
        {
            return View();
        }
        /// <summary>
        /// 报价单明细列表（返回表格JSON）
        /// </summary>
        /// <param name="QuotationId"></param>
        /// <returns></returns>
        public ActionResult GridDetail(string QuotationId, JqGridParam jqgridparam)
        {
            List<BY_QuotationDetail> ListData = by_quotationdetailbll.GetList(QuotationId, jqgridparam);
            if (ListData != null && ListData.Count > 0)
            {
                var JsonData = new
                {
                    rows = ListData,
                };
                return Content(JsonData.ToJson());
            }
            else
            {
                var JsonData = new
                {
                    rows = ListData,
                };
                return Content(JsonData.ToJson());
            }
        }
        /// <summary>
        /// 删除明细
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult DeleteDetail(string KeyValue)
        {
            BY_QuotationDetail by_quotationdetailBegin = DataFactory.Database().FindEntity<BY_QuotationDetail>(KeyValue);
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(by_quotationdetailBegin.QuotationId);
            IList<BY_QuotationDetail> by_quotationdetails = DataFactory.Database().FindList<BY_QuotationDetail>(" and QuotationId='" + by_quotationdetailBegin.QuotationId + "' AND LineCode > " + by_quotationdetailBegin.LineCode + "");

            if (CanNotEdit(KeyValue))
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：已生成发运单或已收款！" }.ToString());
            }
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                by_quotationdetailbll.Repository().Delete(by_quotationdetailBegin, isOpenTrans);
                by_quotation.Qty = by_quotation.Qty - by_quotationdetailBegin.Qty;
                by_quotation.TotalAmount = by_quotation.TotalAmount - by_quotationdetailBegin.SalesAmount;

                if (by_quotation.IsMobileInput == 1)
                {
                    BY_DeliveryDetail by_deliverydetail = database.FindEntity<BY_DeliveryDetail>("QuotationDetailId", KeyValue);
                    if (by_deliverydetail != null && by_deliverydetail.DeliveryDetailId != null)
                    {
                        BY_Delivery by_delivery = database.FindEntity<BY_Delivery>("QuotationCode", by_quotation.QuotationCode);
                        by_delivery.Qty = by_quotation.Qty;
                        by_delivery.TotalAmount = by_quotation.TotalAmount;

                        database.Delete(by_deliverydetail, isOpenTrans);
                        database.Update(by_delivery, isOpenTrans);
                    }
                }
                else
                {
                    //订单修改
                    BY_OrderDetail by_orderdetail = database.FindEntity<BY_OrderDetail>("QuotationDetailId", by_quotationdetailBegin.QuotationDetailId);
                    if (by_orderdetail != null && by_orderdetail.OrderDetailId != null)
                    {
                        BY_Order by_order = database.FindEntity<BY_Order>(by_orderdetail.OrderId);
                        by_order.Qty = by_quotation.Qty;
                        by_order.Amount = by_quotation.TotalAmount;
                        int by_ordercomplete = database.FindCount<BY_OrderComplete>("OrderDetailId", by_orderdetail.OrderDetailId);
                        if (by_ordercomplete > 0)
                        {
                            database.Rollback();
                            return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：此规格已完工，请在完工界面回退！" }.ToString());
                        }
                        database.Delete(by_orderdetail, isOpenTrans);
                        database.Update(by_order, isOpenTrans);

                        List<BY_OrderDetailProcess> by_orderdetailprocesss = DataFactory.Database().FindList<BY_OrderDetailProcess>("OrderDetailId", by_orderdetail.OrderDetailId);
                        foreach (BY_OrderDetailProcess item in by_orderdetailprocesss)
                        {
                            database.Delete(item, isOpenTrans);
                        }
                        List<BY_OrderDetailProcessDetail> by_orderdetailprocessdetails = DataFactory.Database().FindList<BY_OrderDetailProcessDetail>("OrderDetailId", by_orderdetail.OrderDetailId);
                        foreach (BY_OrderDetailProcessDetail item in by_orderdetailprocessdetails)
                        {
                            database.Delete(item, isOpenTrans);
                        }
                    }

                    int by_deliverydetail = database.FindCount<BY_DeliveryDetail>("QuotationDetailId", by_quotationdetailBegin.QuotationDetailId);
                    if (by_deliverydetail > 0)
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：此规格已发运，不可修改！" }.ToString());
                    }

                    if (by_orderdetail == null || by_orderdetail.OrderDetailId == null)
                    {
                        foreach (BY_QuotationDetail item in by_quotationdetails)
                        {
                            item.LineCode -= 1;
                            database.Update(item, isOpenTrans);
                        }
                    }
                }


                DataFactory.Database().Update<BY_Quotation>(by_quotation, isOpenTrans);

                DataFactory.Database().Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "删除成功。" }.ToString());
            }
            catch (Exception ex)
            {
                DataFactory.Database().Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        /// <summary>
        /// 表单赋值
        /// </summary>
        /// <param name="KeyValue">主键值</param>
        /// <returns></returns>
        public virtual ActionResult SetDetailForm(string KeyValue)
        {
            BY_QuotationDetail entity = DataFactory.Database().FindEntity<BY_QuotationDetail>(KeyValue);
            return Content(entity.ToJson());
        }
        /// <summary>
        /// 表单赋值
        /// </summary>
        /// <param name="KeyValue">主键值</param>
        /// <returns></returns>
        public virtual ActionResult SetDetailFormByLine(string QuotationId, string LineCode)
        {
            try
            {
                BY_QuotationDetail entity = DataFactory.Database().FindEntityByWhere<BY_QuotationDetail>(string.Format(" and QuotationId='{0}' and LineCode = '{1}'", QuotationId, LineCode));
                return Content(entity.ToJson());
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private bool CanNotEdit(string KeyValue)
        {
            if (KeyValue != null)
            {
                BY_DeliveryDetail BY_DeliveryDetail = DataFactory.Database().FindEntity<BY_DeliveryDetail>("QuotationDetailId", KeyValue);
                if (BY_DeliveryDetail != null && BY_DeliveryDetail.DeliveryId != null)
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
            return false;
        }
        /// <summary>
        /// 提交表单
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="KeyValue">主键值</param>
        /// <returns></returns>
        public virtual ActionResult SubmitDetailForm(BY_QuotationDetail entity, string KeyValue, string Insert, string SelectedField = "", string Code = "")
        {
            BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(entity.QuotationId);
            List<BY_OrderDetailProcessDetail> processdetails = DataFactory.Database().FindList<BY_OrderDetailProcessDetail>("MnemonicCode", by_quotation.MnemonicCode);
            BY_QuotationDetail by_quotationdetailBegin = new BY_QuotationDetail();
            IList<BY_QuotationDetail> by_quotationdetails = DataFactory.Database().FindList<BY_QuotationDetail>(" and QuotationId='" + entity.QuotationId + "' AND LineCode >=" + entity.LineCode + "");

            List<BY_OrderDetailProcess> by_orderdetailprocesss = DataFactory.Database().FindList<BY_OrderDetailProcess>("QuotationDetailId", KeyValue);

            if (!string.IsNullOrEmpty(KeyValue))
            {
                by_quotationdetailBegin = DataFactory.Database().FindEntity<BY_QuotationDetail>(KeyValue);
            }
            if (CanNotEdit(KeyValue))
            {
                 return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：已生成发运单或已收款！" }.ToString());
            }
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                if (!string.IsNullOrEmpty(by_quotation.OrderCode) && !string.IsNullOrEmpty(Insert) && Insert == "1")
                {
                    database.Rollback();
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：此规格已生成加工单，不能插入行只能新增！" }.ToString());
                }

                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                entity.QuotationCode = by_quotation.QuotationCode;
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    entity.Modify(KeyValue);
                    database.Update(entity, isOpenTrans);
                }
                else
                {
                    if (!string.IsNullOrEmpty(Insert) && Insert == "1")
                    {
                        foreach (BY_QuotationDetail item in by_quotationdetails)
                        {
                            item.LineCode += 1;
                            database.Update(item, isOpenTrans);
                        }
                    }
                    entity.Create();
                    database.Insert(entity, isOpenTrans);

                }

                


                by_quotation.QuotationId = entity.QuotationId;
                //气割，吊孔数量不加，只加金额
                if (by_quotationdetailBegin.Length > 0)
                {
                    //总数量
                    by_quotation.Qty = by_quotation.Qty - CommonHelper.GetDecimal(by_quotationdetailBegin.Qty);
                }

                if (entity.Length > 0)
                {
                    //总数量
                    by_quotation.Qty = by_quotation.Qty + entity.Qty;
                }

                //总金额
                by_quotation.TotalAmount = by_quotation.TotalAmount + entity.SalesAmount - CommonHelper.GetDecimal(by_quotationdetailBegin.SalesAmount);
                //折扣后总额
                //by_quotation.LessedAmount = by_quotation.TotalAmount * (100 - by_quotation.LessRate) / 100;

                database.Update(by_quotation, isOpenTrans);


                BY_DeliveryDetail by_deliverydetail = database.FindEntity<BY_DeliveryDetail>("QuotationDetailId", KeyValue);
                if (by_deliverydetail != null && by_deliverydetail.DeliveryDetailId != null)
                {
                    database.Rollback();
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：此规格已生成发运单，请到发运单修改！" }.ToString());
                }
                //订单修改
                BY_OrderDetail by_orderdetail = database.FindEntity<BY_OrderDetail>("QuotationDetailId", KeyValue);
                if (by_orderdetail != null && by_orderdetail.OrderDetailId != null)
                {
                    BY_Order by_order = database.FindEntity<BY_Order>(by_orderdetail.OrderId);
                    by_order.Qty = by_quotation.Qty;
                    by_order.Amount = by_quotation.TotalAmount;
                    if (by_orderdetail.OpenQty > entity.Qty)
                    {
                        by_orderdetail.OpenQty = entity.Qty;
                    }
                    if (by_orderdetail.CompleteQty > entity.Qty)
                    {
                        by_orderdetail.CompleteQty = entity.Qty;
                        by_orderdetail.BillStatus = "已完工";
                    }

                    BY_OrderComplete by_ordercomplete = database.FindEntity<BY_OrderComplete>("OrderDetailId", by_orderdetail.OrderDetailId);
                    if (by_ordercomplete != null && by_ordercomplete.OrderCompleteId != null)
                    {
                        if (by_orderdetail.Length > 0)
                        {
                            if (by_ordercomplete.Qty > Convert.ToInt32(entity.Qty))
                            {
                                by_ordercomplete.Qty = Convert.ToInt32(entity.Qty);
                                by_ordercomplete.Weight = entity.Weight;
                                by_ordercomplete.TheoryWeight = entity.TheoryWeight;
                                database.Update(by_ordercomplete, isOpenTrans);
                            }
                        }
                    }

                    CommonHelper.CopySameFieldsObjectALL<BY_QuotationDetail, BY_OrderDetail>(entity, ref by_orderdetail);
                    by_orderdetail.ExecRemark = entity.Remark;
                    if (by_orderdetail.WeightFact > entity.Weight)
                    {
                        by_orderdetail.WeightFact = entity.Weight;
                    }

                    by_orderdetail.Fang = entity.Fang;
                    by_orderdetail.Brand = entity.Brand;
                    by_orderdetail.RemarkKL = entity.RemarkKL;
                    by_orderdetail.DingCode = entity.DingCode;
                    by_orderdetail.TheoryWeight = entity.TheoryWeight;
                    by_orderdetail.Delivery = entity.Delivery;

                    by_orderdetail.KSize = entity.KSize;
                    by_orderdetail.ZSize = entity.ZSize;
                    by_orderdetail.STSize = entity.STSize;
                    by_orderdetail.Hardness = entity.Hardness;
                    by_orderdetail.NowStatus = entity.NowStatus;

                    by_orderdetail.LJtype1 = entity.LJtype1;
                    by_orderdetail.LJtype2 = entity.LJtype2;
                    by_orderdetail.LJsize = entity.LJsize;
                    by_orderdetail.DJ = entity.DJ;


                    by_orderdetail.Modify(by_orderdetail.OrderDetailId);
                    database.Update(by_orderdetail, isOpenTrans);
                    database.Update(by_order, isOpenTrans);



                    #region 费用写到完工表
                    if (by_orderdetail.Length == 0)
                    {
                        BY_OrderComplete BY_OrderComplete = DataFactory.Database().FindEntity<BY_OrderComplete>("OrderDetailId", by_orderdetail.OrderDetailId);
                        if (BY_OrderComplete != null && BY_OrderComplete.OrderCompleteId != null)
                        {
                            DataFactory.Database().Delete(BY_OrderComplete, isOpenTrans);
                        }
                        BY_OrderComplete.Create();
                        BY_OrderComplete.CompleteDate = DateTime.Now;
                        BY_OrderComplete.CompleteUserId = ManageProvider.Provider.Current().UserName;
                        BY_OrderComplete.OrderDetailId = by_orderdetail.OrderDetailId;
                        BY_OrderComplete.OrderId = by_orderdetail.OrderId;
                        BY_OrderComplete.Qty = Convert.ToInt32(by_orderdetail.Qty);
                        BY_OrderComplete.TheoryWeight = by_orderdetail.TheoryWeight;
                        BY_OrderComplete.Weight = by_orderdetail.Weight;
                        database.Insert<BY_OrderComplete>(BY_OrderComplete, isOpenTrans);
                    }
                    #endregion
                }
                else
                {
                    if (!string.IsNullOrEmpty(by_quotation.OrderCode))
                    {
                        BY_Order by_order = database.FindEntity<BY_Order>("OrderCode", by_quotation.OrderCode);
                        if (by_order != null && by_order.OrderId != null)
                        {
                            by_order.Qty = by_quotation.Qty;
                            by_order.Amount = by_quotation.TotalAmount;
                            CommonHelper.CopySameFieldsObjectALL<BY_QuotationDetail, BY_OrderDetail>(entity, ref by_orderdetail);
                            by_orderdetail.ExecRemark = entity.Remark;
                            if (by_orderdetail.WeightFact > entity.Weight)
                            {
                                by_orderdetail.WeightFact = entity.Weight;
                            }
                            by_orderdetail.OrderId = by_order.OrderId;
                            by_orderdetail.OrderCode = by_order.OrderCode;
                            if (by_order.OrderStatus == "已发运")
                            {
                                by_orderdetail.BillStatus = "已发运";
                            }
                            else
                            {
                                by_orderdetail.BillStatus = "正常生产";
                            }
                            if (by_orderdetail.Length == 0)
                            {
                                by_orderdetail.CompleteQty = by_orderdetail.Qty;
                                by_orderdetail.BillStatus = "已完工";
                            }
                            by_orderdetail.RemarkKL = entity.RemarkKL;
                            by_orderdetail.DingCode = entity.DingCode;
                            by_orderdetail.TheoryWeight = entity.TheoryWeight;

                            by_orderdetail.Create();
                            database.Insert(by_orderdetail, isOpenTrans);
                            database.Update(by_order, isOpenTrans);
                            #region 费用写到完工表
                            if (by_orderdetail.Length == 0)
                            {
                                BY_OrderComplete BY_OrderComplete = DataFactory.Database().FindEntity<BY_OrderComplete>("OrderDetailId", by_orderdetail.OrderDetailId);
                                if (BY_OrderComplete != null && BY_OrderComplete.OrderCompleteId != null)
                                {
                                    DataFactory.Database().Delete(BY_OrderComplete, isOpenTrans);
                                }
                                BY_OrderComplete.Create();
                                BY_OrderComplete.CompleteDate = DateTime.Now;
                                BY_OrderComplete.CompleteUserId = ManageProvider.Provider.Current().UserName;
                                BY_OrderComplete.OrderDetailId = by_orderdetail.OrderDetailId;
                                BY_OrderComplete.OrderId = by_orderdetail.OrderId;
                                BY_OrderComplete.Qty = Convert.ToInt32(by_orderdetail.Qty);
                                BY_OrderComplete.TheoryWeight = by_orderdetail.TheoryWeight;
                                BY_OrderComplete.Weight = by_orderdetail.Weight;
                                database.Insert<BY_OrderComplete>(BY_OrderComplete, isOpenTrans);
                            }
                            #endregion
                        }
                    }
                }

                List<BY_OrderDetailProcess> lists = DataFactory.Database().FindList<BY_OrderDetailProcess>("QuotationDetailId", entity.QuotationDetailId);
                List<BY_OrderDetailProcess> deletes = new List<BY_OrderDetailProcess>();
                if (!string.IsNullOrEmpty(SelectedField))
                {
                    for (int i = 0; i < lists.Count; i++)
                    {
                        if (SelectedField.IndexOf(lists[i].TechnologyName) < 0)
                        {
                            DataFactory.Database().Delete(lists[i], isOpenTrans);
                            DataFactory.Database().Delete<BY_OrderDetailProcessDetail>("OrderDetailProcessId", lists[i].OrderDetailProcessId, isOpenTrans);
                            deletes.Add(lists[i]);
                        }
                    }

                    lists = lists.Except(deletes).ToList();

                    string[] names = SelectedField.Trim(',').Split(',');
                    for (int i = 0; i < names.Length; i++)
                    {
                        //and (FullName not like '%账%' and FullName not like '%帐%'  and FullName not like '%优惠%'  and FullName not like '%费%')
                        BY_OrderDetailProcess by_orderdetailprocess = new BY_OrderDetailProcess();
                        Base_DataDictionaryDetail base_datadictionarydetail = DataFactory.Database().FindEntityBySql<Base_DataDictionaryDetail>(string.Format(@" SELECT *
         FROM   Base_DataDictionaryDetail ddd
 WHERE  Enabled = 1 AND DataDictionaryDetailId='{0}' 
        AND DataDictionaryId IN ( SELECT    DataDictionaryId
                                  FROM      Base_DataDictionary
                                  WHERE     Code = '{1}')", names[i].Split(';')[0], Code));
                        if (base_datadictionarydetail.DataDictionaryDetailId != null)
                        {
                            if (lists.Where(p => p.TechnologyName == names[i].Split(';')[0]).Count() > 0)
                            {
                                by_orderdetailprocess = lists.Where(p => p.TechnologyName == names[i].Split(';')[0]).FirstOrDefault();
                                by_orderdetailprocess.Modify(by_orderdetailprocess.OrderDetailProcessId);
                                CommonHelper.CopySameFieldsObjectALL<BY_QuotationDetail, BY_OrderDetailProcess>(entity, ref by_orderdetailprocess);
                                by_orderdetailprocess.TechnologyId = base_datadictionarydetail.Code;
                                by_orderdetailprocess.TechnologyName = base_datadictionarydetail.FullName;
                                by_orderdetailprocess.OrderCode = by_quotation.QuotationCode.Substring(2, 4) + by_quotation.QuotationCode.Substring(8);
                                by_orderdetailprocess.Remark = names[i].Split(';')[4];
                                by_orderdetailprocess.MnemonicCode = by_quotation.QuotationCode;
                                by_orderdetailprocess.SortCode = base_datadictionarydetail.SortCode;
                                by_orderdetailprocess.OrderDetailId = by_orderdetail != null ? by_orderdetail.OrderDetailId:"";
                                by_orderdetailprocess.SaleMc = (decimal?)CommonHelper.GetFloat(names[i].Split(';')[1]);
                                by_orderdetailprocess.SalePer = (decimal?)CommonHelper.GetFloat(names[i].Split(';')[2]);
                                by_orderdetailprocess.SaleTotal = (decimal?)CommonHelper.GetFloat(names[i].Split(';')[3]);

                                DataFactory.Database().Update(by_orderdetailprocess, isOpenTrans);
                            }
                            else
                            {
                                by_orderdetailprocess.Create();
                                CommonHelper.CopySameFieldsObjectALL<BY_QuotationDetail, BY_OrderDetailProcess>(entity, ref by_orderdetailprocess);
                                by_orderdetailprocess.TechnologyId = base_datadictionarydetail.Code;
                                by_orderdetailprocess.TechnologyName = base_datadictionarydetail.FullName;
                                by_orderdetailprocess.OrderCode = by_quotation.QuotationCode.Substring(2, 4) + by_quotation.QuotationCode.Substring(8);
                                by_orderdetailprocess.MnemonicCode = by_quotation.QuotationCode;
                                by_orderdetailprocess.Remark = names[i].Split(';')[4];
                                by_orderdetailprocess.SortCode = base_datadictionarydetail.SortCode;
                                by_orderdetailprocess.OrderDetailId = by_orderdetail != null ? by_orderdetail.OrderDetailId : "";
                                by_orderdetailprocess.SaleMc = (decimal?)CommonHelper.GetFloat(names[i].Split(';')[1]);
                                by_orderdetailprocess.SalePer = (decimal?)CommonHelper.GetFloat(names[i].Split(';')[2]);
                                by_orderdetailprocess.SaleTotal = (decimal?)CommonHelper.GetFloat(names[i].Split(';')[3]);

                                DataFactory.Database().Insert(by_orderdetailprocess, isOpenTrans);
                                lists.Add(by_orderdetailprocess);
                            }
                        }


                    }
                }

                if (by_quotation.LType != "LJ")
                {
                    if (string.IsNullOrEmpty(KeyValue))
                    {
                        new QuotationPlaceOrderController().GentQuotationDetailProcess(by_quotation, entity, by_orderdetailprocesss, by_orderdetail != null ? by_orderdetail.OrderDetailId : "", isOpenTrans);
                    }
                    else if (by_quotationdetailBegin.Kprocess != entity.Kprocess)
                    {
                        new QuotationPlaceOrderController().GentQuotationDetailProcess(by_quotation, entity, by_orderdetailprocesss, by_orderdetail != null ? by_orderdetail.OrderDetailId : "", isOpenTrans);
                    }
                    else
                    {
                        foreach (var item in by_orderdetailprocesss)
                        {
                            item.Qty = entity.Qty;
                        }
                        DataFactory.Database().Update(by_orderdetailprocesss, isOpenTrans);
                    }
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

        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult chai(string QuotationDetailId)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                BY_QuotationDetail by_quotationdetail = DataFactory.Database().FindEntity<BY_QuotationDetail>(QuotationDetailId);
                decimal? count = by_quotationdetail.Qty;

                if (by_quotationdetail != null && !string.IsNullOrWhiteSpace(by_quotationdetail.QuotationDetailId))
                {
                    BY_Quotation quotation = DataFactory.Database().FindEntity<BY_Quotation>(by_quotationdetail.QuotationId);
                    BY_Order by_order = DataFactory.Database().FindEntity<BY_Order>("QuotationId", quotation.QuotationId);
                    List<BY_OrderDetailProcess> odplist = DataFactory.Database().FindList<BY_OrderDetailProcess>("QuotationDetailId", by_quotationdetail.QuotationDetailId).ToList();

                    if (by_order != null && !string.IsNullOrWhiteSpace(by_order.OrderId))
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：已生成订单，不能拆分！！！" }.ToString());
                    }
                    if (by_quotationdetail.Qty < 2)
                    {
                        database.Rollback();
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：数量为1，不能拆分！！！" }.ToString());
                    }


                    //List<BY_QuotationDetail> list2 = list.Where(x => x.LineCode < by_quotationdetail.LineCode).OrderBy(p => p.LineCode).ToList();
                    //foreach (var item in list2)
                    //{
                    //    //listend.Add(item);
                    //}

                    for (int i = 1; i < by_quotationdetail.Qty; i++)
                    {
                        BY_QuotationDetail obj = new BY_QuotationDetail();
                        CommonHelper.CopySameFieldsObjectALL<BY_QuotationDetail, BY_QuotationDetail>(by_quotationdetail, ref obj);
                        obj.SalesAmount = by_quotationdetail.SalesAmount / by_quotationdetail.Qty;
                        obj.MianCost = by_quotationdetail.MianCost / by_quotationdetail.Qty;
                        obj.BianCost = by_quotationdetail.BianCost / by_quotationdetail.Qty;
                        obj.DDCost = by_quotationdetail.DDCost / by_quotationdetail.Qty;
                        obj.MaterialAmount = by_quotationdetail.MaterialAmount / by_quotationdetail.Qty;
                        obj.WorkAmount = by_quotationdetail.WorkAmount / by_quotationdetail.Qty;
                        obj.HeatAmount = by_quotationdetail.HeatAmount / by_quotationdetail.Qty;
                        obj.JKCost = by_quotationdetail.JKCost / by_quotationdetail.Qty;
                        obj.SmallAmount = by_quotationdetail.SmallAmount / by_quotationdetail.Qty;
                        obj.Create();
                        obj.Qty = 1;
                        database.Insert(obj, isOpenTrans);
                        foreach (var item in odplist)
                        {
                            BY_OrderDetailProcess odp = new BY_OrderDetailProcess();
                            CommonHelper.CopySameFieldsObjectALL<BY_OrderDetailProcess, BY_OrderDetailProcess>(item, ref odp);
                            odp.QuotationDetailId = obj.QuotationDetailId;
                            odp.Qty = 1;
                            odp.Create();
                            database.Insert(odp, isOpenTrans);
                        }
                    }
                    by_quotationdetail.SalesAmount = by_quotationdetail.SalesAmount / by_quotationdetail.Qty;
                    by_quotationdetail.MianCost = by_quotationdetail.MianCost / by_quotationdetail.Qty;
                    by_quotationdetail.BianCost = by_quotationdetail.BianCost / by_quotationdetail.Qty;
                    by_quotationdetail.DDCost = by_quotationdetail.DDCost / by_quotationdetail.Qty;
                    by_quotationdetail.MaterialAmount = by_quotationdetail.MaterialAmount / by_quotationdetail.Qty;
                    by_quotationdetail.WorkAmount = by_quotationdetail.WorkAmount / by_quotationdetail.Qty;
                    by_quotationdetail.HeatAmount = by_quotationdetail.HeatAmount / by_quotationdetail.Qty;
                    by_quotationdetail.JKCost = by_quotationdetail.JKCost / by_quotationdetail.Qty;
                    by_quotationdetail.SmallAmount = by_quotationdetail.SmallAmount / by_quotationdetail.Qty;
                    by_quotationdetail.Qty = 1;
                    database.Update(by_quotationdetail, isOpenTrans);
                    database.Commit();

                    List<BY_QuotationDetail> list1 = DataFactory.Database().FindList<BY_QuotationDetail>("QuotationId", quotation.QuotationId).OrderBy(p => p.LineCode).ToList();
                    int line = 1;
                    foreach (var item in list1)
                    {
                        item.LineCode = line;
                        line += 1;
                        database.Update(item);
                        List<BY_OrderDetailProcess> odplist2 = DataFactory.Database().FindList<BY_OrderDetailProcess>("QuotationDetailId", item.QuotationDetailId).ToList();
                        foreach (var item2 in odplist2)
                        {
                            item2.LineCode = item.LineCode;
                            item2.Qty = 1;
                            item2.Modify(item2.OrderDetailProcessId);
                            database.Update(item2);
                        }
                    }
                }
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
        public virtual ActionResult SubmitForm1(BY_QuotationDetail entity, string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                BY_QuotationDetail by_quotationdetail = DataFactory.Database().FindEntity<BY_QuotationDetail>(KeyValue);
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    entity.Modify(KeyValue);
                    DataFactory.Database().Update(entity, isOpenTrans);
                    BY_OrderDetail by_orderdetail = DataFactory.Database().FindEntity<BY_OrderDetail>("QuotationDetailId", KeyValue);
                    if (by_orderdetail.OrderDetailId != null)
                    {
                        by_orderdetail.AddLength = entity.AddLength;
                        by_orderdetail.AddWeigh = entity.AddWeigh;
                        by_orderdetail.AddThick = entity.AddThick;
                        by_orderdetail.CustomerProduct = entity.CustomerProduct;
                        by_orderdetail.ProjectCode = entity.ProjectCode;
                        //by_orderdetail.IsChamfer = entity.IsChamfer;
                        by_orderdetail.WorkCostForWork = entity.WorkCostForWork;
                        by_orderdetail.ShipDescription = entity.ShipDescription;
                        by_orderdetail.ProductName = entity.ProductName;
                        by_orderdetail.ExecRemark = entity.Remark;
                        by_orderdetail.Fang = entity.Fang;
                        by_orderdetail.Brand = entity.Brand;
                        by_orderdetail.BYDUnit = entity.BYDUnit;
                        by_orderdetail.BYDNum = entity.BYDNum;
                        by_orderdetail.ProductName1 = entity.ProductName1;
                        by_orderdetail.RemarkKL = entity.RemarkKL;
                        by_orderdetail.DingCode = entity.DingCode;
                        by_orderdetail.TheoryWeight = entity.TheoryWeight;
                        by_orderdetail.Delivery = entity.Delivery;

                        by_orderdetail.KSize = entity.KSize;
                        by_orderdetail.ZSize = entity.ZSize;
                        by_orderdetail.STSize = entity.STSize;
                        by_orderdetail.Hardness = entity.Hardness;
                        by_orderdetail.NowStatus = entity.NowStatus;


                        by_orderdetail.LJtype1 = entity.LJtype1;
                        by_orderdetail.LJtype2 = entity.LJtype2;
                        by_orderdetail.LJsize = entity.LJsize;

                        by_orderdetail.DJ = entity.DJ;

                        DataFactory.Database().Update(by_orderdetail, isOpenTrans);
                    }


                    List<BY_DeliveryDetail> by_deliverydetails = DataFactory.Database().FindList<BY_DeliveryDetail>("QuotationDetailId", entity.QuotationDetailId);
                    foreach (BY_DeliveryDetail item in by_deliverydetails)
                    {
                        item.CustomerProduct = entity.CustomerProduct;
                        item.ProjectCode = entity.ProjectCode;

                        item.ShipDescription = entity.ShipDescription;
                        item.ProductName = entity.ProductName;
                        //item.IsChamfer = entity.IsChamfer;
                        item.Remark = entity.Remark;
                        DataFactory.Database().Update(item, isOpenTrans);
                    }


                    BY_Quotation by_quotation = DataFactory.Database().FindEntity<BY_Quotation>(by_quotationdetail.QuotationId);
                    if (by_orderdetail != null && by_orderdetail.OrderDetailId != null)
                    {
                        if (entity.IsChamfer == 1)
                        {
                            BY_OrderDetailProcess by_orderdetailprocess4 = DataFactory.Database().FindEntityBySql<BY_OrderDetailProcess>(string.Format(@"  SELECT  *
FROM    BY_OrderDetailProcess
WHERE   OrderCode='{0}' and LineCode={1} and TechnologyId='D001' ", by_orderdetail.OrderCode, by_orderdetail.LineCode));
                            if (by_orderdetailprocess4.OrderDetailProcessId == null)
                            {
                                by_orderdetailprocess4.OrderDetailId = by_orderdetail.OrderDetailId;
                                by_orderdetailprocess4.OrderCode = by_orderdetail.OrderCode;
                                by_orderdetailprocess4.LineCode = by_orderdetail.LineCode;
                                by_orderdetailprocess4.SortCode = 1;
                                by_orderdetailprocess4.Qty = by_orderdetail.Qty;
                                by_orderdetailprocess4.Length = by_orderdetail.Length;
                                by_orderdetailprocess4.Weigh = by_orderdetail.Weigh;
                                by_orderdetailprocess4.Thick = by_orderdetail.Thick;
                                by_orderdetailprocess4.SteelId = by_orderdetail.SteelId;
                                by_orderdetailprocess4.SteelCode = by_orderdetail.SteelCode;
                                by_orderdetailprocess4.SteelDescription = by_orderdetail.SteelDescription;
                                by_orderdetailprocess4.SteelOtherDescription = by_orderdetail.SteelOtherDescription;
                                by_orderdetailprocess4.SteelState = by_orderdetail.SteelState;
                                by_orderdetailprocess4.OrderType = by_orderdetail.OrderType;
                                by_orderdetailprocess4.MnemonicCode = by_quotation.MnemonicCode;
                                by_orderdetailprocess4.TechnologyId = "D001";
                                by_orderdetailprocess4.TechnologyName = "倒角";
                                by_orderdetailprocess4.WorkerPer = CommonHelper.GetDecimal(0.04);
                                by_orderdetailprocess4.WorkerTotal = entity.WorkCostForWork;
                                by_orderdetailprocess4.Create();
                                DataFactory.Database().Insert<BY_OrderDetailProcess>(by_orderdetailprocess4, isOpenTrans);
                            }
                            else
                            {
                                by_orderdetailprocess4.WorkerTotal = entity.WorkCostForWork;
                                DataFactory.Database().Update<BY_OrderDetailProcess>(by_orderdetailprocess4, isOpenTrans);
                            }
                        }
                        else
                        {
                            BY_OrderDetailProcess by_orderdetailprocess = DataFactory.Database().FindEntityBySql<BY_OrderDetailProcess>(string.Format(@" SELECT  *
FROM    BY_OrderDetailProcess
WHERE     OrderCode='{0}' and LineCode={1}  and TechnologyId='D001' ", by_orderdetail.OrderCode, by_orderdetail.LineCode));
                            if (by_orderdetailprocess.OrderDetailProcessId != null)
                            {
                                DataFactory.Database().Delete<BY_OrderDetailProcess>(by_orderdetailprocess, isOpenTrans);
                            }
                        }

                        if (by_quotationdetail.WorkCostForWork != entity.WorkCostForWork)
                        {
                            CountWorkCost(by_orderdetail, entity.WorkCostForWork, isOpenTrans);

                        }
                    }

                    database.Commit();
                }
                return Content(new JsonMessage { Success = true, Code = "1", Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        public void CountWorkCost(BY_OrderDetail by_orderdetail, decimal? WorkCostForWork, DbTransaction isOpenTrans)
        {
            List<BY_OrderDetailProcess> by_orderdetailprocess = DataFactory.Database().FindListBySql<BY_OrderDetailProcess>(string.Format(@"  SELECT  *
FROM    BY_OrderDetailProcess
WHERE   OrderCode='{0}' and LineCode={1} and TechnologyId != 'D001' ", by_orderdetail.OrderCode, by_orderdetail.LineCode));
            foreach (var item in by_orderdetailprocess)
            {
                item.WorkerTotal = WorkCostForWork;
                DataFactory.Database().Update(item, isOpenTrans);
            }
            List<BY_OrderDetailProcessDetail> by_orderdetailprocessdetails = DataFactory.Database().FindListBySql<BY_OrderDetailProcessDetail>(string.Format(@"  SELECT  *
FROM    BY_OrderDetailProcessDetail
WHERE   OrderCode='{0}' and LineCode={1}  ", by_orderdetail.OrderCode, by_orderdetail.LineCode));
            foreach (var item in by_orderdetailprocessdetails)
            {
                item.WorkerAmount = WorkCostForWork * item.WorkerPer * item.Qty / by_orderdetail.Qty;
                item.WorkerTotal = WorkCostForWork;
                DataFactory.Database().Update(item, isOpenTrans);
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult SubmitFormUpdateDetail(BY_QuotationDetail entity, string KeyValue)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    BY_OrderDetail by_orderdetail = database.FindEntity<BY_OrderDetail>("QuotationDetailId", KeyValue);
                    if (by_orderdetail != null && by_orderdetail.OrderDetailId != null)
                    {
                        by_orderdetail.ShipDescription = entity.ShipDescription;
                        by_orderdetail.CustomerProduct = entity.CustomerProduct;
                        by_orderdetail.ProjectCode = entity.ProjectCode;

                        by_orderdetail.Fang = entity.Fang;
                        by_orderdetail.Brand = entity.Brand;
                        by_orderdetail.BYDUnit = entity.BYDUnit;
                        by_orderdetail.BYDNum = entity.BYDNum;
                        by_orderdetail.ProductName1 = entity.ProductName1;
                        by_orderdetail.Delivery = entity.Delivery;

                        by_orderdetail.KSize = entity.KSize;
                        by_orderdetail.ZSize = entity.ZSize;
                        by_orderdetail.STSize = entity.STSize;
                        by_orderdetail.Hardness = entity.Hardness;
                        by_orderdetail.NowStatus = entity.NowStatus;


                        by_orderdetail.LJtype1 = entity.LJtype1;
                        by_orderdetail.LJtype2 = entity.LJtype2;
                        by_orderdetail.LJsize = entity.LJsize;
                        by_orderdetail.DJ = entity.DJ;

                        by_orderdetail.Modify(by_orderdetail.OrderDetailId);
                        database.Update(by_orderdetail, isOpenTrans);
                    }
                    BY_DeliveryDetail by_deliverydetail = database.FindEntity<BY_DeliveryDetail>("QuotationDetailId", KeyValue);
                    if (by_deliverydetail != null && by_deliverydetail.DeliveryDetailId != null)
                    {
                        by_deliverydetail.ShipDescription = entity.ShipDescription;
                        by_deliverydetail.CustomerProduct = entity.CustomerProduct;
                        by_deliverydetail.ProjectCode = entity.ProjectCode;

                        by_deliverydetail.Modify(by_deliverydetail.DeliveryDetailId);
                        database.Update(by_deliverydetail, isOpenTrans);
                    }

                    entity.Modify(KeyValue);
                    database.Update(entity, isOpenTrans);
                    database.Commit();
                }
                return Content(new JsonMessage { Success = true, Code = "1", Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        public List<BY_QuotationDetail> ReturnArea(List<BY_QuotationDetail> by_quotationdetails, Base_SetCost base_setcost)
        {
            foreach (BY_QuotationDetail item in by_quotationdetails)
            {
                item.Area = ReturnArea(item.Thick, item.Weigh, item.Length, item.OrderType);
                decimal price = 0;
                decimal dcost = 0;
                //item.WorkCost = ReturnWorkCost(CommonHelper.GetDecimal(item.Area), CommonHelper.GetDecimal(item.Weigh), item.SteelType, item.OrderType, new Base_Supplier(), 0, 0, ref price, ref dcost);
            }
            return by_quotationdetails;
        }

        
        public decimal? ReturnArea(decimal? Thick, decimal? Weigh, decimal? Length,string OrderType)
        {
            if (Weigh > 0)
            {
                decimal[] at = new decimal[3] { CommonHelper.GetDecimal(Thick), CommonHelper.GetDecimal(Weigh), CommonHelper.GetDecimal(Length) };
                Array.Sort(at);
                Thick = at[0];
                Weigh = at[1];
                Length = at[2];
            }
            //计算面积
            decimal areacost = 0;

            if (OrderType == "毛料生产" || Weigh == 0)
            {
                return 0;
            }

            if (CommonHelper.GetDecimal(Thick) < 80 && OrderType == "光板加工")
            {
                areacost = CommonHelper.GetDecimal(Length) * CommonHelper.GetDecimal(Weigh) * 2 / 1000000;
            }
            else 
            {
                areacost = ((CommonHelper.GetDecimal(Thick) + CommonHelper.GetDecimal(Weigh)) * (CommonHelper.GetDecimal(Length) + CommonHelper.GetDecimal(Thick))) * 2 / 1000000;
            }

            return areacost;
        }

        //public decimal? ReturnWorkCost(decimal areacost, decimal weigh, string SteelType, string OrderType, Base_Supplier base_supplier, decimal defualtPrice, decimal defualtLimiteCost, ref decimal Price, ref decimal LimiteCost)
        //{
        //    if (OrderType == "毛料生产" || weigh == 0)
        //    {
        //        Price = 0;
        //        return 0;
        //    }

        //    if (base_supplier == null || base_supplier.SupplierId == null)
        //    {
        //        base_supplier.GRsteelPrice7 = 100;
        //        base_supplier.GYsteelPrice7 = 160;
        //        base_supplier.GRsteelPrice8 = 100;
        //        base_supplier.GYsteelPrice8 = 160;
        //        base_supplier.JGsteelPrice = 200;
        //        base_supplier.JsteelPrice = 260;
        //        base_supplier.GRsteelPrice71 = CommonHelper.GetDecimal(6.5);
        //        base_supplier.GYsteelPrice71 = 8;
        //        base_supplier.GRsteelPrice81 = 8;
        //        base_supplier.GYsteelPrice81 = 8;
        //        base_supplier.JGsteelPrice1 = 11;
        //        base_supplier.JsteelPrice1 = 11;
        //    }


        //    if (defualtLimiteCost > 0)
        //    {
        //        base_supplier.GRsteelPrice71 = defualtLimiteCost;
        //        base_supplier.GYsteelPrice71 = defualtLimiteCost;
        //        base_supplier.GRsteelPrice81 = defualtLimiteCost;
        //        base_supplier.GYsteelPrice81 = defualtLimiteCost;
        //        base_supplier.JGsteelPrice1 = defualtLimiteCost;
        //        base_supplier.JsteelPrice1 = defualtLimiteCost;
        //    }

        //    if (defualtPrice > 0)
        //    {
        //        base_supplier.GRsteelPrice7 = defualtPrice;
        //        base_supplier.GYsteelPrice7 = defualtPrice;
        //        base_supplier.GRsteelPrice8 = defualtPrice;
        //        base_supplier.GYsteelPrice8 = defualtPrice;
        //        base_supplier.JGsteelPrice = defualtPrice;
        //        base_supplier.JsteelPrice = defualtPrice;
        //    }

        //    decimal WorkCost = 0;
        //    if (OrderType.IndexOf("光板")>=0)
        //    {
        //        if (SteelType == "普通钢")
        //        {
        //            if (weigh < 80)
        //            {
        //                WorkCost = areacost * CommonHelper.GetDecimal(base_supplier.GRsteelPrice7);
        //                Price = CommonHelper.GetDecimal(base_supplier.GRsteelPrice7);
        //                if (WorkCost < CommonHelper.GetDecimal(base_supplier.GRsteelPrice71) )
        //                {
        //                    WorkCost = CommonHelper.GetDecimal(base_supplier.GRsteelPrice71);
        //                }
        //                LimiteCost = CommonHelper.GetDecimal(base_supplier.GRsteelPrice71);
        //            }
        //            else
        //            {
        //                WorkCost = areacost * CommonHelper.GetDecimal(base_supplier.GRsteelPrice8);
        //                Price = CommonHelper.GetDecimal(base_supplier.GRsteelPrice8);
        //                if (WorkCost < CommonHelper.GetDecimal(base_supplier.GRsteelPrice81))
        //                {
        //                    WorkCost = CommonHelper.GetDecimal(base_supplier.GRsteelPrice81);
        //                }
        //                LimiteCost = CommonHelper.GetDecimal(base_supplier.GRsteelPrice81);
        //            }
        //        }
        //        else
        //        {
        //            if (weigh < 80)
        //            {
        //                WorkCost = areacost * CommonHelper.GetDecimal(base_supplier.GYsteelPrice7);
        //                Price = CommonHelper.GetDecimal(base_supplier.GYsteelPrice7);
        //                if (WorkCost < CommonHelper.GetDecimal(base_supplier.GYsteelPrice71))
        //                {
        //                    WorkCost = CommonHelper.GetDecimal(base_supplier.GYsteelPrice71);
        //                }
        //                LimiteCost = CommonHelper.GetDecimal(base_supplier.GYsteelPrice71);
        //            }
        //            else
        //            {
        //                WorkCost = areacost * CommonHelper.GetDecimal(base_supplier.GYsteelPrice8);
        //                Price = CommonHelper.GetDecimal(base_supplier.GYsteelPrice8);
        //                if (WorkCost < CommonHelper.GetDecimal(base_supplier.GYsteelPrice81))
        //                {
        //                    WorkCost = CommonHelper.GetDecimal(base_supplier.GYsteelPrice81);
        //                }
        //                LimiteCost = CommonHelper.GetDecimal(base_supplier.GYsteelPrice81);
        //            }
        //        }

        //    }
        //    else if (OrderType.IndexOf("精光板") >= 0 )
        //    {
        //        WorkCost = areacost * CommonHelper.GetDecimal(base_supplier.JGsteelPrice);
        //        Price = CommonHelper.GetDecimal(base_supplier.JGsteelPrice);
        //        if (WorkCost < CommonHelper.GetDecimal(base_supplier.JGsteelPrice1))
        //        {
        //            WorkCost = CommonHelper.GetDecimal(base_supplier.JGsteelPrice1);
        //        }
        //        LimiteCost = CommonHelper.GetDecimal(base_supplier.JGsteelPrice1);
        //    }
        //    else if (OrderType.IndexOf("精板") >= 0)
        //    {
        //        WorkCost = areacost * CommonHelper.GetDecimal(base_supplier.JsteelPrice);
        //        Price = CommonHelper.GetDecimal(base_supplier.JsteelPrice);
        //        if (WorkCost < CommonHelper.GetDecimal(base_supplier.JsteelPrice1))
        //        {
        //            WorkCost = CommonHelper.GetDecimal(base_supplier.JsteelPrice1);
        //        }
        //        LimiteCost = CommonHelper.GetDecimal(base_supplier.JsteelPrice1);
        //    }
        //    return WorkCost;
        //}

        //public decimal? ReturnWorkCost1(decimal areacost, decimal weigh, string SteelType, string OrderType, Base_Supplier base_supplier, ref decimal Price)
        //{
        //    if (base_supplier == null || base_supplier.SupplierId == null)
        //    {
        //        base_supplier.GRsteelPrice7 = 90;
        //        base_supplier.GYsteelPrice7 = 120;
        //        base_supplier.GRsteelPrice8 = 100;
        //        base_supplier.GYsteelPrice8 = 120;
        //        base_supplier.JGsteelPrice = 180;
        //        base_supplier.JsteelPrice = 230;
        //        base_supplier.GRsteelPrice71 = CommonHelper.GetDecimal(6);
        //        base_supplier.GYsteelPrice71 = 8;
        //        base_supplier.GRsteelPrice81 = 8;
        //        base_supplier.GYsteelPrice81 = 8;
        //        base_supplier.JGsteelPrice1 = 10;
        //        base_supplier.JsteelPrice1 = 10;
        //    }
        //    decimal WorkCost = 0;
        //    if (OrderType == "光板加工")
        //    {
        //        if (SteelType == "普通钢")
        //        {
        //            if (weigh < 80)
        //            {
        //                WorkCost = areacost * CommonHelper.GetDecimal(base_supplier.GRsteelPrice7);
        //                Price = CommonHelper.GetDecimal(base_supplier.GRsteelPrice7);
        //                if (WorkCost < CommonHelper.GetDecimal(base_supplier.GRsteelPrice71))
        //                {
        //                    WorkCost = CommonHelper.GetDecimal(base_supplier.GRsteelPrice71);
        //                }
        //            }
        //            else
        //            {
        //                WorkCost = areacost * CommonHelper.GetDecimal(base_supplier.GRsteelPrice8);
        //                Price = CommonHelper.GetDecimal(base_supplier.GRsteelPrice8);
        //                if (WorkCost < CommonHelper.GetDecimal(base_supplier.GRsteelPrice81))
        //                {
        //                    WorkCost = CommonHelper.GetDecimal(base_supplier.GRsteelPrice81);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (weigh < 80)
        //            {
        //                WorkCost = areacost * CommonHelper.GetDecimal(base_supplier.GYsteelPrice7);
        //                Price = CommonHelper.GetDecimal(base_supplier.GYsteelPrice7);
        //                if (WorkCost < CommonHelper.GetDecimal(base_supplier.GYsteelPrice71))
        //                {
        //                    WorkCost = CommonHelper.GetDecimal(base_supplier.GYsteelPrice71);
        //                }
        //            }
        //            else
        //            {
        //                WorkCost = areacost * CommonHelper.GetDecimal(base_supplier.GYsteelPrice8);
        //                Price = CommonHelper.GetDecimal(base_supplier.GYsteelPrice8);
        //                if (WorkCost < CommonHelper.GetDecimal(base_supplier.GYsteelPrice81))
        //                {
        //                    WorkCost = CommonHelper.GetDecimal(base_supplier.GYsteelPrice81);
        //                }
        //            }
        //        }

        //    }
        //    if (OrderType == "精光板加工")
        //    {
        //        WorkCost = areacost * CommonHelper.GetDecimal(base_supplier.JGsteelPrice);
        //        Price = CommonHelper.GetDecimal(base_supplier.JGsteelPrice);
        //        if (WorkCost < CommonHelper.GetDecimal(base_supplier.JGsteelPrice1))
        //        {
        //            WorkCost = CommonHelper.GetDecimal(base_supplier.JGsteelPrice1);
        //        }
        //    }
        //    if (OrderType == "精板加工")
        //    {
        //        WorkCost = areacost * CommonHelper.GetDecimal(base_supplier.JsteelPrice);
        //        Price = CommonHelper.GetDecimal(base_supplier.JsteelPrice);
        //        if (WorkCost < CommonHelper.GetDecimal(base_supplier.JsteelPrice1))
        //        {
        //            WorkCost = CommonHelper.GetDecimal(base_supplier.JsteelPrice1);
        //        }
        //    }
        //    return WorkCost;
        //}
        #endregion

        #region 报价单
        /// <summary>
        /// 查询报价单页面
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult QueryQuotation()
        {
            return View();
        }
        #endregion
    }
}
