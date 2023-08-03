using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class FAQQuestionRepo : GenericRepository<T_FAQ_QUESTION>, IFAQQuestionRepo
    {
        public FAQQuestionRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_FAQ_QUESTION> Search(T_FAQ_QUESTION objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
