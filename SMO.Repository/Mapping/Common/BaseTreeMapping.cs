using NHibernate.Type;

using SMO.Core.Common;

namespace SMO.Repository.Mapping
{
    class BaseTreeMapping<T> : BaseMapping<T> where T : CoreTree
    {
        public BaseTreeMapping()
        {
            Map(x => x.C_ORDER);
            Id(x => x.CODE);
            Map(x => x.PARENT_CODE);
            Map(x => x.NAME);
            Map(x => x.ACTIVE).CustomType<YesNoType>();
        }
    }
}
