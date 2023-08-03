using SMO.Core.Common;
using SMO.Core.Entities.MD;
using SMO.Service.Class.Base;
using SMO.Service.Class.REVENUE_CF;

using System.Collections.Generic;

namespace SMO.Service.Class
{
    public class RevenueCFReviewCenterViewModel : ReviewCenterViewModelBase<IRevenueCFElementReviewCenter>, IReviewCenterViewModelBase<IRevenueCFElementReviewCenter>
    {
    }
    public class RevenueCFElementReviewCenter : ElementReviewCenterBase, IRevenueCFElementReviewCenter
    {
        public RevenueCFElementReviewCenter(CoreElement element, IList<string> success, IList<string> failure, IList<string> notReviewed, bool? status, int comments, int commentsInOrg) : base(element, success, failure, notReviewed, status, comments, commentsInOrg)
        {
        }
    }
}
