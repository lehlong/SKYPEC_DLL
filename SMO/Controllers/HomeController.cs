using System.Web.Mvc;

namespace SMO.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult UnAuthorize(string auth)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertDanger
            };
            SMOUtilities.GetMessage("1100", result, auth);
            return result.ToJsonResult();
        }
    }
}