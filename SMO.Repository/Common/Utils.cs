using System.Collections.Generic;
using System.Dynamic;

namespace SMO.Repository.Common
{
    public static class UtilsRepo
    {
        public static bool IsPropertyExist(dynamic obj, string name)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is ExpandoObject)
                return ((IDictionary<string, object>)obj).ContainsKey(name);

            return obj.GetType().GetProperty(name) != null;
        }
    }
}
