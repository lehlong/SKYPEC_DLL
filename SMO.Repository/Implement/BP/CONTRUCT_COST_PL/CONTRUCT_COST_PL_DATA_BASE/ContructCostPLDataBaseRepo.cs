using SMO.Core.Entities.BP.CONTRUCT_COST_PL.CONTRUCT_COST_PL_DATA_BASE;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.CONTRUCT_COST_PL.CONTRUCT_COST_PL_DATA_BASE;

namespace SMO.Repository.Implement.BP.CONTRUCT_COST_PL.CONTRUCT_COST_PL_DATA_BASE
{
    public class ContructCostPLDataBaseRepo : GenericRepository<T_BP_CONTRUCT_COST_PL_DATA_BASE>, IContructCostPLDataBaseRepo
    {
        public ContructCostPLDataBaseRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
