using SMO.Core.Entities.BP;
using SMO.Repository.Common;
using SMO.Repository.Interface.BP;

namespace SMO.Repository.Implement.BP
{
    public class TypeRepo : GenericRepository<T_BP_TYPE>, ITypeRepo
    {
        public TypeRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
