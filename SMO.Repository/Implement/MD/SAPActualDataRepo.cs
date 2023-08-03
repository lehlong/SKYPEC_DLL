using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class SAPActualDataRepo : GenericRepository<T_SAP_ACTUAL_DATA>, ISAPActualDataRepo
    {
        public SAPActualDataRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }

        public override IList<T_SAP_ACTUAL_DATA> Search(T_SAP_ACTUAL_DATA objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.DOCUMENT_NUMBER))
            {
                query = query.Where(x => x.DOCUMENT_NUMBER.Contains(objFilter.DOCUMENT_NUMBER));
            }

            if (objFilter.POSTING_DATE.HasValue)
            {
                query = query.Where(x => x.POSTING_DATE == objFilter.POSTING_DATE);
            }


            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
