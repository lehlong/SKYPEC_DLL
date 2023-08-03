using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    class T_MD_TEMPLATE_DETAIL_CONTRUCT_COST_CF_Map : BaseMapping<T_MD_TEMPLATE_DETAIL_CONTRUCT_COST_CF>
    {
        public T_MD_TEMPLATE_DETAIL_CONTRUCT_COST_CF_Map()
        {
            Id(x => x.PKID);
            Map(x => x.CENTER_CODE, "INTERNAL_ORDER_CODE");
            Map(x => x.ELEMENT_CODE, "COST_CF_ELEMENT_CODE");
            Map(x => x.TEMPLATE_CODE);
            Map(x => x.TIME_YEAR);

            References(x => x.Center, "INTERNAL_ORDER_CODE")
                .Not.Insert().Not.Update();
            References(x => x.Element).Columns("COST_CF_ELEMENT_CODE", "TIME_YEAR")
                .Not.Insert().Not.Update();
            References(x => x.Template, "TEMPLATE_CODE")
                .Not.Insert().Not.Update();
        }
    }
}
