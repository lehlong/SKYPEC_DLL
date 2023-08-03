using SMO.Core.Entities.MD;

using System.Collections.Generic;

namespace SMO.Service.Class
{
    public class ContructCostCFReviewViewModel : BaseReviewViewModel
    {
        public ContructCostCFReviewViewModel()
        {
            Elements = new List<ContructCostCFElementReview>();
        }
        public IList<ContructCostCFElementReview> Elements { get; set; }
    }

    public class ContructCostCFElementReview : T_MD_COST_CF_ELEMENT
    {
        public ContructCostCFElementReview()
        {

        }
        public ContructCostCFElementReview(T_MD_COST_CF_ELEMENT element, bool? status, int comments, int commentsInOrg)
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