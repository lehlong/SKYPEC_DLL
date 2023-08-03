using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class SAPMdGLAccountRepo : GenericRepository<T_SAP_MD_GLACCOUNT>, ISAPMdGLAccountRepo
    {
        public SAPMdGLAccountRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }

        public override IList<T_SAP_MD_GLACCOUNT> Search(T_SAP_MD_GLACCOUNT objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.CODE))
            {
                query = query.Where(x => x.CODE.Contains(objFilter.CODE) || x.NAME.Contains(objFilter.CODE));
            }

            //query = query.Where(x => string.IsNullOrWhiteSpace(x.PARENT_CODE));

            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
