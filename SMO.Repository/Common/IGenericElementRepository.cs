using SMO.Core.Common;

namespace SMO.Repository.Common
{
    public interface IGenericElementRepository<T> : IGenericRepository<T> where T : CoreElement
    {

    }
}
