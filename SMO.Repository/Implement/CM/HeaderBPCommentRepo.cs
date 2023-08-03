using SMO.Core.Entities.CM;
using SMO.Repository.Common;
using SMO.Repository.Interface.CM;

namespace SMO.Repository.Implement.CM
{
    public class HeaderBPCommentRepo : GenericRepository<T_CM_HEADER_BP_COMMENT>, IHeaderBPComment
    {
        public HeaderBPCommentRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
