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
    /// BY_FKReport
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.04.19 15:50</date>
    /// </author>
    /// </summary>
    [Description("BY_FKReport")]
    [PrimaryKey("FKReportId")]
    public class BY_FKReport : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// FKReportId
        /// </summary>
        /// <returns></returns>
        [DisplayName("FKReportId")]
        public string FKReportId { get; set; }
        /// <summary>
        /// FKReportCode
        /// </summary>
        /// <returns></returns>
        [DisplayName("FKReportCode")]
        public string FKReportCode { get; set; }
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
        public string PlaceOrigin { get; set; }
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
        /// 炉号
        /// </summary>
        /// <returns></returns>
        [DisplayName("炉号")]
        public string StoveCode { get; set; }
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
        /// CheckCompany
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckCompany")]
        public string CheckCompany { get; set; }
        /// <summary>
        /// CheckSteelName
        /// </summary>
        /// <returns></returns>
        [DisplayName("CheckSteelName")]
        public string CheckSteelName { get; set; }
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
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.FKReportId = CommonHelper.GetGuid;
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
            this.FKReportId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}