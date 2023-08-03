using SMO.Core.Entities.BP;

namespace SMO.Repository.Mapping
{
    public class BaseBPVersionMapping<T> : BaseMapping<T> where T : BaseBPVersionEntity
    {
        public BaseBPVersionMapping()
        {
            Id(x => x.PKID);
            Map(x => x.ORG_CODE);
            Map(x => x.TEMPLATE_CODE);
            Map(x => x.VERSION);
            Map(x => x.TIME_YEAR);
            Map(x => x.FILE_ID);
            Map(x => x.IS_DELETED).Not.Nullable().Default("0");

            References(x => x.Template, "TEMPLATE_CODE").Not.Insert().Not.Update().LazyLoad();
            References(x => x.Organize).Column("ORG_CODE").Not.Insert().Not.Update();
            References(x => x.FileUpload, "FILE_ID").Not.Insert().Not.Update().LazyLoad();
        }
    }
}
