using SMO.Core.Entities.WF;
using SMO.Service.Class.WF;
using SMO.Service.WF;

using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SMO.Areas.WF.Controllers
{
    [AuthorizeCustom(Right = "R194")]
    public class WorkFlowController : Controller
    {
        private readonly WorkFlowService _service;

        public WorkFlowController()
        {
            _service = new WorkFlowService();
        }

        public ActionResult Index()
        {
            return PartialView(_service);
        }

        public ActionResult BuildTree(string idSelected)
        {
            var lstNode = _service.BuildTree();
            JavaScriptSerializer oSerializer = new JavaScriptSerializer
            {
                MaxJsonLength = int.MaxValue
            };
            ViewBag.zNode = oSerializer.Serialize(lstNode);
            ViewBag.IdSelected = idSelected;
            return PartialView();
        }

        public ActionResult CreateProcess()
        {
            return PartialView(_service);
        }

        public ActionResult CreateActivity(string idParent)
        {
            _service.ObjActivity.PROCESS_CODE = idParent;
            return PartialView(_service);
        }

        public ActionResult EditProcess(string id)
        {
            _service.GetProcessById(id);
            return PartialView(_service);
        }

        public ActionResult EditActivity(string id)
        {
            _service.GetActivityById(id);
            return PartialView(_service);
        }

        public ActionResult EditCom(string id)
        {
            _service.GetComById(id);
            return PartialView(_service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProcess(WorkFlowService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.CreateProcess();
            if (service.State)
            {
                SMOUtilities.GetMessage("1002", service, result);
                result.ExtData = string.Format("BuildTree('{0}');", service.ObjProcess.CODE);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", service, result);
            }
            return result.ToJsonResult();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProcess(WorkFlowService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.UpdateProcess();
            if (service.State)
            {
                SMOUtilities.GetMessage("1002", service, result);
                result.ExtData = string.Format("BuildTree('{0}');", service.ObjProcess.CODE);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", service, result);
            }
            return result.ToJsonResult();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateActivity(WorkFlowService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.CreateActivity();
            if (service.State)
            {
                SMOUtilities.GetMessage("1002", service, result);
                result.ExtData = string.Format("BuildTree('{0}');", service.ObjActivity.CODE);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateActivity(WorkFlowService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.UpdateActivity();
            if (service.State)
            {
                SMOUtilities.GetMessage("1002", service, result);
                result.ExtData = string.Format("BuildTree('{0}');", service.ObjActivity.CODE);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", service, result);
            }
            return result.ToJsonResult();
        }

        public ActionResult ConfigSend()
        {
            return PartialView();
        }

        public ActionResult UpdateUserInformation(ConfigSendViewModel model)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.UpdateUserInformation(model);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCom(WorkFlowService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.UpdateCom();
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
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult SaveCommunication(T_WF_ACTIVITY_COM obj)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.ObjCom = obj;
            _service.SaveCom();
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "RefreshTableCommunications();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public virtual ActionResult ToggleActiveCom(string id)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.ToggleActiveCom(id);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "RefreshTableCommunications();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        public ActionResult GetCommunications(string activityCode)
        {
            var coms = _service.GetComs(activityCode);
            return PartialView("_PartialViewCommunications", coms);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult DeleteCommunications(string pStrListSelected)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.DeleteComs(pStrListSelected);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1003", _service, result);
                result.ExtData = "RefreshTableCommunications();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1006", _service, result);
            }
            return result.ToJsonResult();
        }

        public JsonResult BuildTreeConfigSend()
        {
            var lstTreeWorkflows = _service.GetNodeWorkflow();
            var lstTreeOrganizes = _service.GetNodeOrganize();
            var lstUsers = _service.GetNodeUser();
            return Json(new { workflows = lstTreeWorkflows, organizes = lstTreeOrganizes, users = lstUsers }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDetailInformationSenders(string workflow, string organize)
        {
            return Json(_service.GetDetailInformationSenders(workflow, organize), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDetailInformationReceivers(string workflow, string organize, string sender)
        {
            return Json(_service.GetDetailInformationReceivers(workflow, organize, sender), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDetailInformationWorkflow(string organize)
        {
            return Json(_service.GetDetailInformationWorkflow(organize), JsonRequestBehavior.AllowGet);
        }



    }
}