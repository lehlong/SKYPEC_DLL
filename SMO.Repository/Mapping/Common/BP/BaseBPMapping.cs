using NHibernate.Type;

using SMO.Core.Entities.BP;

namespace SMO.Repository.Mapping
{
    public class BaseBPMapping<T> : BaseMapping<T> where T : T_BP_BASE
    {
        public BaseBPMapping()
        {
            Id(x => x.PKID);
            Map(x => x.ORG_CODE);
            Map(x => x.TEMPLATE_CODE);
            Map(x => x.VERSION);
            Map(x => x.TIME_YEAR);
            Map(x => x.STATUS);
            Map(x => x.PHASE);
            Map(x => x.FILE_ID);
            Map(x => x.IS_SUMUP).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.IS_DELETED).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.IS_REVIEWED).Not.Nullable().CustomType<YesNoType>();

            References(x => x.FileUpload).Column("FILE_ID").Not.Insert().Not.Update().LazyLoad();
            References(x => x.Template, "TEMPLATE_CODE").Not.Insert().Not.Update().LazyLoad();
            References(x => x.Organize).Column("ORG_CODE").Not.Insert().Not.Update();
        }
    }
}
