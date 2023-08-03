using SMO.Core.Entities.BP;

namespace SMO.Repository.Mapping.BP
{
    public class T_BP_BUDGET_PERIOD_Mapping : BaseMapping<T_BP_BUDGET_PERIOD>
    {
        public T_BP_BUDGET_PERIOD_Mapping()
        {
            Id(x => x.ID);
            Map(x => x.STATUS);
            Map(x => x.PERIOD_ID);
            Map(x => x.TIME_NEXT_PERIOD);
            Map(x => x.NOTIFY_USER);
            Map(x => x.TIME_YEAR).Not.Update();
            Map(x => x.AUTO_NEXT_PERIOD);

            References(x => x.Period, "PERIOD_ID")
                .Not.Insert()
                .Not.Update();

            HasMany(x => x.History).KeyColumn("BUDGET_PERIOD_ID")
                .LazyLoad()
                .Inverse()
                .Cascade.Delete();

        }
    }
}
