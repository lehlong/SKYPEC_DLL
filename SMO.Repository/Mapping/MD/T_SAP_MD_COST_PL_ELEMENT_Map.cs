using NHibernate.Type;

using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_SAP_MD_COST_PL_ELEMENT_Map : BaseMapping<T_SAP_MD_COST_PL_ELEMENT>
    {
        public T_SAP_MD_COST_PL_ELEMENT_Map()
        {
            Table("T_SAP_MD_COST_PL_ELEMENT");
            Id(x => x.CODE);
            Map(x => x.NAME);
            Map(x => x.PARENT_CODE);
            Map(x => x.C_ORDER);
            Map(x => x.ACTIVE).Not.Nullable().CustomType<YesNoType>();
        }
    }
}
