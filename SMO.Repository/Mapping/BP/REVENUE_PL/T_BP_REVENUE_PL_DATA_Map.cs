using SMO.Core.Entities;

namespace SMO.Repository.Mapping.BP
{
    public class T_BP_REVENUE_PL_DATA_Map : BaseMapping<T_BP_REVENUE_PL_DATA>
    {
        public T_BP_REVENUE_PL_DATA_Map()
        {
            Table("T_BP_REVENUE_PL_DATA");
            Id(x => x.PKID);
            Map(x => x.ORG_CODE);
            Map(x => x.PROFIT_CENTER_CODE);
            Map(x => x.TEMPLATE_CODE);
            Map(x => x.REVENUE_PL_ELEMENT_CODE);
            Map(x => x.VERSION);
            Map(x => x.COMPANY_CODE);
            Map(x => x.PROJECT_CODE);
            Map(x => x.TIME_YEAR);
            Map(x => x.VALUE_JAN);
            Map(x => x.VALUE_FEB);
            Map(x => x.VALUE_MAR);
            Map(x => x.VALUE_APR);
            Map(x => x.VALUE_MAY);
            Map(x => x.VALUE_JUN);
            Map(x => x.VALUE_JUL);
            Map(x => x.VALUE_AUG);
            Map(x => x.VALUE_SEP);
            Map(x => x.VALUE_OCT);
            Map(x => x.VALUE_NOV);
            Map(x => x.VALUE_DEC);
            Map(x => x.DESCRIPTION);
            Map(x => x.STATUS);

            References(x => x.RevenueElement).Columns("REVENUE_PL_ELEMENT_CODE", "TIME_YEAR")
                .Not.Insert().Not.Update();
            References(x => x.ProfitCenter, "PROFIT_CENTER_CODE")
                .Not.Insert().Not.Update();
            References(x => x.Template, "TEMPLATE_CODE")
                .Not.Insert().Not.Update();
            References(x => x.Company, "COMPANY_CODE")
                .Not.Insert().Not.Update();
            References(x => x.Project, "PROJECT_CODE")
                .Not.Insert().Not.Update();
            References(x => x.Organize, "ORG_CODE")
                .Not.Insert().Not.Update();

        }
    }
}
