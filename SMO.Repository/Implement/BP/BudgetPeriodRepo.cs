
using SMO.Core.Entities.BP;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

using System.Collections.Generic;

namespace SMO.Repository.Implement.BP
{
    public class BudgetPeriodRepo : GenericRepository<T_BP_BUDGET_PERIOD>, IBudgetPeriodRepo
    {
        public BudgetPeriodRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
            // https://www.cssscript.com/demo/responsive-step-progress-indicator-with-pure-css/#
        }

        public override IList<T_BP_BUDGET_PERIOD> Search(T_BP_BUDGET_PERIOD objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = NHibernateSession.QueryOver<T_BP_BUDGET_PERIOD>();

            if (objFilter.TIME_YEAR != 0)
            {
                query = query.Where(x => x.TIME_YEAR == objFilter.TIME_YEAR);
            }

            query = query.Fetch(x => x.Period).Eager;
            return base.Paging(query, pageSize, pageIndex, out total);
        }
    }
}
