﻿using SMO.Service.CF;

using System.Web.Mvc;

namespace SMO.Areas.CF.Controllers
{
    [AuthorizeCustom(Right = "R193")]
    public class ConfigTemplateNotifyController : Controller
    {
        private readonly ConfigTemplateNotifyService _service;

        public ConfigTemplateNotifyController()
        {
            _service = new ConfigTemplateNotifyService();
        }

        public ActionResult Index()
        {
            _service.GetTemplate();
            return PartialView(_service);
        }


        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Update(ConfigTemplateNotifyService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.Update();
            if (service.State)
            {
                SMOUtilities.GetMessage("1002", service, result);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult Delete(string pStrListSelected)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.Delete(pStrListSelected);
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