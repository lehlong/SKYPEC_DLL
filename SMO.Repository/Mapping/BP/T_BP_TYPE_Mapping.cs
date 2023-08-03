using SMO.Core.Entities.BP;

namespace SMO.Repository.Mapping.BP
{
    public class T_BP_TYPE_Mapping : BaseMapping<T_BP_TYPE>
    {
        public T_BP_TYPE_Mapping()
        {
            Id(x => x.ID);
            Map(x => x.ACTIVE);
            Map(x => x.NAME)
                .Not.Nullable();
            Map(x => x.ACRONYM_NAME)
                .Not.Nullable();
            Map(x => x.BUDGET_TYPE)
                .Not.Update()
                .Not.Nullable();
            Map(x => x.OBJECT_TYPE)
                .Not.Update()
                .Not.Nullable();
            Map(x => x.ELEMENT_TYPE)
                .Not.Update()
                .Not.Nullable();
        }
    }
}
