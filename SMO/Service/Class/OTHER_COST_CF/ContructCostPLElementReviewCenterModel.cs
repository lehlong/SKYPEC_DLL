using SMO.Core.Common;
using SMO.Service.Class.Base;
using SMO.Service.Class.OTHER_COST_CF;

using System.Collections.Generic;

namespace SMO.Service.Class
{
    public class OtherCostCFReviewCenterViewModel : ReviewCenterViewModelBase<IOtherCostCFElementReviewCenter>, IReviewCenterViewModelBase<IOtherCostCFElementReviewCenter>
    {
    }
    public class OtherCostCFElementReviewCenter : ElementReviewCenterBase, IOtherCostCFElementReviewCenter
    {
        public OtherCostCFElementReviewCenter(CoreElement element, IList<string> success, IList<string> failure, IList<string> notReviewed, bool? status, int comments, int commentsInOrg) : base(element, success, failure, notReviewed, status, comments, commentsInOrg)
        {
        }
    }
}
