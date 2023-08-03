using SMO.Core.Entities.WF;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

namespace SMO.Repository.Implement.WF
{
    public class ActivityRepo : GenericRepository<T_WF_ACTIVITY>, IActivityRepo
    {
        public ActivityRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
