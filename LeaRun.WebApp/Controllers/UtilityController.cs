using DNS.Business;
using DNS.DataAccess;
using DNS.Entity;
using DNS.Entity.EntityModel;
using DNS.Repository;
using DNS.Utilities;
using Newtonsoft.Json;
using Senparc.Weixin.MP.AdvancedAPIs.CustomService;
using Svg;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace DNS.WebApp.Controllers
{
    /// <summary>
    /// 公共控制器
    /// </summary>
    public class UtilityController : Controller
    {
        public ActionResult GetImg()
        {
            Bitmap bmp = new Bitmap(100, 35);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            g.FillRectangle(Brushes.Red, 2, 2, 65, 31);
            g.DrawString("学习MVC", new Font("黑体", 15f), Brushes.Yellow, new PointF(5f, 5f));
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            g.Dispose();
            bmp.Dispose();
            return File(ms.ToArray(), "image/jpeg");
        }

        public ActionResult DeleteFile(string Path)
        {
            try
            {
                string FilePath = this.Server.MapPath(Path);
                if (System.IO.File.Exists(FilePath))
                    System.IO.File.Delete(FilePath);
                return Content(new JsonMessage { Success = true, Code = "1", Message = "删除成功" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        public ActionResult UploadJs(HttpPostedFileBase Filedata, string saveDocument, string batchcode = "")
        {
            try
            {
                FileProperty fileproperty = new FileProperty();
                string evalfilename = Filedata.FileName;
                evalfilename = RemoveChars(evalfilename);

                //没有文件上传，直接返回
                if (Filedata == null || string.IsNullOrEmpty(evalfilename) || Filedata.ContentLength == 0)
                {
                    return HttpNotFound();
                }
                //获取文件完整文件名(包含绝对路径)
                //文件存放路径格式：/Resource/Document/Email/{日期}/{guid}.{后缀名}
                //例如：/Resource/Document/Email/20130913/43CA215D947F8C1F1DDFCED383C4D706.jpg
                string fileGuid = CommonHelper.GetGuid;
                long filesize = Filedata.ContentLength;
                //string FileEextension = Path.GetExtension(evalfilename);
                string FileEextension = ".jpg";
                string uploadDate = DateTime.Now.ToString("yyyyMMdd");

                string virtualPath = string.Format("~/Resource/Document/{0}/{1}{2}", saveDocument, batchcode, FileEextension);
                string fullFileName = this.Server.MapPath(virtualPath);
                //创建文件夹，保存文件
                string path = Path.GetDirectoryName(fullFileName);

                string FilePath = this.Server.MapPath(virtualPath);
                if (System.IO.File.Exists(FilePath))
                    System.IO.File.Delete(FilePath);

                Directory.CreateDirectory(path);
                if (!System.IO.File.Exists(fullFileName))
                {
                    Filedata.SaveAs(fullFileName);
                    fileproperty.Id = fileGuid;
                    fileproperty.Name = evalfilename;
                    fileproperty.Eextension = FileEextension;
                    fileproperty.CreateDate = DateTime.Now;
                    fileproperty.Path = virtualPath;
                    fileproperty.Size = SizeHelper.CountSize(filesize);
                }
                return Content(fileproperty.ToJson());
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult Upload(HttpPostedFileBase Filedata, string saveDocument, string filenameid = "")
        {
            try
            {
                if (saveDocument.IndexOf("自动") > 0 || filenameid == "" || saveDocument == "LJ/")
                {
                    return Content("-1");
                }
                FileProperty fileproperty = new FileProperty();
                string evalfilename = Filedata.FileName;
                evalfilename = RemoveChars(evalfilename);
                //没有文件上传，直接返回
                if (Filedata == null || string.IsNullOrEmpty(evalfilename) || Filedata.ContentLength == 0)
                {
                    return Content("-1");
                }
                string FileEextension = Path.GetExtension(evalfilename);
                string virtualPath = string.Format("~/Resource/Document/{0}/{1}", saveDocument, evalfilename);
                if (saveDocument == "Stove" || saveDocument == "HeatReport" || saveDocument == "BY_FKReport" || saveDocument == "BY_CJReport" || saveDocument == "BY_CSBReport" || saveDocument == "BY_JXReport")
                {
                    virtualPath = string.Format("~/Resource/Document/{0}/{1}.jpg", saveDocument, filenameid);
                }
                if (saveDocument == "Customer")
                {
                    virtualPath = string.Format("~/Resource/Document/{0}/{1}/{2}", saveDocument, filenameid, evalfilename);
                }


                //获取文件完整文件名(包含绝对路径)
                //文件存放路径格式：/Resource/Document/Email/{日期}/{guid}.{后缀名}
                //例如：/Resource/Document/Email/20130913/43CA215D947F8C1F1DDFCED383C4D706.jpg
                string fileGuid = CommonHelper.GetGuid;
                long filesize = Filedata.ContentLength;
                //string FileEextension = ".jpg";
                string uploadDate = DateTime.Now.ToString("yyyyMMdd");

                string fullFileName = this.Server.MapPath(virtualPath);
                //创建文件夹，保存文件
                string path = Path.GetDirectoryName(fullFileName);

                string FilePath = this.Server.MapPath(virtualPath);
                if (System.IO.File.Exists(FilePath))
                    System.IO.File.Delete(FilePath);
                //Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", FilePath);

                Directory.CreateDirectory(path);
                if (!System.IO.File.Exists(fullFileName))
                {
                    Filedata.SaveAs(fullFileName);
                    fileproperty.Id = fileGuid;
                    fileproperty.Name = evalfilename;
                    fileproperty.Eextension = FileEextension;
                    fileproperty.CreateDate = DateTime.Now;
                    fileproperty.Path = virtualPath;
                    fileproperty.Size = SizeHelper.CountSize(filesize);
                }

                BY_QuotationHeat heat = DataFactory.Database().FindEntity<BY_QuotationHeat>(filenameid);
                if (heat != null && heat.CustomerId != null && saveDocument != "HeatReport")
                {
                    base_file baseFile = DataFactory.Database().FindEntityBySql<base_file>("select top 1 * from base_file where name = '" + evalfilename + "' and AttachId = '" + filenameid + "'");
                    if (baseFile != null && baseFile.Id != null)
                    {
                        baseFile.Name = fileproperty.Name;
                        baseFile.Eextension = fileproperty.Eextension;
                        baseFile.CreateDate = fileproperty.CreateDate;
                        baseFile.Path = fileproperty.Path;
                        baseFile.Size = fileproperty.Size;
                        baseFile.Type = FileEextension.ToLower().IndexOf("jpg") > 0 || FileEextension.ToLower().IndexOf("png") > 0 || FileEextension.ToLower().IndexOf("bmp") > 0
                            || FileEextension.ToLower().IndexOf("svg") > 0 || FileEextension.ToLower().IndexOf("gif") > 0 || FileEextension.ToLower().IndexOf("tga") > 0
                              ? "图片" : "其他";  // bmp，jpg，png，tif，gif
                        baseFile.Status = baseFile.Type == "图片" ?
                            "<a  style='display: inline;color:blue; ' href='/Utility/DownFile?fileName=" + fileproperty.Name + "&filePath=/Resource/Document/RCL/" + filenameid + "/" + fileproperty.Name + "' target='_self'><img style='width:30px;display: inline;' src='../../Resource/Document/RCL/" + filenameid + "/" + fileproperty.Name + "?" + new Random().Next() + "'  onMouseOver='toolTip(\"<img style=width:600px src=../../Resource/Document/RCL/" + filenameid + "/" + fileproperty.Name + "?" + new Random().Next() + ">\")' onMouseOut='toolTip()' /></a>"
                            : "<a  style='display: inline;color:blue; ' href='/Utility/DownFile?fileName=" + fileproperty.Name + "&filePath=/Resource/Document/RCL/" + filenameid + "/" + fileproperty.Name + "' target='_self'>" + fileproperty.Name + "</a>";
                        baseFile.AttachId = filenameid;
                        DataFactory.Database().Update(baseFile);
                    }
                    else
                    {
                        baseFile.Create();
                        baseFile.Name = fileproperty.Name;
                        baseFile.Eextension = fileproperty.Eextension;
                        baseFile.ModifyDate = DateTime.Now;
                        baseFile.Path = fileproperty.Path;
                        baseFile.Size = fileproperty.Size;
                        baseFile.Type = FileEextension.ToLower().IndexOf("jpg") > 0 || FileEextension.ToLower().IndexOf("png") > 0 || FileEextension.ToLower().IndexOf("bmp") > 0
                            || FileEextension.ToLower().IndexOf("svg") > 0 || FileEextension.ToLower().IndexOf("gif") > 0 || FileEextension.ToLower().IndexOf("tga") > 0
                              ? "图片" : "其他";  // bmp，jpg，png，tif，gif
                        baseFile.Status = baseFile.Type == "图片" ?
                       "<a  style='display: inline;color:blue; ' href='/Utility/DownFile?fileName=" + fileproperty.Name + "&filePath=/Resource/Document/RCL/" + filenameid + "/" + fileproperty.Name + "' target='_self'><img style='width:30px;display: inline;' src='../../Resource/Document/RCL/" + filenameid + "/" + fileproperty.Name + "?" + new Random().Next() + "'  onMouseOver='toolTip(\"<img style=width:600px src=../../Resource/Document/RCL/" + filenameid + "/" + fileproperty.Name + "?" + new Random().Next() + ">\")' onMouseOut='toolTip()' /></a>"
                       : "<a  style='display: inline;color:blue; ' href='/Utility/DownFile?fileName=" + fileproperty.Name + "&filePath=/Resource/Document/RCL/" + filenameid + "/" + fileproperty.Name + "' target='_self'>" + fileproperty.Name + "</a>";
                        baseFile.AttachId = filenameid;
                        DataFactory.Database().Insert(baseFile);
                    }
                }




                Base_Customer base_customer = DataFactory.Database().FindEntity<Base_Customer>(filenameid);
                if (base_customer != null && base_customer.CustomerId != null)
                {
                    base_file baseFile = DataFactory.Database().FindEntityBySql<base_file>("select top 1 * from base_file where name = '" + evalfilename + "' and AttachId = '" + filenameid + "'");
                    if (baseFile != null && baseFile.Id != null)
                    {
                        baseFile.Name = fileproperty.Name;
                        baseFile.Eextension = fileproperty.Eextension;
                        baseFile.CreateDate = fileproperty.CreateDate;
                        baseFile.Path = fileproperty.Path;
                        baseFile.Size = fileproperty.Size;
                        baseFile.Type = FileEextension.ToLower().IndexOf("jpg") > 0 || FileEextension.ToLower().IndexOf("png") > 0 || FileEextension.ToLower().IndexOf("bmp") > 0
                            || FileEextension.ToLower().IndexOf("svg") > 0 || FileEextension.ToLower().IndexOf("gif") > 0 || FileEextension.ToLower().IndexOf("tga") > 0
                              ? "图片" : "其他";  // bmp，jpg，png，tif，gif
                        baseFile.Status = baseFile.Type == "图片" ?
                            "<a  style='display: inline;color:blue; ' href='/Utility/DownFile?fileName=" + fileproperty.Name + "&filePath=/Resource/Document/Customer/" + filenameid + "/" + fileproperty.Name + "' target='_self'><img style='width:30px;display: inline;' src='../../Resource/Document/Customer/" + filenameid + "/" + fileproperty.Name + "?" + new Random().Next() + "'  onMouseOver='toolTip(\"<img style=width:600px src=../../Resource/Document/Customer/" + filenameid + "/" + fileproperty.Name + "?" + new Random().Next() + ">\")' onMouseOut='toolTip()' /></a>"
                            : "<a  style='display: inline;color:blue; ' href='/Utility/DownFile?fileName=" + fileproperty.Name + "&filePath=/Resource/Document/Customer/" + filenameid + "/" + fileproperty.Name + "' target='_self'>" + fileproperty.Name + "</a>";
                        baseFile.AttachId = filenameid;
                        DataFactory.Database().Update(baseFile);
                    }
                    else
                    {
                        baseFile.Create();
                        baseFile.Name = fileproperty.Name;
                        baseFile.Eextension = fileproperty.Eextension;
                        baseFile.ModifyDate = DateTime.Now;
                        baseFile.Path = fileproperty.Path;
                        baseFile.Size = fileproperty.Size;
                        baseFile.Type = FileEextension.ToLower().IndexOf("jpg") > 0 || FileEextension.ToLower().IndexOf("png") > 0 || FileEextension.ToLower().IndexOf("bmp") > 0
                            || FileEextension.ToLower().IndexOf("svg") > 0 || FileEextension.ToLower().IndexOf("gif") > 0 || FileEextension.ToLower().IndexOf("tga") > 0
                              ? "图片" : "其他";  // bmp，jpg，png，tif，gif
                        baseFile.Status = baseFile.Type == "图片" ?
                       "<a  style='display: inline;color:blue; ' href='/Utility/DownFile?fileName=" + fileproperty.Name + "&filePath=/Resource/Document/Customer/" + filenameid + "/" + fileproperty.Name + "' target='_self'><img style='width:30px;display: inline;' src='../../Resource/Document/Customer/" + filenameid + "/" + fileproperty.Name + "?" + new Random().Next() + "'  onMouseOver='toolTip(\"<img style=width:600px src=../../Resource/Document/Customer/" + filenameid + "/" + fileproperty.Name + "?" + new Random().Next() + ">\")' onMouseOut='toolTip()' /></a>"
                       : "<a  style='display: inline;color:blue; ' href='/Utility/DownFile?fileName=" + fileproperty.Name + "&filePath=/Resource/Document/Customer/" + filenameid + "/" + fileproperty.Name + "' target='_self'>" + fileproperty.Name + "</a>";
                        baseFile.AttachId = filenameid;
                        DataFactory.Database().Insert(baseFile);
                    }
                }






                BY_OrderDetailProcess by_orderdetailprocess = DataFactory.Database().FindEntity<BY_OrderDetailProcess>(filenameid);
                if (by_orderdetailprocess != null && by_orderdetailprocess.OrderDetailProcessId != null)
                {
                    base_file baseFile = DataFactory.Database().FindEntityBySql<base_file>("select top 1 * from base_file where name = '" + evalfilename + "' and AttachId = '" + filenameid + "'");
                    if (baseFile != null && baseFile.Id != null)
                    {
                        baseFile.Name = fileproperty.Name;
                        baseFile.Eextension = fileproperty.Eextension;
                        baseFile.CreateDate = fileproperty.CreateDate;
                        baseFile.Path = fileproperty.Path;
                        baseFile.Size = fileproperty.Size;
                        baseFile.Type = FileEextension.ToLower().IndexOf("jpg") > 0 || FileEextension.ToLower().IndexOf("png") > 0 || FileEextension.ToLower().IndexOf("bmp") > 0
                            || FileEextension.ToLower().IndexOf("svg") > 0 || FileEextension.ToLower().IndexOf("gif") > 0 || FileEextension.ToLower().IndexOf("tga") > 0
                              ? "图片" : "其他";  // bmp，jpg，png，tif，gif
                        baseFile.Status = baseFile.Type == "图片" ?
                            "<a  style='display: inline;color:blue; ' href='/Utility/DownFile?fileName=" + fileproperty.Name + "&filePath=/Resource/Document/LJ/" + by_orderdetailprocess.OrderCode + "/" + by_orderdetailprocess.OrderDetailProcessId + "/" + fileproperty.Name + "' target='_self'><img style='width:30px;display: inline;' src='../../Resource/Document/LJ/" + by_orderdetailprocess.OrderCode + "/" + by_orderdetailprocess.OrderDetailProcessId + "/" + fileproperty.Name + "?" + new Random().Next() + "'  onMouseOver='toolTip(\"<img style=width:600px src=../../Resource/Document/LJ/" + by_orderdetailprocess.OrderCode + "/" + by_orderdetailprocess.OrderDetailProcessId + "/" + fileproperty.Name + "?" + new Random().Next() + ">\")' onMouseOut='toolTip()' /></a>"
                            : "<a  style='display: inline;color:blue; ' href='/Utility/DownFile?fileName=" + fileproperty.Name + "&filePath=/Resource/Document/LJ/" + by_orderdetailprocess.OrderCode + "/" + by_orderdetailprocess.OrderDetailProcessId + "/" + fileproperty.Name + "' target='_self'>" + fileproperty.Name + "</a>";
                        baseFile.AttachId = filenameid;
                        DataFactory.Database().Update(baseFile);
                    }
                    else
                    {
                        baseFile.Create();
                        baseFile.Name = fileproperty.Name;
                        baseFile.Eextension = fileproperty.Eextension;
                        baseFile.ModifyDate = DateTime.Now;
                        baseFile.Path = fileproperty.Path;
                        baseFile.Size = fileproperty.Size;
                        baseFile.Type = FileEextension.ToLower().IndexOf("jpg") > 0 || FileEextension.ToLower().IndexOf("png") > 0 || FileEextension.ToLower().IndexOf("bmp") > 0
                            || FileEextension.ToLower().IndexOf("svg") > 0 || FileEextension.ToLower().IndexOf("gif") > 0 || FileEextension.ToLower().IndexOf("tga") > 0
                              ? "图片" : "其他";  // bmp，jpg，png，tif，gif
                        baseFile.Status = baseFile.Type == "图片" ?
                            "<a style='display: inline;color:blue; '  href='/Utility/DownFile?fileName=" + fileproperty.Name + "&filePath=/Resource/Document/LJ/" + by_orderdetailprocess.OrderCode + "/" + by_orderdetailprocess.OrderDetailProcessId + "/" + fileproperty.Name + "' target='_self'><img style='width:30px;display: inline; ' src='../../Resource/Document/LJ/" + by_orderdetailprocess.OrderCode + "/" + by_orderdetailprocess.OrderDetailProcessId + "/" + fileproperty.Name + "?" + new Random().Next() + "'  onMouseOver='toolTip(\"<img style=width:600px src=../../Resource/Document/LJ/" + by_orderdetailprocess.OrderCode + "/" + by_orderdetailprocess.OrderDetailProcessId + "/" + fileproperty.Name + "?" + new Random().Next() + ">\")' onMouseOut='toolTip()' /></a>"
                            : "<a style='display: inline;color:blue; '  href='/Utility/DownFile?fileName=" + fileproperty.Name + "&filePath=/Resource/Document/LJ/" + by_orderdetailprocess.OrderCode + "/" + by_orderdetailprocess.OrderDetailProcessId + "/" + fileproperty.Name + "' target='_self'>" + fileproperty.Name + "</a>";
                        baseFile.AttachId = filenameid;
                        DataFactory.Database().Insert(baseFile);
                    }
                }
                BY_Quotation by_quotation = DataFactory.Database().FindEntityBySql<BY_Quotation>("select top 1 * from BY_Quotation where QuotationCode = '" + filenameid + "' ");
                if (by_quotation != null && by_quotation.QuotationId != null)
                {
                    base_file baseFile = DataFactory.Database().FindEntityBySql<base_file>("select top 1 * from base_file where name = '" + evalfilename + "' and AttachId = '" + filenameid + "'");
                    if (baseFile != null && baseFile.Id != null)
                    {
                        baseFile.Name = fileproperty.Name;
                        baseFile.Eextension = fileproperty.Eextension;
                        baseFile.CreateDate = fileproperty.CreateDate;
                        baseFile.Path = fileproperty.Path;
                        baseFile.Size = fileproperty.Size;
                        baseFile.Type = FileEextension.ToLower().IndexOf("jpg") > 0 || FileEextension.ToLower().IndexOf("png") > 0 || FileEextension.ToLower().IndexOf("bmp") > 0
                            || FileEextension.ToLower().IndexOf("svg") > 0 || FileEextension.ToLower().IndexOf("gif") > 0 || FileEextension.ToLower().IndexOf("tga") > 0
                              ? "图片" : "其他";  // bmp，jpg，png，tif，gif
                        baseFile.Status = baseFile.Type == "图片" ?
                            "<a  style='display: inline;color:blue; ' href='/Utility/DownFile?fileName=" + fileproperty.Name + "&filePath=/Resource/Document/LJ/" + by_quotation.QuotationCode.Substring(2, 4) + by_quotation.QuotationCode.Substring(8) + "/" + fileproperty.Name + "' target='_self'><img style='width:30px;display: inline;' src='../../Resource/Document/LJ/" + by_quotation.QuotationCode.Substring(2, 4) + by_quotation.QuotationCode.Substring(8) + "/" + fileproperty.Name + "?" + new Random().Next() + "'  onMouseOver='toolTip(\"<img style=width:600px src=../../Resource/Document/LJ/" + by_quotation.QuotationCode.Substring(2, 4) + by_quotation.QuotationCode.Substring(8) + "/" + fileproperty.Name + "?" + new Random().Next() + ">\")' onMouseOut='toolTip()' /></a>"
                            : "<a  style='display: inline;color:blue; ' href='/Utility/DownFile?fileName=" + fileproperty.Name + "&filePath=/Resource/Document/LJ/" + by_quotation.QuotationCode.Substring(2, 4) + by_quotation.QuotationCode.Substring(8) + "/" + fileproperty.Name + "' target='_self'>" + fileproperty.Name + "</a>";
                        baseFile.AttachId = filenameid;

                        DataFactory.Database().Update(baseFile);
                    }
                    else
                    {
                        baseFile.Create();
                        baseFile.Name = fileproperty.Name;
                        baseFile.Eextension = fileproperty.Eextension;
                        baseFile.ModifyDate = DateTime.Now;
                        baseFile.Path = fileproperty.Path;
                        baseFile.Size = fileproperty.Size;
                        baseFile.Type = FileEextension.ToLower().IndexOf("jpg") > 0 || FileEextension.ToLower().IndexOf("png") > 0 || FileEextension.ToLower().IndexOf("bmp") > 0
                            || FileEextension.ToLower().IndexOf("svg") > 0 || FileEextension.ToLower().IndexOf("gif") > 0 || FileEextension.ToLower().IndexOf("tga") > 0
                              ? "图片" : "其他";  // bmp，jpg，png，tif，gif
                        baseFile.Status = baseFile.Type == "图片" ?
                            "<a style='display: inline;color:blue; '  href='/Utility/DownFile?fileName=" + fileproperty.Name + "&filePath=/Resource/Document/LJ/" + by_quotation.QuotationCode.Substring(2, 4) + by_quotation.QuotationCode.Substring(8) + "/" + fileproperty.Name + "' target='_self'><img style='width:30px;display: inline; ' src='../../Resource/Document/LJ/" + by_quotation.QuotationCode.Substring(2, 4) + by_quotation.QuotationCode.Substring(8) + "/" + fileproperty.Name + "?" + new Random().Next() + "'  onMouseOver='toolTip(\"<img style=width:600px src=../../Resource/Document/LJ/" + by_quotation.QuotationCode.Substring(2, 4) + by_quotation.QuotationCode.Substring(8) + "/" + fileproperty.Name + "?" + new Random().Next() + ">\")' onMouseOut='toolTip()' /></a>"
                            : "<a style='display: inline;color:blue; '  href='/Utility/DownFile?fileName=" + fileproperty.Name + "&filePath=/Resource/Document/LJ/" + by_quotation.QuotationCode.Substring(2, 4) + by_quotation.QuotationCode.Substring(8) + "/" + fileproperty.Name + "' target='_self'>" + fileproperty.Name + "</a>";
                        baseFile.AttachId = filenameid;
                        DataFactory.Database().Insert(baseFile);
                    }
                }

                BY_QuotationDetail by_quotationdetail = DataFactory.Database().FindEntity<BY_QuotationDetail>(filenameid);
                if (by_quotationdetail != null && by_quotationdetail.QuotationDetailId != null)
                {
                    base_file baseFile = DataFactory.Database().FindEntityBySql<base_file>("select top 1 * from base_file where name = '" + evalfilename + "' and AttachId = '" + filenameid + "'");
                    if (baseFile != null && baseFile.Id != null)
                    {
                        baseFile.Name = fileproperty.Name;
                        baseFile.Eextension = fileproperty.Eextension;
                        baseFile.CreateDate = fileproperty.CreateDate;
                        baseFile.Path = fileproperty.Path;
                        baseFile.Size = fileproperty.Size;
                        baseFile.Type = FileEextension.ToLower().IndexOf("jpg") > 0 || FileEextension.ToLower().IndexOf("png") > 0 || FileEextension.ToLower().IndexOf("bmp") > 0
                            || FileEextension.ToLower().IndexOf("svg") > 0 || FileEextension.ToLower().IndexOf("gif") > 0 || FileEextension.ToLower().IndexOf("tga") > 0
                              ? "图片" : "其他";  // bmp，jpg，png，tif，gif
                        baseFile.Status = baseFile.Type == "图片" ?
                            "<a  style='display: inline;color:blue; ' href='/Utility/DownFile?fileName=" + fileproperty.Name + "&filePath=/Resource/Document/LJ/" + by_quotationdetail.QuotationCode.Substring(2, 4) + by_quotationdetail.QuotationCode.Substring(8) + "/" + fileproperty.Name + "' target='_self'><img style='width:30px;display: inline;' src='../../Resource/Document/LJ/" + by_quotationdetail.QuotationCode.Substring(2, 4) + by_quotationdetail.QuotationCode.Substring(8) + "/" + fileproperty.Name + "?" + new Random().Next() + "'  onMouseOver='toolTip(\"<img style=width:600px src=../../Resource/Document/LJ/" + by_quotationdetail.QuotationCode.Substring(2, 4) + by_quotationdetail.QuotationCode.Substring(8) + "/" + fileproperty.Name + "?" + new Random().Next() + ">\")' onMouseOut='toolTip()' /></a>"
                            : "<a  style='display: inline;color:blue; ' href='/Utility/DownFile?fileName=" + fileproperty.Name + "&filePath=/Resource/Document/LJ/" + by_quotationdetail.QuotationCode.Substring(2, 4) + by_quotationdetail.QuotationCode.Substring(8) + "/" + fileproperty.Name + "' target='_self'>" + fileproperty.Name + "</a>";
                        baseFile.AttachId = filenameid;

                        DataFactory.Database().Update(baseFile);
                    }
                    else
                    {
                        baseFile.Create();
                        baseFile.Name = fileproperty.Name;
                        baseFile.Eextension = fileproperty.Eextension;
                        baseFile.ModifyDate = DateTime.Now;
                        baseFile.Path = fileproperty.Path;
                        baseFile.Size = fileproperty.Size;
                        baseFile.Type = FileEextension.ToLower().IndexOf("jpg") > 0 || FileEextension.ToLower().IndexOf("png") > 0 || FileEextension.ToLower().IndexOf("bmp") > 0
                            || FileEextension.ToLower().IndexOf("svg") > 0 || FileEextension.ToLower().IndexOf("gif") > 0 || FileEextension.ToLower().IndexOf("tga") > 0
                              ? "图片" : "其他";  // bmp，jpg，png，tif，gif
                        baseFile.Status = baseFile.Type == "图片" ?
                            "<a style='display: inline;color:blue; '  href='/Utility/DownFile?fileName=" + fileproperty.Name + "&filePath=/Resource/Document/LJ/" + by_quotationdetail.QuotationCode.Substring(2, 4) + by_quotationdetail.QuotationCode.Substring(8) + "/" + fileproperty.Name + "' target='_self'><img style='width:30px;display: inline; ' src='../../Resource/Document/LJ/" + by_quotationdetail.QuotationCode.Substring(2, 4) + by_quotationdetail.QuotationCode.Substring(8) + "/" + fileproperty.Name + "?" + new Random().Next() + "'  onMouseOver='toolTip(\"<img style=width:600px src=../../Resource/Document/LJ/" + by_quotationdetail.QuotationCode.Substring(2, 4) + by_quotationdetail.QuotationCode.Substring(8) + "/" + fileproperty.Name + "?" + new Random().Next() + ">\")' onMouseOut='toolTip()' /></a>"
                            : "<a style='display: inline;color:blue; '  href='/Utility/DownFile?fileName=" + fileproperty.Name + "&filePath=/Resource/Document/LJ/" + by_quotationdetail.QuotationCode.Substring(2, 4) + by_quotationdetail.QuotationCode.Substring(8) + "/" + fileproperty.Name + "' target='_self'>" + fileproperty.Name + "</a>";
                        baseFile.AttachId = filenameid;
                        DataFactory.Database().Insert(baseFile);
                    }
                }

                return Content(fileproperty.ToJson());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message + "\r\n条件：");
                return Content(ex.Message);
            }
        }

        #region base64转图片
        /// <summary>
        /// 图片上传 Base64解码
        /// </summary>
        /// <param name="dataURL">Base64数据</param>
        /// <param name="imgName">图片名字</param>
        /// <returns>返回一个相对路径</returns>
        public ActionResult decodeBase64ToImage(string dataURL, string saveDocument, string filenameid = "")
        {
            if (string.IsNullOrWhiteSpace(filenameid))
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：请慢一点" }.ToString());

            }
            string massage = "ok";

            //string filename = filenameid;//声明一个string类型的相对路径
            string filename = @DNS.Utilities.ManageProvider.Provider.Current().Account + DateTime.Now.ToString().Replace(" ", "(").Replace(":", "-") + ")";//声明一个string类型的相对路径

            String base64 = dataURL.Substring(dataURL.IndexOf(",") + 1);      //将‘，’以前的多余字符串删除
            System.Drawing.Bitmap bitmap = null;//定义一个Bitmap对象，接收转换完成的图片

            //BY_QuotationHeat by_quotationheat = DataFactory.Database().FindEntity<BY_QuotationHeat>("QuotationHeatCode", filenameid);
            //by_quotationheat.imgbase64 = dataURL;
            //DataFactory.Database().Update(by_quotationheat);
            try//会有异常抛出，try，catch一下
            {

                byte[] arr = Convert.FromBase64String(base64);//将纯净资源Base64转换成等效的8位无符号整形数组

                System.IO.MemoryStream ms = new System.IO.MemoryStream(arr);//转换成无法调整大小的MemoryStream对象
                bitmap = new System.Drawing.Bitmap(ms);//将MemoryStream对象转换成Bitmap对象

                //
                string FileEextension = ".jpg";
                string uploadDate = DateTime.Now.ToString("yyyyMMdd");
                string virtualPath = string.Format("~/Resource/Document/{0}/{1}{2}", saveDocument, filenameid, FileEextension);
                string fullFileName = this.Server.MapPath(virtualPath);

                //创建文件夹
                string path = Path.GetDirectoryName(fullFileName);
                Directory.CreateDirectory(path);

                string FilePath = this.Server.MapPath(virtualPath);
                if (System.IO.File.Exists(FilePath))
                    System.IO.File.Delete(FilePath);



                bitmap.Save(fullFileName, System.Drawing.Imaging.ImageFormat.Jpeg);//保存到服务器路径
                //bitmap.Save(filePath + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                //bitmap.Save(filePath + ".gif", System.Drawing.Imaging.ImageFormat.Gif);
                //bitmap.Save(filePath + ".png", System.Drawing.Imaging.ImageFormat.Png);
                ms.Close();//关闭当前流，并释放所有与之关联的资源
                bitmap.Dispose();

                base_file baseFile = new base_file();
                baseFile.Create();
                baseFile.Name = filename + FileEextension;
                baseFile.Eextension = FileEextension;
                baseFile.ModifyDate = DateTime.Now;
                baseFile.Path = virtualPath;
                baseFile.Size = (base64.Length / 1000).ToString() + "KB";
                baseFile.Type = FileEextension.ToLower().IndexOf("jpg") > 0 || FileEextension.ToLower().IndexOf("png") > 0 || FileEextension.ToLower().IndexOf("bmp") > 0
                    || FileEextension.ToLower().IndexOf("svg") > 0 || FileEextension.ToLower().IndexOf("gif") > 0 || FileEextension.ToLower().IndexOf("tga") > 0
                      ? "图片" : "其他";  // bmp，jpg，png，tif，gif
                baseFile.Status = baseFile.Type == "图片" ?
                    "<a style='display: inline;color:blue; '  href='/Utility/DownFile?fileName=" + baseFile.Name + "&filePath=/Resource/Document/" + saveDocument + "/" + baseFile.Name + "' target='_self'><img style='width:30px;display: inline; ' src='../../Resource/Document/" + saveDocument + "/" + baseFile.Name + "?" + new Random().Next() + "'  onMouseOver='toolTip(\"<img style=width:600px src=../../Resource/Document/" + saveDocument + "/" + baseFile.Name + "?" + new Random().Next() + ">\")' onMouseOut='toolTip()' /></a>"
                    : "<a style='display: inline;color:blue; '  href='/Utility/DownFile?fileName=" + baseFile.Name + "&filePath=/Resource/Document/" + saveDocument + "/" + baseFile.Name + "' target='_self'>" + baseFile.Name + "</a>";
                baseFile.AttachId = filenameid;
                DataFactory.Database().Insert(baseFile);
            }
            catch (Exception e)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + e.Message }.ToString());
            }
            return Content(new JsonMessage { Success = true, Code = "1", Message = "保存图片成功" }.ToString());
        }

        public ActionResult UploadForUniapp(BY_OrderDetailProcessDetail entity)
        {
            try
            {
                string[] strtemp = entity.OrderCode.Split('-');
                if (strtemp.Length < 3)
                {
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：订单号+行号！" }.ToString());
                }
                List<DbParameter> parameter = new List<DbParameter>();

                parameter.Add(DbFactory.CreateDbParameter("@OrderCodeLine", strtemp[0] + "-" + strtemp[1] + "-" + strtemp[2]));
                parameter.Add(DbFactory.CreateDbParameter("@Qty", entity.Qty));
                parameter.Add(DbFactory.CreateDbParameter("@Weight", entity.Weight));
                parameter.Add(DbFactory.CreateDbParameter("@ProcessCode", entity.TechnologyId));
                parameter.Add(DbFactory.CreateDbParameter("@MachineCode", entity.MachineId));
                parameter.Add(DbFactory.CreateDbParameter("@UserId", entity.OperatorId));
                DbParameter ReturnMessage = DbFactory.CreateDbParameter();
                ReturnMessage.Direction = ParameterDirection.Output;
                ReturnMessage.ParameterName = "@ReturnMessage";
                ReturnMessage.Size = 50;
                ReturnMessage.DbType = DbType.String;
                parameter.Add(ReturnMessage);

                DbParameter ReturnID = DbFactory.CreateDbParameter();
                ReturnID.Direction = ParameterDirection.Output;
                ReturnID.ParameterName = "@processdetailid";
                ReturnID.Size = 50;
                ReturnID.DbType = DbType.String;
                parameter.Add(ReturnID);

                DataFactory.Database().ExecuteByProc("StoredPBYERP", parameter.ToArray());
                if (ReturnMessage.Value.ToString() == "1")
                {


                    System.Web.HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                    for (int i = 0; i < files.AllKeys.Count(); i++)
                    {

                        if (files[i].FileName.Length > 0)
                        {
                            System.Web.HttpPostedFile postedfile = files[i];
                            string virtualPath = "";
                            var ext = Path.GetExtension(postedfile.FileName);
                            var fileName = DateTime.Now.Ticks.ToString() + ext;
                            virtualPath = "/Resource/Document/ProcessDetail/" + entity.OrderCode + "/" + fileName;
                            // 将相对路径转换成物理路径
                            var path = Server.MapPath(virtualPath);

                            if (System.IO.File.Exists(path))
                                System.IO.File.Delete(path);
                            //创建文件夹，保存文件
                            string path1 = Path.GetDirectoryName(path);
                            Directory.CreateDirectory(path1);

                            postedfile.SaveAs(path);

                            base_file baseFile = new base_file();
                            baseFile.Create();
                            baseFile.Name = fileName;
                            baseFile.Eextension = ext;
                            baseFile.Path = virtualPath;
                            baseFile.Size = SizeHelper.CountSize(postedfile.ContentLength);
                            baseFile.Type = ext.ToLower().IndexOf("jpg") > 0 || ext.ToLower().IndexOf("png") > 0 || ext.ToLower().IndexOf("bmp") > 0
                                || ext.ToLower().IndexOf("svg") > 0 || ext.ToLower().IndexOf("gif") > 0 || ext.ToLower().IndexOf("tga") > 0
                                  ? "图片" : "其他";  // bmp，jpg，png，tif，gif
                            baseFile.Status = baseFile.Type == "图片" ?
                                "<a style='display: inline;color:blue; '  href='/Utility/DownFile?fileName=" + baseFile.Name + "&filePath=/Resource/Document/ProcessDetail/" + entity.OrderCode + "/" + fileName + "' target='_self'><img style='width:30px;display: inline; ' src='../../Resource/Document/ProcessDetail/" + entity.OrderCode + "/" + fileName + "?" + new Random().Next() + "'  onMouseOver='toolTip(\"<img style=width:600px src=../../Resource/Document/ProcessDetail/" + entity.OrderCode + "/" + fileName + "?" + new Random().Next() + ">\")' onMouseOut='toolTip()' /></a>"
                                : "<a style='display: inline;color:blue; '  href='/Utility/DownFile?fileName=" + baseFile.Name + "&filePath=/Resource/Document/ProcessDetail/" + entity.OrderCode + "/" + fileName + "' target='_self'>" + baseFile.Name + "</a>";
                            baseFile.AttachId = ReturnID.Value.ToString();
                            DataFactory.Database().Insert(baseFile);
                        }
                    }

                    return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功！" }.ToString());
                }
                else
                {
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ReturnMessage.Value }.ToString());
                }
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        public ActionResult UploadForUniappHeat(string QuotationHeatList)
        {
            try
            {
                IDatabase database = DataFactory.Database();
                DbTransaction isOpenTrans = database.BeginTrans();
                try
                {
                    string Message = "新增成功。";
                    string toto = DateTime.Now.ToString("yyMM");
                    string seriousCode = "00001";
                    int QuotationHeatCodemax = CommonHelper.GetInt(DataFactory.Database().FindMax<BY_QuotationHeat>("QuotationHeatCode"));
                    if (QuotationHeatCodemax < 100000000)
                    {
                        seriousCode = "0";
                    }
                    else
                    {
                        seriousCode = DataFactory.Database().FindMax<BY_QuotationHeat>("QuotationHeatCode").ToString().Substring(4, 5);
                        //流水号+1
                    }
                    int seritemp = int.Parse(seriousCode);
                    List<BY_QuotationHeat> by_quotationheats = (QuotationHeatList.JonsToList<BY_QuotationHeat>()).OrderBy(P => P.SortCode).ToList<BY_QuotationHeat>();
                    string cates = "";
                    foreach (BY_QuotationHeat item in by_quotationheats)
                    {
                        Base_Customer custor = DataFactory.Database().FindEntity<Base_Customer>("FullName", item.CustomerName);
                        if (custor.Category == "冻结")
                        {
                            cates = cates + custor.FullName + ",";
                            continue;
                        }
                        seriousCode = (seritemp + 1).ToString().PadLeft(5, '0').Substring((seritemp + 1).ToString().PadLeft(5, '0').Length - 5, 5);
                        seritemp = seritemp + 1;
                        item.Create();
                        item.CustomerId = custor.CustomerId;
                        item.CustomerCode = custor.Code;
                        item.CustomerName = custor.ShortName;
                        item.QuotationHeatCode = Convert.ToInt32(toto + seriousCode);
                        if (item.SteelCode == "FY")
                        {
                            item.BillStatus = "检验通过";
                        }
                        else
                        {
                            item.BillStatus = "正常生产";

                        }
                        database.Insert(item, isOpenTrans);
                    }
                    database.Commit();
                    if (cates != "")
                    {
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + cates + "被冻结无法录入！" }.ToString());

                    }
                    else
                    {
                        System.Web.HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                        for (int i = 0; i < files.AllKeys.Count(); i++)
                        {
                            if (files[i].FileName.Length > 0)
                            {
                                System.Web.HttpPostedFile postedfile = files[i];
                                string virtualPath = "";
                                //var ext = Path.GetExtension(postedfile.FileName);
                                //var fileName = DateTime.Now.Ticks.ToString() + ext;
                                var ext = ".jpg";
                                var fileName = by_quotationheats[0].QuotationHeatCode + ext;

                                virtualPath = "/Resource/Document/HeatReport/" + fileName;
                                // 将相对路径转换成物理路径
                                var path = Server.MapPath(virtualPath);

                                if (System.IO.File.Exists(path))
                                    System.IO.File.Delete(path);
                                //创建文件夹，保存文件
                                string path1 = Path.GetDirectoryName(path);
                                Directory.CreateDirectory(path1);

                                postedfile.SaveAs(path);

                            }
                        }

                        return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功！" }.ToString());
                    }
                }
                catch (Exception ex)
                {
                    database.Rollback();
                    return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：录入的数据有错误，请检查" + ex.Message + ex.StackTrace }.ToString());
                }






            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        public ActionResult UploadForUniappStove(string StoveId)
        {
            try
            {
                System.Web.HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                for (int i = 0; i < files.AllKeys.Count(); i++)
                {
                    if (files[i].FileName.Length > 0)
                    {
                        System.Web.HttpPostedFile postedfile = files[i];
                        string virtualPath = "";
                        //var ext = Path.GetExtension(postedfile.FileName);
                        //var fileName = DateTime.Now.Ticks.ToString() + ext;
                        var ext = ".jpg";
                        var fileName = StoveId + ext;

                        virtualPath = "/Resource/Document/Stove/" + fileName;
                        // 将相对路径转换成物理路径
                        var path = Server.MapPath(virtualPath);

                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                        //创建文件夹，保存文件
                        string path1 = Path.GetDirectoryName(path);
                        Directory.CreateDirectory(path1);

                        postedfile.SaveAs(path);

                    }
                }

                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功！" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：请检查" + ex.Message + ex.StackTrace }.ToString());
            }

        }
        #endregion

        #region Image To base64
        public Image UrlToImage(string url)
        {
            url = "http://" + HttpContext.Request.Url.Host + "/" + url;
            WebClient mywebclient = new WebClient();
            try
            {
                byte[] Bytes = mywebclient.DownloadData(url);
                using (MemoryStream ms = new MemoryStream(Bytes))
                {
                    Image outputImg = Image.FromStream(ms);
                    return outputImg;
                }
            }
            catch (Exception)
            {
                return null;
            }

        }
        /// <summary>
        /// Image 转成 base64
        /// </summary>
        /// <param name="fileFullName"></param>
        public string CImageToBase64(Image img)
        {
            if (img == null)
            {
                return null;
            }
            try
            {
                Bitmap bmp = new Bitmap(img);
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string ImageToBase64(string url)
        {
            return "data:image/png;base64," + CImageToBase64(UrlToImage(url));
        }

        #endregion

        #region 图片转base64
        /// <summary>
        /// 图片转base64
        /// </summary>
        /// <param name="path">图片路径</param><br>        
        /// <returns>返回一个base64字符串</returns>
        public string decodeImageToBase64(string path)
        {
            path = "E:/vs 2015/newaqtcprj/WEB/UpFile/2018/12/20181229174708_7471722c425a2ec08fa513ddf4f8c76306d55fbb0fbd9d8.jpg";
            string base64str = "";

            //站点文件目录
            string fileDir = HttpContext.Server.MapPath("/");
            string[] arrfileDir = fileDir.Split('\\');
            fileDir = arrfileDir[0] + "\\" + arrfileDir[1] + "\\" + arrfileDir[2];
            try
            {
                //读图片转为Base64String
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(Path.Combine(fileDir, "WEB\\UpFile\\2018\\12\\20181229174708_7471722c425a2ec08fa513ddf4f8c76306d55fbb0fbd9d8.jpg"));
                //System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(path);
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                bmp.Dispose();
                base64str = Convert.ToBase64String(arr);
            }
            catch (Exception e)
            {
                string mss = e.Message;
            }
            return "data:image/jpg;base64," + base64str;
        }
        #endregion

        #region 工具栏/右击栏 按钮
        /// <summary>
        /// 加载工具栏/右击栏 按钮
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadButton()
        {
            Base_ButtonPermissionBll base_buttonpermissionbll = new Base_ButtonPermissionBll();
            string ObjectId = ManageProvider.Provider.Current().ObjectId;
            string ModuleId = DESEncrypt.Decrypt(CookieHelper.GetCookie("ModuleId"));
            List<Base_Button> list = base_buttonpermissionbll.GetButtonList(ObjectId, ModuleId).FindAll(t => t.Enabled == 1);
            return Content(list.ToJson().Replace("&nbsp;", ""));
        }
        #endregion

        #region 表格视图列名
        /// <summary>
        /// 表格视图列名
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadViewColumn()
        {
            Base_ViewPermissionBll base_viewpermissionbll = new Base_ViewPermissionBll();
            string ObjectId = ManageProvider.Provider.Current().ObjectId;
            string ModuleId = DESEncrypt.Decrypt(CookieHelper.GetCookie("ModuleId"));
            List<Base_View> list = base_viewpermissionbll.GetViewList(ObjectId, ModuleId).FindAll(t => t.Enabled == 1);
            return Content(list.ToJson());
        }
        /// <summary>
        /// 自定义格式化
        /// </summary>
        /// <param name="CustomSwitch"></param>
        /// <returns></returns>
        public string ToFormatter(string CustomSwitch)
        {
            StringBuilder str = new StringBuilder();
            str.Append("function (cellvalue, options, rowObject) {");
            Hashtable ht = HashtableHelper.JsonToHashtable(CustomSwitch);
            if (ht != null && ht.Count > 0)
            {
                foreach (string key in ht.Keys)
                {
                    str.Append("if (cellvalue == '" + key + "')");
                    str.Append("    return \"" + ht[key] + "\";");
                }
            }
            else
            {
                str.Append("return \"<img src='../../Content/Images/\" + cellvalue + \"'/>\";");
            }
            str.Append("}");
            return str.ToString();
        }
        #endregion

        #region 公共查询
        public Base_QueryRecordBll base_queryrecordbll = new Base_QueryRecordBll();
        /// <summary>
        /// 【查询页面】
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult QueryPage()
        {
            return View();
        }
        /// <summary>
        /// 【查询页面】方案列表
        /// </summary>
        /// <param name="ModuleId">模块ID</param>
        /// <returns></returns>
        public ActionResult QueryListProject(string ModuleId)
        {
            string CreateUserId = ManageProvider.Provider.Current().UserId;
            List<Base_QueryRecord> list = base_queryrecordbll.GetList(ModuleId, CreateUserId);
            string JsonData = list.ToJson().Replace("ParamName", "FieldName").Replace("Operation", "CompareValue").Replace("ParamValue", "FilterValue");
            return Content(JsonData);
        }
        /// <summary>
        /// 【查询页面】删除方案
        /// </summary>
        /// <param name="QueryRecordId"></param>
        /// <returns></returns>
        public ActionResult QueryDeleteProject(string QueryRecordId)
        {
            try
            {
                var Message = "删除失败。";
                int IsOk = DataFactory.Database().Delete<Base_QueryRecord>(QueryRecordId);
                if (IsOk > 0)
                {
                    Message = "删除成功。";
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败，错误：" + ex.Message }.ToString());
            }
        }
        /// <summary>
        /// 【查询页面】保存方案
        /// </summary>
        /// <param name="QueryRecordId">主键</param>
        /// <param name="entity">对象实体</param>
        /// <returns></returns>
        public ActionResult QuerySaveProject(string QueryRecordId, Base_QueryRecord entity)
        {
            try
            {
                int IsOk = 0;
                string Message = QueryRecordId == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(QueryRecordId))
                {
                    entity.Modify(QueryRecordId);
                    IsOk = DataFactory.Database().Update(entity);
                }
                else
                {
                    entity.Create();
                    IsOk = DataFactory.Database().Insert(entity);
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败，错误：" + ex.Message }.ToString());
            }
        }
        /// <summary>
        /// 【查询页面】设置初始化默认方案
        /// </summary>
        /// <param name="ModuleId">模块ID</param>
        /// <param name="QueryRecordId">主键</param>
        /// <returns></returns>
        public ActionResult QueryDefaultProject(string ModuleId, string QueryRecordId)
        {
            try
            {
                int IsOk = base_queryrecordbll.DefaultProject(ModuleId, QueryRecordId);
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "操作成功。" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败，错误：" + ex.Message }.ToString());
            }
        }
        #endregion

        #region 省、市、区 列表
        Base_ProvinceCityBll base_provincecitybll = new Base_ProvinceCityBll();
        /// <summary>
        /// 获取省、市、区 列表
        /// </summary>
        /// <param name="ParentId"></param>
        /// <returns></returns>
        public ActionResult GetProvinceCityListJson(string ParentId)
        {
            List<Base_ProvinceCity> ListData = base_provincecitybll.GetList(ParentId);
            return Content(ListData.ToJson());
        }
        #endregion

        #region 验证对象值不能重复
        /// <summary>
        /// 验证对象值不能重复
        /// </summary>
        /// <param name="tablename">实体类</param>
        /// <param name="fieldname">属性字段</param>
        /// <param name="fieldvalue">属性字段值</param>
        /// <param name="keyfield">主键字段</param>
        /// <param name="keyvalue">主键字段值</param>
        /// <returns></returns>
        public ActionResult FieldExist(string tablename, string fieldname, string fieldvalue, string keyfield, string keyvalue)
        {
            bool IsOk = BaseFactory.BaseHelper().FieldExist(tablename, fieldname, fieldvalue, keyfield, keyvalue);
            return Content(IsOk.ToString().ToLower());
        }
        #endregion

        #region JqGrid导出Excel
        /// <summary>
        /// 获取要导出表头字段
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDeriveExcelColumn()
        {

            ExportExcels exportexcels = exportexcelsbll.Repository().FindEntity("ExportExcelsTemp");
            string JsonColumn = GZipHelper.Uncompress(exportexcels.JsonColumn);
            return Content(JsonColumn);
        }


        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="ExportField">要导出字段</param>
        public void GetDeriveExcel()
        {
            //string JsonColumn = GZipHelper.Uncompress(CookieHelper.GetCookie("JsonColumn_DeriveExcel"));
            //string JsonData = GZipHelper.Uncompress(CookieHelper.GetCookie("JsonData_DeriveExcel"));
            //string JsonFooter = GZipHelper.Uncompress(CookieHelper.GetCookie("JsonFooter_DeriveExcel"));
            //string fileName = GZipHelper.Uncompress(CookieHelper.GetCookie("FileName_DeriveExcel"));
            ExportExcels exportexcels = exportexcelsbll.Repository().FindEntity("ExportExcelsTemp");
            string JsonColumn = GZipHelper.Uncompress(exportexcels.JsonColumn);
            string JsonData = GZipHelper.Uncompress(exportexcels.JsonData);
            string fileName = GZipHelper.Uncompress(exportexcels.fileName);
            string ExportField = string.Empty;
            if (!string.IsNullOrEmpty(exportexcels.ExportField))
            {
                ExportField = GZipHelper.Uncompress(exportexcels.ExportField);
            }
            else
            {
                ExportField = JsonColumn;
            }

            DeriveExcel.JqGridToExcel(JsonColumn, JsonData, ExportField, fileName);

            //Session["JsonColumn_DeriveExcel"] = null;
            //Session["JsonData_DeriveExcel"] = null;
            //Session["FileName_DeriveExcel"] = null;
            //CookieHelper.DelCookie("JsonColumn_DeriveExcel");
            //CookieHelper.DelCookie("JsonData_DeriveExcel");
            //CookieHelper.DelCookie("JsonFooter_DeriveExcel");
            //CookieHelper.DelCookie("FileName_DeriveExcel");
        }


        //public FileResult GetDeriveExcel()
        //{

        //    ExportExcels exportexcels = exportexcelsbll.Repository().FindEntity("ExportExcelsTemp");
        //    string JsonColumn = GZipHelper.Uncompress(exportexcels.JsonColumn);
        //    string JsonData = GZipHelper.Uncompress(exportexcels.JsonData);
        //    string fileName = GZipHelper.Uncompress(exportexcels.fileName);
        //    string ExportField = string.Empty;
        //    List<JqGridColumn> ListColumn = JsonConvert.DeserializeObject<List<JqGridColumn>>(JsonColumn);
        //    if (!string.IsNullOrEmpty(exportexcels.ExportField))
        //    {
        //        ExportField = GZipHelper.Uncompress(exportexcels.ExportField);
        //    }
        //    else
        //    {
        //        ExportField = JsonColumn;
        //    }

        //    StringBuilder sbHtml = new StringBuilder();
        //    sbHtml.AppendLine("<table cellspacing=\"0\" cellpadding=\"5\" rules=\"all\" border=\"1\">");
        //    写出列名
        //    sbHtml.AppendLine("<tr style=\"background-color: #FFE88C;font-weight: bold; white-space: nowrap;\">");
        //    string[] FieldInfo = ExportField.Split(',');
        //    foreach (string item in FieldInfo)
        //    {
        //        IList list = ListColumn.FindAll(t => t.name == item);
        //        foreach (JqGridColumn jqgridcolumn in list)
        //        {
        //            if (jqgridcolumn.hidden.ToLower() == "false" && jqgridcolumn.label != null)
        //            {
        //                sbHtml.AppendLine("<td style=\"width:" + jqgridcolumn.width + "px;text-align:" + jqgridcolumn.align + "\">" + jqgridcolumn.label + "</td>");
        //            }
        //        }
        //    }
        //    sbHtml.AppendLine("</tr>");
        //    写数据
        //    DataTable dt = JsonData.JsonToDataTable();
        //    if (dt != null)
        //    {
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            sbHtml.Append("<tr>");
        //            foreach (string item in FieldInfo)
        //            {
        //                IList list = ListColumn.FindAll(t => t.name == item);
        //                foreach (JqGridColumn jqgridcolumn in list)
        //                {
        //                    if (jqgridcolumn.hidden.ToLower() == "false" && jqgridcolumn.label != null)
        //                    {
        //                        object text = row[jqgridcolumn.name];
        //                        sbHtml.Append("<td style=\"width:" + jqgridcolumn.width + "px;text-align:" + jqgridcolumn.align + "\">").Append(text).Append("</td>");
        //                    }
        //                }
        //            }
        //            sbHtml.AppendLine("</tr>");
        //        }
        //    }
        //    sbHtml.AppendLine("</table>");
        //    byte[] fileContents = Encoding.Default.GetBytes(sbHtml.ToString());
        //    return File(fileContents, "application/ms-excel", HttpUtility.UrlEncode("下载.xls", System.Text.Encoding.UTF8));
        //}


        /// <summary>
        /// 写入数据到Cookie
        /// </summary>
        /// <param name="JsonColumn">表头</param>
        /// <param name="JsonData">数据</param>
        /// <param name="JsonFooter">底部合计</param>
        ExportExcelsBll exportexcelsbll = new ExportExcelsBll();
        [ValidateInput(false)]
        public void SetDeriveExcel(string JsonColumn, string JsonData, string JsonFooter, string FileName, string SelectedField)
        {
            //CookieHelper.WriteCookie("JsonColumn_DeriveExcel", GZipHelper.Compress(JsonColumn));
            //CookieHelper.WriteCookie("JsonData_DeriveExcel", GZipHelper.Compress(JsonData));
            //CookieHelper.WriteCookie("JsonFooter_DeriveExcel", GZipHelper.Compress(JsonFooter));
            //CookieHelper.WriteCookie("FileName_DeriveExcel", GZipHelper.Compress(FileName));
            ExportExcels exportexcels = exportexcelsbll.Repository().FindEntity("ExportExcelsTemp");
            if (!string.IsNullOrEmpty(JsonColumn))
            {
                exportexcels.JsonColumn = GZipHelper.Compress(JsonColumn);
            }
            if (!string.IsNullOrEmpty(JsonData))
            {
                exportexcels.JsonData = GZipHelper.Compress(JsonData);
            }
            if (!string.IsNullOrEmpty(FileName))
            {
                exportexcels.fileName = GZipHelper.Compress(FileName);
            }
            if (!string.IsNullOrEmpty(SelectedField))
            {
                exportexcels.ExportField = GZipHelper.Compress(SelectedField);
            }
            exportexcelsbll.Repository().Update(exportexcels);
        }


        #endregion

        #region 选择用户
        Base_UserBll base_userbll = new Base_UserBll();
        /// <summary>
        /// 选择用户
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult OptionUser()
        {
            return View();
        }
        /// <summary>
        ///用户列表JSON
        /// </summary>
        /// <param name="keyword">模糊查询</param>
        /// <returns></returns>
        public ActionResult OptionUserJson(string keyword)
        {
            DataTable ListData = base_userbll.OptionUserList(keyword);
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            if (!DataHelper.IsExistRows(ListData))
            {
                foreach (DataRow item in ListData.Rows)
                {
                    sb.Append("{");
                    sb.Append("\"id\":\"" + item["userid"] + "\",");
                    sb.Append("\"text\":\"" + item["realname"] + "（" + item["account"] + "）\",");
                    sb.Append("\"account\":\"" + item["account"] + "\",");
                    sb.Append("\"code\":\"" + item["code"] + "\",");
                    sb.Append("\"realname\":\"" + item["realname"] + "\",");
                    string Genderimg = "user_female.png";
                    if (item["Gender"].ToString() == "男")
                    {
                        Genderimg = "user_green.png";
                    }
                    sb.Append("\"img\":\"/Content/Images/Icon16/" + Genderimg + "\",");
                    sb.Append("\"isexpand\":true,");
                    sb.Append("\"hasChildren\":false");
                    sb.Append("},");
                }
                sb = sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]");
            return Content(sb.ToString());
        }
        #endregion

        #region 生成打印
        /// <summary>
        /// 打印当前页
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "Cache1Hour")]
        public ActionResult PrintPage()
        {
            return View();
        }
        #endregion

        #region 下载
        //方式一：
        //<a href="/Document/DownFile?filePath=@item.Value&fileName=@item.Key">下载</a>  
        public FileStreamResult DownFile(string filePath, string fileName)
        {
            string absoluFilePath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentPath"] + filePath);
            return File(new FileStream(absoluFilePath, FileMode.Open), "application/octet-stream", Server.UrlEncode(fileName));
        }


        static string RemoveChars(string fileName)
        {
            // 正则表达式模式匹配不可作为文件名的特殊符号
            // 使用正则表达式替换特殊符号为空字符串
            string cleanedFileName = fileName.Replace("\\", "").Replace("~", "").Replace("!", "").Replace("@", "").Replace("#", "").Replace("$", "").Replace("%", "").Replace("^", "").Replace("&", "")
                .Replace("*", "").Replace("?", "").Replace(":", "").Replace(";", "").Replace("/", "").Replace("'", "").Replace("\"", "").Replace("{", "").Replace("}", "").Replace("|", "").Replace(" ", "").Replace("　", "")
                .Replace("、", "").Replace("+", "").Replace("=", "");
            return cleanedFileName;
        }


        //方式二：
        //<a href="/Document/DownFile?filePath=@item.Value&fileName=@item.Key">下载</a>  
        //public ActionResult DownFile(string filePath, string fileName)
        //{
        //    filePath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["AttachmentPath"] + filePath);
        //    FileStream fs = new FileStream(filePath, FileMode.Open);
        //    byte[] bytes = new byte[(int)fs.Length];
        //    fs.Read(bytes, 0, bytes.Length);
        //    fs.Close();
        //    Response.Charset = "UTF-8";
        //    Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
        //    Response.ContentType = "application/octet-stream";

        //    Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(fileName));
        //    Response.BinaryWrite(bytes);
        //    Response.Flush();
        //    Response.End();
        //    return new EmptyResult();

        //}

        #endregion
    }
}
