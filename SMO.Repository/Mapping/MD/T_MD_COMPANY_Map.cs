using NHibernate.Type;

using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_COMPANY_Map : BaseMapping<T_MD_COMPANY>
    {
        public T_MD_COMPANY_Map()
        {
            Table("T_MD_COMPANY");
            Id(x => x.CODE);
            Map(x => x.NAME).Nullable();
            Map(x => x.ACTIVE).Not.Nullable().CustomType<YesNoType>();
        }
    }
}
