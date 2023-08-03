using SMO.Service;

using System.Web.Mvc;

namespace SMO.Controllers
{
    public class QuanLyDBController : Controller
    {
        private readonly DynamicSqlService _service;

        public QuanLyDBController()
        {
            _service = new DynamicSqlService();
        }

        [Authorize]
        public ActionResult Index(string strSql)
        {
            if (ProfileUtilities.User.USER_NAME.ToLower() != "superadmin")
            {
                return Content("");
            }
            _service.ObjDetail.TextSql = strSql;
            return View(_service);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Run(DynamicSqlService _service)
        {

            if (ProfileUtilities.User.USER_NAME.ToLower() != "superadmin")
            {
                return Content("");
            }
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            if (ModelState.IsValid)
            {
                var bolResult = _service.RunSql(_service.ObjDetail.TextSql);
                if (bolResult)
                {
                    return PartialView(_service);
                }
                else
                {
                    result.Type = TransferType.AlertDanger;
                    SMOUtilities.GetMessage("1101", _service, result);
                }
            }

            return result.ToJsonResult();
        }
    }
}