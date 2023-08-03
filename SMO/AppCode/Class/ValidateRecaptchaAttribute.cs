using SMO.Service;

using System.Configuration;
using System.Web.Mvc;

namespace SMO
{
    public class ValidateRecaptchaAttribute : ActionFilterAttribute
    {
        private const string RECAPTCHA_RESPONSE_KEY = "g-recaptcha-response";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (Global.ListIPLogin.ContainsKey(ip))
            {
                Global.ListIPLogin.TryGetValue(ip, out int count);
                if (count < 4)
                {
                    return;
                }
                var isValidate = new RecaptchaValidationService(ConfigurationManager.AppSettings["RecaptchaSecretKey"]).Validate(filterContext.HttpContext.Request[RECAPTCHA_RESPONSE_KEY], ip);
                if (!isValidate)
                    filterContext.Controller.ViewData.ModelState.AddModelError("Recaptcha", "Captcha validation failed.");
            }
        }
    }
}