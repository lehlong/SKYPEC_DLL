using NHibernate.Type;

using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_PROJECT_Map : BaseMapping<T_MD_PROJECT>
    {
        public T_MD_PROJECT_Map()
        {
            Table("T_MD_PROJECT");
            Id(x => x.CODE);
            Map(x => x.NAME).Nullable();
            Map(x => x.ACTIVE).Not.Nullable().CustomType<YesNoType>();
        }
    }
}
