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
    /// 机床设置
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.05.06 10:21</date>
    /// </author>
    /// </summary>
    [Description("机床设置")]
    [PrimaryKey("MachineID")]
    public class BY_Machine : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// id主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("id主键")]
        public string MachineID { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        /// <returns></returns>
        [DisplayName("设备编号")]
        public string MachineCode { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        /// <returns></returns>
        [DisplayName("设备名称")]
        public string MachineName { get; set; }
        /// <summary>
        /// MachineIP
        /// </summary>
        /// <returns></returns>
        [DisplayName("MachineIP")]
        public string MachineIP { get; set; }
        /// <summary>
        /// 订单行
        /// </summary>
        /// <returns></returns>
        [DisplayName("订单行")]
        public string OrderCodeLine { get; set; }

        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.MachineID = CommonHelper.GetGuid;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.MachineID = KeyValue;
        }
        #endregion
    }
}