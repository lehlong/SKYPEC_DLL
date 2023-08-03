using SMO.Core.Entities.BP.CONTRUCT_COST_CF.CONTRUCT_COST_CF_DATA_BASE;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.CONTRUCT_COST_CF.CONTRUCT_COST_CF_DATA_BASE;

namespace SMO.Repository.Implement.BP.CONTRUCT_COST_CF.CONTRUCT_COST_CF_DATA_BASE
{
    public class ContructCostCFDataBaseHistoryRepo : GenericRepository<T_BP_CONTRUCT_COST_CF_DATA_BASE_HISTORY>, IContructCostCFDataBaseHistoryRepo
    {
        public ContructCostCFDataBaseHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
