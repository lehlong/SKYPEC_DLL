using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class UserReviewRepo : GenericRepository<T_MD_USER_REVIEW>, IUserReviewRepo
    {
        public UserReviewRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }

        public override IList<T_MD_USER_REVIEW> Search(T_MD_USER_REVIEW objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            query = query.Where(x => x.TIME_YEAR == objFilter.TIME_YEAR);

            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
