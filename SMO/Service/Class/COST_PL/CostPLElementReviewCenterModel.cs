using SMO.Core.Common;
using SMO.Service.Class.Base;
using SMO.Service.Class.COST_PL;

using System.Collections.Generic;

namespace SMO.Service.Class
{
    public class CostPLReviewCenterViewModel : ReviewCenterViewModelBase<ICostPLElementReviewCenter>, IReviewCenterViewModelBase<ICostPLElementReviewCenter>
    {
    }
    public class CostPLElementReviewCenter : ElementReviewCenterBase, ICostPLElementReviewCenter
    {
        public CostPLElementReviewCenter(CoreElement element, IList<string> success, IList<string> failure, IList<string> notReviewed, bool? status, int comments, int commentsInOrg) : base(element, success, failure, notReviewed, status, comments, commentsInOrg)
        {
        }
    }
}
