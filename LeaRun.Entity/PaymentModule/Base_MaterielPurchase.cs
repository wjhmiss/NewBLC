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
    /// 物料购买申请
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.07.10 11:52</date>
    /// </author>
    /// </summary>
    [Description("物料购买申请")]
    [PrimaryKey("MaterielPurchaseId")]
    public class Base_MaterielPurchase : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 物料id
        /// </summary>
        /// <returns></returns>
        [DisplayName("物料id")]
        public string MaterielPurchaseId { get; set; }
        /// <summary>
        /// 物料号
        /// </summary>
        /// <returns></returns>
        [DisplayName("物料号")]
        public string MaterielPurchaseCode { get; set; }
        /// <summary>
        /// 物料id
        /// </summary>
        /// <returns></returns>
        [DisplayName("物料id")]
        public string MaterielId { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        /// <returns></returns>
        [DisplayName("物料名称")]
        public string MaterielName { get; set; }
        /// <summary>
        /// 物料号
        /// </summary>
        /// <returns></returns>
        [DisplayName("物料号")]
        public string MaterielCode { get; set; }
        /// <summary>
        /// 物料类型
        /// </summary>
        /// <returns></returns>
        [DisplayName("物料类型")]
        public string MaterielType { get; set; }
        /// <summary>
        /// 尺寸
        /// </summary>
        /// <returns></returns>
        [DisplayName("尺寸")]
        public string MaterielSize { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        /// <returns></returns>
        [DisplayName("单位")]
        public string Unit { get; set; }
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
        /// 单价
        /// </summary>
        /// <returns></returns>
        [DisplayName("单价")]
        public decimal? PricePer { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        /// <returns></returns>
        [DisplayName("数量")]
        public decimal? Qty { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        /// <returns></returns>
        [DisplayName("总金额")]
        public decimal? TotalAmount { get; set; }
        /// <summary>
        /// 报警库存
        /// </summary>
        /// <returns></returns>
        [DisplayName("报警库存")]
        public int? AlarmNumber { get; set; }
        /// <summary>
        /// 状态0未审核，1已审核，-1拒绝
        /// </summary>
        /// <returns></returns>
        [DisplayName("状态0未审核，1已审核，-1拒绝")]
        public int? Status { get; set; }
        /// <summary>
        /// 已付款
        /// </summary>
        /// <returns></returns>
        [DisplayName("已付款")]
        public int? IsPay { get; set; }
        /// <summary>
        /// 已开票
        /// </summary>
        /// <returns></returns>
        [DisplayName("已开票")]
        public int? IsInvoice { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        /// <returns></returns>
        [DisplayName("支付方式")]
        public string PayType { get; set; }
        /// <summary>
        /// 员工ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("员工ID")]
        public string WorkerId { get; set; }
        /// <summary>
        /// 员工姓名
        /// </summary>
        /// <returns></returns>
        [DisplayName("员工姓名")]
        public string WorkerName { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        /// <returns></returns>
        [DisplayName("工号")]
        public string WorkerCode { get; set; }
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
        /// 部门
        /// </summary>
        /// <returns></returns>
        [DisplayName("部门")]
        public string DepartmentName { get; set; }

        /// <summary>
        /// 对账单
        /// </summary>
        /// <returns></returns>
        [DisplayName("对账单")]
        public string PayCheckSheetCode { get; set; }

        /// <summary>
        /// 已获金额
        /// </summary>
        /// <returns></returns>
        [DisplayName("已获金额")]
        public decimal? GetedAmount { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.MaterielPurchaseId = CommonHelper.GetGuid;
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
            this.MaterielPurchaseId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}