using NHibernate.Type;

using SMO.Core.Entities;

namespace SMO.Repository.Mapping.AD
{
    public class T_AD_SYSTEM_CONFIG_Map : BaseMapping<T_AD_SYSTEM_CONFIG>
    {
        public T_AD_SYSTEM_CONFIG_Map()
        {
            Table("T_AD_SYSTEM_CONFIG");
            Id(x => x.PKID);
            Map(x => x.SAP_HOST);
            Map(x => x.SAP_CLIENT);
            Map(x => x.SAP_NUMBER);
            Map(x => x.SAP_USER_NAME);
            Map(x => x.SAP_PASSWORD);
            Map(x => x.SAP_TIME_DIFF);
            Map(x => x.CURRENT_CONNECTION);
            Map(x => x.CURRENT_DATABASE_NAME);
            Map(x => x.DIRECTORY_CACHE);
            Map(x => x.LAST_UPDATE_PR);
            Map(x => x.MAIL_HOST);
            Map(x => x.MAIL_PORT);
            Map(x => x.MAIL_USER);
            Map(x => x.MAIL_PASSWORD);
            Map(x => x.MAIL_IS_SSL).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.AD_CONNECTION);
            Map(x => x.TABLEAU_SERVER_PROTOCOL);
            Map(x => x.TABLEAU_SERVER_URL);
            Map(x => x.TABLEAU_SERVER_URL_LOCALHOST);
            Map(x => x.TABLEAU_SERVER_USER);
            Map(x => x.TABLEAU_SERVER_PASSWORD);
            Map(x => x.TABLEAU_SERVER_API_VERSION);
            References(x => x.Connection).Column("CURRENT_CONNECTION").Not.Insert().Not.Update().LazyLoad();
        }
    }
}
