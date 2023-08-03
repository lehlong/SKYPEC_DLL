using SMO.Service.MD;

using System.Collections.Generic;
using System.Web.Mvc;

namespace SMO.Areas.MD.Controllers
{
    [AuthorizeCustom(Right = "R217")]
    public class UserReviewController : Controller
    {
        private readonly UserReviewService _service;
        public UserReviewController()
        {
            _service = new UserReviewService();
        }
        // GET: MD/UserReview
        public ActionResult Index()
        {
            return PartialView(_service);
        }

        public ActionResult List(UserReviewService service)
        {
            ViewBag.PeriodYears = service.GetPeriodTime();
            service.GetAll();
            return PartialView(service);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult Create(IList<string> userNames, int year)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.Create(year, userNames);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1001", _service, result);
                result.ExtData = "SubmitIndex();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult Delete(string userName, int year)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand,
                State = true
            };
            _service.DeleteEntity(userName, year);

            if (_service.State)
            {
                SMOUtilities.GetMessage("1003", _service, result);
                result.ExtData = "SubmitIndex();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1006", _service, result);
            }
            return result.ToJsonResult();
        }

    }
}