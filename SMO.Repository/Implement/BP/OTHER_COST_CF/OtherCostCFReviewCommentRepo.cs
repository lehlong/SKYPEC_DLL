using SMO.Core.Entities.BP.COST_CF;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.COST_CF;

namespace SMO.Repository.Implement.BP.COST_CF
{
    public class OtherCostCFReviewCommentRepo : GenericRepository<T_BP_OTHER_COST_CF_REVIEW_COMMENT>, IOtherCostCFReviewCommentRepo
    {
        public OtherCostCFReviewCommentRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
