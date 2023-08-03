using SMO.Core.Common;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Common
{
    public class GenericCenterRepository<T> : GenericRepository<T> where T : CoreCenter
    {
        public GenericCenterRepository(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }

        public override IList<T> Search(T objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.CODE))
            {
                query = query.Where(x => x.CODE.Equals(objFilter.CODE));
            }
            if (!string.IsNullOrWhiteSpace(objFilter.NAME))
            {
                query = query.Where(x => x.NAME.ToLower().Contains(objFilter.NAME.ToLower()));
            }

            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
