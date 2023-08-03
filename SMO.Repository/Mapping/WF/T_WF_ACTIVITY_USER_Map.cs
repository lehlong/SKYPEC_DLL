using SMO.Core.Entities.WF;

namespace SMO.Repository.Mapping.WF
{
    public class T_WF_ACTIVITY_USER_Map : BaseMapping<T_WF_ACTIVITY_USER>
    {
        public T_WF_ACTIVITY_USER_Map()
        {
            Id(x => x.PKID);
            Map(x => x.ORG_CODE);
            Map(x => x.ACTIVITY_CODE);
            Map(x => x.USER_SENDER);
            Map(x => x.USER_RECEIVER);

            References(x => x.Activity, "ACTIVITY_CODE")
                .Not.Insert()
                .Not.Update();

            References(x => x.Organize, "ORG_CODE")
                .Not.Insert()
                .Not.Update();

            References(x => x.UserReceiver, "USER_RECEIVER")
                .Not.Insert()
                .Not.Update();

            References(x => x.UserSender, "USER_SENDER")
                .Not.Insert()
                .Not.Update();
        }
    }
}
