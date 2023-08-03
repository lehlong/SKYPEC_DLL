using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class ContructCostPLDataHistoryRepo : GenericRepository<T_BP_CONTRUCT_COST_PL_DATA_HISTORY>, IContructCostPLDataHistoryRepo
    {
        public ContructCostPLDataHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
