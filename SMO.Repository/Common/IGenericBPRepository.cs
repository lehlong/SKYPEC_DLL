using SMO.Core.Entities.BP;

namespace SMO.Repository.Common
{
    public interface IGenericBPRepository<TEntity> : IGenericRepository<TEntity> where TEntity : T_BP_BASE
    {

    }
}
