using SMO.Core.Entities.WF;

namespace SMO.Repository.Mapping.MD
{
    public class T_WF_PROCESS_Map : BaseMapping<T_WF_PROCESS>
    {
        public T_WF_PROCESS_Map()
        {
            Id(x => x.CODE);

            Map(x => x.NAME).Not.Nullable();
            Map(x => x.ACTIVE).Not.Nullable();

            HasMany(x => x.Activities)
                .KeyColumn("PROCESS_CODE")
                .Inverse().Cascade.All();
        }
    }
}
