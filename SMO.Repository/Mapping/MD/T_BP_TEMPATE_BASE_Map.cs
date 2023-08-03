using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    class T_BP_TEMPATE_BASE_Map : BaseMapping<T_BP_TEMPLATE_BASE>
    {
        public T_BP_TEMPATE_BASE_Map()
        {
            Table("T_BP_TEMPLATE_BASE");
            Id(x => x.PKID);
            Map(x => x.TEMPLATE_CODE);
            Map(x => x.TIME_YEAR);
            Map(x => x.FILE_ID);
            References(x => x.FileUpload).Column("FILE_ID").Not.Insert().Not.Update().LazyLoad();
        }
    }
}
