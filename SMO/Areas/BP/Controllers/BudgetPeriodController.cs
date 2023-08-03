using SMO.Service.BP;

using System.Web.Mvc;
using System.Web.SessionState;

namespace SMO.Areas.BP.Controllers
{
    [AuthorizeCustom(Right = "R325")]
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class BudgetPeriodController : Controller
    {
        private readonly BudgetPeriodService _service;
        public BudgetPeriodController()
        {
            _service = new BudgetPeriodService();
        }

        public ActionResult Index()
        {
            return PartialView(_service);
        }

        public ActionResult List(BudgetPeriodService service)
        {
            service.Search();
            if (service.State)
            {
                return PartialView(service);
            }
            else
            {
                return Content("Error when archived data!");
            }
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Edit(string id, string editFormId)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                _service.Get(id);
            }
            _service.FormId = editFormId;
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult History(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                _service.GetHistory(id);
            }
            return PartialView(_service);
        }



        [HttpPost]
        public ActionResult Update(BudgetPeriodService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.Update();
            if (service.State)
            {
                SMOUtilities.GetMessage("1002", service, result);
                result.ExtData = $"SubmitIndex();loadHistory('{service.ObjDetail.ID}');";
            }
            else
            {
                result.Type = TransferType.AlertDangerAndJsCommand;
                result.ExtData = $"SubmitIndex();)";
                SMOUtilities.GetMessage("1005", service, result);
            }
            return result.ToJsonResult();
        }
    }
}
