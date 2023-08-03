
using SMO.Core.Entities.BP;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class PeriodRepo : GenericRepository<T_BP_PERIOD>, IPeriodRepo
    {
        public PeriodRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
