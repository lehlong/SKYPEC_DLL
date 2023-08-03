using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class OtherCostCFDataHistoryRepo : GenericRepository<T_BP_OTHER_COST_CF_DATA_HISTORY>, IOtherCostCFDataHistoryRepo
    {
        public OtherCostCFDataHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
