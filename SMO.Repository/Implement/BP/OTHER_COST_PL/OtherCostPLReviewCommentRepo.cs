using SMO.Core.Entities.BP.COST_PL;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.COST_PL;

namespace SMO.Repository.Implement.BP.COST_PL
{
    public class OtherCostPLReviewCommentRepo : GenericRepository<T_BP_OTHER_COST_PL_REVIEW_COMMENT>, IOtherCostPLReviewCommentRepo
    {
        public OtherCostPLReviewCommentRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
