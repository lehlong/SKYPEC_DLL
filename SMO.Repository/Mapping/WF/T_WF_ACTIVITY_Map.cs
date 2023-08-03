using SMO.Core.Entities.WF;

namespace SMO.Repository.Mapping.MD
{
    public class T_WF_ACTIVITY_Map : BaseMapping<T_WF_ACTIVITY>
    {
        public T_WF_ACTIVITY_Map()
        {
            Id(x => x.CODE);
            Map(x => x.PROCESS_CODE);
            Map(x => x.NAME);
            Map(x => x.C_ORDER);
            Map(x => x.ACTIVE);
            HasMany(x => x.ActivityComs)
                .KeyColumn("ACTIVITY_CODE")
                .Inverse().Cascade.All();
            HasMany(x => x.ActivityUsers)
                .KeyColumn("ACTIVITY_CODE")
                .Inverse().Cascade.All();
            References(x => x.Process, "PROCESS_CODE")
                .Not.Insert()
                .Not.Update();
        }
    }
}
