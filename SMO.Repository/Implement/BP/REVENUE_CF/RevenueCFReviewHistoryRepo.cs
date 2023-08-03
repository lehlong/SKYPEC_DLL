using SMO.Core.Entities.BP.REVENUE_CF;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.REVENUE_CF;

namespace SMO.Repository.Implement.BP.REVENUE_CF
{
    public class RevenueCFReviewHistoryRepo : GenericRepository<T_BP_REVENUE_CF_REVIEW_HISTORY>, IRevenueCFReviewHistoryRepo
    {
        public RevenueCFReviewHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
