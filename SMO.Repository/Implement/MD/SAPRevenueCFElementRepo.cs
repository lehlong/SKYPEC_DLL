using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

namespace SMO.Repository.Implement.MD
{
    public class SAPRevenueCFElementRepo : GenericRepository<T_SAP_MD_REVENUE_CF_ELEMENT>, ISAPRevenueCFElementRepo
    {
        public SAPRevenueCFElementRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
