using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.AD;

namespace SMO.Repository.Implement.AD
{
    public class UserApproveRepo : GenericRepository<T_AD_USER_APPROVE>, IUserApproveRepo
    {
        public UserApproveRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
