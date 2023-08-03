using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class RevenueCFVersionRepo : GenericRepository<T_BP_REVENUE_CF_VERSION>, IRevenueCFVersionRepo
    {
        public RevenueCFVersionRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
