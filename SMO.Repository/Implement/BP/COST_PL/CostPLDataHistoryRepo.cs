using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class CostPLDataHistoryRepo : GenericRepository<T_BP_COST_PL_DATA_HISTORY>, ICostPLDataHistoryRepo
    {
        public CostPLDataHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
