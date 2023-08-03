using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class CostCFHistoryRepo : GenericRepository<T_BP_COST_CF_HISTORY>, ICostCFHistoryRepo
    {
        public CostCFHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
