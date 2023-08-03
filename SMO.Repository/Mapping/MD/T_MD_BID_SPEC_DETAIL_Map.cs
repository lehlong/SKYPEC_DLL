using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_BID_SPEC_DETAIL_Map : BaseMapping<T_MD_BID_SPEC_DETAIL>
    {
        public T_MD_BID_SPEC_DETAIL_Map()
        {
            Table("T_MD_BID_SPEC_DETAIL");
            Id(x => x.PKID);
            Map(x => x.HEADER_ID);
            Map(x => x.FROM_VALUE);
            Map(x => x.TO_VALUE);
            Map(x => x.VALUE);
            Map(x => x.VALUE_EN);
            Map(x => x.SCORE);
            Map(x => x.C_ORDER);
        }
    }
}
