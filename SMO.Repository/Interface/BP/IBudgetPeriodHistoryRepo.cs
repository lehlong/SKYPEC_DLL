using SMO.Core.Entities.BP;
using SMO.Repository.Common;

using System.Collections.Generic;

namespace SMO.Repository.Interface.BP
{
    public interface IBudgetPeriodHistoryRepo : IGenericRepository<T_BP_BUDGET_PERIOD_HISTORY>
    {
        IList<T_BP_BUDGET_PERIOD_HISTORY> GetAllHistory(string budgetPeriodId);
    }
}
