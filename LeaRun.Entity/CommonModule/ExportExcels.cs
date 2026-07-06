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
    /// ExportExcels
    /// <author>
    ///		<name>she</name>
    ///		<date>2015.04.24 20:00</date>
    /// </author>
    /// </summary>
    [Description("ExportExcels")]
    [PrimaryKey("GUID")]
    public class ExportExcels : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// GUID
        /// </summary>
        /// <returns></returns>
        [DisplayName("GUID")]
        public string GUID { get; set; }
        /// <summary>
        /// JsonColumn
        /// </summary>
        /// <returns></returns>
        [DisplayName("JsonColumn")]
        public string JsonColumn { get; set; }
        /// <summary>
        /// JsonData
        /// </summary>
        /// <returns></returns>
        [DisplayName("JsonData")]
        public string JsonData { get; set; }
        /// <summary>
        /// ExportField
        /// </summary>
        /// <returns></returns>
        [DisplayName("ExportField")]
        public string ExportField { get; set; }
        /// <summary>
        /// fileName
        /// </summary>
        /// <returns></returns>
        [DisplayName("fileName")]
        public string fileName { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.GUID = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.GUID = KeyValue;
                                            }
        #endregion
    }
}