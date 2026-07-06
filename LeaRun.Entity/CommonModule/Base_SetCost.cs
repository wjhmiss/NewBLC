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
    /// 设置成本
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.07.08 10:48</date>
    /// </author>
    /// </summary>
    [Description("设置成本")]
    [PrimaryKey("SetCostId")]
    public class Base_SetCost : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// id
        /// </summary>
        /// <returns></returns>
        [DisplayName("id")]
        public string SetCostId { get; set; }
        /// <summary>
        /// 光硬价格
        /// </summary>
        /// <returns></returns>
        [DisplayName("光硬价格")]
        public decimal? GYPrice { get; set; }
        /// <summary>
        /// 光软价格
        /// </summary>
        /// <returns></returns>
        [DisplayName("光软价格")]
        public decimal? GRPrice { get; set; }
        /// <summary>
        /// 精板价格
        /// </summary>
        /// <returns></returns>
        [DisplayName("精板价格")]
        public decimal? JBPrice { get; set; }
        /// <summary>
        /// 热处理价格
        /// </summary>
        /// <returns></returns>
        [DisplayName("热处理价格")]
        public decimal? RclPrice { get; set; }
        /// <summary>
        /// 光硬保底面积
        /// </summary>
        /// <returns></returns>
        [DisplayName("光硬保底面积")]
        public decimal? GYLimiteArea { get; set; }
        /// <summary>
        /// 光软保底面积
        /// </summary>
        /// <returns></returns>
        [DisplayName("光软保底面积")]
        public decimal? GRLimiteArea { get; set; }
        /// <summary>
        /// 精板保底面积
        /// </summary>
        /// <returns></returns>
        [DisplayName("精板保底面积")]
        public decimal? JBLimiteArea { get; set; }
        /// <summary>
        /// 热处理保底面积
        /// </summary>
        /// <returns></returns>
        [DisplayName("热处理保底面积")]
        public decimal? RclLimiteArea { get; set; }
        /// <summary>
        /// 光硬保底面积
        /// </summary>
        /// <returns></returns>
        [DisplayName("光硬保底面积")]
        public decimal? GYLimiteAreaPer { get; set; }
        /// <summary>
        /// 光软保底面积
        /// </summary>
        /// <returns></returns>
        [DisplayName("光软保底面积")]
        public decimal? GRLimiteAreaPer { get; set; }
        /// <summary>
        /// 精板保底面积
        /// </summary>
        /// <returns></returns>
        [DisplayName("精板保底面积")]
        public decimal? JBLimiteAreaPer { get; set; }
        /// <summary>
        /// 热处理保底面积
        /// </summary>
        /// <returns></returns>
        [DisplayName("热处理保底面积")]
        public decimal? RclLimiteAreaPer { get; set; }
        /// <summary>
        /// 超长
        /// </summary>
        /// <returns></returns>
        [DisplayName("超长")]
        public decimal? GreatLong { get; set; }
        /// <summary>
        /// 超宽
        /// </summary>
        /// <returns></returns>
        [DisplayName("超宽")]
        public decimal? GreatWeigh { get; set; }
        /// <summary>
        /// 超长宽后价格
        /// </summary>
        /// <returns></returns>
        [DisplayName("超长宽后价格")]
        public decimal? GreatPrice { get; set; }
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
        /// 面积计算类型
        /// </summary>
        /// <returns></returns>
        [DisplayName("面积计算类型")]
        public int? SetAreaType { get; set; }
        
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.SetCostId = CommonHelper.GetGuid;
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
            this.SetCostId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}