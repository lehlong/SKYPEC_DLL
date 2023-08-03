using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

namespace SMO.Repository.Implement.MD
{
    public class CostCenterRepo : GenericCenterRepository<T_MD_COST_CENTER>, ICostCenterRepo
    {
        public CostCenterRepo(NHUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
