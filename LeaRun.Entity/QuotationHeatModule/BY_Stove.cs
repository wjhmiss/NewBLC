//=====================================================================================
// All Rights Reserved , Copyright @ DNS 2016
// Software Developers @ DNS 2016
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
    /// BY_Stove
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.12.24 13:16</date>
    /// </author>
    /// </summary>
    [Description("BY_Stove")]
    [PrimaryKey("StoveId")]
    public class BY_Stove : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// StoveId
        /// </summary>
        /// <returns></returns>
        [DisplayName("StoveId")]
        public string StoveId { get; set; }
        /// <summary>
        /// StoveCode
        /// </summary>
        /// <returns></returns>
        [DisplayName("StoveCode")]
        public string StoveCode { get; set; }
        /// <summary>
        /// StoveNum
        /// </summary>
        /// <returns></returns>
        [DisplayName("StoveNum")]
        public string StoveNum { get; set; }
        /// <summary>
        /// StartDateTime
        /// </summary>
        /// <returns></returns>
        [DisplayName("StartDateTime")]
        public DateTime? StartDateTime { get; set; }
        /// <summary>
        /// MethodBuilder
        /// </summary>
        /// <returns></returns>
        [DisplayName("MethodBuilder")]
        public string MethodBuilder { get; set; }
        /// <summary>
        /// OperatorName
        /// </summary>
        /// <returns></returns>
        [DisplayName("OperatorName")]
        public string OperatorName { get; set; }
        /// <summary>
        /// StoveType
        /// </summary>
        /// <returns></returns>
        [DisplayName("StoveType")]
        public string StoveType { get; set; }
        /// <summary>
        /// BuildMethod
        /// </summary>
        /// <returns></returns>
        [DisplayName("BuildMethod")]
        public string BuildMethod { get; set; }
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

        public string H1Time1 { get; set; }
        public string H1Code1 { get; set; }
        public string H1Temprature1 { get; set; }
        public string H1Time2 { get; set; }
        public string H1Code2 { get; set; }
        public string H1Temprature2 { get; set; }
        public string H1Time3 { get; set; }
        public string H1Code3 { get; set; }
        public string H1Temprature3 { get; set; }
        public string H1Time4 { get; set; }
        public string H1Code4 { get; set; }
        public string H1Temprature4 { get; set; }
        public string H1Time5 { get; set; }
        public string H1Code5 { get; set; }
        public string H1Temprature5 { get; set; }
        public string H1Time6 { get; set; }
        public string H1Code6 { get; set; }
        public string H1Temprature6 { get; set; }
        public string H1Time7 { get; set; }
        public string H1Code7 { get; set; }
        public string H1Temprature7 { get; set; }
        public string H1Time8 { get; set; }
        public string H1Code8 { get; set; }
        public string H1Temprature8 { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Weight { get; set; }

        public DateTime? SDDateTime { get; set; }
        public DateTime? DWDateTime { get; set; }
        public DateTime? CLDateTime { get; set; }
        public string OPname { get; set; }



        public string H1Time9 { get; set; }
        public string H1Code9 { get; set; }
        public string H1Temprature9 { get; set; }
        public string H1Time10 { get; set; }
        public string H1Code10 { get; set; }
        public string H1Temprature10 { get; set; }
        public string H1q1 { get; set; }
        public string H1d1 { get; set; }
        public string H1z1 { get; set; }
        public string H1a1 { get; set; }
        public string H1q2 { get; set; }
        public string H1d2 { get; set; }
        public string H1z2 { get; set; }
        public string H1a2 { get; set; }
        public string H1q3 { get; set; }
        public string H1d3 { get; set; }
        public string H1z3 { get; set; }
        public string H1a3 { get; set; }
        public string H1q4 { get; set; }
        public string H1d4 { get; set; }
        public string H1z4 { get; set; }
        public string H1a4 { get; set; }
        public string H1q5 { get; set; }
        public string H1d5 { get; set; }
        public string H1z5 { get; set; }
        public string H1a5 { get; set; }
        public string H1q6 { get; set; }
        public string H1d6 { get; set; }
        public string H1z6 { get; set; }
        public string H1a6 { get; set; }
        public string H1q7 { get; set; }
        public string H1d7 { get; set; }
        public string H1z7 { get; set; }
        public string H1a7 { get; set; }
        public string H1q8 { get; set; }
        public string H1d8 { get; set; }
        public string H1z8 { get; set; }
        public string H1a8 { get; set; }
        public string H1q9 { get; set; }
        public string H1d9 { get; set; }
        public string H1z9 { get; set; }
        public string H1a9 { get; set; }
        public string H1q10 { get; set; }
        public string H1d10 { get; set; }
        public string H1z10 { get; set; }
        public string H1a10 { get; set; }


        public string yd { get; set; }
        public string gp { get; set; }
        public string qx { get; set; }
        public string fsk { get; set; }
        public string pz { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.StoveId = CommonHelper.GetGuid;
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
            this.StoveId = KeyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}