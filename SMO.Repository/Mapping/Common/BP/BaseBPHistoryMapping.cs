using SMO.Core.Entities.BP;

namespace SMO.Repository.Mapping
{
    public class BaseBPHistoryMapping<T> : BaseMapping<T> where T : BaseBPHistoryEntity
    {
        public BaseBPHistoryMapping()
        {
            Id(x => x.PKID);
            Map(x => x.ORG_CODE);
            Map(x => x.TEMPLATE_CODE);
            Map(x => x.VERSION);
            Map(x => x.TIME_YEAR);
            Map(x => x.ACTION);
            Map(x => x.ACTION_DATE);
            Map(x => x.ACTION_USER);
            Map(x => x.NOTES);

            References(x => x.Template, "TEMPLATE_CODE").Not.Insert().Not.Update().LazyLoad();
            References(x => x.ActionUser, "ACTION_USER").Not.Insert().Not.Update().LazyLoad();
            References(x => x.Organize).Column("ORG_CODE").Not.Insert().Not.Update();
        }
    }
}
