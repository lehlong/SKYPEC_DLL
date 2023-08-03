using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.AD;

namespace SMO.Repository.Implement.AD
{
    public class UserOrgRepo : GenericRepository<T_AD_USER_ORG>, IUserOrgRepo
    {
        public UserOrgRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
