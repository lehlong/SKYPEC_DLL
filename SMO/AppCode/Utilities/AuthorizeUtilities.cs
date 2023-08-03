using System.Collections.Generic;
using System.Linq;

namespace SMO
{
    public static class AuthorizeUtilities
    {
        internal static List<string> IGNORE_USERS { get; set; }

        /// <summary>
        /// Kiểm tra xem user current có nắm trong list user full quyền hay không
        /// </summary>
        /// <returns></returns>
        public static bool CheckIgnoreUser(string userName)
        {
            if (IGNORE_USERS != null)
            {
                return IGNORE_USERS.Contains(userName);
            }
            return false;
        }

        public static bool CheckUserRight(string right)
        {
            if (ProfileUtilities.User.IS_IGNORE_USER)
            {
                return true;
            }
            if (ProfileUtilities.UserRight.Select(x => x.CODE).Contains(right))
            {
                return true;
            }
            return false;
        }
    }
}