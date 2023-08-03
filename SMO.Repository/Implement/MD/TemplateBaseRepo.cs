using NHibernate.Transform;

using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class TemplateBaseRepo : GenericRepository<T_BP_TEMPLATE_BASE>, ITemplateBaseRepo
    {
        public TemplateBaseRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_BP_TEMPLATE_BASE> Search(T_BP_TEMPLATE_BASE objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = NHibernateSession.QueryOver<T_BP_TEMPLATE_BASE>();

            if (!string.IsNullOrWhiteSpace(objFilter.TEMPLATE_CODE))
            {
                query = query.Where(x => x.TEMPLATE_CODE == objFilter.TEMPLATE_CODE);
            }

            if (objFilter.TIME_YEAR != 0)
            {
                query = query.Where(x => x.TIME_YEAR == objFilter.TIME_YEAR);
            }

            query = query.Fetch(x => x.FileUpload).Eager;

            query = query.TransformUsing(Transformers.DistinctRootEntity);
            total = 0;
            return query.List().ToList();
        }
    }
}
