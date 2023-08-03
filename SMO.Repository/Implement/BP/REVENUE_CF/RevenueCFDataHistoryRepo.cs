using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class RevenueCFDataHistoryRepo : GenericRepository<T_BP_REVENUE_CF_DATA_HISTORY>, IRevenueCFDataHistoryRepo
    {
        public RevenueCFDataHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
