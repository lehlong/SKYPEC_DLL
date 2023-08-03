using SMO.Core.Entities.WF;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

namespace SMO.Repository.Implement.WF
{
    public class ProcessRepo : GenericRepository<T_WF_PROCESS>, IProcessRepo
    {
        public ProcessRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }


    }
}
