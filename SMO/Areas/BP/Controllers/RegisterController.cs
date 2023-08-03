using SMO.Service.BP;

using System.Web.Mvc;

namespace SMO.Areas.BP.Controllers
{
    [AuthorizeCustom(Right = "R322")]
    public class RegisterController : Controller
    {
        private readonly RegisterService _service;
        public RegisterController()
        {
            _service = new RegisterService();
        }
        // GET: BP/Register
        public ActionResult Index()
        {
            return PartialView(_service);
        }

        public ActionResult List(RegisterService service)
        {
            service.GetMyRegisters();
            return PartialView(service);
        }

        [HttpPost]
        public ActionResult SaveRegister(RegisterService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.SaveRegister();
            if (service.State)
            {
                SMOUtilities.GetMessage("1002", service, result);
                result.ExtData = "SubmitIndex();";
            }
            else
            {
                result.Type = TransferType.AlertDangerAndJsCommand;
                result.ExtData = "SubmitIndex();";
                SMOUtilities.GetMessage("1005", service, result);
            }
            return result.ToJsonResult();
        }
    }
}
