using NHibernate.Type;

using SMO.Core.Entities;

namespace SMO.Repository.Mapping.CF
{
    public class T_CF_TABLEAU_Map : BaseMapping<T_CF_TABLEAU>
    {
        public T_CF_TABLEAU_Map()
        {
            Table("T_CF_TABLEAU");
            Id(x => x.PKID);
            Map(x => x.PARENT_ID);
            Map(x => x.NAME);
            Map(x => x.C_ORDER);
            Map(x => x.IS_GROUP).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.RIGHT_CODE);
            Map(x => x.SITE_NAME);
            Map(x => x.SITE_ID);
            Map(x => x.WORKBOOK_NAME);
            Map(x => x.WORKBOOK_CONTENT_URL);
            Map(x => x.WORKBOOK_ID);
            Map(x => x.VIEW_NAME);
            Map(x => x.VIEW_CONTENT_URL);
            Map(x => x.VIEW_ID);
            Map(x => x.PATH_IMAGE_PREVIEW);
            Map(x => x.IS_TAB).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.IS_SHOW_APP_BANNER).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.IS_TOOLBAR).Not.Nullable().CustomType<YesNoType>();
        }
    }
}
