using SMO.Core.Common;
using SMO.Service.Class.Base;
using SMO.Service.Class.CONTRUCT_COST_PL;

using System.Collections.Generic;

namespace SMO.Service.Class
{
    public class ContructCostPLReviewCenterViewModel : ReviewCenterViewModelBase<IContructCostPLElementReviewCenter>, IReviewCenterViewModelBase<IContructCostPLElementReviewCenter>
    {
    }
    public class ContructCostPLElementReviewCenter : ElementReviewCenterBase, IContructCostPLElementReviewCenter
    {
        public ContructCostPLElementReviewCenter(CoreElement element, IList<string> success, IList<string> failure, IList<string> notReviewed, bool? status, int comments, int commentsInOrg) : base(element, success, failure, notReviewed, status, comments, commentsInOrg)
        {
        }
    }
}
