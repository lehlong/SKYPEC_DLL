using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

namespace SMO.Repository.Implement.MD
{
    public class SAPRevenuePLElementRepo : GenericRepository<T_SAP_MD_REVENUE_PL_ELEMENT>, ISAPRevenuePLElementRepo
    {
        public SAPRevenuePLElementRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
