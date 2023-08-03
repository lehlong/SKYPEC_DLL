using SMO.Service.MD;
using System;
using System.Collections.Generic;

using System.Web.Mvc;

namespace SMO.Areas.MD.Controllers
{
    public class InternalOrderController : Controller
    {
        private readonly InternalOrderService _service;

        public InternalOrderController()
        {
            _service = new InternalOrderService();
        }

        [AuthorizeCustom(Right = "R208")]
        [MyValidateAntiForgeryToken]
        public ActionResult Index()
        {
            return PartialView(_service);
        }


        [AuthorizeCustom(Right = "R208")]
        [ValidateAntiForgeryToken]
        public ActionResult List(InternalOrderService service)
        {
            service.Search();
            return PartialView(service);
        }

        [AuthorizeCustom(Right = "R302")]
        [MyValidateAntiForgeryToken]
        public JsonResult BuildTreeByTemplate(string templateId, int year)
        {
            var lstCostCenter = _service.GetNodeCostCenterByTemplate(templateId, year);
            var result = Json(lstCostCenter, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = Int32.MaxValue;
            return result;
        }

        [HttpPost]
        [AuthorizeCustom(Right = "R208")]
        [MyValidateAntiForgeryToken]
        public ActionResult Synchronize()
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.Synchronize();
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "SubmitIndex();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }
    }
}
