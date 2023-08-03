using SMO.Core.Common;

using System.Collections.Generic;

namespace SMO.Service.Class.Base
{
    public interface IElementReviewCenterBase : ICoreElement
    {
        int Comments { get; set; }
        int CommentsInOrg { get; set; }
        IList<string> Failure { get; set; }
        IList<string> NotReviewed { get; set; }
        bool? Status { get; set; }
        IList<string> Success { get; set; }
    }
}