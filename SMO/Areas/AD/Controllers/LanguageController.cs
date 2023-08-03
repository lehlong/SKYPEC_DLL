﻿using SMO.Service.AD;

using System.Web.Mvc;

namespace SMO.Areas.AD.Controllers
{
    [AuthorizeCustom(Right = "R190")]
    public class LanguageController : Controller
    {
        private readonly LanguageService _service;

        public LanguageController()
        {
            _service = new LanguageService();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Index()
        {
            return PartialView(_service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult List(LanguageService service)
        {
            service.Search();
            return PartialView(service);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult Update(string value, string id)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.Update(value, id);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult DongBo()
        {
            return PartialView(_service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DongBo(LanguageService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            if (service.LangDestination != service.LangSource)
            {
                service.DongBo();
            }
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult MaintainLanguageForm(string formCode)
        {
            _service.ObjDetail.FORM_CODE = formCode;
            return PartialView(_service);
        }
    }
}