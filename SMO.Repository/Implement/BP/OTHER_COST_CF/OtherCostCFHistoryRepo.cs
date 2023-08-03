using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class OtherCostCFHistoryRepo : GenericRepository<T_BP_OTHER_COST_CF_HISTORY>, IOtherCostCFHistoryRepo
    {
        public OtherCostCFHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
