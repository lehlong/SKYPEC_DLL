using SMO.Service.MD;

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SMO.Areas.MD.Controllers
{
    public class CostCFElementController : Controller
    {
        private readonly CostCFElementService _service;
        public CostCFElementController()
        {
            _service = new CostCFElementService();
        }
        // GET: MD/CostElement
        [AuthorizeCustom(Right = "R212")]
        [MyValidateAntiForgeryToken]
        public ActionResult Index(int? year)
        {
            if (!year.HasValue)
            {
                year = DateTime.Now.Year;
            }
            _service.ObjDetail.TIME_YEAR = year.Value;
            return PartialView(_service);
        }

        [AuthorizeCustom(Right = "R302")]
        [MyValidateAntiForgeryToken]
        public ActionResult BuildTree(string elementSelected, int year)
        {
            var lstNode = _service.GetNodeCostElement(year);
            JavaScriptSerializer oSerializer = new JavaScriptSerializer
            {
                MaxJsonLength = int.MaxValue
            };
            ViewBag.zNode = oSerializer.Serialize(lstNode);
            ViewBag.ElementSelected = elementSelected;
            return PartialView();
        }

        [AuthorizeCustom(Right = "R212")]
        [MyValidateAntiForgeryToken]
        public ActionResult BuildTreeSap(int year)
        {
            var lstNode = _service.GetNodeSapCostElement(year);
            JavaScriptSerializer oSerializer = new JavaScriptSerializer
            {
                MaxJsonLength = int.MaxValue
            };
            ViewBag.zNodeSap = oSerializer.Serialize(lstNode);
            return PartialView();
        }

        [AuthorizeCustom(Right = "R212")]
        [MyValidateAntiForgeryToken]
        public ActionResult Create(string parent, int year)
        {
            _service.ObjDetail.PARENT_CODE = parent;
            _service.ObjDetail.TIME_YEAR = year;
            return PartialView(_service);
        }

        [AuthorizeCustom(Right = "R212")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CostCFElementService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.ObjDetail.IS_GROUP = true;
            service.ObjDetail.ACTIVE = true;
            service.Create();
            if (service.State)
            {
                SMOUtilities.GetMessage("1001", service, result);
                result.ExtData = string.Format("BuildTree('{0}', true);", service.ObjDetail.CODE);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", service, result);
            }
            return result.ToJsonResult();
        }

        [AuthorizeCustom(Right = "R212")]
        [MyValidateAntiForgeryToken]
        public ActionResult Edit(string id, int year)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                _service.ObjDetail = _service.GetFirstByExpression(x => x.CODE == id && x.TIME_YEAR == year);
            }
            return PartialView(_service);
        }


        [AuthorizeCustom(Right = "R212")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(CostCFElementService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.Update();
            if (service.State)
            {
                SMOUtilities.GetMessage("1002", service, result);
                result.ExtData = string.Format("BuildTree('{0}');", service.ObjDetail.CODE);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", service, result);
            }
            return result.ToJsonResult();
        }


        [AuthorizeCustom(Right = "R212")]
        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult Delete(string code, int year)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand,
                State = true
            };
            _service.Delete(code, year);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1003", _service, result);
                result.ExtData = string.Format("BuildTree('');");
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1006", _service, result);
            }
            return result.ToJsonResult();
        }

        /// <summary>
        /// Copy khoản mục từ năm này sang năm khác
        /// </summary>
        /// <param name="year"></param>
        /// <param name="yearCopy"></param>
        /// <returns></returns>
        [AuthorizeCustom(Right = "R212")]
        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult Copy(int year, int yearCopy)
        {
            var result = new TransferObject
            {
                State = true,
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.Copy(year, yearCopy);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [AuthorizeCustom(Right = "R212")]
        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult UpdateTree(List<NodeCostCenter> lstNode, List<string> lstRemove, List<string> lstAdd, int year)
        {
            var result = new TransferObject
            {
                State = true,
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.UpdateTree(lstNode, lstRemove, lstAdd, year);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [AuthorizeCustom(Right = "R302")]
        [MyValidateAntiForgeryToken]
        public JsonResult BuildTreeByTemplate(int? year)
        {
            var lstCostCenter = _service.GetNodeCostElement(year ?? DateTime.Now.Year);
            return Json(lstCostCenter, JsonRequestBehavior.AllowGet);
        }
    }
}
