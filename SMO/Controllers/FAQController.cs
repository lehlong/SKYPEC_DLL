using SMO.Service;
using SMO.Service.MD;

using System.Configuration;
using System.Web.Mvc;

namespace SMO.Controllers
{
    public class FAQController : Controller
    {
        private readonly FrequentlyAskedService _service;

        public FAQController()
        {
            _service = new FrequentlyAskedService();
        }

        public ActionResult Index()
        {
            _service.GetAll();
            return View(_service);
        }

        public ActionResult CreateQuestion(FrequentlyAskedService service)
        {
            var lang = "vi";
            if (Request.Cookies["lang"] != null)
            {
                lang = Request.Cookies["lang"].Value;
            }

            if (lang != "vi" && lang != "en")
            {
                lang = "vi";
            }

            var result = new TransferObject
            {
                Type = TransferType.JsCommand,
                State = true
            };

            var ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            var isValidate = new RecaptchaValidationService(ConfigurationManager.AppSettings["RecaptchaSecretKey"]).Validate(System.Web.HttpContext.Current.Request["g-recaptcha-response"], ip);

            if (!isValidate)
            {
                result.State = false;
                var message = "CAPCHA KHÔNG CHÍNH XÁC! CÓ THỂ BẠN CHƯA CHỌN : TÔI KHÔNG PHẢI LÀ NGƯỜI MÁY";
                if (lang != "vi")
                {
                    message = "CAPCHA NOT VALID! YOU MUST CHECK: IM NOT A ROBOT";
                }
                result.ExtData = $"alert(\"{message}\");";
            }
            else
            {
                service.CreateQuestion();
                if (service.State)
                {
                    var message = "GỬI CÂU HỎI THÀNH CÔNG. CHÚNG TÔI SẼ LIÊN HỆ LẠI SỚM, TRÂN TRỌNG CẢM ƠN QUÝ KHÁCH!";
                    if (lang != "vi")
                    {
                        message = "SUCCESS. WE WILL CONTACT YOU SOON, THANK YOU VERY MUCH!";
                    }
                    result.ExtData = $"alert(\"{message}\");$(\"#frmCauHoi\").trigger(\"reset\");location.reload();";
                }
                else
                {
                    var message = "GỬI CÂU HỎI KHÔNG THÀNH CÔNG! QUÝ KHÁCH VUI LÒNG THỬ LẠI.";
                    if (lang != "vi")
                    {
                        message = "NOT SUCCESS! PLEASE TRY AGAIN.";
                    }
                    result.ExtData = $"alert(\"{message}\");";
                }
            }

            return result.ToJsonResult();
        }
    }
}