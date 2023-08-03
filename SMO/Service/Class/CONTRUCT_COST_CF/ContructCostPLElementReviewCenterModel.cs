using SMO.Core.Common;
using SMO.Service.Class.Base;
using SMO.Service.Class.CONTRUCT_COST_CF;

using System.Collections.Generic;

namespace SMO.Service.Class
{
    public class ContructCostCFReviewCenterViewModel : ReviewCenterViewModelBase<IContructCostCFElementReviewCenter>, IReviewCenterViewModelBase<IContructCostCFElementReviewCenter>
    {
    }
    public class ContructCostCFElementReviewCenter : ElementReviewCenterBase, IContructCostCFElementReviewCenter
    {
        public ContructCostCFElementReviewCenter(CoreElement element, IList<string> success, IList<string> failure, IList<string> notReviewed, bool? status, int comments, int commentsInOrg) : base(element, success, failure, notReviewed, status, comments, commentsInOrg)
        {
        }
    }
}
