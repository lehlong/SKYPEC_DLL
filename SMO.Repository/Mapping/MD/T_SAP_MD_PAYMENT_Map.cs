using NHibernate.Type;

using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_SAP_MD_PAYMENT_Map : BaseMapping<T_SAP_MD_PAYMENT>
    {
        public T_SAP_MD_PAYMENT_Map()
        {
            Table("T_SAP_MD_PAYMENT");
            Id(x => x.CODE);
            Map(x => x.NAME);
            Map(x => x.PARENT_CODE);
            Map(x => x.C_ORDER);
            Map(x => x.ACTIVE).Not.Nullable().CustomType<YesNoType>();
        }
    }
}
