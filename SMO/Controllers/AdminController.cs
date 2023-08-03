using SMO.Helper;
using SMO.Service;
using SMO.Service.AD;
using SMO.Service.CM;
using SMO.Service.MD;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMO.Controllers
{
    //[SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]

    public class AdminController : Controller
    {
        //[System.Web.Mvc.Authorize]
        public ActionResult Index()
        {
            //var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            //hubContext.Clients.All.testNotify("kakalot");
            if (Request.Browser.Browser == "InternetExplorer" || Request.Browser.Browser == "IE")
            {
                return RedirectToAction("ChonTrinhDuyet");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Authorize");
            }

            var lang = "vi";
            if (Request.Cookies["lang"] != null)
            {
                lang = Request.Cookies["lang"].Value;
            }

            if (lang != "vi" && lang != "en")
            {
                lang = "vi";
            }

            ProfileUtilities.User.LANGUAGE = lang;
            //InitLanguage();

            var service = new NotifyService();
            service.GetNotifyOfUser(ProfileUtilities.User.USER_NAME);
            if (lang != "vi")
            {
                foreach (var item in service.ObjList)
                {
                    item.CONTENTS = item.CONTENTS_EN;
                    item.RAW_CONTENTS_EN = item.RAW_CONTENTS_EN;
                }
            }
            ViewBag.NotifyService = service;

            if (ProfileUtilities.User.USER_TYPE == UserType.NhaThau)
            {
                return View();
            }
            else
            {
                return View("IndexManager");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult ExportExcel(string html, IList<int> ignoreColumns)
        {
            var fileName = $"ExportExcel_{DateTime.Now.ToLongTimeString()}";
            MemoryStream outFileStream = new MemoryStream();
            var result = ExcelHelper.DownloadFile(ref outFileStream, string.Empty, html, ignoreColumns ?? new List<int>());
            if (result)
            {
                return File(outFileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
            }
            else
            {
                return Content("Đã có lỗi xảy ra trong quá trình xử lý!");
            }
        }

        public ActionResult THMCenterTemplate()
        {
            return PartialView();
        }

        public ActionResult THMCenterBudget(string modul)
        {
            ViewBag.Modul = modul;
            return PartialView();
        }

        public ActionResult THMCenterReviewResult()
        {
            return PartialView();
        }




        public ActionResult DashboardTableau()
        {
            return PartialView();
        }

        public ActionResult ChangeOrg(string orgCode)
        {
            var serviceOrg = new CostCenterService();
            serviceOrg.Get(orgCode);
            if (serviceOrg.ObjDetail != null && ProfileUtilities.UserOrg.Count(x => x.ORG_CODE == orgCode) > 0)
            {
                ProfileUtilities.User.ORGANIZE_CODE = orgCode;
                ProfileUtilities.User.Organize = serviceOrg.ObjDetail;
            }

            var authorizeService = new AuthorizeService();
            authorizeService.GetInfoUser(ProfileUtilities.User.USER_NAME);
            authorizeService.GetUserRight(orgCode);

            ProfileUtilities.UserRight = authorizeService.ListUserRight;
            return RedirectToAction("Index");
        }

        public ActionResult RefreshLanguage()
        {
            InitLanguage();
            return Content("OKI");
        }

        public ActionResult ChangLanguage()
        {
            ProfileUtilities.User.IS_CHANGE_LANG = !ProfileUtilities.User.IS_CHANGE_LANG;
            return Content(ProfileUtilities.User.IS_CHANGE_LANG ? "Bật chỉnh sửa ngôn ngữ" : "Tắt chỉnh sửa ngôn ngữ");
        }

        private void InitLanguage()
        {
            var service = new LanguageService();
            service.GetAll();
            foreach (var item in service.ObjList)
            {
                LanguageUtilities.AddToCache(new LanguageObject()
                {
                    Code = item.OBJECT_TYPE + "-" + item.FORM_CODE + "-" + item.FK_CODE,
                    Language = item.LANG,
                    Value = item.VALUE
                });
            }
        }

        public ActionResult SendMail()
        {
            var result = SMOUtilities.SendEmail();
            return Content(result);
        }

        public ActionResult ChonTrinhDuyet()
        {
            return View();
        }

        public ActionResult Menu()
        {
            var service = new MenuService();
            service.GetMenuRole();

            ViewBag.CountRequest = 0;
            ViewBag.CountFeeback = 0;

            return PartialView("MenuTree", service);
        }

        public ActionResult UnAuthorize(string auth)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertDanger
            };
            SMOUtilities.GetMessage("1100", result, auth);
            return result.ToJsonResult();
        }

        //[AuthorizeCustom(Right = "R111")]
        public void UploadImage(HttpPostedFileWrapper upload)
        {
            try
            {
                if (upload != null)
                {
                    var supportedTypes = new[] { "png", "jpeg", "jpg" };
                    var fileExt = System.IO.Path.GetExtension(upload.FileName).Substring(1);
                    if (!supportedTypes.Contains(fileExt))
                    {
                        throw new Exception("Chỉ chấp nhận ảnh : png, jpeg, jpg!");
                    }
                    else if (upload.ContentLength > (2 * 1024 * 1024))
                    {
                        throw new Exception("Ảnh vượt quá dung lượng 2 MB!");
                    }

                    try
                    {
                        using (var img = Image.FromStream(upload.InputStream))
                        {
                            if (img.RawFormat.Equals(ImageFormat.Png) & img.RawFormat.Equals(ImageFormat.Jpeg))
                            {
                                throw new Exception("Chỉ chấp nhận ảnh : png, jpeg, jpg!");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    string imageName = upload.FileName;
                    string path = System.IO.Path.Combine(Server.MapPath("~/Content/ImageUpload"), imageName);
                    upload.SaveAs(path);
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        public ActionResult SelectImage()
        {
            var appData = Server.MapPath("~/Content/ImageUpload");
            List<string> lstUrlImage = new List<string>();
            var images = Directory.GetFiles(appData);
            foreach (var item in images)
            {
                lstUrlImage.Add(Url.Content("/Content/ImageUpload/" + Path.GetFileName(item)));
            }
            ViewBag.ListImage = lstUrlImage;
            return PartialView();
        }
    }
}