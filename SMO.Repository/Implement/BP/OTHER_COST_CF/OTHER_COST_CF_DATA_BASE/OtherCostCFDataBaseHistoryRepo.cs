using SMO.Core.Entities.BP.OTHER_COST_CF.OTHER_COST_CF_DATA_BASE;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.OTHER_COST_CF.OTHER_COST_CF_DATA_BASE;

namespace SMO.Repository.Implement.BP.OTHER_COST_CF.OTHER_COST_CF_DATA_BASE
{
    public class OtherCostCFDataBaseHistoryRepo : GenericRepository<T_BP_OTHER_COST_CF_DATA_BASE_HISTORY>, IOtherCostCFDataBaseHistoryRepo
    {
        public OtherCostCFDataBaseHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
