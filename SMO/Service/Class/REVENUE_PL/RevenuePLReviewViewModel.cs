using SMO.Core.Entities.MD;

using System.Collections.Generic;

namespace SMO.Service.Class
{
    public class RevenuePLReviewViewModel : BaseReviewViewModel
    {
        public RevenuePLReviewViewModel()
        {
            Elements = new List<RevenuePLElementReview>();
        }
        public IList<RevenuePLElementReview> Elements { get; set; }
    }

    public class RevenuePLElementReview : T_MD_REVENUE_PL_ELEMENT
    {
        public RevenuePLElementReview()
        {

        }
        public RevenuePLElementReview(T_MD_REVENUE_PL_ELEMENT element, bool? status, int comments, int commentsInOrg)
        {
            Status = status;
            CENTER_CODE = CENTER_CODE;
            CODE = element.CODE;
            Values = element.Values;
            IS_GROUP = element.IS_GROUP;
            DESCRIPTION = element.DESCRIPTION;
            NAME = element.NAME;
            LEVEL = element.LEVEL;

            IsChildren = element.IsChildren;
            Comments = comments;
            CommentsInOrg = commentsInOrg;
        }
        public int Comments { get; private set; }
        public int CommentsInOrg { get; private set; }
        public bool? Status { get; set; }
    }
}