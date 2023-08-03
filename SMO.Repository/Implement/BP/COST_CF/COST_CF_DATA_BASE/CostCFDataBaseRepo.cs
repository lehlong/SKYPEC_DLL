using SMO.Core.Entities.BP.COST_CF.COST_CF_DATA_BASE;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.COST_CF.COST_CF_DATA_BASE;

namespace SMO.Repository.Implement.BP.COST_CF.COST_CF_DATA_BASE
{
    public class CostCFDataBaseRepo : GenericRepository<T_BP_COST_CF_DATA_BASE>, ICostCFDataBaseRepo
    {
        public CostCFDataBaseRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
