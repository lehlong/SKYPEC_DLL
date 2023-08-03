using SMO.Core.Entities.MD;

using System.Collections.Generic;

namespace SMO.Service.Class
{
    public class CostCFReviewViewModel : BaseReviewViewModel
    {
        public CostCFReviewViewModel()
        {
            Elements = new List<CostCFElementReview>();
        }
        public IList<CostCFElementReview> Elements { get; set; }
    }

    public class CostCFElementReview : T_MD_COST_CF_ELEMENT
    {
        public CostCFElementReview()
        {

        }
        public CostCFElementReview(T_MD_COST_CF_ELEMENT element, bool? status, int comments, int commentsInOrg)
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