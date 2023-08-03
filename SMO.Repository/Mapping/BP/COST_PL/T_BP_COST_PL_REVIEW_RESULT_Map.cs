using NHibernate.Type;

using SMO.Core.Entities.BP.COST_PL;

namespace SMO.Repository.Mapping.BP.COST_PL
{
    class T_BP_COST_PL_REVIEW_RESULT_Map : BaseMapping<T_BP_COST_PL_REVIEW_RESULT>
    {
        public T_BP_COST_PL_REVIEW_RESULT_Map()
        {
            Id(x => x.PKID);
            Map(x => x.COST_PL_ELEMENT_CODE);
            Map(x => x.HEADER_ID);
            Map(x => x.TIME_YEAR);
            Map(x => x.RESULT).CustomType<YesNoType>();

            References(x => x.Header, "HEADER_ID")
                .Not.Insert()
                .Not.Update();
            References(x => x.Element).Columns("COST_PL_ELEMENT_CODE", "TIME_YEAR")
                .Not.Insert()
                .Not.Update();
        }
    }
}
