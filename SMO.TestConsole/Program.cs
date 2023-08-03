//using SMO.SAPINT;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using SAP.Middleware.Connector;
//using SharpSapRfc;
//using SharpSapRfc.Plain;
//using SMO.SAPINT.Function;
//using SMO.Repository.Common;
//using NHibernate.Linq;
//using SMO.Core.Entities;
//using SMO;
//using System.DirectoryServices.AccountManagement;
//using System.DirectoryServices.Protocols;
//using System.Net;
using System.DirectoryServices;
using SMO.Service.MD;
using SMO.Service.AD;
using System;

namespace SMO.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //var serviceSystem = new SystemConfigService();
            //serviceSystem.GetConfig();
            //SAPDestitination.Init(serviceSystem.ObjDetail.SAP_HOST, serviceSystem.ObjDetail.SAP_CLIENT, serviceSystem.ObjDetail.SAP_NUMBER);

            var service = new ConfigTableauService();
            service.ValidateTableau();
        }

        public static bool IsAuthenticated(string ldap, string usr, string pwd)
        {
            bool authenticated = false;

            try
            {
                DirectoryEntry entry = new DirectoryEntry(ldap, usr, pwd);
                object nativeObject = entry.NativeObject;
                authenticated = true;
            }
            catch (DirectoryServicesCOMException cex)
            {
                Console.WriteLine(cex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return authenticated;
        }
    }
}
