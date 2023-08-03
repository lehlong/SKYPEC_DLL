using NHibernate.Type;

using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    class T_MD_REVENUE_CF_ELEMENT_Map : BaseMapping<T_MD_REVENUE_CF_ELEMENT>
    {
        public T_MD_REVENUE_CF_ELEMENT_Map()
        {
            CompositeId()
                .KeyProperty(x => x.CODE)
                .KeyProperty(x => x.TIME_YEAR);
            Map(x => x.C_ORDER);
            Map(x => x.PARENT_CODE);
            Map(x => x.NAME);
            Map(x => x.ACTIVE).CustomType<YesNoType>();
            Map(x => x.IS_GROUP).CustomType<YesNoType>();
        }
    }
}
