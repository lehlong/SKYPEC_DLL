using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

namespace SMO.Repository.Implement.MD
{
    public class SAPCostCFElementRepo : GenericRepository<T_SAP_MD_COST_CF_ELEMENT>, ISAPCostCFElementRepo
    {
        public SAPCostCFElementRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
