﻿using SMO.Cache;

namespace SMO
{
    public static class MessageUtilities
    {
        public static void AddToCache(MessageObject obj)
        {
            var strKey = Global.ApplicationName + "-Message-" + obj.Code + "-" + obj.Language;
            CachingProvider.AddItem(strKey, obj);
        }

        public static string GetMessage(string code)
        {
            var lang = Global.LanguageDefault;
            if (ProfileUtilities.User != null)
            {
                //lang = ProfileUtilities.User.l
            }
            var strKey = Global.ApplicationName + "-Message-" + code + "-" + lang;
            return CachingProvider.GetItem(strKey) is MessageObject obj ? obj.Message : code;
        }
    }
}