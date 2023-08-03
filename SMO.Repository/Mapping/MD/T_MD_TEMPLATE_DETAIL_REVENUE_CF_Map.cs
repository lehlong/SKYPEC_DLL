using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    class T_MD_TEMPLATE_DETAIL_REVENUE_CF_Map : BaseMapping<T_MD_TEMPLATE_DETAIL_REVENUE_CF>
    {
        public T_MD_TEMPLATE_DETAIL_REVENUE_CF_Map()
        {
            Id(x => x.PKID);
            Map(x => x.CENTER_CODE, "PROFIT_CENTER_CODE");
            Map(x => x.ELEMENT_CODE, "REVENUE_CF_ELEMENT_CODE");
            Map(x => x.TEMPLATE_CODE);
            Map(x => x.TIME_YEAR);

            References(x => x.Center, "PROFIT_CENTER_CODE")
                .Not.Insert().Not.Update();
            References(x => x.Element).Columns("REVENUE_CF_ELEMENT_CODE", "TIME_YEAR")
                .Not.Insert().Not.Update();
            References(x => x.Template, "TEMPLATE_CODE")
                .Not.Insert().Not.Update();
        }
    }
}
