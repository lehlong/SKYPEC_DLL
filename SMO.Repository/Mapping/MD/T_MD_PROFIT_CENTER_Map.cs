using NHibernate.Type;

using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    class T_MD_PROFIT_CENTER_Map : BaseMapping<T_MD_PROFIT_CENTER>
    {
        public T_MD_PROFIT_CENTER_Map()
        {
            Id(x => x.CODE);
            Map(x => x.NAME);
            Map(x => x.ACTIVE).CustomType<YesNoType>();
            Map(x => x.COMPANY_CODE);
            Map(x => x.PROJECT_CODE);

            References(x => x.Company, "COMPANY_CODE")
                .Not.Insert()
                .Not.Update();
            References(x => x.Project, "PROJECT_CODE")
                .Not.Insert()
                .Not.Update();

        }
    }
}
