using SMO.Core.Entities.BP.COST_PL;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.COST_PL;

namespace SMO.Repository.Implement.BP.COST_PL
{
    public class CostPLReviewHistoryRepo : GenericRepository<T_BP_COST_PL_REVIEW_HISTORY>, ICostPLReviewHistoryRepo
    {
        public CostPLReviewHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
