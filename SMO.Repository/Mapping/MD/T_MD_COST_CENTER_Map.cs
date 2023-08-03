using NHibernate.Type;

using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    class T_MD_COST_CENTER_Map : BaseCoreCenterMapping<T_MD_COST_CENTER>
    {
        public T_MD_COST_CENTER_Map() : base()
        {
            Map(x => x.SAP_CODE);
            Map(x => x.IS_GROUP).Not.Nullable().CustomType<YesNoType>(); ;
            HasMany(x => x.ListUser).KeyColumn("ORGANIZE_CODE").Inverse().Cascade.None();
            References(x => x.Parent, "PARENT_CODE")
                .Not.Insert()
                .Not.Update()
                .LazyLoad();
        }
    }
}
