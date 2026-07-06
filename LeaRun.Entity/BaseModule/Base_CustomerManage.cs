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
    /// 客户热处理费折扣表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.15 10:53</date>
    /// </author>
    /// </summary>
    [Description("客户热处理费折扣表")]
    [PrimaryKey("CustomerManageId")]
    public class Base_CustomerManage : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 热处理费折扣主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("热处理费折扣主键")]
        public string CustomerManageId { get; set; }
        /// <summary>
        /// 客户主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户主键")]
        public string CustomerId { get; set; }
        /// <summary>
        /// 工艺编码
        /// </summary>
        /// <returns></returns>
        [DisplayName("工艺编码")]
        public string CraftTypeCode { get; set; }
        /// <summary>
        /// 工艺名称
        /// </summary>
        /// <returns></returns>
        [DisplayName("工艺名称")]
        public string CraftTypeName { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        /// <returns></returns>
        [DisplayName("单位")]
        public string Unit { get; set; }
        /// <summary>
        /// 折扣率
        /// </summary>
        /// <returns></returns>
        [DisplayName("折扣率")]
        public decimal? DiscountRate { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        /// <returns></returns>
        [DisplayName("单价")]
        public decimal? Price { get; set; }
        /// <summary>
        /// 有效时间 从
        /// </summary>
        /// <returns></returns>
        [DisplayName("有效时间 从")]
        public DateTime? EnabledTimeStart { get; set; }
        /// <summary>
        /// 有效时间 至
        /// </summary>
        /// <returns></returns>
        [DisplayName("有效时间 至")]
        public DateTime? EnabledTimeEnd { get; set; }
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
        /// 钢材主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢材主键")]
        public string SteelId { get; set; }
        /// <summary>
        /// 钢材编码
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢材编码")]
        public string SteelCode { get; set; }
        /// <summary>
        /// 钢材名称
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢材名称")]
        public string SteelName { get; set; }


        public decimal? HeatLimitMoney { get; set; }

        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.CustomerManageId = CommonHelper.GetGuid;
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
            this.CustomerManageId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}