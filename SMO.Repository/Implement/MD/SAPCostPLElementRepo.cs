using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

namespace SMO.Repository.Implement.MD
{
    public class SAPCostPLElementRepo : GenericRepository<T_SAP_MD_COST_PL_ELEMENT>, ISAPCostPLElementRepo
    {
        public SAPCostPLElementRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
