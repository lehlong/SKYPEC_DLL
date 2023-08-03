using SMO.Core.Common;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Common
{
    public class GenericElementRepository<T> : GenericRepository<T> where T : CoreElement
    {
        public GenericElementRepository(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }

        public override IList<T> Search(T objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            query = query.Where(x => x.TIME_YEAR == objFilter.TIME_YEAR);
            query = query.OrderBy(x => x.C_ORDER);
            total = 0;

            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
