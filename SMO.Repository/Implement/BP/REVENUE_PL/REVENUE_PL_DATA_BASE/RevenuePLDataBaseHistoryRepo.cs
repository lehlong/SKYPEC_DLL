using SMO.Core.Entities.BP.REVENUE_PL.REVENUE_PL_DATA_BASE;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.REVENUE_PL.REVENUE_PL_DATA_BASE;

namespace SMO.Repository.Implement.BP.REVENUE_PL.REVENUE_PL_DATA_BASE
{
    public class RevenuePLDataBaseHistoryRepo : GenericRepository<T_BP_REVENUE_PL_DATA_BASE_HISTORY>, IRevenuePLDataBaseHistoryRepo
    {
        public RevenuePLDataBaseHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
