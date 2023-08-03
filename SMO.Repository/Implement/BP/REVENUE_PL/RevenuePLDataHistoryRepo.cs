using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class RevenuePLDataHistoryRepo : GenericRepository<T_BP_REVENUE_PL_DATA_HISTORY>, IRevenuePLDataHistoryRepo
    {
        public RevenuePLDataHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
