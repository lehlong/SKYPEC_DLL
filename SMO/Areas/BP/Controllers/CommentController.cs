using SMO.Service.BP;

using System.Web.Mvc;

namespace SMO.Areas.BP.Controllers
{
    public class CommentController : Controller
    {
        private readonly CommentHeaderService _service;
        public CommentController()
        {
            _service = new CommentHeaderService();
        }

        // GET: BP/Comment
        [MyValidateAntiForgeryToken]
        public ActionResult Index(string orgCode, string referenceCode, int year, int version, string objectType, string budgetType, string elementType)
        {
            if (objectType is null)
            {
                throw new System.ArgumentNullException(nameof(objectType));
            }

            _service.ObjDetail.ORG_CODE = orgCode;
            _service.ObjDetail.REFERENCE_CODE = referenceCode;
            _service.ObjDetail.YEAR = year;
            _service.ObjDetail.VERSION = version;
            _service.ObjDetail.OBJECT_TYPE = objectType;
            _service.ObjDetail.BUDGET_TYPE = budgetType;
            _service.ObjDetail.ELEMENT_TYPE = elementType;
            _service.GetHeader();
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult ExportData(string orgCode, string referenceCode, int year, int? version, string objectType, string budgetType, string elementType)
        {
            if (version == null)
            {
                var contentResult = new ContentResult
                {
                    Content = "<div class='title p-l-15'><h5>Không có dữ liệu</h5>"
                };
                return contentResult;
            }

            if (objectType is null)
            {
                throw new System.ArgumentNullException(nameof(objectType));
            }

            if (!_service.IsSelfUploadTemplate(orgCode, referenceCode))
            {
                var contentResult = new ContentResult
                {
                    Content = "<div class='title p-l-15'><h5>Không có quyền xem dữ liệu</h5>"
                };
                return contentResult;
            }
            _service.ObjDetail.ORG_CODE = orgCode;
            _service.ObjDetail.REFERENCE_CODE = referenceCode;
            _service.ObjDetail.YEAR = year;
            _service.ObjDetail.VERSION = version.Value;
            _service.ObjDetail.OBJECT_TYPE = objectType;
            _service.ObjDetail.ELEMENT_TYPE = elementType;
            _service.ObjDetail.BUDGET_TYPE = budgetType;
            _service.GetHeader();
            return PartialView(_service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult List(CommentHeaderService service)
        {
            service.GetComments();
            return PartialView(service.ObjDetail.Comments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Comment(CommentHeaderService service)
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
    }
}
