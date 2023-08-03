using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class CompanyRepo : GenericRepository<T_MD_COMPANY>, ICompanyRepo
    {
        public CompanyRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_MD_COMPANY> Search(T_MD_COMPANY objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.CODE))
            {
                query = query.Where(x => x.CODE.ToLower().Contains(objFilter.CODE.ToLower()) || x.NAME.ToLower().Contains(objFilter.CODE.ToLower()));
            }

            total = 0;
            query = query.OrderByDescending(x => x.CODE);
            return query.ToList();
        }
    }
}
