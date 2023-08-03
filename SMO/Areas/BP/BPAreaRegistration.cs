using System.Web.Mvc;

namespace SMO.Areas.BP
{
    public class BPAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "BP";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "BP_default",
                "BP/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}