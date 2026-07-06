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
    /// 不合格品通知单
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.01.29 13:38</date>
    /// </author>
    /// </summary>
    [Description("不合格品通知单")]
    [PrimaryKey("FlawedNoticeId")]
    public class BY_FlawedNotice : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 不合格品通知单ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("不合格品通知单ID")]
        public string FlawedNoticeId { get; set; }
        /// <summary>
        /// 单号
        /// </summary>
        /// <returns></returns>
        [DisplayName("单号")]
        public string BillCode { get; set; }
        /// <summary>
        /// 订单明细ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("订单明细ID")]
        public string OrderDetailId { get; set; }
        /// <summary>
        /// 订单号（单号+行号）
        /// </summary>
        /// <returns></returns>
        [DisplayName("订单号（单号+行号）")]
        public string OrderBarcode { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        /// <returns></returns>
        [DisplayName("数量")]
        public decimal? Qty { get; set; }
        /// <summary>
        /// 开单日期
        /// </summary>
        /// <returns></returns>
        [DisplayName("开单日期")]
        public DateTime? WriteDate { get; set; }
        /// <summary>
        /// 开单人工号
        /// </summary>
        /// <returns></returns>
        [DisplayName("开单人工号")]
        public string WriteUserNo { get; set; }
        /// <summary>
        /// 开单人姓名
        /// </summary>
        /// <returns></returns>
        [DisplayName("开单人姓名")]
        public string WriteUserName { get; set; }
        /// <summary>
        /// 开单部门
        /// </summary>
        /// <returns></returns>
        [DisplayName("开单部门")]
        public string WriteDepart { get; set; }
        /// <summary>
        /// 处理日期
        /// </summary>
        /// <returns></returns>
        [DisplayName("处理日期")]
        public DateTime? DealDate { get; set; }
        /// <summary>
        /// 处理人工号
        /// </summary>
        /// <returns></returns>
        [DisplayName("处理人工号")]
        public string DealUserNo { get; set; }
        /// <summary>
        /// 处理人姓名
        /// </summary>
        /// <returns></returns>
        [DisplayName("处理人姓名")]
        public string DealUserName { get; set; }
        /// <summary>
        /// 处理部门
        /// </summary>
        /// <returns></returns>
        [DisplayName("处理部门")]
        public string DealDepart { get; set; }
        /// <summary>
        /// 超差工序
        /// </summary>
        /// <returns></returns>
        [DisplayName("超差工序")]
        public string ErrorProcess { get; set; }
        /// <summary>
        /// 责任主管工号
        /// </summary>
        /// <returns></returns>
        [DisplayName("责任主管工号")]
        public string DirectorNo { get; set; }
        /// <summary>
        /// 主管姓名
        /// </summary>
        /// <returns></returns>
        [DisplayName("主管姓名")]
        public string DirectorName { get; set; }
        /// <summary>
        /// 责任部门
        /// </summary>
        /// <returns></returns>
        [DisplayName("责任部门")]
        public string DutyDepart { get; set; }
        /// <summary>
        /// 机床编号
        /// </summary>
        /// <returns></returns>
        [DisplayName("机床编号")]
        public string MachineCode { get; set; }
        /// <summary>
        /// 责任组长姓名
        /// </summary>
        /// <returns></returns>
        [DisplayName("责任组长姓名")]
        public string DutyHeadman { get; set; }
        /// <summary>
        /// 发现工序
        /// </summary>
        /// <returns></returns>
        [DisplayName("发现工序")]
        public string CatchProcess { get; set; }
        /// <summary>
        /// 是否联客
        /// </summary>
        /// <returns></returns>
        [DisplayName("是否联客")]
        public int? IsRelationCust { get; set; }
        /// <summary>
        /// 责任人姓名
        /// </summary>
        /// <returns></returns>
        [DisplayName("责任人姓名")]
        public string DutyUserName { get; set; }
        /// <summary>
        /// 责任班次
        /// </summary>
        /// <returns></returns>
        [DisplayName("责任班次")]
        public string DutyClass { get; set; }
        /// <summary>
        /// 不合格品情况代码
        /// </summary>
        /// <returns></returns>
        [DisplayName("不合格品情况代码")]
        public string SituationCode { get; set; }
        /// <summary>
        /// 不合格品原因代码
        /// </summary>
        /// <returns></returns>
        [DisplayName("不合格品原因代码")]
        public string ReasonCode { get; set; }
        /// <summary>
        /// 处理结果代码
        /// </summary>
        /// <returns></returns>
        [DisplayName("处理结果代码")]
        public string DealResultCode { get; set; }
        /// <summary>
        /// 责任人工号
        /// </summary>
        /// <returns></returns>
        [DisplayName("责任人工号")]
        public string DutyUserNo { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        /// <returns></returns>
        [DisplayName("备注信息")]
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
        /// 责任组长工号
        /// </summary>
        /// <returns></returns>
        [DisplayName("责任组长工号")]
        public string DutyHeadNo { get; set; }
        /// <summary>
        /// 补料单ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("补料单ID")]
        public string AddOrderId { get; set; }
        /// <summary>
        /// 补料单号
        /// </summary>
        /// <returns></returns>
        [DisplayName("补料单号")]
        public string AddOrderCode { get; set; }

        public decimal? FMoney { get; set; }


        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.FlawedNoticeId = CommonHelper.GetGuid;
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
            this.FlawedNoticeId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}