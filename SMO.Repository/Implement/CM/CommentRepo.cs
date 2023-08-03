using NHibernate.Linq;

using SMO.Core.Entities;
using SMO.Core.Entities.BP.COST_CF;
using SMO.Core.Entities.BP.COST_PL;
using SMO.Core.Entities.BP.REVENUE_CF;
using SMO.Core.Entities.BP.REVENUE_PL;
using SMO.Core.Entities.CM;
using SMO.Repository.Common;
using SMO.Repository.Interface.CM;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.CM
{
    public class CommentRepo : GenericRepository<T_CM_COMMENT>, ICommentRepo
    {
        public CommentRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
        public List<T_CM_COMMENT> GetCommentsOfDocument(string referenceId)
        {
            var query = Queryable();
            query = query.Where(x => x.REFRENCE_ID == referenceId).OrderByDescending(x => x.CREATE_DATE);
            query = query.Fetch(x => x.USER_CREATE);
            return query.ToList();
        }

        public IList<T_CM_COMMENT> GetCommentsBP(T_CM_HEADER_BP_COMMENT header)
        {
            var query = NHibernateSession.QueryOver<T_CM_HEADER_BP_COMMENT>();
            query = query
                .Where(x => x.ORG_CODE == header.ORG_CODE)
                .Where(x => x.YEAR == header.YEAR)
                .Where(x => x.OBJECT_TYPE == header.OBJECT_TYPE)
                .Where(x => x.BUDGET_TYPE == header.BUDGET_TYPE)
                .Where(x => x.ELEMENT_TYPE == header.ELEMENT_TYPE)
                .Where(x => x.REFERENCE_CODE == header.REFERENCE_CODE)
                .OrderBy(x => x.CREATE_DATE).Desc;
            query = query.Fetch(x => x.USER_CREATE).Eager;
            var headers = query.List();
            var lstComments = new List<T_CM_COMMENT>();
            foreach (var h in headers)
            {
                lstComments.AddRange(GetCommentsOfDocument(h.PKID));
            }

            return lstComments;
        }

        public IList<T_CM_COMMENT> GetCommentsCostPLReview(T_BP_COST_PL_REVIEW_COMMENT header)
        {
            var query = NHibernateSession.QueryOver<T_BP_COST_PL_REVIEW_COMMENT>();
            query = query
                .Where(x => x.ORG_CODE == header.ORG_CODE)
                .Where(x => x.TIME_YEAR == header.TIME_YEAR)
                .Where(x => x.ON_ORG_CODE == header.ON_ORG_CODE)
                .Where(x => x.COST_PL_ELEMENT_CODE == header.COST_PL_ELEMENT_CODE)
                .OrderBy(x => x.CREATE_DATE).Desc;
            query = query.Fetch(x => x.USER_CREATE).Eager;
            var headers = query.List();
            var lstComments = new List<T_CM_COMMENT>();
            foreach (var h in headers)
            {
                lstComments.AddRange(GetCommentsOfDocument(h.PKID));
            }

            return lstComments;
        }

        public IList<T_CM_COMMENT> GetCommentsCostCFReview(T_BP_COST_CF_REVIEW_COMMENT header)
        {
            var query = NHibernateSession.QueryOver<T_BP_COST_CF_REVIEW_COMMENT>();
            query = query
                .Where(x => x.ORG_CODE == header.ORG_CODE)
                .Where(x => x.TIME_YEAR == header.TIME_YEAR)
                .Where(x => x.ON_ORG_CODE == header.ON_ORG_CODE)
                .Where(x => x.COST_CF_ELEMENT_CODE == header.COST_CF_ELEMENT_CODE)
                .OrderBy(x => x.CREATE_DATE).Desc;
            query = query.Fetch(x => x.USER_CREATE).Eager;
            var headers = query.List();
            var lstComments = new List<T_CM_COMMENT>();
            foreach (var h in headers)
            {
                lstComments.AddRange(GetCommentsOfDocument(h.PKID));
            }

            return lstComments;
        }

        public IList<T_CM_COMMENT> GetCommentsRevenuePLReview(T_BP_REVENUE_PL_REVIEW_COMMENT header)
        {
            var query = NHibernateSession.QueryOver<T_BP_REVENUE_PL_REVIEW_COMMENT>();
            query = query
                .Where(x => x.ORG_CODE == header.ORG_CODE)
                .Where(x => x.TIME_YEAR == header.TIME_YEAR)
                .Where(x => x.ON_ORG_CODE == header.ON_ORG_CODE)
                .Where(x => x.REVENUE_PL_ELEMENT_CODE == header.REVENUE_PL_ELEMENT_CODE)
                .OrderBy(x => x.CREATE_DATE).Desc;
            query = query.Fetch(x => x.USER_CREATE).Eager;
            var headers = query.List();
            var lstComments = new List<T_CM_COMMENT>();
            foreach (var h in headers)
            {
                lstComments.AddRange(GetCommentsOfDocument(h.PKID));
            }

            return lstComments;
        }

        public IList<T_CM_COMMENT> GetCommentsRevenueCFReview(T_BP_REVENUE_CF_REVIEW_COMMENT header)
        {
            var query = NHibernateSession.QueryOver<T_BP_REVENUE_CF_REVIEW_COMMENT>();
            query = query
                .Where(x => x.ORG_CODE == header.ORG_CODE)
                .Where(x => x.TIME_YEAR == header.TIME_YEAR)
                .Where(x => x.ON_ORG_CODE == header.ON_ORG_CODE)
                .Where(x => x.REVENUE_CF_ELEMENT_CODE == header.REVENUE_CF_ELEMENT_CODE)
                .OrderBy(x => x.CREATE_DATE).Desc;
            query = query.Fetch(x => x.USER_CREATE).Eager;
            var headers = query.List();
            var lstComments = new List<T_CM_COMMENT>();
            foreach (var h in headers)
            {
                lstComments.AddRange(GetCommentsOfDocument(h.PKID));
            }

            return lstComments;
        }


    }
}
