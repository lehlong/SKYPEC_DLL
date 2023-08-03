using SMO.Core.Entities.WF;

namespace SMO.Repository.Mapping.WF
{
    public class T_WF_ACTIVITY_COM_Map : BaseMapping<T_WF_ACTIVITY_COM>
    {
        public T_WF_ACTIVITY_COM_Map()
        {
            Id(x => x.PKID);

            Map(x => x.SUBJECT);
            Map(x => x.TYPE_NOTIFY).Not.Nullable();
            Map(x => x.CONTENTS);
            Map(x => x.ACTIVE).Not.Nullable();
            Map(x => x.ACTIVITY_CODE).Not.Nullable();

            References(x => x.Activity, "ACTIVITY_CODE")
                .Not.Insert()
                .Not.Update();
        }
    }
}
