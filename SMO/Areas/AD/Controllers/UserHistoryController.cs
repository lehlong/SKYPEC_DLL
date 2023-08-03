using SMO.Service.AD;

using System.Web.Mvc;

namespace SMO.Areas.AD.Controllers
{
    [AuthorizeCustom(Right = "R140")]
    public class UserHistoryController : Controller
    {
        private readonly UserHistoryService _service;
        public UserHistoryController()
        {
            _service = new UserHistoryService();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Index(string userName)
        {
            _service.ObjDetail.USER_NAME = userName;
            return PartialView(_service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult List(UserHistoryService service)
        {
            service.Search();
            return PartialView(service);
        }
    }
}