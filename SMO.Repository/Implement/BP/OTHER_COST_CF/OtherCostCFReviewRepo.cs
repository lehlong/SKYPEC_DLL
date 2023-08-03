using SMO.Core.Entities.BP.COST_CF;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.COST_CF;

namespace SMO.Repository.Implement.BP.COST_CF
{
    public class OtherCostCFReviewRepo : GenericRepository<T_BP_OTHER_COST_CF_REVIEW>, IOtherCostCFReviewRepo
    {
        public OtherCostCFReviewRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
