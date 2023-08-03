using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class ContructCostPLVersionRepo : GenericRepository<T_BP_CONTRUCT_COST_PL_VERSION>, IContructCostPLVersionRepo
    {
        public ContructCostPLVersionRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
