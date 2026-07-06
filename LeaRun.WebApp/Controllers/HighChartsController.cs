using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using xxoo.Common.HighChart;

namespace ToolsSite.Controllers
{
    public class HighChartsController : Controller
    {
        //
        // GET: /HighCharts/

        public ActionResult Index()
        {
            

            return View();
        }

        //修改 HighChart 导出服务器路径
        //exporting: { url: '/HighCharts/Export',   filename: 'MyChart',     width: 1200  },
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Export(string filename, string type, int width,string svg)
        {
            
            Exporter export = new Exporter(filename, type, width, svg);

            //// 1、文件名输出
            //FileStream fileWrite = new FileStream(@"d:\HighCharts导出图片.png", FileMode.Create, FileAccess.Write);
            //ExportFileInfo fi = export.WriteToStream(fileWrite);
            //string fileName = "导出的图片." + fi.Extension;
            //fileWrite.Close();
            //fileWrite.Dispose();
            //return File(@"d:\HighCharts导出图片.png", fi.ContentType, fileName);

            // 2、 字节输出
            ExportFileInfo fi;
            byte[] buffer = export.WriteToBuffer(out fi);
            string fileName = "导出的图片." + fi.Extension;
            return File(buffer, fi.ContentType, fileName);
        }


        #region 点击一个按钮导出图片

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ManualExport(string filename, string type, int width, string svg)
        {
            Exporter export = new Exporter(filename, type, width, svg);
            string fileId = Guid.NewGuid().ToString();
            string tempFilePath = Request.MapPath("~/DownFile/") + export.FileName;

            if (System.IO.File.Exists(tempFilePath))
            {
                System.IO.File.Delete(tempFilePath);
            }

            // 1、文件名输出
            FileStream fileWrite = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write);
            ExportFileInfo fi = export.WriteToStream(fileWrite);
            fileWrite.Close();
            fileWrite.Dispose();

            //Timer tr = new Timer(1000 * 60 * 1); // 1分钟后删除
            //tr.Elapsed += (s, e) =>
            //{
            //    if (System.IO.File.Exists(tempFilePath))
            //    {
            //        System.IO.File.Delete(tempFilePath);
            //    }
            //};

            // 添加定时器，5分钟后自动删除
            string downloadPath = string.Format( "/HighCharts/ManualDownload?fileId={0}&ContentType={1}&fileName={2}"
                    , fileId, fi.ContentType, export.FileName);

            var o = new { state = true, downloadpath = downloadPath };
            string result = JsonConvert.SerializeObject(o);

            return Content(result);
        }

        // 手动下载导出文件
        [HttpGet]
        public ActionResult ManualDownload(string fileId, string ContentType,string fileName)
        {
            string tempFilePath = Request.MapPath("~/DownFile/") + fileId;
            if (!System.IO.File.Exists(tempFilePath))
            {
                return Redirect("/HighCharts/Index?msg=文件已失效");
            }

            byte[] buffer = System.IO.File.ReadAllBytes(tempFilePath);
            System.IO.File.Delete(tempFilePath);
            return File(buffer, ContentType, fileName);
        }

        #endregion

    }
}
