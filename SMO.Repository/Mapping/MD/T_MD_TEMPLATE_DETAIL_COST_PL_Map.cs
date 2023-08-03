﻿using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    class T_MD_TEMPLATE_DETAIL_COST_PL_Map : BaseMapping<T_MD_TEMPLATE_DETAIL_COST_PL>
    {
        public T_MD_TEMPLATE_DETAIL_COST_PL_Map()
        {
            Id(x => x.PKID);
            Map(x => x.CENTER_CODE, "COST_CENTER_CODE");
            Map(x => x.ELEMENT_CODE, "COST_PL_ELEMENT_CODE");
            Map(x => x.TEMPLATE_CODE);
            Map(x => x.TIME_YEAR);

            References(x => x.Center, "COST_CENTER_CODE")
                .Not.Insert().Not.Update();
            References(x => x.Element).Columns("COST_PL_ELEMENT_CODE", "TIME_YEAR")
                .Not.Insert().Not.Update();
            References(x => x.Template, "TEMPLATE_CODE")
                .Not.Insert().Not.Update();
        }
    }
}