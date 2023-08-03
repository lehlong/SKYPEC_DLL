using SMO.Core.Entities.BP.COST_PL;

namespace SMO.Repository.Mapping.BP.COST_PL
{
    class T_BP_COST_PL_REVIEW_COMMENT_Map : BaseMapping<T_BP_COST_PL_REVIEW_COMMENT>
    {
        public T_BP_COST_PL_REVIEW_COMMENT_Map()
        {
            Id(x => x.PKID);
            Map(x => x.COST_PL_ELEMENT_CODE);
            Map(x => x.DATA_VERSION);
            Map(x => x.ORG_CODE);
            Map(x => x.TIME_YEAR);
            Map(x => x.ON_ORG_CODE);
            Map(x => x.NUMBER_COMMENTS);

            References(x => x.Organize, "ORG_CODE")
                .Not.Insert()
                .Not.Update();
            References(x => x.OnCostCenter, "ON_ORG_CODE")
                .Not.Insert()
                .Not.Update();
            References(x => x.Element).Columns("COST_PL_ELEMENT_CODE", "TIME_YEAR")
                .Not.Insert()
                .Not.Update();
            HasMany(x => x.Comments).KeyColumn("REFRENCE_ID")
                .LazyLoad().Inverse().Cascade.Delete();
        }
    }
}
