using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class FrequentlyAskedRepo : GenericRepository<T_FAQ_FREQUENTLY_ASKED>, IFrequentlyAskedRepo
    {
        public FrequentlyAskedRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_FAQ_FREQUENTLY_ASKED> Search(T_FAQ_FREQUENTLY_ASKED objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
