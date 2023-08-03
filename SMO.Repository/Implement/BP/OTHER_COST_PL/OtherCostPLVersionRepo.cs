using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class OtherCostPLVersionRepo : GenericRepository<T_BP_OTHER_COST_PL_VERSION>, IOtherCostPLVersionRepo
    {
        public OtherCostPLVersionRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
