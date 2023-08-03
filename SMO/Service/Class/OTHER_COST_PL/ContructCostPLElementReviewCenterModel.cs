using SMO.Core.Common;
using SMO.Service.Class.Base;
using SMO.Service.Class.OTHER_COST_PL;

using System.Collections.Generic;

namespace SMO.Service.Class
{
    public class OtherCostPLReviewCenterViewModel : ReviewCenterViewModelBase<IOtherCostPLElementReviewCenter>, IReviewCenterViewModelBase<IOtherCostPLElementReviewCenter>
    {
    }
    public class OtherCostPLElementReviewCenter : ElementReviewCenterBase, IOtherCostPLElementReviewCenter
    {
        public OtherCostPLElementReviewCenter(CoreElement element, IList<string> success, IList<string> failure, IList<string> notReviewed, bool? status, int comments, int commentsInOrg) : base(element, success, failure, notReviewed, status, comments, commentsInOrg)
        {
        }
    }
}
