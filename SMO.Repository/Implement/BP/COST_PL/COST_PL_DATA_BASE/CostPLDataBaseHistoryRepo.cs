using SMO.Core.Entities.BP.COST_PL.COST_PL_DATA_BASE;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.COST_PL.COST_PL_DATA_BASE;

namespace SMO.Repository.Implement.BP.COST_PL.COST_PL_DATA_BASE
{
    public class CostPLDataBaseHistoryRepo : GenericRepository<T_BP_COST_PL_DATA_BASE_HISTORY>, ICostPLDataBaseHistoryRepo
    {
        public CostPLDataBaseHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
