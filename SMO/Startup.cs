using Hangfire;
using Hangfire.SqlServer;

using Microsoft.Owin;

using Owin;

using SMO.HangfireJobs;

[assembly: OwinStartupAttribute(typeof(SMO.Startup))]
namespace SMO
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();

            GlobalConfiguration.Configuration.UseSqlServerStorage("HangfireDB");

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new MyRestrictiveAuthorizationFilter() }
            });
            app.UseHangfireServer();

            InitBackgroundJob();
        }

        protected void InitBackgroundJob()
        {
            // cron expression generation https://freeformatter.com/cron-expression-generator-quartz.html
            // cron expression every minute
            // RecurringJob.AddOrUpdate("SendEmail", () => SMOUtilities.SendEmail(), "0 * * ? * *");
            // Every day at midnight -12am
            RecurringJob.AddOrUpdate("AutoChangePeriod",
                () => ChangeBudgetPeriodJob.AutoChangePeriod(), "0 17 1-31 1-12 0-6");
        }
    }
}
