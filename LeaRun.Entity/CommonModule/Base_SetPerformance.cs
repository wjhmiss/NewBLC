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
    /// 设置绩效
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.07.08 21:09</date>
    /// </author>
    /// </summary>
    [Description("设置绩效")]
    [PrimaryKey("SetPerformanceId")]
    public class Base_SetPerformance : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// id
        /// </summary>
        /// <returns></returns>
        [DisplayName("id")]
        public string SetPerformanceId { get; set; }
        /// <summary>
        /// 工艺ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("工艺ID")]
        public string TechnologyId { get; set; }
        /// <summary>
        /// 工艺名称
        /// </summary>
        /// <returns></returns>
        [DisplayName("工艺名称")]
        public string TechnologyName { get; set; }
        /// <summary>
        /// 钢材类型
        /// </summary>
        /// <returns></returns>
        [DisplayName("钢材类型")]
        public string SteelType { get; set; }
        /// <summary>
        /// 加工类型
        /// </summary>
        /// <returns></returns>
        [DisplayName("加工类型")]
        public string OrderType { get; set; }
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
        /// 保底面积
        /// </summary>
        /// <returns></returns>
        [DisplayName("保底面积")]
        public decimal? LimitArea { get; set; }
        /// <summary>
        ///每平米单价
        /// </summary>
        /// <returns></returns>
        [DisplayName("每平米单价")]
        public decimal? PriceArea { get; set; }


        /// <summary>
        ///比例
        /// </summary>
        /// <returns></returns>
        [DisplayName("比例")]
        public decimal? PercentWorkAmount { get; set; }

        /// <summary>
        /// 厚 由
        /// </summary>
        /// <returns></returns>
        [DisplayName("厚 由")]
        public decimal? PachyStart { get; set; }
        /// <summary>
        /// 厚 至
        /// </summary>
        /// <returns></returns>
        [DisplayName("厚 至")]
        public decimal? PachyEnd { get; set; }
        /// <summary>
        /// 宽 由
        /// </summary>
        /// <returns></returns>
        [DisplayName("宽 由")]
        public decimal? WidthStart { get; set; }
        /// <summary>
        /// 宽 至
        /// </summary>
        /// <returns></returns>
        [DisplayName("宽 至")]
        public decimal? WidthEnd { get; set; }
        /// <summary>
        /// 长 由
        /// </summary>
        /// <returns></returns>
        [DisplayName("长 由")]
        public decimal? LengthStart { get; set; }
        /// <summary>
        /// 长 至
        /// </summary>
        /// <returns></returns>
        [DisplayName("长 至")]
        public decimal? LengthEnd { get; set; }

        /// <summary>
        /// 或者
        /// </summary>
        /// <returns></returns>
        [DisplayName("OrOr")]
        public int? OrOr { get; set; }


        /// <summary>
        /// 厚 由
        /// </summary>
        /// <returns></returns>
        [DisplayName("厚 由")]
        public decimal? WeightStart { get; set; }
        /// <summary>
        /// 厚 至
        /// </summary>
        /// <returns></returns>
        [DisplayName("厚 至")]
        public decimal? WeightEnd { get; set; }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <returns></returns>
        [DisplayName("修改用户")]
        public string WhereQ { get; set; }
        /// <summary>
        /// 厚 至
        /// </summary>
        /// <returns></returns>
        [DisplayName("厚 至")]
        public decimal? WhereQty { get; set; }
        /// <summary>
        /// 修改用户
        /// </summary>
        /// <returns></returns>
        [DisplayName("修改用户")]
        public string Unit { get; set; }

        /// <summary>
        /// 厚 由
        /// </summary>
        /// <returns></returns>
        [DisplayName("厚 由")]
        public decimal? Price { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.SetPerformanceId = CommonHelper.GetGuid;
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
            this.SetPerformanceId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}