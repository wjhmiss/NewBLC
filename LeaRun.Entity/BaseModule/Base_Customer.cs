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
    /// 客户信息表
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.14 16:49</date>
    /// </author>
    /// </summary>
    [Description("客户信息表")]
    [PrimaryKey("CustomerId")]
    public class Base_Customer : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 客户主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户主键")]
        public string CustomerId { get; set; }
        /// <summary>
        /// 客户编号
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户编号")]
        public string Code { get; set; }
        /// <summary>
        /// 客户密码
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户密码")]
        public string Password { get; set; }
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
        /// 客户分类
        /// </summary>
        /// <returns></returns>
        [DisplayName("客户分类")]
        public string Category { get; set; }
        /// <summary>
        /// 运输方式
        /// </summary>
        /// <returns></returns>
        [DisplayName("运输方式")]
        public string TransitMode { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        /// <returns></returns>
        [DisplayName("付款方式")]
        public string PaymentMode { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        /// <returns></returns>
        [DisplayName("邮编")]
        public string Postcode { get; set; }
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
        /// 可用性
        /// </summary>
        /// <returns></returns>
        [DisplayName("可用性")]
        public int? Usability { get; set; }
        /// <summary>
        /// 整单发运
        /// </summary>
        /// <returns></returns>
        [DisplayName("整单发运")]
        public int? FullShipping { get; set; }
        /// <summary>
        /// 材质证书
        /// </summary>
        /// <returns></returns>
        [DisplayName("材质证书")]
        public int? MaterialLCE { get; set; }
        /// <summary>
        /// 货单重印
        /// </summary>
        /// <returns></returns>
        [DisplayName("货单重印")]
        public int? InvoiceReprint { get; set; }
        /// <summary>
        /// 货单格式
        /// </summary>
        /// <returns></returns>
        [DisplayName("货单格式")]
        public string InvoiceFormat { get; set; }
        /// <summary>
        /// 加工费算法
        /// </summary>
        /// <returns></returns>
        [DisplayName("加工费算法")]
        public string ProcessingAlgorithm { get; set; }
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
        /// 总折扣率
        /// </summary>
        /// <returns></returns>
        [DisplayName("总折扣率")]
        public decimal? DiscountRate { get; set; }
        /// <summary>
        /// 总折扣率有效开始时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("总折扣率有效开始时间")]
        public DateTime? DiscountRateStartTime { get; set; }
        /// <summary>
        /// 总折扣率有效结束时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("总折扣率有效结束时间")]
        public DateTime? DiscountRateEndTime { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        /// <returns></returns>
        [DisplayName("税率")]
        public decimal? TaxRate { get; set; }
        /// <summary>
        /// 管理费
        /// </summary>
        /// <returns></returns>
        [DisplayName("管理费")]
        public decimal? ManagementCostRate { get; set; }
        /// <summary>
        /// 单价信息
        /// </summary>
        /// <returns></returns>
        [DisplayName("单价信息")]
        public string PriceMessage { get; set; }
        /// <summary>
        /// 用料说明
        /// </summary>
        /// <returns></returns>
        [DisplayName("用料说明")]
        public string MateRemark { get; set; }
        /// <summary>
        /// 硬料加工要求
        /// </summary>
        /// <returns></returns>
        [DisplayName("硬料加工要求")]
        public string YLrequire { get; set; }
        /// <summary>
        /// 软料加工要求
        /// </summary>
        /// <returns></returns>
        [DisplayName("软料加工要求")]
        public string RLrequire { get; set; }
        /// <summary>
        /// 精料加工要求
        /// </summary>
        /// <returns></returns>
        [DisplayName("精料加工要求")]
        public string JLrequire { get; set; }

        /// <summary>
        /// 打底重量
        /// </summary>
        /// <returns></returns>
        [DisplayName("打底重量")]
        public decimal? LimitWeight { get; set; }
        /// <summary>
        /// 打底厚度
        /// </summary>
        /// <returns></returns>
        [DisplayName("打底厚度")]
        public decimal? LimitThick { get; set; }
        /// <summary>
        /// 外发加工硬料价格
        /// </summary>
        /// <returns></returns>
        [DisplayName("外发加工硬料价格")]
        public decimal? YBeOutAreaPrice { get; set; }
        /// <summary>
        /// 外发加工软料价格
        /// </summary>
        /// <returns></returns>
        [DisplayName("外发加工软料价格")]
        public decimal? RBeOutAreaPrice { get; set; }

        /// <summary>
        /// 计价余量
        /// </summary>
        /// <returns></returns>
        [DisplayName("计价余量")]
        public decimal? Addrequire { get; set; }
        /// <summary>
        /// 毛料备注
        /// </summary>
        /// <returns></returns>
        [DisplayName("毛料备注")]
        public string Mrequire { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        /// <returns></returns>
        [DisplayName("公司代码")]
        public int? PrintTypeGS { get; set; }
        /// <summary>
        /// 模号
        /// </summary>
        /// <returns></returns>
        [DisplayName("模号")]
        public int? PrintTypeMO { get; set; }
        [DisplayName("配件号")]
        public int? PrintTypCode { get; set; }
        /// <summary>
        /// 料件编号
        /// </summary>
        /// <returns></returns>
        [DisplayName("料件编号")]
        public int? PrintTypeID { get; set; }
        /// <summary>
        /// 材质
        /// </summary>
        /// <returns></returns>
        [DisplayName("材质")]
        public int? PrintTypeST { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        /// <returns></returns>
        [DisplayName("规格")]
        public int? PrintTypeSZ { get; set; }
        /// <summary>
        /// 对账人
        /// </summary>
        /// <returns></returns>
        [DisplayName("对账人")]
        public string CheckMan { get; set; }
        /// <summary>
        /// 对账传真
        /// </summary>
        /// <returns></returns>
        [DisplayName("对账传真")]
        public string CheckFax { get; set; }
        /// <summary>
        /// 对账电话
        /// </summary>
        /// <returns></returns>
        [DisplayName("对账电话")]
        public string CheckTel { get; set; }
        /// <summary>
        /// 打底重量
        /// </summary>
        /// <returns></returns>
        [DisplayName("打底重量")]
        public decimal? LimitWeightGB { get; set; }
        /// <summary>
        /// 打底厚度
        /// </summary>
        /// <returns></returns>
        [DisplayName("打底厚度")]
        public decimal? LimitThickGB { get; set; }
        /// <summary>
        /// 送货单备注
        /// </summary>
        /// <returns></returns>
        [DisplayName("送货单备注")]
        public string DeliveryRemark { get; set; }

        /// <summary>
        /// 报价模式(材料+加工费)
        /// </summary>
        /// <returns></returns>
        [DisplayName("报价模式")]
        public int? ByPrice { get; set; }
        /// <summary>
        /// 打底厚度
        /// </summary>
        /// <returns></returns>
        [DisplayName("打底厚度")]
        public decimal? SmallKG { get; set; }

        /// <summary>
        /// 锯工
        /// </summary>
        /// <returns></returns>
        [DisplayName("锯工")]
        public decimal? JKCost { get; set; }
        /// <summary>
        /// </summary>
        /// <returns></returns>
        [DisplayName("含税")]
        public int? IsInvoice { get; set; }


        /// <summary>
        /// 送货单备注
        /// </summary>
        /// <returns></returns>
        [DisplayName("送货单备注")]
        public string Mfang { get; set; }
        /// <summary>
        /// 送货单备注
        /// </summary>
        /// <returns></returns>
        [DisplayName("送货单备注")]
        public string Gfang { get; set; }
        /// <summary>
        /// 送货单备注
        /// </summary>
        /// <returns></returns>
        [DisplayName("送货单备注")]
        public string Jfang { get; set; }

        /// <summary>
        /// 打底厚度
        /// </summary>
        /// <returns></returns>
        [DisplayName("打底厚度")]
        public decimal? MCKAdd { get; set; }
        /// <summary>
        /// 打底厚度
        /// </summary>
        /// <returns></returns>
        [DisplayName("打底厚度")]
        public decimal? JCKAdd { get; set; }
        /// <summary>
        /// 打底厚度
        /// </summary>
        /// <returns></returns>
        [DisplayName("打底厚度")]
        public decimal? GCKAdd { get; set; }
        public decimal? XYmoney { get; set; }

        
        public string Leader { get; set; }
        public string LeaderRole { get; set; }
        public string Checker { get; set; }
        public string CheckRole { get; set; }
        public string LeaderTele { get; set; }
        public string LeaderMobile { get; set; }
        public string CheckFaxs { get; set; }
        public string CheckQQ { get; set; }
        public string LeaderMail { get; set; }
        public string LeaderQQ { get; set; }
        public string CheckTele { get; set; }
        public string CheckMobile { get; set; }
        public string LeaderOther { get; set; }
        public string LeaderOtherRole { get; set; }
        public string CheckOther { get; set; }
        public string CheckOtherRole { get; set; }
        public string LeaderOtherTele { get; set; }
        public string LeaderOtherMobile { get; set; }
        public string CheckOtherTele { get; set; }
        public string CheckOtherMobile { get; set; }
        public string LeaderRemark { get; set; }
        public string CheckRemark { get; set; }
        public int Awayfrom { get; set; }

        /// <summary>
        /// 0：按余量算价格，1：按净重算价格
        /// </summary>
        public int Calc { get; set; }

        public decimal MiniThick { get; set; }

        public string CompanyName { get; set; }
        
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.CustomerId = CommonHelper.GetGuid;
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
            this.CustomerId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}