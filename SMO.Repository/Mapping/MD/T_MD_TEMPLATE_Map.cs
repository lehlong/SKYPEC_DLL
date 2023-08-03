using NHibernate.Type;

using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_TEMPLATE_Map : BaseMapping<T_MD_TEMPLATE>
    {
        public T_MD_TEMPLATE_Map()
        {
            Id(x => x.CODE);
            Map(x => x.ACTIVE).CustomType<YesNoType>();
            Map(x => x.BUDGET_TYPE)
                .Not.Update();
            Map(x => x.OBJECT_TYPE)
                .Not.Update();
            Map(x => x.ELEMENT_TYPE)
                .Not.Update();
            Map(x => x.NAME);
            Map(x => x.NOTES);
            Map(x => x.TITLE);
            Map(x => x.ORG_CODE)
                .Not.Update();
            Map(x => x.IS_BASE).CustomType<YesNoType>();

            References(x => x.Organize, "ORG_CODE")
                .Not.Insert()
                .Not.Update()
                .LazyLoad();

            HasMany(x => x.DetailCosts)
                .KeyColumn("TEMPLATE_CODE")
                .LazyLoad().Inverse().Cascade.Delete();
            HasMany(x => x.DetailCostsCF)
                .KeyColumn("TEMPLATE_CODE")
                .LazyLoad().Inverse().Cascade.Delete();
            HasMany(x => x.DetailRevenuesCF)
                .KeyColumn("TEMPLATE_CODE")
                .LazyLoad().Inverse().Cascade.Delete();
            HasMany(x => x.DetailRevenues)
                .KeyColumn("TEMPLATE_CODE")
                .LazyLoad().Inverse().Cascade.Delete();
            HasMany(x => x.DetailContructCostPL)
                .KeyColumn("TEMPLATE_CODE")
                .LazyLoad().Inverse().Cascade.Delete();
            HasMany(x => x.DetailContructCostCF)
                .KeyColumn("TEMPLATE_CODE")
                .LazyLoad().Inverse().Cascade.Delete();
            HasMany(x => x.DetailOtherCostPL)
                .KeyColumn("TEMPLATE_CODE")
                .LazyLoad().Inverse().Cascade.Delete();
            HasMany(x => x.DetailOtherCostCF)
                        .KeyColumn("TEMPLATE_CODE")
                        .LazyLoad().Inverse().Cascade.Delete();

        }
    }
}
