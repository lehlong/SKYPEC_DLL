using NHibernate.Linq;
using SMO.Core.Entities;
using SMO.Core.Entities.BP.COST_PL;
using SMO.Core.Entities.MD;
using SMO.Repository.Implement.BP.COST_PL;
using SMO.Repository.Implement.CM;
using SMO.Repository.Implement.MD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SMO.Service.BP.COST_PL
{
    public class OtherCostPLReviewCommentService : GenericService<T_BP_OTHER_COST_PL_REVIEW_COMMENT, OtherCostPLReviewCommentRepo>
    {
        internal void GetHeader(string orgCode, string elementCode, int year, int version, string onOrgCode)
        {
            var header = GetFirstByExpression(
                        x => x.COST_PL_ELEMENT_CODE == elementCode &&
                        x.ORG_CODE == orgCode &&
                        x.ON_ORG_CODE == onOrgCode &&
                        x.TIME_YEAR == year &&
                        x.DATA_VERSION == version);
            if (header == null)
            {
                ObjDetail.PKID = Guid.NewGuid().ToString();
                ObjDetail.COST_PL_ELEMENT_CODE = elementCode;
                ObjDetail.TIME_YEAR = year;
                ObjDetail.ORG_CODE = orgCode;
                ObjDetail.DATA_VERSION = version;
                ObjDetail.ON_ORG_CODE = onOrgCode;
            }
            else
            {
                ObjDetail = header;
            }
        }

        internal void GetComments()
        {
            //Get(ObjDetail.PKID);
            //if (ObjDetail == null)
            //{
            //    return;
            //}
            var query = CurrentRepository.Queryable();
            query = query
                .Where(x => x.ORG_CODE == ObjDetail.ORG_CODE)
                .Where(x => x.TIME_YEAR == ObjDetail.TIME_YEAR)
                .Where(x => x.ON_ORG_CODE == ObjDetail.ON_ORG_CODE)
                .Where(x => x.COST_PL_ELEMENT_CODE == ObjDetail.COST_PL_ELEMENT_CODE)
                .OrderByDescending(x => x.CREATE_DATE);
            query = query.Fetch(x => x.USER_CREATE).FetchMany(x => x.Comments);
            ObjList = query.ToList();
        }

        public override void Create()
        {
            try
            {
                UnitOfWork.BeginTransaction();
                var content = ObjDetail.CONTENT;
                var header = new T_BP_OTHER_COST_PL_REVIEW_COMMENT();
                var pkId = ObjDetail.PKID;
                header = CurrentRepository.Get(pkId);
                if (header != null)
                {
                    ObjDetail = header;
                    ObjDetail.NUMBER_COMMENTS++;
                    ObjDetail = CurrentRepository.Update(ObjDetail);
                }
                else
                {
                    ObjDetail.NUMBER_COMMENTS = 1;

                    if (ProfileUtilities.User != null)
                    {
                        ObjDetail.CREATE_BY = ProfileUtilities.User.USER_NAME;
                        ObjDetail.CREATE_DATE = CurrentRepository.GetDateDatabase();
                    }
                    ObjDetail = CurrentRepository.Create(ObjDetail);
                }
                var comment = new T_CM_COMMENT
                {
                    CONTENTS = content,
                    REFRENCE_ID = pkId,
                    CODE = Guid.NewGuid().ToString(),
                    CREATE_BY = ProfileUtilities.User?.USER_NAME,
                    CREATE_DATE = CurrentRepository.GetDateDatabase()
                };
                UnitOfWork.Repository<CommentRepo>().Create(comment);

                UnitOfWork.Commit();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                State = false;
                ErrorMessage = e.Message;
            }
        }

        internal string GetComments(int year, string orgCode, string elementCode, string onOrgCode)
        {
            var comments = GetManyByExpression(x =>
                    x.ORG_CODE == orgCode &&
                    x.TIME_YEAR == year &&
                    x.COST_PL_ELEMENT_CODE == elementCode);
            var numberComments = comments.Sum(x => x.NUMBER_COMMENTS);
            var commentsInOrg = comments.Where(x => x.ON_ORG_CODE == onOrgCode)
                .Sum(x => x.NUMBER_COMMENTS);
            if (orgCode == onOrgCode)
            {
                return $"{numberComments} | {commentsInOrg}";
            } else
            {
                return commentsInOrg.ToString();
            }
        }

        /// <summary>
        /// Lấy cây cấu trúc của pl element vào select list
        /// </summary>
        /// <param name="year">Năm của cây khoản mục</param>
        /// <returns></returns>
        internal SelectList GetElements(int year)
        {
            return SelectListUtilities.GetChildElement<T_MD_COST_PL_ELEMENT, CostPLElementRepo>(year);
        }

        /// <summary>
        /// Lấy danh sách những user comment
        /// </summary>
        /// <param name="year"></param>
        /// <param name="version"></param>
        /// <param name="elementCode"></param>
        /// <param name="centerCode"></param>
        /// <returns></returns>
        internal IList<string> GetUsersComment(int year, int? version, string elementCode, string centerCode)
        {
            var query = CurrentRepository.Queryable();
            query = query.Where(x => x.TIME_YEAR == year);
            if (version.HasValue)
            {
                query = query.Where(x => x.DATA_VERSION == version.Value);
            }
            if (!string.IsNullOrEmpty(elementCode))
            {
                query = query.Where(x => x.COST_PL_ELEMENT_CODE == elementCode);
            }
            if (!string.IsNullOrEmpty(centerCode))
            {
                query = query.Where(x => x.ORG_CODE == centerCode);
            }
            return query.ToList()
                .Select(x => x.CREATE_BY)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }
        internal IList<int> GetVersions(int year, string elementCode)
        {
            var query = CurrentRepository.Queryable();
            query = query.Where(x => x.TIME_YEAR == year);
            if (!string.IsNullOrEmpty(elementCode))
            {
                query = query.Where(x => x.COST_PL_ELEMENT_CODE == elementCode);
            }
            return query.ToList()
                .Select(x => x.DATA_VERSION)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }

    }
}
