using SMO.Core.Entities.WF;
using SMO.Repository.Common;
using SMO.Repository.Interface.WF;

namespace SMO.Repository.Implement.WF
{
    public class ActivityUserRepo : GenericRepository<T_WF_ACTIVITY_USER>, IActivityUserRepo
    {
        public ActivityUserRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
