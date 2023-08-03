using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

namespace SMO.Repository.Implement.MD
{
    public class MdBidSpecDetailRepo : GenericRepository<T_MD_BID_SPEC_DETAIL>, IMdBidSpecDetailRepo
    {
        public MdBidSpecDetailRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
