using SMO.Core.Entities.BP;

namespace SMO.Repository.Mapping.BP
{
    public class T_BP_PERIOD_Mapping : BaseMapping<T_BP_PERIOD>
    {
        public T_BP_PERIOD_Mapping()
        {
            Id(x => x.ID);

            Map(x => x.NAME);
            Map(x => x.NOTES);
            Map(x => x.NEXT_PERIOD_ID);
            Map(x => x.ORDER, "[ORDER]");
        }
    }
}
