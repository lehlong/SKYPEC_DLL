using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.CM;

namespace SMO.Repository.Implement.CM
{
    public class HistorySmoApiRepo : GenericRepository<T_CM_HISTORY_SMO_API>, IHistorySmoApiRepo
    {
        public HistorySmoApiRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
