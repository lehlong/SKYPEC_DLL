using SMO.Service.AD;
using SMO.Service.CM;

using System;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace SMO.Areas.CM.Controllers
{
    [Authorize]
    public class FileController : Controller
    {
        public ActionResult DownloadFile(string id, string isViewFile = "0")
        {
            try
            {
                var serviceFile = new FileUploadService();
                serviceFile.Get(id);

                var serviceConnection = new ConnectionService();
                serviceConnection.Get(serviceFile.ObjDetail.CONNECTION_ID);

                var filePath = Path.Combine(serviceConnection.ObjDetail.DIRECTORY.TrimEnd(System.IO.Path.DirectorySeparatorChar), serviceFile.ObjDetail.DIRECTORY_PATH.TrimStart(System.IO.Path.DirectorySeparatorChar), serviceFile.ObjDetail.FILE_NAME);
                if (!System.IO.File.Exists(filePath))
                {
                    return Content($"<script>alert('Đường dẫn tới file không tồn tại!');</script>;");
                }

                if (isViewFile == "1")
                {
                    var contentDispositionHeader = new System.Net.Mime.ContentDisposition
                    {
                        Inline = true,
                        FileName = serviceFile.ObjDetail.FILE_NAME
                    };
                    Response.Headers.Add("Content-Disposition", contentDispositionHeader.ToString());
                    var fileByte = System.IO.File.ReadAllBytes(filePath);
                    return File(fileByte, MimeMapping.GetMimeMapping(serviceFile.ObjDetail.FILE_NAME));
                }

                return File(filePath, MimeMapping.GetMimeMapping(serviceFile.ObjDetail.FILE_NAME), serviceFile.ObjDetail.FILE_OLD_NAME);
            }
            catch
            {
                return Content($"<script>alert('Download file bị lỗi!');</script>;");
            }
        }

        [Authorize]
        public ActionResult ViewFileOnline(string id)
        {
            var serviceFile = new FileUploadService();
            serviceFile.Get(id);
            return PartialView(serviceFile);
        }

        [Authorize]
        public ActionResult MoveFileToTemp(string id)
        {
            var result = new TransferObject();
            result.Type = TransferType.JsCommand;
            try
            {
                var serviceFile = new FileUploadService();
                serviceFile.Get(id);

                var serviceConnection = new ConnectionService();
                serviceConnection.Get(serviceFile.ObjDetail.CONNECTION_ID);

                var filePath = Path.Combine(serviceConnection.ObjDetail.DIRECTORY.TrimEnd(System.IO.Path.DirectorySeparatorChar), serviceFile.ObjDetail.DIRECTORY_PATH.TrimStart(System.IO.Path.DirectorySeparatorChar), serviceFile.ObjDetail.FILE_NAME);
                if (!System.IO.File.Exists(filePath))
                {
                    result.State = false;
                    result.Message.Message = "Đường dẫn tới file không tồn tại!";
                }
                else
                {
                    var filePathTemp = Path.Combine("\\TempViewFile", serviceFile.ObjDetail.FILE_NAME);
                    if (!System.IO.File.Exists(HostingEnvironment.MapPath(filePathTemp)))
                    {
                        System.IO.File.Copy(filePath, HostingEnvironment.MapPath(filePathTemp));
                    }

                    result.State = true;
                    result.ExtData = filePathTemp.Replace('\\', '/');
                }
            }
            catch (Exception ex)
            {
                result.State = false;
                result.Message.Message = "Quá trình xem file bị lỗi!";
                result.Message.Detail = ex.ToString();
            }
            return result.ToJsonResult();
        }
    }
}