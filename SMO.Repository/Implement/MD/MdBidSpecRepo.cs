using NHibernate.Criterion;

using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class MdBidSpecRepo : GenericRepository<T_MD_BID_SPEC>, IMdBidSpecRepo
    {
        public MdBidSpecRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_MD_BID_SPEC> Search(T_MD_BID_SPEC objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = NHibernateSession.QueryOver<T_MD_BID_SPEC>();
            var subQuery = QueryOver.Of<T_MD_BID_SPEC>();

            var queryListDetail = NHibernateSession.QueryOver<T_MD_BID_SPEC_DETAIL>()
                .WithSubquery.WhereProperty(x => x.HEADER_ID).In(
                    subQuery.Select(Projections.Property<T_MD_BID_SPEC>(p => p.PKID))
                ).OrderBy(x => x.C_ORDER).Asc.Future();

            query = query.OrderBy(p => p.C_ORDER).Asc;
            var result = query.List();

            var lstDetail = queryListDetail.ToList();
            foreach (var item in result)
            {
                item.ListDetail = lstDetail.Where(x => x.HEADER_ID == item.PKID).ToList();
            }
            total = 0;
            return result;
        }

        public override T_MD_BID_SPEC Get(object id, dynamic param = null)
        {
            var query = NHibernateSession.QueryOver<T_MD_BID_SPEC>();

            query = query.Where(x => x.PKID == id as string).Fetch(x => x.ListDetail).Eager;

            return query.SingleOrDefault();
        }

    }
}
