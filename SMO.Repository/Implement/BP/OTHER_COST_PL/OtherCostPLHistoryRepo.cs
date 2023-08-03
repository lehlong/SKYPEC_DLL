using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class OtherCostPLHistoryRepo : GenericRepository<T_BP_OTHER_COST_PL_HISTORY>, IOtherCostPLHistoryRepo
    {
        public OtherCostPLHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
