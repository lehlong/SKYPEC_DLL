using SMO.Core.Entities.BP;

namespace SMO.Repository.Common
{
    public class GenericBPRepository<TEntity> : GenericRepository<TEntity>, IGenericBPRepository<TEntity> where TEntity : T_BP_BASE
    {
        public GenericBPRepository(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
