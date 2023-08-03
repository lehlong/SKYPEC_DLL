using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    class T_MD_TEMPLATE_DETAIL_REVENUE_PL_Map : BaseMapping<T_MD_TEMPLATE_DETAIL_REVENUE_PL>
    {
        public T_MD_TEMPLATE_DETAIL_REVENUE_PL_Map()
        {
            Id(x => x.PKID);
            Map(x => x.CENTER_CODE, "PROFIT_CENTER_CODE");
            Map(x => x.ELEMENT_CODE, "REVENUE_PL_ELEMENT_CODE");
            Map(x => x.TEMPLATE_CODE);
            Map(x => x.TIME_YEAR);

            References(x => x.Center, "PROFIT_CENTER_CODE")
                .Not.Insert().Not.Update();
            References(x => x.Element).Columns("REVENUE_PL_ELEMENT_CODE", "TIME_YEAR")
                .Not.Insert().Not.Update();
            References(x => x.Template, "TEMPLATE_CODE")
                .Not.Insert().Not.Update();
        }
    }
}
