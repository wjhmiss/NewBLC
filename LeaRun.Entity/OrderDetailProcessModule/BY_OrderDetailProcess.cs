//=====================================================================================
// All Rights Reserved , Copyright @ DNS 2015
// Software Developers @ DNS 2015
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
    /// 工单工序表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.03.14 11:44</date>
    /// </author>
    /// </summary>
    [Description("工单工序表")]
    [PrimaryKey("OrderDetailProcessId")]
    public class BY_OrderDetailProcess : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 工单工序ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("工单工序ID")]
        public string OrderDetailProcessId { get; set; }
        /// <summary>
        /// 工单详情ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("工单详情ID")]
        public string OrderDetailId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        /// <returns></returns>
        [DisplayName("订单号")]
        public string OrderCode { get; set; }
        /// <summary>
        /// 订单类型
        /// </summary>
        /// <returns></returns>
        [DisplayName("订单类型")]
        public string OrderType { get; set; }
        /// <summary>
        /// 行号
        /// </summary>
        /// <returns></returns>
        [DisplayName("行号")]
        public int? LineCode { get; set; }
        /// <summary>
        /// 厚度
        /// </summary>
        /// <returns></returns>
        [DisplayName("厚度")]
        public decimal? Thick { get; set; }
        /// <summary>
        /// 宽
        /// </summary>
        /// <returns></returns>
        [DisplayName("宽")]
        public decimal? Weigh { get; set; }
        /// <summary>
        /// 长
        /// </summary>
        /// <returns></returns>
        [DisplayName("长")]
        public decimal? Length { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        /// <returns></returns>
        [DisplayName("数量")]
        public decimal? Qty { get; set; }
        /// <summary>
        /// 完成数量
        /// </summary>
        /// <returns></returns>
        [DisplayName("完成数量")]
        public decimal? QtyComplete { get; set; }
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
        /// 钢种其它描述
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢种其它描述")]
        public string SteelOtherDescription { get; set; }
        /// <summary>
        /// 钢种状态
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢种状态")]
        public string SteelState { get; set; }
        /// <summary>
        /// 工序ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("工序ID")]
        public string TechnologyId { get; set; }
        /// <summary>
        /// 工序名称
        /// </summary>
        /// <returns></returns>
        [DisplayName("工序名称")]
        public string TechnologyName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        /// <returns></returns>
        [DisplayName("备注")]
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
        /// <summary>
        /// 助记码
        /// </summary>
        /// <returns></returns>
        [DisplayName("助记码")]
        public string MnemonicCode { get; set; }

        /// <summary>
        /// 加工费
        /// </summary>
        /// <returns></returns>
        [DisplayName("比例")]
        public decimal? WorkerPer { get; set; }
        /// <summary>
        /// 加工费
        /// </summary>
        /// <returns></returns>
        [DisplayName("加工费")]
        public decimal? WorkerTotal { get; set; }
        public decimal? Area { get; set; }

        public string QuotationDetailId { get; set; }

        public decimal? SalePer { get; set; }
        public decimal? SaleMc { get; set; }
        public decimal? SaleTotal { get; set; }

        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.OrderDetailProcessId = CommonHelper.GetGuid;
            if (this.CreateDate == null)
            {
                this.CreateDate = DateTime.Now;
            }
            this.CreateUserId = ManageProvider.Provider.Current().UserId;
            this.CreateUserName = ManageProvider.Provider.Current().UserName;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.OrderDetailProcessId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}