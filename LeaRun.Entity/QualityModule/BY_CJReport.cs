//=====================================================================================
// All Rights Reserved , Copyright @ DNS 2017
// Software Developers @ DNS 2017
//=====================================================================================

using DNS.DataAccess.Attributes;
using DNS.Utilities;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DNS.Entity
{
    /// <summary>
    /// BY_CJReport
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.04.14 14:44</date>
    /// </author>
    /// </summary>
    [Description("BY_CJReport")]
    [PrimaryKey("CJReportId")]
    public class BY_CJReport : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// CJReportId
        /// </summary>
        /// <returns></returns>
        [DisplayName("CJReportId")]
        public string CJReportId { get; set; }
        /// <summary>
        /// CJReportCode
        /// </summary>
        /// <returns></returns>
        [DisplayName("CJReportCode")]
        public string CJReportCode { get; set; }
        /// <summary>
        /// SteelQualityreportCode
        /// </summary>
        /// <returns></returns>
        [DisplayName("SteelQualityreportCode")]
        public string SteelQualityreportCode { get; set; }
        /// <summary>
        /// QuotationHeatCode
        /// </summary>
        /// <returns></returns>
        [DisplayName("QuotationHeatCode")]
        public string QuotationHeatCode { get; set; }
        /// <summary>
        /// 订单明细ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("订单明细ID")]
        public string OrderDetailId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        /// <returns></returns>
        [DisplayName("订单号")]
        public string OrderLineCode { get; set; }
        /// <summary>
        /// 客户ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户ID")]
        public string CustomerId { get; set; }
        /// <summary>
        /// 客户编号
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户编号")]
        public string CustomerCode { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户名称")]
        public string CustomerName { get; set; }
        /// <summary>
        /// Qty
        /// </summary>
        /// <returns></returns>
        [DisplayName("Qty")]
        public int? Qty { get; set; }
        /// <summary>
        /// 钢种
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢种")]
        public string SteelId { get; set; }
        /// <summary>
        /// 钢种编号
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢种编号")]
        public string SteelCode { get; set; }
        /// <summary>
        /// 钢种描述
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢种描述")]
        public string SteelDescription { get; set; }
        /// <summary>
        /// Size
        /// </summary>
        /// <returns></returns>
        [DisplayName("Size")]
        public string Size { get; set; }
        /// <summary>
        /// CheckMethod
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckMethod")]
        public string CheckMethod { get; set; }
        /// <summary>
        /// CheckA1
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckA1")]
        public string CheckA1 { get; set; }
        /// <summary>
        /// CheckA1Condition
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckA1Condition")]
        public string CheckA1Condition { get; set; }
        /// <summary>
        /// CheckA1Result
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckA1Result")]
        public string CheckA1Result { get; set; }
        /// <summary>
        /// CheckA2
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckA2")]
        public string CheckA2 { get; set; }
        /// <summary>
        /// CheckA2Condition
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckA2Condition")]
        public string CheckA2Condition { get; set; }
        /// <summary>
        /// CheckA2Result
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckA2Result")]
        public string CheckA2Result { get; set; }
        /// <summary>
        /// CheckA3
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckA3")]
        public string CheckA3 { get; set; }
        /// <summary>
        /// CheckA3Condition
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckA3Condition")]
        public string CheckA3Condition { get; set; }
        /// <summary>
        /// CheckA3Result
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckA3Result")]
        public string CheckA3Result { get; set; }
        /// <summary>
        /// CheckA4
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckA4")]
        public string CheckA4 { get; set; }
        /// <summary>
        /// CheckA4Condition
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckA4Condition")]
        public string CheckA4Condition { get; set; }
        /// <summary>
        /// CheckA4Result
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckA4Result")]
        public string CheckA4Result { get; set; }
        /// <summary>
        /// CheckA5
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckA5")]
        public string CheckA5 { get; set; }
        /// <summary>
        /// CheckA5Condition
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckA5Condition")]
        public string CheckA5Condition { get; set; }
        /// <summary>
        /// CheckA5Result
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckA5Result")]
        public string CheckA5Result { get; set; }
        /// <summary>
        /// CheckA6
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckA6")]
        public string CheckA6 { get; set; }
        /// <summary>
        /// CheckA6Condition
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckA6Condition")]
        public string CheckA6Condition { get; set; }
        /// <summary>
        /// CheckA6Result
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckA6Result")]
        public string CheckA6Result { get; set; }
        /// <summary>
        /// CheckSteelCode
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckSteelCode")]
        public string CheckSteelCode { get; set; }
        /// <summary>
        /// CheckList
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckList")]
        public string CheckList { get; set; }
        /// <summary>
        /// 执行备注信息
        /// </summary>
        /// <returns></returns>
        [DisplayName("执行备注信息")]
        public string Remark { get; set; }
        /// <summary>
        /// 有效
        /// </summary>
        /// <returns></returns>
        [DisplayName("有效")]
        public int? Enabled { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        /// <returns></returns>
        [DisplayName("排序码")]
        public int? SortCode { get; set; }
        /// <summary>
        /// 删除标记
        /// </summary>
        /// <returns></returns>
        [DisplayName("删除标记")]
        public int? DeleteMark { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("创建时间")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 创建用户主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("创建用户主键")]
        public string CreateUserId { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <returns></returns>
        [DisplayName("创建用户")]
        public string CreateUserName { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("修改时间")]
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 修改用户主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("修改用户主键")]
        public string ModifyUserId { get; set; }
        /// <summary>
        /// 修改用户
        /// </summary>
        /// <returns></returns>
        [DisplayName("修改用户")]
        public string ModifyUserName { get; set; }
        public string PJResult { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.CJReportId = CommonHelper.GetGuid;
              if (this.CreateDate == null)
            {
                this.CreateDate = DateTime.Now;
            }             this.CreateUserId = ManageProvider.Provider.Current().UserId;
            this.CreateUserName = ManageProvider.Provider.Current().UserName;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.CJReportId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}