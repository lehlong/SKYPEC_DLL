using SMO.Core.Entities.BP.COST_CF;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.COST_CF;

namespace SMO.Repository.Implement.BP.COST_CF
{
    public class CostCFReviewRepo : GenericRepository<T_BP_COST_CF_REVIEW>, ICostCFReviewRepo
    {
        public CostCFReviewRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
