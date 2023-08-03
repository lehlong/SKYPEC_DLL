using SMO.Core.Entities.BP.COST_CF;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.COST_CF;

namespace SMO.Repository.Implement.BP.COST_CF
{
    public class ContructCostCFReviewRepo : GenericRepository<T_BP_CONTRUCT_COST_CF_REVIEW>, IContructCostCFReviewRepo
    {
        public ContructCostCFReviewRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
