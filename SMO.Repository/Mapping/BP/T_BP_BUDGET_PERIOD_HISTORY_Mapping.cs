using SMO.Core.Entities.BP;

namespace SMO.Repository.Mapping.BP
{
    public class T_BP_BUDGET_PERIOD_HISTORY_Mapping : BaseMapping<T_BP_BUDGET_PERIOD_HISTORY>
    {
        public T_BP_BUDGET_PERIOD_HISTORY_Mapping()
        {
            Id(x => x.ID);
            Map(x => x.ACTION_DATE).Not.Update();
            Map(x => x.ACTION_USER).Not.Update();
            Map(x => x.BUDGET_PERIOD_ID).Not.Update();
            Map(x => x.TYPE).Not.Update();

            References(x => x.BudgetPeriod, "BUDGET_PERIOD_ID")
                .Not.Insert()
                .Not.Update();
        }
    }
}
