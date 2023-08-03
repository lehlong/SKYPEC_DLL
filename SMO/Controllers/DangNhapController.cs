using IZ.WebFileManager;

using SMO.Service;
using SMO.Service.AD;

using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SMO.Areas.Controllers
{
    public class DangNhapController : Controller
    {

        public DangNhapController()
        {
        }

        public ActionResult Index(string ReturnUrl)
        {
            AuthorizeService _service = new AuthorizeService();
            if (Request.IsAuthenticated)
            {
                return Redirect("/Admin");
            }
            if (Request.Cookies["Login"] != null)
            {
                _service.ObjUser.USER_NAME = Request.Cookies["Login"].Values["USER_NAME"];
                if (Request.Cookies["Login"].Values["IS_LOGIN_AD"] == "1")
                {
                    _service.ObjUser.IS_LOGIN_AD = true;
                }
            }
            _service.ReturnUrl = ReturnUrl;
            return PartialView(_service);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        //[ValidateRecaptcha]
        public ActionResult Index(AuthorizeService service)
        {
            //if (ModelState.ContainsKey("Recaptcha"))
            //{
            //    ViewBag.Error = "3";
            //    return PartialView(service);
            //}
            var isLoginAD = service.ObjUser.IS_LOGIN_AD;

            var historyService = new UserHistoryService();
            historyService.ObjDetail.PKID = Guid.NewGuid().ToString();
            historyService.ObjDetail.USER_NAME = service.ObjUser.USER_NAME;
            historyService.ObjDetail.LOGON_TIME = DateTime.Now;
            historyService.ObjDetail.BROWSER = Request.Browser.Browser;
            historyService.ObjDetail.VERSION = Request.Browser.Version;
            historyService.ObjDetail.IS_MOBILE = Request.Browser.IsMobileDevice;
            historyService.ObjDetail.MOBILE_MODEL = Request.Browser.MobileDeviceModel;
            historyService.ObjDetail.MANUFACTURER = Request.Browser.MobileDeviceManufacturer;
            historyService.ObjDetail.OS = Request.Browser.Platform;
            historyService.ObjDetail.IP_ADDRESS = System.Web.HttpContext.Current.Request.ServerVariables["X_FORWARDED_FOR"];
            //if (string.IsNullOrEmpty(historyService.ObjDetail.IP_ADDRESS))
            //{
            //    historyService.ObjDetail.IP_ADDRESS = System.Web.HttpContext.Current.Request.ServerVariables["X_FORWARDED_FOR"];
            //}
            if (string.IsNullOrEmpty(historyService.ObjDetail.IP_ADDRESS))
            {
                historyService.ObjDetail.IP_ADDRESS = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            service.IsValid();
            if (service.State)
            {
                if (!service.ObjUser.ACTIVE)
                {
                    ViewBag.Error = "2";
                    historyService.ObjDetail.STATUS = false;
                    historyService.Create();
                    return PartialView(service);
                }

                FormsAuthentication.SetAuthCookie(service.ObjUser.USER_NAME, service.IsRemember);
                if (!service.ObjUser.IS_IGNORE_USER)
                {
                    service.GetUserRight();
                }

                HttpCookie cookie = new HttpCookie("Login");
                cookie.Values.Add("USER_NAME", service.ObjUser.USER_NAME);
                cookie.Values.Add("IS_LOGIN_AD", isLoginAD ? "1" : "0");
                cookie.Expires = DateTime.Now.AddDays(15);
                Response.Cookies.Add(cookie);


                ProfileUtilities.User = service.ObjUser;
                ProfileUtilities.UserRight = service.ListUserRight;
                //TEST
                //ProfileUtilities.User.IS_IGNORE_USER = true;
                service.ReturnUrl = string.IsNullOrWhiteSpace(service.ReturnUrl) ? "/" : service.ReturnUrl;
                historyService.ObjDetail.STATUS = true;
                historyService.Create();

                if (Global.ListIPLogin.ContainsKey(historyService.ObjDetail.IP_ADDRESS))
                {
                    Global.ListIPLogin[historyService.ObjDetail.IP_ADDRESS] = 0;
                }

                MB.FileBrowser.MagicSession.Current.FileBrowserAccessMode = AccessMode.Delete;
                return Redirect(service.ReturnUrl);
            }
            else
            {
                ViewBag.Error = service.ErrorMessage;
                historyService.ObjDetail.STATUS = false;
                historyService.Create();
                if (Global.ListIPLogin.ContainsKey(historyService.ObjDetail.IP_ADDRESS))
                {
                    Global.ListIPLogin[historyService.ObjDetail.IP_ADDRESS] += 1;
                }
                else
                {
                    Global.ListIPLogin.Add(historyService.ObjDetail.IP_ADDRESS, 1);
                }
            }
            return PartialView(service);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            FormsAuthentication.SignOut();
            return Redirect(FormsAuthentication.LoginUrl);
        }

    }
}