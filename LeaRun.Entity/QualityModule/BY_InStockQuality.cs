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
    /// 原材料入仓品质信息
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.29 20:29</date>
    /// </author>
    /// </summary>
    [Description("原材料入仓品质信息")]
    [PrimaryKey("InStockQualityId")]
    public class BY_InStockQuality : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 入仓品质ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("入仓品质ID")]
        public string InStockQualityId { get; set; }
        /// <summary>
        /// 入仓单ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("入仓单ID")]
        public string BillCode { get; set; }
        /// <summary>
        /// 入仓品检单号
        /// </summary>
        /// <returns></returns>
        [DisplayName("入仓品检单号")]
        public string InStockQualityCode { get; set; }
        /// <summary>
        /// 外观及尺寸是否合格(0-不合格，1-合格)
        /// </summary>
        /// <returns></returns>
        [DisplayName("外观及尺寸是否合格(0-不合格，1-合格)")]
        public int? FacadeSize { get; set; }
        /// <summary>
        /// 表面硬度
        /// </summary>
        /// <returns></returns>
        [DisplayName("表面硬度")]
        public int? SurfaceHardness { get; set; }
        /// <summary>
        /// 锯口面硬度
        /// </summary>
        /// <returns></returns>
        [DisplayName("锯口面硬度")]
        public int? KerfHardness { get; set; }
        /// <summary>
        /// 硬度检测人
        /// </summary>
        /// <returns></returns>
        [DisplayName("硬度检测人")]
        public string HardnessChecker { get; set; }
        /// <summary>
        /// 火花鉴别是否合格(0-不合格，1-合格)
        /// </summary>
        /// <returns></returns>
        [DisplayName("火花鉴别是否合格(0-不合格，1-合格)")]
        public int? SparkDiscern { get; set; }
        /// <summary>
        /// 探伤级别
        /// </summary>
        /// <returns></returns>
        [DisplayName("探伤级别")]
        public string FindLevel { get; set; }
        /// <summary>
        /// 探伤检测人
        /// </summary>
        /// <returns></returns>
        [DisplayName("探伤检测人")]
        public string FindCheckUser { get; set; }
        /// <summary>
        /// 综合判定(0-不合格，1-合格)
        /// </summary>
        /// <returns></returns>
        [DisplayName("综合判定(0-不合格，1-合格)")]
        public int? MultiJudge { get; set; }
        /// <summary>
        /// 异议编号
        /// </summary>
        /// <returns></returns>
        [DisplayName("异议编号")]
        public string DiffCode { get; set; }
        /// <summary>
        /// 其它检验项目
        /// </summary>
        /// <returns></returns>
        [DisplayName("其它检验项目")]
        public string OtherItem { get; set; }
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
            this.InStockQualityId = CommonHelper.GetGuid;
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
            this.InStockQualityId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}