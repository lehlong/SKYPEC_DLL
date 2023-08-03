using SMO.Core.Entities.BP.REVENUE_CF.REVENUE_CF_DATA_BASE;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.REVENUE_CF.REVENUE_CF_DATA_BASE;

namespace SMO.Repository.Implement.BP.REVENUE_CF.REVENUE_CF_DATA_BASE
{
    public class RevenueCFDataBaseHistoryRepo : GenericRepository<T_BP_REVENUE_CF_DATA_BASE_HISTORY>, IRevenueCFDataBaseHistoryRepo
    {
        public RevenueCFDataBaseHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
