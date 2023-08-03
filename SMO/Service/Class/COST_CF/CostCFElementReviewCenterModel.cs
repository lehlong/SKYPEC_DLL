using SMO.Core.Common;
using SMO.Service.Class.Base;
using SMO.Service.Class.COST_CF;

using System.Collections.Generic;

namespace SMO.Service.Class
{
    public class CostCFReviewCenterViewModel : ReviewCenterViewModelBase<ICostCFElementReviewCenter>, IReviewCenterViewModelBase<ICostCFElementReviewCenter>
    {
    }
    public class CostCFElementReviewCenter : ElementReviewCenterBase, ICostCFElementReviewCenter
    {
        public CostCFElementReviewCenter(CoreElement element, IList<string> success, IList<string> failure, IList<string> notReviewed, bool? status, int comments, int commentsInOrg) : base(element, success, failure, notReviewed, status, comments, commentsInOrg)
        {
        }
    }
}
