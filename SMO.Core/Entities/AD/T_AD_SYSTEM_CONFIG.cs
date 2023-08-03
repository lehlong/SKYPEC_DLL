using System;

namespace SMO.Core.Entities
{
    public partial class T_AD_SYSTEM_CONFIG : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string SAP_HOST { get; set; }
        public virtual string SAP_CLIENT { get; set; }
        public virtual string SAP_NUMBER { get; set; }
        public virtual string SAP_USER_NAME { get; set; }
        public virtual string SAP_PASSWORD { get; set; }
        public virtual int SAP_TIME_DIFF { get; set; }
        public virtual string CURRENT_CONNECTION { get; set; }
        public virtual string CURRENT_DATABASE_NAME { get; set; }
        public virtual string DIRECTORY_CACHE { get; set; }
        public virtual string MAIL_HOST { get; set; }
        public virtual int MAIL_PORT { get; set; }
        public virtual string MAIL_USER { get; set; }
        public virtual string MAIL_PASSWORD { get; set; }
        public virtual bool MAIL_IS_SSL { get; set; }
        public virtual string AD_CONNECTION { get; set; }

        public virtual string TABLEAU_SERVER_PROTOCOL { get; set; }
        public virtual string TABLEAU_SERVER_URL { get; set; }
        public virtual string TABLEAU_SERVER_URL_LOCALHOST { get; set; }
        public virtual string TABLEAU_SERVER_USER { get; set; }
        public virtual string TABLEAU_SERVER_PASSWORD { get; set; }
        public virtual string TABLEAU_SERVER_API_VERSION { get; set; }



        public virtual DateTime? LAST_UPDATE_PR { get; set; }
        public virtual T_AD_CONNECTION Connection { get; set; }
    }
}
