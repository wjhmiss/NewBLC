
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using sharpPDF;
using Svg;
using Svg.Transforms;

namespace DNS.Utilities
{
    /// <summary>
    /// Processes web requests to export Highcharts JS JavaScript charts.
    /// </summary>
    public static class ExportChart
    {
        /// <summary>
        /// Processes HTTP Web requests to export SVG.
        /// </summary>
        /// <param name="context">An HttpContext object that provides references 
        /// to the intrinsic server objects (for example, Request, Response, 
        /// Session, and Server) used to service HTTP requests.</param>
        public static void ProcessExportRequest(HttpContext context)
        {
            if (context != null &&
              context.Request != null &&
              context.Response != null &&
              context.Request.HttpMethod == "POST")
            {
                HttpRequest request = context.Request;

                // Get HTTP POST form variables, ensuring they are not null.
                string filename = request.Form["filename"];
                string type = request.Form["type"];
                int width = 0;
                string svg = request.Form["svg"];

                if (filename != null &&
                  type != null &&
                  Int32.TryParse(request.Form["width"], out width) &&
                  svg != null)
                {
                    // Create a new chart export object using form variables.
                    Exporter export = new Exporter(filename, type, width, svg);

                    // Write the exported chart to the HTTP Response object.
                    export.WriteToHttpResponse(context.Response);

                    // Short-circuit this ASP.NET request and end. Short-circuiting
                    // prevents other modules from adding/interfering with the output.
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                    context.Response.End();
                }
            }
        }

        public static void ProcessExportRequest(string filename, string type, string svg,string widthStr,Stream stream)
        {
            // Get HTTP POST form variables, ensuring they are not null. 
            int width = 0; 

            if (filename != null &&
                type != null &&
                Int32.TryParse(widthStr, out width) &&
                svg != null)
            {
                // Create a new chart export object using form variables.
                Exporter export = new Exporter(filename, type, width, svg); 

                // Write the exported chart to the HTTP Response object.
                export.WriteToStream(stream);

                // Short-circuit this ASP.NET request and end. Short-circuiting
                // prevents other modules from adding/interfering with the output.
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                HttpContext.Current.Response.End();
            } 
        }
    }

    /// <summary>
    /// 导出的文件信息
    /// </summary>
    public class ExportFileInfo
    {
        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// 响应输出文件类型
        /// </summary>
        public string ContentType { get; set; }
    }

    /// <summary>
    /// .NET chart exporting class for Highcharts JS JavaScript charts.
    /// </summary>
    public class Exporter
    {
        /// <summary>
        /// Default file name to use for chart exports if not otherwise specified.
        /// </summary>
        private const string DefaultFileName = "Chart";

        /// <summary>
        /// PDF metadata Creator string.
        /// </summary>
        private const string PdfMetaCreator =
          "Tek4(TM) Exporting Module for Highcharts JS from Tek4.com";

        /// <summary>
        /// Gets the HTTP Content-Disposition header to be sent with an HTTP
        /// response that will cause the client's browser to open a file save
        /// dialog with the proper file name.
        /// </summary>
        public string ContentDisposition { get; private set; }

        /// <summary>
        /// Gets the MIME type of the exported output.
        /// </summary>
        public string ContentType { get; private set; }

        /// <summary>
        /// Gets the file name with extension to use for the exported chart.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the chart name (same as file name without extension).
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the SVG chart document (XML text).
        /// </summary>
        public string Svg { get; private set; }

        /// <summary>
        /// Gets the pixel width of the exported chart image.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Initializes a new chart Export object using the specified file name, 
        /// output type, chart width and SVG text data.
        /// </summary>
        /// <param name="fileName">The file name (without extension) to be used 
        /// for the exported chart.</param>
        /// <param name="type">The requested MIME type to be generated. Can be
        /// 'image/jpeg', 'image/png', 'application/pdf' or 'image/svg+xml'.</param>
        /// <param name="width">The pixel width of the exported chart image.</param>
        /// <param name="svg">An SVG chart document to export (XML text).</param>
        public Exporter(
          string fileName,
          string type,
          int width,
          string svg)
        {
            string extension;

            this.ContentType = type.ToLower();
            this.Name = fileName;
            this.Svg = svg;
            this.Width = width;

            // Validate requested MIME type.
            switch (ContentType)
            {
                case "image/jpeg":
                    extension = "jpg";
                    break;

                case "image/png":
                    extension = "png";
                    break;

                case "application/pdf":
                    extension = "pdf";
                    break;

                case "image/svg+xml":
                    extension = "svg";
                    break;

                // Unknown type specified. Throw exception.
                default:
                    throw new ArgumentException(
                      string.Format("Invalid type specified: '{0}'.", type));
            }

            // Determine output file name.
            this.FileName = string.Format(
              "{0}.{1}",
              string.IsNullOrEmpty(fileName) ? DefaultFileName : fileName,
              extension);

            // Create HTTP Content-Disposition header.
            this.ContentDisposition =
              string.Format("attachment; filename={0}", this.FileName);
        }

        /// <summary>
        /// Creates an SvgDocument from the SVG text string.
        /// </summary>
        /// <returns>An SvgDocument object.</returns>
        private SvgDocument CreateSvgDocument()
        {
            SvgDocument svgDoc;

            // Create a MemoryStream from SVG string.
            using (MemoryStream streamSvg = new MemoryStream(
              Encoding.UTF8.GetBytes(this.Svg)))
            {
                svgDoc = SvgDocument.Open<SvgDocument>(streamSvg);
            }

            // Scale SVG document to requested width.
            svgDoc.Transforms = new SvgTransformCollection();
            float scalar = (float)this.Width / (float)svgDoc.Width;
            svgDoc.Transforms.Add(new SvgScale(scalar, scalar));
            svgDoc.Width = new SvgUnit(svgDoc.Width.Type, svgDoc.Width * scalar);
            svgDoc.Height = new SvgUnit(svgDoc.Height.Type, svgDoc.Height * scalar);

            return svgDoc;
        }

        /// <summary>
        /// Exports the chart to the specified HttpResponse object. This method
        /// is preferred over WriteToStream() because it handles clearing the
        /// output stream and setting the HTTP reponse headers.
        /// </summary>
        /// <param name="httpResponse"></param>
        public void WriteToHttpResponse(HttpResponse httpResponse)
        {
            httpResponse.ClearContent();
            httpResponse.ClearHeaders();
            httpResponse.ContentType = this.ContentType;
            httpResponse.AddHeader("Content-Disposition", this.ContentDisposition);
            WriteToStream(httpResponse.OutputStream);
        }

        /// <summary>
        ///  导出
        /// </summary>
        /// <param name="outputStream">流</param>
        /// <returns>文件扩展名 和 ContentType</returns>
        public ExportFileInfo WriteToStream(Stream outputStream)
        {
            ExportFileInfo fi = new ExportFileInfo(); 

            string[] fileExtenContentType = new string[2];
            
            switch (this.ContentType)
            {
                case "image/jpeg":
                    CreateSvgDocument().Draw().Save(
                      outputStream,
                      ImageFormat.Jpeg);
                    fi.Extension    = "jpg";
                    fi.ContentType  = "image/jpeg";
                    break;

                case "image/png":
                    // PNG output requires a seekable stream.
                    using (MemoryStream seekableStream = new MemoryStream())
                    {
                        CreateSvgDocument().Draw().Save(
                            seekableStream,
                            ImageFormat.Png);
                        seekableStream.WriteTo(outputStream);
                    }
                    fi.Extension   = "png";
                    fi.ContentType = "image/png";
                    break;

                case "application/pdf":
                    SvgDocument svgDoc = CreateSvgDocument();
                    Bitmap bmp = svgDoc.Draw();

                    pdfDocument doc = new pdfDocument(this.Name, null);
                    pdfPage page = doc.addPage(bmp.Height, bmp.Width);
                    page.addImage(bmp, 0, 0);
                    doc.createPDF(outputStream);
                    fi.Extension    = "pdf";
                    fi.ContentType  = "application/pdf";
                    break;

                case "image/svg+xml":
                    using (StreamWriter writer = new StreamWriter(outputStream))
                    {
                        writer.Write(this.Svg);
                        writer.Flush();
                    }
                    fi.Extension    = "svg";
                    fi.ContentType  = "image/svg";
                    break;

                default:
                    throw new InvalidOperationException(string.Format(
                      "ContentType '{0}' is invalid.", this.ContentType));
            } 

            return fi;
        }

        /// <summary>
        ///  导出
        /// </summary>
        /// <param name="ExportFileInfo">文件信息</param>
        /// <returns>图片字节数组</returns>
        public byte[] WriteToBuffer(out ExportFileInfo fi)
        {
            fi = new ExportFileInfo();
            byte[] buffer = null;
            int length = -1;
            // File(buffer, fileExtenContentType[1], fileName);

            string[] fileExtenContentType = new string[2];

            switch (this.ContentType)
            {
                case "image/jpeg":
                    using (MemoryStream seekableStream = new MemoryStream())
                    {
                        Bitmap bmp = CreateSvgDocument().Draw();
                        bmp.Save(
                           seekableStream,
                           ImageFormat.Jpeg);
                        buffer = seekableStream.GetBuffer();
                    }
                    fi.Extension    = "jpg";
                    fi.ContentType  = "image/jpeg";
                    break;

                case "image/png":
                     
                    // PNG output requires a seekable stream.
                    using (MemoryStream seekableStream = new MemoryStream())
                    {
                        CreateSvgDocument().Draw().Save(
                            seekableStream,
                            ImageFormat.Png);
                        buffer = seekableStream.GetBuffer();
                    }
                    fi.Extension    = "png";
                    fi.ContentType  = "image/png";
                    break;

                case "application/pdf":
                    using (MemoryStream seekableStream = new MemoryStream())
                    {
                        SvgDocument svgDoc = CreateSvgDocument();
                        Bitmap bmp = svgDoc.Draw();

                        pdfDocument doc = new pdfDocument(this.Name, null);
                        pdfPage page = doc.addPage(bmp.Height, bmp.Width);
                        page.addImage(bmp, 0, 0);
                        doc.createPDF(seekableStream);
                        buffer = seekableStream.GetBuffer();
                    }
                    fi.Extension    = "pdf";
                    fi.ContentType  = "application/pdf";
                    break;

                case "image/svg+xml":

                    buffer = Encoding.UTF8.GetBytes(this.Svg);
                    fi.Extension    = "svg";
                    fi.ContentType  = "image/svg";
                    break;

                default:
                    throw new InvalidOperationException(string.Format(
                      "ContentType '{0}' is invalid.", this.ContentType));
            }

            

            return buffer;
        }

    }


    /* 调用示例
     * 引用程序集: sharpPDF.dll,Svg.dll
     * 
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
     */


}
