using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class OtherCostPLDataHistoryRepo : GenericRepository<T_BP_OTHER_COST_PL_DATA_HISTORY>, IOtherCostPLDataHistoryRepo
    {
        public OtherCostPLDataHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
