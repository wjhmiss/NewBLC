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
    /// Base_Supplier
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.07.04 16:25</date>
    /// </author>
    /// </summary>
    [Description("Base_Supplier")]
    [PrimaryKey("SupplierId")]
    public class Base_Supplier : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 客户主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户主键")]
        public string SupplierId { get; set; }
        /// <summary>
        /// 客户编号
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户编号")]
        public string Code { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户名称")]
        public string FullName { get; set; }
        /// <summary>
        /// 拼音
        /// </summary>
        /// <returns></returns>
        [DisplayName("拼音")]
        public string Spell { get; set; }
        /// <summary>
        /// 客户简称
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户简称")]
        public string ShortName { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        /// <returns></returns>
        [DisplayName("付款方式")]
        public string PaymentMode { get; set; }
        /// <summary>
        /// 销售大区
        /// </summary>
        /// <returns></returns>
        [DisplayName("销售大区")]
        public string Sellregion { get; set; }
        /// <summary>
        /// 销售区域
        /// </summary>
        /// <returns></returns>
        [DisplayName("销售区域")]
        public string Sellarea { get; set; }
        /// <summary>
        /// 销售人员
        /// </summary>
        /// <returns></returns>
        [DisplayName("销售人员")]
        public string Sellpeople { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        /// <returns></returns>
        [DisplayName("开户行")]
        public string BankName { get; set; }
        /// <summary>
        /// 税号
        /// </summary>
        /// <returns></returns>
        [DisplayName("税号")]
        public string TaxNumber { get; set; }
        /// <summary>
        /// 账户
        /// </summary>
        /// <returns></returns>
        [DisplayName("账户")]
        public string Account { get; set; }
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
        /// 单价信息
        /// </summary>
        /// <returns></returns>
        [DisplayName("单价信息")]
        public string PriceMessage { get; set; }
        /// <summary>
        /// 采购说明
        /// </summary>
        /// <returns></returns>
        [DisplayName("采购说明")]
        public string MateRemark { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        /// <returns></returns>
        [DisplayName("电话")]
        public string SellpeoplePhone { get; set; }
        /// <summary>
        /// 传真
        /// </summary>
        /// <returns></returns>
        [DisplayName("传真")]
        public string Fax { get; set; }
        /// <summary>
        /// 客户地址
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户地址")]
        public string Address { get; set; }
        public int? IsInvoice { get; set; }

        public string SellpeopleQQ { get; set; }
        public string SellpeopleOther { get; set; }

        public string GSPhone { get; set; }
        public string OtherPhone { get; set; }
        public string Email { get; set; }


        public decimal? GRsteelPrice7 { get; set; }
        public decimal? GYsteelPrice7 { get; set; }
        public decimal? GRsteelPrice8 { get; set; }
        public decimal? GYsteelPrice8 { get; set; }
        public decimal? JGsteelPrice { get; set; }
        public decimal? JsteelPrice { get; set; }
        public decimal? GRsteelPrice71 { get; set; }
        public decimal? GYsteelPrice71 { get; set; }
        public decimal? GRsteelPrice81 { get; set; }
        public decimal? GYsteelPrice81 { get; set; }
        public decimal? JGsteelPrice1 { get; set; }
        public decimal? JsteelPrice1 { get; set; }



        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.SupplierId = CommonHelper.GetGuid;
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
            this.SupplierId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}