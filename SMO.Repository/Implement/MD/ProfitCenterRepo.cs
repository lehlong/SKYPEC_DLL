using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class ProfitCenterRepo : GenericCenterRepository<T_MD_PROFIT_CENTER>, IProfitCenterRepo
    {
        public ProfitCenterRepo(NHUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override IList<T_MD_PROFIT_CENTER> Search(T_MD_PROFIT_CENTER objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.CODE))
            {
                query = query.Where(x => x.CODE.Contains(objFilter.CODE) || x.NAME.Contains(objFilter.NAME));
            }

            if (!string.IsNullOrWhiteSpace(objFilter.COMPANY_CODE))
            {
                query = query.Where(x => x.COMPANY_CODE == objFilter.COMPANY_CODE);
            }

            if (!string.IsNullOrWhiteSpace(objFilter.PROJECT_CODE))
            {
                query = query.Where(x => x.PROJECT_CODE == objFilter.PROJECT_CODE);
            }

            query = query.OrderBy(x => x.COMPANY_CODE).ThenBy(x => x.PROJECT_CODE).ThenBy(x => x.CODE);
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
