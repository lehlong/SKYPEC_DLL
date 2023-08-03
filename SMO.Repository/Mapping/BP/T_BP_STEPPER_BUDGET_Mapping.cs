using SMO.Core.Entities.BP;

namespace SMO.Repository.Mapping.BP
{
    public class T_BP_STEPPER_BUDGET_Mapping : BaseMapping<T_BP_STEPPER_BUDGET>
    {
        public T_BP_STEPPER_BUDGET_Mapping()
        {
            Id(x => x.ID);

            Map(x => x.ORDER, "[ORDER]");
            Map(x => x.DISPLAY_TEXT);
            Map(x => x.STATUS_ID);
            Map(x => x.ACTION_ID);
            Map(x => x.ACTION_USER);
            Map(x => x.BASE_ACTION_ID);
            Map(x => x.LEVEL, "[LEVEL]");
            Map(x => x.DESCRIPTION);
        }
    }
}
