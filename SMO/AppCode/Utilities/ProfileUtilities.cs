using SMO.Core.Entities;

using System.Collections.Generic;
using System.Web;

namespace SMO
{
    public class ProfileUtilities
    {
        public static T_AD_USER User
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session["Profile"] != null)
                {
                    return HttpContext.Current.Session["Profile"] as T_AD_USER;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                HttpContext.Current.Session["Profile"] = value;
            }
        }

        public static List<T_AD_RIGHT> UserRight
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session["Profile"] != null)
                {
                    return HttpContext.Current.Session["UserRight"] as List<T_AD_RIGHT>;
                }
                else
                {
                    return new List<T_AD_RIGHT>();
                }
            }
            set
            {
                HttpContext.Current.Session["UserRight"] = value;
            }
        }

        public static List<T_AD_USER_ORG> UserOrg
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session["Profile"] != null)
                {
                    return HttpContext.Current.Session["UserOrg"] as List<T_AD_USER_ORG>;
                }
                else
                {
                    return new List<T_AD_USER_ORG>();
                }
            }
            set
            {
                HttpContext.Current.Session["UserOrg"] = value;
            }
        }
    }
}