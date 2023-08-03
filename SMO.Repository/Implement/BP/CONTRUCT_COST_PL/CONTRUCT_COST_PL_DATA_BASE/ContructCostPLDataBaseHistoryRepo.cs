using SMO.Core.Entities.BP.CONTRUCT_COST_PL.CONTRUCT_COST_PL_DATA_BASE;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.CONTRUCT_COST_PL.CONTRUCT_COST_PL_DATA_BASE;

namespace SMO.Repository.Implement.BP.CONTRUCT_COST_PL.CONTRUCT_COST_PL_DATA_BASE
{
    public class ContructCostPLDataBaseHistoryRepo : GenericRepository<T_BP_CONTRUCT_COST_PL_DATA_BASE_HISTORY>, IContructCostPLDataBaseHistoryRepo
    {
        public ContructCostPLDataBaseHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
