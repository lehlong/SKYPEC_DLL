using SMO.Service.CM;

using System.Web.Mvc;

namespace SMO.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly CommentService _service;

        public CommentController()
        {
            _service = new CommentService();
        }

        public ActionResult Index(string referenceId)
        {
            _service.ObjDetail.REFRENCE_ID = referenceId;
            return PartialView(_service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult List(CommentService service)
        {
            service.GetComments();
            return PartialView(service);
        }

        //[ValidateAntiForgeryToken]
        //public ActionResult ListBP(T_CM_HEADER_BP_COMMENT header)
        //{
        //    _service.GetComments(header);
        //    return PartialView(nameof(List), _service);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CommentService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.Create();
            if (service.State)
            {
                SMOUtilities.GetMessage("1001", service, result);
                result.ExtData = string.Format("Forms.SubmitForm('{0}'); $('#txtContent').val('')", service.ObjDetail.REFRENCE_ID);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", service, result);
            }
            return result.ToJsonResult();
        }
    }
}