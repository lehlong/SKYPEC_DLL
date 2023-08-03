using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_AD_USER_APPROVE_Map : BaseMapping<T_AD_USER_APPROVE>
    {
        public T_AD_USER_APPROVE_Map()
        {
            Table("T_AD_USER_APPROVE");
            Id(x => x.PKID);
            Map(x => x.USER_NAME);
            Map(x => x.USER_APPROVE);
            Map(x => x.MODUL);
        }
    }
}
