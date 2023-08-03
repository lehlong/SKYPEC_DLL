using SMO.Core.Common;
using SMO.Service.Class.Base;
using SMO.Service.Class.REVENUE_PL;

using System.Collections.Generic;

namespace SMO.Service.Class
{
    public class RevenuePLReviewCenterViewModel : ReviewCenterViewModelBase<IRevenuePLElementReviewCenter>, IReviewCenterViewModelBase<IRevenuePLElementReviewCenter>
    {
    }
    public class RevenuePLElementReviewCenter : ElementReviewCenterBase, IRevenuePLElementReviewCenter
    {
        public RevenuePLElementReviewCenter(CoreElement element, IList<string> success, IList<string> failure, IList<string> notReviewed, bool? status, int comments, int commentsInOrg) : base(element, success, failure, notReviewed, status, comments, commentsInOrg)
        {
        }
    }
}
