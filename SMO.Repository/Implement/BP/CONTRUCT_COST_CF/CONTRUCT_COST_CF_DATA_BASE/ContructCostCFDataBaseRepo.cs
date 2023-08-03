using SMO.Core.Entities.BP.CONTRUCT_COST_CF.CONTRUCT_COST_CF_DATA_BASE;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.CONTRUCT_COST_CF.CONTRUCT_COST_CF_DATA_BASE;

namespace SMO.Repository.Implement.BP.CONTRUCT_COST_CF.CONTRUCT_COST_CF_DATA_BASE
{
    public class ContructCostCFDataBaseRepo : GenericRepository<T_BP_CONTRUCT_COST_CF_DATA_BASE>, IContructCostCFDataBaseRepo
    {
        public ContructCostCFDataBaseRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
