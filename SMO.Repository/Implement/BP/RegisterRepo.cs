using SMO.Core.Entities.BP;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class RegisterRepo : GenericRepository<T_BP_REGISTER>, IRegisterRepo
    {
        public RegisterRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
