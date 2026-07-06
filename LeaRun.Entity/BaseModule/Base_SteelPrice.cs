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
    /// 钢材价目表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.14 10:31</date>
    /// </author>
    /// </summary>
    [Description("钢材价目表")]
    [PrimaryKey("SteelPriceId")]
    public class Base_SteelPrice : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 钢材价目主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢材价目主键")]
        public string SteelPriceId { get; set; }
        /// <summary>
        /// 加工类型
        /// </summary>
        /// <returns></returns>
        [DisplayName("加工类型")]
        public string ProcessType { get; set; }
        /// <summary>
        /// 钢材信息主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢材信息主键")]
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
        /// <summary>
        /// 通用编码
        /// </summary>
        /// <returns></returns>
        [DisplayName("通用编码")]
        public string CommonCode { get; set; }
        /// <summary>
        /// 厚度/直径开始
        /// </summary>
        /// <returns></returns>
        [DisplayName("厚度/直径开始")]
        public decimal? SizeStart { get; set; }
        /// <summary>
        /// 厚度/直径结束
        /// </summary>
        /// <returns></returns>
        [DisplayName("厚度/直径结束")]
        public decimal? SizeEnd { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        /// <returns></returns>
        [DisplayName("单位")]
        public string Unit { get; set; }
        /// <summary>
        /// 圆料
        /// </summary>
        /// <returns></returns>
        [DisplayName("圆料")]
        public int? RoundFeed { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        /// <returns></returns>
        [DisplayName("单价")]
        public decimal? Price { get; set; }
        /// <summary>
        /// 折扣率
        /// </summary>
        /// <returns></returns>
        [DisplayName("折扣率")]
        public decimal? DiscountRate { get; set; }
        /// <summary>
        /// 折扣单价
        /// </summary>
        /// <returns></returns>
        [DisplayName("折扣单价")]
        public decimal? DiscountPrice { get; set; }
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
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.SteelPriceId = CommonHelper.GetGuid;
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
            this.SteelPriceId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}