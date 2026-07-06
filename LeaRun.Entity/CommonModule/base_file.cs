//=====================================================================================
// All Rights Reserved , Copyright @ DNS 2022
// Software Developers @ DNS 2022
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
    /// base_file
    /// <author>
    ///		<name>she</name>
    ///		<date>2022.05.01 18:25</date>
    /// </author>
    /// </summary>
    [Description("base_file")]
    [PrimaryKey("Id")]
    public class base_file : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// Id
        /// </summary>
        /// <returns></returns>
        [DisplayName("Id")]
        public string Id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        /// <returns></returns>
        [DisplayName("Name")]
        public string Name { get; set; }
        /// <summary>
        /// Size
        /// </summary>
        /// <returns></returns>
        [DisplayName("Size")]
        public string Size { get; set; }
        /// <summary>
        /// Type
        /// </summary>
        /// <returns></returns>
        [DisplayName("Type")]
        public string Type { get; set; }
        /// <summary>
        /// Eextension
        /// </summary>
        /// <returns></returns>
        [DisplayName("Eextension")]
        public string Eextension { get; set; }
        /// <summary>
        /// CreateDate
        /// </summary>
        /// <returns></returns>
        [DisplayName("CreateDate")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// ModifyDate
        /// </summary>
        /// <returns></returns>
        [DisplayName("ModifyDate")]
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// Path
        /// </summary>
        /// <returns></returns>
        [DisplayName("Path")]
        public string Path { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        /// <returns></returns>
        [DisplayName("Status")]
        public string Status { get; set; }
        /// <summary>
        /// AttachId
        /// </summary>
        /// <returns></returns>
        [DisplayName("AttachId")]
        public string AttachId { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.Id = CommonHelper.GetGuid;
            if (this.CreateDate == null)
            {
                this.CreateDate = DateTime.Now;
            }
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.Id = KeyValue;
            this.ModifyDate = DateTime.Now;
                                }
        #endregion
    }
}