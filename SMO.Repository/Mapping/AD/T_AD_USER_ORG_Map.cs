
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.AD
{
    public class T_AD_USER_ORG_Map : BaseMapping<T_AD_USER_ORG>
    {
        public T_AD_USER_ORG_Map()
        {
            Table("T_AD_USER_ORG");
            CompositeId()
                .KeyProperty(x => x.ORG_CODE)
                .KeyProperty(x => x.USER_NAME);
            References(x => x.Organize).Column("ORG_CODE").Not.Insert().Not.Update().LazyLoad();
            References(x => x.User).Column("USER_NAME").Not.Insert().Not.Update().LazyLoad();
        }
    }
}
