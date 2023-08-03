using System.Web.Mvc;

namespace SMO.Areas.RP
{
    public class RPAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "RP";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "RP_default",
                "RP/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}