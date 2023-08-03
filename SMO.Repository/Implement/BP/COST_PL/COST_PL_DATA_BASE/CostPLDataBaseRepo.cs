using SMO.Core.Entities.BP.COST_PL.COST_PL_DATA_BASE;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.COST_PL.COST_PL_DATA_BASE;

namespace SMO.Repository.Implement.BP.COST_PL.COST_PL_DATA_BASE
{
    public class CostPLDataBaseRepo : GenericRepository<T_BP_COST_PL_DATA_BASE>, ICostPLDataBaseRepo
    {
        public CostPLDataBaseRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
