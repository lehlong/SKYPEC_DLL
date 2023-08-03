using NHibernate.Type;

using SMO.Core.Entities.BP;

namespace SMO.Repository.Mapping
{
    public class BaseBPReviewMapping<T, TResult> : BaseMapping<T> where T : BPBaseReviewEntity<TResult>
    {
        public BaseBPReviewMapping()
        {
            Id(x => x.PKID);
            Map(x => x.ORG_CODE);
            Map(x => x.REVIEW_USER);
            Map(x => x.IS_END).CustomType<YesNoType>();
            Map(x => x.IS_SUMMARY).CustomType<YesNoType>();
            Map(x => x.TIME_YEAR);
            Map(x => x.DATA_VERSION);

            References(x => x.Organize, "ORG_CODE")
                .Not.Insert()
                .Not.Update();
            References(x => x.UserReview, "REVIEW_USER")
                .Not.Insert()
                .Not.Update();
            HasMany(x => x.Results)
                .KeyColumn("HEADER_ID")
                .Inverse().Cascade.All();
        }
    }
}
