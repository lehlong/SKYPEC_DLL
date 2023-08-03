using SMO.Core.Entities.BP.OTHER_COST_CF.OTHER_COST_CF_DATA_BASE;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP.OTHER_COST_CF.OTHER_COST_CF_DATA_BASE;

namespace SMO.Repository.Implement.BP.OTHER_COST_CF.OTHER_COST_CF_DATA_BASE
{
    public class OtherCostCFDataBaseRepo : GenericRepository<T_BP_OTHER_COST_CF_DATA_BASE>, IOtherCostCFDataBaseRepo
    {
        public OtherCostCFDataBaseRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
