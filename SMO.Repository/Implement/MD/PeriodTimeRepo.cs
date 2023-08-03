using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class PeriodTimeRepo : GenericRepository<T_MD_PERIOD_TIME>, IPeriodTimeRepo
    {
        public PeriodTimeRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_MD_PERIOD_TIME> Search(T_MD_PERIOD_TIME objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();
            query = query.OrderByDescending(x => x.TIME_YEAR);
            total = 0;
            return query.ToList();
        }
    }
}
