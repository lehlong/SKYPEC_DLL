using NHibernate.Type;

using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_BID_SPEC_Map : BaseMapping<T_MD_BID_SPEC>
    {
        public T_MD_BID_SPEC_Map()
        {
            Table("T_MD_BID_SPEC");
            Id(x => x.PKID);
            Map(x => x.PARENT_ID);
            Map(x => x.NAME);
            Map(x => x.NAME_EN);
            Map(x => x.DESCRIPTION);
            Map(x => x.DESCRIPTION_EN);
            Map(x => x.TYPE);
            Map(x => x.C_ORDER);
            Map(x => x.IS_SPEC_PRICE).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.IS_NOT_SHOW_CONTRACTOR).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.IS_NOT_COPY).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.ROLE);
            HasMany(x => x.ListDetail).KeyColumn("HEADER_ID").LazyLoad().Inverse().Cascade.Delete();
        }
    }
}
