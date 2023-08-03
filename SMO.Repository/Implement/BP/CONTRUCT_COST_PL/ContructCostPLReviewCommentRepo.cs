using SMO.Core.Entities.BP.COST_PL;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.COST_PL;

namespace SMO.Repository.Implement.BP.COST_PL
{
    public class ContructCostPLReviewCommentRepo : GenericRepository<T_BP_CONTRUCT_COST_PL_REVIEW_COMMENT>, IContructCostPLReviewCommentRepo
    {
        public ContructCostPLReviewCommentRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
