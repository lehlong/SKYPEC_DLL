using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class CostPLHistoryRepo : GenericRepository<T_BP_COST_PL_HISTORY>, ICostPLHistoryRepo
    {
        public CostPLHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
