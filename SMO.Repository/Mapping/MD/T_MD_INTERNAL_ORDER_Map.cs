using NHibernate.Type;

using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    class T_MD_INTERNAL_ORDER_Map : BaseMapping<T_MD_INTERNAL_ORDER>
    {
        public T_MD_INTERNAL_ORDER_Map()
        {
            Id(x => x.CODE);
            Map(x => x.NAME);
            Map(x => x.ACTIVE).CustomType<YesNoType>();
            Map(x => x.PROJECT_CODE);
            Map(x => x.PROJECT_NAME);
            Map(x => x.BLOCK_CODE);
            Map(x => x.BLOCK_NAME);
            Map(x => x.IO_LEVEL1_CODE);
            Map(x => x.IO_LEVEL1_NAME);
            Map(x => x.IO_LEVEL2_CODE);
            Map(x => x.IO_LEVEL2_NAME);
        }
    }
}
