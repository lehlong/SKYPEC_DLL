using NHibernate.Type;

using SMO.Core.Entities.BP.REVENUE_CF;

namespace SMO.Repository.Mapping.BP.REVENUE_CF
{
    class T_BP_REVENUE_CF_REVIEW_RESULT_Map : BaseMapping<T_BP_REVENUE_CF_REVIEW_RESULT>
    {
        public T_BP_REVENUE_CF_REVIEW_RESULT_Map()
        {
            Id(x => x.PKID);
            Map(x => x.REVENUE_CF_ELEMENT_CODE);
            Map(x => x.HEADER_ID);
            Map(x => x.RESULT).CustomType<YesNoType>();

            References(x => x.Header, "HEADER_ID")
                .Not.Insert()
                .Not.Update();
            References(x => x.Element).Columns("REVENUE_CF_ELEMENT_CODE", "TIME_YEAR")
                .Not.Insert()
                .Not.Update();
        }
    }
}
