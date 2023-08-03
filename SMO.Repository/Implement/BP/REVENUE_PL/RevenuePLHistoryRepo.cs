using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class RevenuePLHistoryRepo : GenericRepository<T_BP_REVENUE_PL_HISTORY>, IRevenuePLHistoryRepo
    {
        public RevenuePLHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
