
using SMO.Core.Entities.BP;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

using System.Collections.Generic;

namespace SMO.Repository.Implement.BP
{
    public class BudgetPeriodHistoryRepo : GenericRepository<T_BP_BUDGET_PERIOD_HISTORY>, IBudgetPeriodHistoryRepo
    {
        public BudgetPeriodHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }

        public IList<T_BP_BUDGET_PERIOD_HISTORY> GetAllHistory(string budgetPeriodId)
        {
            var query = NHibernateSession.QueryOver<T_BP_BUDGET_PERIOD_HISTORY>();

            query = query.Where(x => x.BUDGET_PERIOD_ID == budgetPeriodId);
            query = query.OrderBy(x => x.ACTION_DATE).Desc;
            return query.List();

        }
    }
}
