using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.AD;

namespace SMO.Repository.Implement.AD
{
    public class RoleDetailRepo : GenericRepository<T_AD_ROLE_DETAIL>, IRoleDetailRepo
    {
        public RoleDetailRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
