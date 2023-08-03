using Hangfire.Annotations;
using Hangfire.Dashboard;

using Microsoft.Owin;

namespace SMO
{
    public class MyRestrictiveAuthorizationFilter : IDashboardAuthorizationFilter
    {
        //public bool Authorize(IDictionary<string, object> owinEnvironment)
        //{
        //    // In case you need an OWIN context, use the next line,
        //    // `OwinContext` class is the part of the `Microsoft.Owin` package.
        //    var context = new OwinContext(owinEnvironment);

        //    // Allow all authenticated users to see the Dashboard (potentially dangerous).
        //    return (context.Authentication.User.Identity.IsAuthenticated && 
        //        (context.Authentication.User.Identity.Name == "admin" || context.Authentication.User.Identity.Name == "superadmin"));
        //}

        public bool Authorize([NotNull] DashboardContext context)
        {
            // In case you need an OWIN context, use the next line,
            // `OwinContext` class is the part of the `Microsoft.Owin` package.
            var owinContext = new OwinContext(context.GetOwinEnvironment());

            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            return (owinContext.Authentication.User.Identity.IsAuthenticated &&
                (owinContext.Authentication.User.Identity.Name == "admin" || owinContext.Authentication.User.Identity.Name == "superadmin"));
        }
    }
}