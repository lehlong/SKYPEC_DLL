using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class CostCFDataHistoryRepo : GenericRepository<T_BP_COST_CF_DATA_HISTORY>, ICostCFDataHistoryRepo
    {
        public CostCFDataHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
