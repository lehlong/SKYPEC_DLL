using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class InternalOrderRepo : GenericCenterRepository<T_MD_INTERNAL_ORDER>, IInternalOrderRepo
    {
        public InternalOrderRepo(NHUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override IList<T_MD_INTERNAL_ORDER> Search(T_MD_INTERNAL_ORDER objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.CODE))
            {
                query = query.Where(x => x.CODE.Contains(objFilter.CODE) ||
                    x.PROJECT_CODE.Contains(objFilter.CODE) ||
                    x.PROJECT_NAME.Contains(objFilter.CODE) ||
                    x.BLOCK_CODE.Contains(objFilter.CODE) ||
                    x.BLOCK_NAME.Contains(objFilter.CODE) ||
                    x.IO_LEVEL1_NAME.Contains(objFilter.CODE) ||
                    x.IO_LEVEL1_CODE.Contains(objFilter.CODE) ||
                    x.IO_LEVEL2_CODE.Contains(objFilter.CODE) ||
                    x.IO_LEVEL2_NAME.Contains(objFilter.CODE)
                );
            }

            query = query.OrderByDescending(x => x.PROJECT_CODE).ThenByDescending(x => x.BLOCK_CODE).ThenByDescending(x => x.IO_LEVEL1_CODE).ThenByDescending(x => x.IO_LEVEL2_CODE).ThenBy(x => x.CODE);
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
