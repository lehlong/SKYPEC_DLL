using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class RevenueCFHistoryRepo : GenericRepository<T_BP_REVENUE_CF_HISTORY>, IRevenueCFHistoryRepo
    {
        public RevenueCFHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
