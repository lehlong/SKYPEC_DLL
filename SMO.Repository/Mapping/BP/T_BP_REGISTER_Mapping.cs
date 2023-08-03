using SMO.Core.Entities.BP;

namespace SMO.Repository.Mapping.BP
{
    public class T_BP_REGISTER_Mapping : BaseMapping<T_BP_REGISTER>
    {
        public T_BP_REGISTER_Mapping()
        {
            Id(x => x.ID);
            Map(x => x.IS_REGISTER);
            Map(x => x.ORG_CODE).Not.Update();
            Map(x => x.TIME_YEAR).Not.Update();
            Map(x => x.DESCRIPTION);
            Map(x => x.TYPE_ID).Not.Update();

            References(x => x.Organize, "ORG_CODE")
                .Not.Insert()
                .Not.Update();
            References(x => x.BpType, "TYPE_ID")
                .Not.Insert()
                .Not.Update();

        }
    }
}
