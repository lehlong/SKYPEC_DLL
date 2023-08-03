using Newtonsoft.Json;

using SMO.Service.BP.REVENUE_CF;
using SMO.Service.Class;

using System.Web.Mvc;

namespace SMO.Areas.BP.Controllers
{
    public class RevenueCFReviewCommentController : Controller
    {
        private readonly RevenueCFReviewCommentService _service;

        public RevenueCFReviewCommentController()
        {
            _service = new RevenueCFReviewCommentService();
        }

        /// <summary>
        /// modal
        /// </summary>
        /// <param name="orgCode"></param>
        /// <param name="elementCode"></param>
        /// <param name="year"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        [MyValidateAntiForgeryToken]
        public ActionResult ReviewData(string orgCode, string elementCode, int year, int version, string onOrgCode)
        {
            _service.GetHeader(orgCode, elementCode, year, version, onOrgCode);

            return PartialView(_service);
        }

        //[MyValidateAntiForgeryToken]
        public ActionResult Index(string orgCode, string elementCode, int year, int version, string onOrgCode)
        {
            _service.GetHeader(orgCode, elementCode, year, version, onOrgCode);

            return PartialView(_service);
        }

        [HttpGet]
        [MyValidateAntiForgeryToken]
        public JsonResult RefreshComment(int year, string orgCode, string elementCode, string onOrgCode)
        {
            return Json(_service.GetComments(year, orgCode, elementCode, onOrgCode), JsonRequestBehavior.AllowGet);
        }


        [ValidateAntiForgeryToken]
        public ActionResult List(RevenueCFReviewCommentService service)
        {
            service.GetComments();
            return PartialView("../Comment/ListRevenueCF", service.ObjList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Comment(RevenueCFReviewCommentService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.Create();
            if (service.State)
            {
                SMOUtilities.GetMessage("1001", service, result);
                result.ExtData = string.Format("Forms.SubmitForm('{0}'); $('#txtContent').val('')", service.ObjDetail.PKID);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", service, result);
            }
            return result.ToJsonResult();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CommentDataCenter(RevenueCFReviewCommentService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.Create();
            if (service.State)
            {
                SMOUtilities.GetMessage("1001", service, result);
                result.ExtData = $"Forms.SubmitForm('{service.ObjDetail.PKID}'); " +
                    $"RefreshComment('{service.ObjDetail.REVENUE_CF_ELEMENT_CODE}', '{service.ObjDetail.ON_ORG_CODE}'); " +
                    $"$('#txtContent').val('')";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", service, result);
            }
            return result.ToJsonResult();

        }

        [MyValidateAntiForgeryToken]
        public ActionResult FilterCommentCenter(FilterCommentCenterViewModel model)
        {
            return PartialView(model);
        }

        [HttpGet]
        public JsonResult GetElements(int year)
        {
            var lstElements = _service.GetElements(year);
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            };
            return Json(JsonConvert.SerializeObject(lstElements, settings), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetUsersComment(int year, int? version, string elementCode, string centerCode)
        {
            var lstUser = _service.GetUsersComment(year, version, elementCode, centerCode);

            return Json(lstUser, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetVersions(int year, string elementCode)
        {
            var lstUser = _service.GetVersions(year, elementCode);

            return Json(lstUser, JsonRequestBehavior.AllowGet);
        }

    }
}