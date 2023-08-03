using NHibernate.Criterion;

using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class TemplateRepo : GenericRepository<T_MD_TEMPLATE>, ITemplateRepo
    {
        public TemplateRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }

        public override IList<T_MD_TEMPLATE> Search(T_MD_TEMPLATE objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = NHibernateSession.QueryOver<T_MD_TEMPLATE>();

            if (!string.IsNullOrWhiteSpace(objFilter.CODE))
            {
                query = query.Where(x => x.CODE.IsLike($"%{objFilter.CODE}%") || x.NAME.IsLike($"%{objFilter.CODE}%"));
            }

            if (!string.IsNullOrWhiteSpace(objFilter.ORG_CODE))
            {
                query = query.Where(x => x.ORG_CODE == objFilter.ORG_CODE);
            }
            if (!string.IsNullOrWhiteSpace(objFilter.ELEMENT_TYPE))
            {
                query = query.Where(x => x.ELEMENT_TYPE == objFilter.ELEMENT_TYPE);
            }
            if (!string.IsNullOrWhiteSpace(objFilter.BUDGET_TYPE))
            {
                query = query.Where(x => x.BUDGET_TYPE == objFilter.BUDGET_TYPE);
            }
            if (!string.IsNullOrWhiteSpace(objFilter.OBJECT_TYPE))
            {
                query = query.Where(x => x.OBJECT_TYPE == objFilter.OBJECT_TYPE);
            }


            query = query.OrderBy(x => x.ORG_CODE).Asc.ThenBy(x => x.CODE).Asc;
            query = query.Fetch(x => x.Organize).Eager;
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
