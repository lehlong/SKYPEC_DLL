using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class RevenuePLVersionRepo : GenericRepository<T_BP_REVENUE_PL_VERSION>, IRevenuePLVersionRepo
    {
        public RevenuePLVersionRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
