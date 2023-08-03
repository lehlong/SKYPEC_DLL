using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class OtherCostCFVersionRepo : GenericRepository<T_BP_OTHER_COST_CF_VERSION>, IOtherCostCFVersionRepo
    {
        public OtherCostCFVersionRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
