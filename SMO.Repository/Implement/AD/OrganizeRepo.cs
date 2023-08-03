using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.AD;

namespace SMO.Repository.Implement.AD
{
    public class OrganizeRepo : GenericRepository<T_AD_ORGANIZE>, IOrganizeRepo
    {
        public OrganizeRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
