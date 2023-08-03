using SMO.Core.Entities.BP.COST_CF;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.COST_CF;

namespace SMO.Repository.Implement.BP.COST_CF
{
    public class CostCFReviewCommentRepo : GenericRepository<T_BP_COST_CF_REVIEW_COMMENT>, ICostCFReviewCommentRepo
    {
        public CostCFReviewCommentRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
