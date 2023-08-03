using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.CF;

namespace SMO.Repository.Implement.CF
{
    public class ConfigTableauRepo : GenericRepository<T_CF_TABLEAU>, IConfigTableauRepo
    {
        public ConfigTableauRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
