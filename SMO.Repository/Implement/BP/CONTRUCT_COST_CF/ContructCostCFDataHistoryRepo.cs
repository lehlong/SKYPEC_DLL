using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class ContructCostCFDataHistoryRepo : GenericRepository<T_BP_CONTRUCT_COST_CF_DATA_HISTORY>, IContructCostCFDataHistoryRepo
    {
        public ContructCostCFDataHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
