using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class PreventiveRepo : GenericRepository<T_MD_PREVENTIVE>, IPreventiveRepo
    {
        public PreventiveRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }

        public override IList<T_MD_PREVENTIVE> Search(T_MD_PREVENTIVE objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.ORG_CODE))
            {
                query = query.Where(x => x.CostCenter.NAME.ToLower().Contains(objFilter.ORG_CODE.ToLower()));
            }
            if (objFilter.TIME_YEAR > 0)
            {
                query = query.Where(x => x.TIME_YEAR == objFilter.TIME_YEAR);
            }

            query = query.OrderByDescending(x => x.PERCENTAGE);
            return base.Paging(query, pageSize, pageIndex, out total);
        }
    }
}
