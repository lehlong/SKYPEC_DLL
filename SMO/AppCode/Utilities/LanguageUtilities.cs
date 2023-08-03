using SMO.Cache;

namespace SMO
{
    public static class LanguageUtilities
    {
        public static void AddToCache(LanguageObject obj)
        {
            var strKey = Global.ApplicationName + "-Language-" + obj.Code + "-" + obj.Language;
            CachingProvider.AddItem(strKey, obj);
        }

        public static string GetLanguage(string code)
        {
            var lang = Global.LanguageDefault;
            if (ProfileUtilities.User != null)
            {
                lang = ProfileUtilities.User.LANGUAGE;
            }
            var strKey = Global.ApplicationName + "-Language-" + code + "-" + lang;
            LanguageObject obj = CachingProvider.GetItem(strKey) as LanguageObject;
            if (obj == null)
            {
                if (lang != Global.LanguageDefault)
                {
                    var strKey_VI = Global.ApplicationName + "-Language-" + code + "-" + Global.LanguageDefault;
                    obj = CachingProvider.GetItem(strKey_VI) as LanguageObject;
                }
            }
            return obj != null ? obj.Value : code;
        }

        /// <summary>
        /// Lấy ngôn ngữ của control
        /// </summary>
        /// <param name="code"></param>
        /// <param name="form"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static string LangControl(string code, string form)
        {
            string strResult = GetLanguage("C" + "-" + form + "-" + code);
            return strResult;
        }

        /// <summary>
        /// Lấy ngôn ngữ của label, title, tooltip... trong một form 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="form"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static string LangText(string code, string form)
        {
            string strResult = GetLanguage("T" + "-" + form + "-" + code);
            return strResult;
        }
    }
}