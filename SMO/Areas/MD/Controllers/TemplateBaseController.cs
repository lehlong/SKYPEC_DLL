using SMO.Service.MD;

using System.Web.Mvc;

namespace SMO.Areas.MD.Controllers
{
    //[AuthorizeCustom(Right = "R270")]
    public class TemplateBaseController : Controller
    {
        private readonly TemplateBaseService _service;

        public TemplateBaseController()
        {
            _service = new TemplateBaseService();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Index(string templateCode, int year)
        {
            _service.ObjDetail.TEMPLATE_CODE = templateCode;
            _service.ObjDetail.TIME_YEAR = year;
            return PartialView(_service);
        }

        public ActionResult List(TemplateBaseService service)
        {
            service.Search();
            return PartialView(service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult UploadFile(TemplateBaseService service)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            service.UploadFile(Request);
            if (service.State)
            {
                SMOUtilities.GetMessage("1002", service, result);
                result.ExtData = "SubmitIndexTemplateBase();UploadFile.ClearFile();$('#divPreviewFile').html('');";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", service, result);
            }
            return result.ToJsonResult();
        }
    }
}