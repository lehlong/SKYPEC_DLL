using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class CostCFVersionRepo : GenericRepository<T_BP_COST_CF_VERSION>, ICostCFVersionRepo
    {
        public CostCFVersionRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
