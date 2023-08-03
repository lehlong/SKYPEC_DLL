using SMO.Core.Common;

namespace SMO.Repository.Common
{
    interface IGenericCenterRepository<T> : IGenericRepository<T> where T : CoreCenter
    {
    }
}
