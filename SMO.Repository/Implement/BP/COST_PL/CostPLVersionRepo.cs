using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class CostPLVersionRepo : GenericRepository<T_BP_COST_PL_VERSION>, ICostPLVersionRepo
    {
        public CostPLVersionRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
