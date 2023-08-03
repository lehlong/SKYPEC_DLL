using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class ContructCostPLHistoryRepo : GenericRepository<T_BP_CONTRUCT_COST_PL_HISTORY>, IContructCostPLHistoryRepo
    {
        public ContructCostPLHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
