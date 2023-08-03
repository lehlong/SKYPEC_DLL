using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class ContructCostCFVersionRepo : GenericRepository<T_BP_CONTRUCT_COST_CF_VERSION>, IContructCostCFVersionRepo
    {
        public ContructCostCFVersionRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
