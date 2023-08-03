using SMO.Core.Entities.CM;

namespace SMO.Repository.Mapping.CM
{
    public class T_CM_HEADER_BP_COMMENT_Map : BaseMapping<T_CM_HEADER_BP_COMMENT>
    {
        public T_CM_HEADER_BP_COMMENT_Map()
        {
            Id(x => x.PKID);
            Map(x => x.REFERENCE_CODE);
            Map(x => x.ORG_CODE);
            Map(x => x.VERSION);
            Map(x => x.YEAR);
            Map(x => x.NUMBER_COMMENTS);
            Map(x => x.BUDGET_TYPE);
            Map(x => x.ELEMENT_TYPE);
            Map(x => x.OBJECT_TYPE);

            References(x => x.CostCenter, "REFERENCE_CODE")
                .Not.Insert()
                .Not.Update();
            HasMany(x => x.Comments).KeyColumn("REFERENCE_ID")
                .LazyLoad().Inverse().Cascade.Delete();
        }
    }
}
