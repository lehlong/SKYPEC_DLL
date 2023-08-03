using NHibernate.Type;

using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_PERIOD_TIME_Map : BaseMapping<T_MD_PERIOD_TIME>
    {
        public T_MD_PERIOD_TIME_Map()
        {
            Table("T_MD_PERIOD_TIME");
            Id(x => x.TIME_YEAR).GeneratedBy.Assigned();
            Map(x => x.IS_CLOSE).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.IS_DEFAULT).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.IS_EDIT).Not.Nullable().CustomType<YesNoType>();
        }
    }
}
