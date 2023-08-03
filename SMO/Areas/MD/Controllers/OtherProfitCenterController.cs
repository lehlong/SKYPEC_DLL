using SMO.Service.MD;

using System.Web.Mvc;

namespace SMO.Areas.MD.Controllers
{
    //[AuthorizeCustom(Right = "R208")]
    public class OtherProfitCenterController : Controller
    {
        private readonly OtherProfitCenterService _service;

        public OtherProfitCenterController()
        {
            _service = new OtherProfitCenterService();
        }

        [AuthorizeCustom(Right = "R302")]
        [MyValidateAntiForgeryToken]
        public JsonResult BuildTreeByTemplate(string templateId, int year)
        {
            var lstCompanies = _service.GetNodeCompanyByTemplate(templateId, year);
            var lstProjects = _service.GetNodeProjectByTemplate(templateId, year);
            return Json(new { companies = lstCompanies, projects = lstProjects }, JsonRequestBehavior.AllowGet);
        }
    }
}
