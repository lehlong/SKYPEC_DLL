using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    class T_MD_USER_REVIEW_Map : BaseMapping<T_MD_USER_REVIEW>
    {
        public T_MD_USER_REVIEW_Map()
        {
            Id(x => x.PKID);
            Map(x => x.USER_NAME).Not.Nullable();
            Map(x => x.TIME_YEAR).Not.Nullable();

            References(x => x.User, "USER_NAME")
                .Not.Insert()
                .Not.Update()
                .LazyLoad();
        }
    }
}
