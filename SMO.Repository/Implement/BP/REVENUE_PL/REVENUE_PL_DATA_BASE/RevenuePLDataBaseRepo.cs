using SMO.Core.Entities.BP.REVENUE_PL.REVENUE_PL_DATA_BASE;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.REVENUE_PL.REVENUE_PL_DATA_BASE;

namespace SMO.Repository.Implement.BP.REVENUE_PL.REVENUE_PL_DATA_BASE
{
    public class RevenuePLDataBaseRepo : GenericRepository<T_BP_REVENUE_PL_DATA_BASE>, IRevenuePLDataBaseRepo
    {
        public RevenuePLDataBaseRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
