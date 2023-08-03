using SMO.Core.Common;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Common
{
    public class GenericTemplateDetailRepository<T, TElement, TCenter> : GenericRepository<T>
        where T : BaseTemplateDetail<TElement, TCenter>
        where TElement : CoreElement
        where TCenter : CoreCenter
    {
        public GenericTemplateDetailRepository(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }

        public override IList<T> Search(T objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.CENTER_CODE))
            {
                query = query.Where(x => x.CENTER_CODE.Equals(objFilter.CENTER_CODE));
            }
            if (!string.IsNullOrWhiteSpace(objFilter.TEMPLATE_CODE))
            {
                query = query.Where(x => x.TEMPLATE_CODE.Equals(objFilter.TEMPLATE_CODE));
            }

            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
