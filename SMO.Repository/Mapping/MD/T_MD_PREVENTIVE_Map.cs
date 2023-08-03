using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_PREVENTIVE_Map : BaseMapping<T_MD_PREVENTIVE>
    {
        public T_MD_PREVENTIVE_Map()
        {
            Id(x => x.ID);

            Map(x => x.ORG_CODE)
                .Not.Update();
            Map(x => x.TIME_YEAR)
                .Not.Update();
            Map(x => x.PERCENTAGE);
            Map(x => x.DESCRIPTION);

            References(x => x.CostCenter, "ORG_CODE")
                .Not.Insert()
                .Not.Update();
        }
    }
}
