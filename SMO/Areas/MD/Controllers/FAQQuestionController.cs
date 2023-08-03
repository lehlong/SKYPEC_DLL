using SMO.Service.MD;

using System.Web.Mvc;

namespace SMO.Areas.MD.Controllers
{
    public class FAQQuestionController : Controller
    {
        private readonly FAQQuestionService _service;

        public FAQQuestionController()
        {
            _service = new FAQQuestionService();
        }

        [AuthorizeCustom(Right = "R800")]
        [MyValidateAntiForgeryToken]
        public ActionResult Index()
        {
            return PartialView(_service);
        }

        [AuthorizeCustom(Right = "R800")]
        [ValidateAntiForgeryToken]
        public ActionResult List(FAQQuestionService service)
        {
            service.Search();
            return PartialView(service);
        }

        [AuthorizeCustom(Right = "R801")]
        [MyValidateAntiForgeryToken]
        public ActionResult Edit(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                _service.Get(id);
            }
            return PartialView(_service);
        }

        [AuthorizeCustom(Right = "R801")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Update(FAQQuestionService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.UpdateAnswer();
            if (service.State)
            {
                SMOUtilities.GetMessage("1002", service, result);
                result.ExtData = "SubmitIndex();";
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