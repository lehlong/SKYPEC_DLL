using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class SAPMdExpenseRepo : GenericRepository<T_SAP_MD_EXPENSE>, ISAPMdExpenseRepo
    {
        public SAPMdExpenseRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }

        public override IList<T_SAP_MD_EXPENSE> Search(T_SAP_MD_EXPENSE objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.CODE))
            {
                query = query.Where(x => x.CODE.Contains(objFilter.CODE) || x.NAME.Contains(objFilter.CODE));
            }

            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
