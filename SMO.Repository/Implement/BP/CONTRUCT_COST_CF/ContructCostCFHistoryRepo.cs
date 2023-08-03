using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class ContructCostCFHistoryRepo : GenericRepository<T_BP_CONTRUCT_COST_CF_HISTORY>, IContructCostCFHistoryRepo
    {
        public ContructCostCFHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
