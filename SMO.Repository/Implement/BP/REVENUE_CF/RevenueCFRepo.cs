using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

using System.Collections.Generic;

namespace SMO.Repository.Implement.BP
{
    public class RevenueCFRepo : GenericBPRepository<T_BP_REVENUE_CF>, IRevenueCFRepo
    {
        public RevenueCFRepo(NHUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public override IList<T_BP_REVENUE_CF> Search(T_BP_REVENUE_CF objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = NHibernateSession.QueryOver<T_BP_REVENUE_CF>();

            if (!string.IsNullOrWhiteSpace(objFilter.ORG_CODE))
            {
                query = query.Where(x => x.ORG_CODE == objFilter.ORG_CODE);
            }
            if (objFilter.TIME_YEAR != 0)
            {
                query = query.Where(x => x.TIME_YEAR == objFilter.TIME_YEAR);
            }
            

            query = query.Fetch(x => x.Template).Eager
                .Fetch(x => x.Organize).Eager;
            query = query.OrderBy(x => x.CREATE_DATE).Desc;
            return base.Paging(query, pageSize, pageIndex, out total);
        }

    }
}
