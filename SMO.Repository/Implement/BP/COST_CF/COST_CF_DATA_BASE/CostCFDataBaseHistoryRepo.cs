using SMO.Core.Entities.BP.COST_CF.COST_CF_DATA_BASE;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.COST_CF.COST_CF_DATA_BASE;

namespace SMO.Repository.Implement.BP.COST_CF.COST_CF_DATA_BASE
{
    public class CostCFDataBaseHistoryRepo : GenericRepository<T_BP_COST_CF_DATA_BASE_HISTORY>, ICostCFDataBaseHistoryRepo
    {
        public CostCFDataBaseHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
