using SMO.Core.Entities.BP.REVENUE_PL;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.REVENUE_PL;

namespace SMO.Repository.Implement.BP.REVENUE_PL
{
    public class RevenuePLReviewHistoryRepo : GenericRepository<T_BP_REVENUE_PL_REVIEW_HISTORY>, IRevenuePLReviewHistoryRepo
    {
        public RevenuePLReviewHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
