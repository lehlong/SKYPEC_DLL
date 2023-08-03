using SMO.Service.AD;

using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace SMO
{
    public static class TableauTicketUtilities
    {
        public static string GetTicket()
        {
            var serviceConfig = new SystemConfigService();
            serviceConfig.GetConfig();

            var ticket = "";
            var tableauServerUrl = serviceConfig.ObjDetail.TABLEAU_SERVER_URL;
            var tableauServerUser = serviceConfig.ObjDetail.TABLEAU_SERVER_USER;
            if (!String.IsNullOrWhiteSpace(tableauServerUrl) && !String.IsNullOrWhiteSpace(tableauServerUser))
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        var values = new NameValueCollection
                        {
                            ["username"] = tableauServerUser
                        };
                        var response = client.UploadValues(tableauServerUrl, values);
                        var responseString = Encoding.Default.GetString(response);
                        //-1 gets returned if user being used does not have enough permissions
                        if (!String.IsNullOrWhiteSpace(responseString) && responseString != "-1")
                        {
                            ticket = responseString;
                        }
                    }
                }
                catch
                {
                    //Log Exception
                }
            }
            return ticket;
        }
    }
}