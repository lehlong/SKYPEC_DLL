using SMO.Core.Entities.BP.OTHER_COST_PL.OTHER_COST_PL_DATA_BASE;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.OTHER_COST_PL.OTHER_COST_PL_DATA_BASE;

namespace SMO.Repository.Implement.BP.OTHER_COST_PL.OTHER_COST_PL_DATA_BASE
{
    public class OtherCostPLDataBaseHistoryRepo : GenericRepository<T_BP_OTHER_COST_PL_DATA_BASE_HISTORY>, IOtherCostPLDataBaseHistoryRepo
    {
        public OtherCostPLDataBaseHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
