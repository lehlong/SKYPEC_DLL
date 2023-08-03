using SMO.Core.Entities.WF;
using SMO.Repository.Common;
using SMO.Repository.Interface.WF;

namespace SMO.Repository.Implement.WF
{
    public class ActivityComRepo : GenericRepository<T_WF_ACTIVITY_COM>, IActivityComRepo
    {
        public ActivityComRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
